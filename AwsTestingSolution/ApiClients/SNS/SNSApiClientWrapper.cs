using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients.SNS
{
    public class SNSApiClientWrapper : AwsApiClientBase
    {
        public SNSApiClientWrapper() => GetAwsCredentials();
        private AmazonSimpleNotificationServiceClient SNSClient => new AmazonSimpleNotificationServiceClient(awsCredentials, AwsConfig.RegionEndpoint);

        public ListTopicsResponse GetAllTopics()
        {
            var response = SNSClient.ListTopicsAsync();
            return response.Result;
        }

        public GetTopicAttributesResponse GetTopicAttributes(string topicArn)
        {
            var snsTopicResponse = SNSClient.GetTopicAttributesAsync(new GetTopicAttributesRequest
            {
                TopicArn = topicArn
            });

            return snsTopicResponse.Result;
        }

        public ListTagsForResourceResponse GetTagsForTopic(string topicArn)
        {
            var request = new ListTagsForResourceRequest
            {
                ResourceArn = topicArn
            };

            var response = SNSClient.ListTagsForResourceAsync(request);
            return response.Result;
        }
    }
}
