// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.VirtualSyncOptions
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class VirtualSyncOptions : IVirtualSyncOptions
  {
    [JsonConstructor]
    public VirtualSyncOptions(
      bool syncCertificates = false,
      bool syncUserAndSecurityGroups = false,
      bool syncMetricConfigurations = false,
      bool syncMonitorConfigurations = false,
      bool syncIcmSettings = false,
      bool syncResourceTypes = false)
    {
      this.SyncCertificates = syncCertificates;
      this.SyncUserAndSecurityGroups = syncUserAndSecurityGroups;
      this.SyncMetricConfigurations = syncMetricConfigurations;
      this.SyncMonitorConfigurations = syncMonitorConfigurations;
      this.SyncIcmSettings = syncIcmSettings;
      this.SyncResourceTypes = syncResourceTypes;
    }

    public bool SyncCertificates { get; }

    public bool SyncUserAndSecurityGroups { get; }

    public bool SyncMetricConfigurations { get; }

    public bool SyncMonitorConfigurations { get; }

    public bool SyncIcmSettings { get; }

    public bool SyncResourceTypes { get; }
  }
}
