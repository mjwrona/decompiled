// Decompiled with JetBrains decompiler
// Type: Nest.IMultiGetOperation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (MultiGetOperationDescriptor<object>))]
  public interface IMultiGetOperation
  {
    bool CanBeFlattened { get; }

    Type ClrType { get; }

    [DataMember(Name = "_id")]
    Id Id { get; set; }

    [DataMember(Name = "_index")]
    IndexName Index { get; set; }

    [DataMember(Name = "routing")]
    string Routing { get; set; }

    [DataMember(Name = "_source")]
    Union<bool, ISourceFilter> Source { get; set; }

    [DataMember(Name = "stored_fields")]
    Fields StoredFields { get; set; }

    [DataMember(Name = "version")]
    long? Version { get; set; }

    [DataMember(Name = "version_type")]
    Elasticsearch.Net.VersionType? VersionType { get; set; }
  }
}
