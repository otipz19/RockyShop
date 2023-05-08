namespace RockyShop.Models.ViewModels
{
    public class HomeIndexVM
    {
        public IEnumerable<Product> Products { get; set; }
        
        public IEnumerable<Category> Categories { get; set; }
    }
}
