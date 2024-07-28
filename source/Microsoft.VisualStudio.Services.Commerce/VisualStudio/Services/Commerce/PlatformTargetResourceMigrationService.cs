// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformTargetResourceMigrationService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class PlatformTargetResourceMigrationService : IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "PlatformTargetResourceMigrationService";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckHostedDeployment();
    }

    public void MigrateResources(
      IVssRequestContext requestContext,
      ResourceMigrationRequest migrationRequest,
      OperationType operationType)
    {
      try
      {
        requestContext.TraceEnter(0, "Commerce", nameof (PlatformTargetResourceMigrationService), nameof (MigrateResources));
        requestContext.CheckDeploymentRequestContext();
        this.CheckMigrationResourcesAndDualWritesPermission(requestContext);
        if (migrationRequest == null)
          return;
        switch (operationType)
        {
          case OperationType.Migration:
            this.SubscriptionResourceMigration(requestContext, migrationRequest);
            this.AzureResourceAccountMigration(requestContext, migrationRequest);
            this.CreateSubscriptionForDualWrites(requestContext, migrationRequest);
            break;
          case OperationType.Purchase:
          case OperationType.CancelPurchase:
          case OperationType.ModifyPurchase:
            this.SubscriptionResourceMigration(requestContext, migrationRequest);
            break;
          case OperationType.Link:
            this.AzureResourceAccountMigration(requestContext, migrationRequest);
            break;
          case OperationType.Unlink:
            this.UnlinkResource(requestContext, migrationRequest);
            break;
          case OperationType.CreateSubscription:
            this.CreateSubscriptionForDualWrites(requestContext, migrationRequest);
            break;
          default:
            requestContext.Trace(0, TraceLevel.Error, "Commerce", nameof (PlatformTargetResourceMigrationService), "Invalid OperationType received from the payload.");
            throw new InvalidOperationException("Invalid OperationType received from the payload.");
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(0, "Commerce", nameof (PlatformTargetResourceMigrationService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "Commerce", nameof (PlatformTargetResourceMigrationService), nameof (MigrateResources));
      }
    }

    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
    internal virtual bool CheckMigrationResourcesAndDualWritesPermission(
      IVssRequestContext requestContext,
      bool throwAccessDenied = true)
    {
      return requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext, 2, CommerceSecurity.CommerceSecurityNamespaceId, SubscriptionManagementPermissions.Tokens.CommerceResourceMigrationAndDualWritesSecurityNamespaceToken, throwAccessDenied);
    }

    private void CreateSubscriptionForDualWrites(
      IVssRequestContext deploymentContext,
      ResourceMigrationRequest migrationRequest)
    {
      if (migrationRequest.AzureSubscription == null)
        return;
      deploymentContext.Trace(5109137, TraceLevel.Info, "Commerce", nameof (PlatformTargetResourceMigrationService), "Invalidating Azure Subscription cache for the host for the subscription" + string.Format(" {0} with status ", (object) migrationRequest.AzureSubscription?.AzureSubscriptionId) + string.Format("{0}", (object) migrationRequest.AzureSubscription?.AzureSubscriptionStatusId));
      AzureSubscriptionInternal subscription = migrationRequest.AzureSubscription;
      deploymentContext.UsingSqlComponent<CommerceSqlComponent>((Action<CommerceSqlComponent>) (sqlComponent => sqlComponent.AddAzureSubscription(subscription.AzureSubscriptionId, subscription.AzureSubscriptionStatusId, subscription.AzureSubscriptionSource, subscription.AzureOfferCode)));
      deploymentContext.GetService<PlatformSubscriptionService>().InvalidateSubscriptionServiceCache(deploymentContext, subscription.AzureSubscriptionId.ToString(), new AccountProviderNamespace?(), new Guid?());
    }

    private void UnlinkResource(
      IVssRequestContext deploymentContext,
      ResourceMigrationRequest migrationRequest)
    {
      PlatformSubscriptionService service = deploymentContext.GetService<PlatformSubscriptionService>();
      foreach (AzureResourceAccount azureResourceAccount in migrationRequest.AzureResourceAccounts)
      {
        AzureResourceAccount azureResAccount = azureResourceAccount;
        deploymentContext.UsingSqlComponent<CommerceSqlComponent>((Action<CommerceSqlComponent>) (sqlComponent => sqlComponent.RemoveAzureResourceAccount(azureResAccount.AzureSubscriptionId, azureResAccount.ProviderNamespaceId, azureResAccount.AzureResourceName, azureResAccount.AzureCloudServiceName)));
        service.InvalidateSubscriptionServiceCache(deploymentContext, azureResAccount.AzureSubscriptionId.ToString(), new AccountProviderNamespace?(azureResAccount.ProviderNamespaceId), new Guid?(azureResAccount.AccountId));
      }
    }

    private void SubscriptionResourceMigration(
      IVssRequestContext requestContext,
      ResourceMigrationRequest migrationRequest)
    {
      if (migrationRequest.ResourceUsages.IsNullOrEmpty<SubscriptionResourceUsage>())
        return;
      CollectionHelper.WithCollectionContext(requestContext, migrationRequest.HostId, (Action<IVssRequestContext>) (collectionContext =>
      {
        using (CommerceMeteringComponent component = collectionContext.CreateComponent<CommerceMeteringComponent>())
          component.MigrateSubscriptionResourceUsages(migrationRequest.ResourceUsages, true);
        this.InvalidateSubscriptionResourceUsageCache(collectionContext);
      }), method: nameof (SubscriptionResourceMigration));
    }

    private void InvalidateSubscriptionResourceUsageCache(IVssRequestContext collectionContext)
    {
      try
      {
        collectionContext.TraceAlways(5109133, TraceLevel.Info, "Commerce", nameof (PlatformTargetResourceMigrationService), string.Format("Invalidating offerSubscription service cache for the host {0}", (object) collectionContext.ServiceHost.InstanceId));
        collectionContext.GetService<OfferSubscriptionCachedAccessService>().Invalidate(collectionContext);
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5109136, "Commerce", nameof (PlatformTargetResourceMigrationService), ex);
        throw;
      }
    }

    private void AzureResourceAccountMigration(
      IVssRequestContext requestContext,
      ResourceMigrationRequest migrationRequest)
    {
      if (migrationRequest.AzureResourceAccounts.IsNullOrEmpty<AzureResourceAccount>())
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      using (CommerceSqlComponent component = vssRequestContext.CreateComponent<CommerceSqlComponent>())
        component.MigrateAzureResourceAccounts(migrationRequest.AzureResourceAccounts, true);
      this.InvalidateAzureResourceAccountsCache(vssRequestContext, migrationRequest.AzureResourceAccounts);
    }

    private void InvalidateAzureResourceAccountsCache(
      IVssRequestContext deploymentContext,
      IEnumerable<AzureResourceAccount> azureResourceAccounts)
    {
      try
      {
        PlatformSubscriptionService service = deploymentContext.GetService<PlatformSubscriptionService>();
        foreach (AzureResourceAccount azureResourceAccount in azureResourceAccounts)
        {
          Guid collectionId = azureResourceAccount.CollectionId;
          Guid guid1 = new Guid();
          Guid guid2 = guid1;
          Guid guid3 = collectionId != guid2 ? azureResourceAccount.CollectionId : azureResourceAccount.AccountId;
          deploymentContext.TraceAlways(5109133, TraceLevel.Info, "Commerce", nameof (PlatformTargetResourceMigrationService), string.Format("Invalidating subscription service cache for the host {0} on subscription {1} on the provider namespace", (object) guid3, (object) azureResourceAccount.AzureSubscriptionId) + string.Format("{0}", (object) azureResourceAccount.ProviderNamespaceId));
          PlatformSubscriptionService subscriptionService = service;
          IVssRequestContext requestContext = deploymentContext;
          guid1 = azureResourceAccount.AzureSubscriptionId;
          string subscriptionId = guid1.ToString();
          AccountProviderNamespace? providerNamespaceId = new AccountProviderNamespace?(azureResourceAccount.ProviderNamespaceId);
          Guid? linkedHostId = new Guid?(guid3);
          subscriptionService.InvalidateSubscriptionServiceCache(requestContext, subscriptionId, providerNamespaceId, linkedHostId);
        }
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5109134, "Commerce", nameof (PlatformTargetResourceMigrationService), ex);
        throw;
      }
    }
  }
}
