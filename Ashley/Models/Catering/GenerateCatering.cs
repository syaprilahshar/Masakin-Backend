using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masakin.Models.Catering
{
    public class GenerateCatering
    {
        public int MerchantID { get; set; }
        public bool Week1 { get; set; }
        public DateTime Week1DateFirst { get; set; }
        public DateTime Week1DateLast { get; set; }
        public bool Week2 { get; set; }
        public DateTime Week2DateFirst { get; set; }
        public DateTime Week2DateLast { get; set; }
        public bool Week3 { get; set; }
        public DateTime Week3DateFirst { get; set; }
        public DateTime Week3DateLast { get; set; }
        public tblT_Catering CateringWeek1 { get; set; }
        public tblT_Catering CateringWeek2 { get; set; }
        public tblT_Catering CateringWeek3 { get; set; }
    }
}