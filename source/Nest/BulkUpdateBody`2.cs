// Decompiled with JetBrains decompiler
// Type: Nest.BulkUpdateBody`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  internal class BulkUpdateBody<TDocument, TPartialUpdate>
    where TDocument : class
    where TPartialUpdate : class
  {
    [DataMember(Name = "doc_as_upsert")]
    public bool? DocAsUpsert { get; set; }

    [DataMember(Name = "doc")]
    [JsonFormatter(typeof (CollapsedSourceFormatter<>))]
    internal TPartialUpdate PartialUpdate { get; set; }

    [DataMember(Name = "script")]
    internal IScript Script { get; set; }

    [DataMember(Name = "scripted_upsert")]
    internal bool? ScriptedUpsert { get; set; }

    [DataMember(Name = "upsert")]
    [JsonFormatter(typeof (CollapsedSourceFormatter<>))]
    internal TDocument Upsert { get; set; }

    [DataMember(Name = "if_seq_no")]
    internal long? IfSequenceNumber { get; set; }

    [DataMember(Name = "if_primary_term")]
    internal long? IfPrimaryTerm { get; set; }

    [DataMember(Name = "_source")]
    internal Union<bool, ISourceFilter> Source { get; set; }
  }
}
