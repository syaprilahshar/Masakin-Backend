//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Masakin
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblT_Log_Admin
    {
        public long ID { get; set; }
        public Nullable<long> AdminID { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
    }
}
