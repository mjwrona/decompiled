// Decompiled with JetBrains decompiler
// Type: Nest.Specification.ClusterApi.ClusterNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.ClusterApi
{
  public class ClusterNamespace : Nest.NamespacedClientProxy
  {
    internal ClusterNamespace(ElasticClient client)
      : base(client)
    {
    }

    public ClusterAllocationExplainResponse AllocationExplain(
      Func<ClusterAllocationExplainDescriptor, IClusterAllocationExplainRequest> selector = null)
    {
      return this.AllocationExplain(selector.InvokeOrDefault<ClusterAllocationExplainDescriptor, IClusterAllocationExplainRequest>(new ClusterAllocationExplainDescriptor()));
    }

    public Task<ClusterAllocationExplainResponse> AllocationExplainAsync(
      Func<ClusterAllocationExplainDescriptor, IClusterAllocationExplainRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AllocationExplainAsync(selector.InvokeOrDefault<ClusterAllocationExplainDescriptor, IClusterAllocationExplainRequest>(new ClusterAllocationExplainDescriptor()), ct);
    }

    public ClusterAllocationExplainResponse AllocationExplain(
      IClusterAllocationExplainRequest request)
    {
      return this.DoRequest<IClusterAllocationExplainRequest, ClusterAllocationExplainResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<ClusterAllocationExplainResponse> AllocationExplainAsync(
      IClusterAllocationExplainRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClusterAllocationExplainRequest, ClusterAllocationExplainResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteComponentTemplateResponse DeleteComponentTemplate(
      Name name,
      Func<DeleteComponentTemplateDescriptor, IDeleteComponentTemplateRequest> selector = null)
    {
      return this.DeleteComponentTemplate(selector.InvokeOrDefault<DeleteComponentTemplateDescriptor, IDeleteComponentTemplateRequest>(new DeleteComponentTemplateDescriptor(name)));
    }

    public Task<DeleteComponentTemplateResponse> DeleteComponentTemplateAsync(
      Name name,
      Func<DeleteComponentTemplateDescriptor, IDeleteComponentTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteComponentTemplateAsync(selector.InvokeOrDefault<DeleteComponentTemplateDescriptor, IDeleteComponentTemplateRequest>(new DeleteComponentTemplateDescriptor(name)), ct);
    }

    public DeleteComponentTemplateResponse DeleteComponentTemplate(
      IDeleteComponentTemplateRequest request)
    {
      return this.DoRequest<IDeleteComponentTemplateRequest, DeleteComponentTemplateResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<DeleteComponentTemplateResponse> DeleteComponentTemplateAsync(
      IDeleteComponentTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteComponentTemplateRequest, DeleteComponentTemplateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteVotingConfigExclusionsResponse DeleteVotingConfigExclusions(
      Func<DeleteVotingConfigExclusionsDescriptor, IDeleteVotingConfigExclusionsRequest> selector = null)
    {
      return this.DeleteVotingConfigExclusions(selector.InvokeOrDefault<DeleteVotingConfigExclusionsDescriptor, IDeleteVotingConfigExclusionsRequest>(new DeleteVotingConfigExclusionsDescriptor()));
    }

    public Task<DeleteVotingConfigExclusionsResponse> DeleteVotingConfigExclusionsAsync(
      Func<DeleteVotingConfigExclusionsDescriptor, IDeleteVotingConfigExclusionsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteVotingConfigExclusionsAsync(selector.InvokeOrDefault<DeleteVotingConfigExclusionsDescriptor, IDeleteVotingConfigExclusionsRequest>(new DeleteVotingConfigExclusionsDescriptor()), ct);
    }

    public DeleteVotingConfigExclusionsResponse DeleteVotingConfigExclusions(
      IDeleteVotingConfigExclusionsRequest request)
    {
      return this.DoRequest<IDeleteVotingConfigExclusionsRequest, DeleteVotingConfigExclusionsResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<DeleteVotingConfigExclusionsResponse> DeleteVotingConfigExclusionsAsync(
      IDeleteVotingConfigExclusionsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteVotingConfigExclusionsRequest, DeleteVotingConfigExclusionsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExistsResponse ComponentTemplateExists(
      Name name,
      Func<ComponentTemplateExistsDescriptor, IComponentTemplateExistsRequest> selector = null)
    {
      return this.ComponentTemplateExists(selector.InvokeOrDefault<ComponentTemplateExistsDescriptor, IComponentTemplateExistsRequest>(new ComponentTemplateExistsDescriptor(name)));
    }

    public Task<ExistsResponse> ComponentTemplateExistsAsync(
      Name name,
      Func<ComponentTemplateExistsDescriptor, IComponentTemplateExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ComponentTemplateExistsAsync(selector.InvokeOrDefault<ComponentTemplateExistsDescriptor, IComponentTemplateExistsRequest>(new ComponentTemplateExistsDescriptor(name)), ct);
    }

    public ExistsResponse ComponentTemplateExists(IComponentTemplateExistsRequest request) => this.DoRequest<IComponentTemplateExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> ComponentTemplateExistsAsync(
      IComponentTemplateExistsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IComponentTemplateExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetComponentTemplateResponse GetComponentTemplate(
      Names name = null,
      Func<GetComponentTemplateDescriptor, IGetComponentTemplateRequest> selector = null)
    {
      return this.GetComponentTemplate(selector.InvokeOrDefault<GetComponentTemplateDescriptor, IGetComponentTemplateRequest>(new GetComponentTemplateDescriptor().Name(name)));
    }

    public Task<GetComponentTemplateResponse> GetComponentTemplateAsync(
      Names name = null,
      Func<GetComponentTemplateDescriptor, IGetComponentTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetComponentTemplateAsync(selector.InvokeOrDefault<GetComponentTemplateDescriptor, IGetComponentTemplateRequest>(new GetComponentTemplateDescriptor().Name(name)), ct);
    }

    public GetComponentTemplateResponse GetComponentTemplate(IGetComponentTemplateRequest request) => this.DoRequest<IGetComponentTemplateRequest, GetComponentTemplateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetComponentTemplateResponse> GetComponentTemplateAsync(
      IGetComponentTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IGetComponentTemplateRequest, GetComponentTemplateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClusterGetSettingsResponse GetSettings(
      Func<ClusterGetSettingsDescriptor, IClusterGetSettingsRequest> selector = null)
    {
      return this.GetSettings(selector.InvokeOrDefault<ClusterGetSettingsDescriptor, IClusterGetSettingsRequest>(new ClusterGetSettingsDescriptor()));
    }

    public Task<ClusterGetSettingsResponse> GetSettingsAsync(
      Func<ClusterGetSettingsDescriptor, IClusterGetSettingsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetSettingsAsync(selector.InvokeOrDefault<ClusterGetSettingsDescriptor, IClusterGetSettingsRequest>(new ClusterGetSettingsDescriptor()), ct);
    }

    public ClusterGetSettingsResponse GetSettings(IClusterGetSettingsRequest request) => this.DoRequest<IClusterGetSettingsRequest, ClusterGetSettingsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClusterGetSettingsResponse> GetSettingsAsync(
      IClusterGetSettingsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClusterGetSettingsRequest, ClusterGetSettingsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClusterHealthResponse Health(
      Indices index = null,
      Func<ClusterHealthDescriptor, IClusterHealthRequest> selector = null)
    {
      return this.Health(selector.InvokeOrDefault<ClusterHealthDescriptor, IClusterHealthRequest>(new ClusterHealthDescriptor().Index(index)));
    }

    public Task<ClusterHealthResponse> HealthAsync(
      Indices index = null,
      Func<ClusterHealthDescriptor, IClusterHealthRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.HealthAsync(selector.InvokeOrDefault<ClusterHealthDescriptor, IClusterHealthRequest>(new ClusterHealthDescriptor().Index(index)), ct);
    }

    public ClusterHealthResponse Health(IClusterHealthRequest request) => this.DoRequest<IClusterHealthRequest, ClusterHealthResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClusterHealthResponse> HealthAsync(
      IClusterHealthRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClusterHealthRequest, ClusterHealthResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClusterPendingTasksResponse PendingTasks(
      Func<ClusterPendingTasksDescriptor, IClusterPendingTasksRequest> selector = null)
    {
      return this.PendingTasks(selector.InvokeOrDefault<ClusterPendingTasksDescriptor, IClusterPendingTasksRequest>(new ClusterPendingTasksDescriptor()));
    }

    public Task<ClusterPendingTasksResponse> PendingTasksAsync(
      Func<ClusterPendingTasksDescriptor, IClusterPendingTasksRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PendingTasksAsync(selector.InvokeOrDefault<ClusterPendingTasksDescriptor, IClusterPendingTasksRequest>(new ClusterPendingTasksDescriptor()), ct);
    }

    public ClusterPendingTasksResponse PendingTasks(IClusterPendingTasksRequest request) => this.DoRequest<IClusterPendingTasksRequest, ClusterPendingTasksResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClusterPendingTasksResponse> PendingTasksAsync(
      IClusterPendingTasksRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClusterPendingTasksRequest, ClusterPendingTasksResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PostVotingConfigExclusionsResponse PostVotingConfigExclusions(
      Func<PostVotingConfigExclusionsDescriptor, IPostVotingConfigExclusionsRequest> selector = null)
    {
      return this.PostVotingConfigExclusions(selector.InvokeOrDefault<PostVotingConfigExclusionsDescriptor, IPostVotingConfigExclusionsRequest>(new PostVotingConfigExclusionsDescriptor()));
    }

    public Task<PostVotingConfigExclusionsResponse> PostVotingConfigExclusionsAsync(
      Func<PostVotingConfigExclusionsDescriptor, IPostVotingConfigExclusionsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PostVotingConfigExclusionsAsync(selector.InvokeOrDefault<PostVotingConfigExclusionsDescriptor, IPostVotingConfigExclusionsRequest>(new PostVotingConfigExclusionsDescriptor()), ct);
    }

    public PostVotingConfigExclusionsResponse PostVotingConfigExclusions(
      IPostVotingConfigExclusionsRequest request)
    {
      return this.DoRequest<IPostVotingConfigExclusionsRequest, PostVotingConfigExclusionsResponse>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<PostVotingConfigExclusionsResponse> PostVotingConfigExclusionsAsync(
      IPostVotingConfigExclusionsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPostVotingConfigExclusionsRequest, PostVotingConfigExclusionsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutComponentTemplateResponse PutComponentTemplate(
      Name name,
      Func<PutComponentTemplateDescriptor, IPutComponentTemplateRequest> selector)
    {
      return this.PutComponentTemplate(selector.InvokeOrDefault<PutComponentTemplateDescriptor, IPutComponentTemplateRequest>(new PutComponentTemplateDescriptor(name)));
    }

    public Task<PutComponentTemplateResponse> PutComponentTemplateAsync(
      Name name,
      Func<PutComponentTemplateDescriptor, IPutComponentTemplateRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutComponentTemplateAsync(selector.InvokeOrDefault<PutComponentTemplateDescriptor, IPutComponentTemplateRequest>(new PutComponentTemplateDescriptor(name)), ct);
    }

    public PutComponentTemplateResponse PutComponentTemplate(IPutComponentTemplateRequest request) => this.DoRequest<IPutComponentTemplateRequest, PutComponentTemplateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutComponentTemplateResponse> PutComponentTemplateAsync(
      IPutComponentTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IPutComponentTemplateRequest, PutComponentTemplateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClusterPutSettingsResponse PutSettings(
      Func<ClusterPutSettingsDescriptor, IClusterPutSettingsRequest> selector)
    {
      return this.PutSettings(selector.InvokeOrDefault<ClusterPutSettingsDescriptor, IClusterPutSettingsRequest>(new ClusterPutSettingsDescriptor()));
    }

    public Task<ClusterPutSettingsResponse> PutSettingsAsync(
      Func<ClusterPutSettingsDescriptor, IClusterPutSettingsRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutSettingsAsync(selector.InvokeOrDefault<ClusterPutSettingsDescriptor, IClusterPutSettingsRequest>(new ClusterPutSettingsDescriptor()), ct);
    }

    public ClusterPutSettingsResponse PutSettings(IClusterPutSettingsRequest request) => this.DoRequest<IClusterPutSettingsRequest, ClusterPutSettingsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClusterPutSettingsResponse> PutSettingsAsync(
      IClusterPutSettingsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClusterPutSettingsRequest, ClusterPutSettingsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RemoteInfoResponse RemoteInfo(
      Func<RemoteInfoDescriptor, IRemoteInfoRequest> selector = null)
    {
      return this.RemoteInfo(selector.InvokeOrDefault<RemoteInfoDescriptor, IRemoteInfoRequest>(new RemoteInfoDescriptor()));
    }

    public Task<RemoteInfoResponse> RemoteInfoAsync(
      Func<RemoteInfoDescriptor, IRemoteInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RemoteInfoAsync(selector.InvokeOrDefault<RemoteInfoDescriptor, IRemoteInfoRequest>(new RemoteInfoDescriptor()), ct);
    }

    public RemoteInfoResponse RemoteInfo(IRemoteInfoRequest request) => this.DoRequest<IRemoteInfoRequest, RemoteInfoResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RemoteInfoResponse> RemoteInfoAsync(
      IRemoteInfoRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IRemoteInfoRequest, RemoteInfoResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClusterRerouteResponse Reroute(
      Func<ClusterRerouteDescriptor, IClusterRerouteRequest> selector = null)
    {
      return this.Reroute(selector.InvokeOrDefault<ClusterRerouteDescriptor, IClusterRerouteRequest>(new ClusterRerouteDescriptor()));
    }

    public Task<ClusterRerouteResponse> RerouteAsync(
      Func<ClusterRerouteDescriptor, IClusterRerouteRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RerouteAsync(selector.InvokeOrDefault<ClusterRerouteDescriptor, IClusterRerouteRequest>(new ClusterRerouteDescriptor()), ct);
    }

    public ClusterRerouteResponse Reroute(IClusterRerouteRequest request) => this.DoRequest<IClusterRerouteRequest, ClusterRerouteResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClusterRerouteResponse> RerouteAsync(
      IClusterRerouteRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClusterRerouteRequest, ClusterRerouteResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClusterStateResponse State(
      Indices index = null,
      Func<ClusterStateDescriptor, IClusterStateRequest> selector = null)
    {
      return this.State(selector.InvokeOrDefault<ClusterStateDescriptor, IClusterStateRequest>(new ClusterStateDescriptor().Index(index)));
    }

    public Task<ClusterStateResponse> StateAsync(
      Indices index = null,
      Func<ClusterStateDescriptor, IClusterStateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StateAsync(selector.InvokeOrDefault<ClusterStateDescriptor, IClusterStateRequest>(new ClusterStateDescriptor().Index(index)), ct);
    }

    public ClusterStateResponse State(IClusterStateRequest request) => this.DoRequest<IClusterStateRequest, ClusterStateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClusterStateResponse> StateAsync(IClusterStateRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IClusterStateRequest, ClusterStateResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ClusterStatsResponse Stats(
      Func<ClusterStatsDescriptor, IClusterStatsRequest> selector = null)
    {
      return this.Stats(selector.InvokeOrDefault<ClusterStatsDescriptor, IClusterStatsRequest>(new ClusterStatsDescriptor()));
    }

    public Task<ClusterStatsResponse> StatsAsync(
      Func<ClusterStatsDescriptor, IClusterStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatsAsync(selector.InvokeOrDefault<ClusterStatsDescriptor, IClusterStatsRequest>(new ClusterStatsDescriptor()), ct);
    }

    public ClusterStatsResponse Stats(IClusterStatsRequest request) => this.DoRequest<IClusterStatsRequest, ClusterStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClusterStatsResponse> StatsAsync(IClusterStatsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IClusterStatsRequest, ClusterStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
