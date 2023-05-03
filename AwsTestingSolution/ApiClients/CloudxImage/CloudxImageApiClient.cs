using AwsTestingSolution.ApiClients.CloudxImage.Models;
using AwsTestingSolution.Storages;

namespace AwsTestingSolution.ApiClients.CloudxImage
{
    public class CloudxImageApiClient : ApiClientBase
    {
        public CloudxImageUploadImageModel UploadImageToS3Bucket(string pathToImage, string alternativeName = null)
        {
            return UploadFile<CloudxImageUploadImageModel>(CloudximageDataStorage.AppInstancePublicDns + "/api/image", pathToImage, alternativeName);
        }

        public CloudxImageGetModel[] GetUploadedImages()
        {
            return ExecuteGet<CloudxImageGetModel[]>(CloudximageDataStorage.AppInstancePublicDns + "/api/image");
        }

        public void DownloadFileFromS3Bucket(int fileId, string downloadPath)
        {
            DownloadFileGet(CloudximageDataStorage.AppInstancePublicDns + $"/api/image/{fileId}", downloadPath);
        }

        public void DeleteFileFromS3Bucket(int fileId)
        {
            ExecuteDelete(CloudximageDataStorage.AppInstancePublicDns + $"/api/image/{fileId}");
        }
    }
}
