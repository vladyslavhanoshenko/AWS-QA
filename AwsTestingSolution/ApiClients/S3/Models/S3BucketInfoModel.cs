using Amazon.S3.Model;

namespace AwsTestingSolution.ApiClients.S3.Models
{
    public class S3BucketInfoModel
    {
        public string BucketName { get; set; }
        public List<Tag> Tags { get; set; }
        public bool IsEncryptionEnabled { get; set; }
        public bool IsVersioningEnabled { get; set; }
        public bool PublicAccess { get; set; }
    }
}
