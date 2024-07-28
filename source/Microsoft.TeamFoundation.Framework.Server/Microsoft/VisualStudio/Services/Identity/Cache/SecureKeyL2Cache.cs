// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Cache.SecureKeyL2Cache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity.Cache
{
  public class SecureKeyL2Cache : IL2CacheService, IVssFrameworkService
  {
    private const string TraceArea = "IdentityCache";
    private const string TraceLayer = "SecureKeyL2CacheService";
    private IRedisCacheService m_redisCacheService;
    private SecureKeyL2Cache.CacheRegistrySettings m_registrySettings;
    private ContainerSettings m_cacheContainerSettings;
    internal static readonly Guid DefaultCacheNamespaceId = new Guid("E51C0477-D95C-472A-8155-578B80CCF9D1");
    internal static readonly string DefaultSecretKey = "be56f71ff0d36715a4ea4efde67d8d65022d094bf";
    internal static readonly TimeSpan DefaultCacheExpiration = TimeSpan.FromMinutes(10.0);

    public void Invalidate<T>(IVssRequestContext context, string key)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      IVssRequestContext requestContext = context.To(TeamFoundationHostType.Deployment);
      if (this.m_redisCacheService.IsEnabled(requestContext))
        this.GetRedisContainer(requestContext).Invalidate<string, byte[]>(requestContext, key);
      else
        context.Trace(80520, TraceLevel.Verbose, "IdentityCache", "SecureKeyL2CacheService", "RedisCacheService Disabled: Invalidation did not go through for key: {0}", (object) key);
    }

    public bool TryGet<T>(IVssRequestContext context, string key, out T cachedValue)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      byte[] cachedValue1;
      if (this.TryGet(context, key, out cachedValue1))
      {
        cachedValue = this.Deserialize<T>(cachedValue1);
        return true;
      }
      cachedValue = default (T);
      return false;
    }

    public bool TrySet<T>(IVssRequestContext context, T value, out string key)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      key = this.GetSecureKey(context, Guid.NewGuid(), this.m_registrySettings.SecretKeyForKeyHash);
      context.Trace(80524, TraceLevel.Verbose, "IdentityCache", "SecureKeyL2CacheService", "Key generated: {0}", (object) key);
      if (this.TrySet<T>(context, key, value))
        return true;
      key = (string) null;
      return false;
    }

    public bool TrySet<T>(IVssRequestContext context, string key, T value)
    {
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
        ArgumentUtility.CheckGenericForNull((object) value, nameof (value));
        ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
        IVssRequestContext requestContext = context.To(TeamFoundationHostType.Deployment);
        if (this.m_redisCacheService.IsEnabled(requestContext))
        {
          this.GetRedisContainer(requestContext).Set(requestContext, (IDictionary<string, byte[]>) new Dictionary<string, byte[]>()
          {
            {
              key,
              this.Serialize<T>(value)
            }
          });
          return true;
        }
        context.Trace(80521, TraceLevel.Verbose, "IdentityCache", "SecureKeyL2CacheService", "RedisCacheService Disabled: Set did not go through for key: {0}", (object) key);
        return false;
      }
      catch (Exception ex)
      {
        context.Trace(80525, TraceLevel.Warning, "IdentityCache", "SecureKeyL2CacheService", "TrySet exception - Exception {0}.", (object) ex);
        return false;
      }
    }

    private bool TryGet(IVssRequestContext context, string key, out byte[] cachedValue)
    {
      try
      {
        IVssRequestContext requestContext = context.To(TeamFoundationHostType.Deployment);
        if (this.m_redisCacheService.IsEnabled(requestContext))
        {
          if (this.GetRedisContainer(context).TryGet<string, byte[]>(requestContext, key, out cachedValue))
            return true;
          context.Trace(80523, TraceLevel.Verbose, "IdentityCache", "SecureKeyL2CacheService", "Cache miss from redis cache for the key: {0}", (object) key);
        }
        else
          context.Trace(80522, TraceLevel.Verbose, "IdentityCache", "SecureKeyL2CacheService", "RedisCacheService Disabled: Get did not go through for key: {0}", (object) key);
      }
      catch (Exception ex)
      {
        context.Trace(80526, TraceLevel.Warning, "IdentityCache", "SecureKeyL2CacheService", "RedisCacheService TryGet exception - Exception {0}.", (object) ex);
      }
      cachedValue = (byte[]) null;
      return false;
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new InvalidOperationException(FrameworkResources.ServiceAvailableInHostedTfsOnly());
      this.m_redisCacheService = requestContext.To(TeamFoundationHostType.Deployment).GetService<IRedisCacheService>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), "/Configuration/Identity/Cache/SecureKeyL2Cache/...");
      this.LoadRegistrySettings(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (changedEntries == null || changedEntries.Count <= 0)
        return;
      this.LoadRegistrySettings(requestContext);
    }

    private void LoadRegistrySettings(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      this.m_registrySettings = new SecureKeyL2Cache.CacheRegistrySettings()
      {
        CacheNameSpaceId = service.GetValue<Guid>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/SecureKeyL2Cache/CacheNamespaceId", SecureKeyL2Cache.DefaultCacheNamespaceId),
        CacheExpiry = service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/SecureKeyL2Cache/CacheKeyExpiry", SecureKeyL2Cache.DefaultCacheExpiration),
        SecretKeyForKeyHash = Encoding.UTF8.GetBytes(service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Identity/Cache/SecureKeyL2Cache/SecretKeyForKeyHash", SecureKeyL2Cache.DefaultSecretKey))
      };
      this.m_cacheContainerSettings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(this.m_registrySettings.CacheExpiry),
        CiAreaName = typeof (SecureKeyL2Cache).Name
      };
    }

    private IMutableDictionaryCacheContainer<string, byte[]> GetRedisContainer(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, byte[], SecureKeyL2Cache.SecureKeyStateCacheSecurityToken>(requestContext, this.m_registrySettings.CacheNameSpaceId, this.m_cacheContainerSettings);
    }

    private byte[] Serialize<T>(T value) => Encoding.UTF8.GetBytes(value.Serialize<T>());

    private T Deserialize<T>(byte[] value) => JsonUtilities.Deserialize<T>(Encoding.UTF8.GetString(value));

    protected virtual string GetSecureKey(
      IVssRequestContext context,
      Guid guidKey,
      byte[] hashSecret)
    {
      using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(string.Format("{0}{1}", context.UserContext != (IdentityDescriptor) null ? (object) context.UserContext.Identifier : (object) "", (object) guidKey), hashSecret))
        return hmacshA256Hash.HashBase32Encoded.TrimEnd('=').ToLower();
    }

    internal sealed class SecureKeyStateCacheSecurityToken
    {
    }

    internal static class CacheSettingKeys
    {
      internal const string Root = "/Configuration/Identity/Cache/SecureKeyL2Cache";
      internal const string SettingsRoot = "/Configuration/Identity/Cache/SecureKeyL2Cache/...";
      internal const string CacheNamespaceId = "/Configuration/Identity/Cache/SecureKeyL2Cache/CacheNamespaceId";
      internal const string CacheKeyExpiry = "/Configuration/Identity/Cache/SecureKeyL2Cache/CacheKeyExpiry";
      internal const string SecretKeyForKeyHash = "/Configuration/Identity/Cache/SecureKeyL2Cache/SecretKeyForKeyHash";
    }

    private class CacheRegistrySettings
    {
      internal Guid CacheNameSpaceId { get; set; }

      internal TimeSpan CacheExpiry { get; set; }

      internal byte[] SecretKeyForKeyHash { get; set; }
    }
  }
}
