// Decompiled with JetBrains decompiler
// Type: Nest.IAzureRepositorySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IAzureRepositorySettings : IRepositorySettings
  {
    [DataMember(Name = "base_path")]
    string BasePath { get; set; }

    [DataMember(Name = "chunk_size")]
    string ChunkSize { get; set; }

    [DataMember(Name = "compress")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Compress { get; set; }

    [DataMember(Name = "container")]
    string Container { get; set; }
  }
}
