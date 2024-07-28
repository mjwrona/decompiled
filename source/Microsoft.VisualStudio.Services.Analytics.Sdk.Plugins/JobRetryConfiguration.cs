// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.JobRetryConfiguration
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class JobRetryConfiguration
  {
    public int MinimumRetryIntervalSeconds { get; set; } = 1;

    public int DeltaBaseSeconds { get; set; } = 10;

    public int MaximumRetryIntervalSeconds { get; set; } = 1800;

    public int MaxRetry { get; set; } = 5;

    public JobRetryConfiguration()
    {
    }

    public JobRetryConfiguration(JobRetryConfiguration other)
    {
      this.MinimumRetryIntervalSeconds = other.MinimumRetryIntervalSeconds;
      this.DeltaBaseSeconds = other.DeltaBaseSeconds;
      this.MaximumRetryIntervalSeconds = other.MaximumRetryIntervalSeconds;
      this.MaxRetry = other.MaxRetry;
    }
  }
}
