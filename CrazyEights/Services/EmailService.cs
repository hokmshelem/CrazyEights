using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using CrazyEights.Dtos.Account;

namespace CrazyEights.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(EmailSendDto model)
        {
            MailjetClient client = new MailjetClient(_config["MailJet:ApiKey"], _config["MailJet:SecretKey"]);

            // construct your email with builder
            var email = new TransactionalEmailBuilder()
                   .WithFrom(new SendContact(_config["Email:From"], _config["Email:ApplicationName"]))
                   .WithSubject(model.Subject)
                   .WithHtmlPart(model.Body)
                   .WithTo(new SendContact(model.To))
                   .Build();

            // invoke API to send email
            var response = await client.SendTransactionalEmailAsync(email);
            if (response.Messages != null)
            {
                if (response.Messages[0].Status == "success")
                {
                    return true;
                }
            }

            return false;
        }
    }
}
