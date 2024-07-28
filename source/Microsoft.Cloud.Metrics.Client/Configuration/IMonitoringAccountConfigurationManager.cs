// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.IMonitoringAccountConfigurationManager
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public interface IMonitoringAccountConfigurationManager
  {
    Task<IMonitoringAccount> GetAsync(string monitoringAccountName);

    Task CreateAsync(IMonitoringAccount monitoringAccount, string stampHostName);

    Task CreateAsync(
      string newMonitoringAccountName,
      string monitoringAccountToCopyFrom,
      string stampHostName);

    Task SaveAsync(IMonitoringAccount monitoringAccount, bool skipVersionCheck = false);

    Task DeleteAsync(string monitoringAccount);

    Task UnDeleteAsync(string monitoringAccount);

    Task<IReadOnlyList<ConfigurationUpdateResult>> SyncMonitoringAccountConfigurationAsync(
      IMonitoringAccount monitoringAccount,
      bool skipVersionCheck = false);
  }
}
