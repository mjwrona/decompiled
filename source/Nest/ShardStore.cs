// Decompiled with JetBrains decompiler
// Type: Nest.ShardStore
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [JsonFormatter(typeof (ShardStoreFormatter))]
  public class ShardStore
  {
    [DataMember(Name = "allocation")]
    public ShardStoreAllocation Allocation { get; internal set; }

    [DataMember(Name = "allocation_id")]
    public string AllocationId { get; internal set; }

    [DataMember(Name = "attributes")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, object>))]
    public IReadOnlyDictionary<string, object> Attributes { get; internal set; } = EmptyReadOnly<string, object>.Dictionary;

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "legacy_version")]
    public long? LegacyVersion { get; internal set; }

    [DataMember(Name = "name")]
    public string Name { get; internal set; }

    [DataMember(Name = "store_exception")]
    public ShardStoreException StoreException { get; internal set; }

    [DataMember(Name = "transport_address")]
    public string TransportAddress { get; internal set; }
  }
}
