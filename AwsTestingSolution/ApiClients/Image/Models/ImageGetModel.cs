using Newtonsoft.Json;

namespace AwsTestingSolution.ApiClients.Image.Models
{
    public class ImageGetModel
    {
        public int Id { get; set; }

        [JsonProperty("object_key")]
        public string ObjectKey { get; set; }
    }
}
