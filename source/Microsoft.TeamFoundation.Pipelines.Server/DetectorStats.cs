// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DetectorStats
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class DetectorStats
  {
    public string DetectorName { get; }

    public BuildFrameworkDetectionType DetectionType { get; }

    public int TotalFileCount { get; }

    public int FileCacheHits { get; }

    public int FileCacheMisses { get; }

    public int FilesReadCount { get; }

    public TimeSpan TotalAnalysisTime { get; }

    public DetectorStats(
      string name,
      BuildFrameworkDetectionType type,
      int totalFileCount,
      int filesReadCount,
      int cacheHits = 0,
      int cacheMisses = 0,
      TimeSpan totalAnalysisTime = default (TimeSpan))
    {
      this.DetectorName = name;
      this.DetectionType = type;
      this.TotalFileCount = totalFileCount;
      this.FilesReadCount = filesReadCount;
      this.FileCacheHits = cacheHits;
      this.FileCacheMisses = cacheMisses;
      this.TotalAnalysisTime = totalAnalysisTime;
    }
  }
}
