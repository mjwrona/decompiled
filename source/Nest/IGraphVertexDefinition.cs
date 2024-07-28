// Decompiled with JetBrains decompiler
// Type: Nest.IGraphVertexDefinition
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IGraphVertexDefinition
  {
    [DataMember(Name = "exclude")]
    IEnumerable<string> Exclude { get; set; }

    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "include")]
    IEnumerable<GraphVertexInclude> Include { get; set; }

    [DataMember(Name = "min_doc_count")]
    long? MinimumDocumentCount { get; set; }

    [DataMember(Name = "shard_min_doc_count")]
    long? ShardMinimumDocumentCount { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }
  }
}
