// Decompiled with JetBrains decompiler
// Type: Nest.ISpanQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SpanQuery))]
  public interface ISpanQuery : IQuery
  {
    [DataMember(Name = "span_containing")]
    ISpanContainingQuery SpanContaining { get; set; }

    [DataMember(Name = "field_masking_span")]
    ISpanFieldMaskingQuery SpanFieldMasking { get; set; }

    [DataMember(Name = "span_first")]
    ISpanFirstQuery SpanFirst { get; set; }

    [DataMember(Name = "span_gap")]
    ISpanGapQuery SpanGap { get; set; }

    [DataMember(Name = "span_multi")]
    ISpanMultiTermQuery SpanMultiTerm { get; set; }

    [DataMember(Name = "span_near")]
    ISpanNearQuery SpanNear { get; set; }

    [DataMember(Name = "span_not")]
    ISpanNotQuery SpanNot { get; set; }

    [DataMember(Name = "span_or")]
    ISpanOrQuery SpanOr { get; set; }

    [DataMember(Name = "span_term")]
    ISpanTermQuery SpanTerm { get; set; }

    [DataMember(Name = "span_within")]
    ISpanWithinQuery SpanWithin { get; set; }

    void Accept(IQueryVisitor visitor);
  }
}
