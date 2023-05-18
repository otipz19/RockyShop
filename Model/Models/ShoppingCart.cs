namespace RockyShop.Model.Models
{
    public class ShoppingCart
    {
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();

        public int InquiryId { get; set; }
    }
}
