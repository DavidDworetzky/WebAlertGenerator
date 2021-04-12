using System;
namespace AppSentinel.Core.Models
{
    /// <summary>
    /// Wrapper object for app sentinel settings -> web alert and notification
    /// </summary>
    public class AppSentinelSettings
    {
        public readonly NotificationSettings NotificationSettings;
        public readonly WebAlertSettings WebAlertSettings;

        public AppSentinelSettings(NotificationSettings notificationSettings, WebAlertSettings webAlertSettings)
        {
            NotificationSettings = notificationSettings;
            WebAlertSettings = webAlertSettings;
        }
    }
}
