using Braintree;
using Microsoft.Extensions.Options;
using RockyShop.Model.Enums;
using RockyShop.Model.Models;
using RockyShop.Utility.Interfaces;
using RockyShop.Utility.Utilities;

namespace RockyShop.Utility.Services
{
    public class BraintreeService : IBraintreeService
    {
        private readonly BraintreeSettings _settings;
        private readonly IBraintreeGateway _braintreeGateway;

        public BraintreeService(IOptions<BraintreeSettings> options)
        {
            _settings = options.Value;
            _braintreeGateway = CreateGetaway();
        }

        public async Task<string> GetClientTokenAsync()
        {
            return await _braintreeGateway.ClientToken.GenerateAsync();
        }

        public async Task ProcessTransactionAsync(OrderHeader orderHeader, string nonce)
        {
            var request = new TransactionRequest
            {
                Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                PaymentMethodNonce = nonce,
                OrderId = orderHeader.Id.ToString(),
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = await _braintreeGateway.Transaction.SaleAsync(request);

            try
            {
                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.OrderStatus = OrderStatus.Approved;
                    orderHeader.TransactionId = result.Target.Id;
                }
                else
                {
                    orderHeader.OrderStatus = OrderStatus.Cancelled;
                }
            }
            catch
            {
                orderHeader.OrderStatus = OrderStatus.Cancelled;
            }
        }

        private IBraintreeGateway CreateGetaway()
        {
            return new BraintreeGateway(_settings.Environment, _settings.MerchantId, _settings.PublicKey, _settings.PrivateKey);
        }
    }
}
