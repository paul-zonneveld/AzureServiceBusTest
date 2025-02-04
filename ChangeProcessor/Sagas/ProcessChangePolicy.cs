using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using QueueSender;

namespace Cobra.Batch.Sagas
{
    public class ProcessChangePolicy : Saga<ProcessChangePolicyData>,
        IAmStartedByMessages<StartProcessChangesSaga>,
        IHandleMessages<ChangeCompleted>
    {

        public ProcessChangePolicy()
        {

        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ProcessChangePolicyData> mapper)
        {
            mapper.MapSaga(saga => saga.PersoonId)
                .ToMessage<StartProcessChangesSaga>(msg => msg.PersoonId)
                .ToMessage<ChangeCompleted>(msg => msg.PersoonId);
        }

        public async Task Handle(StartProcessChangesSaga message, IMessageHandlerContext context)
        {
            if (Data.IsActive)
            {
                Console.WriteLine($"Adding {message.Changes.Count} changes to existing saga for {message.Changes[0].Name}");
                // Already processing a request
                foreach (var change in message.Changes)
                {
                    Data.ChangesToProcess.Enqueue(change);
                }
            }
            else
            {
                Console.WriteLine($"Starting a new saga for {message.Changes[0].Name}");
                // Start processing
                Data.PersoonId = message.PersoonId;
                Data.SubAffiliateKey = message.SubAffiliateKey;
                Data.ChangesToProcess = new Queue<Change>(message.Changes);
                Data.IsActive = true;

                var change = Data.ChangesToProcess.Dequeue();

                if (change == null)
                    return;

                Console.WriteLine($"Sending command to start processing change {change.Name} - Batch {change.BatchNumber} - {change.Description}.");
                await context.Send(new ChangeCommand()
                {
                    PersoonId = message.PersoonId,
                    SubAffiliateKey = message.SubAffiliateKey,
                    ChangeToProcess = change
                });
            }
            await context.Publish(new SagaStartedEvent()
            {
                PersoonId = message.PersoonId, 
                BatchNumber = message.BatchNumber
            });
        }

        public async Task Handle(ChangeCompleted message, IMessageHandlerContext context)
        {
            Data.ProcessedChanges.Add(message.ProcessedChange);

            if (Data.ChangesToProcess.Count == 0)
            {
                // All changes processed
                Console.WriteLine($"Saga completed for {message.ProcessedChange.Name} ({Data.ProcessedChanges.Count} changes processed)");
                MarkAsComplete();
            }
            else
            {
                var change = Data.ChangesToProcess.Dequeue();
                if (change == null)
                    return;
                Console.WriteLine($"Sending command to start processing change {change.Name} - Batch {change.BatchNumber} - {change.Description}.");
                await context.Send(new ChangeCommand()
                {
                    PersoonId = Data.PersoonId,
                    SubAffiliateKey = Data.SubAffiliateKey,
                    ChangeToProcess = change
                });
            }
        }
    }
}
