using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    [Serializable]
    public class SagaStartedEvent : IEvent
    {
        public Guid PersoonId { get; set; }
        public int BatchNumber { get; set; }
    }
}
