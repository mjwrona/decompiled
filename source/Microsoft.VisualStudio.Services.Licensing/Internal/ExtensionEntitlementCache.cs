// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.ExtensionEntitlementCache
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class ExtensionEntitlementCache : IExtensionEntitlementCache, IVssFrameworkService
  {
    private readonly ILicensingConfigurationManager m_settingsManager;
    protected ExtensionEntitlementCacheConfiguration m_serviceSettings;
    private readonly CommandPropertiesSetter m_circuitBreakerSettings = new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 30));
    private static readonly string s_DisableExtensionEntitlementCacheForClient = "VisualStudio.Services.DisableExtensionEntitlementCacheForClient";
    private const string s_area = "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService";
    private const string s_layer = "BusinessLogic";
    private readonly Guid s_RedisNamespaceId = new Guid("0FB9CD69-C183-491B-91BF-6ABD61CDCD07");

    public ExtensionEntitlementCache()
      : this((ILicensingConfigurationManager) new LicensingConfigurationRegistryManager())
    {
    }

    internal ExtensionEntitlementCache(ILicensingConfigurationManager settingsManager) => this.m_settingsManager = settingsManager;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(systemRequestContext, nameof (systemRequestContext));
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<CachedRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Licensing/AccountExtensionEntitlement/...");
      this.PopulateSettings(vssRequestContext);
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(vssRequestContext, "Default", SqlNotificationEventClasses.ExtensionEntitlementChangedForPlatformCache, new SqlNotificationHandler(this.OnExtensionEntitlementChanged), false);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (changedEntries == null || !changedEntries.Any<RegistryEntry>())
        return;
      this.PopulateSettings(requestContext);
    }

    private void PopulateSettings(IVssRequestContext requestContext) => this.m_serviceSettings = this.m_settingsManager.GetExtensionEntitlementCacheConfiguration(requestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<CachedRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(vssRequestContext, "Default", SqlNotificationEventClasses.ExtensionEntitlementChangedForPlatformCache, new SqlNotificationHandler(this.OnExtensionEntitlementChanged), true);
    }

    public virtual IList<string> GetExtensionsAssignedToUser(
      IVssRequestContext requestContext,
      IEnumerable<Guid> accountIds,
      Guid userId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(accountIds, nameof (accountIds));
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = ExtensionEntitlementCache.GetIdentity(requestContext, userId);
      if (requestContext.IsFeatureEnabled(ExtensionEntitlementCache.s_DisableExtensionEntitlementCacheForClient))
        return this.GetExtensionsFromAccountHosts(requestContext, userIdentity, accountIds);
      IList<string> valueFromCache = this.GetValueFromCache(requestContext, userIdentity.MasterId);
      if (valueFromCache != null)
      {
        requestContext.Trace(1033801, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", "Retrived extensions for user {0} from Redis Cache", (object) userIdentity.MasterId);
        return valueFromCache;
      }
      IList<string> extensions = this.GetExtensionsFromAccountHosts(requestContext, userIdentity, accountIds);
      try
      {
        this.SetValueToCache(requestContext, userIdentity.MasterId, extensions);
        requestContext.TraceConditionally(1033803, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", (Func<string>) (() => string.Format("Found {0} extensions for user {1} ", extensions.Any<string>() ? (object) string.Join(",", (IEnumerable<string>) extensions) : (object) "no", (object) userIdentity.MasterId)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1034004, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", ex);
      }
      return extensions;
    }

    private void OnExtensionEntitlementChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      if (requestContext.IsFeatureEnabled(ExtensionEntitlementCache.s_DisableExtensionEntitlementCacheForClient))
        return;
      try
      {
        EntitlementChangeMessage entitlementChangeMessage = TeamFoundationSerializationUtility.Deserialize<EntitlementChangeMessage>(args.Data);
        if (entitlementChangeMessage != null)
        {
          foreach (Guid userId1 in entitlementChangeMessage.UserIds)
          {
            Guid userId = userId1;
            this.WrapCircuitBreaker(requestContext, (Action) (() =>
            {
              requestContext.Trace(1033860, TraceLevel.Info, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", "Removing cache for user: {0}", (object) userId);
              this.RemoveValueFromCache(requestContext, userId, out IList<string> _);
            }), nameof (OnExtensionEntitlementChanged));
          }
        }
        else
          requestContext.Trace(1033868, TraceLevel.Error, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", "Failed to deserialize EntitlementChangeMessage from notification event args.");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033869, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", ex);
      }
    }

    private IList<string> GetExtensionsFromAccountHosts(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity,
      IEnumerable<Guid> accountIds)
    {
      requestContext.Trace(1033802, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", "Cache-miss: Getting extensions from ExtensionEntitlementService from user {0}", (object) userIdentity.MasterId);
      List<string> fromAccountHosts = new List<string>();
      foreach (Guid accountId in accountIds)
      {
        IList<string> userExtension = this.GetUserExtension(requestContext, accountId, userIdentity);
        if (userExtension != null)
          fromAccountHosts.AddRange((IEnumerable<string>) userExtension);
      }
      return (IList<string>) fromAccountHosts;
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

    internal virtual IList<string> GetUserExtension(
      IVssRequestContext requestContext,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      try
      {
        using (VssRequestContextLicensingExtensions.VssRequestContextHolder organization = requestContext.ToOrganization(accountId))
        {
          requestContext.Trace(1033811, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", "Starting account host for account {0} to get account license for user {1}", (object) accountId, (object) userIdentity.MasterId);
          IVssRequestContext requestContext1 = organization.RequestContext;
          List<string> list = requestContext1.GetService<IExtensionEntitlementService>().GetExtensionsAssignedToUser(requestContext1, userIdentity.MasterId).Keys.ToList<string>();
          if (!list.Any<string>())
          {
            requestContext.Trace(1033812, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", "Did not find any extension for user {0} in account {1}", (object) userIdentity.MasterId, (object) accountId);
            return (IList<string>) null;
          }
          requestContext.Trace(1033813, TraceLevel.Verbose, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", "Found {0} extension for user {1} in account {1}", (object) list.Count<string>(), (object) userIdentity.MasterId, (object) accountId);
          return (IList<string>) list;
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1033818, TraceLevel.Warning, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", string.Format("Failed to retrieve user's extension for UserId: {0}, AccountId: {1}.", (object) userIdentity.MasterId, (object) accountId));
        requestContext.TraceException(1033819, TraceLevel.Warning, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", ex);
      }
      return (IList<string>) null;
    }

    private void WrapCircuitBreaker(
      IVssRequestContext requestContext,
      Action action,
      string commandKey)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "LicensingAndAccounts.").AndCommandKey((CommandKey) commandKey).AndCommandPropertiesDefaults(this.m_circuitBreakerSettings);
      new CommandService(requestContext, setter, action).Execute();
    }

    internal IList<string> GetValueFromCache(IVssRequestContext requestContext, Guid identityId)
    {
      IList<string> valueFromCache = (IList<string>) null;
      try
      {
        IMutableDictionaryCacheContainer<Guid, IList<string>> distributedCache = this.GetDistributedCache(requestContext);
        if (distributedCache != null)
          distributedCache.TryGet<Guid, IList<string>>(requestContext, identityId, out valueFromCache);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033858, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", ex);
      }
      return valueFromCache;
    }

    internal virtual void SetValueToCache(
      IVssRequestContext requestContext,
      Guid identityId,
      IList<string> entensions)
    {
      try
      {
        IMutableDictionaryCacheContainer<Guid, IList<string>> distributedCache = this.GetDistributedCache(requestContext);
        if (distributedCache == null || entensions == null)
          return;
        distributedCache.Set(requestContext, (IDictionary<Guid, IList<string>>) new Dictionary<Guid, IList<string>>()
        {
          {
            identityId,
            entensions
          }
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033848, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", ex);
      }
    }

    internal bool RemoveValueFromCache(
      IVssRequestContext requestContext,
      Guid identityId,
      out IList<string> entensions)
    {
      entensions = (IList<string>) null;
      try
      {
        IMutableDictionaryCacheContainer<Guid, IList<string>> distributedCache = this.GetDistributedCache(requestContext);
        if (distributedCache != null)
          distributedCache.TryGet<Guid, IList<string>>(requestContext, identityId, out entensions);
        if (entensions != null)
          distributedCache.Invalidate<Guid, IList<string>>(requestContext, identityId);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033838, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", ex);
      }
      return false;
    }

    private IMutableDictionaryCacheContainer<Guid, IList<string>> GetDistributedCache(
      IVssRequestContext requestContext)
    {
      try
      {
        IRedisCacheService service = requestContext.GetService<IRedisCacheService>();
        if (service.IsEnabled(requestContext))
          return service.GetVolatileDictionaryContainer<Guid, IList<string>, ExtensionEntitlementCache.UserExtensionsCacheToken>(requestContext, this.s_RedisNamespaceId, new ContainerSettings()
          {
            KeyExpiry = new TimeSpan?(this.m_serviceSettings.TokenTimeToLive),
            CiAreaName = typeof (ExtensionEntitlementCache).Name
          });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1033828, "VisualStudio.Services.LicensingService.ExtensionEntitlementCacheService", "BusinessLogic", ex);
      }
      return (IMutableDictionaryCacheContainer<Guid, IList<string>>) null;
    }

    private class UserExtensionsCacheToken
    {
    }
  }
}
