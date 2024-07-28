// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.FrameworkSubscriptionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class FrameworkSubscriptionService : ISubscriptionService, IVssFrameworkService
  {
    private const string FrameworkSubscriptionServiceName = "FrameworkSubscriptionService";
    private const string areaName = "Commerce";
    private const string layerName = "FrameworkSubscriptionService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IEnumerable<IAzureSubscription> GetAzureSubscriptions(
      IVssRequestContext requestContext,
      List<Guid> subscriptionIds,
      AccountProviderNamespace providerNamespaceId)
    {
      List<Guid> source = subscriptionIds;
      if ((source != null ? (!source.Any<Guid>() ? 1 : 0) : 0) != 0)
        return (IEnumerable<IAzureSubscription>) new List<IAzureSubscription>();
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      try
      {
        Guid guid = subscriptionIds.IsNullOrEmpty<Guid>() ? new Guid() : subscriptionIds.FirstOrDefault<Guid>();
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<IAzureSubscription>>(requestContext, (Func<IEnumerable<IAzureSubscription>>) (() => (IEnumerable<IAzureSubscription>) this.GetCommerceHttpClient(requestContext).GetAzureSubscriptionsAsync((IEnumerable<Guid>) subscriptionIds, providerNamespaceId).SyncResult<List<AzureSubscription>>()), (Func<IEnumerable<IAzureSubscription>>) (() => this.GetHttpClient(requestContext).GetAzureSubscriptions(subscriptionIds, providerNamespaceId).SyncResult<IEnumerable<IAzureSubscription>>()), "Commerce", nameof (FrameworkSubscriptionService), 5103902, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, requestContext.ServiceHost.InstanceId, new Guid?(guid), nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        throw;
      }
    }

    public IEnumerable<ISubscriptionAccount> GetAccounts(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      FrameworkSubscriptionService.ValidateCommon(requestContext, subscriptionId);
      requestContext.TraceEnter(5103901, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAccounts));
      try
      {
        requestContext.Trace(5103902, TraceLevel.Info, "Commerce", nameof (FrameworkSubscriptionService), "Getting Accounts of subscription {0}", (object) subscriptionId);
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<ISubscriptionAccount>>(requestContext, (Func<IEnumerable<ISubscriptionAccount>>) (() => (IEnumerable<ISubscriptionAccount>) this.GetCommerceHttpClient(requestContext).GetAccountsAsync(subscriptionId, providerNamespaceId).SyncResult<List<SubscriptionAccount>>()), (Func<IEnumerable<ISubscriptionAccount>>) (() => this.GetHttpClient(requestContext).GetAccounts(subscriptionId, providerNamespaceId).SyncResult<IEnumerable<ISubscriptionAccount>>()), "Commerce", nameof (FrameworkSubscriptionService), 5103902, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, requestContext.ServiceHost.InstanceId, new Guid?(subscriptionId), nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103903, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103904, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAccounts));
      }
    }

    public IEnumerable<ISubscriptionAccount> GetAccounts(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid? memberId = null,
      bool queryOnlyOwnerAccounts = false,
      bool inlcudeDisabledAccounts = false,
      bool includeMSAAccounts = false,
      IEnumerable<Guid> serviceOwners = null,
      string galleryId = null,
      bool addUnlinkedSubscription = false,
      bool queryAccountsByUpn = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!memberId.HasValue && requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      requestContext.TraceEnter(5103913, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAccounts));
      try
      {
        requestContext.Trace(5103914, TraceLevel.Info, "Commerce", nameof (FrameworkSubscriptionService), "Getting Accounts owned by the caller");
        if (!memberId.HasValue)
          memberId = new Guid?(requestContext.GetAuthenticatedId());
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<ISubscriptionAccount>>(requestContext, (Func<IEnumerable<ISubscriptionAccount>>) (() => (IEnumerable<ISubscriptionAccount>) this.GetCommerceHttpClient(requestContext).GetAccountsByIdentityForOfferIdAsync(providerNamespace, memberId.Value, queryOnlyOwnerAccounts, inlcudeDisabledAccounts, includeMSAAccounts, serviceOwners, galleryId, new bool?(addUnlinkedSubscription), new bool?(queryAccountsByUpn), (object) null, new CancellationToken()).SyncResult<List<SubscriptionAccount>>()), (Func<IEnumerable<ISubscriptionAccount>>) (() => this.GetHttpClient(requestContext).GetAccounts(providerNamespace, memberId.Value, queryOnlyOwnerAccounts, inlcudeDisabledAccounts, includeMSAAccounts, serviceOwners, galleryId, addUnlinkedSubscription, queryAccountsByUpn).SyncResult<IEnumerable<ISubscriptionAccount>>()), "Commerce", nameof (FrameworkSubscriptionService), 5103914, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, requestContext.ServiceHost.InstanceId, layer: nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103915, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103916, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAccounts));
      }
    }

    public void CreateSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      int anniversaryDay,
      SubscriptionStatus status,
      SubscriptionSource source)
    {
      throw new NotImplementedException();
    }

    public void LinkCollection(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid? ownerId = null,
      bool hydrate = false)
    {
      FrameworkSubscriptionService.ValidateCommon(requestContext, subscriptionId, ownerId.HasValue);
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      requestContext.TraceEnter(5103909, "Commerce", nameof (FrameworkSubscriptionService), nameof (LinkCollection));
      try
      {
        if (!ownerId.HasValue)
          ownerId = new Guid?(requestContext.GetAuthenticatedId());
        requestContext.Trace(5103910, TraceLevel.Info, "Commerce", nameof (FrameworkSubscriptionService), "Linking the collection {0} to the subscription {1}", (object) accountId, (object) subscriptionId);
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceHttpClient(requestContext).LinkAccountAsync(subscriptionId, providerNamespaceId, accountId, ownerId.Value, new bool?(hydrate)).Wait()), (Action) (() => this.GetHttpClient(requestContext, accountId).LinkAccount(subscriptionId, providerNamespaceId, accountId, ownerId.Value, hydrate).Wait()), "Commerce", nameof (FrameworkSubscriptionService), 5103910, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, accountId, new Guid?(subscriptionId), nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103911, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103912, "Commerce", nameof (FrameworkSubscriptionService), nameof (LinkCollection));
      }
    }

    public void UnlinkCollection(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid collectionId,
      Guid? ownerId = null,
      bool hydrate = false)
    {
      FrameworkSubscriptionService.ValidateCommon(requestContext, subscriptionId);
      ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
      requestContext.TraceEnter(5103917, "Commerce", nameof (FrameworkSubscriptionService), nameof (UnlinkCollection));
      try
      {
        if (!ownerId.HasValue)
          ownerId = new Guid?(requestContext.GetAuthenticatedId());
        requestContext.Trace(5103918, TraceLevel.Info, "Commerce", nameof (FrameworkSubscriptionService), "Unlinking the collection {0} from the subscription {1}", (object) collectionId, (object) subscriptionId);
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceHttpClient(requestContext).UnlinkAccountAsync(subscriptionId, providerNamespaceId, collectionId, ownerId.Value, (object) hydrate).Wait()), (Action) (() => this.GetHttpClient(requestContext, collectionId).UnlinkAccount(subscriptionId, providerNamespaceId, collectionId, ownerId.Value, hydrate).Wait()), "Commerce", nameof (FrameworkSubscriptionService), 5103918, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, collectionId, new Guid?(subscriptionId), nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103919, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103920, "Commerce", nameof (FrameworkSubscriptionService), nameof (UnlinkCollection));
      }
    }

    private static void ValidateCommon(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      bool checkForDeployementContext = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      if (checkForDeployementContext && !requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      if (!checkForDeployementContext && requestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }

    public ISubscriptionAccount GetSubscriptionAccount(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid accountId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      requestContext.TraceEnter(5103921, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetSubscriptionAccount));
      try
      {
        requestContext.Trace(5103922, TraceLevel.Info, "Commerce", nameof (FrameworkSubscriptionService), "GetAccountSubscriptionId for the account {0}", (object) accountId);
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<ISubscriptionAccount>(requestContext, (Func<ISubscriptionAccount>) (() => (ISubscriptionAccount) this.GetCommerceHttpClient(requestContext).GetSubscriptionAccountAsync(providerNamespace, accountId).SyncResult<SubscriptionAccount>()), (Func<ISubscriptionAccount>) (() => this.GetHttpClient(requestContext, accountId).GetSubscriptionAccount(providerNamespace, accountId).SyncResult<ISubscriptionAccount>()), "Commerce", nameof (FrameworkSubscriptionService), 5103922, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, accountId, layer: nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103923, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103924, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetSubscriptionAccount));
      }
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.Client.SubscriptionHttpClient GetHttpClient(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      return PartitionedClientHelper.GetSpsClientForHostId<Microsoft.VisualStudio.Services.Commerce.Client.SubscriptionHttpClient>(requestContext.To(TeamFoundationHostType.Deployment), hostId);
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.Client.SubscriptionHttpClient GetHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetClient<Microsoft.VisualStudio.Services.Commerce.Client.SubscriptionHttpClient>();
    }

    internal virtual Microsoft.VisualStudio.Services.Commerce.WebApi.SubscriptionHttpClient GetCommerceHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).GetClient<Microsoft.VisualStudio.Services.Commerce.WebApi.SubscriptionHttpClient>();
    }

    public IEnumerable<ISubscriptionAccount> GetAzureSubscriptionForUser(
      IVssRequestContext requestContext,
      Guid? subscriptionId = null,
      bool queryAcrossTenants = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(5103931, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAzureSubscriptionForUser));
      try
      {
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<IEnumerable<ISubscriptionAccount>>(requestContext, (Func<IEnumerable<ISubscriptionAccount>>) (() => (IEnumerable<ISubscriptionAccount>) this.GetCommerceHttpClient(requestContext).GetAzureSubscriptionForUserAsync(subscriptionId, new bool?(queryAcrossTenants)).SyncResult<List<SubscriptionAccount>>()), (Func<IEnumerable<ISubscriptionAccount>>) (() => this.GetHttpClient(requestContext).GetAzureSubscriptionForUser(subscriptionId, queryAcrossTenants).SyncResult<IEnumerable<ISubscriptionAccount>>()), "Commerce", nameof (FrameworkSubscriptionService), 5103931, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, requestContext.ServiceHost.InstanceId, new Guid?(subscriptionId.GetValueOrDefault()), nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103933, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103934, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAzureSubscriptionForUser));
      }
    }

    public ISubscriptionAccount GetAzureSubscriptionForPurchase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string galleryItemId,
      Guid? collectionId = null)
    {
      requestContext.TraceEnter(5103941, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAzureSubscriptionForPurchase));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(galleryItemId, nameof (galleryItemId));
      try
      {
        return CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback<ISubscriptionAccount>(requestContext, (Func<ISubscriptionAccount>) (() => (ISubscriptionAccount) this.GetCommerceHttpClient(requestContext).GetAzureSubscriptionForPurchaseAsync(subscriptionId, galleryItemId, new Guid?(collectionId.GetValueOrDefault())).SyncResult<SubscriptionAccount>()), (Func<ISubscriptionAccount>) (() => this.GetHttpClient(requestContext).GetAzureSubscriptionForPurchase(subscriptionId, galleryItemId, collectionId ?? Guid.Empty).SyncResult<ISubscriptionAccount>()), "Commerce", nameof (FrameworkSubscriptionService), 5103941, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, collectionId.GetValueOrDefault(), new Guid?(subscriptionId), nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103943, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103944, "Commerce", nameof (FrameworkSubscriptionService), nameof (GetAzureSubscriptionForPurchase));
      }
    }

    public void ChangeSubscriptionCollection(
      IVssRequestContext requestContext,
      Guid newSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      bool hydrate,
      bool performValidations = false)
    {
      FrameworkSubscriptionService.ValidateCommon(requestContext, newSubscriptionId, false);
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      requestContext.TraceEnter(5103909, "Commerce", nameof (FrameworkSubscriptionService), nameof (ChangeSubscriptionCollection));
      try
      {
        requestContext.Trace(5103910, TraceLevel.Info, "Commerce", nameof (FrameworkSubscriptionService), "Swapping subscription for the account {0} with the subscription {1}", (object) accountId, (object) newSubscriptionId);
        CommerceDeploymentHelper.ExecuteFrameworkServiceWithFallback(requestContext, (Action) (() => this.GetCommerceHttpClient(requestContext).ChangeSubscriptionAccountAsync(newSubscriptionId, providerNamespaceId, accountId, new bool?(hydrate)).Wait()), (Action) (() => this.GetHttpClient(requestContext, accountId).ChangeSubscriptionAccount(newSubscriptionId, providerNamespaceId, accountId, hydrate).Wait()), "Commerce", nameof (FrameworkSubscriptionService), 5103910, CommerceDeploymentHelper.IsCommerceServiceRoutingEnabled(requestContext) || CommerceDeploymentHelper.IsCommerceForwardingEnabled(requestContext, accountId, new Guid?(newSubscriptionId), nameof (FrameworkSubscriptionService)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5103911, "Commerce", nameof (FrameworkSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5103912, "Commerce", nameof (FrameworkSubscriptionService), nameof (ChangeSubscriptionCollection));
      }
    }

    public void CreateSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      SubscriptionStatus status,
      SubscriptionSource source)
    {
      throw new NotImplementedException();
    }

    public ISubscriptionAccount GetAzureSubscriptionFromName(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string accountName,
      AccountProviderNamespace providerNamespaceId,
      IEnumerable<Guid> serviceOwners)
    {
      throw new NotImplementedException();
    }

    public bool IsAssignmentBillingEnabled(IVssRequestContext requestContext, Guid accountId) => this.GetCommerceHttpClient(requestContext).IsAssignmentBillingEnabledAsync(accountId).SyncResult<bool>();

    public bool IsProjectCollectionAdmin(
      IVssRequestContext requestContext,
      Guid memberId,
      Guid collectionId)
    {
      return this.GetCommerceHttpClient(requestContext).IsProjectCollectionAdminAsync(memberId, collectionId).SyncResult<bool>();
    }
  }
}
