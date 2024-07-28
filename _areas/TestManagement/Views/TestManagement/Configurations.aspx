<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerPivot.master"
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <%
        Html.UseScriptModules("TestManagement/Scripts/TFS.TestManagement.ConfigurationsHub"); 
        Html.UseAreaCSS("TestManagement");
        Html.AddHubViewClass("test-hub-configurations-view"); 
    %>
</asp:Content>

<asp:Content ID="LeftHubContent" ContentPlaceHolderID="LeftHubContent" runat="server">   
     <% 
        Html.RenderPartial("Templates/ConfigurationDefinitionTree");
    %>
      <% 
        Html.RenderPartial("Templates/ConfigurationListTree");
    %>
    <div class="configurations-view-explorer">
       <div class="left-toolbar toolbar"></div>       
       <div class="search-configuration-action-container"></div>
       <div class="configurations-list-container" data-bind="template: 'configurations_list_explorer_tree', visible: visible"></div>
    </div> 
</asp:Content>

<asp:content contentplaceholderid="HubPivotViews" runat="server">
    <%
        IList<PivotView> rightPivots = new List<PivotView>();
        string pivotClass = "configurations-hub-view-tabs";
        
        rightPivots.Add(new PivotView()
        {
            Id = "configurationTab",            
            Disabled = true
        });
        rightPivots.Add(new PivotView()
        {
            Id = "variableTab",            
            Disabled = true
        });
    %>
    <%: Html.PivotViews(rightPivots, new { @class = pivotClass })%>
</asp:content>


<asp:Content ContentPlaceHolderID="RightHubContent" runat="server"> 
     <div class="configurations-hub-right-pane-content"></div>       
</asp:Content>

