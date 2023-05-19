namespace RockyShop.Model.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public int OrderHeaderId { get; set; }

        public virtual OrderHeader OrderHeader { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int SqFt { get; set; }

        public double PricePerSqFt { get; set; }
    }
}
