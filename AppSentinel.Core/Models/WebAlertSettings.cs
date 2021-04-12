using System;
using System.Collections.Generic;
using System.Linq;

namespace AppSentinel.Core.Models
{
    public class WebAlertSettings
    {
        public readonly IEnumerable<string> Urls;
        public readonly IEnumerable<WebAlertTrigger> Triggers;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="triggers"></param>
        public WebAlertSettings(IEnumerable<string> urls, IEnumerable<WebAlertTrigger> triggers)
        {
            Urls = urls;
            Triggers = triggers;
        }
        /// <summary>
        /// Split Constructor
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="triggers"></param>
        /// <param name="separator1"></param>
        /// <param name="separator2"></param>
        public WebAlertSettings(string urls, string triggers, string separator1 = ",", string separator2= ";")
        {
            Urls = urls.Split(separator1).ToList();
            var splitTriggers = triggers.Split(separator2).ToList();
            Triggers = splitTriggers.Select((val) =>
            {
                var splitValues = val.Split(separator1);
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
        }
    }
}
