// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NuGetSearchTelemetryCollector
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NuGetSearchTelemetryCollector
  {
    public NuGetSearchTelemetryCollector.TimingSet OverallTime { get; } = new NuGetSearchTelemetryCollector.TimingSet();

    public NuGetSearchTelemetryCollector.TimingSet GetNameList { get; } = new NuGetSearchTelemetryCollector.TimingSet();

    public NuGetSearchTelemetryCollector.CountSet PackageNameEntries { get; } = new NuGetSearchTelemetryCollector.CountSet();

    public NuGetSearchTelemetryCollector.TimingSet GetPackageMetadata { get; } = new NuGetSearchTelemetryCollector.TimingSet();

    public NuGetSearchTelemetryCollector.CountSet PackageMetadataEntries { get; } = new NuGetSearchTelemetryCollector.CountSet();

    public NuGetSearchTelemetryCollector.TimingSet FilterVersionsStage1 { get; } = new NuGetSearchTelemetryCollector.TimingSet();

    public NuGetSearchTelemetryCollector.CountSet VersionsAfterFilterStage1 { get; } = new NuGetSearchTelemetryCollector.CountSet();

    public NuGetSearchTelemetryCollector.TimingSet FilterVersionsStage2 { get; } = new NuGetSearchTelemetryCollector.TimingSet();

    public NuGetSearchTelemetryCollector.CountSet VersionsAfterFilterStage2 { get; } = new NuGetSearchTelemetryCollector.CountSet();

    public NuGetSearchTelemetryCollector.CountSet SkippedByCountVersions { get; } = new NuGetSearchTelemetryCollector.CountSet();

    public NuGetSearchTelemetryCollector.TimingSet GetUpstreamNameList { get; } = new NuGetSearchTelemetryCollector.TimingSet();

    public NuGetSearchTelemetryCollector.CountSet SkippedByZeroCountPackages { get; } = new NuGetSearchTelemetryCollector.CountSet();

    public IVersionCountsImplementationMetrics VersionCountsMetrics { get; set; }

    public class TimingSet
    {
      private readonly List<NuGetSearchTelemetryCollector.Timer> entries = new List<NuGetSearchTelemetryCollector.Timer>();

      public TimeSpan Total => this.entries.Aggregate<NuGetSearchTelemetryCollector.Timer, TimeSpan>(TimeSpan.Zero, (Func<TimeSpan, NuGetSearchTelemetryCollector.Timer, TimeSpan>) ((x, y) => x + y.Elapsed));

      public int Count => this.entries.Count;

      public NuGetSearchTelemetryCollector.Timer BeginTiming()
      {
        NuGetSearchTelemetryCollector.Timer timer = new NuGetSearchTelemetryCollector.Timer();
        this.entries.Add(timer);
        return timer;
      }
    }

    public class Timer : IDisposable
    {
      private readonly Stopwatch stopwatch = Stopwatch.StartNew();

      public TimeSpan Elapsed => this.stopwatch.Elapsed;

      void IDisposable.Dispose() => this.Stop();

      private void Stop() => this.stopwatch.Stop();
    }

    public class CountSet
    {
      private readonly List<int> entries = new List<int>();

      public int Count => this.entries.Count;

      public int Total => this.entries.Sum();

      public void Add(int count) => this.entries.Add(count);
    }
  }
}
