using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using DirectLineSDK.Models;

namespace DirectLineSDK
{
    public class BotClient
    {
        #region 配置

        private const string DirectLineRootUrl = "https://directline.botframework.com/v3/directline/conversations/";
        private ConversationResult conversationResult;
        private Conversation currentConversation;
        private HttpClient client;
        private HttpResponseMessage response;
        private string lastWatermark = "";

        #endregion

        /// <summary>
        /// 初始化会话
        /// </summary>
        /// <param name="message"></param>
        /// <param name="botConnectorKey"></param>
        /// <returns></returns>
        public async Task<ConversationResult> Conversation(string message, string botConnectorKey)
        {
            conversationResult = new ConversationResult();

            if (await GetToken(botConnectorKey))
            {
                return await CreateOrGetConversation(message);
            }

            conversationResult.resultMessage = "授权失败！";
            return conversationResult;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="botConnectorKey"></param>
        /// <returns></returns>
        private async Task<bool> GetToken(string botConnectorKey)
        {
            try
            {
                client = new HttpClient()
                {
                    BaseAddress = new Uri(DirectLineRootUrl)
                };
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", botConnectorKey);

                response = await client.PostAsync("/v3/directline/tokens/generate/", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 创建或连接当前会话
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<ConversationResult> CreateOrGetConversation(string message)
        {
            if (await TryGetCurrentConversation())
            {
                return await SendMessage(message);
            }

            return new ConversationResult(false, null, "会话创建失败！");
        }

        /// <summary>
        /// 尝试连接当前会话，失败的话，重新建立
        /// </summary>
        /// <returns></returns>
        private async Task<bool> TryGetCurrentConversation()
        {
            if (currentConversation == null)
            {
                Conversation conversation = new Conversation();
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(conversation), Encoding.UTF8, "application/json");
                response = await client.PostAsync("/v3/directline/conversations/", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    currentConversation = JsonConvert.DeserializeObject<Conversation>(response.Content.ReadAsStringAsync().Result);
                    return true;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<ConversationResult> SendMessage(string message)
        {
            Activities activitiesModel = new Activities()
            {
                type = "message",
                text = message,

                from = new ActivitiesFrom()
                {
                    id = "user1"
                }
            };
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(activitiesModel), Encoding.UTF8, "application/json");
            response = await client.PostAsync(currentConversation.conversationId + "/activities/", stringContent);
            if (response.IsSuccessStatusCode)
            {
                return await GetBotResponse();
            }
            else
            {
                return new ConversationResult(false, null, "消息发送失败！");
            }
        }

        /// <summary>
        /// 获取Bot的返回信息
        /// </summary>
        /// <returns></returns>
        private async Task<ConversationResult> GetBotResponse()
        {
            string conversationUrl = currentConversation.conversationId + "/activities";

            if (!string.IsNullOrEmpty(lastWatermark))
            {
                conversationUrl += $"?watermark={lastWatermark}";
            }

            response = await client.GetAsync(conversationUrl);

            if (response.IsSuccessStatusCode)
            {
                ActivitiesSet BotMessage = JsonConvert.DeserializeObject<ActivitiesSet>(response.Content.ReadAsStringAsync().Result);

                lastWatermark = BotMessage.watermark;
                return new ConversationResult(true, BotMessage);
            }
            else
            {
                return new ConversationResult(false, null, "消息获取失败！");
            }
        }
    }
}
