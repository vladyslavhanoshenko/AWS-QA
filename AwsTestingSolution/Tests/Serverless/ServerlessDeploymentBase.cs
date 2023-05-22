using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.Serverless
{
    [TestFixture]
    public class ServerlessDeploymentBase : AwsTestsBase
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
    }
}
