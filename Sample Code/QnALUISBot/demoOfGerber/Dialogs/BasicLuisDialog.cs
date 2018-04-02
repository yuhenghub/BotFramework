using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Linq;
using demoOfGerber.Common;
using System.Net;
using Newtonsoft.Json;

namespace demoOfGerber.Dialogs
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis

    [LuisModel("6be30be2-13ec-4d51-8211-d735e94b4ffd", "9511c42db4d448f5b7c2b57f58303499")]


    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {

        public class item
        {
            public string name;
            public float price;
            public string link;
        }

        List<item> itemlist;

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"我们支持的功能包括： 价格查询\r\n 天气查询\r\nFAQ");
            context.Wait(MessageReceived);
        }


        [LuisIntent("问候")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"你好 我是Nestle智能助手 我能帮助您进行价格查询以及常见问题的回答，请问有什么可以帮到您？");
            context.Wait(MessageReceived);
        }
        [LuisIntent("感谢")]
        public async Task Bye(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Nestle智能助手感谢您的咨询 欢迎下次使用!");
            context.Wait(MessageReceived);
        }
        [LuisIntent("")]
        public async Task NoneIntent1(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"我们支持的功能包括： 价格查询\r\n 天气查询\r\nFAQ");
            context.Wait(MessageReceived);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("查询价格")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            List<item> itemlist = new List<item>();

            itemlist.Add(new item() { name = "嘉宝混合蔬菜营养米粉", price = 55, link = "https://detail.tmall.com/item.htm?spm=a220m.1000858.1000725.1.73ff5c64PBAwek&id=523935540403&areaId=110100&user_id=1771975008&cat_id=2&is_b=1&rn=785c7c6b26fc9427849a91538484930a" });
            itemlist.Add(new item() { name = "嘉宝胡萝卜营养米粉", price = 53, link = "https://chaoshi.detail.tmall.com/item.htm?spm=a220m.1000858.1000725.5.6ae52cc5E2ziM6&id=522920732025&areaId=110100&user_id=725677994&cat_id=2&is_b=1&rn=8ad12de2109323c54ff237f4d0efae92" });
            itemlist.Add(new item() { name = "嘉宝钙铁锌营养麦粉", price = 53, link = "https://detail.tmall.com/item.htm?spm=a220m.1000858.1000725.5.298e1e1835g47F&id=41440242255&areaId=110100&user_id=1771975008&cat_id=2&is_b=1&rn=984b075c12f140a3968fd3c619bfd609" });
            itemlist.Add(new item() { name = "嘉宝有机香蕉苹果营养米粉", price = 72, link = "https://detail.tmall.com/item.htm?spm=a220m.1000858.1000725.2.35fb3ae0KqlusG&id=563561819160&areaId=110100&user_id=3564603456&cat_id=2&is_b=1&rn=b5a75e58c281c28d055fb7c06d53dee8" });


            EntityRecommendation brand;
            EntityRecommendation category;
            EntityRecommendation flavour;

            string brands = "";
            string categorys = "";
            string flavours = "";

            List<item> ListResult = itemlist;

            if (result.TryFindEntity("品牌", out brand))
            {
                brands = brand.Entity;
                ListResult = ListResult.Where(c => c.name.Contains(brands)).ToList();
            }
            else
            {
                brands = "";
            }
            if (result.TryFindEntity("种类", out category))
            {
                categorys = category.Entity;
                ListResult = ListResult.Where(c => c.name.Contains(categorys)).ToList();
            }
            else
            {
                brands = "";
            }
            if (result.TryFindEntity("口味", out flavour))
            {
                flavours = flavour.Entity;
                ListResult = ListResult.Where(c => c.name.Contains(flavours)).ToList();
            }
            else
            {
                brands = "";
            }
            string reply = string.Join("\r\n", ListResult.Select(c => string.Format("{0} -- 价格:{1} 详情点击:{2}", c.name, c.price, c.link)));

            if (string.IsNullOrEmpty(reply))
            {
                reply = "没有找到匹配数据！";
            }

            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }


        [LuisIntent("FAQ")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            string responseString = string.Empty;

            var query = result.Query; //User Query
            var knowledgebaseId = "e64c20e8-3f0b-4286-8bbe-49dd979057a3"; // Use knowledge base id created.
            var qnamakerSubscriptionKey = "c9729930661e4d1eb1e9eec44e9ae08b"; //Use subscription key assigned to you.

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
            }
            QnAMakerResult response;
            try
            {
                response = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
            }
            catch
            {
                throw new Exception("Unable to deserialize QnA Maker response string.");
            }

            await context.PostAsync(string.Join("---", response.Answers.Select(c => string.Format("【{0}】{1}", c.Score, c.Answer))));
            context.Wait(MessageReceived);
        }

        [LuisIntent("天气查询")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }



    public class QnAMakerResultItem
    {
        /// <summary>
        /// The top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }


        [JsonProperty(PropertyName = "questions")]
        public List<string> Questions { get; set; }


        /// <summary>
        /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }
    //De-serialize the response


    public class QnAMakerResult
    {

        [JsonProperty(PropertyName = "answers")]
        public List<QnAMakerResultItem> Answers { get; set; }


        

        


    }
}
