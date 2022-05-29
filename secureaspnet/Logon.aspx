<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Logon.aspx.cs" Inherits="secureaspnet.Logon" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<title>Login Form</title>
</head>
<body> 
    <form id="form1" runat="server" method="post">  
    <%= System.Web.Helpers.AntiForgery.GetHtml() %>
        <div >  
            <table style="width:100%;">  
                <caption class="style1">  
                    <strong>Login Form</strong>  
                </caption>  
                <tr>  
                    <td class="style2">  
                    </td>  
                    <td>  
                    </td>  
                    <td>  
                </td>  
                </tr>  
                <tr>  
                    <td class="style2">  
                    Username:</td>  
                    <td>  
                        <asp:TextBox ID="username" runat="server" ></asp:TextBox>  
                    </td>  
                    <td>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="username" ErrorMessage="Please Enter Your Username" ForeColor="Red"></asp:RequiredFieldValidator>  
                    </td>  
                </tr>  
                <tr>  
                    <td class="style2">  
                        Password</td>  
                    <td>  
                        <asp:TextBox ID="password" TextMode="Password" runat="server"/>  
                    </td>  
                    <td>  
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="password" ErrorMessage="Please Enter Your word" ForeColor="Red"></asp:RequiredFieldValidator>  
                    </td>  
                </tr>  
                <tr>  
                    <td class="style2">  
                    </td>  
                    <td>  
                    </td>  
                    <td>  
                </td>  
                </tr>  
                <tr>  
                    <td class="style2">  
               </td>  
                    <td>  
                        <asp:Button ID="Button1" runat="server" Text="Log In" onclick="Button1_Click" />  
                    </td>  
                    <td>  
                        <asp:Label ID="Label1" runat="server"></asp:Label>  
                    </td>  
                </tr> 
            </table>  
        </div> 
        
    </form>  
</body> 
</html>
