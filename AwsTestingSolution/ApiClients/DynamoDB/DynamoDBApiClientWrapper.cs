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

        public DescribeTableResponse DescribeTable(string tableName)
        {
            var describeTableRequest = new DescribeTableRequest
            {
                TableName = tableName
            };

            var describeTableResponse = AmazonDynamoDBClient.DescribeTableAsync(describeTableRequest);
            DescribeTableResponse tableDescription = describeTableResponse.Result;
            return tableDescription;
        }

        public ListTagsOfResourceResponse GetTableTags(string tableArn)
        {
            var response = AmazonDynamoDBClient.ListTagsOfResourceAsync(new ListTagsOfResourceRequest { ResourceArn = tableArn});
            var responseResults = response.Result;
            return responseResults;
        }
    }
}
