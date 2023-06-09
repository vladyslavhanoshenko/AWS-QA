﻿using AwsTestingSolution.ApiClients.EC2.Models;

namespace AwsTestingSolution.Storages
{
    public class EC2InstancesDataStorage
    {
        public static EC2InstanceModel[] ExpectedDeployedInstances => new EC2InstanceModel[] { PublicInstanceExpectedData, PrivateInstanceExpectedData };

        public static EC2InstanceModel PublicInstanceExpectedData = new EC2InstanceModel
        {
            InstanceId = "i-058f53810c4397292",
            InstanceType = "t2.micro",
            InstanceTags = new string[] { "qa", "cloudxinfo/PublicInstance/Instance" },
            RootBlockDeviceSize = 8,
            InstanceOs = "Linux/UNIX",
            PublicDns = "ec2-54-173-17-255.compute-1.amazonaws.com",
            PublicIp = "54.173.17.255"
        };

        public static EC2InstanceModel PrivateInstanceExpectedData = new EC2InstanceModel
        {
            InstanceId = "i-0765cdff5ff6f7070",
            InstanceType = "t2.micro",
            InstanceTags = new string[] { "qa", "cloudxinfo/PrivateInstance/Instance" },
            RootBlockDeviceSize = 8,
            InstanceOs = "Linux/UNIX",
        };

        public static string PublicInstanceSecurityGroupId = "sg-03d9f3763cb3a2f44";

        public static string PrivateInstanceSecurityGroupId = "sg-006ae86d26d2b7de6";

        public static string PrivateInstancePrivateDns = "http://ip-10-0-104-227.ec2.internal";

        public static string PrivateInstancePrivateIp = "10.0.104.227";

        public static string EC2UserName = "ec2-user";
    }
}
