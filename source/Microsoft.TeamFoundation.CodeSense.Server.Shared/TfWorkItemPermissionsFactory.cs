// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.TfWorkItemPermissionsFactory
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public sealed class TfWorkItemPermissionsFactory : TfWorkItemFactoryBase
  {
    private static readonly int[] RequiredFields = new int[2]
    {
      -3,
      -2
    };

    private TfWorkItemPermissionsFactory(IVssRequestContext requestContext)
      : base(requestContext, TfWorkItemPermissionsFactory.RequiredFields)
    {
    }

    public static IDictionary<int, bool> GetPermissions(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      TfWorkItemPermissionsFactory permissionsFactory = new TfWorkItemPermissionsFactory(requestContext);
      List<string> list1 = permissionsFactory.GetSecurityTokens(permissionsFactory.PageWorkItems(workItemIds, new DateTime?())).ToList<string>();
      Dictionary<int, bool> permissions = new Dictionary<int, bool>();
      List<int> list2 = workItemIds.ToList<int>();
      TeamFoundationSecurityServiceProxy service = requestContext.GetService<TeamFoundationSecurityServiceProxy>();
      for (int index = 0; index < list2.Count; ++index)
      {
        bool flag = false;
        using (new CodeSenseTraceWatch(requestContext, 1025040, TraceLayer.ExternalSecurity, "Checking permissions for {0} in {1}", new object[2]
        {
          (object) list1[index],
          (object) AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid
        }))
          flag = service.HasPermissions(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, list1[index], 16);
        if (!permissions.ContainsKey(list2[index]))
          permissions.Add(list2[index], flag);
      }
      return (IDictionary<int, bool>) permissions;
    }

    private IEnumerable<string> GetSecurityTokens(IEnumerable<WorkItemFieldData> workItems)
    {
      TfWorkItemPermissionsFactory permissionsFactory = this;
      foreach (WorkItemFieldData workItem in workItems)
        yield return permissionsFactory.GetSecurityToken(permissionsFactory.requestContext, permissionsFactory.GetLatestAreaId(workItem.Id, workItem.AreaId));
    }

    private string GetSecurityToken(IVssRequestContext requestContext, int areaId)
    {
      using (new CodeSenseTraceWatch(requestContext, 1025040, TraceLayer.ExternalWorkitems, "Getting security token for {0} in {1}", new object[2]
      {
        (object) areaId,
        (object) AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid
      }))
      {
        Uri forPermissionCheck = this.treeDictionary.LegacyGetTreeNodeUriForPermissionCheck(requestContext, areaId);
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
        return securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, forPermissionCheck.ToString());
      }
    }
  }
}
