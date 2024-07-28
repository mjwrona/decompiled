// Decompiled with JetBrains decompiler
// Type: Nest.IS3RepositorySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IS3RepositorySettings : IRepositorySettings
  {
    [DataMember(Name = "base_path")]
    string BasePath { get; set; }

    [DataMember(Name = "bucket")]
    string Bucket { get; set; }

    [DataMember(Name = "buffer_size")]
    string BufferSize { get; set; }

    [DataMember(Name = "canned_acl")]
    string CannedAcl { get; set; }

    [DataMember(Name = "chunk_size")]
    string ChunkSize { get; set; }

    [DataMember(Name = "client")]
    string Client { get; set; }

    [DataMember(Name = "compress")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Compress { get; set; }

    [DataMember(Name = "server_side_encryption")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? ServerSideEncryption { get; set; }

    [DataMember(Name = "storage_class")]
    string StorageClass { get; set; }

    [DataMember(Name = "path_style_access")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? PathStyleAccess { get; set; }

    [DataMember(Name = "disable_chunked_encoding")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? DisableChunkedEncoding { get; set; }

    [DataMember(Name = "readonly")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? ReadOnly { get; set; }

    [DataMember(Name = "max_restore_bytes_per_sec")]
    string MaxRestoreBytesPerSecond { get; set; }

    [DataMember(Name = "max_snapshot_bytes_per_sec")]
    string MaxSnapshotBytesPerSecond { get; set; }
  }
}
