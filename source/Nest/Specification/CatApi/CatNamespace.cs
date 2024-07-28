// Decompiled with JetBrains decompiler
// Type: Nest.Specification.CatApi.CatNamespace
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nest.Specification.CatApi
{
  public class CatNamespace : NamespacedClientProxy
  {
    internal CatNamespace(ElasticClient client)
      : base(client)
    {
    }

    public CatResponse<CatAliasesRecord> Aliases(
      Func<CatAliasesDescriptor, ICatAliasesRequest> selector = null)
    {
      return this.Aliases(selector.InvokeOrDefault<CatAliasesDescriptor, ICatAliasesRequest>(new CatAliasesDescriptor()));
    }

    public Task<CatResponse<CatAliasesRecord>> AliasesAsync(
      Func<CatAliasesDescriptor, ICatAliasesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AliasesAsync(selector.InvokeOrDefault<CatAliasesDescriptor, ICatAliasesRequest>(new CatAliasesDescriptor()), ct);
    }

    public CatResponse<CatAliasesRecord> Aliases(ICatAliasesRequest request) => this.DoCat<ICatAliasesRequest, CatAliasesRequestParameters, CatAliasesRecord>(request);

    public Task<CatResponse<CatAliasesRecord>> AliasesAsync(
      ICatAliasesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatAliasesRequest, CatAliasesRequestParameters, CatAliasesRecord>(request, ct);
    }

    public CatResponse<CatAllocationRecord> Allocation(
      Func<CatAllocationDescriptor, ICatAllocationRequest> selector = null)
    {
      return this.Allocation(selector.InvokeOrDefault<CatAllocationDescriptor, ICatAllocationRequest>(new CatAllocationDescriptor()));
    }

    public Task<CatResponse<CatAllocationRecord>> AllocationAsync(
      Func<CatAllocationDescriptor, ICatAllocationRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.AllocationAsync(selector.InvokeOrDefault<CatAllocationDescriptor, ICatAllocationRequest>(new CatAllocationDescriptor()), ct);
    }

    public CatResponse<CatAllocationRecord> Allocation(ICatAllocationRequest request) => this.DoCat<ICatAllocationRequest, CatAllocationRequestParameters, CatAllocationRecord>(request);

    public Task<CatResponse<CatAllocationRecord>> AllocationAsync(
      ICatAllocationRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatAllocationRequest, CatAllocationRequestParameters, CatAllocationRecord>(request, ct);
    }

    public CatResponse<CatCountRecord> Count(
      Func<CatCountDescriptor, ICatCountRequest> selector = null)
    {
      return this.Count(selector.InvokeOrDefault<CatCountDescriptor, ICatCountRequest>(new CatCountDescriptor()));
    }

    public Task<CatResponse<CatCountRecord>> CountAsync(
      Func<CatCountDescriptor, ICatCountRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.CountAsync(selector.InvokeOrDefault<CatCountDescriptor, ICatCountRequest>(new CatCountDescriptor()), ct);
    }

    public CatResponse<CatCountRecord> Count(ICatCountRequest request) => this.DoCat<ICatCountRequest, CatCountRequestParameters, CatCountRecord>(request);

    public Task<CatResponse<CatCountRecord>> CountAsync(
      ICatCountRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatCountRequest, CatCountRequestParameters, CatCountRecord>(request, ct);
    }

    public CatResponse<CatFielddataRecord> Fielddata(
      Func<CatFielddataDescriptor, ICatFielddataRequest> selector = null)
    {
      return this.Fielddata(selector.InvokeOrDefault<CatFielddataDescriptor, ICatFielddataRequest>(new CatFielddataDescriptor()));
    }

    public Task<CatResponse<CatFielddataRecord>> FielddataAsync(
      Func<CatFielddataDescriptor, ICatFielddataRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.FielddataAsync(selector.InvokeOrDefault<CatFielddataDescriptor, ICatFielddataRequest>(new CatFielddataDescriptor()), ct);
    }

    public CatResponse<CatFielddataRecord> Fielddata(ICatFielddataRequest request) => this.DoCat<ICatFielddataRequest, CatFielddataRequestParameters, CatFielddataRecord>(request);

    public Task<CatResponse<CatFielddataRecord>> FielddataAsync(
      ICatFielddataRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatFielddataRequest, CatFielddataRequestParameters, CatFielddataRecord>(request, ct);
    }

    public CatResponse<CatHealthRecord> Health(
      Func<CatHealthDescriptor, ICatHealthRequest> selector = null)
    {
      return this.Health(selector.InvokeOrDefault<CatHealthDescriptor, ICatHealthRequest>(new CatHealthDescriptor()));
    }

    public Task<CatResponse<CatHealthRecord>> HealthAsync(
      Func<CatHealthDescriptor, ICatHealthRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.HealthAsync(selector.InvokeOrDefault<CatHealthDescriptor, ICatHealthRequest>(new CatHealthDescriptor()), ct);
    }

    public CatResponse<CatHealthRecord> Health(ICatHealthRequest request) => this.DoCat<ICatHealthRequest, CatHealthRequestParameters, CatHealthRecord>(request);

    public Task<CatResponse<CatHealthRecord>> HealthAsync(
      ICatHealthRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatHealthRequest, CatHealthRequestParameters, CatHealthRecord>(request, ct);
    }

    public CatResponse<CatHelpRecord> Help(Func<CatHelpDescriptor, ICatHelpRequest> selector = null) => this.Help(selector.InvokeOrDefault<CatHelpDescriptor, ICatHelpRequest>(new CatHelpDescriptor()));

    public Task<CatResponse<CatHelpRecord>> HelpAsync(
      Func<CatHelpDescriptor, ICatHelpRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.HelpAsync(selector.InvokeOrDefault<CatHelpDescriptor, ICatHelpRequest>(new CatHelpDescriptor()), ct);
    }

    public CatResponse<CatHelpRecord> Help(ICatHelpRequest request) => this.DoCat<ICatHelpRequest, CatHelpRequestParameters, CatHelpRecord>(request);

    public Task<CatResponse<CatHelpRecord>> HelpAsync(ICatHelpRequest request, CancellationToken ct = default (CancellationToken)) => this.DoCatAsync<ICatHelpRequest, CatHelpRequestParameters, CatHelpRecord>(request, ct);

    public CatResponse<CatIndicesRecord> Indices(
      Func<CatIndicesDescriptor, ICatIndicesRequest> selector = null)
    {
      return this.Indices(selector.InvokeOrDefault<CatIndicesDescriptor, ICatIndicesRequest>(new CatIndicesDescriptor()));
    }

    public Task<CatResponse<CatIndicesRecord>> IndicesAsync(
      Func<CatIndicesDescriptor, ICatIndicesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.IndicesAsync(selector.InvokeOrDefault<CatIndicesDescriptor, ICatIndicesRequest>(new CatIndicesDescriptor()), ct);
    }

    public CatResponse<CatIndicesRecord> Indices(ICatIndicesRequest request) => this.DoCat<ICatIndicesRequest, CatIndicesRequestParameters, CatIndicesRecord>(request);

    public Task<CatResponse<CatIndicesRecord>> IndicesAsync(
      ICatIndicesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatIndicesRequest, CatIndicesRequestParameters, CatIndicesRecord>(request, ct);
    }

    public CatResponse<CatMasterRecord> Master(
      Func<CatMasterDescriptor, ICatMasterRequest> selector = null)
    {
      return this.Master(selector.InvokeOrDefault<CatMasterDescriptor, ICatMasterRequest>(new CatMasterDescriptor()));
    }

    public Task<CatResponse<CatMasterRecord>> MasterAsync(
      Func<CatMasterDescriptor, ICatMasterRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MasterAsync(selector.InvokeOrDefault<CatMasterDescriptor, ICatMasterRequest>(new CatMasterDescriptor()), ct);
    }

    public CatResponse<CatMasterRecord> Master(ICatMasterRequest request) => this.DoCat<ICatMasterRequest, CatMasterRequestParameters, CatMasterRecord>(request);

    public Task<CatResponse<CatMasterRecord>> MasterAsync(
      ICatMasterRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatMasterRequest, CatMasterRequestParameters, CatMasterRecord>(request, ct);
    }

    public CatResponse<CatDataFrameAnalyticsRecord> DataFrameAnalytics(
      Func<CatDataFrameAnalyticsDescriptor, ICatDataFrameAnalyticsRequest> selector = null)
    {
      return this.DataFrameAnalytics(selector.InvokeOrDefault<CatDataFrameAnalyticsDescriptor, ICatDataFrameAnalyticsRequest>(new CatDataFrameAnalyticsDescriptor()));
    }

    public Task<CatResponse<CatDataFrameAnalyticsRecord>> DataFrameAnalyticsAsync(
      Func<CatDataFrameAnalyticsDescriptor, ICatDataFrameAnalyticsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DataFrameAnalyticsAsync(selector.InvokeOrDefault<CatDataFrameAnalyticsDescriptor, ICatDataFrameAnalyticsRequest>(new CatDataFrameAnalyticsDescriptor()), ct);
    }

    public CatResponse<CatDataFrameAnalyticsRecord> DataFrameAnalytics(
      ICatDataFrameAnalyticsRequest request)
    {
      return this.DoCat<ICatDataFrameAnalyticsRequest, CatDataFrameAnalyticsRequestParameters, CatDataFrameAnalyticsRecord>(request);
    }

    public Task<CatResponse<CatDataFrameAnalyticsRecord>> DataFrameAnalyticsAsync(
      ICatDataFrameAnalyticsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatDataFrameAnalyticsRequest, CatDataFrameAnalyticsRequestParameters, CatDataFrameAnalyticsRecord>(request, ct);
    }

    public CatResponse<CatDatafeedsRecord> Datafeeds(
      Func<CatDatafeedsDescriptor, ICatDatafeedsRequest> selector = null)
    {
      return this.Datafeeds(selector.InvokeOrDefault<CatDatafeedsDescriptor, ICatDatafeedsRequest>(new CatDatafeedsDescriptor()));
    }

    public Task<CatResponse<CatDatafeedsRecord>> DatafeedsAsync(
      Func<CatDatafeedsDescriptor, ICatDatafeedsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DatafeedsAsync(selector.InvokeOrDefault<CatDatafeedsDescriptor, ICatDatafeedsRequest>(new CatDatafeedsDescriptor()), ct);
    }

    public CatResponse<CatDatafeedsRecord> Datafeeds(ICatDatafeedsRequest request) => this.DoCat<ICatDatafeedsRequest, CatDatafeedsRequestParameters, CatDatafeedsRecord>(request);

    public Task<CatResponse<CatDatafeedsRecord>> DatafeedsAsync(
      ICatDatafeedsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatDatafeedsRequest, CatDatafeedsRequestParameters, CatDatafeedsRecord>(request, ct);
    }

    public CatResponse<CatJobsRecord> Jobs(Func<CatJobsDescriptor, ICatJobsRequest> selector = null) => this.Jobs(selector.InvokeOrDefault<CatJobsDescriptor, ICatJobsRequest>(new CatJobsDescriptor()));

    public Task<CatResponse<CatJobsRecord>> JobsAsync(
      Func<CatJobsDescriptor, ICatJobsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.JobsAsync(selector.InvokeOrDefault<CatJobsDescriptor, ICatJobsRequest>(new CatJobsDescriptor()), ct);
    }

    public CatResponse<CatJobsRecord> Jobs(ICatJobsRequest request) => this.DoCat<ICatJobsRequest, CatJobsRequestParameters, CatJobsRecord>(request);

    public Task<CatResponse<CatJobsRecord>> JobsAsync(ICatJobsRequest request, CancellationToken ct = default (CancellationToken)) => this.DoCatAsync<ICatJobsRequest, CatJobsRequestParameters, CatJobsRecord>(request, ct);

    public CatResponse<CatTrainedModelsRecord> TrainedModels(
      Func<CatTrainedModelsDescriptor, ICatTrainedModelsRequest> selector = null)
    {
      return this.TrainedModels(selector.InvokeOrDefault<CatTrainedModelsDescriptor, ICatTrainedModelsRequest>(new CatTrainedModelsDescriptor()));
    }

    public Task<CatResponse<CatTrainedModelsRecord>> TrainedModelsAsync(
      Func<CatTrainedModelsDescriptor, ICatTrainedModelsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TrainedModelsAsync(selector.InvokeOrDefault<CatTrainedModelsDescriptor, ICatTrainedModelsRequest>(new CatTrainedModelsDescriptor()), ct);
    }

    public CatResponse<CatTrainedModelsRecord> TrainedModels(ICatTrainedModelsRequest request) => this.DoCat<ICatTrainedModelsRequest, CatTrainedModelsRequestParameters, CatTrainedModelsRecord>(request);

    public Task<CatResponse<CatTrainedModelsRecord>> TrainedModelsAsync(
      ICatTrainedModelsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatTrainedModelsRequest, CatTrainedModelsRequestParameters, CatTrainedModelsRecord>(request, ct);
    }

    public CatResponse<CatNodeAttributesRecord> NodeAttributes(
      Func<CatNodeAttributesDescriptor, ICatNodeAttributesRequest> selector = null)
    {
      return this.NodeAttributes(selector.InvokeOrDefault<CatNodeAttributesDescriptor, ICatNodeAttributesRequest>(new CatNodeAttributesDescriptor()));
    }

    public Task<CatResponse<CatNodeAttributesRecord>> NodeAttributesAsync(
      Func<CatNodeAttributesDescriptor, ICatNodeAttributesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.NodeAttributesAsync(selector.InvokeOrDefault<CatNodeAttributesDescriptor, ICatNodeAttributesRequest>(new CatNodeAttributesDescriptor()), ct);
    }

    public CatResponse<CatNodeAttributesRecord> NodeAttributes(ICatNodeAttributesRequest request) => this.DoCat<ICatNodeAttributesRequest, CatNodeAttributesRequestParameters, CatNodeAttributesRecord>(request);

    public Task<CatResponse<CatNodeAttributesRecord>> NodeAttributesAsync(
      ICatNodeAttributesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatNodeAttributesRequest, CatNodeAttributesRequestParameters, CatNodeAttributesRecord>(request, ct);
    }

    public CatResponse<CatNodesRecord> Nodes(
      Func<CatNodesDescriptor, ICatNodesRequest> selector = null)
    {
      return this.Nodes(selector.InvokeOrDefault<CatNodesDescriptor, ICatNodesRequest>(new CatNodesDescriptor()));
    }

    public Task<CatResponse<CatNodesRecord>> NodesAsync(
      Func<CatNodesDescriptor, ICatNodesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.NodesAsync(selector.InvokeOrDefault<CatNodesDescriptor, ICatNodesRequest>(new CatNodesDescriptor()), ct);
    }

    public CatResponse<CatNodesRecord> Nodes(ICatNodesRequest request) => this.DoCat<ICatNodesRequest, CatNodesRequestParameters, CatNodesRecord>(request);

    public Task<CatResponse<CatNodesRecord>> NodesAsync(
      ICatNodesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatNodesRequest, CatNodesRequestParameters, CatNodesRecord>(request, ct);
    }

    public CatResponse<CatPendingTasksRecord> PendingTasks(
      Func<CatPendingTasksDescriptor, ICatPendingTasksRequest> selector = null)
    {
      return this.PendingTasks(selector.InvokeOrDefault<CatPendingTasksDescriptor, ICatPendingTasksRequest>(new CatPendingTasksDescriptor()));
    }

    public Task<CatResponse<CatPendingTasksRecord>> PendingTasksAsync(
      Func<CatPendingTasksDescriptor, ICatPendingTasksRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PendingTasksAsync(selector.InvokeOrDefault<CatPendingTasksDescriptor, ICatPendingTasksRequest>(new CatPendingTasksDescriptor()), ct);
    }

    public CatResponse<CatPendingTasksRecord> PendingTasks(ICatPendingTasksRequest request) => this.DoCat<ICatPendingTasksRequest, CatPendingTasksRequestParameters, CatPendingTasksRecord>(request);

    public Task<CatResponse<CatPendingTasksRecord>> PendingTasksAsync(
      ICatPendingTasksRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatPendingTasksRequest, CatPendingTasksRequestParameters, CatPendingTasksRecord>(request, ct);
    }

    public CatResponse<CatPluginsRecord> Plugins(
      Func<CatPluginsDescriptor, ICatPluginsRequest> selector = null)
    {
      return this.Plugins(selector.InvokeOrDefault<CatPluginsDescriptor, ICatPluginsRequest>(new CatPluginsDescriptor()));
    }

    public Task<CatResponse<CatPluginsRecord>> PluginsAsync(
      Func<CatPluginsDescriptor, ICatPluginsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PluginsAsync(selector.InvokeOrDefault<CatPluginsDescriptor, ICatPluginsRequest>(new CatPluginsDescriptor()), ct);
    }

    public CatResponse<CatPluginsRecord> Plugins(ICatPluginsRequest request) => this.DoCat<ICatPluginsRequest, CatPluginsRequestParameters, CatPluginsRecord>(request);

    public Task<CatResponse<CatPluginsRecord>> PluginsAsync(
      ICatPluginsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatPluginsRequest, CatPluginsRequestParameters, CatPluginsRecord>(request, ct);
    }

    public CatResponse<CatRecoveryRecord> Recovery(
      Func<CatRecoveryDescriptor, ICatRecoveryRequest> selector = null)
    {
      return this.Recovery(selector.InvokeOrDefault<CatRecoveryDescriptor, ICatRecoveryRequest>(new CatRecoveryDescriptor()));
    }

    public Task<CatResponse<CatRecoveryRecord>> RecoveryAsync(
      Func<CatRecoveryDescriptor, ICatRecoveryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RecoveryAsync(selector.InvokeOrDefault<CatRecoveryDescriptor, ICatRecoveryRequest>(new CatRecoveryDescriptor()), ct);
    }

    public CatResponse<CatRecoveryRecord> Recovery(ICatRecoveryRequest request) => this.DoCat<ICatRecoveryRequest, CatRecoveryRequestParameters, CatRecoveryRecord>(request);

    public Task<CatResponse<CatRecoveryRecord>> RecoveryAsync(
      ICatRecoveryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatRecoveryRequest, CatRecoveryRequestParameters, CatRecoveryRecord>(request, ct);
    }

    public CatResponse<CatRepositoriesRecord> Repositories(
      Func<CatRepositoriesDescriptor, ICatRepositoriesRequest> selector = null)
    {
      return this.Repositories(selector.InvokeOrDefault<CatRepositoriesDescriptor, ICatRepositoriesRequest>(new CatRepositoriesDescriptor()));
    }

    public Task<CatResponse<CatRepositoriesRecord>> RepositoriesAsync(
      Func<CatRepositoriesDescriptor, ICatRepositoriesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RepositoriesAsync(selector.InvokeOrDefault<CatRepositoriesDescriptor, ICatRepositoriesRequest>(new CatRepositoriesDescriptor()), ct);
    }

    public CatResponse<CatRepositoriesRecord> Repositories(ICatRepositoriesRequest request) => this.DoCat<ICatRepositoriesRequest, CatRepositoriesRequestParameters, CatRepositoriesRecord>(request);

    public Task<CatResponse<CatRepositoriesRecord>> RepositoriesAsync(
      ICatRepositoriesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatRepositoriesRequest, CatRepositoriesRequestParameters, CatRepositoriesRecord>(request, ct);
    }

    public CatResponse<CatSegmentsRecord> Segments(
      Func<CatSegmentsDescriptor, ICatSegmentsRequest> selector = null)
    {
      return this.Segments(selector.InvokeOrDefault<CatSegmentsDescriptor, ICatSegmentsRequest>(new CatSegmentsDescriptor()));
    }

    public Task<CatResponse<CatSegmentsRecord>> SegmentsAsync(
      Func<CatSegmentsDescriptor, ICatSegmentsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SegmentsAsync(selector.InvokeOrDefault<CatSegmentsDescriptor, ICatSegmentsRequest>(new CatSegmentsDescriptor()), ct);
    }

    public CatResponse<CatSegmentsRecord> Segments(ICatSegmentsRequest request) => this.DoCat<ICatSegmentsRequest, CatSegmentsRequestParameters, CatSegmentsRecord>(request);

    public Task<CatResponse<CatSegmentsRecord>> SegmentsAsync(
      ICatSegmentsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatSegmentsRequest, CatSegmentsRequestParameters, CatSegmentsRecord>(request, ct);
    }

    public CatResponse<CatShardsRecord> Shards(
      Func<CatShardsDescriptor, ICatShardsRequest> selector = null)
    {
      return this.Shards(selector.InvokeOrDefault<CatShardsDescriptor, ICatShardsRequest>(new CatShardsDescriptor()));
    }

    public Task<CatResponse<CatShardsRecord>> ShardsAsync(
      Func<CatShardsDescriptor, ICatShardsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ShardsAsync(selector.InvokeOrDefault<CatShardsDescriptor, ICatShardsRequest>(new CatShardsDescriptor()), ct);
    }

    public CatResponse<CatShardsRecord> Shards(ICatShardsRequest request) => this.DoCat<ICatShardsRequest, CatShardsRequestParameters, CatShardsRecord>(request);

    public Task<CatResponse<CatShardsRecord>> ShardsAsync(
      ICatShardsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatShardsRequest, CatShardsRequestParameters, CatShardsRecord>(request, ct);
    }

    public CatResponse<CatSnapshotsRecord> Snapshots(
      Func<CatSnapshotsDescriptor, ICatSnapshotsRequest> selector = null)
    {
      return this.Snapshots(selector.InvokeOrDefault<CatSnapshotsDescriptor, ICatSnapshotsRequest>(new CatSnapshotsDescriptor()));
    }

    public Task<CatResponse<CatSnapshotsRecord>> SnapshotsAsync(
      Func<CatSnapshotsDescriptor, ICatSnapshotsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.SnapshotsAsync(selector.InvokeOrDefault<CatSnapshotsDescriptor, ICatSnapshotsRequest>(new CatSnapshotsDescriptor()), ct);
    }

    public CatResponse<CatSnapshotsRecord> Snapshots(ICatSnapshotsRequest request) => this.DoCat<ICatSnapshotsRequest, CatSnapshotsRequestParameters, CatSnapshotsRecord>(request);

    public Task<CatResponse<CatSnapshotsRecord>> SnapshotsAsync(
      ICatSnapshotsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatSnapshotsRequest, CatSnapshotsRequestParameters, CatSnapshotsRecord>(request, ct);
    }

    public CatResponse<CatTasksRecord> Tasks(
      Func<CatTasksDescriptor, ICatTasksRequest> selector = null)
    {
      return this.Tasks(selector.InvokeOrDefault<CatTasksDescriptor, ICatTasksRequest>(new CatTasksDescriptor()));
    }

    public Task<CatResponse<CatTasksRecord>> TasksAsync(
      Func<CatTasksDescriptor, ICatTasksRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TasksAsync(selector.InvokeOrDefault<CatTasksDescriptor, ICatTasksRequest>(new CatTasksDescriptor()), ct);
    }

    public CatResponse<CatTasksRecord> Tasks(ICatTasksRequest request) => this.DoCat<ICatTasksRequest, CatTasksRequestParameters, CatTasksRecord>(request);

    public Task<CatResponse<CatTasksRecord>> TasksAsync(
      ICatTasksRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatTasksRequest, CatTasksRequestParameters, CatTasksRecord>(request, ct);
    }

    public CatResponse<CatTemplatesRecord> Templates(
      Func<CatTemplatesDescriptor, ICatTemplatesRequest> selector = null)
    {
      return this.Templates(selector.InvokeOrDefault<CatTemplatesDescriptor, ICatTemplatesRequest>(new CatTemplatesDescriptor()));
    }

    public Task<CatResponse<CatTemplatesRecord>> TemplatesAsync(
      Func<CatTemplatesDescriptor, ICatTemplatesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TemplatesAsync(selector.InvokeOrDefault<CatTemplatesDescriptor, ICatTemplatesRequest>(new CatTemplatesDescriptor()), ct);
    }

    public CatResponse<CatTemplatesRecord> Templates(ICatTemplatesRequest request) => this.DoCat<ICatTemplatesRequest, CatTemplatesRequestParameters, CatTemplatesRecord>(request);

    public Task<CatResponse<CatTemplatesRecord>> TemplatesAsync(
      ICatTemplatesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatTemplatesRequest, CatTemplatesRequestParameters, CatTemplatesRecord>(request, ct);
    }

    public CatResponse<CatThreadPoolRecord> ThreadPool(
      Func<CatThreadPoolDescriptor, ICatThreadPoolRequest> selector = null)
    {
      return this.ThreadPool(selector.InvokeOrDefault<CatThreadPoolDescriptor, ICatThreadPoolRequest>(new CatThreadPoolDescriptor()));
    }

    public Task<CatResponse<CatThreadPoolRecord>> ThreadPoolAsync(
      Func<CatThreadPoolDescriptor, ICatThreadPoolRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ThreadPoolAsync(selector.InvokeOrDefault<CatThreadPoolDescriptor, ICatThreadPoolRequest>(new CatThreadPoolDescriptor()), ct);
    }

    public CatResponse<CatThreadPoolRecord> ThreadPool(ICatThreadPoolRequest request) => this.DoCat<ICatThreadPoolRequest, CatThreadPoolRequestParameters, CatThreadPoolRecord>(request);

    public Task<CatResponse<CatThreadPoolRecord>> ThreadPoolAsync(
      ICatThreadPoolRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatThreadPoolRequest, CatThreadPoolRequestParameters, CatThreadPoolRecord>(request, ct);
    }

    public CatResponse<CatTransformsRecord> Transforms(
      Func<CatTransformsDescriptor, ICatTransformsRequest> selector = null)
    {
      return this.Transforms(selector.InvokeOrDefault<CatTransformsDescriptor, ICatTransformsRequest>(new CatTransformsDescriptor()));
    }

    public Task<CatResponse<CatTransformsRecord>> TransformsAsync(
      Func<CatTransformsDescriptor, ICatTransformsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.TransformsAsync(selector.InvokeOrDefault<CatTransformsDescriptor, ICatTransformsRequest>(new CatTransformsDescriptor()), ct);
    }

    public CatResponse<CatTransformsRecord> Transforms(ICatTransformsRequest request) => this.DoCat<ICatTransformsRequest, CatTransformsRequestParameters, CatTransformsRecord>(request);

    public Task<CatResponse<CatTransformsRecord>> TransformsAsync(
      ICatTransformsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoCatAsync<ICatTransformsRequest, CatTransformsRequestParameters, CatTransformsRecord>(request, ct);
    }
  }
}
