using AwsTestingSolution.ApiClients.Notification.Models;
using AwsTestingSolution.Storages;
using System.Net.Mail;

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

        public string UnsubscribeEmailFromSns(string emailAddress)
        {
            return ExecuteDelete(CloudximageDataStorage.AppInstancePublicDns + $"/api/notification/{emailAddress}");
        }
    }
}
