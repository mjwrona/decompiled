// Decompiled with JetBrains decompiler
// Type: Nest.IBulkUpdateOperation`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IBulkUpdateOperation<TDocument, TPartialDocument> : IBulkOperation
    where TDocument : class
    where TPartialDocument : class
  {
    TPartialDocument Doc { get; set; }

    bool? DocAsUpsert { get; set; }

    TDocument IdFrom { get; set; }

    IScript Script { get; set; }

    bool? ScriptedUpsert { get; set; }

    TDocument Upsert { get; set; }

    long? IfSequenceNumber { get; set; }

    long? IfPrimaryTerm { get; set; }

    [DataMember(Name = "_source")]
    Union<bool, ISourceFilter> Source { get; set; }
  }
}
