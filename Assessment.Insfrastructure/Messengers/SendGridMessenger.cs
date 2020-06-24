using Assessment.Domain;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Assessment.Infrastructure.Messengers
{


    /// <summary>
    ///This calls sends emails via the SendGrid service. The Html content is the same as the content.
    ///Please configure the email address to send to in the appsettings file in the correspondence service project
    /// </summary>
    public class SendGridMessenger : IMessenger
    {
        private readonly string _apiKey;
        private readonly IConfiguration _configuration;

        public SendGridMessenger(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["SendGridApiKey"];
        }

        /// <summary>
        /// Sends and email asynchronously 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="from"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<bool> SendMessageAsync(string to, string from, string subject, string message)
        {
            var client = new SendGridClient(_apiKey);
            var fromAddress = new EmailAddress(from);
            var toAddress = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, message, message);
            try
            {
                var response = await client.SendEmailAsync(msg);
                return response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
