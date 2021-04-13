using System;
using System.Collections.Generic;
using System.Linq;
using AppSentinel.Core.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;

namespace AppSentinel.Core.Managers
{
    public class WebAlertGenerator
    {
        private readonly IList<string> Urls;
        private readonly IList<WebAlertTrigger> AlertTriggers;

        public WebAlertGenerator(List<string> urls, List<WebAlertTrigger> alertTriggers)
        {
            Urls = urls;
            AlertTriggers = alertTriggers;
            if (AlertTriggers.Count != Urls.Count)
            {
                throw new System.ArgumentOutOfRangeException($"Alert Triggers and Url parameter sizes do not match in WebAlertGenerator");
            }
        }

        public async Task<IList<WebAlert>> GenerateAlerts()
        {
            var alerts = new List<WebAlert>();
            var alertsEnumerator = AlertTriggers.GetEnumerator();
            //cardinality of alerts should match trigger urls
            alertsEnumerator.MoveNext();
            foreach (var url in Urls)
            {
                var currentAlert = alertsEnumerator.Current;
                using (var client = new HttpClient() { BaseAddress = new Uri(url) })
                {
                    // grab content from url
                    var result = await client.GetAsync("");
                    var content = await result.Content.ReadAsStringAsync();

                    // check if boolean condition
                    if (currentAlert.CheckSuccessful && !result.IsSuccessStatusCode)
                    {
                        alerts.Add(new WebAlert($"Alert!!! Failed to get content at : {url}", WebAlertType.HttpFailure));
                        break;
                    }
                    // now, check if range trigger
                    if (currentAlert.StatusRange.Count > 0)
                    {
                        var lower = currentAlert.StatusRange[0];
                        var upper = currentAlert.StatusRange[1];
                        if (!((int)result.StatusCode <= upper && (int)result.StatusCode >= lower))
                        {
                            alerts.Add(new WebAlert($"Alert!!! Status code of response was : {result.StatusCode} which is not between {lower} and {upper}.", WebAlertType.HttpFailure));
                            break;
                        }
                    }

                    // finally, check for regex match
                    if (!string.IsNullOrEmpty(currentAlert.Regex) && Regex.Matches(content, currentAlert.Regex).Count == 0)
                    {
                        alerts.Add(new WebAlert($"Alert!!! Could not match pattern {currentAlert.Regex} at Url: {url}.", WebAlertType.ContentFailure));
                        break;
                    }
                }
                alertsEnumerator.MoveNext();
            }
            return alerts;
        }
    }
}
