// Decompiled with JetBrains decompiler
// Type: Nest.IScriptedMetricAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (ScriptedMetricAggregation))]
  public interface IScriptedMetricAggregation : IMetricAggregation, IAggregation
  {
    [DataMember(Name = "combine_script")]
    IScript CombineScript { get; set; }

    [DataMember(Name = "init_script")]
    IScript InitScript { get; set; }

    [DataMember(Name = "map_script")]
    IScript MapScript { get; set; }

    [DataMember(Name = "params")]
    IDictionary<string, object> Params { get; set; }

    [DataMember(Name = "reduce_script")]
    IScript ReduceScript { get; set; }
  }
}
