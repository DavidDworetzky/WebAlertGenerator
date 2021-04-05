using System;
using System.Linq;
using System.Collections.Generic;

namespace AppSentinel.Console
{
    public static class AppSentinelConsole
    {
        public static string SendGridApiKey = "";
        public static string FromEmail = "";
        public static string From = "";
        public static string SendGridTargets = "";
        public static string TriggerUrls = "";
        public static string TriggerValues = "";

        /// <summary>
        /// FillValue is used for setting values that we only need initializations for
        /// </summary>
        /// <param name="key"></param>
        /// <param name="current"></param>
        /// <param name="valueSet"></param>
        private static void FillValue(string key, ref string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                System.Console.WriteLine($"Enter a value for {key}");
                value = System.Console.ReadLine();
            }
        }

        public static void TestWebAlertGenerator()
        {
            //Fill Initial Values
            FillValue("SendGridApiKey", ref SendGridApiKey);
            FillValue("FromEmail", ref FromEmail);
            FillValue("From", ref From);
            FillValue("SendGridTargets", ref SendGridTargets);
            FillValue("TriggerUrls", ref TriggerUrls);
            FillValue("TriggerValues", ref TriggerValues);

            var splitUrls = TriggerUrls.Split(',');
            var splitTriggers = TriggerValues.Split(';');


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

            var splitTargets = SendGridTargets.Split(',').ToList();

            var emailNotificationManager = new Core.Managers.EmailNotificationManager(SendGridApiKey, From, FromEmail);
            foreach (var alert in alerts)
            {
                emailNotificationManager.NotifyMessageToTargets(splitTargets, alert.Description, $"<strong>{alert.Description}</strong>", $"App Sentinel Alert from {From}");
            }
        }

        public static void TestSimpleAlert()
        {
            //Fill Initial Values
            FillValue("SendGridApiKey", ref SendGridApiKey);
            FillValue("From Email", ref FromEmail);
            FillValue("From", ref From);
            FillValue("SendGridTargets", ref SendGridTargets);

            var splitTargets = SendGridTargets.Split(',').ToList();
            var emailNotificationManager = new Core.Managers.EmailNotificationManager(SendGridApiKey, From, FromEmail);
            emailNotificationManager.NotifyMessageToTargets(splitTargets, "Test Notification", $"<strong>Test Notification</strong>", $"App Sentinel Alert from {From}");

        }
    }
}
