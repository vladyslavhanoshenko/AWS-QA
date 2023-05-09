using AwsTestingSolution.ApiClients.Notification.Models;
using AwsTestingSolution.Storages;

namespace AwsTestingSolution.ApiClients.Notification
{
    public class NotificationApiClient : ApiClientBase
    {
        public NotificationGetModel[] GetEmailSubscriptionsToSnsTopic()
        {
            return ExecuteGet<NotificationGetModel[]>(CloudximageDataStorage.AppInstancePublicDns + "/api/notification");
        }

        public string SubscribeEmailToSnsTopic(string emailAddress)
        {
            return ExecutePost<string>(CloudximageDataStorage.AppInstancePublicDns + $"/api/notification/{emailAddress}");
        }
    }
}
