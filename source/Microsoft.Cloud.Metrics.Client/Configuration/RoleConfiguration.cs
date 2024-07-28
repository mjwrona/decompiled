// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.RoleConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class RoleConfiguration
  {
    public static readonly RoleConfiguration ReadOnly = new RoleConfiguration(nameof (ReadOnly));
    public static readonly RoleConfiguration MetricPublisher = new RoleConfiguration(nameof (MetricPublisher));
    public static readonly RoleConfiguration DashboardEditor = new RoleConfiguration(nameof (DashboardEditor));
    public static readonly RoleConfiguration MonitorEditor = new RoleConfiguration(nameof (MonitorEditor));
    public static readonly RoleConfiguration MetricAndMonitorEditor = new RoleConfiguration(nameof (MetricAndMonitorEditor));
    public static readonly RoleConfiguration ConfigurationEditor = new RoleConfiguration(nameof (ConfigurationEditor));
    public static readonly RoleConfiguration Administrator = new RoleConfiguration(nameof (Administrator));

    public RoleConfiguration(string name) => this.Name = name;

    public string Name { get; }
  }
}
