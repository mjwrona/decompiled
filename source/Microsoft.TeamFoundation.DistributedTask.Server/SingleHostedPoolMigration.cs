// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.SingleHostedPoolMigration
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class SingleHostedPoolMigration
  {
    public static SingleHostedPoolMigrationStage GetSingleHostedPoolMigrationStage(
      IVssRequestContext requestContext)
    {
      bool flag = requestContext.IsFeatureEnabled("DistributedTask.DisableSingleHostedPool");
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      SingleHostedPoolMigrationStage poolMigrationStage = (SingleHostedPoolMigrationStage) vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/DistributedTask/Settings/HostedPoolMigrationStage", 0);
      if (flag || poolMigrationStage != SingleHostedPoolMigrationStage.RedirectToSinglePool)
        requestContext.TraceAlways(10015210, "DistributedTask", string.Format("Unexpected SingleHostedPoolMigration State, isSingleHostedPoolBlocked:{0} registryValue:{1}", (object) flag, (object) poolMigrationStage));
      return flag && poolMigrationStage > SingleHostedPoolMigrationStage.Ready ? SingleHostedPoolMigrationStage.Ready : poolMigrationStage;
    }

    public static bool ShouldRunDemandsOnSingleHostedPool(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("DistributedTask.RunJobsWithDemandsOnSingleHostedPool") && !requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) RegistryKeys.DemandsOnSingleHostedPoolBlockedRegistrySettingsPath, false);
  }
}
