using Amazon.EC2.Model;
using AwsTestingSolution.ApiClients.EC2;
using FluentAssertions;
using NUnit.Framework;

namespace AwsTestingSolution.Tests.VPC
{
    [TestFixture]
    public class VPCScenarios : AwsTestsBase
    {
        [Test]
        public void VerifyVpc()
        {
            Vpc vpc = EC2ApiClientWrapper.GetVpcByName("cloudxinfo/Network/Vpc");
            vpc.IsDefault.Should().BeFalse();
            vpc.CidrBlock.Should().BeEquivalentTo("10.0.0.0/16");
        }

        [Test]
        public void VerifyVpcSubnets()
        {
            Vpc vpc = EC2ApiClientWrapper.GetVpcByName("cloudxinfo/Network/Vpc");
            DescribeSubnetsResponse actualSubnets = EC2ApiClientWrapper.DescribeSubnetsForVpc(vpc.VpcId);

            actualSubnets.Subnets.Count().Should().Be(2);
            actualSubnets.Subnets.Should().Contain(subnet => subnet.Tags.Any(tag => tag.Key.Equals("aws-cdk:subnet-type") && tag.Value.Equals("Private")));
            actualSubnets.Subnets.Should().Contain(subnet => subnet.Tags.Any(tag => tag.Key.Equals("aws-cdk:subnet-type") && tag.Value.Equals("Public")));
        }
    }
}
