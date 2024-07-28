// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GraphClientConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  internal static class GraphClientConstants
  {
    internal const string Area = "VisualStudio.Services.Aad";
    internal const string Layer = "Graph";
    internal const int MaxRequestsPerBatch = 5;

    internal static class CircuitBreaker
    {
      internal const string GroupKey = "Aad.Graph";
      internal const bool Disabled = false;
      internal const int ErrorThresholdPercentage = 90;
      internal const int RequestVolumeThreshold = 30;
      internal static readonly TimeSpan MinBackoff = TimeSpan.FromSeconds(1.0);
      internal static readonly TimeSpan MaxBackoff = TimeSpan.FromSeconds(30.0);
      internal static readonly TimeSpan DeltaBackoff = TimeSpan.FromSeconds(1.0);
      internal static readonly TimeSpan Long10SecExecutionTimeout = TimeSpan.FromSeconds(10.0);
      internal static readonly TimeSpan Short5SecExecutionTimeout = TimeSpan.FromSeconds(5.0);
      internal static readonly TimeSpan ExtraLong15SecExecutionTimeout = TimeSpan.FromSeconds(15.0);
      internal const bool DefaultFallbackDisabled = true;
      internal static readonly TimeSpan MetricsHealthSnapshotInterval = TimeSpan.FromSeconds(1.0);
      internal static readonly TimeSpan MetricsRollingStatisticalWindow = TimeSpan.FromSeconds(30.0);
      internal const int MetricsRollingStatisticalWindowBuckets = 6;
    }
  }
}
