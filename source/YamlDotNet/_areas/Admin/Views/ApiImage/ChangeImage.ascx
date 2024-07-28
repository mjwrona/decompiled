<%@ Control Language="C#" Inherits="Microsoft.TeamFoundation.Server.WebAccess.TfsViewUserControl<Microsoft.TeamFoundation.Server.WebAccess.IdentityViewModelBase>"%>
<%@ Import Namespace="Microsoft.TeamFoundation.Framework.Server" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Admin" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>

<div class="update-profile-image-control">
    <div class="change-image-error-pane"></div>
    <div class="content">
        <form id="frmImageUpload" action="<%: Url.Action("uploadImage", new { routeArea = TfsRouteArea.Api, tfid = Model.TeamFoundationId, isOrganizationLevel = ViewData["IsOrganizationLevel"] }) %>" method="post" enctype="multipart/form-data" target="imageframe">
            <input id="imageUploadedHidden" name="imageUploadedHidden" type="hidden" value=""/>
            <input id="actionIdHidden" name="actionIdHidden" type="hidden" value=""/>
            <div class="uploadSection">
                <table>
                    <tr>
                        <td>
                            <div class="image-upload-div">
                                <%: Html.IdentityImage(Model.TeamFoundationId, new { @addClass="preview-image large-identity-picture" }, new { @size = (int)Microsoft.TeamFoundation.Framework.Server.ImageSize.Large, @t = DateTime.Now.Ticks, @isOrganizationLevel = ViewData["IsOrganizationLevel"] }) %>
                                <img src="<%= StaticResources.Versioned.Content.GetLocation("spinner.gif") %>" class="image-upload-wait" />
                            </div>
                        </td>
                        <td>
                            <div class="image-input-div bowtie">
                                <label for="idImage"><%: AdminServerResources.SelectImage %></label>
                                <input id="idImage" name="idImage" class="requiredInfoLight browse-for-file " type="file" />
                                <button class="ui-button ui-widget ui-state-default ui-corner-all ui-button-text-only" id="btnImageSelect" role="button" type="button">
                                    <span class="ui-button-text"><%: AdminServerResources.ButtonTextChooseImage %></span>
                                </button>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="profile-image-cell"><a href="#" onkeydown="require('VSS/Utils/UI').buttonKeydownHandler(event);" role="button" class="profile-image-reset"><%: AdminResources.Reset %></a></td>
                    </tr>
                </table>
                <br />
                <div class="legal"><%: AdminServerResources.UploadAgreement %></div>
            </div>
        </form>
        <iframe id="imageframe" tabindex=-1 style="width:0;height:0;border:0px solid #fff;" name="imageframe">
        </iframe>
    </div>
    <div id="imageResult"></div>
</div>
