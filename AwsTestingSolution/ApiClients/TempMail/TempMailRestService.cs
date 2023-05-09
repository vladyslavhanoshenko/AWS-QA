namespace AwsTestingSolution.ApiClients.TempMail
{
    public class TempMailRestService : ApiClientBase
    {
        private string BaseUrl = "https://rapidapi.p.rapidapi.com/request";

        public TempMailRestService()
        {
            Client.Headers.Add("x-rapidapi-host", Environment.GetEnvironmentVariable("x-rapidapi-host"));
            Client.Headers.Add("x-rapidapi-key", Environment.GetEnvironmentVariable("x-rapidapi-key"));
        }

        public string[] GetDomainsList()
        {
            return ExecuteGet<string[]>(BaseUrl + "/domains/");
        }

        public string GetEmails(string md5Hash)
        {
            return ExecuteGet(BaseUrl + $"/mail/id/{md5Hash}/");
        }
    }
}
