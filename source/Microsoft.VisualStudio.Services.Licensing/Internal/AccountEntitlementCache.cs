// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.AccountEntitlementCache
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class AccountEntitlementCache : IAccountEntitlementCache, IVssFrameworkService
  {
    private readonly ILicensingConfigurationManager m_settingsManager;
    protected AccountEntitlementCacheConfiguration m_serviceSettings;
    private const string s_area = "VisualStudio.Services.LicensingService.AccountEntitlementCacheService";
    private const string s_layer = "BusinessLogic";
    private readonly Guid s_RedisNamespaceId = new Guid("8CE841D4-526D-438A-BEE7-C4681D75E984");

    public AccountEntitlementCache()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager())
    {
    }

    internal AccountEntitlementCache(ILicensingConfigurationManager settingsManager) => this.m_settingsManager = settingsManager;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<CachedRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Licensing/AccountEntitlement/...");
      this.PopulateSettings(vssRequestContext);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
        return;
      this.PopulateSettings(requestContext);
    }

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_serviceSettings = this.m_settingsManager.GetAccountEntitlementCacheConfiguration(requestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<CachedRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    public virtual IEnumerable<License> GetAccountEntitlements(
      IVssRequestContext requestContext,
      IEnumerable<Guid> accountIds,
      Guid userId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(accountIds, nameof (accountIds));
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = AccountEntitlementCache.GetIdentity(requestContext, userId);
      return (IEnumerable<License>) accountIds.Select<Guid, License>((Func<Guid, License>) (accountId => this.GetAccountEntitlement(requestContext, accountId, userIdentity))).Where<License>((Func<License, bool>) (entitledLicense => entitledLicense != (License) null)).ToList<License>();
    }

    public virtual License GetAccountEntitlement(
      IVssRequestContext requestContext,
      Guid accountId,
      Guid userId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      Microsoft.VisualStudio.Services.Identity.Identity identity = AccountEntitlementCache.GetIdentity(requestContext, userId);
      return this.GetAccountEntitlement(requestContext, accountId, identity);
    }

    public virtual void InvalidateAccountEntitlement(
      IVssRequestContext requestContext,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      this.RemoveValueFromCache(requestContext, identity.MasterId, out AccountEntitlementToken _);
    }

    public void UpdateAccountEntitlement(
      IVssRequestContext requestContext,
      Guid accountId,
      Guid userId,
      License accountLicense)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      Microsoft.VisualStudio.Services.Identity.Identity identity = AccountEntitlementCache.GetIdentity(requestContext, userId);
      this.UpdateIdentityCache(requestContext, accountId, identity, accountLicense);
    }

    public void UpdateAccountEntitlement(
      IVssRequestContext requestContext,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      License accountLicense)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      this.UpdateIdentityCache(requestContext, accountId, identity, accountLicense);
    }

    private IMutableDictionaryCacheContainer<Guid, AccountEntitlementToken> GetDistributedCache(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(1034020, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", nameof (GetDistributedCache));
      try
      {
        IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
        if (service.IsEnabled(requestContext))
          return service.GetVolatileDictionaryContainer<Guid, AccountEntitlementToken, AccountEntitlementCache.AccountEntitlementCacheToken>(requestContext, this.s_RedisNamespaceId, new ContainerSettings()
          {
            KeyExpiry = new TimeSpan?(this.m_serviceSettings.TokenTimeToLive),
            CiAreaName = typeof (AccountEntitlementCache).Name
          });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034029, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", ex);
      }
      requestContext.TraceLeave(1034030, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", nameof (GetDistributedCache));
      return (IMutableDictionaryCacheContainer<Guid, AccountEntitlementToken>) null;
    }

    internal virtual AccountEntitlementToken GetValueFromCache(
      IVssRequestContext requestContext,
      Guid identityId)
    {
      requestContext.TraceEnter(1034031, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", nameof (GetValueFromCache));
      AccountEntitlementToken valueFromCache = (AccountEntitlementToken) null;
      try
      {
        IMutableDictionaryCacheContainer<Guid, AccountEntitlementToken> distributedCache = this.GetDistributedCache(requestContext);
        if (distributedCache != null)
          distributedCache.TryGet<Guid, AccountEntitlementToken>(requestContext, identityId, out valueFromCache);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034039, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", ex);
      }
      requestContext.TraceLeave(1034040, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", nameof (GetValueFromCache));
      return valueFromCache;
    }

    internal virtual void SetValueToCache(
      IVssRequestContext requestContext,
      Guid identityId,
      AccountEntitlementToken entitlementToken)
    {
      requestContext.TraceEnter(1034041, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", nameof (SetValueToCache));
      try
      {
        IMutableDictionaryCacheContainer<Guid, AccountEntitlementToken> distributedCache = this.GetDistributedCache(requestContext);
        if (distributedCache != null)
        {
          if (entitlementToken != null)
            distributedCache.Set(requestContext, (IDictionary<Guid, AccountEntitlementToken>) new Dictionary<Guid, AccountEntitlementToken>()
            {
              {
                identityId,
                entitlementToken
              }
            });
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034049, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", ex);
      }
      requestContext.TraceEnter(1034050, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", nameof (SetValueToCache));
    }

    internal bool RemoveValueFromCache(
      IVssRequestContext requestContext,
      Guid identityId,
      out AccountEntitlementToken entitlementToken)
    {
      entitlementToken = (AccountEntitlementToken) null;
      try
      {
        IMutableDictionaryCacheContainer<Guid, AccountEntitlementToken> distributedCache = this.GetDistributedCache(requestContext);
        if (distributedCache != null)
          distributedCache.TryGet<Guid, AccountEntitlementToken>(requestContext, identityId, out entitlementToken);
        if (entitlementToken != null)
          distributedCache.Invalidate<Guid, AccountEntitlementToken>(requestContext, identityId);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033838, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", ex);
      }
      return false;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      Guid userId)
    {
      return requestContext.GetExtension<IReadIdentitiesByMasterIdExtension>(ExtensionLifetime.Service).ReadIdentitiesByMasterId(requestContext, (IList<Guid>) new Guid[1]
      {
        userId
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() ?? throw new IdentityNotFoundException(userId);
    }

    internal virtual License GetAccountEntitlement(
      IVssRequestContext requestContext,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      try
      {
        License accountEntitlement1;
        if (this.TryGetEntitlementFromUserIdentity(requestContext, userIdentity, accountId, out accountEntitlement1))
          return accountEntitlement1;
        using (VssRequestContextLicensingExtensions.VssRequestContextHolder collection = requestContext.ToCollection(accountId))
        {
          requestContext.Trace(1034005, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", "Starting account host for account {0} to get account license for user {1}", (object) accountId, (object) userIdentity.MasterId);
          IVssRequestContext requestContext1 = collection.RequestContext;
          AccountEntitlement accountEntitlement2 = requestContext1.GetService<ILicensingEntitlementService>().GetAccountEntitlement(requestContext1, userIdentity.MasterId, true, false);
          if (accountEntitlement2 != (AccountEntitlement) null)
          {
            try
            {
              requestContext.Trace(1034007, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", "Found account license {0} for user {1} in account {2}", (object) accountEntitlement2.License.ToString(), (object) userIdentity.MasterId, (object) accountId);
              this.UpdateIdentityCache(requestContext, accountId, userIdentity, accountEntitlement2.License);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1034005, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", ex);
            }
            return accountEntitlement2.License;
          }
          requestContext.Trace(1034006, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", "Did not find an account license for user {0} in account {1}", (object) userIdentity.MasterId, (object) accountId);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1034008, TraceLevel.Warning, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", string.Format("Failed to retrieve user's account entitlement for UserId: {0}, AccountId: {1}.", (object) userIdentity.MasterId, (object) accountId));
        requestContext.TraceException(1034009, TraceLevel.Warning, "VisualStudio.Services.LicensingService.AccountEntitlementCacheService", "BusinessLogic", ex);
      }
      return (License) null;
    }

    internal virtual void UpdateIdentityCache(
      IVssRequestContext requestContext,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      License accountLicense)
    {
      AccountEntitlementToken tokenToWrite = this.CreateTokenToWrite(requestContext, userIdentity, accountId, accountLicense);
      this.SetValueToCache(requestContext, userIdentity.MasterId, tokenToWrite);
    }

    private AccountEntitlementToken CreateTokenToWrite(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      Guid accountId,
      License accountLicense)
    {
      DateTime utcNow = DateTime.UtcNow;
      AccountEntitlementToken existingToken;
      if (!this.TryGetValidExistingToken(requestContext, userIdentity, out existingToken))
        return new AccountEntitlementToken((DateTimeOffset) utcNow)
        {
          AccountLicenseTokens = {
            {
              accountId,
              new LicenseToken(accountLicense, (DateTimeOffset) utcNow)
            }
          }
        };
      LicenseToken licenseToken = new LicenseToken(accountLicense, (DateTimeOffset) utcNow);
      if (!existingToken.AccountLicenseTokens.ContainsKey(accountId))
        existingToken.AccountLicenseTokens.Add(accountId, licenseToken);
      else
        existingToken.AccountLicenseTokens[accountId] = licenseToken;
      existingToken.LastUpdated = (DateTimeOffset) utcNow;
      return existingToken;
    }

    private bool TryGetValidExistingToken(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      out AccountEntitlementToken existingToken)
    {
      existingToken = this.GetValueFromCache(requestContext, userIdentity.MasterId);
      return existingToken != null && !this.HasTokenExpired(existingToken.LastUpdated);
    }

    private bool TryGetEntitlementFromUserIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      Guid accountId,
      out License accountEntitlement)
    {
      accountEntitlement = (License) null;
      AccountEntitlementToken existingToken;
      LicenseToken licenseToken;
      if (userIdentity == null || !this.TryGetValidExistingToken(requestContext, userIdentity, out existingToken) || !existingToken.AccountLicenseTokens.TryGetValue(accountId, out licenseToken) || this.HasTokenExpired(licenseToken.LastUpdated))
        return false;
      accountEntitlement = licenseToken.License;
      return true;
    }

    private bool HasTokenExpired(DateTimeOffset tokenLastUpdated) => (DateTimeOffset) DateTime.UtcNow > tokenLastUpdated + this.m_serviceSettings.TokenTimeToLive;

    private class AccountEntitlementCacheToken
    {
    }
  }
}
