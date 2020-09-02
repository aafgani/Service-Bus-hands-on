using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace MessageSender
{
    class QueueMessageMonitor : QueueMsgReader
    {
        public override Task<int> ReportMessageCount()
        {
            return null;
        }
    }
}
