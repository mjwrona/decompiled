// Decompiled with JetBrains decompiler
// Type: Nest.ISpanNotQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SpanNotQuery))]
  public interface ISpanNotQuery : ISpanSubQuery, IQuery
  {
    [DataMember(Name = "dist")]
    int? Dist { get; set; }

    [DataMember(Name = "exclude")]
    ISpanQuery Exclude { get; set; }

    [DataMember(Name = "include")]
    ISpanQuery Include { get; set; }

    [DataMember(Name = "post")]
    int? Post { get; set; }

    [DataMember(Name = "pre")]
    int? Pre { get; set; }
  }
}
