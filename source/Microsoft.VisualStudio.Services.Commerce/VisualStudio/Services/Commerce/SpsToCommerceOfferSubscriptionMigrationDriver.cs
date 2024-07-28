// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SpsToCommerceOfferSubscriptionMigrationDriver
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class SpsToCommerceOfferSubscriptionMigrationDriver : IResourceMigrationDriver
  {
    private static readonly Guid MigrationUpdatedByIdentifier = new Guid("A0670F97-98D4-43E1-B042-8684D05181F6");
    private const string Layer = "SpsToCommerceOfferSubscriptionMigrationDriver";
    private const string Area = "Commerce";

    public SpsToCommerceOfferSubscriptionMigrationDriver(IVssRequestContext requestContext)
    {
    }

    public void ExecuteMigration(
      IVssRequestContext requestContext,
      ServicingContext servicingContext)
    {
      servicingContext.LogInfo(string.Format("Executing {0} for host {1}", (object) nameof (SpsToCommerceOfferSubscriptionMigrationDriver), (object) requestContext.ServiceHost.InstanceId));
      ITeamFoundationFeatureAvailabilityService service1 = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      try
      {
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableResourceMigrationFromSps"))
          servicingContext.LogInfo("Resource Migration from SPS is disabled.");
        else if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.ResourceMigrationFromSpsCompleted"))
        {
          servicingContext.LogInfo(string.Format("Migrate {0}: host is already migrated.", (object) requestContext.ServiceHost.InstanceId));
        }
        else
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress"))
            throw new InvalidOperationException(string.Format("Collection {0} trying to migrate while already migrating.", (object) requestContext.ServiceHost.InstanceId));
          this.StartSourceMigration(requestContext);
          IEnumerable<SubscriptionResourceUsage> resourceUsages = requestContext.GetService<PlatformSourceResourceMigrationService>().GetResourceUsages(requestContext, SpsToCommerceOfferSubscriptionMigrationDriver.MigrationUpdatedByIdentifier);
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          PlatformSubscriptionService service2 = vssRequestContext.GetService<PlatformSubscriptionService>();
          AzureResourceAccount accountByCollectionId = service2.GetAzureResourceAccountByCollectionId(vssRequestContext, requestContext.ServiceHost.InstanceId);
          if (!resourceUsages.IsNullOrEmpty<SubscriptionResourceUsage>() || accountByCollectionId != null)
          {
            AzureSubscriptionInternal azureSubscription = accountByCollectionId != null ? service2.GetAzureSubscription(vssRequestContext, accountByCollectionId.AzureSubscriptionId) : (AzureSubscriptionInternal) null;
            this.MigrateToTarget(vssRequestContext, servicingContext, requestContext.ServiceHost.InstanceId, resourceUsages, accountByCollectionId, azureSubscription);
          }
          this.CompleteSourceMigration(requestContext, servicingContext, resourceUsages, accountByCollectionId);
        }
      }
      catch (ServiceOwnerNotFoundException ex)
      {
        requestContext.TraceException(0, "Commerce", nameof (SpsToCommerceOfferSubscriptionMigrationDriver), (Exception) ex);
        servicingContext.Warn(string.Format("Commerce service unavailable, so ignoring this step for the host {0}", (object) requestContext.ServiceHost.InstanceId));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress"))
        {
          servicingContext.LogInfo(string.Format("Resetting the Feature flag {0} for the host {1}", (object) "VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress", (object) requestContext.ServiceHost.InstanceId));
          service1.SetFeatureState(requestContext, "VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress", FeatureAvailabilityState.Off);
        }
        servicingContext.LogInfo(string.Format("Resetting the Feature flag {0} for the host {1}", (object) "VisualStudio.Services.Commerce.ResourceMigrationFromSpsCompleted", (object) requestContext.ServiceHost.InstanceId));
        service1.SetFeatureState(requestContext, "VisualStudio.Services.Commerce.ResourceMigrationFromSpsCompleted", FeatureAvailabilityState.Off);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Commerce", nameof (SpsToCommerceOfferSubscriptionMigrationDriver), ex);
        servicingContext.LogInfo(string.Format("Resetting the Feature flags for the host {0}", (object) requestContext.ServiceHost.InstanceId));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress"))
        {
          servicingContext.LogInfo(string.Format("Resetting the Feature flag {0} for the host {1}", (object) "VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress", (object) requestContext.ServiceHost.InstanceId));
          service1.SetFeatureState(requestContext, "VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress", FeatureAvailabilityState.Off);
        }
        servicingContext.LogInfo(string.Format("Resetting the Feature flag {0} for the host {1}", (object) "VisualStudio.Services.Commerce.ResourceMigrationFromSpsCompleted", (object) requestContext.ServiceHost.InstanceId));
        service1.SetFeatureState(requestContext, "VisualStudio.Services.Commerce.ResourceMigrationFromSpsCompleted", FeatureAvailabilityState.Off);
        throw;
      }
    }

    private void StartSourceMigration(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(requestContext, "VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress", FeatureAvailabilityState.On);

    private void MigrateToTarget(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      Guid collectionId,
      IEnumerable<SubscriptionResourceUsage> resourceUsages,
      AzureResourceAccount azureResourceAccount,
      AzureSubscriptionInternal azureSubscription)
    {
      servicingContext.LogInfo(string.Format("Migrate {0}: found {1} subscriptions.", (object) requestContext.ServiceHost.InstanceId, (object) resourceUsages.Count<SubscriptionResourceUsage>()));
      requestContext.TraceProperties<IEnumerable<SubscriptionResourceUsage>>(5108779, "Commerce", nameof (SpsToCommerceOfferSubscriptionMigrationDriver), resourceUsages, (string) null);
      ResourceMigrationHttpClient client = requestContext.GetClient<ResourceMigrationHttpClient>();
      ResourceMigrationRequest migrationRequest1 = new ResourceMigrationRequest();
      migrationRequest1.HostId = collectionId;
      migrationRequest1.ResourceUsages = resourceUsages;
      ResourceMigrationRequest migrationRequest2 = migrationRequest1;
      AzureResourceAccount[] azureResourceAccountArray;
      if (azureResourceAccount == null)
        azureResourceAccountArray = (AzureResourceAccount[]) null;
      else
        azureResourceAccountArray = new AzureResourceAccount[1]
        {
          azureResourceAccount
        };
      migrationRequest2.AzureResourceAccounts = (IEnumerable<AzureResourceAccount>) azureResourceAccountArray;
      migrationRequest1.AzureSubscription = azureSubscription;
      ResourceMigrationRequest migrationRequest3 = migrationRequest1;
      IVssRequestContext userState = requestContext.Elevate();
      CancellationToken cancellationToken = new CancellationToken();
      client.MigrateResourcesAsync(migrationRequest3, (object) userState, cancellationToken).SyncResult();
    }

    private void CompleteSourceMigration(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      IEnumerable<SubscriptionResourceUsage> resourceUsages,
      AzureResourceAccount azureResourceAccount)
    {
      if (!resourceUsages.IsNullOrEmpty<SubscriptionResourceUsage>())
      {
        using (CommerceMeteringComponent component = requestContext.CreateComponent<CommerceMeteringComponent>())
          component.MigrateSubscriptionResourceUsages(resourceUsages, false);
      }
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      if (azureResourceAccount != null)
      {
        using (CommerceSqlComponent component = context.CreateComponent<CommerceSqlComponent>())
          component.MigrateAzureResourceAccounts((IEnumerable<AzureResourceAccount>) new AzureResourceAccount[1]
          {
            azureResourceAccount
          }, false);
      }
      ITeamFoundationFeatureAvailabilityService service = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      service.SetFeatureState(requestContext, "VisualStudio.Services.Commerce.ResourceMigrationFromSpsCompleted", FeatureAvailabilityState.On);
      service.SetFeatureState(requestContext, "VisualStudio.Services.Commerce.ResourceMigrationFromSpsInProgress", FeatureAvailabilityState.Off);
      servicingContext.LogInfo(string.Format("Migrate {0}: migration complete.", (object) requestContext.ServiceHost.InstanceId));
    }
  }
}
