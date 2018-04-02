using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Deepleo.Web
{
    public class ComputerVisionHelper
    {
        #region
        public static async Task<string> MakeAnalyzeImageRequest(string URL)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "42b722f1409c40909fa2f66a3b90f382");

            // Request parameters
            //queryString["visualFeatures"] = "All";
            queryString["visualFeatures"] = "Description";
            //queryString["visualFeatures"] = "Tags";


            var uri = "https://api.projectoxford.ai/vision/v1/analyses?" + queryString;

            HttpResponseMessage response;

            // Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"https://portalstoragewuprod2.azureedge.net/vision/Analysis/12-1.jpg\"}");
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"" + URL + "\"}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                string JSON = await response.Content.ReadAsStringAsync();

                AnalyzeImageDescriptionObject ro = JsonHelper.Deserialize<AnalyzeImageDescriptionObject>(JSON);

                StringBuilder sb = new StringBuilder();


                if (ro.description.captions.Count != 0)
                    sb.Append("hey I saw ");
                else
                {
                    sb.Append("hey I saw nothing");
                    return sb.ToString();
                }


                for (int i = 0; i < ro.description.captions.Count; i++)
                {
                    Caption caption = ro.description.captions[i];

                    sb.Append(caption.text);

                    if (i + 1 >= ro.description.captions.Count)
                        sb.Append(".");
                    else
                        sb.Append(", and ");

                }

                if (ro.description.tags.Count != 0)
                {
                    sb.Append(" (The picture seems include: ");
                }
                else
                {
                    return sb.ToString();
                }

                for (int i = 0; i < ro.description.tags.Count; i++)
                {
                    string tag = ro.description.tags[i];

                    sb.Append(tag);

                    if (i + 1 >= ro.description.tags.Count)
                        sb.Append(")");
                    else
                        sb.Append(" ");
                }
                return sb.ToString();
            }

        }
        #endregion


        #region OCR
        public static async Task<string> MakeOCRRequest(string URL)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9459d73a95e34e5c8e15ec1bb2802d20");

            // Request parameters
            queryString["language"] = "unk";
            queryString["detectOrientation "] = "true";
            var uri = "https://api.projectoxford.ai/vision/v1.0/ocr?" + queryString;

            HttpResponseMessage response;

            // Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"https://portalstoragewuprod.azureedge.net/vision/OpticalCharacterRecognition/5.jpg\"}");
            byte[] byteData = Encoding.UTF8.GetBytes("{\"url\":\"" + URL + "\"}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //application/octet-stream

                response = await client.PostAsync(uri, content);

                string JSON = await response.Content.ReadAsStringAsync();

                OCRObject ro = JsonHelper.Deserialize<OCRObject>(JSON);

                StringBuilder sb = new StringBuilder();

                foreach (var region in ro.regions)
                {
                    foreach (var line in region.lines)
                    {
                        foreach (var word in line.words)
                        {
                            sb.Append(word.text + " ");
                        }
                        //sb.AppendLine(" ");
                    }
                    //sb.AppendLine(" ");
                }
                return sb.ToString();
            }
        }
        #endregion
    }

    #region OCRObj

    [DataContract]
    public class Word
    {
        [DataMember]
        public string boundingBox { get; set; }
        [DataMember]
        public string text { get; set; }
    }
    [DataContract]
    public class Line
    {
        [DataMember]
        public string boundingBox { get; set; }
        [DataMember]
        public List<Word> words { get; set; }
    }
    [DataContract]
    public class Region
    {
        [DataMember]
        public string boundingBox { get; set; }
        [DataMember]
        public List<Line> lines { get; set; }
    }
    [DataContract]
    public class OCRObject
    {
        [DataMember]
        public string language { get; set; }
        [DataMember]
        public double textAngle { get; set; }
        [DataMember]
        public string orientation { get; set; }
        [DataMember]
        public List<Region> regions { get; set; }
    }

    #endregion

    #region AnalyzeImage All

    [DataContract]
    public class Category
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public double score { get; set; }
    }

    [DataContract]
    public class Adult
    {
        [DataMember]
        public bool isAdultContent { get; set; }
        [DataMember]
        public bool isRacyContent { get; set; }
        [DataMember]
        public double adultScore { get; set; }
        [DataMember]
        public double racyScore { get; set; }
    }

    [DataContract]
    public class Metadata
    {
        [DataMember]
        public int width { get; set; }
        [DataMember]
        public int height { get; set; }
        [DataMember]
        public string format { get; set; }
    }

    [DataContract]
    public class Color
    {
        [DataMember]
        public string dominantColorForeground { get; set; }
        [DataMember]
        public string dominantColorBackground { get; set; }
        [DataMember]
        public List<string> dominantColors { get; set; }
        [DataMember]
        public string accentColor { get; set; }
        [DataMember]
        public bool isBWImg { get; set; }
    }

    [DataContract]
    public class ImageType
    {
        [DataMember]
        public int clipArtType { get; set; }
        [DataMember]
        public int lineDrawingType { get; set; }
    }

    [DataContract]
    public class AnalyzeImageObject
    {
        [DataMember]
        public List<Category> categories { get; set; }
        [DataMember]
        public Adult adult { get; set; }
        [DataMember]
        public string requestId { get; set; }
        [DataMember]
        public Metadata metadata { get; set; }
        [DataMember]
        public List<object> faces { get; set; }
        [DataMember]
        public Color color { get; set; }
        [DataMember]
        public ImageType imageType { get; set; }
    }

    #endregion

    #region AnalyzeImage Description
    [DataContract]
    public class Caption
    {
        [DataMember]
        public string text { get; set; }

        [DataMember]
        public double confidence { get; set; }
    }
    [DataContract]
    public class Description
    {

        [DataMember]
        public List<string> tags { get; set; }

        [DataMember]
        public List<Caption> captions { get; set; }
    }

    [DataContract]
    public class AnalyzeImageDescriptionObject
    {

        [DataMember]
        public Description description { get; set; }

        [DataMember]
        public string requestId { get; set; }

        [DataMember]
        public Metadata metadata { get; set; }
    }

    #endregion

    public class JsonHelper
    {
        /// <summary>
        /// 将JSON字符串反序列化成数据对象
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>返回数据对象</returns>
        public static T Deserialize<T>(string json)
        {
            var _Bytes = Encoding.Unicode.GetBytes(json);
            using (MemoryStream _Stream = new MemoryStream(_Bytes))
            {
                var _Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)_Serializer.ReadObject(_Stream);
            }
        }
    }
}


