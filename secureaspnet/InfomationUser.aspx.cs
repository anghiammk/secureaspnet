using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;

namespace secureaspnet
{
    public partial class InformationUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
                if(reader["profile_avatar"].ToString() != null)
                {
                    if (File.Exists(MapPath(reader["profile_avatar"].ToString())))
                    {
                        Image.ImageUrl = reader["profile_avatar"].ToString();
                    }
                }
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

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            bool val1 = (System.Web.HttpContext.Current.User != null)
                && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            SqlConnection connectionObj;
            SqlCommand command;
            string sqlStr = String.Empty;
            string connStringDB;

            if (!val1)
                return;

            if (Page.IsValid && fileUpload.HasFile)
            {
                string fileName = "images/" + fileUpload.FileName;
                string filePath = MapPath(fileName);

                if (!Directory.Exists(MapPath("images/")))
                {
                    Directory.CreateDirectory(MapPath("images/"));
                }

                connStringDB = ConfigurationManager.ConnectionStrings[1].ToString();
                connectionObj = new SqlConnection(connStringDB);
                connectionObj.Open();

                sqlStr = "UPDATE info SET profile_avatar = @profile_avatar WHERE id = @id";

                command = new SqlCommand(sqlStr, connectionObj);
                command.Parameters.Add(new SqlParameter("@id", Request.QueryString["id"]));
                command.Parameters.Add(new SqlParameter("@profile_avatar", fileName));
                command.ExecuteNonQuery();

                SafeTokenHandle safeTokenHandle = null;
                safeTokenHandle = HandlerPermission.GetHandleUserLogon("", "batman", "098poiA#");
                using (safeTokenHandle)
                {
                    System.Diagnostics.Debug.WriteLine("Before impersonation: " + WindowsIdentity.GetCurrent().Name);
                    using (WindowsIdentity newId = new WindowsIdentity(safeTokenHandle.DangerousGetHandle()))
                    {
                        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                        {
                            System.Diagnostics.Debug.WriteLine("After impersonation: " + WindowsIdentity.GetCurrent().Name);
                            fileUpload.SaveAs(filePath);
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("After closing the context: " + WindowsIdentity.GetCurrent().Name);
                }
                Image.ImageUrl = fileName;
            }
        }
    }
}
