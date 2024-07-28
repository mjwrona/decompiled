// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemProjectPermissionCheckHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class WorkItemProjectPermissionCheckHelper : 
    PermissionCheckHelperBase,
    IPermissionCheckHelper
  {
    private WorkItemTrackingTreeService m_treeService;
    private IVssRequestContext m_requestContext;
    private IVssSecurityNamespace m_projectSecurityNamespace;
    private Dictionary<string, bool?> m_projectPermissions;
    private Dictionary<int, string> m_token;

    public WorkItemProjectPermissionCheckHelper(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_projectPermissions = new Dictionary<string, bool?>();
      this.m_token = new Dictionary<int, string>();
    }

    public string GetWorkItemSecurityToken(int areaId)
    {
      string itemSecurityToken;
      if (!this.m_token.TryGetValue(areaId, out itemSecurityToken))
      {
        string absoluteUri = this.TreeService.LegacyGetTreeNode(this.m_requestContext, areaId, false)?.Project?.Uri?.AbsoluteUri;
        if (absoluteUri != null)
        {
          itemSecurityToken = this.ProjectSecurityNamespace.NamespaceExtension.HandleIncomingToken(this.m_requestContext, this.ProjectSecurityNamespace, absoluteUri);
          this.m_token[areaId] = itemSecurityToken;
        }
      }
      return itemSecurityToken;
    }

    public override bool HasWorkItemPermission(int areaId, int permission) => this.HasWorkItemPermissionCommon(areaId, permission, (Func<IVssRequestContext, string, int, bool?>) ((requestContext, token, requestedPermissions) => new bool?(this.ProjectSecurityNamespace.HasPermission(this.m_requestContext, token, permission)))).GetValueOrDefault();

    public override bool? GetWorkItemPermissionState(int areaId, int permission) => this.HasWorkItemPermissionCommon(areaId, permission, (Func<IVssRequestContext, string, int, bool?>) ((requestContext, token, requestedPermissions) => this.ProjectSecurityNamespace.GetPermissionState(this.m_requestContext, token, permission)));

    private bool? HasWorkItemPermissionCommon(
      int areaId,
      int permission,
      Func<IVssRequestContext, string, int, bool?> checkPermissionLogic)
    {
      return this.m_requestContext.TraceBlock<bool?>(902950, 902960, "WorkItemTracking", nameof (HasWorkItemPermissionCommon), "QueryPermissionChecker", (Func<bool?>) (() =>
      {
        if (this.m_requestContext.IsSystemContext)
          return new bool?(true);
        string cacheKey = this.GetCacheKey(areaId, permission);
        bool? nullable;
        if (!this.m_projectPermissions.TryGetValue(cacheKey, out nullable))
        {
          string itemSecurityToken = this.GetWorkItemSecurityToken(areaId);
          if (itemSecurityToken == null)
            return new bool?(false);
          nullable = checkPermissionLogic(this.m_requestContext, itemSecurityToken, permission);
          this.m_projectPermissions[cacheKey] = nullable;
        }
        return nullable;
      }));
    }

    public bool HasProcessMetadataPermission(Guid projectId)
    {
      IVssSecurityNamespace securityNamespace = this.m_requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, TeamProjectSecurityConstants.NamespaceId);
      string metadataSecurityToken = PermissionCheckHelper.GetProcessMetadataSecurityToken(this.m_requestContext, projectId);
      IVssRequestContext requestContext = this.m_requestContext;
      string token = metadataSecurityToken;
      int genericRead = TeamProjectSecurityConstants.GenericRead;
      return securityNamespace.HasPermission(requestContext, token, genericRead);
    }

    private string GetCacheKey(int areaId, int permission) => string.Format("{0}-{1}", (object) areaId, (object) permission);

    public bool TryGetProjectReadToken(Guid projectId, out string token) => throw new NotImplementedException();

    private IVssSecurityNamespace ProjectSecurityNamespace
    {
      get
      {
        if (this.m_projectSecurityNamespace == null)
          this.m_projectSecurityNamespace = this.m_requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, AuthorizationSecurityConstants.ProjectSecurityGuid);
        return this.m_projectSecurityNamespace;
      }
    }

    private WorkItemTrackingTreeService TreeService
    {
      get
      {
        if (this.m_treeService == null)
          this.m_treeService = this.m_requestContext.GetService<WorkItemTrackingTreeService>();
        return this.m_treeService;
      }
    }
  }
}
