using AwsTestingSolution.ApiClients.CloudxInfo.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace AwsTestingSolution.ApiClients.CloudxInfo
{
    public class CloudxInfoApiClient
    {
        public InstanceMetadataModel GetIntanceMetaData(string url)
        {
            WebClient client = new WebClient();
            var response = client.DownloadString(url);
            return JsonConvert.DeserializeObject<InstanceMetadataModel>(response);
        }
    }
}
