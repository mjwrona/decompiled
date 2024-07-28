// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformSubscriptionService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Commerce.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.HostAcquisition;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.UserMapping;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformSubscriptionService : ISubscriptionService, IVssFrameworkService
  {
    private IVssDateTimeProvider dateTimeProvider;
    private INotificationPublisher _notificationPublisher;
    private readonly PlatformSubscriptionService.CollectionNameResolver collectionNameResolver;
    private IBillingMessageHelper billingChangeMessageHelper;
    private const string Area = "Commerce";
    private const string AdminCheck = "IsPcaCheck";
    private const string Layer = "PlatformSubscriptionService";
    private const string IbizaAccountRegionPath = "/Service/Commerce/Commerce/IbizaAccountRegion";
    private const string IbizaAccountRegionDefault = "SCUS";

    public PlatformSubscriptionService()
      : this(VssDateTimeProvider.DefaultProvider, PlatformSubscriptionService.\u003C\u003EO.\u003C0\u003E__TryGetCollectionServiceHostId ?? (PlatformSubscriptionService.\u003C\u003EO.\u003C0\u003E__TryGetCollectionServiceHostId = new PlatformSubscriptionService.CollectionNameResolver(HostNameResolver.TryGetCollectionServiceHostId)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public PlatformSubscriptionService(IVssDateTimeProvider dateTimeProvider)
      : this(dateTimeProvider, PlatformSubscriptionService.\u003C\u003EO.\u003C0\u003E__TryGetCollectionServiceHostId ?? (PlatformSubscriptionService.\u003C\u003EO.\u003C0\u003E__TryGetCollectionServiceHostId = new PlatformSubscriptionService.CollectionNameResolver(HostNameResolver.TryGetCollectionServiceHostId)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public PlatformSubscriptionService(
      IVssDateTimeProvider dateTimeProvider,
      PlatformSubscriptionService.CollectionNameResolver hostNameResolver)
    {
      this.dateTimeProvider = dateTimeProvider;
      this.collectionNameResolver = hostNameResolver;
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.Trace(5105211, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "PlatformSubscriptionService starting");
      if (!systemRequestContext.ServiceHost.HostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.Trace(5105212, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Loading notification publisher plugin.");
      this._notificationPublisher = this.GetNotificationPublisher(systemRequestContext);
      if (this._notificationPublisher == null)
        systemRequestContext.Trace(5105213, TraceLevel.Warning, "Commerce", nameof (PlatformSubscriptionService), "Loading notification publisher plugin failed, could not locate a valid notification publisher.");
      else
        systemRequestContext.Trace(5105213, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Loading notification publisher plugin completed successfully.");
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.Trace(5105220, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "PlatformSubscriptionService ending");

    [ExcludeFromCodeCoverage]
    internal virtual INotificationPublisher NotificationPublisher => this._notificationPublisher;

    [ExcludeFromCodeCoverage]
    internal virtual Type GetTypeFromName(string typeName) => Type.GetType(typeName, true);

    [ExcludeFromCodeCoverage]
    internal virtual HydrationHelper GetHydrationHelper() => new HydrationHelper();

    [ExcludeFromCodeCoverage]
    internal virtual void UpdateAzureResourceAccountAssociatedResources(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount,
      Dictionary<string, string> meterProperties)
    {
      Guid hostId = azureResourceAccount.CollectionId != new Guid() ? azureResourceAccount.CollectionId : azureResourceAccount.AccountId;
      IntrinsicSettingsManager.UpdateResourceSettings(requestContext, hostId, meterProperties);
    }

    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
    internal virtual bool CheckPermission(
      IVssRequestContext requestContext,
      int permissions,
      bool throwAccessDenied = true)
    {
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext, permissions, CommerceSecurity.CommerceSecurityNamespaceId, throwAccessDenied: throwAccessDenied);
    }

    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
    internal virtual bool CheckChangeSubscriptionPermission(
      IVssRequestContext requestContext,
      bool isCollectionPermissionCheck = true,
      bool throwAccessDenied = true)
    {
      return requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext, 4, isCollectionPermissionCheck ? CollectionBasedPermission.NamespaceId : CommerceSecurity.CommerceSecurityNamespaceId, SubscriptionManagementPermissions.Tokens.CommerceChangeSubscriptionSecurityNamespaceToken, throwAccessDenied);
    }

    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
    internal virtual bool CheckSubscriptionLinkPermission(IVssRequestContext requestContext) => requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext, 2, CollectionBasedPermission.NamespaceId, "/AccountLink");

    [ExcludeFromCodeCoverage]
    internal virtual IBillingMessageHelper BillingChangeMessageHelper => this.billingChangeMessageHelper ?? (this.billingChangeMessageHelper = (IBillingMessageHelper) new BillingMessageHelper());

    internal virtual INotificationPublisher GetNotificationPublisher(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5105541, "Commerce", nameof (PlatformSubscriptionService), nameof (GetNotificationPublisher));
      try
      {
        string typeName = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Service/Commerce/NotificationPublisher/Provider", string.Empty);
        requestContext.Trace(5105542, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Message provider is " + typeName);
        return Activator.CreateInstance(this.GetTypeFromName(typeName)) as INotificationPublisher;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5105549, "Commerce", nameof (PlatformSubscriptionService), ex);
        return (INotificationPublisher) null;
      }
      finally
      {
        requestContext.TraceLeave(5105550, "Commerce", nameof (PlatformSubscriptionService), nameof (GetNotificationPublisher));
      }
    }

    internal virtual void AddAzureSubscriptionToDatabase(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal subscription)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.UsingSqlComponent<CommerceSqlComponent>((Action<CommerceSqlComponent>) (sqlComponent => sqlComponent.AddAzureSubscription(subscription.AzureSubscriptionId, subscription.AzureSubscriptionStatusId, subscription.AzureSubscriptionSource, subscription.AzureOfferCode)));
      this.InvalidateSubscriptionServiceCache(vssRequestContext, subscription.AzureSubscriptionId.ToString(), new AccountProviderNamespace?(), new Guid?());
      DualWritesHelper.DualWriteAzureSubscription(vssRequestContext, subscription);
    }

    internal virtual void UpdateAzureResourceAccountToDatabase(
      IVssRequestContext requestContext,
      AzureResourceAccount resourceAccount)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.UsingSqlComponent<CommerceSqlComponent>((Action<CommerceSqlComponent>) (sqlComponent => sqlComponent.UpdateAzureResourceAccount(resourceAccount.AccountId, resourceAccount.CollectionId, resourceAccount.AzureSubscriptionId, resourceAccount.ProviderNamespaceId, resourceAccount.AzureCloudServiceName, resourceAccount.AzureResourceName)));
      DualWritesHelper.DualWriteAzureResourceAccount(vssRequestContext, (IEnumerable<AzureResourceAccount>) new AzureResourceAccount[1]
      {
        resourceAccount
      }, resourceAccount.CollectionId, true);
    }

    internal virtual void AddAzureResourceAccountToDatabase(
      IVssRequestContext requestContext,
      AzureResourceAccount resourceAccount)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.UsingSqlComponent<CommerceSqlComponent>((Action<CommerceSqlComponent>) (sqlComponent => sqlComponent.AddAzureResourceAccount(resourceAccount.AccountId, resourceAccount.CollectionId, resourceAccount.AzureSubscriptionId, resourceAccount.ProviderNamespaceId, resourceAccount.AzureCloudServiceName, resourceAccount.AlternateCloudServiceName, resourceAccount.AzureResourceName, resourceAccount.ETag, resourceAccount.AzureGeoRegion, resourceAccount.OperationResult)));
      DualWritesHelper.DualWriteAzureResourceAccount(vssRequestContext, (IEnumerable<AzureResourceAccount>) new AzureResourceAccount[1]
      {
        resourceAccount
      }, resourceAccount.CollectionId, true);
    }

    internal virtual void DeleteAzureResourceAccountFromDatabase(
      IVssRequestContext collectionContext,
      AzureResourceAccount resourceAccount)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.UsingSqlComponent<CommerceSqlComponent>((Action<CommerceSqlComponent>) (sqlComponent => sqlComponent.RemoveAzureResourceAccount(resourceAccount.AzureSubscriptionId, resourceAccount.ProviderNamespaceId, resourceAccount.AzureResourceName, resourceAccount.AzureCloudServiceName)));
      this.InvalidateSubscriptionServiceCache(vssRequestContext, resourceAccount.AzureSubscriptionId.ToString(), new AccountProviderNamespace?(resourceAccount.ProviderNamespaceId), new Guid?(resourceAccount.AccountId));
      DualWritesHelper.DualWriteAzureResourceAccount(vssRequestContext, (IEnumerable<AzureResourceAccount>) new AzureResourceAccount[1]
      {
        resourceAccount
      }, resourceAccount.CollectionId, false);
    }

    internal virtual IEnumerable<AzureResourceAccount> DeleteCloudServiceFromDatabase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string cloudServiceName)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, IEnumerable<AzureResourceAccount>>((Func<CommerceSqlComponent, IEnumerable<AzureResourceAccount>>) (sqlComponent => sqlComponent.RemoveCloudService(subscriptionId, providerNamespaceId, cloudServiceName, true)));
    }

    internal virtual AzureSubscriptionInternal GetAzureSubscriptionFromDatabase(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, AzureSubscriptionInternal>((Func<CommerceSqlComponent, AzureSubscriptionInternal>) (sqlComponent => sqlComponent.GetAzureSubscription(subscriptionId)));
    }

    internal virtual IEnumerable<AzureResourceAccount> GetAzureResourceAccountsBySubscriptionIdFromDatabase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, IEnumerable<AzureResourceAccount>>((Func<CommerceSqlComponent, IEnumerable<AzureResourceAccount>>) (sqlComponent => sqlComponent.GetAzureResourceAccountsBySubscriptionId(subscriptionId, providerNamespaceId, true)));
    }

    internal virtual IEnumerable<AzureResourceAccount> GetAllAzureResourceAccountsBySubscriptionIdFromDatabase(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, IEnumerable<AzureResourceAccount>>((Func<CommerceSqlComponent, IEnumerable<AzureResourceAccount>>) (sqlComponent => sqlComponent.GetAzureResourceAccountsBySubscriptionId(subscriptionId, true)));
    }

    internal virtual AzureResourceAccount GetAzureResourceAccountFromDatabase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureResourceAccountName)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, AzureResourceAccount>((Func<CommerceSqlComponent, AzureResourceAccount>) (sqlComponent => sqlComponent.GetAzureResourceAccount(subscriptionId, providerNamespaceId, azureResourceAccountName, true)));
    }

    internal virtual AzureResourceAccount GetAzureResourceAccountByCollectionIdFromDatabase(
      IVssRequestContext requestContext,
      Guid collectionId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, AzureResourceAccount>((Func<CommerceSqlComponent, AzureResourceAccount>) (sqlComponent => sqlComponent.GetAzureResourceAccountByAccountId(collectionId, true)));
    }

    internal virtual int GetAzureResourceAccountsCountBySubscriptionIdFromDatabase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, int>((Func<CommerceSqlComponent, int>) (sqlComponent => sqlComponent.GetAzureResourceAccountsCountBySubscriptionId(subscriptionId, providerNamespaceId)));
    }

    internal virtual IList<AzureResourceAccount> GetAzureResourceAccountsInResourceGroupFromDatabase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string resourceGroupName)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, IList<AzureResourceAccount>>((Func<CommerceSqlComponent, IList<AzureResourceAccount>>) (sqlComponent => sqlComponent.GetAzureResourceAccountsInResourceGroup(subscriptionId, providerNamespaceId, resourceGroupName, true)));
    }

    internal virtual List<AzureSubscriptionAccount> GetSubscriptionsForAccountsFromDatabase(
      IVssRequestContext requestContext,
      IEnumerable<Guid> accountIds,
      AccountProviderNamespace providerNamespaceId)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).UsingSqlComponent<CommerceSqlComponent, List<AzureSubscriptionAccount>>((Func<CommerceSqlComponent, List<AzureSubscriptionAccount>>) (sqlComponent => sqlComponent.GetSubscriptionsForAccounts(accountIds, providerNamespaceId, true)));
    }

    public virtual AzureResourceAccount GetAzureResourceAccount(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string azureResourceAccountName,
      bool bypassCache = false)
    {
      requestContext.TraceEnter(5105221, "Commerce", nameof (PlatformSubscriptionService), new object[4]
      {
        (object) subscriptionId,
        (object) providerNamespaceId,
        (object) azureResourceAccountName,
        (object) bypassCache
      }, nameof (GetAzureResourceAccount));
      try
      {
        this.CheckPermission(requestContext, 1);
        ICommerceCache service = requestContext.GetService<ICommerceCache>();
        IEnumerable<AzureResourceAccount> source;
        if (!bypassCache && service.TryGet<IEnumerable<AzureResourceAccount>>(requestContext, PlatformSubscriptionService.GetAzureResourceAccountCacheKey(subscriptionId.ToString(), providerNamespaceId.ToString()), out source))
        {
          AzureResourceAccount azureResourceAccount = source.FirstOrDefault<AzureResourceAccount>((Func<AzureResourceAccount, bool>) (x => x.AzureResourceName == azureResourceAccountName));
          if (azureResourceAccount != null)
            return azureResourceAccount;
        }
        AzureResourceAccount accountFromDatabase = this.GetAzureResourceAccountFromDatabase(requestContext, subscriptionId, providerNamespaceId, azureResourceAccountName);
        if (accountFromDatabase != null)
        {
          Guid accountId = accountFromDatabase.AccountId;
          service.TrySet<AzureResourceAccount>(requestContext, accountId.ToString(), accountFromDatabase);
        }
        return accountFromDatabase;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5105229, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5105230, "Commerce", nameof (PlatformSubscriptionService), nameof (GetAzureResourceAccount));
      }
    }

    public virtual AzureResourceAccount GetAzureResourceAccountByCollectionId(
      IVssRequestContext requestContext,
      Guid collectionId,
      bool bypassCache = false)
    {
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      deploymentContext.TraceEnter(5105231, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) collectionId,
        (object) bypassCache
      }, nameof (GetAzureResourceAccountByCollectionId));
      try
      {
        this.CheckPermission(deploymentContext, 1);
        return deploymentContext.GetService<ICommerceCache>().GetOrSet<AzureResourceAccount>(deploymentContext, collectionId.ToString(), (Func<AzureResourceAccount>) (() => this.GetAzureResourceAccountByCollectionIdFromDatabase(deploymentContext, collectionId)), bypassCache);
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5105239, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(5105230, "Commerce", nameof (PlatformSubscriptionService), nameof (GetAzureResourceAccountByCollectionId));
      }
    }

    public virtual IEnumerable<AzureResourceAccount> QueryForAzureResourceAccountsBySubscriptionId(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      deploymentContext.TraceEnter(5105241, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) subscriptionId,
        (object) providerNamespaceId
      }, nameof (QueryForAzureResourceAccountsBySubscriptionId));
      try
      {
        this.CheckPermission(deploymentContext, 1);
        return deploymentContext.GetService<ICommerceCache>().GetOrSet<IEnumerable<AzureResourceAccount>>(deploymentContext, PlatformSubscriptionService.GetAzureResourceAccountCacheKey(subscriptionId.ToString(), providerNamespaceId.ToString()), (Func<IEnumerable<AzureResourceAccount>>) (() => this.GetAzureResourceAccountsBySubscriptionIdFromDatabase(deploymentContext, subscriptionId, providerNamespaceId)));
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5105249, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(5105250, "Commerce", nameof (PlatformSubscriptionService), nameof (QueryForAzureResourceAccountsBySubscriptionId));
      }
    }

    public virtual int GetAzureResourceAccountsCountBySubscriptionId(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext1.TraceEnter(5105251, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) subscriptionId,
        (object) providerNamespaceId
      }, nameof (GetAzureResourceAccountsCountBySubscriptionId));
      try
      {
        this.CheckPermission(requestContext1, 1);
        return this.GetAzureResourceAccountsCountBySubscriptionIdFromDatabase(requestContext1, subscriptionId, providerNamespaceId);
      }
      catch (Exception ex)
      {
        requestContext1.TraceException(5105259, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext1.TraceLeave(5105260, "Commerce", nameof (PlatformSubscriptionService), nameof (GetAzureResourceAccountsCountBySubscriptionId));
      }
    }

    public virtual void CreateAzureResourceAccount(
      IVssRequestContext requestContext,
      AzureResourceAccount resourceAccount,
      bool onlyUpdate = false)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.TraceEnter(5105261, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) resourceAccount,
        (object) onlyUpdate
      }, nameof (CreateAzureResourceAccount));
      try
      {
        this.CheckPermission(vssRequestContext, 2);
        AzureSubscriptionInternal subscription = this.EnsureSubscriptionExistForAzureResourceAccount(vssRequestContext, resourceAccount);
        if (subscription.AzureSubscriptionStatusId != SubscriptionStatus.Active)
        {
          subscription.AzureSubscriptionStatusId = subscription.AzureSubscriptionStatusId == SubscriptionStatus.Unregistered ? SubscriptionStatus.Active : throw new AzureSubscriptionDisabledException(subscription.AzureSubscriptionId);
          this.AddAzureSubscriptionToDatabase(requestContext, subscription);
        }
        resourceAccount = (AzureResourceAccount) resourceAccount.Clone();
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableHostIdUpdateFill") && resourceAccount.CollectionId != Guid.Empty && resourceAccount.AccountId == resourceAccount.CollectionId)
          resourceAccount.AccountId = CollectionHelper.GetParentOrganizationId(requestContext, resourceAccount.CollectionId);
        if (onlyUpdate)
          this.UpdateAzureResourceAccountToDatabase(vssRequestContext, resourceAccount);
        else
          this.AddAzureResourceAccountToDatabase(vssRequestContext, resourceAccount);
        Guid guid = resourceAccount.CollectionId != new Guid() ? resourceAccount.CollectionId : resourceAccount.AccountId;
        this.InvalidateSubscriptionServiceCache(vssRequestContext, resourceAccount.AzureSubscriptionId.ToString(), new AccountProviderNamespace?(resourceAccount.ProviderNamespaceId), new Guid?(guid));
        IArmAdapterService service = vssRequestContext.GetService<IArmAdapterService>();
        if (vssRequestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.UseRequestIdentityToArmCall"))
          service.RegisterSubscriptionAgainstResourceProvider(requestContext, subscription.AzureSubscriptionId);
        else
          service.RegisterSubscriptionAgainstResourceProvider(vssRequestContext, subscription.AzureSubscriptionId);
      }
      catch (Exception ex)
      {
        vssRequestContext.TraceException(5105269, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        vssRequestContext.TraceLeave(5105270, "Commerce", nameof (PlatformSubscriptionService), nameof (CreateAzureResourceAccount));
      }
    }

    public virtual void UpdateAzureResourceAccount(
      IVssRequestContext requestContext,
      AzureResourceAccount resourceAccount,
      bool onlyUpdate = false)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext1.TraceEnter(5105271, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) resourceAccount,
        (object) onlyUpdate
      }, nameof (UpdateAzureResourceAccount));
      try
      {
        this.CheckPermission(requestContext1, 2);
        this.CreateAzureResourceAccount(requestContext1, resourceAccount, onlyUpdate);
      }
      finally
      {
        requestContext1.TraceLeave(5105280, "Commerce", nameof (PlatformSubscriptionService), nameof (UpdateAzureResourceAccount));
      }
    }

    public virtual void DeleteAzureResourceAccount(
      IVssRequestContext requestContext,
      AzureResourceAccount resourceAccount,
      CustomerIntelligenceEntryPoint source = CustomerIntelligenceEntryPoint.Azure)
    {
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      deploymentContext.TraceEnter(5105281, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) resourceAccount,
        (object) source
      }, nameof (DeleteAzureResourceAccount));
      try
      {
        this.CheckPermission(deploymentContext, 2);
        if (resourceAccount.AccountId != Guid.Empty)
        {
          try
          {
            requestContext.GetService<CommerceHostManagementService>().ExecuteInCollectionContext(requestContext, resourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
            {
              List<IOfferSubscription> list = collectionContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(collectionContext).ToList<IOfferSubscription>();
              this.DeleteAzureResourceAccountFromDatabase(collectionContext, resourceAccount);
              this.TraceBIEventForAzureSubscriptionChanges(collectionContext, collectionContext.ServiceHost.InstanceId, resourceAccount, (AzureSubscriptionInternal) null, true);
              this.TraceBIEventOnAzureSubscriptionDeactivation(collectionContext, resourceAccount, (IEnumerable<IOfferSubscription>) list, true);
              this.ResetCurrentQuantityUponUnlink(deploymentContext, resourceAccount.AccountId);
              if (!AssignmentBillingHelper.IsAssignmentBillingEnabledForSubscription(deploymentContext, resourceAccount.AzureSubscriptionId))
                return;
              this.ResetIncludedQuantityForAssignmentBilling(collectionContext, deploymentContext, resourceAccount);
            }), method: nameof (DeleteAzureResourceAccount));
          }
          catch (HostDoesNotExistException ex)
          {
            this.DeleteAzureResourceAccountFromDatabase(requestContext, resourceAccount);
            this.TraceBIEventForAzureSubscriptionChanges(requestContext, resourceAccount.AccountId, resourceAccount, (AzureSubscriptionInternal) null, true);
          }
        }
        this.AddAzureResourceAccountDeleteCIEvent(deploymentContext, resourceAccount.AzureSubscriptionId, resourceAccount.ProviderNamespaceId, resourceAccount.AccountId, source);
        this.PublishAccountStatusChangeNotification(deploymentContext, resourceAccount.AzureSubscriptionId, resourceAccount.AccountId, resourceAccount.ProviderNamespaceId, AccountStatusChangeEventType.UnlinkedFromSubscription, 5105315);
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5105289, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(5105290, "Commerce", nameof (PlatformSubscriptionService), nameof (DeleteAzureResourceAccount));
      }
    }

    public virtual IEnumerable<AzureResourceAccount> DeleteAzureCloudService(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string cloudServiceName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.TraceEnter(5105591, "Commerce", nameof (PlatformSubscriptionService), new object[3]
      {
        (object) subscriptionId,
        (object) providerNamespaceId,
        (object) cloudServiceName
      }, nameof (DeleteAzureCloudService));
      try
      {
        this.CheckPermission(vssRequestContext, 2);
        this.InvalidateSubscriptionServiceCache(vssRequestContext, subscriptionId.ToString(), new AccountProviderNamespace?(providerNamespaceId), new Guid?());
        IEnumerable<AzureResourceAccount> source = this.DeleteCloudServiceFromDatabase(vssRequestContext, subscriptionId, providerNamespaceId, cloudServiceName);
        if (source.Any<AzureResourceAccount>())
        {
          AzureResourceAccount deletedAzureAccount = source.SingleOrDefault<AzureResourceAccount>((Func<AzureResourceAccount, bool>) (x => x.AzureCloudServiceName == cloudServiceName));
          Guid hostId = deletedAzureAccount.AccountId;
          CollectionHelper.WithCollectionContext(requestContext, hostId, (Action<IVssRequestContext>) (collectionContext =>
          {
            this.ResetCurrentQuantityUponUnlink(collectionContext, hostId);
            this.TraceBIEventForAzureSubscriptionChanges(collectionContext, collectionContext.ServiceHost.InstanceId, deletedAzureAccount, (AzureSubscriptionInternal) null, true);
          }), method: nameof (DeleteAzureCloudService));
          DualWritesHelper.DualWriteAzureResourceAccount(vssRequestContext, (IEnumerable<AzureResourceAccount>) new AzureResourceAccount[1]
          {
            deletedAzureAccount
          }, hostId, false);
        }
        return source;
      }
      catch (Exception ex)
      {
        vssRequestContext.TraceException(5105599, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        vssRequestContext.TraceLeave(5105600, "Commerce", nameof (PlatformSubscriptionService), nameof (DeleteAzureCloudService));
      }
    }

    public IEnumerable<IAzureSubscription> GetAzureSubscriptions(
      IVssRequestContext requestContext,
      List<Guid> subscriptionIds,
      AccountProviderNamespace providerNamespaceId)
    {
      if (subscriptionIds == null || !subscriptionIds.Any<Guid>())
        return (IEnumerable<IAzureSubscription>) new List<AzureSubscription>();
      List<IAzureSubscription> azureSubscriptions = new List<IAzureSubscription>();
      foreach (Guid subscriptionId in subscriptionIds)
      {
        IAzureSubscription azureSubscription = this.GetAzureSubscription(requestContext, subscriptionId)?.ToAzureSubscription();
        if (azureSubscription != null)
          azureSubscriptions.Add(azureSubscription);
      }
      return (IEnumerable<IAzureSubscription>) azureSubscriptions;
    }

    public virtual AzureSubscriptionInternal GetAzureSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      deploymentContext.TraceEnter(5105300, "Commerce", nameof (PlatformSubscriptionService), new object[1]
      {
        (object) subscriptionId
      }, nameof (GetAzureSubscription));
      try
      {
        this.CheckPermission(deploymentContext, 1);
        return deploymentContext.GetService<ICommerceCache>().GetOrSet<AzureSubscriptionInternal>(deploymentContext, PlatformSubscriptionService.GetAzureSubscriptionCacheKey(subscriptionId.ToString()), (Func<AzureSubscriptionInternal>) (() => this.GetAzureSubscriptionFromDatabase(deploymentContext, subscriptionId)));
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(5105309, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(5105310, "Commerce", nameof (PlatformSubscriptionService), nameof (GetAzureSubscription));
      }
    }

    public virtual void CreateAzureSubscription(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal subscription)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext1.TraceEnter(5105320, "Commerce", nameof (PlatformSubscriptionService), new object[1]
      {
        (object) subscription
      }, nameof (CreateAzureSubscription));
      requestContext1.TraceProperties<AzureSubscriptionInternal>(5108884, "Commerce", nameof (PlatformSubscriptionService), subscription, (string) null);
      try
      {
        this.CheckPermission(requestContext1, 2);
        this.AddAzureSubscriptionToDatabase(requestContext1, subscription);
      }
      catch (Exception ex)
      {
        requestContext1.TraceException(5105329, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext1.TraceLeave(5105330, "Commerce", nameof (PlatformSubscriptionService), nameof (CreateAzureSubscription));
      }
    }

    public virtual void UpdateAzureSubscription(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal subscription,
      AccountProviderNamespace? providerNamespaceId = null,
      bool keepSource = true)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext1.TraceEnter(5105331, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) subscription,
        (object) keepSource
      }, nameof (UpdateAzureSubscription));
      try
      {
        this.CheckPermission(requestContext1, 2);
        AzureSubscriptionInternal subscriptionToUpdate = this.GetAzureSubscriptionFromDatabase(requestContext1, subscription.AzureSubscriptionId);
        if (subscriptionToUpdate == null)
          return;
        if (keepSource)
          subscription.AzureSubscriptionSource = subscriptionToUpdate.AzureSubscriptionSource;
        this.AddAzureSubscriptionToDatabase(requestContext1, subscription);
        if (subscriptionToUpdate.AzureSubscriptionStatusId == subscription.AzureSubscriptionStatusId)
          return;
        bool isDeleteStatus = subscription.AzureSubscriptionStatusId == SubscriptionStatus.Deleted;
        bool isActive = subscription.AzureSubscriptionStatusId == SubscriptionStatus.Active;
        foreach (AzureResourceAccount azureResourceAccount in providerNamespaceId.HasValue ? this.GetAzureResourceAccountsBySubscriptionIdFromDatabase(requestContext1, subscriptionToUpdate.AzureSubscriptionId, providerNamespaceId.Value) : this.GetAllAzureResourceAccountsBySubscriptionIdFromDatabase(requestContext1, subscriptionToUpdate.AzureSubscriptionId))
        {
          AzureResourceAccount account = azureResourceAccount;
          IVssRequestContext requestContext2 = requestContext1;
          AccountRightsChangedEvent eventData = new AccountRightsChangedEvent(account.AccountId, DateTime.UtcNow);
          eventData.ChangeType = AccountRightsChangeType.Account;
          Guid? accountId = new Guid?();
          this.PublishAccountRightsNotification(requestContext2, 5105332, eventData, accountId);
          this.SendMessageForOfferSubscriptionChanges(requestContext1, subscription, account);
          CollectionHelper.WithCollectionContext(requestContext, account.AccountId, (Action<IVssRequestContext>) (collectionContext =>
          {
            if (isDeleteStatus)
              this.UnlinkCollection(collectionContext, subscription.AzureSubscriptionId, account.ProviderNamespaceId, collectionContext.ServiceHost.InstanceId, new Guid?(), false);
            if (!isActive)
            {
              List<IOfferSubscription> list = collectionContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(collectionContext).ToList<IOfferSubscription>();
              this.TraceBIEventOnAzureSubscriptionDeactivation(collectionContext, account, (IEnumerable<IOfferSubscription>) list, false);
            }
            if (subscription != null)
              subscriptionToUpdate.AzureOfferCode = subscription.AzureOfferCode;
            this.TraceBIEventForAzureSubscriptionChanges(collectionContext, collectionContext.ServiceHost.InstanceId, account, subscriptionToUpdate, isDeleteStatus);
          }), method: nameof (UpdateAzureSubscription));
        }
      }
      catch (Exception ex)
      {
        requestContext1.TraceException(5105339, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext1.TraceLeave(5105340, "Commerce", nameof (PlatformSubscriptionService), nameof (UpdateAzureSubscription));
      }
    }

    internal void SendMessageForOfferSubscriptionChanges(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal subscription,
      AzureResourceAccount azureResourceAccount)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      if (azureResourceAccount == null || azureResourceAccount.AccountId == Guid.Empty)
      {
        requestContext1.Trace(5105333, TraceLevel.Warning, "Commerce", nameof (PlatformSubscriptionService), string.Format("Unable to send change notification OfferSubscriptionQuantityChangeMessage for subscription {0}. New Status: {1} as the account is null", (object) subscription.AzureSubscriptionId, (object) subscription.AzureSubscriptionStatusId));
      }
      else
      {
        OfferSubscriptionQuantityChangeMessage offerSubscriptionQuantityChangeMessage = new OfferSubscriptionQuantityChangeMessage()
        {
          EventType = OfferSubscriptionQuantityChangeEventType.SubscriptionStatusChange,
          ActivityId = requestContext1.ActivityId,
          VsoAccountId = azureResourceAccount.AccountId,
          SubscriptionStatus = subscription.AzureSubscriptionStatusId,
          SubscriptionId = subscription.AzureSubscriptionId
        };
        using (CommerceVssRequestContextExtensions.VssRequestContextHolder collection = requestContext1.ToCollection(azureResourceAccount.AccountId))
        {
          IVssRequestContext context = collection.RequestContext.To(TeamFoundationHostType.Application);
          offerSubscriptionQuantityChangeMessage.TenantId = context.GetOrganizationAadTenantId().ToString();
        }
        OfferSubscriptionQuantityChangeMessage[] offerSubscriptionQuantityChangeMessages = (OfferSubscriptionQuantityChangeMessage[]) null;
        CollectionHelper.WithCollectionContext(requestContext1, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext => offerSubscriptionQuantityChangeMessages = collectionContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(collectionContext).Select<IOfferSubscription, OfferSubscriptionQuantityChangeMessage>((Func<IOfferSubscription, OfferSubscriptionQuantityChangeMessage>) (m => new OfferSubscriptionQuantityChangeMessage(offerSubscriptionQuantityChangeMessage)
        {
          OfferMeterName = m.OfferMeter.Name,
          OfferMeterCategory = m.OfferMeter.Category,
          Quantity = subscription.AzureSubscriptionStatusId == SubscriptionStatus.Active ? m.CommittedQuantity : m.IncludedQuantity,
          MeterGalleryId = m.OfferMeter.GalleryId
        })).ToArray<OfferSubscriptionQuantityChangeMessage>()), method: nameof (SendMessageForOfferSubscriptionChanges));
        this.BillingChangeMessageHelper.SendMessageForOfferSubscriptionChanges(requestContext1, offerSubscriptionQuantityChangeMessages);
        requestContext1.Trace(5105334, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Publishing Subscription status change notification OfferSubscriptionQuantityChangeMessage for Account {0}. New Status: {1}", (object) offerSubscriptionQuantityChangeMessage.VsoAccountId, (object) offerSubscriptionQuantityChangeMessage.SubscriptionStatus));
      }
    }

    internal virtual IEnumerable<IOfferSubscription> GetThirdPartyOfferSubscriptions(
      IVssRequestContext collectionContext)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      IEnumerable<IOfferSubscription> thirdPartyExtensions = collectionContext.GetService<IOfferSubscriptionService>().GetOfferSubscriptions(collectionContext).Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (x =>
      {
        if (x.CommittedQuantity <= x.IncludedQuantity)
          return false;
        OfferMeter offerMeter = x.OfferMeter;
        // ISSUE: explicit non-virtual call
        return (object) offerMeter != null && !__nonvirtual (offerMeter.IsFirstParty);
      }));
      if (thirdPartyExtensions.Any<IOfferSubscription>())
        collectionContext.TraceConditionally(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.HasAnyThirdPartyPurchasesTracing, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), (Func<string>) (() =>
        {
          List<OfferMeter> list = thirdPartyExtensions.Select<IOfferSubscription, OfferMeter>((Func<IOfferSubscription, OfferMeter>) (x => x.OfferMeter)).ToList<OfferMeter>();
          StringBuilder stringBuilder = new StringBuilder();
          foreach (OfferMeter offerMeter in list)
            stringBuilder.Append(" " + offerMeter.GalleryId + " : " + offerMeter.Name + " ");
          return "Thirdparty Extensions List: " + stringBuilder.ToString() + " found before swapping subscription for the account.";
        }));
      return thirdPartyExtensions;
    }

    internal virtual AzureResourceAccount UpdateAzureResourceAccountWithNewSubscriptionResourceGroup(
      IVssRequestContext collectionContext,
      AzureResourceAccount azureResourceAccount,
      Guid accountId,
      Guid ownerId,
      string accountName,
      Guid oldSubscriptionId)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      IVssRequestContext requestContext1 = collectionContext.To(TeamFoundationHostType.Deployment);
      ArgumentUtility.CheckForNull<IVssRequestContext>(collectionContext, nameof (collectionContext));
      collectionContext.TraceEnter(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.UpdateAzureResourceAccountWithNewSubscriptionResourceGroupEnter, "Commerce", nameof (PlatformSubscriptionService), new object[5]
      {
        (object) azureResourceAccount,
        (object) accountId,
        (object) ownerId,
        (object) accountName,
        (object) oldSubscriptionId
      }, nameof (UpdateAzureResourceAccountWithNewSubscriptionResourceGroup));
      try
      {
        ArgumentUtility.CheckForNull<AzureResourceAccount>(azureResourceAccount, nameof (azureResourceAccount));
        ArgumentUtility.CheckForNull<string>(accountName, nameof (accountName));
        ArgumentUtility.CheckForEmptyGuid(azureResourceAccount.AzureSubscriptionId, "AzureSubscriptionId");
        ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
        ArgumentUtility.CheckForEmptyGuid(ownerId, nameof (ownerId));
        IVssRequestContext requestContext2 = collectionContext.Elevate();
        IVssRequestContext requestContext3 = requestContext1.Elevate();
        try
        {
          if (azureResourceAccount == null)
            throw new CollectionNotFoundException(HostingResources.AccountDoesNotExist((object) accountId));
          this.UpdateAzureResourceAccount(requestContext3, azureResourceAccount, true);
          this.PublishAccountStatusChangeNotification(requestContext2, azureResourceAccount.AzureSubscriptionId, azureResourceAccount.AccountId, azureResourceAccount.ProviderNamespaceId, AccountStatusChangeEventType.LinkedToSubscription, 5105355);
          this.TraceBIEventForAzureSubscriptionChanges(collectionContext, collectionContext.ServiceHost.InstanceId, azureResourceAccount, (AzureSubscriptionInternal) null, false);
          try
          {
            string str = CommerceUtil.CheckForRequestSource(collectionContext);
            CustomerIntelligenceData eventData = new CustomerIntelligenceData();
            eventData.Add(CustomerIntelligenceProperty.AccountOwner, (object) ownerId);
            eventData.Add(CustomerIntelligenceProperty.ResourceName, azureResourceAccount.AzureResourceName);
            eventData.Add(CustomerIntelligenceProperty.AccountName, accountName);
            eventData.Add(CustomerIntelligenceProperty.SubscriptionId, (object) azureResourceAccount.AzureSubscriptionId);
            eventData.Add(CustomerIntelligenceProperty.OldSubscriptionId, (object) oldSubscriptionId);
            eventData.Add(CustomerIntelligenceProperty.AccountLinkSource, str);
            CustomerIntelligence.PublishEvent(requestContext1, "ChangeSubscriptionAccount", eventData);
          }
          catch (Exception ex)
          {
            requestContext2.Trace(5106096, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), "Exception while publishing CI data " + ex.ToString());
          }
          return azureResourceAccount;
        }
        catch (Exception ex)
        {
          requestContext2.TraceException(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.UpdateAzureResourceAccountWithNewSubscriptionResourceGroupException, "Commerce", nameof (PlatformSubscriptionService), ex);
          throw;
        }
      }
      finally
      {
        collectionContext.TraceLeave(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.UpdateAzureResourceAccountWithNewSubscriptionResourceGroupLeave, "Commerce", nameof (PlatformSubscriptionService), nameof (UpdateAzureResourceAccountWithNewSubscriptionResourceGroup));
      }
    }

    public void ChangeSubscriptionCollection(
      IVssRequestContext requestContext,
      Guid newSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid hostId,
      bool hydrate,
      bool performOnlyValidations = false)
    {
      requestContext.TraceEnter(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.ChangeSubscriptionCollectionEnter, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) hostId,
        (object) hydrate
      }, nameof (ChangeSubscriptionCollection));
      try
      {
        this.ChangeAzureResourceAccountCollection(requestContext, newSubscriptionId, string.Empty, providerNamespaceId, hostId, hydrate);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.ChangeSubscriptionCollectionException, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.ChangeSubscriptionCollectionLeave, "Commerce", nameof (PlatformSubscriptionService), nameof (ChangeSubscriptionCollection));
      }
    }

    internal void ChangeAzureResourceAccountCollection(
      IVssRequestContext requestContext,
      Guid newSubscriptionId,
      string newAzureCloudServiceName,
      AccountProviderNamespace providerNamespaceId,
      Guid hostId,
      bool hydrate,
      bool performOnlyValidations = false)
    {
      requestContext.TraceEnter(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.ChangeAzureResourceAccountCollectionEnter, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) hostId,
        (object) hydrate
      }, nameof (ChangeAzureResourceAccountCollection));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
        IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
        ArgumentUtility.CheckForNull<Guid>(CommerceIdentityHelper.GetRequestAuthenticatedIdentityId(requestContext), "authenticatedIdentityId");
        AzureResourceAccount accountLink = this.GetAzureResourceAccountByCollectionId(deploymentContext.Elevate(), hostId, true);
        CollectionHelper.WithCollectionContext(deploymentContext, hostId, (Action<IVssRequestContext>) (collectionContext =>
        {
          Microsoft.VisualStudio.Services.Organization.Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext.Elevate(), (IEnumerable<string>) null);
          this.ValidateAzureResourceAccountOnMoveCollection(requestContext, collection, accountLink, hostId, newSubscriptionId, newAzureCloudServiceName);
          string thirdPartyExtensionsCancelledList = string.Empty;
          if (this.CheckChangeSubscriptionPermission(deploymentContext, false, false))
          {
            deploymentContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollectionCSSToolSPDeploymentLevelPermissionCheckSucceeded, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Permission check for CSSTool SP at the deployment level succeeded");
            this.UpdateAzureResAccountForMovingCollectionAndCancelThirdPartyPurchaseIfAny(collectionContext, accountLink, newSubscriptionId, newAzureCloudServiceName, providerNamespaceId, collection, thirdPartyExtensionsCancelledList, hydrate, performOnlyValidations);
          }
          else
          {
            try
            {
              this.CheckChangeSubscriptionPermission(collectionContext);
              this.ValidateSubscriptionAdminPrivileges(deploymentContext, newSubscriptionId);
              deploymentContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceCollectionAccountAdminPCAUserCollectionLevelPermissionCheckSucceeded, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Permission check for user who is an admin of the new subscription at the collection level succeeded");
              this.UpdateAzureResAccountForMovingCollectionAndCancelThirdPartyPurchaseIfAny(collectionContext, accountLink, newSubscriptionId, newAzureCloudServiceName, providerNamespaceId, collection, thirdPartyExtensionsCancelledList, hydrate, performOnlyValidations);
            }
            catch (AccessCheckException ex)
            {
              IVssRequestContext context = collectionContext.Elevate().To(TeamFoundationHostType.Application);
              Guid? tenantId = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null)?.TenantId;
              Guid empty = Guid.Empty;
              if ((tenantId.HasValue ? (tenantId.HasValue ? (tenantId.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
              {
                throw;
              }
              else
              {
                Microsoft.VisualStudio.Services.Identity.Identity userIdentity = deploymentContext.GetUserIdentity();
                CollectionHelper.WithCollectionContext(deploymentContext, hostId, (Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) ((collectionMsaContext, identity) =>
                {
                  this.CheckChangeSubscriptionPermission(collectionMsaContext);
                  this.ValidateSubscriptionAdminPrivileges(deploymentContext, newSubscriptionId);
                  deploymentContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResouceAccountCollectionWhenAccessCheckExceptionEncounteredCollectionLevelPermissionCheckSucceeded, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Permission check for MSA FP who is an admin of the new subscription at the collection level succeeded");
                  this.UpdateAzureResAccountForMovingCollectionAndCancelThirdPartyPurchaseIfAny(collectionContext, accountLink, newSubscriptionId, newAzureCloudServiceName, providerNamespaceId, collection, thirdPartyExtensionsCancelledList, hydrate, performOnlyValidations);
                }), false, userIdentity, nameof (ChangeAzureResourceAccountCollection));
              }
            }
          }
          if (!performOnlyValidations)
          {
            string str = CommerceUtil.CheckForRequestSource(collectionContext);
            CustomerIntelligenceData eventData = new CustomerIntelligenceData();
            eventData.Add(CustomerIntelligenceProperty.AccountOwner, (object) collection.Owner);
            eventData.Add(CustomerIntelligenceProperty.ResourceName, accountLink.AzureResourceName);
            eventData.Add(CustomerIntelligenceProperty.ResourceGroup, accountLink.AzureCloudServiceName);
            eventData.Add(CustomerIntelligenceProperty.AccountName, collection.Name);
            eventData.Add(CustomerIntelligenceProperty.OldSubscriptionId, (object) accountLink.AzureSubscriptionId);
            eventData.Add(CustomerIntelligenceProperty.SubscriptionId, (object) newSubscriptionId);
            eventData.Add(CustomerIntelligenceProperty.ChangeSubscriptionSource, str);
            eventData.Add(CustomerIntelligenceProperty.ExtensionsCancelled, thirdPartyExtensionsCancelledList);
            CustomerIntelligence.PublishEvent(deploymentContext, "ChangeSubscriptionAccount", eventData);
          }
          CommerceAuditUtilities.LogSubscriptionUpdate(collectionContext, accountLink.AzureSubscriptionId, newSubscriptionId);
        }), method: nameof (ChangeAzureResourceAccountCollection));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.ChangeAzureResourceAccountCollectionException, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.ChangeAzureResourceAccountCollectionLeave, "Commerce", nameof (PlatformSubscriptionService), nameof (ChangeAzureResourceAccountCollection));
      }
    }

    internal virtual void UpdateAzureResAccountForMovingCollectionAndCancelThirdPartyPurchaseIfAny(
      IVssRequestContext collectionContext,
      AzureResourceAccount accountLink,
      Guid newSubscriptionId,
      string newAzureCloudServiceName,
      AccountProviderNamespace providerNamespaceId,
      Microsoft.VisualStudio.Services.Organization.Collection collection,
      string thirdPartyExtensionsCancelledList,
      bool hydrate,
      bool performOnlyValidations)
    {
      if (performOnlyValidations)
        return;
      if (collectionContext.To(TeamFoundationHostType.Deployment).IsFeatureEnabled("VisualStudio.Services.Commerce.CancelThirdPartyPurchasesForSubscriptionSwap"))
        this.CancelThirdPartyForSwapSubscription(collectionContext, accountLink, out thirdPartyExtensionsCancelledList);
      this.UpdateAzureResAccountForMoveCollection(collectionContext, accountLink, newSubscriptionId, newAzureCloudServiceName, providerNamespaceId, collection.Id, collection.Owner, collection.Name, hydrate);
    }

    private void ValidateAzureResourceAccountOnMoveCollection(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Organization.Collection collection,
      AzureResourceAccount accountLink,
      Guid hostId,
      Guid newSubscriptionId,
      string newAzureCloudServiceName)
    {
      if (collection == null)
        throw new AccountNotFoundException(HostingResources.AccountDoesNotExist((object) hostId));
      if (accountLink == null)
      {
        AccountNotLinkedException notLinkedException = new AccountNotLinkedException(string.Format("Collection {0} is not linked", (object) hostId));
        requestContext.TraceException(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.AccountLinkingException, "Commerce", nameof (PlatformSubscriptionService), (Exception) notLinkedException);
        throw notLinkedException;
      }
      if (accountLink.AzureSubscriptionId == Guid.Empty)
      {
        AccountNotLinkedException notLinkedException = new AccountNotLinkedException(string.Format("Collection {0} is not linked", (object) hostId));
        requestContext.TraceException(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.AccountLinkingException, "Commerce", nameof (PlatformSubscriptionService), (Exception) notLinkedException);
        throw notLinkedException;
      }
      if (accountLink.AzureSubscriptionId == newSubscriptionId && (accountLink.AzureCloudServiceName == newAzureCloudServiceName || newAzureCloudServiceName == string.Empty))
      {
        AccountAlreadyLinkedException alreadyLinkedException = new AccountAlreadyLinkedException(collection.Name);
        requestContext.TraceException(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.AccountLinkingException, "Commerce", nameof (PlatformSubscriptionService), (Exception) alreadyLinkedException);
        throw alreadyLinkedException;
      }
    }

    private void ValidateSubscriptionAdminPrivileges(
      IVssRequestContext deploymentContext,
      Guid newSubscriptionId)
    {
      if (deploymentContext.GetService<IAzureResourceHelper>().GetSubscriptionAuthorization(deploymentContext, newSubscriptionId) == SubscriptionAuthorizationSource.Unauthorized)
        throw new UserIsNotSubscriptionAdminException();
    }

    private void UpdateAzureResAccountForMoveCollection(
      IVssRequestContext collectionContext,
      AzureResourceAccount accountLink,
      Guid newSubscriptionId,
      string newAzureCloudServiceName,
      AccountProviderNamespace providerNamespaceId,
      Guid collectionId,
      Guid collectionOwnerId,
      string collectionName,
      bool hydrate)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      AzureResourceAccount azureResourceAccount1 = (AzureResourceAccount) accountLink.Clone();
      azureResourceAccount1.AzureSubscriptionId = newSubscriptionId;
      if (newAzureCloudServiceName != "" || newAzureCloudServiceName != string.Empty)
        azureResourceAccount1.AzureCloudServiceName = newAzureCloudServiceName;
      azureResourceAccount1.ProviderNamespaceId = providerNamespaceId;
      collectionContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.SubscriptionSwapTracing, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Updating subscriptions started for the account {0} from an old subscription id {2} to a new subscription id {1} from an old resourcegroupname {3} to a new resourcegroupname {2}", (object) azureResourceAccount1.AccountId, (object) azureResourceAccount1.AzureSubscriptionId, (object) accountLink.AzureSubscriptionId, (object) azureResourceAccount1.AzureCloudServiceName, (object) accountLink.AzureCloudServiceName);
      collectionContext.TraceAlways(CommerceTracePoints.PlatformSubscriptionService.ChangeAzureResourceAccountCollection.SubscriptionSwapTracing, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Dual writing the Azure Resource Account metadata for the account {0} for an old subscription id {1}", (object) accountLink.AccountId, (object) accountLink.AzureSubscriptionId);
      DualWritesHelper.DualWriteAzureResourceAccount(vssRequestContext, (IEnumerable<AzureResourceAccount>) new AzureResourceAccount[1]
      {
        accountLink
      }, accountLink.CollectionId, false);
      AzureResourceAccount azureResourceAccount2 = this.UpdateAzureResourceAccountWithNewSubscriptionResourceGroup(collectionContext, azureResourceAccount1, collectionId, collectionOwnerId, collectionName, accountLink.AzureSubscriptionId);
      bool flag1 = AssignmentBillingHelper.IsAssignmentBillingEnabledForSubscription(vssRequestContext, accountLink.AzureSubscriptionId);
      bool flag2 = AssignmentBillingHelper.IsAssignmentBillingEnabledForSubscription(vssRequestContext, azureResourceAccount2.AzureSubscriptionId);
      if (!flag1 & flag2)
        this.SetIncludedQuantityForAssignmentBilling(collectionContext, vssRequestContext, azureResourceAccount2);
      else if (flag1 && !flag2)
        this.ResetIncludedQuantityForAssignmentBilling(collectionContext, vssRequestContext, azureResourceAccount2);
      if (!hydrate || vssRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      HydrationHelper hydrationHelper = this.GetHydrationHelper();
      collectionContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.DehydrationTracing, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Dehydration started for the account {0} with old subscription id {1}", (object) accountLink.AccountId, (object) accountLink.AzureSubscriptionId));
      hydrationHelper.StartDeHydration(vssRequestContext, accountLink);
      collectionContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.HydrationTracing, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Hydration started for the account {0} with new subscription id {1}", (object) accountLink.AccountId, (object) newSubscriptionId));
      hydrationHelper.StartHydration(vssRequestContext, azureResourceAccount2);
    }

    private void CancelThirdPartyForSwapSubscription(
      IVssRequestContext collectionContext,
      AzureResourceAccount accountLink,
      out string thirdPartyExtensionsCancelledList)
    {
      IVssRequestContext context = collectionContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext vssRequestContext = context.Elevate();
      IEnumerable<IOfferSubscription> offerSubscriptions = this.GetThirdPartyOfferSubscriptions(collectionContext);
      StringBuilder stringBuilder = new StringBuilder();
      if (!offerSubscriptions.IsNullOrEmpty<IOfferSubscription>())
      {
        collectionContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.CancellingThirdPartyExtensions, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Cancelling third party extensions for the account {0} purchased under an old subscription id {2}", (object) accountLink.AccountId, (object) accountLink.AzureSubscriptionId);
        PlatformOfferSubscriptionService service = context.GetService<PlatformOfferSubscriptionService>();
        Guid? subscriptionTenantId = CommerceUtil.GetSubscriptionTenantId(vssRequestContext, accountLink.AzureSubscriptionId);
        foreach (IOfferSubscription offerSubscription in offerSubscriptions)
        {
          service.CancelOfferSubscription(vssRequestContext, offerSubscription.OfferMeter.GalleryId, accountLink.AzureSubscriptionId, offerSubscription.RenewalGroup, HostingResources.CancellingThirdPartyExtensionDueToChangeSubscription(), new Guid?(accountLink.CollectionId), false, subscriptionTenantId);
          stringBuilder.Append("Meter:" + offerSubscription.OfferMeter.GalleryId + ";");
        }
        collectionContext.Trace(CommerceTracePoints.PlatformSubscriptionService.ChangeSubscriptionCollection.CancellingThirdPartyExtensions, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Canceled {0} third party extensions for the account {1} purchased under an old subscription id {2}", (object) offerSubscriptions.Count<IOfferSubscription>(), (object) accountLink.AccountId, (object) accountLink.AzureSubscriptionId);
      }
      thirdPartyExtensionsCancelledList = stringBuilder.ToString();
    }

    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
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

    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
    public void CreateSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      SubscriptionStatus status,
      SubscriptionSource source)
    {
      throw new NotImplementedException();
    }

    public void LinkCollection(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid hostId,
      Guid? ownerId,
      bool hydrate = false)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      requestContext.TraceEnter(5108885, "Commerce", nameof (PlatformSubscriptionService), new object[5]
      {
        (object) subscriptionId,
        (object) providerNamespaceId,
        (object) hostId,
        (object) ownerId,
        (object) hydrate
      }, nameof (LinkCollection));
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        AzureResourceAccount azureResAcc = new AzureResourceAccount()
        {
          AzureSubscriptionId = subscriptionId,
          ProviderNamespaceId = providerNamespaceId,
          AzureCloudServiceName = string.Format("{0}-{1}", (object) providerNamespaceId, (object) hostId.ToString("N").ToUpper())
        };
        requestContext.TraceProperties<AzureResourceAccount>(5108892, "Commerce", nameof (PlatformSubscriptionService), azureResAcc, (string) null);
        Microsoft.VisualStudio.Services.Identity.Identity identity1;
        if (requestContext.IsCommerceService())
        {
          if (InfrastructureHostHelper.IsInfrastructureHost(requestContext, hostId))
          {
            requestContext1.Trace(5108889, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Link infrastructure host ${0} to subscription ${1}, providerNamespace ${2}", (object) hostId, (object) subscriptionId, (object) providerNamespaceId));
            identity1 = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
            {
              CommerceConstants.SpsMasterId
            }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          }
          else
          {
            identity1 = requestContext1.GetUserIdentity();
            requestContext.TraceAlways(5109365, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Link Collection OwnerIdentity Values are - " + identity1.Serialize<Microsoft.VisualStudio.Services.Identity.Identity>(true));
          }
        }
        else
        {
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          identity1 = vssRequestContext.GetService<IVssIdentityRetrievalService>().ResolveEligibleActorByMasterId(vssRequestContext, ownerId.Value);
        }
        if (identity1 == null)
        {
          requestContext.TraceAlways(5109366, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), new
          {
            Msg = "Parameters - Link Collection",
            hostId = hostId,
            subscriptionId = subscriptionId,
            providerNamespaceId = providerNamespaceId,
            ownerId = ownerId
          }.Serialize());
        }
        else
        {
          requestContext1.Trace(5106156, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Account owner for this call is {0}", (object) identity1.MasterId));
          AzureResourceAccount linkedAccountResult = (AzureResourceAccount) null;
          try
          {
            this.UpdateAzureSubscriptionAsynchronously(requestContext, azureResAcc);
            CollectionHelper.WithCollectionContext(requestContext, hostId, (Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) ((collectionContext, identity) =>
            {
              azureResAcc.AzureResourceName = collectionContext.ServiceHost.Name;
              azureResAcc.AzureGeoRegion = collectionContext.GetExtension<ICommerceRegionHandler>().GetRegionFromCollectionContext(collectionContext.Elevate());
              linkedAccountResult = this.LinkCollectionInternal(collectionContext, azureResAcc, hostId, identity);
            }), true, identity1, nameof (LinkCollection));
          }
          catch (HostDoesNotExistException ex)
          {
            throw new AccountNotFoundException(hostId);
          }
          if (!hydrate || requestContext1.ExecutionEnvironment.IsOnPremisesDeployment)
            return;
          this.GetHydrationHelper().StartHydration(requestContext1, linkedAccountResult);
        }
      }
      finally
      {
        requestContext.TraceLeave(5108886, "Commerce", nameof (PlatformSubscriptionService), nameof (LinkCollection));
      }
    }

    public void UnlinkCollection(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid collectionId,
      Guid? ownerId,
      bool hydrate = false)
    {
      requestContext.TraceEnter(5108890, "Commerce", nameof (PlatformSubscriptionService), new object[5]
      {
        (object) subscriptionId,
        (object) providerNamespaceId,
        (object) collectionId,
        (object) ownerId,
        (object) hydrate
      }, nameof (UnlinkCollection));
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(collectionId, nameof (collectionId));
        ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
        CollectionHelper.WithCollectionContext(requestContext, collectionId, (Action<IVssRequestContext>) (collectionContext =>
        {
          IVssRequestContext requestContext1 = collectionContext.To(TeamFoundationHostType.Deployment);
          Microsoft.VisualStudio.Services.Identity.Identity identity = CommerceIdentityHelper.GetIdentity(requestContext1);
          IVssRequestContext requestContext2 = requestContext1.Elevate();
          this.CheckCollectionAndOwnership(collectionContext, identity);
          AzureResourceAccount accountByCollectionId = this.GetAzureResourceAccountByCollectionId(requestContext2, collectionId, true);
          if (accountByCollectionId == null)
            throw new AccountNotFoundException(HostingResources.AccountDoesNotExist((object) collectionId));
          requestContext.TraceProperties<AzureResourceAccount>(5108892, "Commerce", nameof (PlatformSubscriptionService), accountByCollectionId, (string) null);
          if (!accountByCollectionId.AzureSubscriptionId.Equals(subscriptionId) || !accountByCollectionId.ProviderNamespaceId.Equals((object) providerNamespaceId))
            throw new AzureSubscriptionNotLinkedToAccountException(subscriptionId, collectionId, providerNamespaceId.ToString());
          this.DeleteAzureResourceAccount(requestContext2, accountByCollectionId);
          if (hydrate)
            this.GetHydrationHelper().StartDeHydration(requestContext2, accountByCollectionId);
          CommerceAuditUtilities.LogSubscriptionUnlink(collectionContext, subscriptionId);
        }), method: nameof (UnlinkCollection));
      }
      finally
      {
        requestContext.TraceLeave(5108891, "Commerce", nameof (PlatformSubscriptionService), nameof (UnlinkCollection));
      }
    }

    public virtual IEnumerable<ISubscriptionAccount> GetAccounts(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId)
    {
      requestContext.TraceEnter(5108878, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) subscriptionId,
        (object) providerNamespaceId
      }, nameof (GetAccounts));
      try
      {
        return (IEnumerable<ISubscriptionAccount>) this.QueryForAzureResourceAccountsBySubscriptionId(requestContext.To(TeamFoundationHostType.Deployment), subscriptionId, providerNamespaceId).Where<AzureResourceAccount>((Func<AzureResourceAccount, bool>) (a => a.OperationResult == OperationResult.Succeeded)).Select<AzureResourceAccount, SubscriptionAccount>((Func<AzureResourceAccount, SubscriptionAccount>) (acc =>
        {
          SubscriptionAccount subscriptionAccount = new SubscriptionAccount()
          {
            AccountId = acc.AccountId,
            GeoLocation = acc.AzureGeoRegion,
            SubscriptionId = new Guid?(subscriptionId),
            ResourceGroupName = acc.AzureCloudServiceName,
            ResourceName = acc.AzureResourceName,
            IsAccountOwner = false,
            SubscriptionStatus = SubscriptionStatus.Unknown
          };
          try
          {
            CollectionHelper.WithCollectionContext(requestContext, acc.AccountId, (Action<IVssRequestContext>) (collectionContext => subscriptionAccount.AccountName = CollectionHelper.GetCollectionName(collectionContext)), method: nameof (GetAccounts));
          }
          catch (HostDoesNotExistException ex)
          {
            requestContext.TraceException(5108878, "Commerce", nameof (PlatformSubscriptionService), (Exception) ex);
            subscriptionAccount = (SubscriptionAccount) null;
          }
          return subscriptionAccount;
        })).Where<SubscriptionAccount>((Func<SubscriptionAccount, bool>) (x => x != null));
      }
      finally
      {
        requestContext.TraceLeave(5108883, "Commerce", nameof (PlatformSubscriptionService), nameof (GetAccounts));
      }
    }

    public IEnumerable<ISubscriptionAccount> GetAccounts(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid? memberId,
      bool queryOnlyOwnerAccounts,
      bool inlcudeDisabledAccounts = false,
      bool includeMSAAccounts = false,
      IEnumerable<Guid> serviceOwners = null,
      string galleryId = null,
      bool addUnlinkedSubscription = false,
      bool queryAccountsByUpn = false)
    {
      return this.GetAccounts(requestContext, providerNamespace, memberId, queryOnlyOwnerAccounts, inlcudeDisabledAccounts, includeMSAAccounts, serviceOwners, galleryId, addUnlinkedSubscription, queryAccountsByUpn, true);
    }

    public IEnumerable<ISubscriptionAccount> GetAccounts(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespaceId,
      Guid? memberId,
      bool queryOnlyOwnerAccounts,
      bool includeDisabledAccounts = false,
      bool includeMSAAccounts = false,
      IEnumerable<Guid> serviceOwners = null,
      string galleryId = null,
      bool addUnlinkedSubscriptions = false,
      bool queryAccountsByUpn = false,
      bool addCommerceDetails = true)
    {
      requestContext.TraceEnter(5108878, "Commerce", nameof (PlatformSubscriptionService), new object[9]
      {
        (object) providerNamespaceId,
        (object) memberId,
        (object) queryOnlyOwnerAccounts,
        (object) false,
        (object) includeMSAAccounts,
        (object) serviceOwners,
        (object) galleryId,
        (object) addUnlinkedSubscriptions,
        (object) queryAccountsByUpn
      }, nameof (GetAccounts));
      try
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRequestContext vssRequestContext = requestContext1;
        if (!this.CheckPermission(requestContext1, 1, false))
        {
          memberId = requestContext.IsCommerceService() ? new Guid?(requestContext.GetUserIdentity().Id) : CommerceIdentityHelper.GetRequestAuthenticatedIdentityId(requestContext1);
          vssRequestContext = requestContext1.Elevate();
          vssRequestContext.Trace(5106254, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Permission check for read subscription data failed, ony returning subscription accounts which the caller owns or is a member of.");
        }
        int num = requestContext.UserContext.IsCspPartnerIdentityType() ? 1 : 0;
        if (num != 0)
          memberId = new Guid?(requestContext.IsCommerceService() ? requestContext.GetUserIdentity().Id : requestContext.GetUserIdentity().MasterId);
        ArgumentUtility.CheckForNull<Guid>(memberId, nameof (memberId));
        List<ISubscriptionAccount> list = this.GetAccountsInternal(vssRequestContext, providerNamespaceId, memberId.Value, queryOnlyOwnerAccounts, includeDisabledAccounts, serviceOwners, addCommerceDetails).ToList<ISubscriptionAccount>();
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        string upnFromIdentity = this.GetUpnFromIdentity(userIdentity);
        if (num == 0 && includeMSAAccounts | queryAccountsByUpn)
        {
          if (requestContext1.IsCommerceService() && requestContext1.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableNewIdentitySearchUsage"))
          {
            list.AddRange(this.GetAccountsForUser(requestContext1, vssRequestContext, providerNamespaceId, upnFromIdentity, serviceOwners, userIdentity, queryAccountsByUpn, queryOnlyOwnerAccounts, includeDisabledAccounts, includeMSAAccounts, addCommerceDetails));
          }
          else
          {
            HashSet<Guid> guidSet = new HashSet<Guid>();
            if (includeMSAAccounts)
            {
              Microsoft.VisualStudio.Services.Identity.Identity primaryMsaIdentity = requestContext.GetService<IdentityService>().GetPrimaryMsaIdentity(requestContext, (IReadOnlyVssIdentity) userIdentity);
              if (primaryMsaIdentity != null)
                guidSet.Add(requestContext.IsCommerceService() ? primaryMsaIdentity.Id : primaryMsaIdentity.MasterId);
            }
            if (queryAccountsByUpn && requestContext1.IsFeatureEnabled("VisualStudio.Services.Commerce.QueryAccountsByUpn"))
              guidSet.UnionWith(this.GetIdentityIdsWithIdenticalUpn(requestContext, userIdentity));
            foreach (Guid memberId1 in guidSet)
              list.AddRange(this.GetAccountsInternal(vssRequestContext, providerNamespaceId, memberId1, queryOnlyOwnerAccounts, serviceOwners: serviceOwners, addCommerceDetails: addCommerceDetails));
          }
        }
        Dictionary<Guid, ISubscriptionAccount> dictionary = new Dictionary<Guid, ISubscriptionAccount>();
        foreach (ISubscriptionAccount subscriptionAccount1 in list)
        {
          ISubscriptionAccount subscriptionAccount2;
          if (dictionary.TryGetValue(subscriptionAccount1.AccountId, out subscriptionAccount2))
            subscriptionAccount2.IsAccountOwner |= subscriptionAccount1.IsAccountOwner;
          else
            dictionary.Add(subscriptionAccount1.AccountId, subscriptionAccount1);
        }
        return (IEnumerable<ISubscriptionAccount>) dictionary.Values;
      }
      finally
      {
        requestContext.TraceLeave(5108883, "Commerce", nameof (PlatformSubscriptionService), nameof (GetAccounts));
      }
    }

    internal virtual string GetUpnFromIdentity(Microsoft.VisualStudio.Services.Identity.Identity authorizedIdentity) => authorizedIdentity.GetProperty<string>("Account", string.Empty);

    private IEnumerable<ISubscriptionAccount> GetAccountsForUser(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedDeploymentContext,
      AccountProviderNamespace providerNamespaceId,
      string accountName,
      IEnumerable<Guid> serviceOwners,
      Microsoft.VisualStudio.Services.Identity.Identity authorizedIdentity,
      bool queryAccountsByUpn,
      bool queryOnlyOwnerAccounts,
      bool includeDisabledAccounts,
      bool includeMSAAccounts,
      bool addCommerceDetails = true)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceEnter(5108893, "Commerce", nameof (PlatformSubscriptionService), nameof (PlatformSubscriptionService));
      List<SubscriptionAccount> accountsForUser = new List<SubscriptionAccount>();
      if (!includeMSAAccounts && !queryAccountsByUpn)
      {
        requestContext.Trace(5109240, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Returning an empty list due to includeMSAAccounts and queryAccountsByUpn are false");
        return Enumerable.Empty<ISubscriptionAccount>();
      }
      try
      {
        List<SubjectDescriptor> subjectDescriptorList = new List<SubjectDescriptor>();
        if (includeMSAAccounts)
        {
          requestContext.Trace(5109240, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Retrieving primary msa identity for the user: {0}", (object) requestContext.GetUserCuid()));
          Microsoft.VisualStudio.Services.Identity.Identity primaryMsaIdentity = requestContext.GetService<IdentityService>().GetPrimaryMsaIdentity(requestContext, (IReadOnlyVssIdentity) authorizedIdentity);
          if (primaryMsaIdentity != null)
          {
            requestContext.Trace(5109240, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Retrieved primary msa identity {0} for the user: {1}", (object) IdentityCuidHelper.ComputeCuid(requestContext, (IReadOnlyVssIdentity) primaryMsaIdentity), (object) requestContext.GetUserCuid()));
            subjectDescriptorList.Add(primaryMsaIdentity.SubjectDescriptor);
          }
        }
        if (queryAccountsByUpn && requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.QueryAccountsByUpn"))
        {
          IEnumerable<Guid> userTenantsFromAad = this.GetUserTenantsFromAAD(requestContext, elevatedDeploymentContext);
          if (!userTenantsFromAad.IsNullOrEmpty<Guid>())
          {
            requestContext.Trace(5109240, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Retrieved {0} tenants for the user: {1}", (object) userTenantsFromAad.Count<Guid>(), (object) requestContext.GetUserCuid()));
            subjectDescriptorList.AddRange(this.BuildSubjectDescriptorsAndIdUsingTenants(requestContext, userTenantsFromAad, accountName));
          }
        }
        HashSet<Guid> guidSet1 = new HashSet<Guid>();
        HashSet<Guid> guidSet2 = new HashSet<Guid>();
        foreach (SubjectDescriptor descriptor in subjectDescriptorList)
        {
          guidSet1.Clear();
          guidSet2.Clear();
          FrameworkUserAccountMappingHttpClient client = elevatedDeploymentContext.GetClient<FrameworkUserAccountMappingHttpClient>();
          guidSet1.AddRange<Guid, HashSet<Guid>>((IEnumerable<Guid>) new HashSet<Guid>((IEnumerable<Guid>) client.QueryAccountIdsAsync(descriptor, queryOnlyOwnerAccounts ? UserRole.Owner : UserRole.Member, new bool?(false), new bool?(false)).SyncResult<List<Guid>>()));
          HashSet<Guid> values = queryOnlyOwnerAccounts ? guidSet1 : new HashSet<Guid>((IEnumerable<Guid>) client.QueryAccountIdsAsync(descriptor, UserRole.Owner, new bool?(false), new bool?(false)).SyncResult<List<Guid>>());
          guidSet2.AddRange<Guid, HashSet<Guid>>((IEnumerable<Guid>) values);
          requestContext.Trace(5109240, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Retrieved {0} member accounts and {1} owner accounts", (object) guidSet1.Count<Guid>(), (object) guidSet2.Count<Guid>()));
          if (guidSet1.Any<Guid>())
            accountsForUser.AddRange(this.GetSubscriptionAccountsFromAccountIds(requestContext, elevatedDeploymentContext, providerNamespaceId, (IEnumerable<Guid>) guidSet1, (IEnumerable<Guid>) guidSet2, descriptor, serviceOwners, addCommerceDetails: addCommerceDetails).Where<SubscriptionAccount>((Func<SubscriptionAccount, bool>) (x => x != null)));
        }
        return (IEnumerable<ISubscriptionAccount>) accountsForUser;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109240, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5109240, "Commerce", nameof (PlatformSubscriptionService), nameof (PlatformSubscriptionService));
      }
    }

    private IEnumerable<SubscriptionAccount> GetSubscriptionAccountsFromAccountIds(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedDeploymentContext,
      AccountProviderNamespace providerNamespaceId,
      IEnumerable<Guid> accountIds,
      IEnumerable<Guid> ownerAccountIds,
      SubjectDescriptor descriptor,
      IEnumerable<Guid> serviceOwners,
      bool queryOnlyOwnerAccounts = false,
      bool addCommerceDetails = true)
    {
      List<SubscriptionAccount> accountsFromAccountIds = new List<SubscriptionAccount>();
      if (addCommerceDetails)
      {
        foreach (AzureSubscriptionAccount azureSubAccount in this.GetSubscriptionsForAccountsFromDatabase(requestContext, accountIds, providerNamespaceId))
        {
          try
          {
            SubscriptionAccount subscriptionAccountDetails = this.GetSubscriptionAccountDetails(elevatedDeploymentContext, azureSubAccount, ownerAccountIds, descriptor, serviceOwners);
            if (subscriptionAccountDetails != null)
            {
              requestContext.Trace(5109240, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Retrieved azure subscription account for the collection {0} under a subscription {1}", (object) azureSubAccount.AccountId, (object) azureSubAccount.SubscriptionId));
              accountsFromAccountIds.Add(subscriptionAccountDetails);
            }
          }
          catch (HostDoesNotExistException ex)
          {
            requestContext.Trace(5106149, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), string.Format("Skipped getting subscription account for host {0} because it could not be found ({1}). descriptor={2}, queryOnlyOwnerAccounts={3}", (object) azureSubAccount.AccountId, (object) ex.Message, (object) descriptor, (object) queryOnlyOwnerAccounts));
          }
          catch (HostShutdownException ex)
          {
            requestContext.Trace(5106149, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), string.Format("Skipped getting subscription account for host {0} because it is stopped ({1}). descriptor={2}, queryOnlyOwnerAccounts={3}", (object) azureSubAccount.AccountId, (object) ex.Message, (object) descriptor, (object) queryOnlyOwnerAccounts));
          }
        }
      }
      else
        accountsFromAccountIds.AddRange(accountIds.Select<Guid, SubscriptionAccount>((Func<Guid, SubscriptionAccount>) (id => new SubscriptionAccount()
        {
          AccountId = id,
          AccountName = CommerceDeploymentHelper.GetHostName(elevatedDeploymentContext, id),
          GeoLocation = string.Empty,
          SubscriptionId = new Guid?(Guid.Empty),
          ResourceGroupName = string.Empty,
          ResourceName = string.Empty,
          IsAccountOwner = false,
          SubscriptionStatus = SubscriptionStatus.Unknown,
          AccountTenantId = Guid.Empty
        })));
      return (IEnumerable<SubscriptionAccount>) accountsFromAccountIds;
    }

    private IEnumerable<SubscriptionAccount> GetSubscriptionAccountsFromAccountIds(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedDeploymentContext,
      AccountProviderNamespace providerNamespaceId,
      IEnumerable<Guid> accountIds,
      IEnumerable<Guid> ownerAccountIds,
      Guid memberId,
      IEnumerable<Guid> serviceOwners,
      bool queryOnlyOwnerAccounts = false,
      bool addCommerceDetails = true)
    {
      List<SubscriptionAccount> accountsFromAccountIds = new List<SubscriptionAccount>();
      if (addCommerceDetails)
      {
        foreach (AzureSubscriptionAccount azureSubAccount in this.GetSubscriptionsForAccountsFromDatabase(elevatedDeploymentContext, accountIds, providerNamespaceId))
        {
          try
          {
            SubscriptionAccount subscriptionAccountDetails = this.GetSubscriptionAccountDetails(elevatedDeploymentContext, azureSubAccount, ownerAccountIds, memberId, serviceOwners);
            if (subscriptionAccountDetails != null)
              accountsFromAccountIds.Add(subscriptionAccountDetails);
          }
          catch (HostDoesNotExistException ex)
          {
            requestContext.Trace(5106149, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), string.Format("Skipped getting subscription account for host {0} because it could not be found ({1}). memberId={2}, queryOnlyOwnerAccounts={3}", (object) azureSubAccount.AccountId, (object) ex.Message, (object) memberId, (object) queryOnlyOwnerAccounts));
          }
          catch (HostShutdownException ex)
          {
            requestContext.Trace(5106149, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), string.Format("Skipped getting subscription account for host {0} because it is stopped ({1}). memberId={2}, queryOnlyOwnerAccounts={3}", (object) azureSubAccount.AccountId, (object) ex.Message, (object) memberId, (object) queryOnlyOwnerAccounts));
          }
        }
      }
      else
        accountsFromAccountIds.AddRange(accountIds.Select<Guid, SubscriptionAccount>((Func<Guid, SubscriptionAccount>) (id => new SubscriptionAccount()
        {
          AccountId = id,
          AccountName = CommerceDeploymentHelper.GetHostName(elevatedDeploymentContext, id),
          GeoLocation = string.Empty,
          SubscriptionId = new Guid?(Guid.Empty),
          ResourceGroupName = string.Empty,
          ResourceName = string.Empty,
          IsAccountOwner = false,
          SubscriptionStatus = SubscriptionStatus.Unknown,
          AccountTenantId = Guid.Empty
        })));
      return (IEnumerable<SubscriptionAccount>) accountsFromAccountIds;
    }

    private IEnumerable<SubjectDescriptor> BuildSubjectDescriptorsAndIdUsingTenants(
      IVssRequestContext deploymentContext,
      IEnumerable<Guid> tenantIds,
      string accountName)
    {
      return tenantIds.IsNullOrEmpty<Guid>() ? Enumerable.Empty<SubjectDescriptor>() : (IEnumerable<SubjectDescriptor>) deploymentContext.GetService<IdentityService>().ReadIdentities(deploymentContext, (IList<IdentityDescriptor>) tenantIds.Select<Guid, IdentityDescriptor>((Func<Guid, IdentityDescriptor>) (tid => new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", string.Format("{0}\\{1}", (object) tid, (object) accountName)))).ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, SubjectDescriptor>) (i => i.SubjectDescriptor)).ToList<SubjectDescriptor>();
    }

    public IEnumerable<Guid> GetUserTenantsFromAAD(
      IVssRequestContext deploymentContext,
      IVssRequestContext elevatedContext)
    {
      deploymentContext.CheckDeploymentRequestContext();
      IEnumerable<Guid> userTenantsFromAad = (IEnumerable<Guid>) new List<Guid>();
      using (new ActionPerformanceTracer("Commerce", nameof (PlatformSubscriptionService)).Trace(deploymentContext, 5109240, nameof (GetUserTenantsFromAAD)))
      {
        try
        {
          AadService service = deploymentContext.GetService<AadService>();
          IVssRequestContext context = elevatedContext;
          GetTenantsRequest request = new GetTenantsRequest();
          request.ToMicrosoftServicesTenant = true;
          userTenantsFromAad = (IEnumerable<Guid>) service.GetTenants(context, request).Tenants.Select<AadTenant, Guid>((Func<AadTenant, Guid>) (tenant => tenant.ObjectId)).ToList<Guid>();
        }
        catch (AadGraphException ex)
        {
          deploymentContext.TraceException(5106999, "Commerce", nameof (PlatformSubscriptionService), (Exception) ex);
        }
      }
      return userTenantsFromAad;
    }

    public ISubscriptionAccount GetSubscriptionAccountForUser(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid hostId,
      IEnumerable<Guid> serviceOwnerIds)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      IVssRequestContext requestContext1 = vssRequestContext.Elevate();
      using (new ActionPerformanceTracer("Commerce", nameof (PlatformSubscriptionService)).Trace(requestContext1, 5109263, nameof (GetSubscriptionAccountForUser)))
      {
        try
        {
          bool flag = requestContext.UserContext.IsCspPartnerIdentityType();
          requestContext.TraceAlways(5109263, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Parameters -  {0}, {1} isCspPartnerIdentityType: {2}", (object) providerNamespace, (object) hostId, (object) flag));
          Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity = requestContext.GetAuthenticatedIdentity();
          ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(requestorIdentity, "requestorIdentity");
          ISubscriptionAccount subscriptionAccount = (ISubscriptionAccount) null;
          if (flag)
          {
            Guid userTenantId = AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext);
            ArgumentUtility.CheckForEmptyGuid(userTenantId, "userTenantId");
            CollectionHelper.WithCollectionContext(requestContext1, hostId, (Action<IVssRequestContext>) (elevatedSystemContext =>
            {
              Guid organizationAadTenantId = elevatedSystemContext.GetOrganizationAadTenantId();
              requestContext.TraceAlways(5109268, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("userTenantId: {0} collectionTenant: {1}", (object) userTenantId, (object) organizationAadTenantId));
              if (!object.Equals((object) userTenantId, (object) organizationAadTenantId))
                throw new AccessCheckException(string.Format("CSP Identity {0} does not have access to the requested resource", (object) requestorIdentity.SubjectDescriptor));
              subscriptionAccount = this.GetSubscriptionAccountForUser(elevatedSystemContext, requestorIdentity, providerNamespace, serviceOwnerIds);
            }), new RequestContextType?(RequestContextType.SystemContext), nameof (GetSubscriptionAccountForUser));
            return subscriptionAccount;
          }
          CollectionHelper.WithCollectionContext(requestContext1, hostId, (Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) ((collectionContext, identity) => subscriptionAccount = this.GetSubscriptionAccountForUser(collectionContext, requestorIdentity, providerNamespace, serviceOwnerIds)), false, requestorIdentity, nameof (GetSubscriptionAccountForUser));
          return subscriptionAccount;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(5109263, "Commerce", nameof (PlatformSubscriptionService), ex);
          throw;
        }
        finally
        {
          requestContext.TraceLeave(5108874, "Commerce", nameof (PlatformSubscriptionService), nameof (GetSubscriptionAccountForUser));
        }
      }
    }

    private ISubscriptionAccount GetSubscriptionAccountForUser(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      AccountProviderNamespace providerNamespace,
      IEnumerable<Guid> serviceOwnerIds)
    {
      ISubscriptionAccount subscriptionAccountForUser1 = (ISubscriptionAccount) null;
      Guid guid = new Guid();
      ArgumentUtility.CheckForNull<IVssRequestContext>(collectionContext, nameof (collectionContext));
      Microsoft.VisualStudio.Services.Organization.Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
      if (collection == null)
      {
        collectionContext.TraceAlways(5109263, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Properties could not retrieved from SPS for the host {0} ", (object) collectionContext.ServiceHost?.InstanceId) + "with description " + collectionContext.ServiceHost?.ParentServiceHost?.Description);
        return subscriptionAccountForUser1;
      }
      Guid owner = collection.Owner;
      collectionContext.GetService<IPermissionCheckerService>().CheckPermission(collectionContext, 1, CollectionBasedPermission.NamespaceId, "/Meter");
      bool isAccountOwner = identity.Id == owner || CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionContext, identity.Id, 5109031, "Commerce", nameof (PlatformSubscriptionService));
      IVssRequestContext requestContext = collectionContext;
      List<Guid> accountIds = new List<Guid>();
      accountIds.Add(collection.Id);
      int providerNamespaceId = (int) providerNamespace;
      AzureSubscriptionAccount azureSubscriptionAccount = this.GetSubscriptionsForAccountsFromDatabase(requestContext, (IEnumerable<Guid>) accountIds, (AccountProviderNamespace) providerNamespaceId).SingleOrDefault<AzureSubscriptionAccount>();
      ISubscriptionAccount subscriptionAccountForUser2 = (ISubscriptionAccount) this.ConstructSubscriptionAccount(collectionContext, collection, azureSubscriptionAccount, isAccountOwner);
      subscriptionAccountForUser2.ServiceUrls = (IDictionary<Guid, Uri>) this.GetServiceOwnerUrls(collectionContext, (string) null, serviceOwnerIds);
      collectionContext.TraceProperties<ISubscriptionAccount>(5109263, "Commerce", nameof (PlatformSubscriptionService), subscriptionAccountForUser2, (string) null);
      return subscriptionAccountForUser2;
    }

    public ISubscriptionAccount GetSubscriptionAccount(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid hostId)
    {
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      requestContext.TraceEnter(5108873, "Commerce", nameof (PlatformSubscriptionService), new object[2]
      {
        (object) providerNamespace,
        (object) hostId
      }, nameof (GetSubscriptionAccount));
      try
      {
        IVssRequestContext elevatedContext = deploymentContext.Elevate();
        Guid accountOwner = new Guid();
        ISubscriptionAccount subscriptionAccount = (ISubscriptionAccount) null;
        string collectionName;
        CollectionHelper.WithCollectionContext(elevatedContext, hostId, (Action<IVssRequestContext>) (collectionContext =>
        {
          Microsoft.VisualStudio.Services.Organization.Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
          if (collection == null)
          {
            requestContext.Trace(5108875, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Host {0} has description {1}", (object) collectionContext.ServiceHost.InstanceId, (object) collectionContext.ServiceHost.ParentServiceHost.Description));
            if (!ServiceHostTags.FromString(collectionContext.ServiceHost.ParentServiceHost.Description).HasTag(WellKnownServiceHostTags.NoOrgMetadata))
              requestContext.Trace(5108876, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), string.Format("Host {0} has no collection but is not an infrastructure host", (object) collectionContext.ServiceHost.InstanceId));
            requestContext.Trace(5108875, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Host {0} was determined to be an infrastructure host", (object) collectionContext.ServiceHost.InstanceId));
            collectionName = collectionContext.ServiceHost.Name;
          }
          else
          {
            collectionName = collection.Name;
            accountOwner = collection.Owner;
          }
          collectionContext.GetService<IPermissionCheckerService>().CheckPermission(collectionContext, 1, CollectionBasedPermission.NamespaceId, "/Meter");
          bool isOwnerOrPca = false;
          Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = deploymentContext.GetAuthenticatedIdentity();
          if (authenticatedIdentity != null)
            isOwnerOrPca = (authenticatedIdentity.MasterId != Guid.Empty ? authenticatedIdentity.MasterId : authenticatedIdentity.Id) == accountOwner || CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionContext, authenticatedIdentity, 5109031, "Commerce", nameof (PlatformSubscriptionService));
          IVssRequestContext requestContext1 = elevatedContext;
          List<Guid> accountIds = new List<Guid>();
          accountIds.Add(hostId);
          int providerNamespaceId = (int) providerNamespace;
          subscriptionAccount = (ISubscriptionAccount) this.GetSubscriptionsForAccountsFromDatabase(requestContext1, (IEnumerable<Guid>) accountIds, (AccountProviderNamespace) providerNamespaceId).Where<AzureSubscriptionAccount>((Func<AzureSubscriptionAccount, bool>) (sa => sa.OperationResult == OperationResult.Succeeded)).Select<AzureSubscriptionAccount, SubscriptionAccount>((Func<AzureSubscriptionAccount, SubscriptionAccount>) (s => new SubscriptionAccount()
          {
            AccountId = s.AccountId,
            SubscriptionId = s.SubscriptionId,
            SubscriptionStatus = s.SubscriptionStatusId,
            AccountName = collectionName,
            ResourceGroupName = s.ResourceGroupName,
            ResourceName = s.ResourceName,
            GeoLocation = s.GeoLocation,
            IsAccountOwner = isOwnerOrPca,
            OfferType = s.SubscriptionOfferType
          })).SingleOrDefault<SubscriptionAccount>();
          requestContext.TraceProperties<ISubscriptionAccount>(5108875, "Commerce", nameof (PlatformSubscriptionService), subscriptionAccount, (string) null);
        }), method: nameof (GetSubscriptionAccount));
        return subscriptionAccount;
      }
      finally
      {
        requestContext.TraceLeave(5108874, "Commerce", nameof (PlatformSubscriptionService), nameof (GetSubscriptionAccount));
      }
    }

    public ISubscriptionAccount GetAzureSubscriptionFromName(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string accountName,
      AccountProviderNamespace providerNamespaceId,
      IEnumerable<Guid> serviceOwners)
    {
      if (subscriptionId == Guid.Empty)
        throw new ArgumentException(string.Format("Subscription id is not valid {0}", (object) subscriptionId));
      if (string.IsNullOrEmpty(accountName))
        throw new ArgumentException("Account name is not valid " + accountName);
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceAlways(CommerceTracePoints.PlatformSubscriptionService.GetAzureSubscriptionFromName.Enter, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Platform subscription serivice GetSubscriptionAccountByName. Parameters providerNamespaceId={0}, account={1}, subscription={2}", (object) providerNamespaceId, (object) accountName, (object) subscriptionId));
      ICommerceAccountHandler extension = requestContext.GetExtension<ICommerceAccountHandler>();
      Guid? userId = CommerceIdentityHelper.GetRequestAuthenticatedIdentityId(requestContext);
      if (!userId.HasValue || userId.Value == Guid.Empty)
        throw new IdentityNotFoundException("The provided identity does not exists.");
      IVssRequestContext context = requestContext;
      Guid userId1 = userId.Value;
      IList<Guid> guidList = extension.QueryAccountIds(context, userId1, UserType.Member);
      Guid? hostId = CommerceDeploymentHelper.ResolveCollectionHostName(requestContext, accountName);
      if (guidList.IsNullOrEmpty<Guid>() || !hostId.HasValue)
        throw new AzureResourceAccountDoesNotExistException(accountName);
      if (!guidList.Any<Guid>((Func<Guid, bool>) (x => x == hostId.Value)))
        throw new AccessCheckException("User does not have access to the organization " + accountName);
      try
      {
        IVssRequestContext requestContext1 = requestContext.Elevate();
        SubscriptionAccount subAccount = (SubscriptionAccount) null;
        Guid hostId1 = hostId.Value;
        Action<IVssRequestContext> action = (Action<IVssRequestContext>) (collectionContext =>
        {
          AzureResourceAccount accountByCollectionId = this.GetAzureResourceAccountByCollectionId(collectionContext, hostId.Value);
          if (accountByCollectionId == null)
            throw new AzureResourceAccountDoesNotExistException("Resource account is not found");
          if (accountByCollectionId.AzureSubscriptionId != subscriptionId)
            throw new InvalidOperationException(string.Format("Subscription {0} is different than the corresponding one under the account {1}", (object) subscriptionId, (object) accountName));
          Microsoft.VisualStudio.Services.Organization.Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
          if (collection == null || collection.Owner == CommerceConstants.MsdnAdminGuid || collection.Status != CollectionStatus.Enabled)
            return;
          Guid owner = collection.Owner;
          Guid? nullable = userId;
          bool isAccountOwner = (nullable.HasValue ? (owner == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0 || CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionContext, userId.Value, 5109031, "Commerce", nameof (PlatformSubscriptionService));
          subAccount = this.ConstructSubscriptionAccountFromAzureRecource(collectionContext, collection, accountByCollectionId, isAccountOwner);
          subAccount.ServiceUrls = (IDictionary<Guid, Uri>) this.GetServiceOwnerUrls(collectionContext, (string) null, serviceOwners);
        });
        RequestContextType? requestContextType = new RequestContextType?();
        CollectionHelper.WithCollectionContext(requestContext1, hostId1, action, requestContextType, nameof (GetAzureSubscriptionFromName));
        return (ISubscriptionAccount) subAccount;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(CommerceTracePoints.PlatformSubscriptionService.GetAzureSubscriptionFromName.RetrieveAccountException, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
    }

    public virtual bool IsAssignmentBillingEnabled(
      IVssRequestContext requestContext,
      Guid accountId)
    {
      try
      {
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableIsAssignmentBillingEnabledInUserContext"))
        {
          if (requestContext.IsUserContext)
          {
            SubjectDescriptor subjectDescriptor = requestContext.GetAuthenticatedDescriptor().ToSubjectDescriptor(requestContext);
            if (!CommerceDeploymentHelper.IsProjectCollectionValidUser(requestContext, subjectDescriptor, accountId))
              throw new InvalidAccessException(string.Format("User {0} does not have permissions to access billing information", (object) subjectDescriptor));
            requestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(CommerceTracePoints.PlatformSubscriptionService.IsAssignmentBillingEnabled.PermissionCheckException, "Commerce", nameof (PlatformSubscriptionService), ex);
        return false;
      }
      requestContext.Trace(5109242, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("PlatformSubscriptionService.IsAssignmentBillingEnabled is called for accountId {0}", (object) accountId));
      requestContext.CheckDeploymentRequestContext();
      AzureResourceAccount accountByCollectionId = this.GetAzureResourceAccountByCollectionId(requestContext, accountId);
      if (accountByCollectionId == null)
      {
        requestContext.TraceAlways(5109340, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Azure resource for organization {0} is null.", (object) accountId));
        return false;
      }
      AzureSubscriptionInternal azureSubscription = this.GetAzureSubscription(requestContext, accountByCollectionId.AzureSubscriptionId);
      if (requestContext.IsFeatureEnabled(accountId, "Microsoft.Azure.DevOps.Commerce.EnableAssignmentBasedBilling") && azureSubscription.AzureSubscriptionStatusId == SubscriptionStatus.Active)
      {
        requestContext.TraceAlways(5109341, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("EnableAssignmentBasedBilling FF is enabled and subscription {0} is active for organization {1}.", (object) azureSubscription.AzureSubscriptionStatusId, (object) accountId));
        return true;
      }
      bool flag = AssignmentBillingHelper.IsAssignmentBillingEnabledForSubscription(requestContext, accountByCollectionId.AzureSubscriptionId);
      requestContext.TraceAlways(5109342, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("PlatformSubscriptionService.IsAssignmentBillingEnabled returns {0} for subscription {1} and organization {2}.", (object) flag, (object) azureSubscription.AzureSubscriptionStatusId, (object) accountId));
      return flag;
    }

    internal virtual void MoveCollection(
      IVssRequestContext requestContext,
      string collectionName,
      Guid sourceSubscriptionId,
      string sourceResourceGroupName,
      Guid targetSubscriptionId,
      string targetResourceGroupName,
      bool validateOnly = false)
    {
      requestContext.TraceEnter(5109110, "Commerce", nameof (PlatformSubscriptionService), new object[6]
      {
        (object) collectionName,
        (object) sourceSubscriptionId,
        (object) sourceResourceGroupName,
        (object) targetSubscriptionId,
        (object) targetResourceGroupName,
        (object) validateOnly
      }, nameof (MoveCollection));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(sourceSubscriptionId, nameof (sourceSubscriptionId));
        ArgumentUtility.CheckForEmptyGuid(targetSubscriptionId, nameof (targetSubscriptionId));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(sourceResourceGroupName, nameof (sourceResourceGroupName));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(targetResourceGroupName, nameof (targetResourceGroupName));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(collectionName, nameof (collectionName));
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableResourceGroupSwapMoveResourcesAPI") && !string.Equals(sourceResourceGroupName, targetResourceGroupName, StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException(HostingResources.ResourceGroupNameCannotBeChangedWhileMovingResourcesSourceRG0TargetRG1((object) sourceResourceGroupName, (object) targetResourceGroupName), nameof (targetResourceGroupName));
        Guid collectionId;
        if (!this.collectionNameResolver(requestContext, collectionName, out collectionId))
          throw new InvalidOperationException(HostingResources.FailedToGetAccountDetailsFor0((object) collectionName));
        ISubscriptionAccount subscriptionAccount = this.GetSubscriptionAccount(requestContext, AccountProviderNamespace.VisualStudioOnline, collectionId);
        if (subscriptionAccount == null || !subscriptionAccount.SubscriptionId.Equals((object) sourceSubscriptionId) || !string.Equals(subscriptionAccount.ResourceGroupName, sourceResourceGroupName, StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException(HostingResources.Account0NotFoundInResourceGroup1InSubscription2((object) collectionName, (object) sourceResourceGroupName, (object) sourceSubscriptionId), nameof (collectionName));
        IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
        if (!deploymentContext.IsFeatureEnabled("VisualStudio.Services.Commerce.DisableThirdPartyOfferSubscriptionsCountCheckFeatureFlag"))
          CollectionHelper.WithCollectionContext(deploymentContext, collectionId, (Action<IVssRequestContext>) (collectionContext =>
          {
            if (!deploymentContext.IsFeatureEnabled("VisualStudio.Services.Commerce.CancelThirdPartyPurchasesForSubscriptionSwap") && !this.GetThirdPartyOfferSubscriptions(collectionContext.Elevate()).IsNullOrEmpty<IOfferSubscription>())
              throw new NotImplementedException("To move this account to a new subscription, first cancel any non-Microsoft extension purchases.<a href=\"https://go.microsoft.com/fwlink/?linkid=870104\">Learn more</a>");
          }), method: nameof (MoveCollection));
        this.ChangeAzureResourceAccountCollection(requestContext, targetSubscriptionId, targetResourceGroupName, AccountProviderNamespace.VisualStudioOnline, collectionId, false, validateOnly);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5109111, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5109112, "Commerce", nameof (PlatformSubscriptionService), nameof (MoveCollection));
      }
    }

    internal virtual AzureResourceAccount LinkCollectionInternal(
      IVssRequestContext collectionContext,
      AzureResourceAccount azureResourceAccount,
      Guid hostId,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      IVssRequestContext vssRequestContext1 = collectionContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext vssRequestContext2 = collectionContext.To(TeamFoundationHostType.Application);
      collectionContext.TraceEnter(5105351, "Commerce", nameof (PlatformSubscriptionService), new object[3]
      {
        (object) azureResourceAccount,
        (object) hostId,
        (object) requestorIdentity
      }, nameof (LinkCollectionInternal));
      try
      {
        ArgumentUtility.CheckForNull<AzureResourceAccount>(azureResourceAccount, nameof (azureResourceAccount));
        ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(requestorIdentity, nameof (requestorIdentity));
        ArgumentUtility.CheckForEmptyGuid(azureResourceAccount.AzureSubscriptionId, "AzureSubscriptionId");
        ArgumentUtility.CheckForEmptyGuid(requestorIdentity.Id, "Id");
        ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
        if (vssRequestContext1.GetService<CommerceHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext1, hostId) == null)
          throw new AccountNotFoundException(azureResourceAccount.AccountId);
        this.CheckCollectionAndOwnership(collectionContext, requestorIdentity);
        collectionContext = collectionContext.Elevate();
        IVssRequestContext vssRequestContext3 = vssRequestContext1.Elevate();
        azureResourceAccount.CollectionId = collectionContext.ServiceHost.InstanceId;
        azureResourceAccount.AccountId = vssRequestContext2.ServiceHost.InstanceId;
        string name = collectionContext.ServiceHost.Name;
        AzureResourceAccount accountByCollectionId = this.GetAzureResourceAccountByCollectionId(vssRequestContext3, hostId, true);
        if (accountByCollectionId != null)
        {
          if (accountByCollectionId.AzureSubscriptionId == azureResourceAccount.AzureSubscriptionId)
            return accountByCollectionId;
          AccountAlreadyLinkedException alreadyLinkedException = new AccountAlreadyLinkedException(name);
          collectionContext.TraceException(5105359, "Commerce", nameof (PlatformSubscriptionService), (Exception) alreadyLinkedException);
          throw alreadyLinkedException;
        }
        try
        {
          AzureResourceAccount azureResourceAccount1 = this.GetAzureResourceAccount(vssRequestContext3, azureResourceAccount.AzureSubscriptionId, azureResourceAccount.ProviderNamespaceId, azureResourceAccount.AzureResourceName, true);
          if (azureResourceAccount1 != null)
          {
            azureResourceAccount1.CollectionId = collectionContext.ServiceHost.InstanceId;
            azureResourceAccount1.AccountId = vssRequestContext2.ServiceHost.InstanceId;
            azureResourceAccount1.OperationResult = OperationResult.Succeeded;
            azureResourceAccount1.AzureGeoRegion = azureResourceAccount.AzureGeoRegion;
            this.UpdateAzureResourceAccount(vssRequestContext3, azureResourceAccount1);
            azureResourceAccount = azureResourceAccount1;
          }
          else
          {
            if (string.IsNullOrEmpty(azureResourceAccount.AzureResourceName))
              azureResourceAccount.AzureResourceName = name;
            azureResourceAccount.OperationResult = OperationResult.Succeeded;
            this.CreateAzureResourceAccount(vssRequestContext3, azureResourceAccount);
          }
          if (AssignmentBillingHelper.IsAssignmentBillingEnabledForSubscription(vssRequestContext3, azureResourceAccount.AzureSubscriptionId))
            this.SetIncludedQuantityForAssignmentBilling(collectionContext, vssRequestContext3, azureResourceAccount);
          this.PublishAccountStatusChangeNotification(collectionContext, azureResourceAccount.AzureSubscriptionId, azureResourceAccount.AccountId, azureResourceAccount.ProviderNamespaceId, AccountStatusChangeEventType.LinkedToSubscription, 5105355);
          this.TraceBIEventForAzureSubscriptionChanges(collectionContext, collectionContext.ServiceHost.InstanceId, azureResourceAccount, (AzureSubscriptionInternal) null, false);
          try
          {
            string str = CommerceUtil.CheckForRequestSource(collectionContext);
            CustomerIntelligenceData eventData = new CustomerIntelligenceData();
            eventData.Add(CustomerIntelligenceProperty.AccountOwner, (object) requestorIdentity.Id);
            eventData.Add(CustomerIntelligenceProperty.ResourceName, azureResourceAccount.AzureResourceName);
            eventData.Add(CustomerIntelligenceProperty.AccountName, name);
            eventData.Add(CustomerIntelligenceProperty.SubscriptionId, (object) azureResourceAccount.AzureSubscriptionId);
            eventData.Add(CustomerIntelligenceProperty.AccountLinkSource, str);
            CustomerIntelligence.PublishEvent(vssRequestContext3, "LinkAccountToAzureSubscription", eventData);
          }
          catch (Exception ex)
          {
            collectionContext.Trace(5106096, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), "Exception while publishing CI data " + ex.ToString());
          }
          CommerceAuditUtilities.LogSubscriptionLink(collectionContext, azureResourceAccount.AzureSubscriptionId);
          collectionContext.GetService<PlatformOfferSubscriptionService>().EnablePaidBillingModeOnLinkingAccount(collectionContext);
          return azureResourceAccount;
        }
        catch (Exception ex)
        {
          collectionContext.TraceException(5105379, "Commerce", nameof (PlatformSubscriptionService), ex);
          if (azureResourceAccount != null)
          {
            collectionContext.Trace(5105378, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Deleting the azure resource account " + azureResourceAccount.AzureResourceName);
            this.DeleteAzureResourceAccountFromDatabase(collectionContext, azureResourceAccount);
            this.TraceBIEventForAzureSubscriptionChanges(collectionContext, collectionContext.ServiceHost.InstanceId, azureResourceAccount, (AzureSubscriptionInternal) null, true);
          }
          throw;
        }
      }
      finally
      {
        collectionContext.TraceLeave(5105360, "Commerce", nameof (PlatformSubscriptionService), nameof (LinkCollectionInternal));
      }
    }

    internal virtual void UpdateAzureSubscriptionAsynchronously(
      IVssRequestContext collectionContext,
      AzureResourceAccount azureResourceAccount)
    {
      collectionContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(collectionContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((rq, state) =>
      {
        try
        {
          AzureSubscriptionInternal azureSubscription = this.GetAzureSubscription(rq, azureResourceAccount.AzureSubscriptionId);
          this.UpdateAzureSubscriptionWithBillingInfo(rq, azureSubscription);
        }
        catch (Exception ex)
        {
          collectionContext.Trace(5105370, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), "Updating azure Subscription(" + azureResourceAccount.Serialize<AzureResourceAccount>() + ") failed with " + ex.ToString() + ". Stack: " + ex.StackTrace);
        }
      })));
    }

    internal virtual void CheckCollectionAndOwnership(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      ServiceHostTags serviceHostTags = ServiceHostTags.FromString(collectionContext.ServiceHost.ParentServiceHost.Description);
      if (serviceHostTags.HasTag(WellKnownServiceHostTags.NoOrgMetadata) || serviceHostTags.HasTag(WellKnownServiceHostTags.IsInfrastructureHost))
        return;
      IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      if (ServicePrincipals.IsServicePrincipal(collectionContext, requestorIdentity.Descriptor))
      {
        this.CheckPermission(requestContext, 2);
      }
      else
      {
        this.CheckSubscriptionLinkPermission(collectionContext);
        collectionContext.Trace(5105352, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Collection Id:{0}, requestor Id:{1}", (object) collectionContext.ServiceHost.InstanceId, (object) requestorIdentity.MasterId));
      }
    }

    internal virtual AzureResourceAccount LinkCollectionByName(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount,
      string accountName,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException();
      requestContext.TraceEnter(5105361, "Commerce", nameof (PlatformSubscriptionService), new object[3]
      {
        (object) azureResourceAccount,
        (object) accountName,
        (object) requestorIdentity
      }, nameof (LinkCollectionByName));
      try
      {
        this.CheckPermission(requestContext, 2);
        Guid collectionId;
        if (!HostNameResolver.TryGetCollectionServiceHostId(requestContext, accountName, out collectionId))
          throw new AccountNotFoundException(accountName);
        this.UpdateAzureSubscriptionAsynchronously(requestContext, azureResourceAccount);
        AzureResourceAccount resourceAccount = (AzureResourceAccount) null;
        CollectionHelper.WithCollectionContext(requestContext, collectionId, (Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) ((collectionContext, identity) => resourceAccount = this.LinkAccountInUserContext(collectionContext, collectionId, azureResourceAccount, identity)), false, requestorIdentity, nameof (LinkCollectionByName));
        return resourceAccount;
      }
      finally
      {
        requestContext.TraceLeave(5105370, "Commerce", nameof (PlatformSubscriptionService), nameof (LinkCollectionByName));
      }
    }

    public void UpdateAzureSubscriptionWithBillingInfo(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal subscription)
    {
      requestContext.TraceEnter(5107407, "Commerce", nameof (PlatformSubscriptionService), nameof (UpdateAzureSubscriptionWithBillingInfo));
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      IAzureBillingService service1 = context.GetService<IAzureBillingService>();
      IArmAdapterService service2 = context.GetService<IArmAdapterService>();
      try
      {
        if (subscription == null)
        {
          requestContext.Trace(5107404, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), "Subscription is null");
        }
        else
        {
          CommerceBillingContextInfo contextForSubscription = service1.GetBillingContextForSubscription(requestContext, subscription.AzureSubscriptionId);
          if (contextForSubscription == null)
          {
            requestContext.Trace(5107404, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), "Billing Context is null");
          }
          else
          {
            requestContext.Trace(5107404, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Billing Context Details TenantId: {0} ObjectId: {1}", (object) contextForSubscription.TenantId, (object) contextForSubscription.ObjectId));
            AzureSubscriptionInfo subscriptionForUser = service2.GetSubscriptionForUser(requestContext, subscription.AzureSubscriptionId);
            if (subscriptionForUser == null)
            {
              requestContext.Trace(5107405, TraceLevel.Error, "Commerce", nameof (PlatformSubscriptionService), "ACIS Subscription Status Id is null");
            }
            else
            {
              requestContext.Trace(5107405, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "ACIS Subscription Billing Info: " + subscriptionForUser.ToString());
              if (subscriptionForUser.Status == subscription.AzureSubscriptionStatusId && !(subscription.AzureOfferCode != subscriptionForUser.QuotaId))
                return;
              SubscriptionStatus subscriptionStatusId = subscription.AzureSubscriptionStatusId;
              string azureOfferCode = subscription.AzureOfferCode;
              subscription.AzureSubscriptionStatusId = subscriptionForUser.Status;
              subscription.AzureOfferCode = subscriptionForUser.QuotaId;
              if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableSubscriptionStatusSyncJobExtension"))
                this.UpdateAzureSubscription(requestContext, subscription);
              requestContext.Trace(5107406, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Old Status: {0} Updated Status: {1}; ", (object) subscriptionStatusId, (object) subscription.AzureSubscriptionStatusId) + "Old offerCode: " + azureOfferCode + ", Updated offerCode:" + subscription.AzureOfferCode + "; ");
            }
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5107408, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5107409, "Commerce", nameof (PlatformSubscriptionService), nameof (UpdateAzureSubscriptionWithBillingInfo));
      }
    }

    private AzureResourceAccount LinkAccountInUserContext(
      IVssRequestContext elevatedRequestContext,
      Guid collectionId,
      AzureResourceAccount azureResourceAccount,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity)
    {
      using (CommerceVssRequestContextExtensions.VssRequestContextHolder collection1 = elevatedRequestContext.ToCollection(collectionId, requestorIdentity.Descriptor))
      {
        IVssRequestContext requestContext = collection1.RequestContext;
        if (string.IsNullOrEmpty(azureResourceAccount.AzureGeoRegion))
        {
          Microsoft.VisualStudio.Services.Organization.Collection collection2 = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) null);
          ICommerceRegionHandler extension = elevatedRequestContext.GetExtension<ICommerceRegionHandler>();
          azureResourceAccount.AzureGeoRegion = extension.GetRegionDisplayName(elevatedRequestContext, collection2.PreferredRegion);
        }
        return this.LinkCollectionInternal(requestContext, azureResourceAccount, collectionId, requestorIdentity);
      }
    }

    internal virtual void InitializeInProgressOperation(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount)
    {
      azureResourceAccount.OperationResult = OperationResult.InProgress;
      this.CreateAzureResourceAccount(requestContext, azureResourceAccount);
    }

    internal virtual AzureSubscriptionInternal EnsureSubscriptionExistForAzureResourceAccount(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount)
    {
      AzureSubscriptionInternal subscription = this.GetAzureSubscription(requestContext, azureResourceAccount.AzureSubscriptionId);
      if (subscription == null)
      {
        requestContext.TraceAlways(5105335, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("No Azure Subscription with subscription id {0} exists for Azure Resource Account with accountId {1} collectionId {2}", (object) azureResourceAccount.AzureSubscriptionId, (object) azureResourceAccount.AccountId, (object) azureResourceAccount.CollectionId));
        subscription = new AzureSubscriptionInternal()
        {
          AzureSubscriptionId = azureResourceAccount.AzureSubscriptionId,
          AzureSubscriptionStatusId = SubscriptionStatus.Active,
          AzureSubscriptionTenantId = this.RetrieveTenantByOrganizationContext(requestContext, azureResourceAccount.AccountId, azureResourceAccount.AzureSubscriptionId)
        };
        requestContext.Trace(5105328, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Creating Azure Subscription with subscription id: {0}, status as Active and with Anniversary day of 1.", (object) azureResourceAccount.AzureSubscriptionId));
        this.CreateAzureSubscription(requestContext, subscription);
      }
      else if (requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.EnableExistedAzureSubscriptionDualWrite"))
      {
        subscription.AzureSubscriptionTenantId = this.RetrieveTenantByOrganizationContext(requestContext, azureResourceAccount.AccountId, azureResourceAccount.AzureSubscriptionId);
        this.DualWriteAzureSubscription(requestContext, subscription);
      }
      return subscription;
    }

    internal virtual Guid RetrieveTenantByOrganizationContext(
      IVssRequestContext requestContext,
      Guid organizationId,
      Guid subscriptionId)
    {
      Guid guid = Guid.Empty;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableTenantRetrievalFromOrgContext"))
      {
        Microsoft.VisualStudio.Services.Organization.Client.Organization organization = HttpClientHelper.CreateSpsClient<OrganizationHttpClient>(requestContext, organizationId, InstanceManagementHelper.ServicePrincipalFromServiceInstance(ServiceInstanceTypes.SPS)).GetOrganizationAsync("me").SyncResult<Microsoft.VisualStudio.Services.Organization.Client.Organization>();
        if (organization != null)
        {
          Guid tenantId = organization.TenantId;
          guid = organization.TenantId;
          requestContext.TraceAlways(5109367, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("TenantId - {0} - associated to the subscription {1}", (object) guid, (object) subscriptionId));
        }
        else
          requestContext.Trace(5109368, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), "Organization not found");
      }
      return guid;
    }

    internal virtual void DualWriteAzureSubscription(
      IVssRequestContext requestContext,
      AzureSubscriptionInternal subscription)
    {
      DualWritesHelper.DualWriteAzureSubscription(requestContext.To(TeamFoundationHostType.Deployment), subscription);
    }

    internal virtual bool CheckIfAccountExists(
      IVssRequestContext requestContext,
      string accountName)
    {
      return HostCreationHelper.CheckIfCollectionNameExists(requestContext, accountName);
    }

    public virtual AzureResourceAccount CreateAndLinkAccount(
      IVssRequestContext requestContext,
      AzureResourceAccount azureResourceAccount,
      string accountName,
      Microsoft.VisualStudio.Services.Identity.Identity ownerIdentity,
      CustomerIntelligenceEntryPoint source)
    {
      requestContext.CheckHostedDeployment();
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext deploymentContext = vssRequestContext1.Elevate();
      vssRequestContext1.TraceEnter(5105371, "Commerce", nameof (PlatformSubscriptionService), new object[4]
      {
        (object) azureResourceAccount,
        (object) accountName,
        (object) ownerIdentity,
        (object) source
      }, nameof (CreateAndLinkAccount));
      try
      {
        this.CheckPermission(vssRequestContext1, 2);
        IVssRequestContext vssRequestContext2 = vssRequestContext1.Elevate();
        IdentityService service = vssRequestContext2.GetService<IdentityService>();
        if (ownerIdentity.Id == Guid.Empty)
          service.UpdateIdentities(vssRequestContext2, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            ownerIdentity
          });
        if (!this.CheckIfAccountExists(vssRequestContext1, accountName))
        {
          string preferredRegion = string.Empty;
          if (!string.IsNullOrEmpty(azureResourceAccount.AzureGeoRegion))
            preferredRegion = vssRequestContext1.GetService<IHostAcquisitionService>().GetRegions(vssRequestContext1).FirstOrDefault<Microsoft.VisualStudio.Services.HostAcquisition.Region>((Func<Microsoft.VisualStudio.Services.HostAcquisition.Region, bool>) (r => string.Equals(r.DisplayName, azureResourceAccount.AzureGeoRegion, StringComparison.OrdinalIgnoreCase) || string.Equals(r.NameInAzure, azureResourceAccount.AzureGeoRegion, StringComparison.OrdinalIgnoreCase)))?.Name;
          if (string.IsNullOrEmpty(preferredRegion))
          {
            if (vssRequestContext1.ServiceHost.IsProduction)
              throw new UnsupportedRegionException(azureResourceAccount.AzureGeoRegion);
            preferredRegion = this.GetDefaultIbizaAccountRegion(vssRequestContext1);
          }
          vssRequestContext1.Trace(5105372, TraceLevel.Verbose, "Commerce", nameof (PlatformSubscriptionService), string.Format("Account creation user identity Id is {0}", (object) ownerIdentity.MasterId));
          AcquisitionFlowHelper acquisitionFlowHelper = new AcquisitionFlowHelper();
          Dictionary<string, object> creationProperties = new Dictionary<string, object>()
          {
            {
              "SignupEntryPoint",
              (object) source
            },
            {
              "HIPVerify.Bypass",
              (object) true
            }
          };
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source1 = service.ReadIdentities(vssRequestContext2, (IList<Guid>) new Guid[1]
          {
            CommerceConstants.SpsMasterId
          }, QueryMembership.None, (IEnumerable<string>) null);
          Microsoft.VisualStudio.Services.Identity.Identity creatorIdentity = ownerIdentity.IsBindPending ? source1.First<Microsoft.VisualStudio.Services.Identity.Identity>() : ownerIdentity;
          acquisitionFlowHelper.CreateCollection(deploymentContext, accountName, preferredRegion, creatorIdentity, (IDictionary<string, object>) creationProperties);
          Guid collectionId;
          if (!HostNameResolver.TryGetCollectionServiceHostId(vssRequestContext1, accountName, out collectionId))
            throw new AccountServiceFailureException(accountName, ownerIdentity);
          if (ownerIdentity.IsBindPending)
          {
            Guid ownerDomain = ownerIdentity.GetProperty<Guid>("Domain", Guid.Empty);
            CollectionHelper.WithCollectionContext(requestContext, collectionId, (Action<IVssRequestContext>) (collectionContext =>
            {
              if (requestContext.IsCommerceService())
              {
                collectionContext.GetClient<CommerceHostHelperHttpClient>().UpdateCollectionOwner(ownerIdentity.Id, ownerDomain).SyncResult<bool>();
              }
              else
              {
                if (ownerDomain != Guid.Empty)
                {
                  IVssRequestContext requestContext1 = collectionContext.To(TeamFoundationHostType.Deployment);
                  requestContext1.GetExtension<ICommerceAccountHandler>().UpdateAccountTenant(requestContext1, ownerDomain, collectionContext.ServiceHost.ParentServiceHost.InstanceId);
                }
                collectionContext.Items["Organization.ChangeOwnerAccountLinkingFlow"] = (object) true;
                collectionContext.GetService<ICollectionService>().UpdateCollectionOwner(collectionContext, ownerIdentity.MasterId);
              }
            }), new RequestContextType?(RequestContextType.SystemContext), nameof (CreateAndLinkAccount));
          }
        }
        else
        {
          AzureResourceAccount azureResourceAccount1 = this.GetAzureResourceAccount(vssRequestContext1, azureResourceAccount.AzureSubscriptionId, azureResourceAccount.ProviderNamespaceId, azureResourceAccount.AzureResourceName, true);
          if (azureResourceAccount1 != null && !azureResourceAccount1.AccountId.Equals(Guid.Empty))
          {
            if (azureResourceAccount1.AzureCloudServiceName.Equals(azureResourceAccount.AzureCloudServiceName, StringComparison.OrdinalIgnoreCase))
              return azureResourceAccount1;
            throw new AccountAlreadyLinkedException(accountName);
          }
          if (!requestContext.IsCommerceService())
            throw new AccountToBeCreatedAlreadyExistsException(accountName);
        }
        this.UpdateAzureSubscriptionAsynchronously(requestContext, azureResourceAccount);
        Guid collectionId1;
        if (!HostNameResolver.TryGetCollectionServiceHostId(vssRequestContext1, accountName, out collectionId1))
          throw new AzureResourceAccountMissingException(accountName);
        AzureResourceAccount resourceAccount = (AzureResourceAccount) null;
        CollectionHelper.WithCollectionContext(requestContext, collectionId1, (Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) ((collectionContext, identity) => resourceAccount = this.LinkCollectionInternal(collectionContext, azureResourceAccount, collectionContext.ServiceHost.InstanceId, identity)), true, ownerIdentity, nameof (CreateAndLinkAccount));
        return resourceAccount;
      }
      catch (Exception ex)
      {
        vssRequestContext1.TraceException(5105379, "Commerce", nameof (PlatformSubscriptionService), ex);
        azureResourceAccount.OperationResult = OperationResult.Failed;
        this.UpdateAzureResourceAccount(vssRequestContext1, azureResourceAccount);
        throw;
      }
      finally
      {
        vssRequestContext1.TraceLeave(5105380, "Commerce", nameof (PlatformSubscriptionService), nameof (CreateAndLinkAccount));
      }
    }

    internal virtual string GetDefaultIbizaAccountRegion(IVssRequestContext deploymentContext) => deploymentContext.GetService<IVssRegistryService>().GetValue<string>(deploymentContext, (RegistryQuery) "/Service/Commerce/Commerce/IbizaAccountRegion", "SCUS");

    internal virtual void PublishAccountRightsNotification(
      IVssRequestContext requestContext,
      int tracePointId,
      AccountRightsChangedEvent eventData,
      Guid? accountId = null)
    {
      if (CommerceUtil.IsRunningOnCommerceServiceAsBackup(requestContext) || this.NotificationPublisher == null || eventData == null)
        return;
      AccountRightsChangedEvent[] serializableObjects = new AccountRightsChangedEvent[1]
      {
        eventData
      };
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext1.Trace(tracePointId, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Publishing notification AccountRightsChangedEvent for Account {0}", (object) (accountId ?? eventData.AccountId)));
      this.NotificationPublisher.Publish(requestContext1, (object) eventData);
      if (requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DisablePublishAccountRightsNotification") || !requestContext.IsCommerceService())
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<IMessageBusPublisherService>().TryPublish(vssRequestContext, "Microsoft.TeamFoundation.Services.OfferSubscriptionChangeForLicensing", (object[]) serializableObjects);
    }

    internal virtual void PublishAccountStatusChangeNotification(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Guid hostId,
      AccountProviderNamespace providerNamespace,
      AccountStatusChangeEventType changeType,
      int tracePointId)
    {
      if (this.NotificationPublisher == null)
        return;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      string source = CommerceUtil.CheckForRequestSource(requestContext);
      requestContext1.Trace(tracePointId, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Publishing notification AccountStatusChangedEvent for Account {0}", (object) hostId));
      this.NotificationPublisher.Publish(requestContext1, (object) new AccountStatusChangeEventData(subscriptionId, hostId, providerNamespace, changeType, source));
    }

    internal virtual IList<AzureResourceAccount> GetAccountsInResourceGroups(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string resourceGroupName)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      this.CheckPermission(requestContext1, 1);
      return this.GetAzureResourceAccountsInResourceGroupFromDatabase(requestContext1, subscriptionId, providerNamespaceId, resourceGroupName);
    }

    public IEnumerable<ISubscriptionAccount> GetAzureSubscriptionForUser(
      IVssRequestContext requestContext,
      Guid? subscriptionId = null,
      bool queryAcrossTenants = false)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IAzureResourceHelper>().GetAzureSubscriptionsForUser(vssRequestContext, subscriptionId, queryAcrossTenants);
    }

    public ISubscriptionAccount GetAzureSubscriptionForPurchase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string galleryItemId,
      Guid? collectionId = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(galleryItemId, nameof (galleryItemId));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IAzureResourceHelper>().GetAzureSubscriptionForPurchase(vssRequestContext, subscriptionId, galleryItemId, collectionId);
    }

    internal virtual void AddAzureResourceAccountDeleteCIEvent(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      CustomerIntelligenceEntryPoint source)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add(CustomerIntelligenceProperty.AzureSubscriptionId, subscriptionId.ToString());
      intelligenceData.Add(CustomerIntelligenceProperty.ProviderNamespaceId, ((int) providerNamespaceId).ToString());
      intelligenceData.Add(CustomerIntelligenceProperty.AccountId, accountId.ToString());
      intelligenceData.Add(CustomerIntelligenceProperty.AccountUnlinkSource, source.ToString());
      intelligenceData.Add(CustomerIntelligenceProperty.ActivityId, requestContext.ActivityId.ToString());
      CustomerIntelligenceData eventData = intelligenceData;
      CustomerIntelligence.PublishEvent(requestContext1, "AccountUnlinked", eventData);
    }

    internal virtual IEnumerable<ISubscriptionAccount> GetAccountsInternal(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool includeDisabledAccounts = false,
      IEnumerable<Guid> serviceOwners = null,
      bool addCommerceDetails = true)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext elevatedDeploymentContext = vssRequestContext.Elevate();
      vssRequestContext.TraceEnter(5106147, "Commerce", nameof (PlatformSubscriptionService), new object[5]
      {
        (object) providerNamespace,
        (object) memberId,
        (object) queryOnlyOwnerAccounts,
        (object) includeDisabledAccounts,
        (object) serviceOwners
      }, nameof (GetAccountsInternal));
      ArgumentUtility.CheckForEmptyGuid(memberId, nameof (memberId));
      vssRequestContext.Trace(5106148, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Account owner for this call is {0}", (object) memberId));
      try
      {
        HashSet<Guid> ownerAccountIds = (HashSet<Guid>) null;
        ICommerceAccountHandler extension = vssRequestContext.GetExtension<ICommerceAccountHandler>();
        HashSet<Guid> guidSet;
        if (requestContext.UserContext.IsCspPartnerIdentityType())
        {
          Guid identityTenantId = AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext);
          guidSet = vssRequestContext.GetService<IOrganizationCatalogService>().GetCollections(vssRequestContext.Elevate(), new CollectionQueryContext(CollectionSearchKind.ByTenantId, identityTenantId.ToString())).Where<CollectionRef>((Func<CollectionRef, bool>) (x => x != null)).Select<CollectionRef, Guid>((Func<CollectionRef, Guid>) (x => x.Id)).ToHashSet<Guid>();
          vssRequestContext.Trace(5106148, TraceLevel.Verbose, "Commerce", nameof (PlatformSubscriptionService), string.Format("Found {0} accounts are backed by AAD tenant: {1}", (object) guidSet.Count, (object) identityTenantId));
        }
        else if (!queryOnlyOwnerAccounts)
        {
          guidSet = new HashSet<Guid>((IEnumerable<Guid>) extension.QueryAccountIds(vssRequestContext, memberId, UserType.Member, includeDeletedAccounts: includeDisabledAccounts));
          ownerAccountIds = new HashSet<Guid>((IEnumerable<Guid>) extension.QueryAccountIds(vssRequestContext, memberId, UserType.Owner, includeDeletedAccounts: includeDisabledAccounts));
          vssRequestContext.Trace(5106148, TraceLevel.Verbose, "Commerce", nameof (PlatformSubscriptionService), string.Format("User {0} is member of {1} accounts", (object) memberId, (object) guidSet.Count));
        }
        else
        {
          guidSet = new HashSet<Guid>((IEnumerable<Guid>) extension.QueryAccountIds(vssRequestContext, memberId, UserType.Owner, includeDeletedAccounts: includeDisabledAccounts));
          vssRequestContext.Trace(5106148, TraceLevel.Verbose, "Commerce", nameof (PlatformSubscriptionService), string.Format("User {0} is owner of {1} accounts", (object) memberId, (object) guidSet.Count));
        }
        List<SubscriptionAccount> accountsInternal = new List<SubscriptionAccount>();
        if (guidSet.Any<Guid>())
          accountsInternal.AddRange(this.GetSubscriptionAccountsFromAccountIds(vssRequestContext, elevatedDeploymentContext, providerNamespace, (IEnumerable<Guid>) guidSet, (IEnumerable<Guid>) ownerAccountIds, memberId, serviceOwners, addCommerceDetails: addCommerceDetails).Where<SubscriptionAccount>((Func<SubscriptionAccount, bool>) (x => x != null)));
        return (IEnumerable<ISubscriptionAccount>) accountsInternal;
      }
      catch (Exception ex)
      {
        vssRequestContext.TraceException(5106149, "Commerce", nameof (PlatformSubscriptionService), ex);
        throw;
      }
      finally
      {
        vssRequestContext.TraceLeave(5106150, "Commerce", nameof (PlatformSubscriptionService), nameof (GetAccountsInternal));
      }
    }

    private SubscriptionAccount GetSubscriptionAccountDetails(
      IVssRequestContext elevatedContext,
      AzureSubscriptionAccount azureSubAccount,
      IEnumerable<Guid> ownerAccounts,
      Guid memberId,
      IEnumerable<Guid> serviceOwners)
    {
      SubscriptionAccount subAccount = (SubscriptionAccount) null;
      ActionPerformanceTracer performanceTracer = new ActionPerformanceTracer("Commerce", nameof (PlatformSubscriptionService));
      using (performanceTracer.Trace(elevatedContext, 5109240, nameof (GetSubscriptionAccountDetails)))
        CollectionHelper.WithCollectionContext(elevatedContext, azureSubAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
        {
          Microsoft.VisualStudio.Services.Organization.Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
          if (collection == null || collection.Owner == CommerceConstants.MsdnAdminGuid || collection.Status != CollectionStatus.Enabled)
            return;
          bool isAccountOwner = ownerAccounts == null || ownerAccounts.Contains<Guid>(azureSubAccount.AccountId);
          if (!isAccountOwner)
          {
            using (performanceTracer.Trace(collectionContext, 5109240, "IsPcaCheck"))
              isAccountOwner = CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionContext, memberId, 5109031, "Commerce", nameof (PlatformSubscriptionService));
          }
          subAccount = this.ConstructSubscriptionAccount(collectionContext, collection, azureSubAccount, isAccountOwner);
          subAccount.ServiceUrls = (IDictionary<Guid, Uri>) this.GetServiceOwnerUrls(collectionContext, (string) null, serviceOwners);
        }), method: nameof (GetSubscriptionAccountDetails));
      return subAccount;
    }

    private SubscriptionAccount GetSubscriptionAccountDetails(
      IVssRequestContext elevatedContext,
      AzureSubscriptionAccount azureSubAccount,
      IEnumerable<Guid> ownerAccounts,
      SubjectDescriptor descriptor,
      IEnumerable<Guid> serviceOwners)
    {
      SubscriptionAccount subAccount = (SubscriptionAccount) null;
      ActionPerformanceTracer performanceTracer = new ActionPerformanceTracer("Commerce", nameof (PlatformSubscriptionService));
      using (performanceTracer.Trace(elevatedContext, 5109240, nameof (GetSubscriptionAccountDetails)))
        CollectionHelper.WithCollectionContext(elevatedContext, azureSubAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
        {
          Microsoft.VisualStudio.Services.Organization.Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
          if (collection == null || collection.Owner == CommerceConstants.MsdnAdminGuid || collection.Status != CollectionStatus.Enabled)
            return;
          bool isAccountOwner = ownerAccounts == null || ownerAccounts.Contains<Guid>(azureSubAccount.AccountId);
          if (!isAccountOwner)
          {
            using (performanceTracer.Trace(collectionContext, 5109240, "IsPcaCheck"))
              isAccountOwner = CommerceDeploymentHelper.IsProjectCollectionAdmin(collectionContext, descriptor, 5109031, "Commerce", nameof (PlatformSubscriptionService));
          }
          subAccount = this.ConstructSubscriptionAccount(collectionContext, collection, azureSubAccount, isAccountOwner);
          subAccount.ServiceUrls = (IDictionary<Guid, Uri>) this.GetServiceOwnerUrls(collectionContext, (string) null, serviceOwners);
        }), method: nameof (GetSubscriptionAccountDetails));
      return subAccount;
    }

    private SubscriptionAccount GetSubscriptionAccountDetailsV2(
      IVssRequestContext deploymentContext,
      AzureSubscriptionAccount azureSubAccount,
      IEnumerable<Guid> ownerAccounts,
      Guid memberId,
      IEnumerable<Guid> serviceOwners)
    {
      SubscriptionAccount accountDetailsV2 = (SubscriptionAccount) null;
      using (new ActionPerformanceTracer("Commerce", nameof (PlatformSubscriptionService)).Trace(deploymentContext, 5109240, nameof (GetSubscriptionAccountDetailsV2)))
      {
        Guid accountId = azureSubAccount.AccountId;
        Microsoft.VisualStudio.Services.Organization.Collection collectionFromSps = CommerceDeploymentHelper.GetCollectionFromSps(deploymentContext, accountId);
        if (collectionFromSps == null || collectionFromSps.Owner == CommerceConstants.MsdnAdminGuid || collectionFromSps.Status != CollectionStatus.Enabled)
          return (SubscriptionAccount) null;
        bool isAccountOwner = ownerAccounts == null || ownerAccounts.Contains<Guid>(azureSubAccount.AccountId) || CommerceDeploymentHelper.IsProjectCollectionAdmin(deploymentContext, memberId, accountId);
        Guid tenantId = Guid.Empty;
        if (deploymentContext.ExecutionEnvironment.IsHostedDeployment)
          tenantId = CommerceDeploymentHelper.GetOrganizationFromSps(deploymentContext, accountId).TenantId;
        accountDetailsV2 = this.ConstructSubscriptionAccount(deploymentContext, collectionFromSps, azureSubAccount, isAccountOwner, tenantId);
        accountDetailsV2.ServiceUrls = (IDictionary<Guid, Uri>) this.GetServiceOwnerUrls(deploymentContext, (string) null, serviceOwners, accountId);
      }
      return accountDetailsV2;
    }

    private SubscriptionAccount GetSubscriptionAccountDetailsV2(
      IVssRequestContext deploymentContext,
      AzureSubscriptionAccount azureSubAccount,
      IEnumerable<Guid> ownerAccounts,
      SubjectDescriptor descriptor,
      IEnumerable<Guid> serviceOwners)
    {
      SubscriptionAccount accountDetailsV2 = (SubscriptionAccount) null;
      using (new ActionPerformanceTracer("Commerce", nameof (PlatformSubscriptionService)).Trace(deploymentContext, 5109233, nameof (GetSubscriptionAccountDetailsV2)))
      {
        Guid accountId = azureSubAccount.AccountId;
        Microsoft.VisualStudio.Services.Organization.Collection collectionFromSps = CommerceDeploymentHelper.GetCollectionFromSps(deploymentContext, accountId);
        if (collectionFromSps == null || collectionFromSps.Owner == CommerceConstants.MsdnAdminGuid || collectionFromSps.Status != CollectionStatus.Enabled)
          return (SubscriptionAccount) null;
        bool isAccountOwner = ownerAccounts == null || ownerAccounts.Contains<Guid>(azureSubAccount.AccountId) || CommerceDeploymentHelper.IsProjectCollectionAdmin(deploymentContext, descriptor, accountId);
        Guid tenantId = Guid.Empty;
        if (deploymentContext.ExecutionEnvironment.IsHostedDeployment)
          tenantId = CommerceDeploymentHelper.GetOrganizationFromSps(deploymentContext, accountId).TenantId;
        accountDetailsV2 = this.ConstructSubscriptionAccount(deploymentContext, collectionFromSps, azureSubAccount, isAccountOwner, tenantId);
        accountDetailsV2.ServiceUrls = (IDictionary<Guid, Uri>) this.GetServiceOwnerUrls(deploymentContext, (string) null, serviceOwners, accountId);
      }
      return accountDetailsV2;
    }

    internal virtual Dictionary<Guid, Uri> GetServiceOwnerUrls(
      IVssRequestContext collectionContext,
      string accountName,
      IEnumerable<Guid> serviceOwnerIds)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      return this.GetServiceOwnerUrls(collectionContext, accountName, serviceOwnerIds, collectionContext.ServiceHost.CollectionServiceHost.InstanceId);
    }

    internal virtual Dictionary<Guid, Uri> GetServiceOwnerUrls(
      IVssRequestContext requestContext,
      string accountName,
      IEnumerable<Guid> serviceOwnerIds,
      Guid collectionId)
    {
      Dictionary<Guid, Uri> serviceOwnerUrls = new Dictionary<Guid, Uri>();
      if (serviceOwnerIds == null)
        return serviceOwnerUrls;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      foreach (Guid serviceOwnerId in serviceOwnerIds)
      {
        try
        {
          IUrlHostResolutionService service = vssRequestContext.GetService<IUrlHostResolutionService>();
          serviceOwnerUrls[serviceOwnerId] = service.GetHostUri(vssRequestContext, collectionId, serviceOwnerId);
        }
        catch (Exception ex) when (ex is InvalidOperationException || ex is ServiceOwnerNotFoundException)
        {
          requestContext.TraceException(5106611, "Commerce", nameof (PlatformSubscriptionService), ex);
        }
      }
      return serviceOwnerUrls;
    }

    internal virtual SubscriptionAccount ConstructSubscriptionAccount(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Organization.Collection collection,
      AzureSubscriptionAccount azureSubscriptionAccount,
      bool isAccountOwner)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      Guid organizationAadTenantId = collectionContext.GetOrganizationAadTenantId();
      return this.ConstructSubscriptionAccount(collectionContext, collection, azureSubscriptionAccount, isAccountOwner, organizationAadTenantId);
    }

    internal virtual SubscriptionAccount ConstructSubscriptionAccount(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Organization.Collection collection,
      AzureSubscriptionAccount azureSubscriptionAccount,
      bool isAccountOwner,
      Guid tenantId)
    {
      requestContext.CheckHostedDeployment();
      if (string.IsNullOrWhiteSpace(azureSubscriptionAccount.GeoLocation))
      {
        string preferredRegion = collection?.PreferredRegion;
        if (preferredRegion != null)
        {
          ICommerceRegionHandler extension = requestContext.GetExtension<ICommerceRegionHandler>();
          azureSubscriptionAccount.GeoLocation = extension.GetRegionDisplayName(requestContext, preferredRegion);
        }
      }
      SubscriptionAccount subscriptionAccount = new SubscriptionAccount()
      {
        AccountId = azureSubscriptionAccount.AccountId,
        SubscriptionId = azureSubscriptionAccount.SubscriptionId,
        SubscriptionStatus = azureSubscriptionAccount.SubscriptionStatusId,
        AccountTenantId = tenantId,
        AccountName = collection?.Name,
        ResourceName = azureSubscriptionAccount.ResourceName,
        ResourceGroupName = azureSubscriptionAccount.ResourceGroupName,
        GeoLocation = azureSubscriptionAccount.GeoLocation,
        IsAccountOwner = isAccountOwner,
        OfferType = azureSubscriptionAccount.SubscriptionOfferType
      };
      requestContext.TraceProperties<SubscriptionAccount>(5108898, "Commerce", nameof (PlatformSubscriptionService), subscriptionAccount, (string) null);
      return subscriptionAccount;
    }

    internal virtual SubscriptionAccount ConstructSubscriptionAccountFromAzureRecource(
      IVssRequestContext collectionContext,
      Microsoft.VisualStudio.Services.Organization.Collection collection,
      AzureResourceAccount azureSubscriptionAccount,
      bool isAccountOwner)
    {
      collectionContext.CheckHostedDeployment();
      collectionContext.CheckProjectCollectionRequestContext();
      AzureSubscriptionInternal azureSubscription = this.GetAzureSubscription(collectionContext, azureSubscriptionAccount.AzureSubscriptionId);
      Guid organizationAadTenantId = collectionContext.GetOrganizationAadTenantId();
      if (string.IsNullOrWhiteSpace(azureSubscriptionAccount.AzureGeoRegion))
      {
        string preferredRegion = collection?.PreferredRegion;
        if (preferredRegion != null)
        {
          ICommerceRegionHandler extension = collectionContext.GetExtension<ICommerceRegionHandler>();
          azureSubscriptionAccount.AzureGeoRegion = extension.GetRegionDisplayName(collectionContext, preferredRegion);
        }
      }
      AzureQuotaId.Mapping mapping;
      bool flag = AzureQuotaId.TryMap(azureSubscription.AzureOfferCode, out mapping);
      SubscriptionAccount subscriptionAccount = new SubscriptionAccount()
      {
        AccountId = azureSubscriptionAccount.AccountId,
        SubscriptionId = new Guid?(azureSubscriptionAccount.AzureSubscriptionId),
        SubscriptionStatus = azureSubscription.AzureSubscriptionStatusId,
        AccountTenantId = organizationAadTenantId,
        AccountName = collection?.Name,
        ResourceName = azureSubscriptionAccount.AzureResourceName,
        ResourceGroupName = azureSubscriptionAccount.AzureCloudServiceName,
        GeoLocation = azureSubscriptionAccount.AzureGeoRegion,
        IsAccountOwner = isAccountOwner,
        OfferType = flag ? new AzureOfferType?(mapping.OfferType) : new AzureOfferType?()
      };
      collectionContext.TraceProperties<SubscriptionAccount>(5108898, "Commerce", nameof (PlatformSubscriptionService), subscriptionAccount, (string) null);
      return subscriptionAccount;
    }

    public virtual bool IsAccountLinked(IVssRequestContext requestContext, string accountName)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      requestContext1.TraceEnter(5105341, "Commerce", nameof (PlatformSubscriptionService), new object[1]
      {
        (object) accountName
      }, nameof (IsAccountLinked));
      try
      {
        this.CheckPermission(requestContext1, 1);
        IVssRequestContext vssRequestContext = requestContext1.Elevate();
        AzureResourceAccount azureResourceAccount = (AzureResourceAccount) null;
        Guid collectionId;
        if (HostNameResolver.TryGetCollectionServiceHostId(vssRequestContext, accountName, out collectionId))
          azureResourceAccount = this.GetAzureResourceAccountByCollectionId(vssRequestContext, collectionId);
        return azureResourceAccount != null && azureResourceAccount.OperationResult == OperationResult.Succeeded;
      }
      finally
      {
        requestContext1.TraceLeave(5105350, "Commerce", nameof (PlatformSubscriptionService), nameof (IsAccountLinked));
      }
    }

    public bool IsProjectCollectionAdmin(
      IVssRequestContext requestContext,
      Guid memberId,
      Guid collectionId)
    {
      return CommerceDeploymentHelper.IsProjectCollectionAdmin(requestContext, memberId, collectionId);
    }

    public void SetDateTimeProvider(IVssDateTimeProvider dateTimeProvider) => this.dateTimeProvider = dateTimeProvider;

    private bool HasSpecialPurchasePermission(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IPermissionCheckerService>().CheckPermission(vssRequestContext, 2, CommerceSecurity.CommerceSecurityNamespaceId, "/PartnerPurchase", false);
    }

    internal virtual void InvalidateSubscriptionServiceCache(
      IVssRequestContext requestContext,
      string subscriptionId,
      AccountProviderNamespace? providerNamespaceId,
      Guid? linkedHostId)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      ICommerceCache service = context.GetService<ICommerceCache>();
      service.Invalidate<AzureSubscriptionInternal>(context, PlatformSubscriptionService.GetAzureSubscriptionCacheKey(subscriptionId));
      IEnumerable<AzureResourceAccount> source;
      if (providerNamespaceId.HasValue && service.TryGet<IEnumerable<AzureResourceAccount>>(context, PlatformSubscriptionService.GetAzureResourceAccountCacheKey(subscriptionId, providerNamespaceId.Value.ToString()), out source) || service.TryGet<IEnumerable<AzureResourceAccount>>(context, PlatformSubscriptionService.GetAzureResourceAccountCacheKey(subscriptionId), out source))
      {
        foreach (string key in source != null ? source.Select<AzureResourceAccount, string>((Func<AzureResourceAccount, string>) (azureResource => !(azureResource.CollectionId != new Guid()) ? azureResource.AccountId.ToString() : azureResource.CollectionId.ToString())) : (IEnumerable<string>) null)
          service.Invalidate<AzureResourceAccount>(context, key);
      }
      string key1 = providerNamespaceId.HasValue ? PlatformSubscriptionService.GetAzureResourceAccountCacheKey(subscriptionId, providerNamespaceId.ToString()) : PlatformSubscriptionService.GetAzureResourceAccountCacheKey(subscriptionId);
      service.Invalidate<IEnumerable<AzureResourceAccount>>(context, key1);
      if (!linkedHostId.HasValue)
        return;
      service.Invalidate<AzureResourceAccount>(context, linkedHostId.ToString());
    }

    internal virtual void ResetCurrentQuantityUponUnlink(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      CollectionHelper.WithCollectionContext(requestContext, hostId, (Action<IVssRequestContext>) (collectionContext => collectionContext.GetService<PlatformOfferSubscriptionService>().ResetResourceUsage(collectionContext, false, true)), method: nameof (ResetCurrentQuantityUponUnlink));
    }

    internal virtual void TraceBIEventForAzureSubscriptionChanges(
      IVssRequestContext requestContext,
      Guid hostId,
      AzureResourceAccount resourceAccount,
      AzureSubscriptionInternal azureSubscription,
      bool isUnlink)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (azureSubscription == null && resourceAccount != null)
      {
        azureSubscription = this.GetAzureSubscription(vssRequestContext.Elevate(), resourceAccount.AzureSubscriptionId);
        hostId = resourceAccount.CollectionId;
      }
      if (azureSubscription == null)
        return;
      HostProperties hostProperties = vssRequestContext.GetService<CommerceHostManagementService>().QueryServiceHostPropertiesCached(vssRequestContext, hostId);
      TeamFoundationTracingService.TraceCommerceAzureSubscription(isUnlink ? Guid.Empty : azureSubscription.AzureSubscriptionId, azureSubscription.Created, azureSubscription.AzureSubscriptionSource.ToString(), azureSubscription.AzureSubscriptionStatusId.ToString(), hostId, hostProperties?.ParentId.GetValueOrDefault(), TeamFoundationHostType.ProjectCollection, CommerceUtil.CheckForRequestSource(requestContext), azureSubscription.AzureOfferCode ?? string.Empty, this.GetUtcNow());
    }

    internal IEnumerable<Guid> GetIdentityIdsWithIdenticalUpn(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string property = authenticatedIdentity.GetProperty<string>("Account", string.Empty);
      return !string.IsNullOrWhiteSpace(property) ? vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, IdentitySearchFilter.AccountName, property, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsBindPending)).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => !identity.IsCspPartnerUser)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (identity => !requestContext.IsCommerceService() ? identity.MasterId : identity.Id)) : Enumerable.Empty<Guid>();
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.dateTimeProvider.UtcNow;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetCacheKey(string keyString1, string keyString2) => string.Format("{0}/{1}", (object) keyString1, (object) keyString2).ToLower();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetAzureResourceAccountCacheKey(
      string subscriptionId,
      string providerNamespace)
    {
      return string.Format("AzureResourceAccount-v2/{0}/{1}", (object) subscriptionId, (object) providerNamespace).ToLower();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetAzureResourceAccountCacheKey(string subscriptionId) => string.Format("AzureResourceAccount-v3/{0}", (object) subscriptionId).ToLower();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetAzureSubscriptionCacheKey(string keyString1, string keyString2) => string.Format("AzureSubscription-v2/{0}/{1}", (object) keyString1, (object) keyString2).ToLower();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetAzureSubscriptionCacheKey(string keyString) => string.Format("AzureSubscription-v3/{0}", (object) keyString).ToLower();

    private void TraceBIEventOnAzureSubscriptionDeactivation(
      IVssRequestContext collectionContext,
      AzureResourceAccount resourceAccount,
      IEnumerable<IOfferSubscription> existingOfferSubscriptions,
      bool unlinkEvent)
    {
      collectionContext.CheckProjectCollectionRequestContext();
      existingOfferSubscriptions = (IEnumerable<IOfferSubscription>) existingOfferSubscriptions.Where<IOfferSubscription>((Func<IOfferSubscription, bool>) (x => x.CommittedQuantity > x.IncludedQuantity)).ToList<IOfferSubscription>();
      if (!existingOfferSubscriptions.Any<IOfferSubscription>())
        return;
      Guid userCUID;
      Guid eventUserId = CommerceIdentityHelper.UserIdentityIdOrCommerceContext(collectionContext, out userCUID);
      string source;
      CustomerIntelligenceEventType intelligenceEventType;
      if (unlinkEvent)
      {
        source = "SubscriptionUnlink";
        intelligenceEventType = CustomerIntelligenceEventType.CancelAllUnlinkedAzureSub;
      }
      else if (resourceAccount != null && resourceAccount.ProviderNamespaceId == AccountProviderNamespace.OnPremise)
      {
        source = "TeamFoundationServerSubscriptionDisabled";
        intelligenceEventType = CustomerIntelligenceEventType.CancelAllDisabledAzureSub;
      }
      else
      {
        source = "SubscriptionDisabled";
        intelligenceEventType = CustomerIntelligenceEventType.CancelAllDisabledAzureSub;
      }
      foreach (IOfferSubscription offerSubscription in existingOfferSubscriptions)
      {
        DateTime utcNow = this.GetUtcNow();
        TeamFoundationTracingService.TraceCommerceMeteredResource(collectionContext.ServiceHost.InstanceId, collectionContext.ServiceHost.ParentServiceHost.InstanceId, TeamFoundationHostType.ProjectCollection, eventUserId, userCUID, offerSubscription.OfferMeter.Name, offerSubscription.OfferMeter.PlatformMeterId, offerSubscription.RenewalGroup.ToString(), offerSubscription.IncludedQuantity, offerSubscription.CommittedQuantity, new int?(offerSubscription.CommittedQuantity), offerSubscription.IncludedQuantity, offerSubscription.MaximumQuantity, 0, offerSubscription.IsPaidBillingEnabled, string.Format("{0}{1}", (object) utcNow.Year, (object) utcNow.Month.ToString("00")), 0.0, offerSubscription.CommittedQuantity, source, resourceAccount != null ? resourceAccount.AzureSubscriptionId : Guid.Empty, offerSubscription.OfferMeter.Category.ToString(), utcNow, intelligenceEventType.ToString());
      }
    }

    private void SetIncludedQuantityForAssignmentBilling(
      IVssRequestContext collectionContext,
      IVssRequestContext deploymentContext,
      AzureResourceAccount azureResourceAccount)
    {
      int licensingMaxQuantity = AssignmentBillingHelper.GetLicensingMaxQuantity(deploymentContext);
      collectionContext.TraceAlways(5109244, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Set basic included and max quantities to {0} for assignment billing, subscriptionId: {1}, collectionId: {2}", (object) licensingMaxQuantity, (object) azureResourceAccount.AzureSubscriptionId, (object) collectionContext.ServiceHost.InstanceId));
      IOfferSubscriptionService service = collectionContext.GetService<IOfferSubscriptionService>();
      service.SetAccountQuantity(collectionContext, "StandardLicense", new int?(licensingMaxQuantity), new int?(licensingMaxQuantity));
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableAssignmentBillingExtensions"))
        return;
      collectionContext.TraceAlways(5109244, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Set Test Manager and Package Management included and max quantities to {0} for assignment billing, subscriptionId: {1}, collectionId: {2}", (object) licensingMaxQuantity, (object) azureResourceAccount.AzureSubscriptionId, (object) collectionContext.ServiceHost.InstanceId));
      service.SetAccountQuantity(collectionContext, "Test Manager", new int?(licensingMaxQuantity), new int?(licensingMaxQuantity));
      service.SetAccountQuantity(collectionContext, "Package Management", new int?(licensingMaxQuantity), new int?(licensingMaxQuantity));
    }

    private void ResetIncludedQuantityForAssignmentBilling(
      IVssRequestContext collectionContext,
      IVssRequestContext deploymentContext,
      AzureResourceAccount azureResourceAccount)
    {
      collectionContext.TraceAlways(5109245, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Reset basic included quantity upon unlink for assignment billing, subscriptionId: {0}, collectionId: {1}", (object) azureResourceAccount.AzureSubscriptionId, (object) collectionContext.ServiceHost.InstanceId));
      IOfferSubscriptionService service1 = collectionContext.GetService<IOfferSubscriptionService>();
      IOfferMeterService service2 = deploymentContext.GetService<IOfferMeterService>();
      service1.SetAccountQuantity(collectionContext, "StandardLicense", new int?(service2.GetOfferMeter(deploymentContext, "ms.vss-vstsuser").IncludedQuantity), new int?());
      if (!collectionContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableAssignmentBillingExtensions"))
        return;
      collectionContext.TraceAlways(5109245, TraceLevel.Info, "Commerce", nameof (PlatformSubscriptionService), string.Format("Reset Test Manager and Packet Management included quantity for assignment billing, subscriptionId: {0}, collectionId: {1}", (object) azureResourceAccount.AzureSubscriptionId, (object) collectionContext.ServiceHost.InstanceId));
      service1.SetAccountQuantity(collectionContext, "Test Manager", new int?(service2.GetOfferMeter(deploymentContext, "ms.vss-testmanager-web").IncludedQuantity), new int?());
      service1.SetAccountQuantity(collectionContext, "Package Management", new int?(service2.GetOfferMeter(deploymentContext, "ms.feed").IncludedQuantity), new int?());
    }

    public delegate bool CollectionNameResolver(
      IVssRequestContext requestContext,
      string collectionName,
      out Guid collectionId);
  }
}
