// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.Kqlm.TimeSeriesSet
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Query.Kqlm
{
  internal sealed class TimeSeriesSet : ITimeSeriesSet
  {
    public TimeSeriesSet(
      IResultsMetadata resultsMetadata,
      IEnumerable<ITimeSeriesData> timeSeriesData)
    {
      if (resultsMetadata == null)
        throw new ArgumentNullException(nameof (resultsMetadata));
      if (timeSeriesData == null)
        throw new ArgumentNullException(nameof (timeSeriesData));
      this.ResultsMetadata = resultsMetadata;
      this.TimeSeriesData = timeSeriesData;
    }

    public IResultsMetadata ResultsMetadata { get; }

    public IEnumerable<ITimeSeriesData> TimeSeriesData { get; }
  }
}
