// Decompiled with JetBrains decompiler
// Type: Nest.IHit`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (Hit<>))]
  public interface IHit<out TDocument> : IHitMetadata<TDocument> where TDocument : class
  {
    [DataMember(Name = "_explanation")]
    Explanation Explanation { get; }

    [DataMember(Name = "fields")]
    FieldValues Fields { get; }

    [DataMember(Name = "highlight")]
    IReadOnlyDictionary<string, IReadOnlyCollection<string>> Highlight { get; }

    [DataMember(Name = "inner_hits")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, InnerHitsResult>))]
    IReadOnlyDictionary<string, InnerHitsResult> InnerHits { get; }

    [DataMember(Name = "_nested")]
    NestedIdentity Nested { get; }

    [DataMember(Name = "matched_queries")]
    IReadOnlyCollection<string> MatchedQueries { get; }

    [DataMember(Name = "_score")]
    double? Score { get; }

    [DataMember(Name = "sort")]
    IReadOnlyCollection<object> Sorts { get; }
  }
}
