using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masakin.Models.Catering
{
    public class MenuShowUser
    {
        public long MenuID { get; set; }
        public long MerchantID { get; set; }
        public string MerchantName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public Nullable<double> Rate { get; set; }
        public int isActive { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string ImageURL { get; set; }
        public string ThumbnailURL { get; set; }
        public string MerchantImageURL { get; set; }
        public string MerchantThumbnailURL { get; set; }
        public string DeliveryLocation { get; set; }
        public string DateFirst { get; set; }
        public string DateLast { get; set; }
    }
}