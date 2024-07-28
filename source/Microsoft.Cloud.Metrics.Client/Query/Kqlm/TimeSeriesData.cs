// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.Kqlm.TimeSeriesData
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Query.Kqlm
{
  internal sealed class TimeSeriesData : ITimeSeriesData
  {
    public TimeSeriesData(
      IEnumerable<KeyValuePair<string, string>> dimensionValues,
      IEnumerable<KeyValuePair<string, IEnumerable<double>>> samplingTypesData)
    {
      if (dimensionValues == null)
        throw new ArgumentNullException(nameof (dimensionValues));
      if (samplingTypesData == null)
        throw new ArgumentNullException(nameof (samplingTypesData));
      this.DimensionValues = dimensionValues;
      this.SamplingTypesData = samplingTypesData;
    }

    public IEnumerable<KeyValuePair<string, string>> DimensionValues { get; }

    public IEnumerable<KeyValuePair<string, IEnumerable<double>>> SamplingTypesData { get; }
  }
}
