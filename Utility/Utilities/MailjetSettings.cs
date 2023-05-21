namespace RockyShop.Utility.Utilities
{
    public class MailjetSettings
    {
        public const string Section = "Mailjet";

        public string ApiKeyPublic { get; set; }

        public string ApiKeyPrivate { get; set; }

        public string SenderEmail { get; set; }
    }
}