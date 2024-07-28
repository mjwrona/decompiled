// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.SamplingType
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public struct SamplingType : IEquatable<SamplingType>
  {
    internal static readonly Dictionary<string, SamplingType> BuiltInSamplingTypes;
    private static readonly SamplingType CountSamplingType = new SamplingType(nameof (Count));
    private static readonly SamplingType SumSamplingType = new SamplingType(nameof (Sum));
    private static readonly SamplingType MinSamplingType = new SamplingType(nameof (Min));
    private static readonly SamplingType MaxSamplingType = new SamplingType(nameof (Max));
    private static readonly SamplingType AverageSamplingType = new SamplingType(nameof (Average));
    private static readonly SamplingType NullableAverageSamplingType = new SamplingType(nameof (NullableAverage));
    private static readonly SamplingType RateSamplingType = new SamplingType(nameof (Rate));
    private static readonly SamplingType Percentile50thSamplingType = new SamplingType("50th percentile");
    private static readonly SamplingType Percentile75thSamplingType = new SamplingType("75th percentile");
    private static readonly SamplingType Percentile90thSamplingType = new SamplingType("90th percentile");
    private static readonly SamplingType Percentile95thSamplingType = new SamplingType("95th percentile");
    private static readonly SamplingType Percentile99thSamplingType = new SamplingType("99th percentile");
    private static readonly SamplingType Percentile999thSamplingType = new SamplingType("99.9th percentile");
    private static readonly SamplingType Percentile9999thSamplingType = new SamplingType("99.99th percentile");
    private readonly string name;
    private readonly int hashcode;

    static SamplingType() => SamplingType.BuiltInSamplingTypes = new Dictionary<string, SamplingType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        SamplingType.Count.ToString(),
        SamplingType.Count
      },
      {
        SamplingType.Sum.ToString(),
        SamplingType.Sum
      },
      {
        SamplingType.Min.ToString(),
        SamplingType.Min
      },
      {
        SamplingType.Max.ToString(),
        SamplingType.Max
      },
      {
        SamplingType.Average.ToString(),
        SamplingType.Average
      },
      {
        SamplingType.NullableAverage.ToString(),
        SamplingType.NullableAverage
      },
      {
        SamplingType.Rate.ToString(),
        SamplingType.Rate
      },
      {
        SamplingType.Percentile50th.ToString(),
        SamplingType.Percentile50th
      },
      {
        SamplingType.Percentile75th.ToString(),
        SamplingType.Percentile75th
      },
      {
        SamplingType.Percentile90th.ToString(),
        SamplingType.Percentile90th
      },
      {
        SamplingType.Percentile95th.ToString(),
        SamplingType.Percentile95th
      },
      {
        SamplingType.Percentile99th.ToString(),
        SamplingType.Percentile99th
      },
      {
        SamplingType.Percentile999th.ToString(),
        SamplingType.Percentile999th
      },
      {
        SamplingType.Percentile9999th.ToString(),
        SamplingType.Percentile9999th
      }
    };

    [JsonConstructor]
    public SamplingType(string name)
      : this()
    {
      this.name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentException("Must not be null or white spaces", nameof (name));
      this.hashcode = StringComparer.OrdinalIgnoreCase.GetHashCode(name);
    }

    public static SamplingType Count => SamplingType.CountSamplingType;

    public static SamplingType Sum => SamplingType.SumSamplingType;

    public static SamplingType Min => SamplingType.MinSamplingType;

    public static SamplingType Max => SamplingType.MaxSamplingType;

    public static SamplingType Average => SamplingType.AverageSamplingType;

    public static SamplingType NullableAverage => SamplingType.NullableAverageSamplingType;

    public static SamplingType Rate => SamplingType.RateSamplingType;

    public static SamplingType Percentile50th => SamplingType.Percentile50thSamplingType;

    public static SamplingType Percentile75th => SamplingType.Percentile75thSamplingType;

    public static SamplingType Percentile90th => SamplingType.Percentile90thSamplingType;

    public static SamplingType Percentile95th => SamplingType.Percentile95thSamplingType;

    public static SamplingType Percentile99th => SamplingType.Percentile99thSamplingType;

    public static SamplingType Percentile999th => SamplingType.Percentile999thSamplingType;

    public static SamplingType Percentile9999th => SamplingType.Percentile9999thSamplingType;

    public string Name => this.name;

    public static SamplingType CreateDistinctCountSamplingType(string distinctCountDimensionName) => new SamplingType("DistinctCount_" + distinctCountDimensionName);

    public override string ToString() => this.name;

    public bool Equals(SamplingType other) => string.Equals(this.name, other.name, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => this.hashcode;
  }
}
