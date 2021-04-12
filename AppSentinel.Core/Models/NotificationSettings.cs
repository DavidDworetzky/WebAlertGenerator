using System;
using System.Collections.Generic;
using System.Linq;

namespace AppSentinel.Core.Models
{
    /// <summary>
    /// Notification Manager Settings for notifying of web alerts in app sentinel
    /// Settings related to SendGrid Api Key and from/  to settings
    /// </summary>
    public class NotificationSettings
    {
        public readonly string SendGridApiKey;
        public readonly string FromEmail;
        public readonly string From;
        public readonly IEnumerable<string> SendGridTargets;

        /// <summary>
        /// Pre-split constructor
        /// </summary>
        /// <param name="sendGridApiKey"></param>
        /// <param name="fromEmail"></param>
        /// <param name="from"></param>
        /// <param name="sendGridTargets"></param>
        public NotificationSettings(string sendGridApiKey, string fromEmail, string from, IEnumerable<string> sendGridTargets)
        {
            SendGridApiKey = sendGridApiKey;
            FromEmail = fromEmail;
            From = from;
            SendGridTargets = sendGridTargets;
        }

        /// <summary>
        /// Split constructor
        /// </summary>
        /// <param name="sendGridApiKey"></param>
        /// <param name="fromEmail"></param>
        /// <param name="from"></param>
        /// <param name="sendGridTargets"></param>
        /// <param name="separator"></param>
        public NotificationSettings(string sendGridApiKey, string fromEmail, string from, string sendGridTargets, string separator = ",")
        {
            SendGridApiKey = sendGridApiKey;
            FromEmail = fromEmail;
            From = from;
            SendGridTargets = sendGridTargets.Split(separator).ToList();
        }
    }
}
