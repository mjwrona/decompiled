// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.Kqlm.ResultsMetadata
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Query.Kqlm
{
  internal sealed class ResultsMetadata : IResultsMetadata
  {
    public ResultsMetadata(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      TimeSpan dataResolution,
      IEnumerable<string> resultantDimensions,
      IEnumerable<string> resultantSamplingTypes,
      long datapointsCount)
    {
      if (resultantDimensions == null)
        throw new ArgumentNullException(nameof (resultantDimensions));
      if (resultantSamplingTypes == null)
        throw new ArgumentNullException(nameof (resultantSamplingTypes));
      this.StartTimeUtc = startTimeUtc;
      this.EndTimeUtc = endTimeUtc;
      this.DataResolution = dataResolution;
      this.ResultantDimensions = resultantDimensions;
      this.ResultantSamplingTypes = resultantSamplingTypes;
      this.DatapointsCount = datapointsCount;
    }

    public DateTime StartTimeUtc { get; }

    public DateTime EndTimeUtc { get; }

    public TimeSpan DataResolution { get; }

    public IEnumerable<string> ResultantDimensions { get; }

    public IEnumerable<string> ResultantSamplingTypes { get; }

    public long DatapointsCount { get; }
  }
}
