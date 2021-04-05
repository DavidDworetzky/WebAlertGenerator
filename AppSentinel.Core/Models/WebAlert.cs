using System;
namespace AppSentinel.Core.Models
{
    public class WebAlert
    {
        public string Description { get; set; }
        public WebAlertType AlertType { get; set; }
        public WebAlert(string description, WebAlertType alertType)
        {
            Description = description;
            AlertType = alertType;
        }
    }
}
