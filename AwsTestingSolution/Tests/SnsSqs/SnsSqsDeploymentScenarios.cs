using Amazon.SimpleNotificationService.Model;
using Amazon.SQS.Model;
using AwsTestingSolution.ApiClients.SNS;
using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.SnsSqs
{
    [TestFixture]
    public class SnsSqsDeploymentScenarios : AwsTestsBase
    {
        [Test]
        [Category("Deployment")]
        public void CheckSnsTopicRequirements()
        {
            ListTopicsResponse actualSnsTopics = SNSApiClientWrapper.GetAllTopics();
            var snsTopic = actualSnsTopics.Topics.Single(t => t.TopicArn.Contains("cloudximage-TopicSNSTopic"));

            GetTopicAttributesResponse snsTopicAttributes = SNSApiClientWrapper.GetTopicAttributes(snsTopic.TopicArn);
            snsTopicAttributes.Attributes.ContainsKey("KmsMasterKeyId").Should().BeFalse(); //If this key is not present it means encryption is not enabled
            snsTopicAttributes.Attributes.ContainsKey("Type").Should().BeFalse(); //If this key is not present it means sns topic standard by default

            ListTagsForResourceResponse snsTopicTags = SNSApiClientWrapper.GetTagsForTopic(snsTopic.TopicArn);
            snsTopicTags.Tags.Should().Contain(t => t.Key.Equals("cloudx") && t.Value.Equals("qa"));
        }

        [Test]
        [Category("Deployment")]
        public void CheckSqsQueueRequirements()
        {
            GetQueueAttributesResponse sqsQueueAttributes = SQSApiClientWrapper.GetSqsQueueAttributes(CloudximageDataStorage.SnsQueueUrl);

            sqsQueueAttributes.Attributes.Should().Contain(a => a.Key.Equals("QueueArn") && a.Value.Contains("cloudximage-QueueSQSQueue"));
            sqsQueueAttributes.Attributes.Should().Contain(a => a.Key.Equals("SqsManagedSseEnabled") && a.Value.Equals("true"));
            sqsQueueAttributes.Attributes.ContainsKey("QueueType").Should().BeFalse(); //If this key is not present it means sns topic standard by default
            sqsQueueAttributes.Attributes.ContainsKey("RedrivePolicy ").Should().BeFalse(); //If this key is not present it means DeadLetterQueue is not enabled

            var sqsQueueTags = SQSApiClientWrapper.GetSqsQueueTags(CloudximageDataStorage.SnsQueueUrl);
            sqsQueueTags.Tags.Should().Contain(t => t.Key.Equals("cloudx") && t.Value.Equals("qa"));
        }
    }
}
