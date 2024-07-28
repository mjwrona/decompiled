// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.StatisticsBuilder
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  internal class StatisticsBuilder
  {
    public string Name { get; }

    public BuildFrameworkDetectionType DetectionType { get; }

    public int TotalFileCount { get; set; }

    public int FilesReadCount { get; set; }

    public int FileCacheHits { get; set; }

    public int FileCacheMisses { get; set; }

    public TimeSpan TotalAnalysisTime { get; set; }

    public StatisticsBuilder(string name, BuildFrameworkDetectionType type, int totalFileCount)
    {
      this.Name = name;
      this.DetectionType = type;
      this.TotalFileCount = totalFileCount;
      this.FilesReadCount = 0;
      this.TotalAnalysisTime = TimeSpan.FromMilliseconds(0.0);
    }

    public StatisticsBuilder IncrementFileReadCount()
    {
      ++this.FilesReadCount;
      return this;
    }

    public StatisticsBuilder AddAnalysisTime(TimeSpan analysisTime)
    {
      this.TotalAnalysisTime += analysisTime;
      return this;
    }

    public StatisticsTimer MeasureAnalysisTime() => new StatisticsTimer(this);

    public StatisticsBuilder IncrementFileCacheHits()
    {
      ++this.FileCacheHits;
      return this;
    }

    public StatisticsBuilder IncrementFileCacheMisses()
    {
      ++this.FileCacheMisses;
      return this;
    }

    public DetectorStats Build() => new DetectorStats(this.Name, this.DetectionType, this.TotalFileCount, this.FilesReadCount, this.FileCacheHits, this.FileCacheMisses, this.TotalAnalysisTime);
  }
}
