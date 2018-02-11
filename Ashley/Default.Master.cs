using Masakin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Masakin
{
    public partial class Default : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SessionLib.Current.ID == String.Empty) { Response.Redirect("~/Login.aspx"); }
        }

        protected void lbtLogout_Click(object sender, EventArgs e)
        {
            //SessionLib.Current.AdminID = string.Empty;
            //SessionLib.Current.Name = string.Empty;
            //SessionLib.Current.Email = string.Empty;
            //SessionLib.Current.HP = string.Empty;
            //SessionLib.Current.Role = string.Empty;
            //Response.Redirect("~/Login");
        }
    }
}