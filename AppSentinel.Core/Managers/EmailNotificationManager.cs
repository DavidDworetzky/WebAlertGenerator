using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Linq;

namespace AppSentinel.Core.Managers
{
    public class EmailNotificationManager
    {
        private readonly string SendGridKey;
        private readonly string FromAddress;
        private readonly string FromName;

        public EmailNotificationManager(string sendGridKey, string fromAddress, string fromName)
        {
            SendGridKey = sendGridKey;
            FromAddress = fromAddress;
            FromName = fromName;
        }

        /// <summary>
        /// Notifies a simple plain message to a set of email targets with SendGrid
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="message"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        public async Task NotifyMessageToTargets(IList<string> targets, string message, string htmlMessage, string subject = "")
        {
            var client = new SendGridClient(SendGridKey);
            var from = new EmailAddress(FromAddress, FromName);
            var toEmails = targets.Select(t => new EmailAddress(t));

            //notify targets of 
            foreach (var target in toEmails)
            {
                var msg = MailHelper.CreateSingleEmail(from, target, subject, message, htmlMessage);
                var response = await client.SendEmailAsync(msg);
            }

        }
    }
}
