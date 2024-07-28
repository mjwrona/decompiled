// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlanPermissionsManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  internal class PlanPermissionsManager : SecurityNamespacePermissionsManager
  {
    public PlanPermissionsManager(
      IVssRequestContext requestContext,
      Guid permissionsIdentifier,
      string planId,
      string projectName)
      : base(permissionsIdentifier)
    {
      string token = PlanPermissionHelper.GetToken(requestContext.GetService<WebAccessWorkItemService>().GetProjectId(requestContext, projectName), new Guid(planId));
      this.Initialize(requestContext, token);
    }

    protected override Dictionary<Guid, SecurityNamespacePermissionSet> CreatePermissionSets(
      IVssRequestContext requestContext)
    {
      Dictionary<Guid, SecurityNamespacePermissionSet> permissionSets = new Dictionary<Guid, SecurityNamespacePermissionSet>();
      int permissionsToDisplay = 15;
      SecurityNamespacePermissionSet namespacePermissionSet = new SecurityNamespacePermissionSet(requestContext, PlanSecurityGroupConstants.SecurityNamespaceId, this.Token, permissionsToDisplay);
      permissionSets.Add(PlanSecurityGroupConstants.SecurityNamespaceId, namespacePermissionSet);
      return permissionSets;
    }
  }
}
