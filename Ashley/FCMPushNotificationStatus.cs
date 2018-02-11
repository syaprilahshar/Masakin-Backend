using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masakin
{
    public class FCMPushNotificationStatus
    {
        public bool Successful
        {
            get;
            set;
        }

        public string Response
        {
            get;
            set;
        }
        public Exception Error
        {
            get;
            set;
        }
    }
}