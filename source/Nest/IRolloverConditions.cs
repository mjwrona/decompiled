// Decompiled with JetBrains decompiler
// Type: Nest.IRolloverConditions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (RolloverConditions))]
  public interface IRolloverConditions
  {
    [DataMember(Name = "max_age")]
    Time MaxAge { get; set; }

    [DataMember(Name = "max_docs")]
    long? MaxDocs { get; set; }

    [DataMember(Name = "max_size")]
    string MaxSize { get; set; }

    [DataMember(Name = "max_primary_shard_size")]
    string MaxPrimaryShardSize { get; set; }
  }
}
