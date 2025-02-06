using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeProcessor
{
    internal class ChangeCommandHandler : IHandleMessages<ChangeCommand>
    {
        public async Task Handle(ChangeCommand message, IMessageHandlerContext context)
        {
            //simulate work
            await Task.Delay(1000, context.CancellationToken);
            Console.WriteLine($"The change {message.ChangeToProcess.Name} - Batch {message.ChangeToProcess.BatchNumber} - {message.ChangeToProcess.Description} has been processed!");

            
            await context.Publish(new ChangeCompleted()
            {
                PersoonId = message.PersoonId,
                SubAffiliateKey = message.SubAffiliateKey,
                ProcessedChange = message.ChangeToProcess
            });
        }
    }
}
