using System;

namespace Core_WebApp.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
