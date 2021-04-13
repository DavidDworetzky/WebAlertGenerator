using System;
namespace AppSentinel.Core.Models
{
    /// <summary>
    /// Wrapper object for app sentinel settings -> web alert and notification
    /// </summary>
    public class AppSentinelSettings
    {
        public NotificationSettings NotificationSettings { get; set; }
        public WebAlertSettings WebAlertSettings { get; set; }

        public AppSentinelSettings(NotificationSettings notificationSettings, WebAlertSettings webAlertSettings)
        {
            NotificationSettings = notificationSettings;
            WebAlertSettings = webAlertSettings;
        }

        public AppSentinelSettings()
        {

        }
    }
}
