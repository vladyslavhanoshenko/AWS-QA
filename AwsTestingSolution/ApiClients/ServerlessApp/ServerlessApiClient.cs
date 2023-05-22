using AwsTestingSolution.ApiClients.Image.Models;
using AwsTestingSolution.ApiClients.ServerlessApp.Models;
using AwsTestingSolution.Storages;

namespace AwsTestingSolution.ApiClients.ServerlessApp
{
    public class ServerlessApiClient : ApiClientBase
    {
        public ServerlessGuidGetImageModel UploadImageToS3Bucket(string pathToImage, string alternativeName = null)
        {
            return UploadFile<ServerlessGuidGetImageModel>(ServerlessDataStorage.AppInstancePublicDns + "/api/image", pathToImage, alternativeName);
        }

        public ImageGetModel[] GetUploadedImages()
        {
            return ExecuteGet<ImageGetModel[]>(ServerlessDataStorage.AppInstancePublicDns + "/api/image");
        }

        public ImageGetModel GetUploadedImageById(Guid imageId)
        {
            return ExecuteGet<ImageGetModel>(ServerlessDataStorage.AppInstancePublicDns + $"/api/image/{imageId}");
        }

        public void DownloadFileFromS3Bucket(int fileId, string downloadPath)
        {
            DownloadFileGet(ServerlessDataStorage.AppInstancePublicDns + $"/api/image/{fileId}", downloadPath);
        }

        public void DeleteFileFromS3Bucket(Guid fileId)
        {
            ExecuteDelete(ServerlessDataStorage.AppInstancePublicDns + $"/api/image/{fileId}");
        }
    }
}
