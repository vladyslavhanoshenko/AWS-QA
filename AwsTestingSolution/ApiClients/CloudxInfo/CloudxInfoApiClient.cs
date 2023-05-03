using AwsTestingSolution.ApiClients.CloudxInfo.Models;

namespace AwsTestingSolution.ApiClients.CloudxInfo
{
    public class CloudxInfoApiClient : ApiClientBase
    {
        public InstanceMetadataModel GetIntanceMetaData(string url)
        {
            return ExecuteGet<InstanceMetadataModel>(url);
        }
    }
}
