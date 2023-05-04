using Newtonsoft.Json;

namespace AwsTestingSolution.ApiClients.CloudxImage.Models
{
    public class CloudxImageGetModel
    {
        public int Id { get; set; }
       
        [JsonProperty("object_key")]
        public string ObjectKey { get; set; }
    }
}
