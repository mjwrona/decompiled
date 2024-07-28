// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.FrameworkLicensingEntitlementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Licensing;
using Microsoft.TeamFoundation.Framework.Server.Licensing.Utilities;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkLicensingEntitlementService : 
    ILicensingEntitlementService,
    IVssFrameworkService
  {
    private ILicenseClaimCacheService m_cacheService;
    private ILicensesUsageCacheService m_usageCacheService;
    private Guid m_serviceHostId;
    private const string s_area = "VisualStudio.Services.FrameworkAccountEntitlementService";
    private const string s_layer = "BusinessLogic";
    private const string s_fallbackAccountRightsReason = "We are temporarily having trouble validating your account rights, so we've granted you the highest account rights.";
    private const string s_LicensesUsageCacheFeatureName = "VisualStudio.Services.LicensesUsage.L1Cache";
    private const string s_DoNotTracePublicLicensingRequests = "VisualStudio.Services.Licensing.NotTracePublicLicensingRequests";
    private const string s_CacheFailedLicenseQueriesInRequestContext = "VisualStudio.Services.Licensing.CacheFailedLicenseQueriesInRequestContext";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckHostedDeployment();
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      this.ValidateNonDeploymentContext(systemRequestContext);
      this.m_cacheService = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ILicenseClaimCacheService>();
      this.m_usageCacheService = systemRequestContext.GetService<ILicensesUsageCacheService>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<AccountEntitlement> GetAccountEntitlements(IVssRequestContext requestContext)
    {
      this.ValidateCollectionContext(requestContext);
      return this.GetAccountEntitlementsInternal(requestContext);
    }

    public IList<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      int top,
      int skip)
    {
      this.ValidateCollectionContext(requestContext);
      return this.GetAccountEntitlementsInternal(requestContext, new int?(top), skip);
    }

    public IList<AccountEntitlement> GetAccountEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds,
      bool bypassCache = false)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
      requestContext.TraceEnter(1038051, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetAccountEntitlements));
      List<AccountEntitlement> accountEntitlements = new List<AccountEntitlement>(userIds.Count);
      if (!bypassCache)
      {
        foreach (Guid userId in userIds.ToList<Guid>())
        {
          AccountEntitlement entitlement;
          if (this.TryGetEntitlementFromCache(requestContext, userId, out entitlement))
          {
            accountEntitlements.Add(entitlement);
            userIds.Remove(userId);
          }
        }
      }
      try
      {
        if (userIds.Any<Guid>())
        {
          IAccountLicensingHttpClient httpClient = this.GetHttpClient(requestContext.Elevate());
          accountEntitlements.AddRange(httpClient.GetAccountEntitlementsAsync(userIds).SyncResult<IEnumerable<AccountEntitlement>>());
        }
        return (IList<AccountEntitlement>) accountEntitlements;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038052, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038053, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetAccountEntitlements));
      }
    }

    public PagedAccountEntitlements SearchAccountEntitlements(
      IVssRequestContext requestContext,
      string continuation = null,
      string filter = null,
      string orderBy = null,
      bool searchUsersOnly = false)
    {
      return requestContext.TraceBlock<PagedAccountEntitlements>(1039161, 1039162, 1039163, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", (Func<PagedAccountEntitlements>) (() => this.GetHttpClient(requestContext).SearchAccountEntitlementsAsync(continuation, filter, orderBy).SyncResult<PagedAccountEntitlements>()), nameof (SearchAccountEntitlements));
    }

    public PagedAccountEntitlements SearchMemberAccountEntitlements(
      IVssRequestContext requestContext,
      string continuation = null,
      string filter = null,
      string orderBy = null)
    {
      return requestContext.TraceBlock<PagedAccountEntitlements>(1039161, 1039162, 1039163, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", (Func<PagedAccountEntitlements>) (() => this.GetHttpClient(requestContext).SearchMemberAccountEntitlementsAsync(continuation, filter, orderBy).SyncResult<PagedAccountEntitlements>()), nameof (SearchMemberAccountEntitlements));
    }

    public IList<AccountEntitlement> ObtainAvailableAccountEntitlements(
      IVssRequestContext requestContext,
      IList<Guid> userIds)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) userIds, nameof (userIds));
      requestContext.TraceEnter(1038054, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (ObtainAvailableAccountEntitlements));
      try
      {
        return this.GetHttpClient(requestContext.Elevate()).ObtainAvailableAccountEntitlementsAsync(userIds).SyncResult<IList<AccountEntitlement>>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038055, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038056, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (ObtainAvailableAccountEntitlements));
      }
    }

    public AccountEntitlement GetAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      bool bypassCache = false,
      bool determineRights = true,
      bool createIfNotExists = true)
    {
      this.ValidateNonDeploymentContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      requestContext.TraceEnter(1038090, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetAccountEntitlement));
      try
      {
        if (requestContext.IsAnonymousPrincipal() || requestContext.IsPublicUser() || requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
        {
          string message = "Returning max rights for public and anonymous requests";
          if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.NotTracePublicLicensingRequests"))
            message = EnvironmentWrapper.ToReadableStackTrace().ToString();
          requestContext.Trace(1038091, TraceLevel.Error, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", message);
          return new AccountEntitlement()
          {
            UserId = userId,
            License = License.Auto,
            Rights = new AccountRights(VisualStudioOnlineServiceLevel.AdvancedPlus)
          };
        }
        AccountEntitlement entitlement;
        return !bypassCache && this.TryGetEntitlementFromCache(requestContext, userId, out entitlement) ? entitlement : this.FetchAccountEntitlementWithFallback(requestContext, userId, determineRights, createIfNotExists);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038098, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038099, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetAccountEntitlement));
      }
    }

    public AccountEntitlement AssignAvailableAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingOrigin origin = LicensingOrigin.None)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      requestContext.TraceEnter(1038060, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignAvailableAccountEntitlement));
      try
      {
        AccountEntitlement accountEntitlement = this.GetHttpClient(requestContext.Elevate()).AssignAvailableEntitlementAsync(userId, origin: origin).SyncResult<AccountEntitlement>();
        if (accountEntitlement.Rights != null)
        {
          requestContext.SetAccountEntitlement(userId, accountEntitlement);
          this.CacheEntitlement(requestContext, userId, new AccountEntitlementClaim(accountEntitlement, DateTimeOffset.UtcNow));
        }
        return accountEntitlement;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038068, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038069, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignAvailableAccountEntitlement));
      }
    }

    public AccountEntitlement AssignAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin = LicensingOrigin.None)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      requestContext.TraceEnter(1038070, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignAccountEntitlement));
      try
      {
        AccountEntitlement accountEntitlement = this.GetHttpClient(requestContext.Elevate()).AssignEntitlementAsync(userId, license, origin: origin).SyncResult<AccountEntitlement>();
        if (accountEntitlement.Rights != null)
        {
          requestContext.SetAccountEntitlement(userId, accountEntitlement);
          this.CacheEntitlement(requestContext, userId, new AccountEntitlementClaim(accountEntitlement, DateTimeOffset.UtcNow));
        }
        return accountEntitlement;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038078, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038079, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (AssignAccountEntitlement));
      }
    }

    public void DeleteAccountEntitlement(IVssRequestContext requestContext, Guid userId)
    {
      this.ValidateCollectionContext(requestContext);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      requestContext.TraceEnter(1038080, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (DeleteAccountEntitlement));
      try
      {
        this.GetHttpClient(requestContext.Elevate()).DeleteEntitlementAsync(userId).SyncResult();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038088, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038089, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (DeleteAccountEntitlement));
      }
    }

    public IList<AccountLicenseUsage> GetLicensesUsage(
      IVssRequestContext requestContext,
      bool bypassCache = false)
    {
      this.ValidateCollectionContext(requestContext);
      requestContext.TraceEnter(1038040, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetLicensesUsage));
      try
      {
        List<AccountLicenseUsage> licensesUsage;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.LicensesUsage.L1Cache") && !bypassCache && this.m_usageCacheService.TryGetValue(requestContext, requestContext.ServiceHost.InstanceId.ToString(), out licensesUsage))
          return (IList<AccountLicenseUsage>) licensesUsage;
        List<AccountLicenseUsage> list = this.GetHttpClient(requestContext).GetAccountLicensesUsageAsync().SyncResult<IEnumerable<AccountLicenseUsage>>().ToList<AccountLicenseUsage>();
        this.m_usageCacheService.Set(requestContext, requestContext.ServiceHost.InstanceId.ToString(), list);
        return (IList<AccountLicenseUsage>) list;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038048, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038049, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetLicensesUsage));
      }
    }

    private protected virtual IAccountLicensingHttpClient GetHttpClient(
      IVssRequestContext requestContext)
    {
      return requestContext.GetAccountLicensingHttpClient();
    }

    private bool TryGetEntitlementFromCache(
      IVssRequestContext requestContext,
      Guid userId,
      out AccountEntitlement entitlement)
    {
      if (requestContext.TryGetAccountEntitlement(userId, out entitlement))
        return true;
      string entitlementCacheKey = this.GetEntitlementCacheKey(requestContext, userId);
      ILicenseClaim licenseClaim;
      if (!this.m_cacheService.TryGetValue(requestContext.To(TeamFoundationHostType.Deployment), entitlementCacheKey, out licenseClaim) || !(licenseClaim is AccountEntitlementClaim))
        return false;
      AccountEntitlement accountEntitlement = (licenseClaim as AccountEntitlementClaim).AccountEntitlement;
      if (accountEntitlement.Rights == null)
        return false;
      requestContext.SetAccountEntitlement(userId, accountEntitlement);
      entitlement = accountEntitlement;
      return true;
    }

    private IList<AccountEntitlement> GetAccountEntitlementsInternal(
      IVssRequestContext requestContext,
      int? top = null,
      int skip = 0)
    {
      requestContext.TraceEnter(1038061, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetAccountEntitlementsInternal));
      try
      {
        IAccountLicensingHttpClient httpClient = this.GetHttpClient(requestContext.Elevate());
        return (IList<AccountEntitlement>) new List<AccountEntitlement>(top.HasValue ? httpClient.GetAccountEntitlementsAsync(top.Value, skip).SyncResult<IEnumerable<AccountEntitlement>>() : httpClient.GetAccountEntitlementsAsync().SyncResult<IEnumerable<AccountEntitlement>>());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1038062, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1038063, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", nameof (GetAccountEntitlementsInternal));
      }
    }

    private string GetEntitlementCacheKey(IVssRequestContext requestContext, Guid userId)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      return Constants.LicenseClaimConstants.AccountEntitlementClaimTypePrefix + (object) instanceId + (object) Constants.LicenseClaimConstants.KeyPathSeparator + (object) userId;
    }

    internal virtual void CacheEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      AccountEntitlementClaim rights)
    {
      string entitlementCacheKey = this.GetEntitlementCacheKey(requestContext, userId);
      this.m_cacheService.Set(requestContext.To(TeamFoundationHostType.Deployment), entitlementCacheKey, (ILicenseClaim) rights);
    }

    private AccountEntitlement FetchAccountEntitlementWithFallback(
      IVssRequestContext requestContext,
      Guid userId,
      bool determineRights,
      bool createIfNotExists)
    {
      bool cacheReturnValue = true;
      AccountEntitlement accountEntitlement;
      try
      {
        IVssRequestContext elevatedRequestContext = requestContext.Elevate();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = elevatedRequestContext.GetService<IdentityService>().ReadIdentities(elevatedRequestContext, (IList<Guid>) new Guid[1]
        {
          userId
        }, QueryMembership.None, (IEnumerable<string>) null);
        if (source.Any<Microsoft.VisualStudio.Services.Identity.Identity>() && source[0] != null)
        {
          if (source[0].IsContainer)
          {
            requestContext.Trace(1038093, TraceLevel.Warning, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", string.Format("Returning null entitlements for userId: {0}, since {1} was a group identity at servicehost {2} ", (object) userId, (object) source[0].DisplayName, (object) requestContext.ServiceHost.InstanceId));
            return (AccountEntitlement) null;
          }
          if (IdentityHelper.IsServiceIdentity(elevatedRequestContext, (IReadOnlyVssIdentity) source[0]))
          {
            requestContext.Trace(1038096, TraceLevel.Warning, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", string.Format("Returning null entitlements for userId: {0}, since {1} was a service identity for servicehost {2} ", (object) userId, (object) source[0].DisplayName, (object) requestContext.ServiceHost.InstanceId));
            return (AccountEntitlement) null;
          }
          if (IdentityHelper.IsServiceIdentity(elevatedRequestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) source[0]))
          {
            requestContext.Trace(1038097, TraceLevel.Warning, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", string.Format("Returning null entitlements for userId: {0}, since {1} was a service identity at deployment for servicehost {2} ", (object) userId, (object) source[0].DisplayName, (object) requestContext.ServiceHost.InstanceId));
            return (AccountEntitlement) null;
          }
        }
        accountEntitlement = this.InvokeCommandWithCircuitBreaker<AccountEntitlement>(elevatedRequestContext, (Func<AccountEntitlement>) (() => this.GetHttpClient(elevatedRequestContext).GetAccountEntitlementAsync(userId, determineRights, createIfNotExists).SyncResult<AccountEntitlement>()), (Func<AccountEntitlement>) (() =>
        {
          cacheReturnValue = false;
          return new AccountEntitlement()
          {
            UserId = userId,
            License = License.Auto,
            Rights = new AccountRights(VisualStudioOnlineServiceLevel.AdvancedPlus, "We are temporarily having trouble validating your account rights, so we've granted you the highest account rights.")
          };
        }), typeof (VssServiceResponseException));
        if (cacheReturnValue)
          requestContext.Trace(1038094, TraceLevel.Info, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", string.Format("Retrieved account entitlement with rights for userId: {0}, Rights: {1}, Reason: {2}", (object) userId, (object) accountEntitlement?.Rights?.Level, (object) accountEntitlement?.Rights?.Reason));
        else
          requestContext.Trace(10380945, TraceLevel.Warning, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", string.Format("Failed to retrieve account entitlements with rights for userId: {0} due to circuit breaker, Returning Max Rights: {1}, Reason: {2}", (object) userId, (object) accountEntitlement?.Rights?.Level, (object) accountEntitlement?.Rights?.Reason));
      }
      catch (VssServiceResponseException ex)
      {
        requestContext.TraceException(1038094, TraceLevel.Error, "VisualStudio.Services.FrameworkAccountEntitlementService", "BusinessLogic", (Exception) ex);
        cacheReturnValue = false;
        accountEntitlement = new AccountEntitlement()
        {
          UserId = userId,
          License = License.Auto,
          Rights = new AccountRights(VisualStudioOnlineServiceLevel.AdvancedPlus, "We are temporarily having trouble validating your account rights, so we've granted you the highest account rights.")
        };
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Licensing.CacheFailedLicenseQueriesInRequestContext"))
          return accountEntitlement;
      }
      if (accountEntitlement != (AccountEntitlement) null)
      {
        requestContext.SetAccountEntitlement(userId, accountEntitlement);
        if (cacheReturnValue)
        {
          AccountEntitlementClaim rights = new AccountEntitlementClaim(accountEntitlement, DateTimeOffset.UtcNow);
          this.CacheEntitlement(requestContext, userId, rights);
        }
      }
      return accountEntitlement;
    }

    private void ValidateCollectionContext(IVssRequestContext context)
    {
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
      context.CheckProjectCollectionRequestContext();
    }

    private void ValidateNonDeploymentContext(IVssRequestContext context)
    {
      context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
      if (context.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(context.ServiceHost.HostType);
    }

    internal virtual T InvokeCommandWithCircuitBreaker<T>(
      IVssRequestContext requestContext,
      Func<T> run,
      Func<T> fallback,
      params Type[] expectedExceptions)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Compliance.Framework").AndCommandKey((CommandKey) "ValidateAccountRights").AndCommandPropertiesDefaults(FrameworkLicensingEntitlementService.CircuitBreakerSettings.CommandPropertiesForOutBoundCallToAuthority);
      Func<T> run1 = run;
      if (expectedExceptions != null && expectedExceptions.Length != 0)
        run1 = (Func<T>) (() =>
        {
          try
          {
            return run();
          }
          catch (Exception ex)
          {
            if (((IEnumerable<Type>) expectedExceptions).Contains<Type>(ex.GetType()))
              ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
            throw;
          }
        });
      return new CommandService<T>(requestContext, setter, run1, fallback).Execute();
    }

    private static class CircuitBreakerSettings
    {
      internal const string CommandGroupKey = "Compliance.Framework";
      internal const string CommandKeyForValidateAccountRights = "ValidateAccountRights";
      internal static readonly CommandPropertiesSetter CommandPropertiesForOutBoundCallToAuthority = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(20).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(10.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(60000).WithMetricsRollingStatisticalWindowBuckets(60);
    }
  }
}
