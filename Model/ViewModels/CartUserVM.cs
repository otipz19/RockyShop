﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RockyShop.Model.Models;

namespace RockyShop.Model.ViewModels
{
    public class CartUserVM
    {
        public AppUser User { get; set; }

        [ValidateNever]
        public IList<ProductInCart> ProductInCartList { get; set; } = new List<ProductInCart>();
    }
}
