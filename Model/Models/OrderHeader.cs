using RockyShop.Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace RockyShop.Model.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        [Required]
        public string CreatedByUserId { get; set; }

        public virtual AppUser CreatedByUser { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime? ShippingDate { get; set; }

        public double FinalOrderTotal { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime? PaymentDate { get; set; }

        //public DateTime PaymentDueDate { get; set; }

        public string TransactionId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string PoastalCode { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }
    }
}
