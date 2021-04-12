using System;
using System.Linq;
using System.Collections.Generic;
using AppSentinel.Core.Models;
using AppSentinel.Core.Managers;

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

        private static AppSentinelSettings GetSettings()
        {
            var settingsDictionary = new Dictionary<string, string>() {
                { "SendGridKey", SendGridApiKey },
                { "SendGridFromEmail", FromEmail },
                { "SendGridFrom", From },
                { "SendGridTargets", SendGridTargets },
                { "TriggerUrls", TriggerUrls },
                { "TriggerValues", TriggerValues }
            };

            var settingsManager = new WebAlertSettingsManager("");

            var allSettings = settingsManager.LoadSettingsFromDictionary(settingsDictionary);
            return allSettings;
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

            var allSettings = GetSettings();

            //generate alerts
            var webAlertGenerator = new Core.Managers.WebAlertGenerator(allSettings.WebAlertSettings.Urls.ToList(), allSettings.WebAlertSettings.Triggers.ToList());
            var alerts = webAlertGenerator.GenerateAlerts() as IEnumerable<Core.Models.WebAlert>;

            var notificationSettings = allSettings.NotificationSettings;

            var emailNotificationManager = new Core.Managers.EmailNotificationManager(notificationSettings.SendGridApiKey, notificationSettings.FromEmail, notificationSettings.From);
            foreach (var alert in alerts)
            {
                emailNotificationManager.NotifyMessageToTargets(allSettings.NotificationSettings.SendGridTargets.ToList(), alert.Description, $"<strong>{alert.Description}</strong>", $"App Sentinel Alert from {notificationSettings.From}");
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
            var emailNotificationManager = new Core.Managers.EmailNotificationManager(SendGridApiKey, FromEmail, From);
            emailNotificationManager.NotifyMessageToTargets(splitTargets, "Test Notification", $"<strong>Test Notification</strong>", $"App Sentinel Alert from {From}");

        }
    }
}
