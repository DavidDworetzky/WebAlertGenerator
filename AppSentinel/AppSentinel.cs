using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppSentinel.Core.Managers;

namespace AppSentinel
{
    /// <summary>
    /// Generates Alerts for 
    /// </summary>
    public static class AppSentinel
    {
        #region TriggerConstants
        private const string Urls = "TriggerUrls";
        private const string Triggers = "TriggerValues";

        private const string SendGridKey = "SendGridKey";
        private const string SendGridFromEmail = "SendGridFromEmail";
        private const string SendGridFrom = "SendGridFrom";
        private const string SendGridTargets = "SendGridTargets";

        private const string HeartbeatEnabled = "HeartbeatEnabled";

        #endregion

        private static Dictionary<string, string> GetSettingsValues()
        {
            Func<string, string> getSettings = (str) => System.Environment.GetEnvironmentVariable(str, EnvironmentVariableTarget.Process);
            return new Dictionary<string, string>()
            {
                {"TriggerUrls", getSettings(Urls) },
                {"TriggerValues", getSettings(Triggers) },
                {"SendGridKey", getSettings(SendGridKey) },
                {"SendGridFrom", getSettings(SendGridFrom) },
                {"SendGridTargets", getSettings(SendGridTargets) },
                {"SendGridFromEmail", getSettings(SendGridFromEmail) }
            };
        }

        [FunctionName("AppSentinel")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            //deserialize settings for urls and triggers for our WebAlertGenerator.
            var settings = GetSettingsValues();
            var settingsManager = new WebAlertSettingsManager(string.Empty);
            var sentinelSettings = settingsManager.LoadSettingsFromDictionary(settings);

            //generate alerts
            var webAlertGenerator = new Core.Managers.WebAlertGenerator(sentinelSettings.WebAlertSettings.Urls.ToList(), sentinelSettings.WebAlertSettings.Triggers.ToList());
            var alerts = await webAlertGenerator.GenerateAlerts();

            var alertSettings = sentinelSettings.WebAlertSettings;
            var notificationSettings = sentinelSettings.NotificationSettings;

            //notify alerts via EmailNotificationManager
            var emailNotificationManager = new Core.Managers.EmailNotificationManager(notificationSettings.SendGridApiKey, notificationSettings.FromEmail, notificationSettings.From);
            foreach(var alert in alerts)
            {
                await emailNotificationManager.NotifyMessageToTargets(notificationSettings.SendGridTargets.ToList(), alert.Description, $"<strong>{alert.Description}</strong>", $"App Sentinel Alert from {notificationSettings.From}");
            }

            //heartbeat
            var hearbeat = System.Environment.GetEnvironmentVariable(HeartbeatEnabled, EnvironmentVariableTarget.Process);
            var doHeartbeat = Boolean.Parse(hearbeat);
            if(doHeartbeat)
            {
                var urlAggregate = sentinelSettings.WebAlertSettings.Urls.Aggregate("", (x, y) => x +":" + y, (result) => result);
                var heartbeatContent = $"AppSentinel checked URLs for errors at {DateTime.Now}. Checked URLs: {urlAggregate}";
                await emailNotificationManager.NotifyMessageToTargets(notificationSettings.SendGridTargets.ToList(), heartbeatContent, $"<strong>{heartbeatContent}</strong>", "AppSentinel Check Notification");
            }


            log.LogInformation($"App Sentinel executed at : {DateTime.Now}");
        }
    }
}
