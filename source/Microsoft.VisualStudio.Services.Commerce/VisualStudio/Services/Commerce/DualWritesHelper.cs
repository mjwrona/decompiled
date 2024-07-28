// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.DualWritesHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Migration.Utilities;
using Microsoft.VisualStudio.Services.Commerce.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal static class DualWritesHelper
  {
    private const string Area = "Commerce";
    private const string Layer = "DualWritesHelper";
    private const string lastUpdatedByDualWrites = "78189DC0-F486-4A1A-9FDF-67C9A20AD792";

    internal static void DualWriteSubscriptionResourceUsage(
      IVssRequestContext collectionContext,
      IEnumerable<SubscriptionResourceUsage> resourceUsage)
    {
      if (!DualWritesHelper.IsSRUDualWritesEnabled(collectionContext))
        return;
      collectionContext.TraceEnter(5109121, "Commerce", nameof (DualWritesHelper), nameof (DualWriteSubscriptionResourceUsage));
      if (collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.SubscriptionResourceUsageDualWrites"))
      {
        try
        {
          collectionContext.TraceAlways(5109117, TraceLevel.Info, "Commerce", nameof (DualWritesHelper), string.Format("DualWrite SRU data {0} from {1} to Commerce for the collection {2} initiated", (object) resourceUsage.ToString(), (object) collectionContext.ServiceName, (object) collectionContext.ServiceHost.InstanceId));
          IVssRequestContext context = collectionContext.To(TeamFoundationHostType.Deployment).Elevate();
          if (collectionContext.IsCommerceService())
          {
            CommerceToSpsResourceMigrationHttpClient client = context.GetClient<CommerceToSpsResourceMigrationHttpClient>();
            ResourceMigrationRequest migrationRequest = new ResourceMigrationRequest();
            migrationRequest.HostId = collectionContext.ServiceHost.InstanceId;
            migrationRequest.ResourceUsages = resourceUsage;
            IVssRequestContext userState = context;
            CancellationToken cancellationToken = new CancellationToken();
            client.CreateSubscriptionResourcesAsync(migrationRequest, (object) userState, cancellationToken).SyncResult();
          }
          else
          {
            ResourceMigrationHttpClient client = context.GetClient<ResourceMigrationHttpClient>();
            ResourceMigrationRequest migrationRequest = new ResourceMigrationRequest();
            migrationRequest.HostId = collectionContext.ServiceHost.InstanceId;
            migrationRequest.ResourceUsages = resourceUsage;
            IVssRequestContext userState = context;
            CancellationToken cancellationToken = new CancellationToken();
            client.CreateSubscriptionResourcesAsync(migrationRequest, (object) userState, cancellationToken).SyncResult();
          }
        }
        catch (Exception ex)
        {
          collectionContext.TraceException(5109118, "Commerce", nameof (DualWritesHelper), ex);
        }
        finally
        {
          collectionContext.TraceLeave(5109122, "Commerce", nameof (DualWritesHelper), nameof (DualWriteSubscriptionResourceUsage));
        }
      }
      if (!collectionContext.IsCommerceService())
        return;
      MigrationUtilities.DualWriteResourceUsage(collectionContext, resourceUsage);
    }

    internal static void DualWriteSubscriptionResourceUsage(
      IVssRequestContext collectionContext,
      int? meterId = null,
      ResourceRenewalGroup? resourceSeq = null)
    {
      if (!DualWritesHelper.IsSRUDualWritesEnabled(collectionContext))
        return;
      List<OfferSubscriptionInternal> resourceUsages = DualWritesHelper.GetResourceUsages(collectionContext, meterId, resourceSeq);
      if (resourceUsages.IsNullOrEmpty<OfferSubscriptionInternal>())
      {
        MigrationUtilities.ClearStaleOrganizationUponUnlink(collectionContext);
      }
      else
      {
        IEnumerable<SubscriptionResourceUsage> resourceUsage = resourceUsages.Select<OfferSubscriptionInternal, SubscriptionResourceUsage>((Func<OfferSubscriptionInternal, SubscriptionResourceUsage>) (offerSubInternal => offerSubInternal.ToSubscriptionResourceUsage(new Guid("78189DC0-F486-4A1A-9FDF-67C9A20AD792"))));
        DualWritesHelper.DualWriteSubscriptionResourceUsage(collectionContext, resourceUsage);
      }
    }

    internal static void SetDualWriteMode(IVssRequestContext requestContext, bool isDualWrite)
    {
      if (!requestContext.Items.ContainsKey("Commerce.DualWrite"))
        requestContext.Items.Add("Commerce.DualWrite", (object) isDualWrite);
      else
        requestContext.Items["Commerce.DualWrite"] = (object) isDualWrite;
    }

    internal static bool GetDualWriteMode(IVssRequestContext requestContext)
    {
      bool dualWriteMode;
      if (!requestContext.TryGetItem<bool>("Commerce.DualWrite", out dualWriteMode) && !requestContext.RootContext.TryGetItem<bool>("Commerce.DualWrite", out dualWriteMode))
        dualWriteMode = true;
      return dualWriteMode;
    }

    internal static void DualWriteAzureResourceAccount(
      IVssRequestContext deploymentContext,
      IEnumerable<AzureResourceAccount> azureResourceAccounts,
      Guid hostId,
      bool isLink)
    {
      deploymentContext.TraceEnter(5109123, "Commerce", nameof (DualWritesHelper), nameof (DualWriteAzureResourceAccount));
      AzureResourceAccount resourceAccount = azureResourceAccounts.Single<AzureResourceAccount>();
      if (!DualWritesHelper.IsARADualWritesEnabled(deploymentContext, resourceAccount?.AzureResourceName, resourceAccount?.AzureSubscriptionId))
        return;
      if (deploymentContext.IsFeatureEnabled("VisualStudio.Services.Commerce.AzureResourceAccountDualWrites"))
      {
        try
        {
          deploymentContext.Trace(5109119, TraceLevel.Info, "Commerce", nameof (DualWritesHelper), string.Format("DualWrite Azure Resource Account {0} from {1} to Commerce for the collection {2} initiated", (object) resourceAccount.AzureResourceName, (object) deploymentContext.ServiceName, (object) hostId));
          IVssRequestContext context = deploymentContext.Elevate();
          if (deploymentContext.IsCommerceService())
          {
            CommerceToSpsResourceMigrationHttpClient client = context.GetClient<CommerceToSpsResourceMigrationHttpClient>();
            ResourceMigrationRequest migrationRequest = new ResourceMigrationRequest();
            migrationRequest.HostId = hostId;
            migrationRequest.AzureResourceAccounts = azureResourceAccounts;
            int num = isLink ? 1 : 0;
            IVssRequestContext userState = context;
            CancellationToken cancellationToken = new CancellationToken();
            client.CreateOrDeleteAzureResourceAccountsAsync(migrationRequest, num != 0, (object) userState, cancellationToken).SyncResult();
          }
          else
          {
            ResourceMigrationHttpClient client = context.GetClient<ResourceMigrationHttpClient>();
            ResourceMigrationRequest migrationRequest = new ResourceMigrationRequest();
            migrationRequest.HostId = hostId;
            migrationRequest.AzureResourceAccounts = azureResourceAccounts;
            int num = isLink ? 1 : 0;
            IVssRequestContext userState = context;
            CancellationToken cancellationToken = new CancellationToken();
            client.CreateOrDeleteAzureResourceAccountsAsync(migrationRequest, num != 0, (object) userState, cancellationToken).SyncResult();
          }
        }
        catch (Exception ex)
        {
          deploymentContext.TraceException(5109120, "Commerce", nameof (DualWritesHelper), ex);
        }
        finally
        {
          deploymentContext.TraceLeave(5109124, "Commerce", nameof (DualWritesHelper), nameof (DualWriteAzureResourceAccount));
        }
      }
      if (!deploymentContext.IsCommerceService())
        return;
      MigrationUtilities.DualWriteResourceAccount(deploymentContext, resourceAccount, isLink);
    }

    internal static void DualWriteAzureSubscription(
      IVssRequestContext deploymentContext,
      AzureSubscriptionInternal azureSubscription)
    {
      if (!DualWritesHelper.IsARADualWritesEnabled(deploymentContext, (string) null, new Guid?(azureSubscription.AzureSubscriptionId)))
        return;
      deploymentContext.TraceEnter(5109140, "Commerce", nameof (DualWritesHelper), nameof (DualWriteAzureSubscription));
      if (deploymentContext.IsFeatureEnabled("VisualStudio.Services.Commerce.AzureResourceAccountDualWrites"))
      {
        try
        {
          deploymentContext.Trace(5109119, TraceLevel.Info, "Commerce", nameof (DualWritesHelper), string.Format("DualWrite Azure Subscription {0} from {1} to Commerce initiated", (object) azureSubscription.AzureSubscriptionId, (object) deploymentContext.ServiceName));
          IVssRequestContext context = deploymentContext.Elevate();
          if (deploymentContext.IsCommerceService())
          {
            CommerceToSpsResourceMigrationHttpClient client = context.GetClient<CommerceToSpsResourceMigrationHttpClient>();
            ResourceMigrationRequest migrationRequest = new ResourceMigrationRequest();
            migrationRequest.AzureSubscription = azureSubscription;
            IVssRequestContext userState = context;
            CancellationToken cancellationToken = new CancellationToken();
            client.CreateAzureSubscriptionForDualWritesAsync(migrationRequest, (object) userState, cancellationToken).SyncResult();
          }
          else
          {
            ResourceMigrationHttpClient client = context.GetClient<ResourceMigrationHttpClient>();
            ResourceMigrationRequest migrationRequest = new ResourceMigrationRequest();
            migrationRequest.AzureSubscription = azureSubscription;
            IVssRequestContext userState = context;
            CancellationToken cancellationToken = new CancellationToken();
            client.CreateAzureSubscriptionForDualWritesAsync(migrationRequest, (object) userState, cancellationToken).SyncResult();
          }
        }
        catch (Exception ex)
        {
          deploymentContext.TraceException(5109142, "Commerce", nameof (DualWritesHelper), ex);
        }
        finally
        {
          deploymentContext.TraceLeave(5109141, "Commerce", nameof (DualWritesHelper), nameof (DualWriteAzureSubscription));
        }
      }
      if (!deploymentContext.IsCommerceService())
        return;
      MigrationUtilities.DualWriteSubscription(deploymentContext, azureSubscription);
    }

    private static bool IsSRUDualWritesEnabled(IVssRequestContext collectionContext)
    {
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      if (collectionContext.IsCommerceService())
        return CommerceDeploymentHelper.IsCommerceMasterEnabled(collectionContext);
      return !requestContext.IsCallerCommerceServicePrincipal() && collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.SubscriptionResourceUsageDualWrites") && DualWritesHelper.GetDualWriteMode(collectionContext);
    }

    private static bool IsARADualWritesEnabled(
      IVssRequestContext deploymentContext,
      string azureResourceName,
      Guid? subscriptionId)
    {
      return deploymentContext.IsCommerceService() ? CommerceDeploymentHelper.IsCsmCommerceMasterEnabled(deploymentContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(deploymentContext, Guid.Empty, subscriptionId, resourceName: azureResourceName) : !deploymentContext.IsCallerCommerceServicePrincipal() && deploymentContext.IsFeatureEnabled("VisualStudio.Services.Commerce.AzureResourceAccountDualWrites") && (!CommerceDeploymentHelper.IsCommerceServiceAccountResourceEnabled(deploymentContext) || !CommerceDeploymentHelper.IsCommerceForwardingEnabled(deploymentContext, Guid.Empty, subscriptionId, resourceName: azureResourceName));
    }

    private static List<OfferSubscriptionInternal> GetResourceUsages(
      IVssRequestContext collectionContext,
      int? meterId = null,
      ResourceRenewalGroup? resourceSeq = null)
    {
      List<OfferSubscriptionInternal> resourceUsages = new List<OfferSubscriptionInternal>();
      try
      {
        using (CommerceMeteringComponent component = collectionContext.CreateComponent<CommerceMeteringComponent>())
        {
          byte? resourceSeq1 = new byte?();
          if (resourceSeq.HasValue)
            resourceSeq1 = new byte?((byte) resourceSeq.Value);
          resourceUsages = component.GetMeteredResources(meterId, resourceSeq1).ToList<OfferSubscriptionInternal>();
        }
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5109118, "Commerce", nameof (DualWritesHelper), ex);
      }
      return resourceUsages;
    }
  }
}
