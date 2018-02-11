using Masakin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Masakin
{
    public partial class Login : System.Web.UI.Page
    {
        Common cm = new Common();

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        //protected void btnLogin_Click(object sender, EventArgs e)
        //{
        //    if (txtUsername.Text != "" && txtPassword.Text != "")
        //    {
        //        bool checkLogin = cm.checkLogin(txtUsername.Text, txtPassword.Text);
        //        if (checkLogin)
        //        {
        //            checkRole();
        //            //Master.checkRole();
        //        }
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Nomor HP dan Password wajib diisi')", true);
        //    }
        //}

        public void checkRole()
        {
            string xxx = SessionLib.Current.Role;
            //if (xxx == Models.Roles.Admin) { Response.Redirect("~/Default.aspx"); }
            //if (xxx == Models.Roles.Agent) { Response.Redirect("~/Login.aspx"); }
        }
    }
}