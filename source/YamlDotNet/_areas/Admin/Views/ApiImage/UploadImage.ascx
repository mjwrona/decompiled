<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.ImageModel>"%>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<script type="text/javascript"<%= Html.GenerateNonce(true) %>>
    window.parent.require("Admin/Scripts/TFS.Admin.Dialogs").ImageUploadComplete('<%: Model.TeamFoundationId %>', '<%: Model.ErrorMessage %>');
</script>