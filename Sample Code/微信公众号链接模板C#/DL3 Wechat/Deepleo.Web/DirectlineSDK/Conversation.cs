using System;

namespace DirectLineSDK.Models
{
    /// <summary>
    /// 会话
    /// </summary>
    public class Conversation
    {
        public string id { get; set; }

        public string conversationId { get; set; }

        public string token { get; set; }

        public string expires_in { get; set; }

        public string streamUrl { get; set; }
    }
}
