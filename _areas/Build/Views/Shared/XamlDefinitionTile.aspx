<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Build" %>
<div class="header" title="<%: ViewBag.DefinitionName %>">
    <%: ViewBag.DefinitionName %>
</div>
<div class="content">
    <div class="build-definition-histogram definition-histogram">
        <%: Html.JsonIsland(new {
                buildDefinitionUri= ViewBag.DefinitionUri,
                defaultBars= false,
                barCount= 25,
                barWidth= 10,
                barHeight= 60,
                barSpacing= 2,
                allowInteraction= false
            }, new {@class="options"}) %>
    </div>
</div>
<div class="footer">
    <span class="icon icon-tfs-build-status-<%: ViewBag.StatusIcon.ToLower() %>"></span>
    <%: ViewBag.BuildSummary %>
</div>
<%: Html.ScriptModules("Build/Scripts/Controls.Histogram") %>
