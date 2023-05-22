using AwsTestingSolution.ApiClients.DynamoDB;
using AwsTestingSolution.ApiClients.EC2;
using AwsTestingSolution.ApiClients.RDS;
using AwsTestingSolution.ApiClients.S3;
using AwsTestingSolution.ApiClients.SNS;
using AwsTestingSolution.ApiClients.SQS;

namespace AwsTestingSolution.Tests
{
    public class AwsTestsBase
    {
        protected EC2ApiClientWrapper EC2ApiClientWrapper => new EC2ApiClientWrapper();
        protected S3ApiClientWrapper S3ApiClientWrapper => new S3ApiClientWrapper();
        protected RDSApiClientWrapper RdsApiClientWrapper => new RDSApiClientWrapper();
        protected SNSApiClientWrapper SNSApiClientWrapper => new SNSApiClientWrapper();
        protected SQSApiClientWrapper SQSApiClientWrapper => new SQSApiClientWrapper();
        protected DynamoDBApiClientWrapper DynamoDBApiClientWrapper => new DynamoDBApiClientWrapper();
        
    }
}
