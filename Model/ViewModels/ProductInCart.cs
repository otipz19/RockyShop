﻿using RockyShop.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace RockyShop.Model.ViewModels
{
    public class ProductInCart
    {
        public Models.Product Product { get; set; } = new Models.Product();

        public bool ExistsInCart { get; set; }

        [Range(1, 10000)]
        public int SqFt { get; set; } = 1;
    }
}
