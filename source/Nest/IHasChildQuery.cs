// Decompiled with JetBrains decompiler
// Type: Nest.IHasChildQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (HasChildQueryDescriptor<object>))]
  public interface IHasChildQuery : IQuery
  {
    [DataMember(Name = "ignore_unmapped")]
    bool? IgnoreUnmapped { get; set; }

    [DataMember(Name = "inner_hits")]
    IInnerHits InnerHits { get; set; }

    [DataMember(Name = "max_children")]
    int? MaxChildren { get; set; }

    [DataMember(Name = "min_children")]
    int? MinChildren { get; set; }

    [DataMember(Name = "query")]
    QueryContainer Query { get; set; }

    [DataMember(Name = "score_mode")]
    ChildScoreMode? ScoreMode { get; set; }

    [DataMember(Name = "type")]
    RelationName Type { get; set; }
  }
}
