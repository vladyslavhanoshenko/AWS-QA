using AwsTestingSolution.ApiClients.EC2;
using AwsTestingSolution.ApiClients.S3;

namespace AwsTestingSolution.Tests
{
    public class AwsTestsBase
    {
        protected EC2ApiClientWrapper EC2ApiClientWrapper => new EC2ApiClientWrapper();
        protected S3ApiClientWrapper S3ApiClientWrapper => new S3ApiClientWrapper();
    }
}
