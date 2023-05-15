using System.ComponentModel.DataAnnotations;

namespace RockyShop.Model.Models
{
    public class InquiryDetails
    {
        [Key]
        public int Id { get; set; }

        public int InquiryHeaderId { get; set; }

        public virtual InquiryHeader InquiryHeader { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
