using NServiceBus;
using QueueSender;

namespace Messages
{
    [Serializable]
    public class StartProcessChangesSaga : ICommand
    {
        public Guid PersoonId { get; set; }
        public string SubAffiliateKey { get; set; }
        public int BatchNumber { get; set; }
        public List<Change> Changes { get; set; } = new List<Change>();
    }
}
