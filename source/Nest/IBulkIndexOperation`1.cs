// Decompiled with JetBrains decompiler
// Type: Nest.IBulkIndexOperation`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IBulkIndexOperation<T> : IBulkOperation
  {
    [JsonFormatter(typeof (SourceWriteFormatter<>))]
    T Document { get; set; }

    [DataMember(Name = "_percolate")]
    string Percolate { get; set; }

    [DataMember(Name = "pipeline")]
    string Pipeline { get; set; }

    [DataMember(Name = "if_seq_no")]
    long? IfSequenceNumber { get; set; }

    [DataMember(Name = "if_primary_term")]
    long? IfPrimaryTerm { get; set; }
  }
}
