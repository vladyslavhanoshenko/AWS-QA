using Amazon.EC2.Model;
using AwsTestingSolution.ApiClients.EC2;
using AwsTestingSolution.Configs;
using AwsTestingSolution.Storages;
using FluentAssertions;
using NUnit.Framework;
using Renci.SshNet;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

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

        [Test]
        public void VerifyAccessibilityOfPublicInstanceViaInternetGateway()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://" + EC2InstancesConfigurationStorage.PublicInstanceExpectedData.PublicDns);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public void PrivateInstanceShouldHaveAccessToTheInternetViaNatGateway()
        {
            var connectionInfo = new ConnectionInfo(EC2InstancesConfigurationStorage.PublicInstanceExpectedData.PublicDns, EC2InstancesConfigurationStorage.EC2UserName, new PrivateKeyAuthenticationMethod(EC2InstancesConfigurationStorage.EC2UserName, new PrivateKeyFile(CredentialsConfig.PemKeyFilePath)));
            var sshClient = new SshClient(connectionInfo);
            sshClient.Connect();

          
            var forwardedPort = new ForwardedPortLocal(SshConfig.LocalIpAddress, SshConfig.LocalPortForPrivateInstance, EC2InstancesConfigurationStorage.PrivateInstancePrivateIp, 22);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            var privateInstance = new ConnectionInfo(SshConfig.LocalIpAddress, (int)SshConfig.LocalPortForPrivateInstance, EC2InstancesConfigurationStorage.EC2UserName, new PrivateKeyAuthenticationMethod(EC2InstancesConfigurationStorage.EC2UserName, new PrivateKeyFile(CredentialsConfig.PemKeyFilePath)));
            var privateSshClient = new SshClient(privateInstance);
            privateSshClient.Connect();

            SshCommand response = privateSshClient.RunCommand($"curl https://swapi.dev/api/people/1/");
            string curlResult = response.Result;
            curlResult.Should().Contain("Luke Skywalker");
        }

        [Test]
        public void PrivateInstanceShouldNotBeAccessibleFromPublicInternet()
        {
            string ipAddress = EC2InstancesConfigurationStorage.PrivateInstancePrivateIp;
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "Test Data";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 120;
            PingReply reply = pingSender.Send(ipAddress, timeout, buffer, options);
            reply.Status.Should().Be(IPStatus.TimedOut);
        }
    }
}
