using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePortal
{
    internal class SagaStartedEventHandler : IHandleMessages<SagaStartedEvent>
    {
        Portal _portal;

        public SagaStartedEventHandler(Portal portal)
        {
            _portal = portal;    
        }

        public async Task Handle(SagaStartedEvent message, IMessageHandlerContext context)
        {
            var key = new EventKey(message.PersoonId, message.BatchNumber);
            if (_portal.EventDictionary.TryGetValue(key, out var eventInfo))
            {
                eventInfo?.Invoke(this, new EventArgs());
            }
        }
    }
}
