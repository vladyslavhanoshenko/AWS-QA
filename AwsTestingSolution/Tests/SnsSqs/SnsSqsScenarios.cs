using Amazon.SimpleNotificationService.Model;
using AwsTestingSolution.ApiClients.SNS;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.SnsSqs
{
    [TestFixture]
    public class SnsSqsScenarios : AwsTestsBase
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
    }
}
