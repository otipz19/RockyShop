using Microsoft.AspNetCore.Identity.UI.Services;
using RockyShop.Models.ViewModels;

namespace RockyShop.Interfaces
{
    public interface IEmailSenderService : IEmailSender
    {
        public Task SendInquiryConfirmationEmailAsync(CartUserVM data);
    }
}
