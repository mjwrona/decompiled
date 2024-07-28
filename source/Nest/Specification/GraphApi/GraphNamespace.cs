// Decompiled with JetBrains decompiler
// Type: Nest.Specification.GraphApi.GraphNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.GraphApi
{
  public class GraphNamespace : Nest.NamespacedClientProxy
  {
    internal GraphNamespace(ElasticClient client)
      : base(client)
    {
    }

    public GraphExploreResponse Explore<TDocument>(
      Func<GraphExploreDescriptor<TDocument>, IGraphExploreRequest> selector = null)
      where TDocument : class
    {
      return this.Explore(selector.InvokeOrDefault<GraphExploreDescriptor<TDocument>, IGraphExploreRequest>(new GraphExploreDescriptor<TDocument>()));
    }

    public Task<GraphExploreResponse> ExploreAsync<TDocument>(
      Func<GraphExploreDescriptor<TDocument>, IGraphExploreRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.ExploreAsync(selector.InvokeOrDefault<GraphExploreDescriptor<TDocument>, IGraphExploreRequest>(new GraphExploreDescriptor<TDocument>()), ct);
    }

    public GraphExploreResponse Explore(IGraphExploreRequest request) => this.DoRequest<IGraphExploreRequest, GraphExploreResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GraphExploreResponse> ExploreAsync(
      IGraphExploreRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGraphExploreRequest, GraphExploreResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
