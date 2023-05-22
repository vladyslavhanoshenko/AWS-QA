using Amazon.SimpleNotificationService.Model;
using Amazon.SQS.Model;
using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.Serverless
{
    [TestFixture]
    public class ServerlessDeploymentScenarios : AwsTestsBase
    {
        [Test]
        public void VerifyApplicationDatabase()
        {
            var actualDynampDbTable = DynamoDBApiClientWrapper.GetDynamoDbTables();
            actualDynampDbTable.TableNames.Should().OnlyContain(t => t.Equals(ServerlessDataStorage.DatabaseTableName));

            var actualRdsInstancesDeployed = RdsApiClientWrapper.GetDbInstances();
            actualRdsInstancesDeployed.DBInstances.Count().Should().Be(0);
        }

        [Test]
        public void VerifyTableItemAttributes()
        {
            var actualTableItems = DynamoDBApiClientWrapper.ScanItemsInTable(ServerlessDataStorage.DatabaseTableName);
            actualTableItems.Items.All(i => i.ContainsKey("object_key")).Should().BeTrue();
            actualTableItems.Items.All(i => i.ContainsKey("object_size")).Should().BeTrue();
            actualTableItems.Items.All(i => i.ContainsKey("created_at")).Should().BeTrue();
            actualTableItems.Items.All(i => i.ContainsKey("object_type")).Should().BeTrue();
            actualTableItems.Items.All(i => i.ContainsKey("last_modified")).Should().BeTrue();
        }

        [Test]
        public void VerifyLambda()
        {
            var allLambdaFunctions = LambdaApiClientWrapper.GetLambdaFunctions();
            var handlerLambda = allLambdaFunctions.Functions.Single(f => f.FunctionName.Contains("EventHandlerLambda"));
            handlerLambda.MemorySize.Should().Be(128);
            handlerLambda.Timeout.Should().Be(3);
            handlerLambda.EphemeralStorage.Size.Should().Be(512);
            handlerLambda.Environment.Variables.Any(i => i.Value.Contains("TopicSNSTopic"));
            var lambdaTags = LambdaApiClientWrapper.GetLambdaTags(handlerLambda.FunctionArn);
            lambdaTags.Tags.Should().Contain(t => t.Key.Equals("cloudx") && t.Value.Equals("qa"));
        }

        [Test]
        public void VerifyDynamoDbTableRequirements()
        {
            var tableDescription = DynamoDBApiClientWrapper.DescribeTable(ServerlessDataStorage.DatabaseTableName);
            var globalSecondaryIndexesEnabled = tableDescription.Table.GlobalSecondaryIndexes != null && tableDescription.Table.GlobalSecondaryIndexes.Count > 0;

            var provisionedReadCapacityUnits = tableDescription.Table.ProvisionedThroughput.ReadCapacityUnits;
            var provisionedWriteCapacityUnits = tableDescription.Table.ProvisionedThroughput.WriteCapacityUnits;

            globalSecondaryIndexesEnabled.Should().BeFalse();
            provisionedReadCapacityUnits.Should().Be(5);
            provisionedWriteCapacityUnits.Should().Be(1);

            var tableTags = DynamoDBApiClientWrapper.GetTableTags(tableDescription.Table.TableArn);
            tableTags.Tags.Should().Contain(t => t.Key.Equals("cloudx") && t.Value.Equals("qa"));
        }

        [Test]
        public void VerifyApplicationUsesSns() //this scenario partially duplicates logic in VerifyLambda scenario
        {
            ListTopicsResponse actualSnsTopics = SNSApiClientWrapper.GetAllTopics();
            actualSnsTopics.Topics.Should().Contain(t => t.TopicArn.Contains("cloudxserverless-TopicSNSTopic"));
        }

        [Test]
        public void VerifyApplicationUsesSqs() 
        {
            ListQueuesResponse actualSqsQueus = SQSApiClientWrapper.GetSqsQueues();
            actualSqsQueus.QueueUrls.Should().Contain(t => t.Contains("cloudxserverless-QueueSQSQueue"));
        }
    }
}

