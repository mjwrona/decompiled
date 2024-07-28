// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.TimeSeriesDefinition`1
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics;
using Microsoft.Cloud.Metrics.Client.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Cloud.Metrics.Client
{
  public sealed class TimeSeriesDefinition<TId>
  {
    private KeyValuePair<string, string>[] dimensionCombination;
    private TId id;
    private DateTime startTimeUtc;
    private DateTime endTimeUtc;

    public TimeSeriesDefinition(
      TId id,
      IEnumerable<KeyValuePair<string, string>> dimensionCombination)
    {
      this.id = id;
      this.dimensionCombination = dimensionCombination != null ? dimensionCombination.ToArray<KeyValuePair<string, string>>() : (KeyValuePair<string, string>[]) null;
    }

    public TimeSeriesDefinition(
      TId id,
      params KeyValuePair<string, string>[] dimensionCombination)
    {
      this.id = id;
      this.dimensionCombination = dimensionCombination;
    }

    [JsonConstructor]
    public TimeSeriesDefinition(
      TId id,
      KeyValuePair<string, string>[] dimensionCombination,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType[] samplingTypes,
      int seriesResolutionInMinutes = 1,
      AggregationType aggregationType = AggregationType.None,
      bool zeroAsNoValueSentinel = false)
    {
      this.id = id;
      this.dimensionCombination = dimensionCombination;
      this.StartTimeUtc = startTimeUtc;
      this.EndTimeUtc = endTimeUtc;
      this.SamplingTypes = samplingTypes;
      this.SeriesResolutionInMinutes = seriesResolutionInMinutes;
      this.AggregationType = aggregationType;
      this.ZeroAsNoValueSentinel = zeroAsNoValueSentinel;
      this.ValidateAndNormalize();
    }

    public TimeSeriesDefinition(
      TId id,
      IEnumerable<KeyValuePair<string, string>> dimensionCombination,
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      SamplingType[] samplingTypes,
      int seriesResolutionInMinutes = 1,
      AggregationType aggregationType = AggregationType.None,
      bool zeroAsNoValueSentinel = false)
      : this(id, dimensionCombination != null ? dimensionCombination.ToArray<KeyValuePair<string, string>>() : (KeyValuePair<string, string>[]) null, startTimeUtc, endTimeUtc, samplingTypes, seriesResolutionInMinutes, aggregationType, zeroAsNoValueSentinel)
    {
    }

    public TId Id
    {
      get => this.id;
      internal set => this.id = value;
    }

    public DateTime StartTimeUtc
    {
      get => this.startTimeUtc;
      internal set
      {
        this.startTimeUtc = value;
        TimeSeriesDefinition<TId>.NormalizeTime(ref this.startTimeUtc);
      }
    }

    public DateTime EndTimeUtc
    {
      get => this.endTimeUtc;
      internal set
      {
        this.endTimeUtc = value;
        TimeSeriesDefinition<TId>.NormalizeTime(ref this.endTimeUtc);
      }
    }

    public SamplingType[] SamplingTypes { get; internal set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(1)]
    public int SeriesResolutionInMinutes { get; internal set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    [DefaultValue(AggregationType.None)]
    public AggregationType AggregationType { get; internal set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool ZeroAsNoValueSentinel { get; set; }

    public IReadOnlyList<KeyValuePair<string, string>> DimensionCombination
    {
      get => (IReadOnlyList<KeyValuePair<string, string>>) this.dimensionCombination;
      internal set => this.dimensionCombination = (KeyValuePair<string, string>[]) value;
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    private static void NormalizeTime(ref DateTime timestamp) => timestamp = new DateTime(timestamp.Ticks / 600000000L * 600000000L);

    private void ValidateAndNormalize()
    {
      if (this.StartTimeUtc > this.EndTimeUtc)
        throw new ArgumentException(string.Format("startTimeUtc [{0}] must be <= endTimeUtc [{1}]", (object) this.StartTimeUtc, (object) this.EndTimeUtc));
      if (this.SamplingTypes == null || this.SamplingTypes.Length == 0)
        throw new ArgumentException("samplingTypes cannot be null or empty");
      if (this.SeriesResolutionInMinutes < 1)
        throw new ArgumentException(string.Format("seriesResolutionInMinutes must be >= {0}", (object) 1));
      TimeSeriesDefinition<TId>.NormalizeTime(ref this.startTimeUtc);
      TimeSeriesDefinition<TId>.NormalizeTime(ref this.endTimeUtc);
    }
  }
}
