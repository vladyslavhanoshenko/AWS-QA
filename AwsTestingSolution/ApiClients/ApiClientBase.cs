using AwsTestingSolution.ApiClients.CloudxImage.Models;
using AwsTestingSolution.Storages;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace AwsTestingSolution.ApiClients
{
    public class ApiClientBase
    {
        public WebClient Client = new WebClient();

        protected string ExecuteGet(string uri)
        {
            return Client.DownloadString(uri);
        }

        protected T ExecuteGet<T>(string uri) where T : class
        {
            string response = ExecuteGet(uri);
            T entities = DeserializeObject<T>(response);
            return entities;
        }

        protected T UploadFile<T>(string uri, string pathToFile, string alternativeName = null) where T : class
        {
            var response = UploadFile(uri, pathToFile, alternativeName);
            return DeserializeObject<T>(response);
        }

        protected string UploadFile(string uri, string pathToFile, string alternativeName = null)
        {
            string stringResult;

            using (var httpClient = new HttpClient())
            {
                using (var content = new MultipartFormDataContent())
                {
                    byte[] imageData;
                    using (var stream = new FileStream(pathToFile, FileMode.Open))
                    {
                        imageData = new byte[stream.Length];
                        stream.ReadAsync(imageData, 0, (int)stream.Length);
                    }
                    var imageContent = new ByteArrayContent(imageData);
                    content.Add(imageContent, "upfile", alternativeName == null ? Path.GetFileName(pathToFile) : alternativeName);
                    var responseResult = httpClient.PostAsync(uri, content).Result;
                    stringResult = responseResult.Content.ReadAsStringAsync().Result;
                }
            }

            return stringResult;
        }

        protected void DownloadFileGet(string uri, string filePath)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            request.Method = "GET";
            request.Accept = MediaTypeNames.Application.Octet;
            var response = (HttpWebResponse)request.GetResponse();
            WriteResponseStreamToFile(response, filePath);
        }

        protected void ExecuteDelete(string uri, string requestData = "")
        {
            Client.Headers.Add("Content-Type", "application/json");
            Client.UploadString(uri, "DELETE", requestData);
        }

        private void WriteResponseStreamToFile(HttpWebResponse response, string filePath)
        {
            using (Stream output = File.OpenWrite(filePath))
            using (Stream input = response.GetResponseStream())
            {
                input.CopyTo(output);
            }
        }

        protected T DeserializeObject<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value);
            }
            catch (JsonReaderException e)
            {
                throw new JsonReaderException($"Error during deserialization", e);
            }
        }
    }
}
