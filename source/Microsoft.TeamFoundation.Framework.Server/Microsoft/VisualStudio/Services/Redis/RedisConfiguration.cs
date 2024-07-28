// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.RedisConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class RedisConfiguration
  {
    private const string c_redisClusterFeature = "VisualStudio.Services.Redis.Cluster";
    private static VssRefreshCache<FeatureAvailabilityInformation> s_redisClusterFeature = new VssRefreshCache<FeatureAvailabilityInformation>(TimeSpan.FromSeconds(10.0), (Func<IVssRequestContext, FeatureAvailabilityInformation>) (rq =>
    {
      IVssRequestContext vssRequestContext = rq.Elevate();
      return vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(vssRequestContext, "VisualStudio.Services.Redis.Cluster", false);
    }), true);
    private VssRefreshCache<FeatureAvailabilityInformation> m_redisDisabledFeature;

    public RedisConfiguration()
      : this(new RedisRegistryKeys())
    {
    }

    public RedisConfiguration(RedisRegistryKeys keys)
    {
      this.BigRedSwitchFeatureName = "VisualStudio.FrameworkService.RedisCache.Disabled";
      this.StrongBoxDrawerName = "ConfigurationSecrets";
      this.StrongBoxCachingServiceKey = "RedisPassword";
      this.CircuitBreakerKey = (CommandGroupKey) "Redis";
      this.Keys = keys;
      this.m_redisDisabledFeature = new VssRefreshCache<FeatureAvailabilityInformation>(TimeSpan.FromSeconds(10.0), (Func<IVssRequestContext, FeatureAvailabilityInformation>) (rq =>
      {
        IVssRequestContext vssRequestContext = rq.Elevate();
        return vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(vssRequestContext, this.BigRedSwitchFeatureName, false);
      }), true);
    }

    private RedisConfiguration(RedisConfiguration configurationToCopy)
      : this(configurationToCopy.Keys)
    {
      this.BigRedSwitchFeatureName = configurationToCopy.BigRedSwitchFeatureName;
      this.CircuitBreakerKey = configurationToCopy.CircuitBreakerKey;
      this.StrongBoxCachingServiceKey = configurationToCopy.StrongBoxCachingServiceKey;
      this.StrongBoxDrawerName = configurationToCopy.StrongBoxDrawerName;
    }

    public RedisRegistryKeys Keys { get; private set; }

    public string BigRedSwitchFeatureName { get; }

    public string StrongBoxDrawerName { get; set; }

    public string StrongBoxCachingServiceKey { get; set; }

    public CommandGroupKey CircuitBreakerKey { get; set; }

    public static TimeSpan MaxExpiry => TimeSpan.MaxValue - TimeSpan.FromSeconds(1.0);

    public static RedisConfiguration Default => new RedisConfiguration();

    public RedisConfiguration Clone() => new RedisConfiguration(this);

    public bool IsRedisEnabled(IVssRequestContext requestContext) => this.m_redisDisabledFeature.Get(requestContext).EffectiveState != FeatureAvailabilityState.On;

    public static bool IsClusterEnabled(IVssRequestContext requestContext) => RedisConfiguration.s_redisClusterFeature.Get(requestContext).EffectiveState == FeatureAvailabilityState.On;

    internal static T GetRegistryValue<T>(
      IVssRequestContext requestContext,
      string registryKey,
      T defaultValue)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IVssRegistryService>().GetValue<T>(vssRequestContext, (RegistryQuery) registryKey, defaultValue);
    }

    internal static void SetRegistryValue<T>(
      IVssRequestContext requestContext,
      string registryKey,
      T value)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().SetValue<T>(vssRequestContext, registryKey, value);
    }
  }
}
