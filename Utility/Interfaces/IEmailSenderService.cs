using Microsoft.AspNetCore.Identity.UI.Services;
using RockyShop.Model.ViewModels;

namespace RockyShop.Utility.Interfaces
{
    public interface IEmailSenderService : IEmailSender
    {
        public Task SendInquiryConfirmationEmailAsync(CartUserVM data);
    }
}
