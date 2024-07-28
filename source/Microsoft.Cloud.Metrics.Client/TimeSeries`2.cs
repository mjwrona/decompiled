// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.TimeSeries`2
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.Cloud.Metrics.Client
{
  public sealed class TimeSeries<TId, TValue>
  {
    public TimeSeries(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      int seriesResolutionInMinutes,
      TimeSeriesDefinition<TId> definition,
      List<List<TValue>> values,
      TimeSeriesErrorCode errorCode)
    {
      this.Definition = definition;
      this.StartTimeUtc = startTimeUtc;
      this.EndTimeUtc = endTimeUtc;
      this.SeriesResolutionInMinutes = seriesResolutionInMinutes;
      this.Values = values;
      this.ErrorCode = errorCode;
    }

    public TimeSeriesDefinition<TId> Definition { get; private set; }

    public DateTime StartTimeUtc { get; internal set; }

    public DateTime EndTimeUtc { get; internal set; }

    public TimeSeriesErrorCode ErrorCode { get; internal set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(1)]
    public int SeriesResolutionInMinutes { get; internal set; }

    public IEnumerable<Datapoint<TValue>> Datapoints => this.GetDatapoints(0);

    public IReadOnlyList<IReadOnlyList<TValue>> RawValues => (IReadOnlyList<IReadOnlyList<TValue>>) this.Values;

    internal List<List<TValue>> Values { get; set; }

    public IEnumerable<Datapoint<TValue>> GetDatapoints(SamplingType samplingType)
    {
      int indexOfSamplingTypes = Array.IndexOf<SamplingType>(this.Definition.SamplingTypes, samplingType);
      return indexOfSamplingTypes >= 0 ? this.GetDatapoints(indexOfSamplingTypes) : throw new ArgumentException(string.Format("'{0}' is not found in [{1}])", (object) samplingType, (object) string.Join<SamplingType>(",", (IEnumerable<SamplingType>) this.Definition.SamplingTypes)), nameof (samplingType));
    }

    public IEnumerable<Datapoint<TValue>> GetDatapoints(int indexOfSamplingTypes)
    {
      if (this.Values != null)
      {
        int count = this.Values.Count;
        if (indexOfSamplingTypes >= count)
          throw new ArgumentOutOfRangeException(nameof (indexOfSamplingTypes), string.Format("indexOfSamplingTypes = {0}, numSamplingTypes = {1}.", (object) indexOfSamplingTypes, (object) count));
        if (this.Values[indexOfSamplingTypes] != null)
        {
          for (int i = 0; i < this.Values[indexOfSamplingTypes].Count; ++i)
            yield return new Datapoint<TValue>(this.StartTimeUtc.AddMinutes((double) (i * this.SeriesResolutionInMinutes)), this.Values[indexOfSamplingTypes][i]);
        }
      }
    }
  }
}
