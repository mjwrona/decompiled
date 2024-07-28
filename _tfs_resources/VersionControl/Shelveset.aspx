<%@ Page Inherits="Microsoft.TeamFoundation.VersionControl.Server.DataRetention.ShelvesetDataRetentionPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
<title>Shelveset Renewal/Deletion</title>
</head>
<body>
    <form id="ShelvesetDataRetentionPage" runat="server">
    <div>
        <h3>Server Information</h3>
        <asp:Table ID="TableServerInfo" runat="server">
        </asp:Table>
    </div>
    <br />
    <div>
        <h3>Shelveset Information</h3>
        <asp:Table ID="TableShelveset" runat="server">
        </asp:Table>
    </div>
    <br />
    <div>
        <h3>Shelved Changes</h3>
        <asp:Table ID="TablePendingChanges" runat="server" GridLines="Both">
        </asp:Table>
    </div>
    <br />    
    <asp:Button ID="Action" runat="server" Text="Extend Expiration" OnClick="Action_Click" />
    </form>
</body>
</html>