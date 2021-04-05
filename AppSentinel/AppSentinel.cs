using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private const string Heartbeat = "TriggerHeartbeat";

        private const string SendGridKey = "SendGridKey";
        private const string SendGridFromEmail = "SendGridFromEmail";
        private const string SendGridFrom = "SendGridFrom";
        private const string SendGridTargets = "SendGridTo";

        private const string HeartbeatEnabled = "AppSentinelHeartbeat";

        #endregion

        [FunctionName("AppSentinel")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            //deserialize settings for urls and triggers for our WebAlertGenerator.
            //url
            var urls = System.Environment.GetEnvironmentVariable(Urls, EnvironmentVariableTarget.Process);
            var splitUrls = urls.Split(',');
            //webalerttriggers
            var triggers = System.Environment.GetEnvironmentVariable(Triggers, EnvironmentVariableTarget.Process);
            var splitTriggers = triggers.Split(';');
            //now, split again on comma separation and create WebAlertTrigger
            //alert format is:
            //200,299,true,,HttpFailure
            var webAlerts = splitTriggers.Select((val) =>
            {
                var splitValues = val.Split(',');
                var statusRange1 = Int32.Parse(splitValues[0]);
                var statusRange2 = Int32.Parse(splitValues[1]);
                var statusRange = new List<int>() { statusRange1, statusRange2 };
                var checkSuccessful = bool.Parse(splitValues[2]);
                var regex = splitValues[3];
                Core.Models.WebAlertType webAlertType;
                var webAlertTypeParse = Enum.TryParse<Core.Models.WebAlertType>(splitValues[4], out webAlertType);
                var webAlert = new Core.Models.WebAlertTrigger(statusRange, checkSuccessful, regex, webAlertType);
                return webAlert;
            }).ToList();
            //generate alerts
            var webAlertGenerator = new Core.Managers.WebAlertGenerator(splitUrls.ToList(), webAlerts);
            var alerts = webAlertGenerator.GenerateAlerts() as IEnumerable<Core.Models.WebAlert>;

            //get sendgrid environment variables
            var sendGridKey = System.Environment.GetEnvironmentVariable(SendGridKey, EnvironmentVariableTarget.Process);
            var sendGridFromEmail = System.Environment.GetEnvironmentVariable(SendGridFromEmail, EnvironmentVariableTarget.Process);
            var sendGridFrom = System.Environment.GetEnvironmentVariable(SendGridFrom, EnvironmentVariableTarget.Process);
            var sendGridTargets = System.Environment.GetEnvironmentVariable(SendGridTargets, EnvironmentVariableTarget.Process).Split(',').ToList();
            //notify alerts via EmailNotificationManager
            var emailNotificationManager = new Core.Managers.EmailNotificationManager(sendGridKey, sendGridFrom, sendGridFromEmail);
            foreach(var alert in alerts)
            {
                await emailNotificationManager.NotifyMessageToTargets(sendGridTargets, alert.Description, $"<strong>{alert.Description}</strong>", $"App Sentinel Alert from {sendGridFrom}");
            }

            //heartbeat
            var hearbeat = System.Environment.GetEnvironmentVariable(HeartbeatEnabled, EnvironmentVariableTarget.Process);
            var doHeartbeat = Boolean.Parse(Heartbeat);
            if(doHeartbeat)
            {
                var heartbeatContent = $"AppSentinel checked URLs for errors at {DateTime.Now}. Checked URLs: {urls}";
                await emailNotificationManager.NotifyMessageToTargets(sendGridTargets, heartbeatContent, $"<strong>{heartbeatContent}</strong>", "AppSentinel Check Notification");
            }


            log.LogInformation($"App Sentinel executed at : {DateTime.Now}");
        }
    }
}
