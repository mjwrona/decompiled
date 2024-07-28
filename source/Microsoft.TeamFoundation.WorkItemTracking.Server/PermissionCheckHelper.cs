// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.PermissionCheckHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class PermissionCheckHelper : PermissionCheckHelperBase, IPermissionCheckHelper
  {
    private IVssRequestContext m_requestContext;
    private WorkItemTrackingTreeService m_treeService;
    private IVssSecurityNamespace m_securityNamespace;
    private Dictionary<int, Dictionary<int, bool?>> m_permissionCache;
    private Dictionary<int, string> m_token;

    public PermissionCheckHelper(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_permissionCache = new Dictionary<int, Dictionary<int, bool?>>()
      {
        {
          16,
          new Dictionary<int, bool?>()
        },
        {
          32,
          new Dictionary<int, bool?>()
        },
        {
          256,
          new Dictionary<int, bool?>()
        },
        {
          512,
          new Dictionary<int, bool?>()
        }
      };
      this.m_token = new Dictionary<int, string>();
    }

    public string GetWorkItemSecurityToken(int areaId)
    {
      string itemSecurityToken;
      if (!this.m_token.TryGetValue(areaId, out itemSecurityToken))
      {
        itemSecurityToken = PermissionCheckHelper.GetWorkItemSecurityToken(this.m_requestContext, areaId);
        if (itemSecurityToken != null)
          this.m_token[areaId] = itemSecurityToken;
      }
      return itemSecurityToken;
    }

    public static string GetWorkItemSecurityToken(IVssRequestContext requestContext, int areaId)
    {
      Uri forPermissionCheck = requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNodeUriForPermissionCheck(requestContext, areaId);
      if (!(forPermissionCheck != (Uri) null))
        return (string) null;
      IVssSecurityNamespace namespaceInstance = PermissionCheckHelper.GetSecurityNamespaceInstance(requestContext);
      return namespaceInstance.NamespaceExtension.HandleIncomingToken(requestContext, namespaceInstance, forPermissionCheck.AbsoluteUri);
    }

    public override bool HasWorkItemPermission(int areaId, int permission) => this.GetWorkItemPermissionStateCommon(areaId, permission, (Func<IVssRequestContext, string, int, bool, bool?>) ((requestContext, token, requestedPermissions, alwaysAllowAdministrators) => new bool?(this.SecurityNamespace.HasPermission(this.m_requestContext, token, permission, false)))).GetValueOrDefault();

    public override bool? GetWorkItemPermissionState(int areaId, int permission) => this.GetWorkItemPermissionStateCommon(areaId, permission, (Func<IVssRequestContext, string, int, bool, bool?>) ((requestContext, token, requestedPermissions, alwaysAllowAdministrators) => this.SecurityNamespace.GetPermissionState(this.m_requestContext, token, permission, false)));

    private bool? GetWorkItemPermissionStateCommon(
      int areaId,
      int permission,
      Func<IVssRequestContext, string, int, bool, bool?> checkPermissionLogic)
    {
      return this.m_requestContext.TraceBlock<bool?>(902900, 902910, "WorkItemTracking", nameof (GetWorkItemPermissionStateCommon), "QueryPermissionChecker", (Func<bool?>) (() =>
      {
        if (this.m_requestContext.IsSystemContext)
          return new bool?(true);
        Dictionary<int, bool?> dictionary;
        if (!this.m_permissionCache.TryGetValue(permission, out dictionary))
          throw new NotSupportedException();
        bool? permissionStateCommon = new bool?();
        if (!dictionary.TryGetValue(areaId, out permissionStateCommon))
        {
          string token = PermissionCheckHelper.GetWorkItemSecurityToken(this.m_requestContext, areaId);
          permissionStateCommon = token != null ? checkPermissionLogic(this.m_requestContext, token, permission, false) : new bool?(false);
          this.m_requestContext.Trace(902901, TraceLevel.Warning, "WorkItemTracking", "QueryPermissionChecker", string.Format("WorkItem permission request for areaId: {0} which resolved to token: {1}. Request was for permission bits: {2}. HasPermission result is: {3}", (object) areaId, (object) token, (object) permission, (object) permissionStateCommon));
          bool? nullable = permissionStateCommon;
          bool flag = false;
          if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
            this.m_requestContext.TraceConditionally(902902, TraceLevel.Warning, "WorkItemTracking", "QueryPermissionChecker", (Func<string>) (() => string.Format("User {0} does not have WorkItem permission for areaId: {1} which resolved to token: {2}. Request was for permission bits: {3}.", (object) this.m_requestContext.GetUserId(), (object) areaId, (object) token, (object) permission)));
          if (!permissionStateCommon.HasValue)
            this.m_requestContext.TraceConditionally(902904, TraceLevel.Warning, "WorkItemTracking", "QueryPermissionChecker", (Func<string>) (() => string.Format("User {0} does not have WorkItem permission value for areaId: {1} which resolved to token: {2}. Request was for permission bits: {3}.", (object) this.m_requestContext.GetUserId(), (object) areaId, (object) token, (object) permission)));
          dictionary[areaId] = permissionStateCommon;
        }
        else
        {
          bool? nullable = permissionStateCommon;
          bool flag = false;
          if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
            this.m_requestContext.TraceConditionally(902903, TraceLevel.Warning, "WorkItemTracking", "QueryPermissionChecker", (Func<string>) (() => string.Format("User {0} does not have WorkItem permission for areaId: {1}. Request was for permission bits: {2}.", (object) this.m_requestContext.GetUserId(), (object) areaId, (object) permission)));
        }
        return permissionStateCommon;
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

    public bool TryGetProjectReadToken(Guid projectId, out string token)
    {
      IVssSecurityNamespace securityNamespace = this.m_requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.m_requestContext, TeamProjectSecurityConstants.NamespaceId);
      token = TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(projectId));
      IVssRequestContext requestContext = this.m_requestContext;
      string token1 = token;
      int genericRead = TeamProjectSecurityConstants.GenericRead;
      if (securityNamespace.HasPermission(requestContext, token1, genericRead))
        return true;
      token = (string) null;
      return false;
    }

    public static string GetProcessMetadataSecurityToken(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(projectId));
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

    private IVssSecurityNamespace SecurityNamespace
    {
      get
      {
        if (this.m_securityNamespace == null)
          this.m_securityNamespace = PermissionCheckHelper.GetSecurityNamespaceInstance(this.m_requestContext);
        return this.m_securityNamespace;
      }
    }

    private static IVssSecurityNamespace GetSecurityNamespaceInstance(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
    }
  }
}
