using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using RockyShop.Model.ViewModels;
using System.Text;
using RockyShop.Utility.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace RockyShop.Utility.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly MailjetClient _mailjetClient;

        private readonly string _apiKeyPublic;
        private readonly string _apiKeyPrivate;
        private readonly string _senderEmail;

        public EmailSenderService(IWebHostEnvironment webHostEnvironment)
        {
            _apiKeyPublic = Environment.GetEnvironmentVariable("$MJ_APIKEY_PUBLIC");
            _apiKeyPrivate = Environment.GetEnvironmentVariable("$MJ_APIKEY_PRIVATE");
            _senderEmail = Environment.GetEnvironmentVariable("$MJ_SENDER_EMAIL");

            _mailjetClient = new MailjetClient(_apiKeyPublic, _apiKeyPrivate);

            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Sends email via Mailjet API
        /// </summary>
        /// <param name="email">Recipient email</param>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
                .Property(Send.FromEmail, _senderEmail)
                .Property(Send.FromName, "Rocky shop")
                .Property(Send.Subject, subject)
                .Property(Send.HtmlPart, htmlMessage)
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {"Email", email}
                    }});

            MailjetResponse response = await _mailjetClient.PostAsync(request);
        }

        public async Task SendInquiryConfirmationEmailAsync(CartUserVM data)
        {
            string template = await ReadTemplate();
            string HtmlBody = InputDataInTemplate(data, template);
            //_senderEmail because this email is sent to admin of website
            await SendEmailAsync(_senderEmail, "Inquiry Confirmation", HtmlBody);
        }

        private async Task<string> ReadTemplate()
        {
            string pathToTemplate = Path.Join(_webHostEnvironment.WebRootPath, "templates", "Inquiry.html");
            string content;
            using (StreamReader sr = File.OpenText(pathToTemplate))
            {
                content = await sr.ReadToEndAsync();
            }
            return content;
        }

        private string InputDataInTemplate(CartUserVM data, string template)
        {
            var productsStringBuilder = new StringBuilder();
            foreach(var p in data.Products)
            {
                productsStringBuilder.Append($" - Name: {p.Name} <span style= 'font-size:14px;'>(ID: {p.Id})</span><br/>");
            }
            //Name  : {0}
            //Email : {1}
            //Phone : {2}
            //Products: {3}
            return String.Format(template,
                data.User.FullName,
                data.User.Email,
                data.User.PhoneNumber,
                productsStringBuilder.ToString());
        }
    }
}
