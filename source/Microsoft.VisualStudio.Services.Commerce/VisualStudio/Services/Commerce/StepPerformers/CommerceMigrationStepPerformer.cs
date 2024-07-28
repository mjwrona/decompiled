// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.StepPerformers.CommerceMigrationStepPerformer
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Migration;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.StepPerformers
{
  [StepPerformer("CommerceMigration")]
  public class CommerceMigrationStepPerformer : TeamFoundationStepPerformerBase
  {
    [ServicingStep]
    public void Migrate(IVssRequestContext requestContext, ServicingContext servicingContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      servicingContext.LogInfo(string.Format("Migrating resources for host {0}", (object) requestContext.ServiceHost.InstanceId));
      requestContext.GetService<IMigratorService>().MigrateData(requestContext, servicingContext);
    }

    [ServicingStep]
    public void MigrateSubscription(
      IVssRequestContext requestContext,
      ServicingContext servicingContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      servicingContext.LogInfo(string.Format("Migrating subscription for host {0}", (object) requestContext.ServiceHost.InstanceId));
      IMigratorService service1 = requestContext.GetService<IMigratorService>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      PlatformSubscriptionService service2 = vssRequestContext.GetService<PlatformSubscriptionService>();
      AzureResourceAccount accountByCollectionId = service2.GetAzureResourceAccountByCollectionId(vssRequestContext, requestContext.ServiceHost.InstanceId, true);
      if (accountByCollectionId == null)
        return;
      AzureSubscriptionInternal azureSubscription = service2.GetAzureSubscription(vssRequestContext, accountByCollectionId.AzureSubscriptionId);
      if (azureSubscription.AzureSubscriptionId != Guid.Empty)
        service1.DualWriteSubscription(requestContext, azureSubscription, azureSubscription.AzureSubscriptionId);
      if (!requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableAzCommMigrationServiceDualWriteResourceAccounts"))
        return;
      service1.DualWriteResourceAccounts(requestContext, accountByCollectionId, requestContext.ServiceHost.InstanceId, accountByCollectionId.AzureSubscriptionId != Guid.Empty);
    }
  }
}
