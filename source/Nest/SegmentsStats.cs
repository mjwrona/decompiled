// Decompiled with JetBrains decompiler
// Type: Nest.SegmentsStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SegmentsStats
  {
    [DataMember(Name = "count")]
    public long Count { get; set; }

    [DataMember(Name = "doc_values_memory_in_bytes")]
    public long DocValuesMemoryInBytes { get; set; }

    [DataMember(Name = "fixed_bit_set_memory_in_bytes")]
    public long FixedBitSetMemoryInBytes { get; set; }

    [DataMember(Name = "index_writer_max_memory_in_bytes")]
    public long IndexWriterMaxMemoryInBytes { get; set; }

    [DataMember(Name = "index_writer_memory_in_bytes")]
    public long IndexWriterMemoryInBytes { get; set; }

    [DataMember(Name = "max_unsafe_auto_id_timestamp")]
    public long MaximumUnsafeAutoIdTimestamp { get; set; }

    [DataMember(Name = "memory_in_bytes")]
    public long MemoryInBytes { get; set; }

    [DataMember(Name = "norms_memory_in_bytes")]
    public long NormsMemoryInBytes { get; set; }

    [DataMember(Name = "points_memory_in_bytes")]
    public long PointsMemoryInBytes { get; set; }

    [DataMember(Name = "stored_fields_memory_in_bytes")]
    public long StoredFieldsMemoryInBytes { get; set; }

    [DataMember(Name = "terms_memory_in_bytes")]
    public long TermsMemoryInBytes { get; set; }

    [DataMember(Name = "term_vectors_memory_in_bytes")]
    public long TermVectorsMemoryInBytes { get; set; }

    [DataMember(Name = "version_map_memory_in_bytes")]
    public long VersionMapMemoryInBytes { get; set; }

    [DataMember(Name = "file_sizes")]
    public IReadOnlyDictionary<string, ShardFileSizeInfo> FileSizes { get; internal set; }
  }
}
