namespace RockyShop.Models.ViewModels
{
    public class HomeDetailsVM
    {
        public Product Product { get; set; } = new Product();

        public bool ExistsInCart { get; set; }
    }
}
