using DirectLineSDK.Models;
using System;

namespace DirectLineSDK.Models
{
    /// <summary>
    /// 发送的信息
    /// </summary>
    public class Activities
    {
        public Conversation conversation { get; set; }

        public ActivitiesFrom from { get; set; }

        public string text { get; set; }

        public string type { get; set; }

        public string channelId { get; set; }
    }
}
