using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Configuration;
using System.Web.Mvc;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using Google.Authenticator;
using System.Web.Helpers;


namespace secureaspnet
{
    public partial class Logon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                AntiForgery.Validate();
        }

        public bool HashPassword(string passwordDB, string passowrdInput, string salt)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(passowrdInput, Convert.FromBase64String(salt));
            pbkdf2.IterationCount = 1000;

            byte[] computedHash = pbkdf2.GetBytes(32);

            bool isAuthenticCredential = passwordDB.Equals(
                                        Convert.ToBase64String(computedHash),
                                        StringComparison.Ordinal);

            return isAuthenticCredential;
        }
        

        private bool CheckLoginUserFromDB(string username, string password)
        {
            bool ret = false;
            
            string connStringDB;
            string sqlStr;

            SqlConnection connectionObj;
            SqlCommand command;
            SqlDataReader reader;

            sqlStr = "SELECT password, salt FROM info WHERE " +
                "username = @username ";

            connStringDB = ConfigurationManager.ConnectionStrings[1].ToString();
            connectionObj = new SqlConnection(connStringDB);
            connectionObj.Open();

            if (connectionObj == null)
                return ret;
            
            command = new SqlCommand(sqlStr, connectionObj);
            if (command == null)
                return ret;

            command.Parameters.Add(new SqlParameter("@username", username));
            reader = command.ExecuteReader();

            if(reader.Read())
            {
                if(HashPassword(
                    reader["password"].ToString(), 
                    password, 
                    reader["salt"].ToString()
                    ))
                {
                    ret = true;
                }
            }

            connectionObj.Close();
            return ret;
        }

        
        [ValidateAntiForgeryToken]
        protected void Button1_Click(object sender, EventArgs e)
        {
            string username = Request.Form["username"];
            string password = Request.Form["password"];
            string sqlStr = String.Empty;
            string connStringDB;
            string key;
            string valueEncrypt;

            SqlConnection connectionObj;
            SqlCommand command;
            SqlDataReader reader;

            if (CheckLoginUserFromDB(username, password))
            {
                sqlStr = "SELECT id FROM info WHERE " +
                            "username = @username ";

                connStringDB = ConfigurationManager.ConnectionStrings[1].ToString();
                connectionObj = new SqlConnection(connStringDB);
                connectionObj.Open();

                command = new SqlCommand(sqlStr, connectionObj);
                command.Parameters.Add(new SqlParameter("@username", username));

                reader = command.ExecuteReader();

                if (reader.Read())
                {
                    key = reader["id"].ToString();
                    connectionObj.Close();

                    LibSecureASP lib = new LibSecureASP();
                    valueEncrypt = "key=" + key + "&user=" + username;
                    lib.EncryptURL(valueEncrypt);

                    Response.Redirect("~/LogonConfirmCode.aspx?q=" + lib.EncryptURL(valueEncrypt));
                }
                connectionObj.Close();
            }
            else
            {
                Label1.Text = "Invalid login!";
            }
        }

    }
}