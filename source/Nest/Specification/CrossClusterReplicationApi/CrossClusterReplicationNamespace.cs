// Decompiled with JetBrains decompiler
// Type: Nest.Specification.CrossClusterReplicationApi.CrossClusterReplicationNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.CrossClusterReplicationApi
{
  public class CrossClusterReplicationNamespace : Nest.NamespacedClientProxy
  {
    internal CrossClusterReplicationNamespace(ElasticClient client)
      : base(client)
    {
    }

    public DeleteAutoFollowPatternResponse DeleteAutoFollowPattern(
      Name name,
      Func<DeleteAutoFollowPatternDescriptor, IDeleteAutoFollowPatternRequest> selector = null)
    {
      return this.DeleteAutoFollowPattern(selector.InvokeOrDefault<DeleteAutoFollowPatternDescriptor, IDeleteAutoFollowPatternRequest>(new DeleteAutoFollowPatternDescriptor(name)));
    }

    public Task<DeleteAutoFollowPatternResponse> DeleteAutoFollowPatternAsync(
      Name name,
      Func<DeleteAutoFollowPatternDescriptor, IDeleteAutoFollowPatternRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteAutoFollowPatternAsync(selector.InvokeOrDefault<DeleteAutoFollowPatternDescriptor, IDeleteAutoFollowPatternRequest>(new DeleteAutoFollowPatternDescriptor(name)), ct);
    }

    public DeleteAutoFollowPatternResponse DeleteAutoFollowPattern(
      IDeleteAutoFollowPatternRequest request)
    {
      return this.DoRequest<IDeleteAutoFollowPatternRequest, DeleteAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<DeleteAutoFollowPatternResponse> DeleteAutoFollowPatternAsync(
      IDeleteAutoFollowPatternRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteAutoFollowPatternRequest, DeleteAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CreateFollowIndexResponse CreateFollowIndex(
      IndexName index,
      Func<CreateFollowIndexDescriptor, ICreateFollowIndexRequest> selector)
    {
      return this.CreateFollowIndex(selector.InvokeOrDefault<CreateFollowIndexDescriptor, ICreateFollowIndexRequest>(new CreateFollowIndexDescriptor(index)));
    }

    public Task<CreateFollowIndexResponse> CreateFollowIndexAsync(
      IndexName index,
      Func<CreateFollowIndexDescriptor, ICreateFollowIndexRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CreateFollowIndexAsync(selector.InvokeOrDefault<CreateFollowIndexDescriptor, ICreateFollowIndexRequest>(new CreateFollowIndexDescriptor(index)), ct);
    }

    public CreateFollowIndexResponse CreateFollowIndex(ICreateFollowIndexRequest request) => this.DoRequest<ICreateFollowIndexRequest, CreateFollowIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CreateFollowIndexResponse> CreateFollowIndexAsync(
      ICreateFollowIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICreateFollowIndexRequest, CreateFollowIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public FollowInfoResponse FollowInfo(
      Indices index,
      Func<FollowInfoDescriptor, IFollowInfoRequest> selector = null)
    {
      return this.FollowInfo(selector.InvokeOrDefault<FollowInfoDescriptor, IFollowInfoRequest>(new FollowInfoDescriptor(index)));
    }

    public Task<FollowInfoResponse> FollowInfoAsync(
      Indices index,
      Func<FollowInfoDescriptor, IFollowInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.FollowInfoAsync(selector.InvokeOrDefault<FollowInfoDescriptor, IFollowInfoRequest>(new FollowInfoDescriptor(index)), ct);
    }

    public FollowInfoResponse FollowInfo(IFollowInfoRequest request) => this.DoRequest<IFollowInfoRequest, FollowInfoResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<FollowInfoResponse> FollowInfoAsync(
      IFollowInfoRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IFollowInfoRequest, FollowInfoResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public FollowIndexStatsResponse FollowIndexStats(
      Indices index,
      Func<FollowIndexStatsDescriptor, IFollowIndexStatsRequest> selector = null)
    {
      return this.FollowIndexStats(selector.InvokeOrDefault<FollowIndexStatsDescriptor, IFollowIndexStatsRequest>(new FollowIndexStatsDescriptor(index)));
    }

    public Task<FollowIndexStatsResponse> FollowIndexStatsAsync(
      Indices index,
      Func<FollowIndexStatsDescriptor, IFollowIndexStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.FollowIndexStatsAsync(selector.InvokeOrDefault<FollowIndexStatsDescriptor, IFollowIndexStatsRequest>(new FollowIndexStatsDescriptor(index)), ct);
    }

    public FollowIndexStatsResponse FollowIndexStats(IFollowIndexStatsRequest request) => this.DoRequest<IFollowIndexStatsRequest, FollowIndexStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<FollowIndexStatsResponse> FollowIndexStatsAsync(
      IFollowIndexStatsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IFollowIndexStatsRequest, FollowIndexStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ForgetFollowerIndexResponse ForgetFollowerIndex(
      IndexName index,
      Func<ForgetFollowerIndexDescriptor, IForgetFollowerIndexRequest> selector)
    {
      return this.ForgetFollowerIndex(selector.InvokeOrDefault<ForgetFollowerIndexDescriptor, IForgetFollowerIndexRequest>(new ForgetFollowerIndexDescriptor(index)));
    }

    public Task<ForgetFollowerIndexResponse> ForgetFollowerIndexAsync(
      IndexName index,
      Func<ForgetFollowerIndexDescriptor, IForgetFollowerIndexRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ForgetFollowerIndexAsync(selector.InvokeOrDefault<ForgetFollowerIndexDescriptor, IForgetFollowerIndexRequest>(new ForgetFollowerIndexDescriptor(index)), ct);
    }

    public ForgetFollowerIndexResponse ForgetFollowerIndex(IForgetFollowerIndexRequest request) => this.DoRequest<IForgetFollowerIndexRequest, ForgetFollowerIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ForgetFollowerIndexResponse> ForgetFollowerIndexAsync(
      IForgetFollowerIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IForgetFollowerIndexRequest, ForgetFollowerIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetAutoFollowPatternResponse GetAutoFollowPattern(
      Name name = null,
      Func<GetAutoFollowPatternDescriptor, IGetAutoFollowPatternRequest> selector = null)
    {
      return this.GetAutoFollowPattern(selector.InvokeOrDefault<GetAutoFollowPatternDescriptor, IGetAutoFollowPatternRequest>(new GetAutoFollowPatternDescriptor().Name(name)));
    }

    public Task<GetAutoFollowPatternResponse> GetAutoFollowPatternAsync(
      Name name = null,
      Func<GetAutoFollowPatternDescriptor, IGetAutoFollowPatternRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetAutoFollowPatternAsync(selector.InvokeOrDefault<GetAutoFollowPatternDescriptor, IGetAutoFollowPatternRequest>(new GetAutoFollowPatternDescriptor().Name(name)), ct);
    }

    public GetAutoFollowPatternResponse GetAutoFollowPattern(IGetAutoFollowPatternRequest request) => this.DoRequest<IGetAutoFollowPatternRequest, GetAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetAutoFollowPatternResponse> GetAutoFollowPatternAsync(
      IGetAutoFollowPatternRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetAutoFollowPatternRequest, GetAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PauseAutoFollowPatternResponse PauseAutoFollowPattern(
      Name name,
      Func<PauseAutoFollowPatternDescriptor, IPauseAutoFollowPatternRequest> selector = null)
    {
      return this.PauseAutoFollowPattern(selector.InvokeOrDefault<PauseAutoFollowPatternDescriptor, IPauseAutoFollowPatternRequest>(new PauseAutoFollowPatternDescriptor(name)));
    }

    public Task<PauseAutoFollowPatternResponse> PauseAutoFollowPatternAsync(
      Name name,
      Func<PauseAutoFollowPatternDescriptor, IPauseAutoFollowPatternRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PauseAutoFollowPatternAsync(selector.InvokeOrDefault<PauseAutoFollowPatternDescriptor, IPauseAutoFollowPatternRequest>(new PauseAutoFollowPatternDescriptor(name)), ct);
    }

    public PauseAutoFollowPatternResponse PauseAutoFollowPattern(
      IPauseAutoFollowPatternRequest request)
    {
      return this.DoRequest<IPauseAutoFollowPatternRequest, PauseAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<PauseAutoFollowPatternResponse> PauseAutoFollowPatternAsync(
      IPauseAutoFollowPatternRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPauseAutoFollowPatternRequest, PauseAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PauseFollowIndexResponse PauseFollowIndex(
      IndexName index,
      Func<PauseFollowIndexDescriptor, IPauseFollowIndexRequest> selector = null)
    {
      return this.PauseFollowIndex(selector.InvokeOrDefault<PauseFollowIndexDescriptor, IPauseFollowIndexRequest>(new PauseFollowIndexDescriptor(index)));
    }

    public Task<PauseFollowIndexResponse> PauseFollowIndexAsync(
      IndexName index,
      Func<PauseFollowIndexDescriptor, IPauseFollowIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PauseFollowIndexAsync(selector.InvokeOrDefault<PauseFollowIndexDescriptor, IPauseFollowIndexRequest>(new PauseFollowIndexDescriptor(index)), ct);
    }

    public PauseFollowIndexResponse PauseFollowIndex(IPauseFollowIndexRequest request) => this.DoRequest<IPauseFollowIndexRequest, PauseFollowIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PauseFollowIndexResponse> PauseFollowIndexAsync(
      IPauseFollowIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPauseFollowIndexRequest, PauseFollowIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CreateAutoFollowPatternResponse CreateAutoFollowPattern(
      Name name,
      Func<CreateAutoFollowPatternDescriptor, ICreateAutoFollowPatternRequest> selector)
    {
      return this.CreateAutoFollowPattern(selector.InvokeOrDefault<CreateAutoFollowPatternDescriptor, ICreateAutoFollowPatternRequest>(new CreateAutoFollowPatternDescriptor(name)));
    }

    public Task<CreateAutoFollowPatternResponse> CreateAutoFollowPatternAsync(
      Name name,
      Func<CreateAutoFollowPatternDescriptor, ICreateAutoFollowPatternRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CreateAutoFollowPatternAsync(selector.InvokeOrDefault<CreateAutoFollowPatternDescriptor, ICreateAutoFollowPatternRequest>(new CreateAutoFollowPatternDescriptor(name)), ct);
    }

    public CreateAutoFollowPatternResponse CreateAutoFollowPattern(
      ICreateAutoFollowPatternRequest request)
    {
      return this.DoRequest<ICreateAutoFollowPatternRequest, CreateAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<CreateAutoFollowPatternResponse> CreateAutoFollowPatternAsync(
      ICreateAutoFollowPatternRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ICreateAutoFollowPatternRequest, CreateAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ResumeAutoFollowPatternResponse ResumeAutoFollowPattern(
      Name name,
      Func<ResumeAutoFollowPatternDescriptor, IResumeAutoFollowPatternRequest> selector = null)
    {
      return this.ResumeAutoFollowPattern(selector.InvokeOrDefault<ResumeAutoFollowPatternDescriptor, IResumeAutoFollowPatternRequest>(new ResumeAutoFollowPatternDescriptor(name)));
    }

    public Task<ResumeAutoFollowPatternResponse> ResumeAutoFollowPatternAsync(
      Name name,
      Func<ResumeAutoFollowPatternDescriptor, IResumeAutoFollowPatternRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ResumeAutoFollowPatternAsync(selector.InvokeOrDefault<ResumeAutoFollowPatternDescriptor, IResumeAutoFollowPatternRequest>(new ResumeAutoFollowPatternDescriptor(name)), ct);
    }

    public ResumeAutoFollowPatternResponse ResumeAutoFollowPattern(
      IResumeAutoFollowPatternRequest request)
    {
      return this.DoRequest<IResumeAutoFollowPatternRequest, ResumeAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<ResumeAutoFollowPatternResponse> ResumeAutoFollowPatternAsync(
      IResumeAutoFollowPatternRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IResumeAutoFollowPatternRequest, ResumeAutoFollowPatternResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ResumeFollowIndexResponse ResumeFollowIndex(
      IndexName index,
      Func<ResumeFollowIndexDescriptor, IResumeFollowIndexRequest> selector = null)
    {
      return this.ResumeFollowIndex(selector.InvokeOrDefault<ResumeFollowIndexDescriptor, IResumeFollowIndexRequest>(new ResumeFollowIndexDescriptor(index)));
    }

    public Task<ResumeFollowIndexResponse> ResumeFollowIndexAsync(
      IndexName index,
      Func<ResumeFollowIndexDescriptor, IResumeFollowIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ResumeFollowIndexAsync(selector.InvokeOrDefault<ResumeFollowIndexDescriptor, IResumeFollowIndexRequest>(new ResumeFollowIndexDescriptor(index)), ct);
    }

    public ResumeFollowIndexResponse ResumeFollowIndex(IResumeFollowIndexRequest request) => this.DoRequest<IResumeFollowIndexRequest, ResumeFollowIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ResumeFollowIndexResponse> ResumeFollowIndexAsync(
      IResumeFollowIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IResumeFollowIndexRequest, ResumeFollowIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CcrStatsResponse Stats(
      Func<CcrStatsDescriptor, ICcrStatsRequest> selector = null)
    {
      return this.Stats(selector.InvokeOrDefault<CcrStatsDescriptor, ICcrStatsRequest>(new CcrStatsDescriptor()));
    }

    public Task<CcrStatsResponse> StatsAsync(
      Func<CcrStatsDescriptor, ICcrStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatsAsync(selector.InvokeOrDefault<CcrStatsDescriptor, ICcrStatsRequest>(new CcrStatsDescriptor()), ct);
    }

    public CcrStatsResponse Stats(ICcrStatsRequest request) => this.DoRequest<ICcrStatsRequest, CcrStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CcrStatsResponse> StatsAsync(ICcrStatsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ICcrStatsRequest, CcrStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public UnfollowIndexResponse UnfollowIndex(
      IndexName index,
      Func<UnfollowIndexDescriptor, IUnfollowIndexRequest> selector = null)
    {
      return this.UnfollowIndex(selector.InvokeOrDefault<UnfollowIndexDescriptor, IUnfollowIndexRequest>(new UnfollowIndexDescriptor(index)));
    }

    public Task<UnfollowIndexResponse> UnfollowIndexAsync(
      IndexName index,
      Func<UnfollowIndexDescriptor, IUnfollowIndexRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UnfollowIndexAsync(selector.InvokeOrDefault<UnfollowIndexDescriptor, IUnfollowIndexRequest>(new UnfollowIndexDescriptor(index)), ct);
    }

    public UnfollowIndexResponse UnfollowIndex(IUnfollowIndexRequest request) => this.DoRequest<IUnfollowIndexRequest, UnfollowIndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UnfollowIndexResponse> UnfollowIndexAsync(
      IUnfollowIndexRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUnfollowIndexRequest, UnfollowIndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }
  }
}
