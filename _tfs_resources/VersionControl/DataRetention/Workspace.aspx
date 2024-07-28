<%@ Page Inherits="Microsoft.TeamFoundation.VersionControl.Server.DataRetention.WorkspaceDataRetentionPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
<title>Workspace Renewal/Deletion</title>
</head>
<body>
    <form id="WorkspaceDataRetentionPage" runat="server">
    <div>
        <h3>Server Information</h3>
        <asp:Table ID="TableServerInfo" runat="server">
        </asp:Table>
    </div>
    <br />
    <div>
        <h3>Workspace Information</h3>
        <asp:Table ID="TableWorkspace" runat="server">
        </asp:Table>
    </div>
    <br />
    <div>
        <h3>Working Folders</h3>
        <asp:Table ID="TableWorkingFolders" runat="server" GridLines="Both">
        </asp:Table>
    </div>
    <br />    
    <asp:Button ID="Action" runat="server" Text="Extend Expiration" OnClick="Action_Click" />
    </form>
</body>
</html>