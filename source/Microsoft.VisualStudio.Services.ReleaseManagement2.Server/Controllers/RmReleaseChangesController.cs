// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleaseChangesController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "changes")]
  public class RmReleaseChangesController : ReleaseManagementProjectControllerBase
  {
    protected const int DefaultMaxItems = 250;

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("8DCF9FE9-CA37-4113-8EE1-37928E98407C")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    public virtual IEnumerable<Change> GetReleaseChanges(
      int releaseId,
      int baseReleaseId = 0,
      [FromUri(Name = "$top")] int top = 250,
      string artifactAlias = null)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmReleaseChangesController.GetReleaseChanges", 1972109, 5, true))
      {
        IEnumerable<Change> releaseChanges = this.TfsRequestContext.GetService<ReleaseChangesService>().GetReleaseChanges(this.TfsRequestContext, this.ProjectInfo, baseReleaseId, releaseId, top, artifactAlias);
        if (releaseChanges != null)
        {
          ISecuredObject securedObject = this.TfsRequestContext.GetSecuredObject();
          foreach (Change change in releaseChanges)
          {
            if (change.Author != null)
              change.Author = (IdentityRef) new RmReleaseChangesController.ReleaseManagementIdentityRef(change.Author, securedObject.GetToken(), securedObject.RequiredPermissions);
            if (change.PushedBy != null)
              change.PushedBy = (IdentityRef) new RmReleaseChangesController.ReleaseManagementIdentityRef(change.PushedBy, securedObject.GetToken(), securedObject.RequiredPermissions);
          }
          this.TfsRequestContext.SetSecuredObjects<Change>(releaseChanges);
        }
        return releaseChanges;
      }
    }

    private class ReleaseManagementIdentityRef : IdentityRef, ISecuredObject
    {
      private string token;
      private int permissions;

      public ReleaseManagementIdentityRef(IdentityRef identityRef, string token, int permissions)
      {
        this.Descriptor = identityRef.Descriptor;
        this.DisplayName = identityRef.DisplayName;
        this.Url = identityRef.Url;
        this.Id = identityRef.Id;
        this.UniqueName = identityRef.UniqueName;
        this.DirectoryAlias = identityRef.DirectoryAlias;
        this.ProfileUrl = identityRef.ProfileUrl;
        if (identityRef.Links?.Links != null)
        {
          if (identityRef.Links.Links.ContainsKey("avatar"))
            this.ImageUrl = identityRef.Links.Links["avatar"] is ReferenceLink link1 ? link1.Href : (string) null;
          this.Links = new ReferenceLinks();
          foreach (KeyValuePair<string, object> link2 in (IEnumerable<KeyValuePair<string, object>>) identityRef.Links.Links)
          {
            ReferenceLink referenceLink = link2.Value as ReferenceLink;
            this.Links.AddLink(link2.Key, referenceLink.Href, (ISecuredObject) new ReleaseManagementSecuredObject(token, permissions));
          }
        }
        else
          this.ImageUrl = (string) null;
        this.IsContainer = identityRef.IsContainer;
        this.IsAadIdentity = identityRef.IsAadIdentity;
        this.Inactive = identityRef.Inactive;
        this.token = token;
        this.permissions = permissions;
      }

      Guid ISecuredObject.NamespaceId => SecurityConstants.ReleaseManagementSecurityNamespaceId;

      int ISecuredObject.RequiredPermissions => this.permissions;

      string ISecuredObject.GetToken() => this.token;
    }
  }
}
