using Newtonsoft.Json;

namespace AwsTestingSolution.ApiClients.TempMail.Models
{
    public class EmailModel
    {
        [JsonProperty("_id")]
        public Id Id { get; set; }

        [JsonProperty("createdAt")]
        public CreatedAt CreatedAt { get; set; }

        [JsonProperty("mail_id")]
        public string MailId { get; set; }

        [JsonProperty("mail_address_id")]
        public string MailAddressId { get; set; }

        [JsonProperty("mail_from")]
        public string MailFrom { get; set; }

        [JsonProperty("mail_subject")]
        public string MailSubject { get; set; }

        [JsonProperty("mail_preview")]
        public string MailPreview { get; set; }

        [JsonProperty("mail_text_only")]
        public string MailTextOnly { get; set; }

        [JsonProperty("mail_text")]
        public string MailText { get; set; }

        [JsonProperty("mail_html")]
        public string MailHtml { get; set; }

        [JsonProperty("mail_timestamp")]
        public double MailTimestamp { get; set; }

        [JsonProperty("mail_attachments_count")]
        public long MailAttachmentsCount { get; set; }

        [JsonProperty("mail_attachments")]
        public MailAttachments MailAttachments { get; set; }
    }

    public class CreatedAt
    {
        [JsonProperty("milliseconds")]
        public long Milliseconds { get; set; }
    }

    public class Id
    {
        [JsonProperty("oid")]
        public string Oid { get; set; }
    }

    public class MailAttachments
    {
        [JsonProperty("attachment")]
        public object[] Attachment { get; set; }
    }
}
