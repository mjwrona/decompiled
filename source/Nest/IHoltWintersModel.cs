// Decompiled with JetBrains decompiler
// Type: Nest.IHoltWintersModel
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (HoltWintersModel))]
  public interface IHoltWintersModel : IMovingAverageModel
  {
    [DataMember(Name = "alpha")]
    float? Alpha { get; set; }

    [DataMember(Name = "beta")]
    float? Beta { get; set; }

    [DataMember(Name = "gamma")]
    float? Gamma { get; set; }

    [DataMember(Name = "pad")]
    bool? Pad { get; set; }

    [DataMember(Name = "period")]
    int? Period { get; set; }

    [DataMember(Name = "type")]
    HoltWintersType? Type { get; set; }
  }
}
