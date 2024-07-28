// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AnalyticsViewsPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class AnalyticsViewsPermissionsManager : SecurityNamespacePermissionsManager
  {
    public AnalyticsViewsPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string viewId,
      string projectName)
      : base(permissionsIdentifier)
    {
      string securityToken = AnalyticsViewsSecurityNamespace.GetSecurityToken(AnalyticsViewVisibility.Shared, requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName), Guid.Parse(viewId));
      this.Initialize(requestContext, securityToken);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay = 7;
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, AnalyticsViewsSecurityNamespace.SecurityNamespaceId, this.Token, permissionsToDisplay);
      permissionSets.Add(AnalyticsViewsSecurityNamespace.SecurityNamespaceId, namespacePermissionSet);
      return permissionSets;
    }
  }
}
