using System;
using System.Collections.Generic;
using NServiceBus;
using QueueSender;

namespace Cobra.Batch.Sagas
{
    public class ProcessChangePolicyData : ContainSagaData
    {
        public Guid PersoonId { get; set; }
        public string SubAffiliateKey { get; set; }

        public bool IsActive { get; set; }
        public Queue<Change> ChangesToProcess { get; set; } = new Queue<Change>();
        public List<Change> ProcessedChanges { get; set; } = new List<Change>();
    }
}
