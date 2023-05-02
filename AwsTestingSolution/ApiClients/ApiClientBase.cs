using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients
{
    public class ApiClientBase
    {
        protected AWSCredentials awsCredentials;

        protected void GetAwsCredentials()
        {
            var chain = new CredentialProfileStoreChain();
            bool IsCredentialsReceived = chain.TryGetAWSCredentials(AwsConfig.ProfileName, out awsCredentials);
            if (!IsCredentialsReceived) throw new Exception("Credentials are not correct. Please check profile or credentials in AWS CLI");
        }
    }
}
