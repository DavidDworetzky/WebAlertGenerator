using System;
namespace AppSentinel.Models
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
