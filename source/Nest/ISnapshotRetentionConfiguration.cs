// Decompiled with JetBrains decompiler
// Type: Nest.ISnapshotRetentionConfiguration
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [ReadAs(typeof (SnapshotRetentionConfig))]
  [InterfaceDataContract]
  public interface ISnapshotRetentionConfiguration
  {
    [DataMember(Name = "expire_after")]
    Time ExpireAfter { get; set; }

    [DataMember(Name = "min_count")]
    int? MinimumCount { get; set; }

    [DataMember(Name = "max_count")]
    int? MaximumCount { get; set; }
  }
}
