// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.AadTokenServiceBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  public abstract class AadTokenServiceBase : 
    IAadTokenServiceInternal,
    IAadTokenService,
    IVssFrameworkService
  {
    private IRedisCacheService _redisCacheService;
    private AadAccessTokenCache _localCache;
    private AadOnBehalfOfCache _onBehalfOfCache;
    private const int _defaultCacheEvictionOperationIntervalInHours = 2;
    internal const string AadTokenServiceLocalCacheFeatureName = "VisualStudio.Services.AadTokenService.LocalCache";
    internal const string AadTokenServiceRemoteCacheFeatureName = "VisualStudio.Services.AadTokenService.RemoteCache";
    internal const string AadTokenServiceRaiseUserTokenFeatureName = "VisualStudio.Services.AadTokenService.RaiseTokenUpdateEventOnSignIn";
    internal static readonly Guid RemoteTokenCacheNamespace = new Guid("{A8EB573F-3EEC-465D-A4AA-5D9F95D2C2EE}");
    protected const string TraceArea = "AzureActiveDirectory";
    protected const string TraceLayer = "AadTokenService";

    internal AadTokenServiceBase()
    {
    }

    internal AadTokenServiceBase(
      IRedisCacheService redisCacheService,
      AadAccessTokenCache localCache)
    {
      this._redisCacheService = redisCacheService;
      this._localCache = localCache;
    }

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this._localCache = new AadAccessTokenCache();
      this._redisCacheService = systemRequestContext.GetService<IRedisCacheService>();
      this._onBehalfOfCache = new AadOnBehalfOfCache();
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      this.LoadSettings(systemRequestContext, service);
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Aad/...");
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      systemRequestContext.GetService<ITeamFoundationTaskService>().RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.EvictExpiredTokens));
    }

    public string ClientId { get; protected internal set; }

    public string Authority { get; protected internal set; }

    public string DefaultResource { get; protected internal set; }

    public JwtSecurityToken AcquireToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      if (identityDescriptor == (IdentityDescriptor) null)
      {
        ArgumentUtility.CheckForNull<IdentityDescriptor>(requestContext.UserContext, "requestContext.UserContext");
        identityDescriptor = requestContext.UserContext;
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
      {
        requestContext.Trace(9002014, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "An identity with descriptor {0} was not found.", (object) identityDescriptor.Identifier);
        throw new IdentityNotFoundException(identityDescriptor);
      }
      return this.AcquireToken(requestContext, resource, tenantId, identity);
    }

    public JwtSecurityToken AcquireToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      SubjectDescriptor subjectDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        subjectDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
      {
        requestContext.Trace(9002014, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "An identity with subject descriptor {0} was not found.", (object) subjectDescriptor.Identifier);
        throw new IdentityNotFoundException(subjectDescriptor);
      }
      return this.AcquireToken(requestContext, resource, tenantId, identity);
    }

    public virtual JwtSecurityToken AcquireToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      requestContext.TraceEnter(9002011, "AzureActiveDirectory", "AadTokenService", nameof (AcquireToken));
      try
      {
        try
        {
          Guid result;
          if (Guid.TryParse(tenantId, out result))
          {
            if (result == Guid.Empty)
              throw new ArgumentException("The TenantId must not be an empty guid.");
          }
        }
        finally
        {
          requestContext.Trace(9002035, TraceLevel.Warning, "AzureActiveDirectory", "AadTokenService", "Attempting acquire token on tenant \"{0}\" for resource \"{1}\"", (object) (tenantId ?? "<NULL>"), (object) (resource ?? "<NULL>"));
        }
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<IAadTokenService>().AcquireToken(vssRequestContext, resource, tenantId, identity);
        }
        if (IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) identity))
        {
          requestContext.Trace(9002014, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "Access token for service identities cannot be acquired!");
          throw new AadAuthorizationException();
        }
        requestContext.Trace(9002013, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", "Acquiring access token for identity {0}", (object) identity.Descriptor.Identifier);
        try
        {
          string userTokenCacheKey = AadTokenServiceBase.CreateUserTokenCacheKey(identity.Descriptor, resource, tenantId);
          requestContext.Trace(9002013, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", "Getting access token for identity {0} from cache with key {1}", (object) identity.Descriptor.Identifier, (object) userTokenCacheKey);
          return this.GetCachedToken(requestContext, userTokenCacheKey, (Func<JwtSecurityToken>) (() => this.GetUserAccessToken(requestContext, resource, tenantId, identity)));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(9002014, TraceLevel.Warning, "AzureActiveDirectory", "AadTokenService", ex);
          throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(9001012, "AzureActiveDirectory", "AadTokenService", nameof (AcquireToken));
      }
    }

    public virtual JwtSecurityToken AcquireAppToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      Guid result;
      if (Guid.TryParse(tenantId, out result) && result == Guid.Empty)
        throw new ArgumentException("The TenantId must not be an empty guid.");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IAadTokenService>().AcquireAppToken(vssRequestContext, resource, tenantId);
      }
      requestContext.TraceEnter(9002027, "AzureActiveDirectory", "AadTokenService", nameof (AcquireAppToken));
      try
      {
        string appTokenCacheKey = AadTokenServiceBase.CreateAppTokenCacheKey(resource, tenantId);
        requestContext.Trace(9002029, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", "Getting app token from cache with cache key {0}", (object) appTokenCacheKey);
        JwtSecurityToken cachedToken = this.GetCachedToken(requestContext, appTokenCacheKey, (Func<JwtSecurityToken>) (() => this.GetAppAccessToken(requestContext, resource, tenantId)));
        requestContext.TraceLeave(9002029, "AzureActiveDirectory", "AadTokenService", nameof (AcquireAppToken));
        return cachedToken;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9002030, "AzureActiveDirectory", "AadTokenService", ex);
        throw;
      }
    }

    public virtual JwtSecurityToken AcquireTokenByAuthorizationCode(
      IVssRequestContext requestContext,
      string authCode,
      string resource,
      string tenantId,
      Uri replyToUri,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(authCode, nameof (authCode));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckForNull<Uri>(replyToUri, nameof (replyToUri));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      Guid result;
      if (Guid.TryParse(tenantId, out result) && result == Guid.Empty)
        throw new ArgumentException("The TenantId must not be an empty guid.");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IAadTokenService>().AcquireTokenByAuthorizationCode(vssRequestContext, authCode, resource, tenantId, replyToUri, identity);
      }
      requestContext.TraceEnter(9002031, "AzureActiveDirectory", "AadTokenService", nameof (AcquireTokenByAuthorizationCode));
      requestContext.Trace(9002033, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", "Acquiring access token from auth code for identity {0}", (object) identity.Descriptor.Identifier);
      JwtSecurityToken tokenFromAuthCode;
      try
      {
        tokenFromAuthCode = this.GetUserAccessTokenFromAuthCode(requestContext, authCode, resource, tenantId, replyToUri, identity);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.AadTokenService.RaiseTokenUpdateEventOnSignIn"))
          this.RaiseUserTokenUpdateEvent(requestContext, identity.Descriptor);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9002034, "AzureActiveDirectory", "AadTokenService", ex);
        throw;
      }
      if (tokenFromAuthCode != null)
      {
        string userTokenCacheKey = AadTokenServiceBase.CreateUserTokenCacheKey(identity.Descriptor, resource, tenantId);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.AadTokenService.RemoteCache"))
        {
          this.UpdateRemoteCache(requestContext, userTokenCacheKey, tokenFromAuthCode);
          requestContext.Trace(9002026, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Updated remote cache key {0} with token valid till {1}", (object) userTokenCacheKey, (object) tokenFromAuthCode.ValidTo);
        }
        this._localCache.Set(userTokenCacheKey, tokenFromAuthCode);
        requestContext.Trace(9002026, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Updated local cache key {0} with token valid till {1}", (object) userTokenCacheKey, (object) tokenFromAuthCode.ValidTo);
      }
      requestContext.TraceLeave(9002032, "AzureActiveDirectory", "AadTokenService", nameof (AcquireTokenByAuthorizationCode));
      return tokenFromAuthCode;
    }

    public virtual string TryUpdateRefreshTokenOnBehalfOfUser(
      IVssRequestContext requestContext,
      string accessToken,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(accessToken, nameof (accessToken));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      Guid result;
      if (Guid.TryParse(tenantId, out result) && result == Guid.Empty)
        throw new ArgumentException("The TenantId must not be an empty guid.");
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IAadTokenService>().TryUpdateRefreshTokenOnBehalfOfUser(vssRequestContext, accessToken, resource, tenantId, identity);
      }
      if (this._onBehalfOfCache.HasValue(identity.Descriptor))
        return accessToken;
      this._onBehalfOfCache.Set(identity.Descriptor);
      JwtSecurityToken jwtSecurityToken = (JwtSecurityToken) null;
      try
      {
        jwtSecurityToken = this.AcquireToken(requestContext, this.DefaultResource, tenantId, identity);
        if (jwtSecurityToken != null)
          return jwtSecurityToken.RawData;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9002202, TraceLevel.Warning, "AzureActiveDirectory", "AadTokenService", ex);
      }
      if (jwtSecurityToken != null)
        return (string) null;
      return this.UpdateRefreshToken(requestContext, accessToken, resource, tenantId, identity);
    }

    protected virtual void RaiseUserTokenUpdateEvent(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
    }

    internal virtual void RaiseUserMembershipsUpdateEvent(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor,
      string resource,
      bool shouldUpdateTokenCache)
    {
    }

    JwtSecurityToken IAadTokenServiceInternal.AcquireTokenFromCache(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      if (identityDescriptor == (IdentityDescriptor) null)
      {
        ArgumentUtility.CheckForNull<IdentityDescriptor>(requestContext.UserContext, "requestContext.UserContext");
        identityDescriptor = requestContext.UserContext;
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identityDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
      {
        requestContext.Trace(9002040, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "An identity with descriptor {0} was not found.", (object) identityDescriptor.Identifier);
        throw new IdentityNotFoundException(identityDescriptor);
      }
      return ((IAadTokenServiceInternal) this).AcquireTokenFromCache(requestContext, resource, tenantId, identity);
    }

    JwtSecurityToken IAadTokenServiceInternal.AcquireTokenFromCache(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      SubjectDescriptor subjectDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<SubjectDescriptor>) new SubjectDescriptor[1]
      {
        subjectDescriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity == null)
      {
        requestContext.Trace(9002040, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "An identity with subject descriptor {0} was not found.", (object) subjectDescriptor.Identifier);
        throw new IdentityNotFoundException(subjectDescriptor);
      }
      return ((IAadTokenServiceInternal) this).AcquireTokenFromCache(requestContext, resource, tenantId, identity);
    }

    JwtSecurityToken IAadTokenServiceInternal.AcquireTokenFromCache(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      requestContext.TraceEnter(9002037, "AzureActiveDirectory", "AadTokenService", "AcquireTokenFromCache");
      try
      {
        try
        {
          Guid result;
          if (Guid.TryParse(tenantId, out result))
          {
            if (result == Guid.Empty)
              throw new ArgumentException("The TenantId must not be an empty guid.");
          }
        }
        finally
        {
          requestContext.Trace(9002041, TraceLevel.Warning, "AzureActiveDirectory", "AadTokenService", "Attempting acquire token from cache on tenant \"{0}\" for resource \"{1}\"", (object) (tenantId ?? "<NULL>"), (object) (resource ?? "<NULL>"));
        }
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<IAadTokenService>().AcquireToken(vssRequestContext, resource, tenantId, identity);
        }
        if (IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) identity))
        {
          requestContext.Trace(9002040, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", "Access token for service identities cannot be acquired!");
          throw new AadAuthorizationException();
        }
        requestContext.Trace(9002039, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", "Acquiring access token from cache for identity {0}", (object) identity.Descriptor.Identifier);
        try
        {
          string userTokenCacheKey = AadTokenServiceBase.CreateUserTokenCacheKey(identity.Descriptor, resource, tenantId);
          requestContext.Trace(9002039, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", "Getting access token for identity {0} from cache with key {1}", (object) identity.Descriptor.Identifier, (object) userTokenCacheKey);
          return this.GetCachedToken(requestContext, userTokenCacheKey, (Func<JwtSecurityToken>) null);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(9002040, TraceLevel.Warning, "AzureActiveDirectory", "AadTokenService", ex);
          throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(9002038, "AzureActiveDirectory", "AadTokenService", "AcquireTokenFromCache");
      }
    }

    Task<string> IAadTokenServiceInternal.AcquireTokenAsync(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(identityDescriptor, nameof (identityDescriptor));
      return this.GetUserAccessTokenAsync(requestContext, resource, tenantId, identityDescriptor);
    }

    protected internal abstract JwtSecurityToken GetUserAccessToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    protected internal abstract JwtSecurityToken GetAppAccessToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId);

    protected internal abstract JwtSecurityToken GetUserAccessTokenFromAuthCode(
      IVssRequestContext requestContext,
      string authCode,
      string resource,
      string tenantId,
      Uri replyToUri,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    protected internal abstract string UpdateRefreshToken(
      IVssRequestContext requestContext,
      string accessToken,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    protected internal abstract Task<string> GetUserAccessTokenAsync(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor);

    private void LoadSettings(
      IVssRequestContext requestContext,
      IVssRegistryService registryService,
      ITeamFoundationTaskService taskService = null)
    {
      this.Authority = registryService.GetValue(requestContext, (RegistryQuery) "/Service/Aad/AuthAuthority", string.Empty);
      this.ClientId = registryService.GetValue(requestContext, (RegistryQuery) "/Service/Aad/AuthClientId", string.Empty);
      this.DefaultResource = registryService.GetValue(requestContext, (RegistryQuery) "/Service/Aad/GraphApiResource", false, (string) null);
      if (!registryService.GetValue<bool>(requestContext, (RegistryQuery) "/Service/Aad/CacheEvictionEnabled", false))
        return;
      if (taskService == null)
        taskService = requestContext.GetService<ITeamFoundationTaskService>();
      int num = Math.Max(1, registryService.GetValue<int>(requestContext, (RegistryQuery) "/Service/Aad/CacheEvictionIntervalInHours", 2));
      taskService.AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.EvictExpiredTokens), (object) null, num * 60 * 60 * 1000));
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      ITeamFoundationTaskService service1 = requestContext.GetService<ITeamFoundationTaskService>();
      service1.RemoveTask(requestContext, new TeamFoundationTaskCallback(this.EvictExpiredTokens));
      IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
      this.LoadSettings(requestContext, service2, service1);
    }

    private JwtSecurityToken GetCachedToken(
      IVssRequestContext requestContext,
      string cacheKey,
      Func<JwtSecurityToken> missHandler)
    {
      requestContext.TraceEnter(9002015, "AzureActiveDirectory", "AadTokenService", nameof (GetCachedToken));
      try
      {
        JwtSecurityToken cachedToken = (JwtSecurityToken) null;
        DateTimeOffset utcNow;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.AadTokenService.LocalCache"))
        {
          if (this._localCache.TryGetValue(cacheKey, out cachedToken))
          {
            DateTimeOffset validTo = (DateTimeOffset) cachedToken.ValidTo;
            utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset dateTimeOffset = utcNow.AddSeconds(30.0);
            if (validTo >= dateTimeOffset)
            {
              requestContext.Trace(9002017, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Local cache hit for cache key {0}", (object) cacheKey);
              return cachedToken;
            }
            requestContext.Trace(9002019, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Found cached access token for cache key {0} but cached token has expired. Token valid to {1}", (object) cacheKey, (object) cachedToken.ValidTo);
          }
          else
            requestContext.Trace(9002018, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Local cache miss cache key {0}", (object) cacheKey);
        }
        else
          requestContext.Trace(9002020, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Aad token service local cache disabled.");
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.AadTokenService.RemoteCache"))
        {
          if (this.TryGetTokenFromRemoteCache(requestContext, cacheKey, out cachedToken))
          {
            DateTimeOffset validTo = (DateTimeOffset) cachedToken.ValidTo;
            utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset dateTimeOffset = utcNow.AddSeconds(30.0);
            if (validTo >= dateTimeOffset)
            {
              this._localCache.Set(cacheKey, cachedToken);
              requestContext.Trace(9002017, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Remote cache hit for cache key {0}", (object) cacheKey);
              requestContext.Trace(9002025, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Updated local cache key {0} with token valid till {1}", (object) cacheKey, (object) cachedToken.ValidTo);
              return cachedToken;
            }
            requestContext.Trace(9002023, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Found cached access token for cache key {0} but cached token has expired. Token valid to {1}", (object) cacheKey, (object) cachedToken.ValidTo);
          }
          else
            requestContext.Trace(9002022, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Remote cache miss cache key {0}", (object) cacheKey);
        }
        else
          requestContext.Trace(9002024, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Aad token service remote cache disabled.");
        if (missHandler != null)
        {
          cachedToken = missHandler();
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.AadTokenService.RemoteCache"))
          {
            this.UpdateRemoteCache(requestContext, cacheKey, cachedToken);
            requestContext.Trace(9002026, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Updated remote cache key {0} with token valid till {1}", (object) cacheKey, (object) cachedToken.ValidTo);
          }
          this._localCache.Set(cacheKey, cachedToken);
          requestContext.Trace(9002025, TraceLevel.Info, "AzureActiveDirectory", "AadTokenService", "Updated local cache key {0} with token valid till {1}", (object) cacheKey, (object) cachedToken.ValidTo);
        }
        return cachedToken;
      }
      finally
      {
        requestContext.TraceLeave(9002016, "AzureActiveDirectory", "AadTokenService", nameof (GetCachedToken));
      }
    }

    private bool TryGetTokenFromRemoteCache(
      IVssRequestContext requestContext,
      string cacheKey,
      out JwtSecurityToken token)
    {
      requestContext.TraceEnter(0, "AzureActiveDirectory", "AadTokenService", nameof (TryGetTokenFromRemoteCache));
      try
      {
        string empty = string.Empty;
        if (this._redisCacheService.IsEnabled(requestContext))
        {
          if (this.GetRemoteCacheContainer(requestContext).TryGet<string, string>(requestContext, cacheKey, out empty))
          {
            token = new JwtSecurityToken(empty);
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9002024, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", ex);
      }
      finally
      {
        requestContext.TraceLeave(0, "AzureActiveDirectory", "AadTokenService", nameof (TryGetTokenFromRemoteCache));
      }
      token = (JwtSecurityToken) null;
      return false;
    }

    private void UpdateRemoteCache(
      IVssRequestContext requestContext,
      string cacheKey,
      JwtSecurityToken token)
    {
      requestContext.TraceEnter(0, "AzureActiveDirectory", "AadTokenService", nameof (UpdateRemoteCache));
      try
      {
        if (!this._redisCacheService.IsEnabled(requestContext))
          return;
        this.GetRemoteCacheContainer(requestContext).Set(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            cacheKey,
            token.RawData
          }
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9002024, TraceLevel.Verbose, "AzureActiveDirectory", "AadTokenService", ex);
      }
      finally
      {
        requestContext.TraceLeave(0, "AzureActiveDirectory", "AadTokenService", nameof (UpdateRemoteCache));
      }
    }

    private IMutableDictionaryCacheContainer<string, string> GetRemoteCacheContainer(
      IVssRequestContext requestContext)
    {
      ContainerSettings settings = new ContainerSettings()
      {
        CiAreaName = "AadRemoteTokenCache"
      };
      return this._redisCacheService.GetVolatileDictionaryContainer<string, string, AadTokenServiceBase.RedisCacheSecurityToken>(requestContext, AadTokenServiceBase.RemoteTokenCacheNamespace, settings);
    }

    public string GetAuthority(string tenantId) => new UriBuilder(Uri.UriSchemeHttps, this.Authority)
    {
      Path = tenantId
    }.ToString();

    internal static string CreateUserTokenCacheKey(
      IdentityDescriptor identityDescriptor,
      string resource,
      string tenantId)
    {
      return string.Format("{0}-{1}-{2}-{3}", (object) identityDescriptor.IdentityType, (object) identityDescriptor.Identifier, (object) resource, (object) tenantId);
    }

    internal static string CreateAppTokenCacheKey(string resource, string tenantId) => string.Format("App-{0}-{1}", (object) resource, (object) tenantId);

    private void EvictExpiredTokens(IVssRequestContext requestContext, object taskArgs)
    {
      requestContext.TraceEnter(0, "AzureActiveDirectory", "AadTokenService", nameof (EvictExpiredTokens));
      try
      {
        this._localCache.Sweep();
        this._onBehalfOfCache.Sweep();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(9002036, TraceLevel.Error, "AzureActiveDirectory", "AadTokenService", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(0, "AzureActiveDirectory", "AadTokenService", nameof (EvictExpiredTokens));
      }
    }

    internal sealed class RedisCacheSecurityToken
    {
    }
  }
}
