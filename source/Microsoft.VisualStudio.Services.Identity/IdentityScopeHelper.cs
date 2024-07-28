// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityScopeHelper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityScopeHelper
  {
    private static string c_area = "IMS";
    private static string c_layer = nameof (IdentityScopeHelper);
    private static string c_checkGroupMembershipIsAllowedInRequestScopeFF = "VisualStudio.Services.Identity.CheckGroupMembershipManagementIsAllowedInRequestScope.Disable";

    internal static IList<Guid> GetScopeIds(IVssRequestContext collectionContext)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      IdentityService service = collectionContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(collectionContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.EveryoneGroup
      }, QueryMembership.Direct, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
      {
        collectionContext.Trace(5641456, TraceLevel.Error, IdentityScopeHelper.c_area, IdentityScopeHelper.c_layer, "Failed to find the everyone group");
        return (IList<Guid>) new List<Guid>();
      }
      List<IdentityDescriptor> list1 = readIdentity.Members.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null && IdentityHelper.IsWellKnownGroup(x, GroupWellKnownIdentityDescriptors.EveryoneGroup))).ToList<IdentityDescriptor>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list2 = service.ReadIdentities(collectionContext, (IList<IdentityDescriptor>) list1, QueryMembership.None, (IEnumerable<string>) null).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      list2.Add(readIdentity);
      return (IList<Guid>) list2.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.LocalScopeId)).ToList<Guid>();
    }

    internal static void SetDefaultPermissions(
      IVssRequestContext collectionContext,
      IServicingContext servicingContext,
      IList<Guid> scopeIds)
    {
      PlatformIdentityService service = collectionContext.GetService<PlatformIdentityService>();
      foreach (Guid scopeId in (IEnumerable<Guid>) scopeIds)
      {
        IdentityScope scope = service.GetScope(collectionContext, scopeId);
        if (scope == null)
          servicingContext.LogInfo(string.Format("Did not find a scope for id: {0}", (object) scopeId));
        else if (scope.ScopeType == GroupScopeType.ServiceHost && scope.Id != collectionContext.ServiceHost.InstanceId)
          servicingContext.LogInfo(string.Format("Skipping SetDefaultPermissions on this scope since we ran into an unexpected data shape. ScopeId did not match the collection service host id. Scope: {0} CollectionHost: {1}", (object) scope, (object) collectionContext.ServiceHost));
        else if ((scope.ScopeType == GroupScopeType.TeamProject || scope.ScopeType == GroupScopeType.Generic) && scope.ParentId != collectionContext.ServiceHost.InstanceId)
        {
          servicingContext.LogInfo(string.Format("Skipping SetDefaultPermissions on this scope since we ran into an unexpected data shape. The parent scopeId did not match the collection service host id. Scope: {0} CollectionHost: {1}", (object) scope, (object) collectionContext.ServiceHost));
        }
        else
        {
          servicingContext.LogInfo(string.Format("Setting default permissions for scope: {0}", (object) scope));
          service.SetDefaultPermissions(collectionContext, scope);
        }
      }
    }

    internal static void CheckGroupMembershipManagementIsAllowedInRequestScope(
      IVssRequestContext requestContext,
      Guid groupScopeId,
      SubjectDescriptor groupDescriptor)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application) || requestContext.IsFeatureEnabled(IdentityScopeHelper.c_checkGroupMembershipIsAllowedInRequestScopeFF))
        return;
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      if (groupScopeId != instanceId)
        throw new IncompatibleScopeException(HostingResources.CannotManageGroupMembershipInEnterpriseScope((object) groupDescriptor.ToString(), (object) requestContext.ServiceHost.InstanceId.ToString()));
    }
  }
}
