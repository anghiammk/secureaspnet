<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InfomationUser.aspx.cs" Inherits="secureaspnet.InformationUser" ValidateRequest="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <style type="text/css">
        body
        {
            font-family: Arial;
            font-size: 10pt;
        }
        table
        {
            border:1px solid #ccc;
            border-collapse:collapse;
        }
        table th
        {
            background-color: #F7F7F7;
            color: #333;
            font-weight: bold;
        }
        table th, table td
        {
            padding: 5px;
            border-color: #ccc;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
         <div >  
            <table style="width:25%;">  
                <caption class="style1">  
                    <strong>Employee Details</strong>  
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
                    Name:</td>  
                    <td>  
                        <asp:Label ID="name" runat="server" ></asp:Label>  
                    </td>  
                    
                </tr>  
                <tr>  
                    <td class="style2">  
                        Address</td>  
                    <td>  
                        <asp:Label ID="address" runat="server"/>  
                    </td>  
                </tr>  
                <tr>  
                    <td class="style2">  
                        Age</td>  
                    <td>  
                        <asp:Label ID="age" runat="server"/>  
                    </td>  
                </tr>
                <tr>  
                    <td class="style2">  
                        Phone</td>  
                    <td>  
                        <asp:Label ID="phone" runat="server"/>  
                    </td>  
                </tr>
                <tr>  
                    <td class="style2">  
                        Time Create</td>  
                    <td>  
                        <asp:Label ID="time" runat="server"/>  
                    </td>  
                </tr>
                <tr>  
                    <td class="style2">  
               </td>  
                    <td>  
                        <asp:Button ID="Button1" runat="server" Text="Logout" onclick="Logout" />  
                    </td>  
                </tr> 
            </table>  
        </div> 
    </form>
</body>
</html>
