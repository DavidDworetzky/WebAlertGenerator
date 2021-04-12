using System;
using AppSentinel.Core.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace AppSentinel.Core.Managers
{

    public class WebAlertSettingsManager
    {
        private const string AppSentinelSettingsFileName = "AppSentinel.json";
        private readonly string _settingsPath;
        public WebAlertSettingsManager(string settingsPath)
        {
            _settingsPath = settingsPath;
        }

        /// <summary>
        /// Load AppSentinel Settings from disk
        /// </summary>
        /// <returns></returns>
        public async Task<AppSentinelSettings> LoadSettingsFromDisk()
        {
            var current = Directory.GetCurrentDirectory();
            //current path + relative path of settings
            var relative = Path.Join(current, _settingsPath);
            var file = Path.Join(relative, AppSentinelSettingsFileName);
            //if exists, deserialize our AppSentinelSettings model and return
            if(File.Exists(file))
            {
                var contents = await File.ReadAllTextAsync(file);
                var AppSentinelSettings = JsonConvert.DeserializeObject<AppSentinelSettings>(contents);
                return AppSentinelSettings;
            }
            return null;
        }

        /// <summary>
        /// Persist AppSentinelSettings back to disk
        /// </summary>
        /// <param name="settings"></param>
        public async Task WriteSettingsToDisk(AppSentinelSettings settings)
        {
            var current = Directory.GetCurrentDirectory();
            //current path + relative path of settings
            var relative = Path.Join(current, _settingsPath);
            var file = Path.Join(relative, AppSentinelSettingsFileName);

            var content = JsonConvert.SerializeObject(settings);
            await File.WriteAllTextAsync(file, content);
            return;
        }

        public AppSentinelSettings LoadSettingsFromDictionary(Dictionary<string, string> dict)
        {
            //create notificationSettings from dictionary
            var sendGridApikey = dict["SendGridKey"];
            var fromEmail = dict["SendGridFromEmail"];
            var fromName = dict["SendGridFrom"];
            var targets = dict["SendGridTargets"];
            var notificationSettings = new NotificationSettings(sendGridApikey, fromEmail, fromName, targets);
            //create WebAlertSettings from dictionary

            var urls = dict["TriggerUrls"];
            var triggerValues = dict["TriggerValues"];
            var webAlertSettings = new WebAlertSettings(urls, triggerValues);

            //return AppSentinelSettings object
            return new AppSentinelSettings(notificationSettings, webAlertSettings);
        }
    }
}
