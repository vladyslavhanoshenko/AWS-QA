using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AwsTestingSolution.Configs;
using System.Text;

namespace AwsTestingSolution.ApiClients.DynamoDB
{
    public class DynamoDBApiClientWrapper : AwsApiClientBase
    {
        public DynamoDBApiClientWrapper() => GetAwsCredentials();
        private AmazonDynamoDBClient AmazonDynamoDBClient => new AmazonDynamoDBClient(awsCredentials, AwsConfig.RegionEndpoint);

        public ListTablesResponse GetDynamoDbTables()
        {
            var request = new ListTablesRequest();
            return AmazonDynamoDBClient.ListTablesAsync(request).Result;
        }

        public ScanResponse ScanItemsInTable(string tableName)
        {
            var request = new ScanRequest
            {
                TableName = tableName
            };

            var response = AmazonDynamoDBClient.ScanAsync(request).Result;
            return response;
        }
    }
}
