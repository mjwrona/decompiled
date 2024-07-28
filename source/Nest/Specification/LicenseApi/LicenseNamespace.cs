// Decompiled with JetBrains decompiler
// Type: Nest.Specification.LicenseApi.LicenseNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.LicenseApi
{
  public class LicenseNamespace : Nest.NamespacedClientProxy
  {
    internal LicenseNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteLicenseResponse Delete(
      Func<DeleteLicenseDescriptor, IDeleteLicenseRequest> selector = null)
    {
      return this.Delete(selector.InvokeOrDefault<DeleteLicenseDescriptor, IDeleteLicenseRequest>(new DeleteLicenseDescriptor()));
    }

    public Task<DeleteLicenseResponse> DeleteAsync(
      Func<DeleteLicenseDescriptor, IDeleteLicenseRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAsync(selector.InvokeOrDefault<DeleteLicenseDescriptor, IDeleteLicenseRequest>(new DeleteLicenseDescriptor()), ct);
    }

    public DeleteLicenseResponse Delete(IDeleteLicenseRequest request) => this.DoRequest<IDeleteLicenseRequest, DeleteLicenseResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteLicenseResponse> DeleteAsync(
      IDeleteLicenseRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteLicenseRequest, DeleteLicenseResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetLicenseResponse Get(
      Func<GetLicenseDescriptor, IGetLicenseRequest> selector = null)
    {
      return this.Get(selector.InvokeOrDefault<GetLicenseDescriptor, IGetLicenseRequest>(new GetLicenseDescriptor()));
    }

    public Task<GetLicenseResponse> GetAsync(
      Func<GetLicenseDescriptor, IGetLicenseRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAsync(selector.InvokeOrDefault<GetLicenseDescriptor, IGetLicenseRequest>(new GetLicenseDescriptor()), ct);
    }

    public GetLicenseResponse Get(IGetLicenseRequest request) => this.DoRequest<IGetLicenseRequest, GetLicenseResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetLicenseResponse> GetAsync(IGetLicenseRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetLicenseRequest, GetLicenseResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public GetBasicLicenseStatusResponse GetBasicStatus(
      Func<GetBasicLicenseStatusDescriptor, IGetBasicLicenseStatusRequest> selector = null)
    {
      return this.GetBasicStatus(selector.InvokeOrDefault<GetBasicLicenseStatusDescriptor, IGetBasicLicenseStatusRequest>(new GetBasicLicenseStatusDescriptor()));
    }

    public Task<GetBasicLicenseStatusResponse> GetBasicStatusAsync(
      Func<GetBasicLicenseStatusDescriptor, IGetBasicLicenseStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetBasicStatusAsync(selector.InvokeOrDefault<GetBasicLicenseStatusDescriptor, IGetBasicLicenseStatusRequest>(new GetBasicLicenseStatusDescriptor()), ct);
    }

    public GetBasicLicenseStatusResponse GetBasicStatus(IGetBasicLicenseStatusRequest request) => this.DoRequest<IGetBasicLicenseStatusRequest, GetBasicLicenseStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetBasicLicenseStatusResponse> GetBasicStatusAsync(
      IGetBasicLicenseStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetBasicLicenseStatusRequest, GetBasicLicenseStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetTrialLicenseStatusResponse GetTrialStatus(
      Func<GetTrialLicenseStatusDescriptor, IGetTrialLicenseStatusRequest> selector = null)
    {
      return this.GetTrialStatus(selector.InvokeOrDefault<GetTrialLicenseStatusDescriptor, IGetTrialLicenseStatusRequest>(new GetTrialLicenseStatusDescriptor()));
    }

    public Task<GetTrialLicenseStatusResponse> GetTrialStatusAsync(
      Func<GetTrialLicenseStatusDescriptor, IGetTrialLicenseStatusRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetTrialStatusAsync(selector.InvokeOrDefault<GetTrialLicenseStatusDescriptor, IGetTrialLicenseStatusRequest>(new GetTrialLicenseStatusDescriptor()), ct);
    }

    public GetTrialLicenseStatusResponse GetTrialStatus(IGetTrialLicenseStatusRequest request) => this.DoRequest<IGetTrialLicenseStatusRequest, GetTrialLicenseStatusResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetTrialLicenseStatusResponse> GetTrialStatusAsync(
      IGetTrialLicenseStatusRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetTrialLicenseStatusRequest, GetTrialLicenseStatusResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PostLicenseResponse Post(
      Func<PostLicenseDescriptor, IPostLicenseRequest> selector = null)
    {
      return this.Post(selector.InvokeOrDefault<PostLicenseDescriptor, IPostLicenseRequest>(new PostLicenseDescriptor()));
    }

    public Task<PostLicenseResponse> PostAsync(
      Func<PostLicenseDescriptor, IPostLicenseRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PostAsync(selector.InvokeOrDefault<PostLicenseDescriptor, IPostLicenseRequest>(new PostLicenseDescriptor()), ct);
    }

    public PostLicenseResponse Post(IPostLicenseRequest request) => this.DoRequest<IPostLicenseRequest, PostLicenseResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PostLicenseResponse> PostAsync(IPostLicenseRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPostLicenseRequest, PostLicenseResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public StartBasicLicenseResponse StartBasic(
      Func<StartBasicLicenseDescriptor, IStartBasicLicenseRequest> selector = null)
    {
      return this.StartBasic(selector.InvokeOrDefault<StartBasicLicenseDescriptor, IStartBasicLicenseRequest>(new StartBasicLicenseDescriptor()));
    }

    public Task<StartBasicLicenseResponse> StartBasicAsync(
      Func<StartBasicLicenseDescriptor, IStartBasicLicenseRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartBasicAsync(selector.InvokeOrDefault<StartBasicLicenseDescriptor, IStartBasicLicenseRequest>(new StartBasicLicenseDescriptor()), ct);
    }

    public StartBasicLicenseResponse StartBasic(IStartBasicLicenseRequest request) => this.DoRequest<IStartBasicLicenseRequest, StartBasicLicenseResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StartBasicLicenseResponse> StartBasicAsync(
      IStartBasicLicenseRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStartBasicLicenseRequest, StartBasicLicenseResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public StartTrialLicenseResponse StartTrial(
      Func<StartTrialLicenseDescriptor, IStartTrialLicenseRequest> selector = null)
    {
      return this.StartTrial(selector.InvokeOrDefault<StartTrialLicenseDescriptor, IStartTrialLicenseRequest>(new StartTrialLicenseDescriptor()));
    }

    public Task<StartTrialLicenseResponse> StartTrialAsync(
      Func<StartTrialLicenseDescriptor, IStartTrialLicenseRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StartTrialAsync(selector.InvokeOrDefault<StartTrialLicenseDescriptor, IStartTrialLicenseRequest>(new StartTrialLicenseDescriptor()), ct);
    }

    public StartTrialLicenseResponse StartTrial(IStartTrialLicenseRequest request) => this.DoRequest<IStartTrialLicenseRequest, StartTrialLicenseResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<StartTrialLicenseResponse> StartTrialAsync(
      IStartTrialLicenseRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IStartTrialLicenseRequest, StartTrialLicenseResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
