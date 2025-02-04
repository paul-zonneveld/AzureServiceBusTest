using NServiceBus;
using QueueSender;

namespace Messages
{
    [Serializable]
    public class ChangeCompleted : IEvent
    {
        public Guid PersoonId { get; set; }
        public string SubAffiliateKey { get; set; }
        public Change ProcessedChange { get; set; }
    }
}
