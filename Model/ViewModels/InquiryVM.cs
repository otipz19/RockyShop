using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class InquiryVM
    {
        public InquiryHeader InquiryHeader { get; set; }

        public IEnumerable<InquiryDetails> InquiryDetailsList { get; set; }
    }
}
