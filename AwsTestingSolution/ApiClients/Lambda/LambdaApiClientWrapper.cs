using Amazon.Lambda;
using Amazon.Lambda.Model;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients.Lambda
{
    public class LambdaApiClientWrapper : AwsApiClientBase
    {
        public LambdaApiClientWrapper() => GetAwsCredentials();
        private AmazonLambdaClient AmazonLambdaClient => new AmazonLambdaClient(awsCredentials, AwsConfig.RegionEndpoint);

        public ListFunctionsResponse GetLambdaFunctions()
        {
            var listFunctionsRequest = new ListFunctionsRequest();
            var listFunctionsResponse = AmazonLambdaClient.ListFunctionsAsync(listFunctionsRequest);
            return listFunctionsResponse.Result;
        }

        public ListTagsResponse GetLambdaTags(string functionName)
        {
            var listTagsRequest = new ListTagsRequest
            {
                Resource = functionName
            };
            var listTagsResponse = AmazonLambdaClient.ListTagsAsync(listTagsRequest);
            return listTagsResponse.Result;
        }
    }
}
