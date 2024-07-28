// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.CommandPropertiesRegistry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public sealed class CommandPropertiesRegistry : CommandPropertiesDefault
  {
    public CommandPropertiesRegistry(
      IVssRequestContext requestContext,
      CommandKey key,
      CommandPropertiesSetter setter)
      : base(setter)
    {
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery = new RegistryQuery(FrameworkServerConstants.CircuitBreakerRegistryRootPath + "/" + key.Name, (string) null, 1);
      IVssRequestContext requestContext1 = requestContext;
      ref RegistryQuery local = ref registryQuery;
      IEnumerable<RegistryItem> source = service.Read(requestContext1, in local);
      if (!(source is ICollection<RegistryItem> registrySettings))
        registrySettings = (ICollection<RegistryItem>) source.ToList<RegistryItem>();
      if (registrySettings.Count == 0)
        return;
      this.CircuitBreakerDisabled = CommandPropertiesRegistry.GetValue<bool>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerDisabled", this.CircuitBreakerDisabled);
      this.CircuitBreakerForceClosed = CommandPropertiesRegistry.GetValue<bool>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerForceClosed", this.CircuitBreakerForceClosed);
      this.CircuitBreakerForceOpen = CommandPropertiesRegistry.GetValue<bool>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerForceOpen", this.CircuitBreakerForceOpen);
      int num1 = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerErrorThresholdPercentage", this.CircuitBreakerErrorThresholdPercentage);
      if (num1 >= 0 && num1 <= 100)
        this.CircuitBreakerErrorThresholdPercentage = num1;
      int num2 = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerRequestVolumeThreshold", this.CircuitBreakerRequestVolumeThreshold);
      if (num2 > 0)
        this.CircuitBreakerRequestVolumeThreshold = num2;
      TimeSpan timeSpan1 = CommandPropertiesRegistry.GetValue<TimeSpan>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerMaxBackoff", this.CircuitBreakerMaxBackoff);
      if (timeSpan1 != TimeSpan.Zero)
      {
        this.CircuitBreakerMaxBackoff = timeSpan1;
        this.CircuitBreakerMinBackoff = CommandPropertiesRegistry.GetValue<TimeSpan>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerMinBackoff", this.CircuitBreakerMinBackoff);
        this.CircuitBreakerDeltaBackoff = CommandPropertiesRegistry.GetValue<TimeSpan>((IEnumerable<RegistryItem>) registrySettings, "CircuitBreakerDeltaBackoff", this.CircuitBreakerDeltaBackoff);
      }
      TimeSpan timeSpan2 = CommandPropertiesRegistry.GetValue<TimeSpan>((IEnumerable<RegistryItem>) registrySettings, "ExecutionTimeout", this.ExecutionTimeout);
      if (timeSpan2 != TimeSpan.Zero)
        this.ExecutionTimeout = timeSpan2;
      int num3 = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "ExecutionMaxConcurrentRequests", this.ExecutionMaxConcurrentRequests);
      if (num3 > 0)
        this.ExecutionMaxConcurrentRequests = num3;
      int num4 = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "FallbackMaxConcurrentRequests", this.FallbackMaxConcurrentRequests);
      if (num4 > 0)
        this.FallbackMaxConcurrentRequests = num4;
      int num5 = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "ExecutionMaxRequests", this.ExecutionMaxRequests);
      if (num5 > 0)
        this.ExecutionMaxRequests = num5;
      int num6 = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "FallbackMaxRequests", this.FallbackMaxRequests);
      if (num6 > 0)
        this.FallbackMaxRequests = num6;
      this.FallbackDisabled = CommandPropertiesRegistry.GetValue<bool>((IEnumerable<RegistryItem>) registrySettings, "FallbackDisabled", this.FallbackDisabled);
      this.MetricsHealthSnapshotInterval = CommandPropertiesRegistry.GetValue<TimeSpan>((IEnumerable<RegistryItem>) registrySettings, "MetricsHealthSnapshotInterval", this.MetricsHealthSnapshotInterval);
      int num7 = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "MetricsRollingStatisticalWindowInMilliseconds", this.MetricsRollingStatisticalWindowInMilliseconds);
      int num8 = this.MetricsRollingStatisticalWindowBuckets = CommandPropertiesRegistry.GetValue<int>((IEnumerable<RegistryItem>) registrySettings, "MetricsRollingStatisticalWindowBuckets", this.MetricsRollingStatisticalWindowBuckets);
      if (num7 == 0 || num8 == 0 || num8 % num7 != 0)
        return;
      this.MetricsRollingStatisticalWindowInMilliseconds = num7;
      this.MetricsRollingStatisticalWindowBuckets = num8;
    }

    private static T GetValue<T>(
      IEnumerable<RegistryItem> registrySettings,
      string settingName,
      T defaultValue)
    {
      foreach (RegistryItem registrySetting in registrySettings)
      {
        int num = registrySetting.Path.LastIndexOf('/');
        if (num >= 0 && registrySetting.Path.Length - (num + 1) == settingName.Length && string.Compare(registrySetting.Path, num + 1, settingName, 0, settingName.Length, StringComparison.OrdinalIgnoreCase) == 0)
          return RegistryUtility.FromString<T>(registrySetting.Value, defaultValue);
      }
      return defaultValue;
    }
  }
}
