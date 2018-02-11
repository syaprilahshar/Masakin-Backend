using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masakin.Models.Catering
{
    public class SMSNotification
    {
        public int code { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public long msgid { get; set; }
    }
}