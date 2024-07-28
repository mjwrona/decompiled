// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Monitors.IMonitorConfigurationManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Monitors
{
  internal interface IMonitorConfigurationManager
  {
    Task<IReadOnlyList<ConfigurationUpdateResultList>> SyncConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      bool skipVersionCheck = false,
      bool validate = true);

    Task<IReadOnlyList<ConfigurationUpdateResultList>> SyncConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      bool skipVersionCheck = false,
      bool validate = true);

    Task<ConfigurationUpdateResultList> SyncConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      string metricNamespace,
      string metricName,
      bool skipVersionCheck = false,
      bool validate = true);

    Task<ConfigurationUpdateResultList> SyncMonitorV2ConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      bool validate = true);
  }
}
