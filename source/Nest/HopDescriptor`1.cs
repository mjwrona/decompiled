// Decompiled with JetBrains decompiler
// Type: Nest.HopDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class HopDescriptor<T> : DescriptorBase<HopDescriptor<T>, IHop>, IHop where T : class
  {
    IHop IHop.Connections { get; set; }

    QueryContainer IHop.Query { get; set; }

    IEnumerable<IGraphVertexDefinition> IHop.Vertices { get; set; }

    public HopDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(querySelector, (Action<IHop, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public HopDescriptor<T> Vertices(
      Func<GraphVerticesDescriptor<T>, IPromise<IList<IGraphVertexDefinition>>> selector)
    {
      return this.Assign<Func<GraphVerticesDescriptor<T>, IPromise<IList<IGraphVertexDefinition>>>>(selector, (Action<IHop, Func<GraphVerticesDescriptor<T>, IPromise<IList<IGraphVertexDefinition>>>>) ((a, v) => a.Vertices = v != null ? (IEnumerable<IGraphVertexDefinition>) v(new GraphVerticesDescriptor<T>())?.Value : (IEnumerable<IGraphVertexDefinition>) null));
    }

    public HopDescriptor<T> Connections(Func<HopDescriptor<T>, IHop> selector) => this.Assign<Func<HopDescriptor<T>, IHop>>(selector, (Action<IHop, Func<HopDescriptor<T>, IHop>>) ((a, v) => a.Connections = v != null ? v(new HopDescriptor<T>()) : (IHop) null));
  }
}
