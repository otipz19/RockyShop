using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }

        [ValidateNever]
        public IEnumerable<OrderDetails> OrderDetailsList { get; set; }
    }
}
