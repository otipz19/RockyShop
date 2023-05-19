using Braintree;
using RockyShop.Model.Models;
using RockyShop.Utility.Utilities;

namespace RockyShop.Utility.Interfaces
{
    public interface IBraintreeService
    {
        public Task<string> GetClientTokenAsync();

        public Task ProcessTransactionAsync(OrderHeader orderHeader, string nonce);
    }
}
