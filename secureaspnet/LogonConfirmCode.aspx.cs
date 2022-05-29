using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Google.Authenticator;
using System.Data;
using System.Data.SqlClient;
using System.Web.Security;
using System.Configuration;

namespace secureaspnet
{
    public partial class LogonConfirmCode : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["key"]))
            {
                Response.Redirect("~/Logon.aspx");
            }

            this.lblSecretKey.Text = Request.QueryString["key"];


            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode(
                "secureasp", 
                Request.QueryString["user"] + "@actvn.edu.vn", 
                Request.QueryString["key"], 
                false, 
                3);

            string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            string manualEntrySetupCode = setupInfo.ManualEntryKey;

            this.imgQrCode.ImageUrl = qrCodeImageUrl;
            this.lblManualSetupCode.Text = manualEntrySetupCode;
        }

        private string GetRoleFromGUIDUser(string id)
        {
            string role = "";
            string sqlStr;
            string connStringDB;

            SqlConnection connectionObj;
            SqlCommand command;
            SqlDataReader reader;

            sqlStr = "SELECT role FROM roles WHERE " +
                            "id = @id ";

            connStringDB = ConfigurationManager.ConnectionStrings[1].ToString();
            connectionObj = new SqlConnection(connStringDB);
            connectionObj.Open();

            command = new SqlCommand(sqlStr, connectionObj);
            command.Parameters.Add(new SqlParameter("@id", id));

            reader = command.ExecuteReader();

            if (reader.Read())
                role = Convert.ToString(reader["role"]);

            connectionObj.Close();

            System.Diagnostics.Debug.WriteLine(role);

            return role;
        }


        protected void btnValidate_Click(object sender, EventArgs e)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var result = tfa.ValidateTwoFactorPIN(Request.QueryString["key"], this.txtCode.Text);
            System.Diagnostics.Debug.WriteLine(Request.QueryString["key"]);
            if (result)
            {
                this.lblValidationResult.Text = this.txtCode.Text + " OK " + DateTime.UtcNow.ToString();
                this.lblValidationResult.ForeColor = System.Drawing.Color.Green;

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    Request.QueryString["user"],
                    DateTime.Now,
                    DateTime.Now.AddMinutes(2880),
                    true,
                    GetRoleFromGUIDUser(Request.QueryString["key"]),
                    FormsAuthentication.FormsCookiePath);

                string hash = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

                if (ticket.IsPersistent)
                {
                    cookie.Expires = ticket.Expiration;
                }

                FormsAuthentication.SetAuthCookie(Request.QueryString["user"], true);

                Response.Cookies.Add(cookie);
                Response.Redirect("~/Admin/Manager.aspx", true);
            }
            else
            {
                this.lblValidationResult.Text = this.txtCode.Text + " ............. " + DateTime.UtcNow.ToString();
                this.lblValidationResult.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}