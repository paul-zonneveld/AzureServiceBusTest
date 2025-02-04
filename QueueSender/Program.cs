using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using QueueSender;

// name of your Service Bus queue
// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// number of messages to be sent to the queue
const int numOfMessages = 300;

// The Service Bus client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when messages are being published or read
// regularly.
//
// Set the transport type to AmqpWebSockets so that the ServiceBusClient uses the port 443. 
// If you use the default AmqpTcp, ensure that ports 5671 and 5672 are open.
var clientOptions = new ServiceBusClientOptions
{
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
client = new ServiceBusClient(
    "sdc3test.servicebus.windows.net",
    new DefaultAzureCredential(),
    clientOptions);
sender = client.CreateSender("testqueue_in");

// create a batch 
using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

var testData = getTestData();

foreach (var item in testData)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage(JsonConvert.SerializeObject(item.Value)) { SessionId = item.Value[0].SubAffiliateKey }))
    {
        // if it is too large for the batch
        throw new Exception($"The message {item.Key} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();

Dictionary<KeyValuePair<Guid, int>, List<Change>> getTestData()
{
    var testData = new List<Change>();
    for (int i = 1; i <= 100; i++)
    {
        var id = Guid.NewGuid();
        testData.Add(new Change
        {
            PersoonId = id,
            SubAffiliateKey = (i % 10).ToString(),
            BatchNumber = 1,
            Name = $"Name {i}",
            Description = $"Change A"
        });
        testData.Add(new Change
        {
            PersoonId = id,
            SubAffiliateKey = (i % 10).ToString(),
            BatchNumber = 1,
            Name = $"Name {i}",
            Description = $"Change B"
        });
        testData.Add(new Change
        {
            PersoonId = id,
            SubAffiliateKey = (i % 10).ToString(),
            BatchNumber = 1,
            Name = $"Name {i}",
            Description = $"Change C"
        });
        testData.Add(new Change
        {
            PersoonId = id,
            SubAffiliateKey = (i % 10).ToString(),
            BatchNumber = 2,
            Name = $"Name {i}",
            Description = $"Change D"
        });
        testData.Add(new Change
        {
            PersoonId = id,
            SubAffiliateKey = (i % 10).ToString(),
            BatchNumber = 2,
            Name = $"Name {i}",
            Description = $"Change E"
        });
    }
    return testData.GroupBy(c => new KeyValuePair<Guid, int>(c.PersoonId, c.BatchNumber)).ToDictionary(g => g.Key, g => g.ToList());
}