// Decompiled with JetBrains decompiler
// Type: Nest.Specification.DanglingIndicesApi.DanglingIndicesNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.DanglingIndicesApi
{
  public class DanglingIndicesNamespace : Nest.NamespacedClientProxy
  {
    internal DanglingIndicesNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteDanglingIndexResponse DeleteDanglingIndex(
      IndexUuid indexUuid,
      Func<DeleteDanglingIndexDescriptor, IDeleteDanglingIndexRequest> selector = null)
    {
      return this.DeleteDanglingIndex(selector.InvokeOrDefault<DeleteDanglingIndexDescriptor, IDeleteDanglingIndexRequest>(new DeleteDanglingIndexDescriptor(indexUuid)));
    }

    public Task<DeleteDanglingIndexResponse> DeleteDanglingIndexAsync(
      IndexUuid indexUuid,
      Func<DeleteDanglingIndexDescriptor, IDeleteDanglingIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteDanglingIndexAsync(selector.InvokeOrDefault<DeleteDanglingIndexDescriptor, IDeleteDanglingIndexRequest>(new DeleteDanglingIndexDescriptor(indexUuid)), ct);
    }

    public DeleteDanglingIndexResponse DeleteDanglingIndex(IDeleteDanglingIndexRequest request) => this.DoRequest<IDeleteDanglingIndexRequest, DeleteDanglingIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteDanglingIndexResponse> DeleteDanglingIndexAsync(
      IDeleteDanglingIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteDanglingIndexRequest, DeleteDanglingIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ImportDanglingIndexResponse ImportDanglingIndex(
      IndexUuid indexUuid,
      Func<ImportDanglingIndexDescriptor, IImportDanglingIndexRequest> selector = null)
    {
      return this.ImportDanglingIndex(selector.InvokeOrDefault<ImportDanglingIndexDescriptor, IImportDanglingIndexRequest>(new ImportDanglingIndexDescriptor(indexUuid)));
    }

    public Task<ImportDanglingIndexResponse> ImportDanglingIndexAsync(
      IndexUuid indexUuid,
      Func<ImportDanglingIndexDescriptor, IImportDanglingIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ImportDanglingIndexAsync(selector.InvokeOrDefault<ImportDanglingIndexDescriptor, IImportDanglingIndexRequest>(new ImportDanglingIndexDescriptor(indexUuid)), ct);
    }

    public ImportDanglingIndexResponse ImportDanglingIndex(IImportDanglingIndexRequest request) => this.DoRequest<IImportDanglingIndexRequest, ImportDanglingIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ImportDanglingIndexResponse> ImportDanglingIndexAsync(
      IImportDanglingIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IImportDanglingIndexRequest, ImportDanglingIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ListDanglingIndicesResponse List(
      Func<ListDanglingIndicesDescriptor, IListDanglingIndicesRequest> selector = null)
    {
      return this.List(selector.InvokeOrDefault<ListDanglingIndicesDescriptor, IListDanglingIndicesRequest>(new ListDanglingIndicesDescriptor()));
    }

    public Task<ListDanglingIndicesResponse> ListAsync(
      Func<ListDanglingIndicesDescriptor, IListDanglingIndicesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ListAsync(selector.InvokeOrDefault<ListDanglingIndicesDescriptor, IListDanglingIndicesRequest>(new ListDanglingIndicesDescriptor()), ct);
    }

    public ListDanglingIndicesResponse List(IListDanglingIndicesRequest request) => this.DoRequest<IListDanglingIndicesRequest, ListDanglingIndicesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ListDanglingIndicesResponse> ListAsync(
      IListDanglingIndicesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IListDanglingIndicesRequest, ListDanglingIndicesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
