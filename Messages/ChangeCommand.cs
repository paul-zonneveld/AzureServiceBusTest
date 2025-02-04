using NServiceBus;
using QueueSender;

namespace Messages
{
    [Serializable]
    public class ChangeCommand : ICommand
    {
        public Guid PersoonId { get; set; }
        public string SubAffiliateKey { get; set; }
        public Change ChangeToProcess { get; set; }
    }
}
