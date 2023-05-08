using NuGet.Packaging;
using RockyShop.Models;
using System.Text.Json;

namespace RockyShop.Tests
{
    [TestClass]
    public class ShoppingCartTests
    {
        [TestMethod]
        public void JsonSerialize()
        {
            var cart = new ShoppingCart();
            cart.ProductsId.AddRange(new int[] { 1, 2, 3, 4 });
            string json = JsonSerializer.Serialize(cart);
            ShoppingCart deserializedCart = JsonSerializer.Deserialize<ShoppingCart>(json);
            Assert.AreEqual(deserializedCart.ProductsId.Count, cart.ProductsId.Count);
            for (int i = 0; i < cart.ProductsId.Count; i++)
            {
                Assert.AreEqual(deserializedCart.ProductsId[i], cart.ProductsId[i]);
            }
        }
    }
}