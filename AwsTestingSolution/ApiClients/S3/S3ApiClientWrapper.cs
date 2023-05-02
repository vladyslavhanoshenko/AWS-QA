using Amazon.S3;
using Amazon.S3.Model;
using AwsTestingSolution.ApiClients.S3.Models;
using AwsTestingSolution.Configs;

namespace AwsTestingSolution.ApiClients.S3
{
    public class S3ApiClientWrapper : ApiClientBase
    {
        public S3ApiClientWrapper() => GetAwsCredentials();
        private AmazonS3Client S3Client => new AmazonS3Client(awsCredentials, AwsConfig.S3Config);

        public ListBucketsResponse GetS3Buckets()
        {
            var response = S3Client.ListBucketsAsync(new ListBucketsRequest());
            return response.Result;
        }

        public S3BucketInfoModel GetS3BucketInfoByName(string bucketName)
        {
            var encriptionInfo = GetBucketEncriptionAsync(bucketName);
            var versioningInfo = GetBucketVersioningAsync(bucketName);
            var publicAccessInfo = GetPublicAccessBlock(bucketName);
            var tagsInfo = GetTags(bucketName);

            var info = new S3BucketInfoModel
            {
                BucketName = bucketName,
                IsEncryptionEnabled = encriptionInfo.ServerSideEncryptionConfiguration.ServerSideEncryptionRules.Single().BucketKeyEnabled,
                IsVersioningEnabled = versioningInfo.VersioningConfig.Status.Value.Equals("Off") ? false : true,
                PublicAccess = publicAccessInfo.PublicAccessBlockConfiguration.RestrictPublicBuckets == true ? false : true,
                Tags = tagsInfo.TagSet
            };

            return info;
        }

        public GetBucketEncryptionResponse GetBucketEncriptionAsync(string bucketName)
        {
            var encryptionResponse = S3Client.GetBucketEncryptionAsync(new GetBucketEncryptionRequest
            {
                BucketName = bucketName
            });

            return encryptionResponse.Result;
        }

        public GetBucketVersioningResponse GetBucketVersioningAsync(string bucketName)
        {
            var versioningResponse = S3Client.GetBucketVersioningAsync(new GetBucketVersioningRequest
            {
                BucketName = bucketName
            });

            return versioningResponse.Result;
        }

        public GetPublicAccessBlockResponse GetPublicAccessBlock(string bucketName)
        {
            var publicAccessResponse = S3Client.GetPublicAccessBlockAsync(new GetPublicAccessBlockRequest
            {
                BucketName = bucketName
            });

            return publicAccessResponse.Result;
        }

        public GetBucketTaggingResponse GetTags(string bucketName)
        {
            var taggingResponse = S3Client.GetBucketTaggingAsync(new GetBucketTaggingRequest
            {
                BucketName = bucketName
            });

            return taggingResponse.Result;
        }
    }
}
