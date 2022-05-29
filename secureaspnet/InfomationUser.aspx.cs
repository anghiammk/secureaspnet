using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Security;
using System.Configuration;
using System.Data.SqlClient;

namespace secureaspnet
{
    public partial class InformationUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (!System.Web.HttpContext.Current.User.Identity.IsAuthenticated 
                || string.IsNullOrEmpty(User.Identity.Name) 
                || string.IsNullOrEmpty(Request.QueryString["id"]))
            {
                Response.Redirect("~/Logon.aspx");
            }

            string sqlStr = String.Empty;
            string connStringDB;

            SqlConnection connectionObj;
            SqlCommand command;
            SqlDataReader reader;

            connStringDB = ConfigurationManager.ConnectionStrings[1].ToString();
            connectionObj = new SqlConnection(connStringDB);
            connectionObj.Open();

            sqlStr = "SELECT * FROM info WHERE " +
                           "id = @id ";

            command = new SqlCommand(sqlStr, connectionObj);
            command.Parameters.Add(new SqlParameter("@id", Request.QueryString["id"]));

            reader = command.ExecuteReader();
            if (reader.Read())
            {
                name.Text    = reader["name"].ToString();
                age.Text     = reader["age"].ToString();
                time.Text    = reader["time_create"].ToString();
                address.Text = reader["address"].ToString();
                phone.Text   = reader["phone"].ToString();
            }
            connectionObj.Close();
        }

        protected void Logout(object sender, EventArgs e)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (val1)
            {
                FormsAuthentication.SignOut();
                Response.Redirect("~/Logon.aspx");
            }
        }
    }
}