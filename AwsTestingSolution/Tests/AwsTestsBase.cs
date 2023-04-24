using AwsTestingSolution.ApiClients.EC2;

namespace AwsTestingSolution.Tests
{
    public class AwsTestsBase
    {
        protected EC2ApiClientWrapper EC2ApiClientWrapper => new EC2ApiClientWrapper();
    }
}
