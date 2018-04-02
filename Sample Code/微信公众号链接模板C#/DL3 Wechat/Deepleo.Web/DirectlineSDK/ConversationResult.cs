using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectLineSDK.Models
{
    /// <summary>
    /// 会话结果
    /// </summary>
    public class ConversationResult
    {
        public bool isReplyReceived { get; set; }

        public string resultMessage { get; set; }

        public ActivitiesSet botActivities { get; set; }

        public ConversationResult() { }

        public ConversationResult(bool isReplyReceived, ActivitiesSet botActivities, string resultMessage = "")
        {
            this.isReplyReceived = isReplyReceived;
            this.botActivities = botActivities;
            this.resultMessage = resultMessage;
        }
    }
}
