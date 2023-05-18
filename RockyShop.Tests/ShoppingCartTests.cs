using NuGet.Packaging;
using RockyShop.Model.Models;
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
            cart.Items.AddRange(new int[] { 1, 2, 3, 4 });
            string json = JsonSerializer.Serialize(cart);
            ShoppingCart deserializedCart = JsonSerializer.Deserialize<ShoppingCart>(json);
            Assert.AreEqual(deserializedCart.Items.Count, cart.Items.Count);
            for (int i = 0; i < cart.Items.Count; i++)
            {
                Assert.AreEqual(deserializedCart.Items[i], cart.Items[i]);
            }
        }
    }
}