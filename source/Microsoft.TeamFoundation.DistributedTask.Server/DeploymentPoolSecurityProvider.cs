// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentPoolSecurityProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class DeploymentPoolSecurityProvider : 
    PoolSecurityProvider,
    IAgentPoolSecurityProvider
  {
    public static readonly string AgentPoolToken = "DeploymentPools";
    public static readonly string AgentPoolRoleScopeId = "distributedtask.deploymentpoolrole";
    public static readonly string GlobalAgentPoolRoleScopeId = "distributedtask.globaldeploymentpoolrole";

    internal DeploymentPoolSecurityProvider()
    {
    }

    protected override string AgentPoolSecurityToken => DeploymentPoolSecurityProvider.AgentPoolToken;

    public override bool HasPoolPermission(
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
  }
}
