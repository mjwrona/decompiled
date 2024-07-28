// Decompiled with JetBrains decompiler
// Type: Nest.Specification.NodesApi.NodesNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.NodesApi
{
  public class NodesNamespace : Nest.NamespacedClientProxy
  {
    internal NodesNamespace(ElasticClient client)
      : base(client)
    {
    }

    public NodesHotThreadsResponse HotThreads(
      Func<NodesHotThreadsDescriptor, INodesHotThreadsRequest> selector = null)
    {
      return this.HotThreads(selector.InvokeOrDefault<NodesHotThreadsDescriptor, INodesHotThreadsRequest>(new NodesHotThreadsDescriptor()));
    }

    public Task<NodesHotThreadsResponse> HotThreadsAsync(
      Func<NodesHotThreadsDescriptor, INodesHotThreadsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.HotThreadsAsync(selector.InvokeOrDefault<NodesHotThreadsDescriptor, INodesHotThreadsRequest>(new NodesHotThreadsDescriptor()), ct);
    }

    public NodesHotThreadsResponse HotThreads(INodesHotThreadsRequest request) => this.DoRequest<INodesHotThreadsRequest, NodesHotThreadsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<NodesHotThreadsResponse> HotThreadsAsync(
      INodesHotThreadsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<INodesHotThreadsRequest, NodesHotThreadsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public NodesInfoResponse Info(
      Func<NodesInfoDescriptor, INodesInfoRequest> selector = null)
    {
      return this.Info(selector.InvokeOrDefault<NodesInfoDescriptor, INodesInfoRequest>(new NodesInfoDescriptor()));
    }

    public Task<NodesInfoResponse> InfoAsync(
      Func<NodesInfoDescriptor, INodesInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.InfoAsync(selector.InvokeOrDefault<NodesInfoDescriptor, INodesInfoRequest>(new NodesInfoDescriptor()), ct);
    }

    public NodesInfoResponse Info(INodesInfoRequest request) => this.DoRequest<INodesInfoRequest, NodesInfoResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<NodesInfoResponse> InfoAsync(INodesInfoRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<INodesInfoRequest, NodesInfoResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ReloadSecureSettingsResponse ReloadSecureSettings(
      Func<ReloadSecureSettingsDescriptor, IReloadSecureSettingsRequest> selector = null)
    {
      return this.ReloadSecureSettings(selector.InvokeOrDefault<ReloadSecureSettingsDescriptor, IReloadSecureSettingsRequest>(new ReloadSecureSettingsDescriptor()));
    }

    public Task<ReloadSecureSettingsResponse> ReloadSecureSettingsAsync(
      Func<ReloadSecureSettingsDescriptor, IReloadSecureSettingsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ReloadSecureSettingsAsync(selector.InvokeOrDefault<ReloadSecureSettingsDescriptor, IReloadSecureSettingsRequest>(new ReloadSecureSettingsDescriptor()), ct);
    }

    public ReloadSecureSettingsResponse ReloadSecureSettings(IReloadSecureSettingsRequest request) => this.DoRequest<IReloadSecureSettingsRequest, ReloadSecureSettingsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ReloadSecureSettingsResponse> ReloadSecureSettingsAsync(
      IReloadSecureSettingsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IReloadSecureSettingsRequest, ReloadSecureSettingsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public NodesStatsResponse Stats(
      Func<NodesStatsDescriptor, INodesStatsRequest> selector = null)
    {
      return this.Stats(selector.InvokeOrDefault<NodesStatsDescriptor, INodesStatsRequest>(new NodesStatsDescriptor()));
    }

    public Task<NodesStatsResponse> StatsAsync(
      Func<NodesStatsDescriptor, INodesStatsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.StatsAsync(selector.InvokeOrDefault<NodesStatsDescriptor, INodesStatsRequest>(new NodesStatsDescriptor()), ct);
    }

    public NodesStatsResponse Stats(INodesStatsRequest request) => this.DoRequest<INodesStatsRequest, NodesStatsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<NodesStatsResponse> StatsAsync(INodesStatsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<INodesStatsRequest, NodesStatsResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public NodesUsageResponse Usage(
      Func<NodesUsageDescriptor, INodesUsageRequest> selector = null)
    {
      return this.Usage(selector.InvokeOrDefault<NodesUsageDescriptor, INodesUsageRequest>(new NodesUsageDescriptor()));
    }

    public Task<NodesUsageResponse> UsageAsync(
      Func<NodesUsageDescriptor, INodesUsageRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UsageAsync(selector.InvokeOrDefault<NodesUsageDescriptor, INodesUsageRequest>(new NodesUsageDescriptor()), ct);
    }

    public NodesUsageResponse Usage(INodesUsageRequest request) => this.DoRequest<INodesUsageRequest, NodesUsageResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<NodesUsageResponse> UsageAsync(INodesUsageRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<INodesUsageRequest, NodesUsageResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
