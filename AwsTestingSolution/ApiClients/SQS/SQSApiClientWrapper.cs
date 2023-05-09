using Amazon.SQS;
using Amazon.SQS.Model;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients.SQS
{
    public class SQSApiClientWrapper : AwsApiClientBase
    {
        public SQSApiClientWrapper() => GetAwsCredentials();
        private AmazonSQSClient SQSClient => new AmazonSQSClient(awsCredentials, AwsConfig.RegionEndpoint);

        public GetQueueAttributesResponse GetSqsQueueAttributes(string queueUrl)
        {
            var sqsQueueResponse = SQSClient.GetQueueAttributesAsync(new GetQueueAttributesRequest
            {
                QueueUrl = queueUrl,
                AttributeNames = new List<string> { "All" }
            });
            return sqsQueueResponse.Result;
        }

        public ListQueueTagsResponse GetSqsQueueTags(string queueUrl)
        {
            var sqsQueueResponse = SQSClient.ListQueueTagsAsync(new ListQueueTagsRequest
            {
                QueueUrl = queueUrl,
            });
            return sqsQueueResponse.Result;
        }
    }
}
