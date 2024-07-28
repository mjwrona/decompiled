// Decompiled with JetBrains decompiler
// Type: Nest.GraphVertexDefinitionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class GraphVertexDefinitionDescriptor : 
    DescriptorBase<GraphVertexDefinitionDescriptor, IGraphVertexDefinition>,
    IGraphVertexDefinition
  {
    public GraphVertexDefinitionDescriptor(Field field) => this.Assign<Field>(field, (Action<IGraphVertexDefinition, Field>) ((a, v) => a.Field = v));

    IEnumerable<string> IGraphVertexDefinition.Exclude { get; set; }

    Field IGraphVertexDefinition.Field { get; set; }

    IEnumerable<GraphVertexInclude> IGraphVertexDefinition.Include { get; set; }

    long? IGraphVertexDefinition.MinimumDocumentCount { get; set; }

    long? IGraphVertexDefinition.ShardMinimumDocumentCount { get; set; }

    int? IGraphVertexDefinition.Size { get; set; }

    public GraphVertexDefinitionDescriptor Size(int? size) => this.Assign<int?>(size, (Action<IGraphVertexDefinition, int?>) ((a, v) => a.Size = v));

    public GraphVertexDefinitionDescriptor MinimumDocumentCount(int? minDocCount) => this.Assign<int?>(minDocCount, (Action<IGraphVertexDefinition, int?>) ((a, v) =>
    {
      IGraphVertexDefinition vertexDefinition = a;
      int? nullable1 = v;
      long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
      vertexDefinition.MinimumDocumentCount = nullable2;
    }));

    public GraphVertexDefinitionDescriptor ShardMinimumDocumentCount(int? shardMinDocCount) => this.Assign<int?>(shardMinDocCount, (Action<IGraphVertexDefinition, int?>) ((a, v) =>
    {
      IGraphVertexDefinition vertexDefinition = a;
      int? nullable1 = v;
      long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
      vertexDefinition.ShardMinimumDocumentCount = nullable2;
    }));

    public GraphVertexDefinitionDescriptor Exclude(params string[] excludes) => this.Assign<string[]>(excludes, (Action<IGraphVertexDefinition, string[]>) ((a, v) => a.Exclude = (IEnumerable<string>) v));

    public GraphVertexDefinitionDescriptor Exclude(IEnumerable<string> excludes) => this.Assign<IEnumerable<string>>(excludes, (Action<IGraphVertexDefinition, IEnumerable<string>>) ((a, v) => a.Exclude = v));

    public GraphVertexDefinitionDescriptor Include(params string[] includes) => this.Include((Func<GraphVertexIncludeDescriptor, IPromise<List<GraphVertexInclude>>>) (i => (IPromise<List<GraphVertexInclude>>) i.IncludeRange(includes)));

    public GraphVertexDefinitionDescriptor Include(IEnumerable<string> includes) => this.Include((Func<GraphVertexIncludeDescriptor, IPromise<List<GraphVertexInclude>>>) (i => (IPromise<List<GraphVertexInclude>>) i.IncludeRange(includes)));

    public GraphVertexDefinitionDescriptor Include(
      Func<GraphVertexIncludeDescriptor, IPromise<List<GraphVertexInclude>>> selector)
    {
      return this.Assign<Func<GraphVertexIncludeDescriptor, IPromise<List<GraphVertexInclude>>>>(selector, (Action<IGraphVertexDefinition, Func<GraphVertexIncludeDescriptor, IPromise<List<GraphVertexInclude>>>>) ((a, v) => a.Include = v != null ? (IEnumerable<GraphVertexInclude>) v(new GraphVertexIncludeDescriptor())?.Value : (IEnumerable<GraphVertexInclude>) null));
    }
  }
}
