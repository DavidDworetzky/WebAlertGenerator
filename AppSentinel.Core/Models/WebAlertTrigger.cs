using System;
using System.Collections.Generic;

namespace AppSentinel.Core.Models
{
    public class WebAlertTrigger
    {
        public List<int> StatusRange { get; set; }
        public bool CheckSuccessful { get; set; }
        public string Regex { get; set; }
        public WebAlertType AlertType { get; set; }
        public WebAlertTrigger(List<int> statusRange, bool checkSuccessful, string regex, WebAlertType alertType)
        {
            StatusRange = statusRange;
            CheckSuccessful = checkSuccessful;
            Regex = regex;
            AlertType = alertType;
        }
    }
}
