// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap.AadCapCacheService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class AadCapCacheService : IAadCapCacheService, IVssFrameworkService
  {
    private const string TraceArea = "AzureActiveDirectory";
    private const string TraceLayer = "AadCapCacheService";
    private static readonly Guid s_namespace = new Guid("0EE5F0C7-4C0E-4B43-8073-BA28C7C8CE62");
    private IMutableDictionaryCacheContainer<string, AadCapResult> m_remoteCache;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service1 = vssRequestContext.GetService<IVssRegistryService>();
      TimeSpan timeSpan = service1.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) AadCapConfigHelper.Constants.SettingKeys.CacheEntryTimeout, AadCapConfigHelper.Constants.SettingDefaults.CacheEntryTimeout);
      IRedisCacheService service2 = systemRequestContext.GetService<IRedisCacheService>();
      systemRequestContext.Trace(9003400, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Setting CAP remote cache expiration to {0}", (object) timeSpan);
      this.m_remoteCache = service2.GetVolatileDictionaryContainer<string, AadCapResult, AadCapCacheService.SecurityToken>(systemRequestContext, AadCapCacheService.s_namespace, new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(timeSpan),
        CiAreaName = nameof (AadCapCacheService),
        NoThrowMode = new bool?(true)
      });
      service1.RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), AadCapConfigHelper.Constants.SettingKeys.CacheEntryTimeout);
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext requestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      ArgumentUtility.CheckForNull<RegistryEntryCollection>(changedEntries, nameof (changedEntries));
      if (!changedEntries.Any<RegistryEntry>())
        return;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      TimeSpan expirationTime = vssRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) AadCapConfigHelper.Constants.SettingKeys.CacheEntryTimeout, AadCapConfigHelper.Constants.SettingDefaults.CacheEntryTimeout);
      context.GetService<AadCapCacheService.AadCapMemoryCacheService>().SetExpiry(context, expirationTime);
      this.m_remoteCache = context.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, AadCapResult, AadCapCacheService.SecurityToken>(context, AadCapCacheService.s_namespace, new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(expirationTime),
        CiAreaName = nameof (AadCapCacheService),
        NoThrowMode = new bool?(true)
      });
      context.Trace(9003411, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), string.Format("Setting CAP  cache expiration to {0}", (object) expirationTime));
    }

    public AadCapResult CheckIsUserConditionAllowed(
      IVssRequestContext context,
      Guid tenantId,
      SubjectDescriptor subjectDescriptor,
      string clientIp)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      try
      {
        context.TraceEnter(9003400, "AzureActiveDirectory", nameof (AadCapCacheService), nameof (CheckIsUserConditionAllowed));
        context.TraceDataConditionally(9003401, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Checking for CAP Cache", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          subjectDescriptor = subjectDescriptor,
          clientIp = clientIp
        }), nameof (CheckIsUserConditionAllowed));
        AadCapResult cacheResult = (AadCapResult) null;
        string cacheKey = this.GetCacheKey(context, subjectDescriptor, clientIp);
        if (!context.RootContext.IsImsFeatureEnabled(AadCapConfigHelper.Constants.FeatureFlags.DisableMemoryCacheConditionalAccessValidation))
          cacheResult = context.GetService<AadCapCacheService.AadCapMemoryCacheService>().CheckIsUserConditionAllowed(context, cacheKey);
        if (cacheResult != null)
        {
          context.TraceDataConditionally(9003402, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Found CAP result in local cache", (Func<object>) (() => (object) new
          {
            cacheKey = cacheKey,
            cacheResult = cacheResult
          }), nameof (CheckIsUserConditionAllowed));
        }
        else
        {
          AadCapResult aadCapResult;
          if (!context.RootContext.IsImsFeatureEnabled(AadCapConfigHelper.Constants.FeatureFlags.DisableRedisCacheConditionalAccessValidation) && this.m_remoteCache.TryGet<string, AadCapResult>(context, cacheKey, out aadCapResult))
          {
            cacheResult = aadCapResult;
            context.TraceDataConditionally(9003403, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Found CAP result in remote cache", (Func<object>) (() => (object) new
            {
              cacheKey = cacheKey,
              cacheResult = cacheResult
            }), nameof (CheckIsUserConditionAllowed));
          }
          else
            context.TraceDataConditionally(9003404, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Could not find CAP result in cache", (Func<object>) (() => (object) new
            {
              cacheKey = cacheKey
            }), nameof (CheckIsUserConditionAllowed));
        }
        return cacheResult;
      }
      finally
      {
        context.TraceLeave(9003400, "AzureActiveDirectory", nameof (AadCapCacheService), nameof (CheckIsUserConditionAllowed));
      }
    }

    public void Set(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor,
      string clientIp,
      AadCapResult capResult)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      string cacheKey = this.GetCacheKey(context, subjectDescriptor, clientIp);
      context.TraceDataConditionally(9003405, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Setting CAP result value", (Func<object>) (() => (object) new
      {
        cacheKey = cacheKey,
        capResult = capResult
      }), nameof (Set));
      if (!context.RootContext.IsImsFeatureEnabled(AadCapConfigHelper.Constants.FeatureFlags.DisableMemoryCacheConditionalAccessValidation))
      {
        context.GetService<AadCapCacheService.AadCapMemoryCacheService>().Set(context, cacheKey, capResult);
        context.TraceDataConditionally(9003406, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Setting CAP result in local cache", (Func<object>) (() => (object) new
        {
          cacheKey = cacheKey,
          capResult = capResult
        }), nameof (Set));
      }
      if (context.RootContext.IsImsFeatureEnabled(AadCapConfigHelper.Constants.FeatureFlags.DisableRedisCacheConditionalAccessValidation))
        return;
      this.m_remoteCache.Set(context, (IDictionary<string, AadCapResult>) new Dictionary<string, AadCapResult>()
      {
        {
          cacheKey,
          capResult
        }
      });
      context.TraceDataConditionally(9003407, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Setting CAP result in remote cache", (Func<object>) (() => (object) new
      {
        cacheKey = cacheKey,
        capResult = capResult
      }), nameof (Set));
    }

    private string GetCacheKey(
      IVssRequestContext requestContext,
      SubjectDescriptor subjectDescriptor,
      string clientIp)
    {
      return string.Format("{0}:{1}", (object) subjectDescriptor, (object) clientIp);
    }

    internal class SecurityToken
    {
    }

    internal class AadCapMemoryCacheService : VssMemoryCacheService<string, AadCapResult>
    {
      public AadCapMemoryCacheService()
        : base(TimeSpan.FromHours(1.0))
      {
      }

      protected override void ServiceStart(IVssRequestContext systemRequestContext)
      {
        base.ServiceStart(systemRequestContext);
        IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Deployment);
        TimeSpan timeSpan = vssRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) AadCapConfigHelper.Constants.SettingKeys.CacheEntryTimeout, AadCapConfigHelper.Constants.SettingDefaults.CacheEntryTimeout);
        systemRequestContext.Trace(9003409, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Setting CAP local cache expiration to {0}", (object) timeSpan);
        this.ExpiryInterval.Value = timeSpan;
      }

      protected override void ServiceEnd(IVssRequestContext systemRequestContext)
      {
      }

      public AadCapResult CheckIsUserConditionAllowed(IVssRequestContext context, string cacheKey)
      {
        AadCapResult capResult;
        if (!this.TryGetValue(context, cacheKey, out capResult))
          return (AadCapResult) null;
        context.TraceDataConditionally(9003410, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), "Return CAP result from local cache", (Func<object>) (() => (object) new
        {
          cacheKey = cacheKey,
          capResult = capResult
        }), nameof (CheckIsUserConditionAllowed));
        return capResult;
      }

      public void SetExpiry(IVssRequestContext context, TimeSpan expirationTime)
      {
        context.Trace(9003411, TraceLevel.Verbose, "AzureActiveDirectory", nameof (AadCapCacheService), string.Format("Setting local cache expiration to {0}", (object) expirationTime));
        this.ExpiryInterval.Value = expirationTime;
      }
    }
  }
}
