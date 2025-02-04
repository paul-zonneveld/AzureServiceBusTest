using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePortal
{
    [Serializable]
    public struct EventKey
    {
        public EventKey(Guid persoonId, int batchNumber)
        {
            PersoonId = persoonId;
            BatchNumber = batchNumber;
        }

        public Guid PersoonId { get; }
        public int BatchNumber { get; }

    }
}
