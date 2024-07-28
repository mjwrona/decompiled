// Decompiled with JetBrains decompiler
// Type: Nest.StandardDeviationBounds
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class StandardDeviationBounds
  {
    [DataMember(Name = "lower")]
    public double? Lower { get; set; }

    [DataMember(Name = "upper")]
    public double? Upper { get; set; }

    [DataMember(Name = "lower_population")]
    public double? LowerPopulation { get; set; }

    [DataMember(Name = "upper_population")]
    public double? UpperPopulation { get; set; }

    [DataMember(Name = "lower_sampling")]
    [JsonFormatter(typeof (NullableStringDoubleFormatter))]
    public double? LowerSampling { get; set; }

    [DataMember(Name = "upper_sampling")]
    [JsonFormatter(typeof (NullableStringDoubleFormatter))]
    public double? UpperSampling { get; set; }
  }
}
