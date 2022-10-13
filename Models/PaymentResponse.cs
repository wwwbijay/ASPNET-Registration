namespace EventRegistration.Models
{
    public class PaymentResponse
    {
        public string MerchantTransactionId { get; set; }
        public string RedirectURL { get; set; }
        public string Message { get; set; }
        public string ResponseMessage { get; set; }
        public string Details { get; set; }
        public int ReponseCode { get; set; }
        public bool Status { get; set; }
        public string IosVersion { get; set; }
        public string AndroidVersion { get; set; }
    }
}
