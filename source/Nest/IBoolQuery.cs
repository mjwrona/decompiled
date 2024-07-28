// Decompiled with JetBrains decompiler
// Type: Nest.IBoolQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (BoolQuery))]
  public interface IBoolQuery : IQuery
  {
    [DataMember(Name = "filter")]
    IEnumerable<QueryContainer> Filter { get; set; }

    [IgnoreDataMember]
    bool Locked { get; }

    [DataMember(Name = "minimum_should_match")]
    MinimumShouldMatch MinimumShouldMatch { get; set; }

    [DataMember(Name = "must")]
    IEnumerable<QueryContainer> Must { get; set; }

    [DataMember(Name = "must_not")]
    IEnumerable<QueryContainer> MustNot { get; set; }

    [DataMember(Name = "should")]
    IEnumerable<QueryContainer> Should { get; set; }

    bool ShouldSerializeShould();

    bool ShouldSerializeMust();

    bool ShouldSerializeMustNot();

    bool ShouldSerializeFilter();
  }
}
