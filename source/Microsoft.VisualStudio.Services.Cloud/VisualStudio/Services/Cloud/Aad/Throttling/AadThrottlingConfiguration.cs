// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Aad.Throttling.AadThrottlingConfiguration
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.Aad.Throttling
{
  internal class AadThrottlingConfiguration
  {
    internal const string AadThrottlingServiceConfigurationRoot = "/Configuration/AadThrottlingService";

    private AadThrottlingConfiguration(
      IVssRequestContext requestcontext,
      AadServiceType serviceType)
    {
      IVssRegistryService service = requestcontext.GetService<IVssRegistryService>();
      this.DefaultThrottlingTimeSpan = service.GetValue<TimeSpan>(requestcontext, new RegistryQuery(AadThrottlingConfiguration.GetDefaultThrottlingRegistryKey(serviceType)), TimeSpan.FromSeconds(10.0));
      this.MaxThrottlingTimeSpan = service.GetValue<TimeSpan>(requestcontext, new RegistryQuery(AadThrottlingConfiguration.GetMaxThrottlingRegistryKey(serviceType)), TimeSpan.FromHours(1.0));
      this.GlobalThrottlingLimit = service.GetValue<int>(requestcontext, new RegistryQuery(AadThrottlingConfiguration.GetGlobalThrottlingLimitRegistryKey(serviceType)), 100);
      this.IsEnabled = service.GetValue<bool>(requestcontext, new RegistryQuery(AadThrottlingConfiguration.GetIsEnabledRegistryKey(serviceType)), true);
      this.CleanupInterval = service.GetValue<TimeSpan>(requestcontext, new RegistryQuery(AadThrottlingConfiguration.GetCleanupIntervalRegistryKey(serviceType)), TimeSpan.FromHours(1.0));
      this.ThrottlingBlackList = (IReadOnlyDictionary<string, TimeSpan>) JsonConvert.DeserializeObject<Dictionary<string, TimeSpan>>(service.GetValue(requestcontext, new RegistryQuery(AadThrottlingConfiguration.GetThrottlingBlackListKey(serviceType)), "{}"));
    }

    internal IReadOnlyDictionary<string, TimeSpan> ThrottlingBlackList { get; private set; }

    public static AadThrottlingConfiguration GetAadThrottlingConfiguration(
      IVssRequestContext requestcontext,
      AadServiceType serviceType)
    {
      return new AadThrottlingConfiguration(requestcontext, serviceType);
    }

    public TimeSpan DefaultThrottlingTimeSpan { get; private set; }

    public TimeSpan MaxThrottlingTimeSpan { get; private set; }

    public int GlobalThrottlingLimit { get; private set; }

    public bool IsEnabled { get; private set; }

    public TimeSpan CleanupInterval { get; private set; }

    internal static string GetDefaultThrottlingRegistryKey(AadServiceType serviceType) => "/Configuration/AadThrottlingService" + "/" + (object) serviceType + "/DefaultThrottling";

    internal static string GetMaxThrottlingRegistryKey(AadServiceType serviceType) => "/Configuration/AadThrottlingService" + "/" + (object) serviceType + "/MaxThrottling";

    internal static string GetGlobalThrottlingLimitRegistryKey(AadServiceType serviceType) => "/Configuration/AadThrottlingService" + "/" + (object) serviceType + "/GlobalThrottlingLimit";

    internal static string GetIsEnabledRegistryKey(AadServiceType serviceType) => "/Configuration/AadThrottlingService" + "/" + (object) serviceType + "/IsEnabled";

    internal static string GetCleanupIntervalRegistryKey(AadServiceType serviceType) => "/Configuration/AadThrottlingService" + "/" + (object) serviceType + "/CleanupInterval";

    internal static string GetMaxThrottlingLimitRegistryKey(AadServiceType serviceType) => "/Configuration/AadThrottlingService" + "/" + (object) serviceType + "/MaxThrottlingLimit";

    internal static string GetThrottlingBlackListKey(AadServiceType serviceType) => "/Configuration/AadThrottlingService" + "/" + (object) serviceType + "/ThrottlingBlackList";
  }
}
