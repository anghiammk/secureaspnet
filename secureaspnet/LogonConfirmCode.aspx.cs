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
        private string key;
        private string user;

        private void GetValueFromURLEncrypt()
        {

            LibSecureASP lib = new LibSecureASP();
            string cipherText = lib.DecryptURL(Request.QueryString["q"]);
            string[] arryItem = cipherText.Split('&');
            string[] keys = arryItem[0].Split('=');
            string[] users = arryItem[1].Split('=');

            key = keys[1];
            user = users[1];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["q"]))
            {
                Response.Redirect("~/Logon.aspx");
            }

            GetValueFromURLEncrypt();
            lblSecretKey.Text = key;

            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var setupInfo = tfa.GenerateSetupCode(
                "secureasp",
                user + "@actvn.edu.vn",
                key, 
                false, 
                3);

            string qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            string manualEntrySetupCode = setupInfo.ManualEntryKey;

            imgQrCode.ImageUrl = qrCodeImageUrl;
            lblManualSetupCode.Text = manualEntrySetupCode;
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
            var result = tfa.ValidateTwoFactorPIN(key, txtCode.Text);
            if (result)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    user,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(2880),
                    true,
                    GetRoleFromGUIDUser(key),
                    FormsAuthentication.FormsCookiePath);

                string hash = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, hash);

                if (ticket.IsPersistent)
                {
                    cookie.Expires = ticket.Expiration;
                }

                FormsAuthentication.SetAuthCookie(user, true);

                Response.Cookies.Add(cookie);

                if (GetRoleFromGUIDUser(key) == "Administrator")
                {
                    Response.Redirect("~/Admin/Manager.aspx", true);
                }
                else
                {
                    Response.Redirect("~/InfomationUser.aspx?id=" + key, true);
                }
            }
            else
            {
                lblValidationResult.Text = txtCode.Text + " ............. " + DateTime.UtcNow.ToString();
                lblValidationResult.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}