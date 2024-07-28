// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.PercentileConfiguration
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class PercentileConfiguration : IPercentileConfiguration
  {
    public static readonly PercentileConfiguration PercentileEnabled = new PercentileConfiguration(true);
    public static readonly PercentileConfiguration PercentileDisabled = new PercentileConfiguration(false);

    [JsonConstructor]
    internal PercentileConfiguration(bool enabled) => this.Enabled = enabled;

    public bool Enabled { get; }
  }
}
