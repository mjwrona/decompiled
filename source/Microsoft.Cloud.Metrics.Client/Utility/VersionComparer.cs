// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Utility.VersionComparer
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Configuration;
using Microsoft.Cloud.Metrics.Client.Logging;

namespace Microsoft.Cloud.Metrics.Client.Utility
{
  internal static class VersionComparer
  {
    private static readonly object LogId = Logger.CreateCustomLogId(nameof (VersionComparer));

    internal static int CompareMetricsVersionWithServer(
      IMetricConfiguration metricConfigFromFile,
      IMetricConfiguration metricConfigurationOnServer)
    {
      if (metricConfigurationOnServer == null)
        return 1;
      if ((int) metricConfigFromFile.Version == (int) metricConfigurationOnServer.Version)
      {
        Logger.Log(LoggerLevel.Warning, VersionComparer.LogId, nameof (CompareMetricsVersionWithServer), "The version in the file is the same as the one on the server. Skip.");
        return 0;
      }
      if (metricConfigFromFile.Version >= metricConfigurationOnServer.Version)
        return 1;
      Logger.Log(LoggerLevel.Warning, VersionComparer.LogId, nameof (CompareMetricsVersionWithServer), string.Format("The version {0} in the file is less than the one {1} on the server! Please download the latest version first. Skip.", (object) metricConfigFromFile.Version, (object) metricConfigurationOnServer.Version));
      return -1;
    }
  }
}
