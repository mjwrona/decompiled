
<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Masters/HubPageExplorerTriSplitPivot.master" 
    Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewPage<dynamic>"%>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.TestManagement" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% 
        Html.UseScriptModules("TestManagement/Scripts/TFS.TestManagement.DataSetsView"); 
        Html.UseAreaCSS("TestManagement");
        Html.AddHubViewClass("test-hub-datasets-view");
    %>
</asp:Content>
<asp:content ID="LeftHubContent" ContentPlaceHolderID="LeftHubContent" runat="server">
  <div class="datasets-view-explorer">
        <div class="left-toolbar toolbar"></div>
        <div class="search-input-wrapper"></div>
        <div class="datasets-list-container"></div>
    </div>
    <%: Html.JsonIsland(Html.GetLastSelectedSharedParameterId(TfsWebContext), new {@class="selected-shared-parameter-id"}) %>
</asp:content>

<asp:Content ContentPlaceHolderID="HubPivotViews" runat="server">
    <%: Html.PivotViews(new[] 
            { 
                new PivotView(TestManagementServerResources.SharedParameterValues)
                { 
                    Id = "dataset-values",
                    Link = Url.FragmentAction("values")
                }, 
                new PivotView(TestManagementServerResources.SharedParameterProperties)
                { 
                    Id = "dataset-properties",
                    Link = Url.FragmentAction("properties"),
                }
            }, new
            {
                @class = "dataset-items-tabs"
            }) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="HubPivotFilters" runat="server"> 
    <%: Html.PivotFilter(
            TestManagementServerResources.ReferencedTestCasesPaneStatusTitle, 
            new[] { 
                new PivotFilterItem(TestManagementServerResources.ReferencedTestCasesPaneStatusOn, "on") { Selected = Html.IsSharedParametersTestCasePaneFilterOn(TfsWebContext) }, 
                new PivotFilterItem(TestManagementServerResources.ReferencedTestCasesPaneStatusOff, "off") { Selected = !Html.IsSharedParametersTestCasePaneFilterOn(TfsWebContext) }
            }, 
            new { @class = "test-case-pane-filter" }) %>
</asp:Content>

<asp:Content  ContentPlaceHolderID="CenterHubContent" runat="server">
    <div class="datasets-view-right-pane">
            <div class="datasets-values-view-right-pane">
                <div class="toolbar hub-pivot-toolbar"></div>
                <div class="datasets-view-grid"></div>
            </div>
            <div class="datasets-properties-view-right-pane">
                <div class="datasets-view-properties"></div>
            </div>
        </div>
</asp:Content>
<asp:Content  ContentPlaceHolderID="HubEnd" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="FarRightHubContent" runat="server">
    <div class="referenced-testcases-pane">
        <div class="reference-testcases-pane-title"><%:TestManagementServerResources.ReferencedTestCasesPaneTitle%></div>
        <div class="reference-testcases-pane-status-indicator"></div>
        <div class="referenced-testcases-view-explorer">
            <div class="referenced-testcases-toolbar toolbar"></div>
            <div class="referenced-testcases-list-container"></div>
        </div>
    </div>
</asp:Content>