// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.ConfigurationUpdateResult
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class ConfigurationUpdateResult : IConfigurationUpdateResult
  {
    [JsonConstructor]
    public ConfigurationUpdateResult(string monitoringAccount, bool success, string message)
    {
      this.MonitoringAccount = monitoringAccount;
      this.Success = success;
      this.Message = message;
    }

    public string MonitoringAccount { get; }

    public bool Success { get; set; }

    public string Message { get; set; }
  }
}
