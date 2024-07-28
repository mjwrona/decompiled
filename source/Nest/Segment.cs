﻿// Decompiled with JetBrains decompiler
// Type: Nest.Segment
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class Segment
  {
    [DataMember(Name = "attributes")]
    public IReadOnlyDictionary<string, string> Attributes { get; internal set; } = EmptyReadOnly<string, string>.Dictionary;

    [DataMember(Name = "committed")]
    public bool Committed { get; internal set; }

    [DataMember(Name = "compound")]
    public bool Compound { get; internal set; }

    [DataMember(Name = "deleted_docs")]
    public long DeletedDocuments { get; internal set; }

    [DataMember(Name = "generation")]
    public int Generation { get; internal set; }

    [DataMember(Name = "memory_in_bytes")]
    public double MemoryInBytes { get; internal set; }

    [DataMember(Name = "search")]
    public bool Search { get; internal set; }

    [DataMember(Name = "size_in_bytes")]
    public double SizeInBytes { get; internal set; }

    [DataMember(Name = "num_docs")]
    public long TotalDocuments { get; internal set; }

    [DataMember(Name = "version")]
    public string Version { get; internal set; }
  }
}
