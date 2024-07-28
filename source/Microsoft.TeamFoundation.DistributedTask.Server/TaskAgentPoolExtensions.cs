// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentPoolExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentPoolExtensions
  {
    public static TaskAgentPoolReference AsReference(this TaskAgentPool pool)
    {
      if (pool == null)
        return (TaskAgentPoolReference) null;
      return new TaskAgentPoolReference()
      {
        Id = pool.Id,
        Name = pool.Name,
        Scope = pool.Scope,
        PoolType = pool.PoolType,
        IsHosted = pool.IsHosted,
        Size = pool.Size
      };
    }

    public static bool ShouldHidePool(
      this TaskAgentPoolReference pool,
      IVssRequestContext requestContext)
    {
      return false;
    }

    public static void SetPoolVisibility(
      this TaskAgentPoolReference pool,
      IVssRequestContext requestContext)
    {
      if (!pool.IsHosted)
        return;
      switch (SingleHostedPoolMigration.GetSingleHostedPoolMigrationStage(requestContext))
      {
        case SingleHostedPoolMigrationStage.NotStarted:
          pool.IsLegacy = new bool?(false);
          break;
        case SingleHostedPoolMigrationStage.Ready:
          TaskAgentPoolReference agentPoolReference = pool;
          bool? isLegacy = pool.IsLegacy;
          bool? nullable = isLegacy.HasValue ? new bool?(!isLegacy.GetValueOrDefault()) : new bool?();
          agentPoolReference.IsLegacy = nullable;
          break;
        case SingleHostedPoolMigrationStage.SinglePoolVisible:
          pool.IsLegacy = new bool?(false);
          break;
      }
    }

    public static void Validate(this TaskAgentPool pool)
    {
      if (pool == null || pool.PoolType != TaskAgentPoolType.Deployment)
        return;
      bool? nullable = !pool.IsHosted ? pool.AutoProvision : throw new InvalidTaskAgentPoolException(TaskResources.DeploymentPoolsCanNotBeHosted());
      if (!nullable.HasValue)
        return;
      nullable = pool.AutoProvision;
      if (nullable.Value)
        throw new InvalidTaskAgentPoolException(TaskResources.DeploymentPoolsCanNotAutoProvisionQueues());
    }
  }
}
