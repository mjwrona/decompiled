// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.ConfigFileValidator
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Configuration;
using Microsoft.Cloud.Metrics.Client.Logging;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  internal static class ConfigFileValidator
  {
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (ConfigFileValidator));

    internal static bool ValidateRawMetricConfigFromFile(
      IRawMetricConfiguration metricConfigFromFile)
    {
      if (metricConfigFromFile.Preaggregations != null && metricConfigFromFile.Preaggregations.Any<IPreaggregation>())
        return true;
      Logger.Log(LoggerLevel.Error, ConfigFileValidator.LogId, nameof (ValidateRawMetricConfigFromFile), "Preaggregations property is not set or empty so it seems to be an invalid raw metric config.");
      return false;
    }

    internal static bool ValidateCompositeMetricConfigFromFile(
      ICompositeMetricConfiguration metricConfigFromFile)
    {
      if (metricConfigFromFile.MetricSources != null && metricConfigFromFile.MetricSources.Any<CompositeMetricSource>())
        return true;
      Logger.Log(LoggerLevel.Error, ConfigFileValidator.LogId, nameof (ValidateCompositeMetricConfigFromFile), "MetricSources property is not set or empty so it seems to be an invalid composite metric config.");
      return false;
    }

    internal static bool ValidateMetricConfigFromFile(IMetricConfiguration metricConfigFromFile) => !(metricConfigFromFile is RawMetricConfiguration) ? ConfigFileValidator.ValidateCompositeMetricConfigFromFile((ICompositeMetricConfiguration) metricConfigFromFile) : ConfigFileValidator.ValidateRawMetricConfigFromFile((IRawMetricConfiguration) metricConfigFromFile);
  }
}
