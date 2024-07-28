// Decompiled with JetBrains decompiler
// Type: Nest.GraphVerticesDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class GraphVerticesDescriptor<T> : 
    DescriptorPromiseBase<GraphVerticesDescriptor<T>, IList<IGraphVertexDefinition>>
    where T : class
  {
    public GraphVerticesDescriptor()
      : base((IList<IGraphVertexDefinition>) new List<IGraphVertexDefinition>())
    {
    }

    public GraphVerticesDescriptor<T> Vertex<TValue>(
      Expression<Func<T, TValue>> field,
      Func<GraphVertexDefinitionDescriptor, IGraphVertexDefinition> selector = null)
    {
      return this.Assign<IGraphVertexDefinition>(selector.InvokeOrDefault<GraphVertexDefinitionDescriptor, IGraphVertexDefinition>(new GraphVertexDefinitionDescriptor((Field) (Expression) field)), (Action<IList<IGraphVertexDefinition>, IGraphVertexDefinition>) ((a, v) => a.Add(v)));
    }

    public GraphVerticesDescriptor<T> Vertex(
      Field field,
      Func<GraphVertexDefinitionDescriptor, IGraphVertexDefinition> selector = null)
    {
      return this.Assign<IGraphVertexDefinition>(selector.InvokeOrDefault<GraphVertexDefinitionDescriptor, IGraphVertexDefinition>(new GraphVertexDefinitionDescriptor(field)), (Action<IList<IGraphVertexDefinition>, IGraphVertexDefinition>) ((a, v) => a.Add(v)));
    }
  }
}
