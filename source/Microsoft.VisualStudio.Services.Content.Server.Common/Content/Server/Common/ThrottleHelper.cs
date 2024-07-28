// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ThrottleHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  internal static class ThrottleHelper
  {
    private const string ThrottlingUpperCpuThresholdKey = "ThrottlingUpperCpuThreshold";
    private const string ThrottlingLowerCpuThresholdKey = "ThrottlingLowerCpuThreshold";
    private const string ThrottlingUpperContentLengthThresholdKey = "ThrottlingUpperContentLengthThreshold";
    private const string ThrottleCpuAggregationWindowInSecondsKey = "ThrottleCpuAggregationWindowInSeconds";

    private static string RegistryKeyString(ThrottleHelper.RegistryKey key)
    {
      switch (key)
      {
        case ThrottleHelper.RegistryKey.CpuPercentageUpperThreshold:
          return "ThrottlingUpperCpuThreshold";
        case ThrottleHelper.RegistryKey.CpuPercentageLowerThreshold:
          return "ThrottlingLowerCpuThreshold";
        case ThrottleHelper.RegistryKey.ContentLengthUpperThreshold:
          return "ThrottlingUpperContentLengthThreshold";
        case ThrottleHelper.RegistryKey.CpuAggregationWindowInSeconds:
          return "ThrottleCpuAggregationWindowInSeconds";
        default:
          throw new InvalidCastException(string.Format("Invalid ${0} value: {1}.", (object) "RegistryKey", (object) key));
      }
    }

    internal static T GetThrottlingParameter<T>(
      IVssRequestContext requestContext,
      string registryRootPath,
      ThrottleHelper.RegistryKey registryKey,
      string controllerLabel,
      T defaultValue)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<T>(requestContext, (RegistryQuery) (registryRootPath + "/" + ThrottleHelper.RegistryKeyString(registryKey) + "/" + controllerLabel), true, defaultValue);
    }

    internal enum RegistryKey
    {
      CpuPercentageUpperThreshold,
      CpuPercentageLowerThreshold,
      ContentLengthUpperThreshold,
      CpuAggregationWindowInSeconds,
    }
  }
}
