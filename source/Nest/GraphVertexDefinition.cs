// Decompiled with JetBrains decompiler
// Type: Nest.GraphVertexDefinition
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class GraphVertexDefinition : IGraphVertexDefinition
  {
    public IEnumerable<string> Exclude { get; set; }

    public Field Field { get; set; }

    public IEnumerable<GraphVertexInclude> Include { get; set; }

    public long? MinimumDocumentCount { get; set; }

    public long? ShardMinimumDocumentCount { get; set; }

    public int? Size { get; set; }
  }
}
