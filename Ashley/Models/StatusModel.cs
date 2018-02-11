using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masakin.Models
{
    public class StatusModel
    {
        public const string Expired = "Expired";
        public const string Pending = "Pending";
        public const string Reject = "Reject";
        public const string PaymentConfirm = "Payment Confirmation";
        public const string Paid = "Paid";
        public const string OnProcess = "On Process";
        public const string Send = "Send";
        public const string Sent = "Sent";
        public const string Done = "Done";
    }
}