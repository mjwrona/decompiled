<%@ Page Title="" Language="C#" MasterPageFile="~/_views/Shared/Platform.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.VisualStudio.Services.Web.UserManagement" %>

<asp:content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%: StaticResources.Versioned.Content.GetLocation("Userhub.css") %>" type="text/css" rel="stylesheet"/>
</asp:content>

<asp:Content ContentPlaceHolderID="DocumentBegin" runat="server">
    <% Html.ContentTitle(UserManagementResources.AccountHomeTitle); %>
    <% Html.UseScriptModules("UserManagement/Scripts/SPS.Users.Controls"); %>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <% if (ViewBag.AuthenticationFail)
       { %>
        <script type="application/json" class="userHub-redirectUrl"><%= ViewBag.ActionUrl %></script>
        <script type="application/json" class="tfsAccount-Url"><%= ViewBag.TfsAccountUrl %></script>
        <div class="userHub-redirect-view"></div>
    <% }
       else
       { %>
        <script type="application/json" class="userHub-resetDate"><%= ViewBag.NextResetDate %></script>
        <div class="splitter toggle-button-enabled horizontal hub-splitter">
            <div class="leftPane" role="navigation">
                <div class="extension-view">
                    <div id="extensionCol">
                    </div>
                </div>
            </div>
            <div class="handleBar"></div>
            <div class="rightPane" role="main">
                <div class="userHub-account-view">
                    <div id="headerInfoMessage" class="header-message"></div>
                    <div id="commonMessage" class="common-message"></div>
                    <div id="userHubTitle" class="left-panel-title">
                    </div>
                    <span id="learnMoreLink" class="learn-more-link"></span>
                    <div class="userHubSubTitle">
                         <span id="extensionLicenseStatusLabel" class="extension-license-status-label"></span>
                         <span id="buyMoreLink" class="buy-more-link"></span>
                     </div>
                    <div id="userCol">
                        <div id="menuBar">
                        </div>
                        <div id="panel" class="panel-region">
                        </div>
                    </div>
                </div>
                <div id="licenseDataContainer" class="license-view">
                    <div id="scrollContainer" class="license-scroll">
                        <script type="application/json" class="permissions-context"><%= ViewData["PermissionsData"] %></script>
                        <div id="userTitle" class="license-panel-title">
                        </div>
                        <table id="userSection" class="users-section">

                        </table>

                        <div id="licenseSection">

                        </div>
                        <div id="pricingInfoLink" class="pricing-info">

                        </div>
                    </div>
                </div>
            </div>
        </div>
    <% } %>
</asp:Content>
