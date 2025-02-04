using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Messages;
using Newtonsoft.Json;
using QueueSender;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace MessagePortal
{
    public partial class Portal : Form
    {
        // the client that owns the connection and can be used to create senders and receivers
        ServiceBusClient client;

        ServiceBusAdministrationClient adminClient;

        // the processor that reads and processes messages from the queue
        ServiceBusSessionProcessor processor;

        StartSagaService _startSagaService;

        public readonly ConcurrentDictionary<EventKey, EventHandler> EventDictionary;

        public Portal(StartSagaService startSagaService)
        {
            EventDictionary = new ConcurrentDictionary<EventKey, EventHandler>();
            _startSagaService = startSagaService;
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };
            client = new ServiceBusClient("sdc3test.servicebus.windows.net",
                new DefaultAzureCredential(), clientOptions);
            adminClient = new ServiceBusAdministrationClient("sdc3test.servicebus.windows.net", new DefaultAzureCredential());
            // create the options to use for configuring the processor
            var options = new ServiceBusSessionProcessorOptions
            {
                // By default after the message handler returns, the processor will complete the message
                // If I want more fine-grained control over settlement, I can set this to false.
                AutoCompleteMessages = false,

                // I can also allow for processing multiple sessions
                MaxConcurrentSessions = 10,

                // By default or when AutoCompleteMessages is set to true, the processor will complete the message after executing the message handler
                // Set AutoCompleteMessages to false to [settle messages](https://learn.microsoft.com/azure/service-bus-messaging/message-transfers-locks-settlement#peeklock) on your own.
                // In both cases, if the message handler throws an exception without settling the message, the processor will abandon the message.
                MaxConcurrentCallsPerSession = 1,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                PrefetchCount = 1,
                MaxAutoLockRenewalDuration = Timeout.InfiniteTimeSpan,
                //SessionIdleTimeout = Timeout.InfiniteTimeSpan
            };
            // create a session processor that we can use to process the messages
            processor = client.CreateSessionProcessor("testqueue_in", options);


            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;
            processor.SessionInitializingAsync += SessionInitializingHandler;
            processor.SessionClosingAsync += SessionClosingHandler;

            InitializeComponent();
        }

        private async void startManual_Click(object sender, EventArgs e)
        {
            if (processor.IsProcessing)
            {
                await processor.StopProcessingAsync();

                startManual.Text = "Start";
                if (InProcessPanel.InvokeRequired)
                {
                    InProcessPanel.Invoke(new MethodInvoker(InProcessPanel.Controls.Clear));
                }
                else
                {
                    InProcessPanel.Controls.Clear();
                }
            }
            else
            {
                await processor.StartProcessingAsync();
                startManual.Text = "Stop";
            }
            QueueRuntimeProperties queue = await adminClient.GetQueueRuntimePropertiesAsync("testqueue_in");
            txtMessageCount.Text = queue.ActiveMessageCount.ToString();
        }

       async Task MessageHandler(ProcessSessionMessageEventArgs args)
        {
            await args.RenewSessionLockAsync();
            Color color = GetColorBySessionId(args);
            string body = args.Message.Body.ToString();
            List<Change>? items = JsonConvert.DeserializeObject<List<Change>>(body);
            if (items is null)
            {
                return;
            }
            Console.WriteLine($"Received: {items[0].Name}");
            var card = new MessageCard() { Text = items[0].Name + Environment.NewLine + "# Changes: " + items.Count, Color = color, SessionId = args.SessionId };

            EventHandler handler = (sender, e) => OnMessageCardCompleted(sender, e, card, args);
            card.MessageCardCompleted += handler;

            if (InProcessPanel.InvokeRequired)
            {
                InProcessPanel.Invoke(new MethodInvoker(delegate { InProcessPanel.Controls.Add(card); }));
            }
            else
            {
                InProcessPanel.Controls.Add(card);
            }
        }

        private static Color GetColorBySessionId(ProcessSessionMessageEventArgs args)
        {
            var color = Color.Black;
            switch (args.SessionId)
            {
                case "0":
                    color = Color.Red;
                    break;
                case "1":
                    color = Color.Green;
                    break;
                case "2":
                    color = Color.Blue;
                    break;
                case "3":
                    color = Color.Yellow;
                    break;
                case "4":
                    color = Color.Purple;
                    break;
                case "5":
                    color = Color.Orange;
                    break;
                case "6":
                    color = Color.Black;
                    break;
                case "7":
                    color = Color.Cyan;
                    break;
                case "8":
                    color = Color.Aquamarine;
                    break;
                case "9":
                    color = Color.Brown;
                    break;
                default:
                    break;
            }
            return color;
        }

        private async void OnMessageCardCompleted(object? sender, EventArgs e, MessageCard card, ProcessSessionMessageEventArgs args)
        {
            await args.CompleteMessageAsync(args.Message);
            args.ReleaseSession();
            card.IsEnabled = false;
            if (InProcessPanel.InvokeRequired)
            {
                InProcessPanel.Invoke(new MethodInvoker(delegate { InProcessPanel.Controls.Remove(card); }));
            }
            else
            {
                InProcessPanel.Controls.Remove(card);
            }
            if (ProcessedMessagesPanel.InvokeRequired)
            {
                ProcessedMessagesPanel.Invoke(new MethodInvoker(delegate { ProcessedMessagesPanel.Controls.Add(card); }));
            }
            else
            {
                ProcessedMessagesPanel.Controls.Add(card);
            }
            QueueRuntimeProperties queue = await adminClient.GetQueueRuntimePropertiesAsync("testqueue_in");
            txtMessageCount.Text = queue.ActiveMessageCount.ToString();
        }

        // handle any errors when receiving messages
        Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        async Task SessionInitializingHandler(ProcessSessionEventArgs args)
        {
            await args.SetSessionStateAsync(new BinaryData($"Session start for {args.SessionId}."));
        }

        async Task SessionClosingHandler(ProcessSessionEventArgs args)
        {
            var card = InProcessPanel.Controls.OfType<MessageCard>().SingleOrDefault(m => m.SessionId == args.SessionId);
            if (card is not null)
            {
                if (InProcessPanel.InvokeRequired)
                {
                    InProcessPanel.Invoke(new MethodInvoker(delegate { InProcessPanel.Controls.Remove(card); }));
                }
                else
                {
                    InProcessPanel.Controls.Remove(card);
                }
            }

            // We may want to clear the session state when no more messages are available for the session or when some known terminal message
            // has been received. This is entirely dependent on the application scenario.
            BinaryData sessionState = await args.GetSessionStateAsync();
            if (sessionState.ToString() ==
                "Some state that indicates the final message was received for the session")
            {
                await args.SetSessionStateAsync(null);
            }
        }

        //async Task MessageHandler(ProcessSessionMessageEventArgs args)
        //{
        //    string body = args.Message.Body.ToString();
        //    List<Change>? items = JsonConvert.DeserializeObject<List<Change>>(body);
        //    if (items is null)
        //    {
        //        return;
        //    }
        //    Console.WriteLine($"Received: {items[0].Name}");
        //    var cmd = new StartProcessChangesSaga()
        //    { Changes = items, PersoonId = items[0].PersoonId, SubAffiliateKey = items[0].SubAffiliateKey, BatchNumber = items[0].BatchNumber };
        //    await _startSagaService.SendMessage(cmd);

        //    EventHandler handler = (sender, e) => OnSagaStarted(sender, e, items[0].PersoonId, args);
        //    if (!EventDictionary.TryAdd(new EventKey(items[0].PersoonId, items[0].BatchNumber), handler))
        //    {
        //        Console.WriteLine($"Failed to add event to dictionary: {items[0].Name} - {items[0].BatchNumber}");
        //    }
        //}

        private async  void OnSagaStarted(object sender, EventArgs e, Guid persoonId, ProcessSessionMessageEventArgs args)
        {
            await args.CompleteMessageAsync(args.Message);
            QueueRuntimeProperties queue = await adminClient.GetQueueRuntimePropertiesAsync("testqueue_in");
            
            if (txtMessageCount.InvokeRequired)
            {
                txtMessageCount.Invoke(new MethodInvoker(delegate { txtMessageCount.Text = queue.ActiveMessageCount.ToString(); }));
            }
            else
            {
                txtMessageCount.Text = queue.ActiveMessageCount.ToString();
            }
        }
    }
}
