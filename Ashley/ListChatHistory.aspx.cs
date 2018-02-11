using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Masakin
{
    public partial class ListChatHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            int x = GridView1.Rows.Count;
            if (x != 0)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //Find the DropDownList in the Row
                    //DropDownList ddlCountries = (e.Row.FindControl("ddlShippingServices") as DropDownList);
                    //DataSet ss = gs.GetItemsMaster("tblM_Shipping_Services");
                    //ddlCountries.DataSource = ss;
                    //ddlCountries.DataTextField = "ShippingService";
                    //ddlCountries.DataValueField = "ShippingServiceID";
                    //ddlCountries.DataBind();

                }
            }
        }

        protected void btnViewChat_Click(object sender, EventArgs e)
        {

        }
    }
}