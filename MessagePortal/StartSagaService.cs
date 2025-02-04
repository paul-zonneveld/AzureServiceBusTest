using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagePortal
{
    public class StartSagaService
    {
        private readonly IMessageSession messageSession;

        public StartSagaService(IMessageSession messageSession)
        {
            this.messageSession = messageSession;
        }

        internal async Task SendMessage(StartProcessChangesSaga cmd)
        {
            await messageSession.Send(cmd);
        }
    }
}
