<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.Admin.ManageViewModel>" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>

<div class="hub-view explorer manage-identities-view">
<%
    if ((bool)ViewData["IsInactiveOrganization"])
    {
%>
        <div class="manage-identities-error-view">
            <%: AdminServerResources.InactiveOrganizationError %>
        </div>
<% 
    } else { 
%>
        <div class="hub-content">
            <div class="splitter horizontal hub-splitter stateful toggle-button-enabled toggle-button-hotkey-enabled">
                <script type="application/json" class="permissions-context"><%= ViewData["PermissionsData"] %></script>            
                <script class="options" defer="defer" type="application/json">{"settingPath":"Web/UIState/security/LeftHubSplitter"}</script>
                <div class="organization-page-level"/>
                <div class="leftPane" role="navigation">
                    <div class="left-hub-content">
                        <div class="hub-pivot-content">
                            <div class="identity-list-section vertical-fill-layout">
                                <div class="identity-search-box fixed-header">
                                    <div class="toolbar"></div>
                                    <div class="ip-groups-search-container"></div>
                                </div>
                                <div class="main-identity-grid fill-content"></div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="handleBar"></div>
                <div class="rightPane" role="main">
                    <div class="hub-title">
                        <div class="label"></div>
                        <div class="image"></div>
                    </div>
                    <div class="hub-progress pageProgressIndicator"></div>
                    <div class="right-hub-content">
                        <div class="hub-pivot">
                            <div class="views">
                                <%: Html.PivotViews(new[]
                                {
                                    new PivotView(AdminResources.Permissions)
                                    {
                                        Id = "summary",
                                        Link = Url.FragmentAction("summary", null)
                                    },
                                    new PivotView(AdminResources.Members)
                                    {
                                        Id = "members",
                                        Link = Url.FragmentAction("members", null)
                                    },
                                    new PivotView(AdminResources.MemberOf)
                                    {
                                        Id = "memberOf",
                                        Link = Url.FragmentAction("memberOf", null)
                                    }
                                }, new { @class = "manage-view-tabs" })
                                %>
                            </div>
                        </div>
                        <div class="hub-pivot-content">
                            <div class="identity-details-section">
                                <div id="identityInfo" class="manage-info"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
<% 
    }
%>
 </div>
