namespace RockyShop.Utility.Utilities
{
    public class BraintreeSettings
    {
        public const string Section = "Braintree";

        public string Environment { get; set; }

        public string MerchantId { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }
    }
}
