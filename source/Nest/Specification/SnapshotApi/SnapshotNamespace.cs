// Decompiled with JetBrains decompiler
// Type: Nest.Specification.SnapshotApi.SnapshotNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.SnapshotApi
{
  public class SnapshotNamespace : Nest.NamespacedClientProxy
  {
    internal SnapshotNamespace(ElasticClient client)
      : base(client)
    {
    }

    public CleanupRepositoryResponse CleanupRepository(
      Name repository,
      Func<CleanupRepositoryDescriptor, ICleanupRepositoryRequest> selector = null)
    {
      return this.CleanupRepository(selector.InvokeOrDefault<CleanupRepositoryDescriptor, ICleanupRepositoryRequest>(new CleanupRepositoryDescriptor(repository)));
    }

    public Task<CleanupRepositoryResponse> CleanupRepositoryAsync(
      Name repository,
      Func<CleanupRepositoryDescriptor, ICleanupRepositoryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CleanupRepositoryAsync(selector.InvokeOrDefault<CleanupRepositoryDescriptor, ICleanupRepositoryRequest>(new CleanupRepositoryDescriptor(repository)), ct);
    }

    public CleanupRepositoryResponse CleanupRepository(ICleanupRepositoryRequest request) => this.DoRequest<ICleanupRepositoryRequest, CleanupRepositoryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CleanupRepositoryResponse> CleanupRepositoryAsync(
      ICleanupRepositoryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICleanupRepositoryRequest, CleanupRepositoryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CloneSnapshotResponse Clone(
      Name repository,
      Name snapshot,
      Name targetSnapshot,
      Func<CloneSnapshotDescriptor, ICloneSnapshotRequest> selector)
    {
      return this.Clone(selector.InvokeOrDefault<CloneSnapshotDescriptor, ICloneSnapshotRequest>(new CloneSnapshotDescriptor(repository, snapshot, targetSnapshot)));
    }

    public Task<CloneSnapshotResponse> CloneAsync(
      Name repository,
      Name snapshot,
      Name targetSnapshot,
      Func<CloneSnapshotDescriptor, ICloneSnapshotRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CloneAsync(selector.InvokeOrDefault<CloneSnapshotDescriptor, ICloneSnapshotRequest>(new CloneSnapshotDescriptor(repository, snapshot, targetSnapshot)), ct);
    }

    public CloneSnapshotResponse Clone(ICloneSnapshotRequest request) => this.DoRequest<ICloneSnapshotRequest, CloneSnapshotResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CloneSnapshotResponse> CloneAsync(
      ICloneSnapshotRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICloneSnapshotRequest, CloneSnapshotResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public SnapshotResponse Snapshot(
      Name repository,
      Name snapshot,
      Func<SnapshotDescriptor, ISnapshotRequest> selector = null)
    {
      return this.Snapshot(selector.InvokeOrDefault<SnapshotDescriptor, ISnapshotRequest>(new SnapshotDescriptor(repository, snapshot)));
    }

    public Task<SnapshotResponse> SnapshotAsync(
      Name repository,
      Name snapshot,
      Func<SnapshotDescriptor, ISnapshotRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SnapshotAsync(selector.InvokeOrDefault<SnapshotDescriptor, ISnapshotRequest>(new SnapshotDescriptor(repository, snapshot)), ct);
    }

    public SnapshotResponse Snapshot(ISnapshotRequest request) => this.DoRequest<ISnapshotRequest, SnapshotResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SnapshotResponse> SnapshotAsync(ISnapshotRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ISnapshotRequest, SnapshotResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public CreateRepositoryResponse CreateRepository(
      Name repository,
      Func<CreateRepositoryDescriptor, ICreateRepositoryRequest> selector)
    {
      return this.CreateRepository(selector.InvokeOrDefault<CreateRepositoryDescriptor, ICreateRepositoryRequest>(new CreateRepositoryDescriptor(repository)));
    }

    public Task<CreateRepositoryResponse> CreateRepositoryAsync(
      Name repository,
      Func<CreateRepositoryDescriptor, ICreateRepositoryRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CreateRepositoryAsync(selector.InvokeOrDefault<CreateRepositoryDescriptor, ICreateRepositoryRequest>(new CreateRepositoryDescriptor(repository)), ct);
    }

    public CreateRepositoryResponse CreateRepository(ICreateRepositoryRequest request) => this.DoRequest<ICreateRepositoryRequest, CreateRepositoryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CreateRepositoryResponse> CreateRepositoryAsync(
      ICreateRepositoryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICreateRepositoryRequest, CreateRepositoryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteSnapshotResponse Delete(
      Name repository,
      Name snapshot,
      Func<DeleteSnapshotDescriptor, IDeleteSnapshotRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<DeleteSnapshotDescriptor, IDeleteSnapshotRequest>(new DeleteSnapshotDescriptor(repository, snapshot)));
    }

    public Task<DeleteSnapshotResponse> DeleteAsync(
      Name repository,
      Name snapshot,
      Func<DeleteSnapshotDescriptor, IDeleteSnapshotRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<DeleteSnapshotDescriptor, IDeleteSnapshotRequest>(new DeleteSnapshotDescriptor(repository, snapshot)), ct);
    }

    public DeleteSnapshotResponse Delete(IDeleteSnapshotRequest request) => this.DoRequest<IDeleteSnapshotRequest, DeleteSnapshotResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteSnapshotResponse> DeleteAsync(
      IDeleteSnapshotRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteSnapshotRequest, DeleteSnapshotResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteRepositoryResponse DeleteRepository(
      Names repository,
      Func<DeleteRepositoryDescriptor, IDeleteRepositoryRequest> selector = null)
    {
      return this.DeleteRepository(selector.InvokeOrDefault<DeleteRepositoryDescriptor, IDeleteRepositoryRequest>(new DeleteRepositoryDescriptor(repository)));
    }

    public Task<DeleteRepositoryResponse> DeleteRepositoryAsync(
      Names repository,
      Func<DeleteRepositoryDescriptor, IDeleteRepositoryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteRepositoryAsync(selector.InvokeOrDefault<DeleteRepositoryDescriptor, IDeleteRepositoryRequest>(new DeleteRepositoryDescriptor(repository)), ct);
    }

    public DeleteRepositoryResponse DeleteRepository(IDeleteRepositoryRequest request) => this.DoRequest<IDeleteRepositoryRequest, DeleteRepositoryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteRepositoryResponse> DeleteRepositoryAsync(
      IDeleteRepositoryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteRepositoryRequest, DeleteRepositoryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetSnapshotResponse Get(
      Name repository,
      Names snapshot,
      Func<GetSnapshotDescriptor, IGetSnapshotRequest> selector = null)
    {
      return this.Get(selector.InvokeOrDefault<GetSnapshotDescriptor, IGetSnapshotRequest>(new GetSnapshotDescriptor(repository, snapshot)));
    }

    public Task<GetSnapshotResponse> GetAsync(
      Name repository,
      Names snapshot,
      Func<GetSnapshotDescriptor, IGetSnapshotRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAsync(selector.InvokeOrDefault<GetSnapshotDescriptor, IGetSnapshotRequest>(new GetSnapshotDescriptor(repository, snapshot)), ct);
    }

    public GetSnapshotResponse Get(IGetSnapshotRequest request) => this.DoRequest<IGetSnapshotRequest, GetSnapshotResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetSnapshotResponse> GetAsync(IGetSnapshotRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetSnapshotRequest, GetSnapshotResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetRepositoryResponse GetRepository(
      Func<GetRepositoryDescriptor, IGetRepositoryRequest> selector = null)
    {
      return this.GetRepository(selector.InvokeOrDefault<GetRepositoryDescriptor, IGetRepositoryRequest>(new GetRepositoryDescriptor()));
    }

    public Task<GetRepositoryResponse> GetRepositoryAsync(
      Func<GetRepositoryDescriptor, IGetRepositoryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetRepositoryAsync(selector.InvokeOrDefault<GetRepositoryDescriptor, IGetRepositoryRequest>(new GetRepositoryDescriptor()), ct);
    }

    public GetRepositoryResponse GetRepository(IGetRepositoryRequest request) => this.DoRequest<IGetRepositoryRequest, GetRepositoryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetRepositoryResponse> GetRepositoryAsync(
      IGetRepositoryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetRepositoryRequest, GetRepositoryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public AnalyzeRepositoryResponse AnalyzeRepository(
      Name repository,
      Func<AnalyzeRepositoryDescriptor, IAnalyzeRepositoryRequest> selector = null)
    {
      return this.AnalyzeRepository(selector.InvokeOrDefault<AnalyzeRepositoryDescriptor, IAnalyzeRepositoryRequest>(new AnalyzeRepositoryDescriptor(repository)));
    }

    public Task<AnalyzeRepositoryResponse> AnalyzeRepositoryAsync(
      Name repository,
      Func<AnalyzeRepositoryDescriptor, IAnalyzeRepositoryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AnalyzeRepositoryAsync(selector.InvokeOrDefault<AnalyzeRepositoryDescriptor, IAnalyzeRepositoryRequest>(new AnalyzeRepositoryDescriptor(repository)), ct);
    }

    public AnalyzeRepositoryResponse AnalyzeRepository(IAnalyzeRepositoryRequest request) => this.DoRequest<IAnalyzeRepositoryRequest, AnalyzeRepositoryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<AnalyzeRepositoryResponse> AnalyzeRepositoryAsync(
      IAnalyzeRepositoryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IAnalyzeRepositoryRequest, AnalyzeRepositoryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RestoreResponse Restore(
      Name repository,
      Name snapshot,
      Func<RestoreDescriptor, IRestoreRequest> selector = null)
    {
      return this.Restore(selector.InvokeOrDefault<RestoreDescriptor, IRestoreRequest>(new RestoreDescriptor(repository, snapshot)));
    }

    public Task<RestoreResponse> RestoreAsync(
      Name repository,
      Name snapshot,
      Func<RestoreDescriptor, IRestoreRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RestoreAsync(selector.InvokeOrDefault<RestoreDescriptor, IRestoreRequest>(new RestoreDescriptor(repository, snapshot)), ct);
    }

    public RestoreResponse Restore(IRestoreRequest request) => this.DoRequest<IRestoreRequest, RestoreResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RestoreResponse> RestoreAsync(IRestoreRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IRestoreRequest, RestoreResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public SnapshotStatusResponse Status(
      Func<SnapshotStatusDescriptor, ISnapshotStatusRequest> selector = null)
    {
      return this.Status(selector.InvokeOrDefault<SnapshotStatusDescriptor, ISnapshotStatusRequest>(new SnapshotStatusDescriptor()));
    }

    public Task<SnapshotStatusResponse> StatusAsync(
      Func<SnapshotStatusDescriptor, ISnapshotStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatusAsync(selector.InvokeOrDefault<SnapshotStatusDescriptor, ISnapshotStatusRequest>(new SnapshotStatusDescriptor()), ct);
    }

    public SnapshotStatusResponse Status(ISnapshotStatusRequest request) => this.DoRequest<ISnapshotStatusRequest, SnapshotStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SnapshotStatusResponse> StatusAsync(
      ISnapshotStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ISnapshotStatusRequest, SnapshotStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public VerifyRepositoryResponse VerifyRepository(
      Name repository,
      Func<VerifyRepositoryDescriptor, IVerifyRepositoryRequest> selector = null)
    {
      return this.VerifyRepository(selector.InvokeOrDefault<VerifyRepositoryDescriptor, IVerifyRepositoryRequest>(new VerifyRepositoryDescriptor(repository)));
    }

    public Task<VerifyRepositoryResponse> VerifyRepositoryAsync(
      Name repository,
      Func<VerifyRepositoryDescriptor, IVerifyRepositoryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.VerifyRepositoryAsync(selector.InvokeOrDefault<VerifyRepositoryDescriptor, IVerifyRepositoryRequest>(new VerifyRepositoryDescriptor(repository)), ct);
    }

    public VerifyRepositoryResponse VerifyRepository(IVerifyRepositoryRequest request) => this.DoRequest<IVerifyRepositoryRequest, VerifyRepositoryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<VerifyRepositoryResponse> VerifyRepositoryAsync(
      IVerifyRepositoryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IVerifyRepositoryRequest, VerifyRepositoryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
