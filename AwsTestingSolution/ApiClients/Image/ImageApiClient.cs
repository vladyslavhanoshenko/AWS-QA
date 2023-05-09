using AwsTestingSolution.ApiClients.Image.Models;
using AwsTestingSolution.Storages;

namespace AwsTestingSolution.ApiClients.Image
{
    public class ImageApiClient : ApiClientBase
    {
        public ImageModel UploadImageToS3Bucket(string pathToImage, string alternativeName = null)
        {
            return UploadFile<ImageModel>(CloudximageDataStorage.AppInstancePublicDns + "/api/image", pathToImage, alternativeName);
        }

        public ImageGetModel[] GetUploadedImages()
        {
            return ExecuteGet<ImageGetModel[]>(CloudximageDataStorage.AppInstancePublicDns + "/api/image");
        }

        public ImageGetModel GetUploadedImageById(int imageId)
        {
            return ExecuteGet<ImageGetModel>(CloudximageDataStorage.AppInstancePublicDns + $"/api/image/{imageId}");
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
