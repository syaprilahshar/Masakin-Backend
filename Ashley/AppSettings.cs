using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masakin
{
    public class AppSettings
    {
        public const string ApplicationUrl = @"http://api.decodeideas.com";
        //public const string ApplicationUrl = @"http://Masakin.decodeideas.com";
        public const string IntroVideoUrl = ApplicationUrl + "/videos/intro.mp4";
        public const string TutorialVideoUrl = ApplicationUrl + "/videos/tutorial.mp4";
        ////public const string MasakinAreaUrl = AdminDashboardUrl + "/Masakin";
        //public const string CallCenterStaffInfoUrl = MasakinAreaUrl + "/CallCenterStaff/GetStaffInfo";
        //public const string SoftPhoneTokenUrl = MasakinAreaUrl + "/SoftPhone/Token";
    }
}