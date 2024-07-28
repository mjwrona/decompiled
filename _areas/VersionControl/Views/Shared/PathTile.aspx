<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.VersionControl" %>
<div class="header" title="<%: ViewBag.TileTooltip %>">
    <%: ViewBag.TileDisplayName%>
</div>
<div class="content">
    <%: ViewBag.ChangesetCount %>
</div>
<div class="footer">
    <%: VCServerResources.VersionControlPathTileFooter %>
</div>
