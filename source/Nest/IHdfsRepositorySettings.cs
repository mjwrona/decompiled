// Decompiled with JetBrains decompiler
// Type: Nest.IHdfsRepositorySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IHdfsRepositorySettings : IRepositorySettings
  {
    [DataMember(Name = "chunk_size")]
    string ChunkSize { get; set; }

    [DataMember(Name = "compress")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Compress { get; set; }

    [DataMember(Name = "concurrent_streams")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? ConcurrentStreams { get; set; }

    [DataMember(Name = "conf_location")]
    string ConfigurationLocation { get; set; }

    [IgnoreDataMember]
    Dictionary<string, object> InlineHadoopConfiguration { get; set; }

    [DataMember(Name = "load_defaults")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? LoadDefaults { get; set; }

    [DataMember(Name = "path")]
    string Path { get; set; }

    [DataMember(Name = "uri")]
    string Uri { get; set; }
  }
}
