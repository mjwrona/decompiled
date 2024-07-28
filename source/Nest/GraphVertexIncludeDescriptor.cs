// Decompiled with JetBrains decompiler
// Type: Nest.GraphVertexIncludeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class GraphVertexIncludeDescriptor : 
    DescriptorPromiseBase<GraphVertexIncludeDescriptor, List<GraphVertexInclude>>
  {
    public GraphVertexIncludeDescriptor()
      : base(new List<GraphVertexInclude>())
    {
    }

    public GraphVertexIncludeDescriptor Include(string term, double? boost = null) => this.Assign<GraphVertexInclude>(new GraphVertexInclude()
    {
      Term = term,
      Boost = boost
    }, (Action<List<GraphVertexInclude>, GraphVertexInclude>) ((a, v) => a.Add(v)));

    public GraphVertexIncludeDescriptor IncludeRange(params string[] terms) => this.Assign<IEnumerable<GraphVertexInclude>>(((IEnumerable<string>) terms).Select<string, GraphVertexInclude>((Func<string, GraphVertexInclude>) (t => new GraphVertexInclude()
    {
      Term = t
    })), (Action<List<GraphVertexInclude>, IEnumerable<GraphVertexInclude>>) ((a, v) => a.AddRange(v)));

    public GraphVertexIncludeDescriptor IncludeRange(IEnumerable<string> terms) => this.Assign<IEnumerable<GraphVertexInclude>>(terms.Select<string, GraphVertexInclude>((Func<string, GraphVertexInclude>) (t => new GraphVertexInclude()
    {
      Term = t
    })), (Action<List<GraphVertexInclude>, IEnumerable<GraphVertexInclude>>) ((a, v) => a.AddRange(v)));

    public GraphVertexIncludeDescriptor IncludeRange(IEnumerable<GraphVertexInclude> includes) => this.Assign<IEnumerable<GraphVertexInclude>>(includes, (Action<List<GraphVertexInclude>, IEnumerable<GraphVertexInclude>>) ((a, v) => a.AddRange(v)));
  }
}
