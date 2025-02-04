using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueSender
{
    public class Change
    {
        public Guid PersoonId { get; set; }
        public string SubAffiliateKey { get; set; }
        public int BatchNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
