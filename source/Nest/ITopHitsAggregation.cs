// Decompiled with JetBrains decompiler
// Type: Nest.ITopHitsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (TopHitsAggregation))]
  public interface ITopHitsAggregation : IMetricAggregation, IAggregation
  {
    [DataMember(Name = "docvalue_fields")]
    Fields DocValueFields { get; set; }

    [DataMember(Name = "explain")]
    bool? Explain { get; set; }

    [DataMember(Name = "from")]
    int? From { get; set; }

    [DataMember(Name = "highlight")]
    IHighlight Highlight { get; set; }

    [DataMember(Name = "script_fields")]
    [ReadAs(typeof (Nest.ScriptFields))]
    IScriptFields ScriptFields { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }

    [DataMember(Name = "sort")]
    IList<ISort> Sort { get; set; }

    [DataMember(Name = "_source")]
    Union<bool, ISourceFilter> Source { get; set; }

    [DataMember(Name = "stored_fields")]
    Fields StoredFields { get; set; }

    [DataMember(Name = "track_scores")]
    bool? TrackScores { get; set; }

    [DataMember(Name = "version")]
    bool? Version { get; set; }
  }
}
