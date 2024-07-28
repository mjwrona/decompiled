// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PoolSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal abstract class PoolSecurityProvider : IAgentPoolSecurityProvider
  {
    protected static readonly Guid NamespaceId = DefaultSecurityProvider.NamespaceId;
    protected static readonly char NamespaceSeparator = DefaultSecurityProvider.NamespaceSeparator;
    protected static readonly string AgentSecurityToken = "Agents";
    protected static readonly string RequestSecurityToken = "Requests";

    protected abstract string AgentPoolSecurityToken { get; }

    public static void ThrowPoolAccessDeniedException(
      IVssRequestContext requestContext,
      int requiredPermissions,
      int poolId,
      string poolName = null)
    {
      if (string.IsNullOrEmpty(poolName))
      {
        TaskAgentPool agentPool = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPool(requestContext.Elevate(), poolId);
        if (agentPool != null)
          poolName = agentPool.Name;
        else
          PoolSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
      }
      PoolSecurityProvider.ThrowPoolAccessDeniedException(requestContext, poolName, requiredPermissions);
    }

    public string GetAgentPoolToken(int poolId) => poolId == 0 ? this.AgentPoolSecurityToken : this.AgentPoolSecurityToken + (object) PoolSecurityProvider.NamespaceSeparator + (object) poolId;

    public string GetAgentRequestToken(int poolId, int? agentId, long? requestId)
    {
      string agentRequestToken = this.GetAgentPoolToken(poolId);
      if (agentId.HasValue)
      {
        agentRequestToken = agentRequestToken + (object) PoolSecurityProvider.NamespaceSeparator + PoolSecurityProvider.AgentSecurityToken + (object) PoolSecurityProvider.NamespaceSeparator + agentId.Value.ToString();
        if (requestId.HasValue)
          agentRequestToken = agentRequestToken + (object) PoolSecurityProvider.NamespaceSeparator + PoolSecurityProvider.RequestSecurityToken + (object) PoolSecurityProvider.NamespaceSeparator + requestId.Value.ToString();
      }
      return agentRequestToken;
    }

    public bool HasAgentPermission(
      IVssRequestContext requestContext,
      int poolId,
      int? agentId,
      long? requestId,
      int requiredPermissions)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId);
      string agentRequestToken = this.GetAgentRequestToken(poolId, agentId, requestId);
      IVssRequestContext requestContext1 = requestContext;
      string token = agentRequestToken;
      int requestedPermissions = requiredPermissions;
      return securityNamespace.HasPermission(requestContext1, token, requestedPermissions, false);
    }

    public bool HasPoolPermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      return requestContext.IsSystemContext || requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId).HasPermission(requestContext, this.AgentPoolSecurityToken, requiredPermissions, alwaysAllowAdministrators);
    }

    public bool HasPoolPermission(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      return this.HasPoolPermission(requestContext, pool.Id, requiredPermissions, alwaysAllowAdministrators);
    }

    public virtual bool HasPoolPermission(
      IVssRequestContext requestContext,
      int poolId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (requestContext.IsSystemContext)
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId);
      string agentPoolToken = this.GetAgentPoolToken(poolId);
      IVssRequestContext requestContext1 = requestContext;
      string token = agentPoolToken;
      int requestedPermissions = requiredPermissions;
      int num = alwaysAllowAdministrators ? 1 : 0;
      return securityNamespace.HasPermission(requestContext1, token, requestedPermissions, num != 0);
    }

    public void CheckPoolPermission(
      IVssRequestContext requestContext,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasPoolPermission(requestContext, requiredPermissions, alwaysAllowAdministrators))
        return;
      PoolSecurityProvider.ThrowAccessDeniedException(requestContext, requiredPermissions);
    }

    public void CheckPoolPermission(
      IVssRequestContext requestContext,
      int poolId,
      int requiredPermissions,
      bool alwaysAllowAdministrators = false)
    {
      if (this.HasPoolPermission(requestContext, poolId, requiredPermissions, alwaysAllowAdministrators))
        return;
      PoolSecurityProvider.ThrowPoolAccessDeniedException(requestContext, requiredPermissions, poolId);
    }

    public void SetDefaultPermissions(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> poolAdministrators)
    {
      List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>();
      if (poolAdministrators != null)
        accessControlEntryList.AddRange((IEnumerable<IAccessControlEntry>) poolAdministrators.Select<Microsoft.VisualStudio.Services.Identity.Identity, AccessControlEntry>((Func<Microsoft.VisualStudio.Services.Identity.Identity, AccessControlEntry>) (x => new AccessControlEntry()
        {
          Descriptor = x.Descriptor,
          Allow = 27
        })));
      if (accessControlEntryList.Count <= 0)
        return;
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId).SetAccessControlEntries(requestContext.Elevate(), this.GetAgentPoolToken(pool.Id), (IEnumerable<IAccessControlEntry>) accessControlEntryList, false);
    }

    public void GrantAdministratorPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId).EnsurePermissions(requestContext.Elevate(), this.GetAgentPoolToken(poolId), identity.Descriptor, 27, 0, false);
    }

    public void GrantListenPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId).EnsurePermissions(requestContext.Elevate(), this.GetAgentPoolToken(poolId), identity.Descriptor, 21, 0, false);
    }

    public void GrantReadPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId).EnsurePermissions(requestContext.Elevate(), this.GetAgentPoolToken(poolId), identity.Descriptor, 1, 0, false);
    }

    public void RemoveAccessControlLists(IVssRequestContext requestContext, TaskAgentPool pool) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId).RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
    {
      this.GetAgentPoolToken(pool.Id)
    }, true);

    public void RevokeAdministratorPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId)?.RemoveAccessControlEntries(requestContext.Elevate(), this.GetAgentPoolToken(poolId), (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identity.Descriptor
      });
    }

    public void RevokeListenPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId)?.RemoveAccessControlEntries(requestContext.Elevate(), this.GetAgentPoolToken(poolId), (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identity.Descriptor
      });
    }

    public void RevokeReadPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      this.RevokeReadPermissionToPool(requestContext, poolId, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      });
    }

    public void RevokeReadPermissionToPool(
      IVssRequestContext requestContext,
      int poolId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, PoolSecurityProvider.NamespaceId)?.RemoveAccessControlEntries(requestContext.Elevate(), this.GetAgentPoolToken(poolId), identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (i => i.Descriptor)));
    }

    private static void ThrowPoolAccessDeniedException(
      IVssRequestContext requestContext,
      string poolName,
      int requiredPermissions)
    {
      throw new AccessDeniedException(TaskResources.AccessDeniedForPool((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", DefaultSecurityProvider.GetPermissionStrings(requiredPermissions)), (object) poolName));
    }

    private static void ThrowAccessDeniedException(
      IVssRequestContext requestContext,
      int requiredPermissions)
    {
      throw new AccessDeniedException(TaskResources.AccessDenied((object) requestContext.GetUserIdentity().DisplayName, (object) string.Join(", ", DefaultSecurityProvider.GetPermissionStrings(requiredPermissions))));
    }
  }
}
