// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.StorageMetricsTransactionPopulatorJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Azure;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class StorageMetricsTransactionPopulatorJob : VssAsyncJobExtension
  {
    public static readonly Guid StorageMetricsTransactionPopulatorJobId = new Guid("38D29E68-9E82-4919-9998-AC5F679AC762");
    public const string AzureStorageResourceType = "Microsoft.Storage/storageAccounts";
    private const int DefaultLookbackInHours = 6;

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      if (!requestContext.IsFeatureEnabled("BlobStore.Features.EnableStorageMetricsTransactionPopulatorJob"))
        return new VssJobResult(TeamFoundationJobExecutionResult.Blocked, "Job is disabled due to FF setting. FF: BlobStore.Features.EnableStorageMetricsTransactionPopulatorJob is disabled.");
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      DateTimeOffset dateTimeOffset = registryService.GetValue<DateTimeOffset>(requestContext, (RegistryQuery) "/Configuration/BlobStore/StorageMetricsTransactionPopulatorJob/LastRunTime", DateTimeOffset.UtcNow.AddHours(-6.0));
      DateTimeOffset timeRangeEnd = DateTimeOffset.UtcNow;
      DateTimeOffset timeRangeStart = dateTimeOffset;
      if (timeRangeEnd - timeRangeStart > TimeSpan.FromDays(3.0))
        timeRangeStart = DateTimeOffset.UtcNow.AddHours(-6.0);
      requestContext.CheckDeploymentRequestContext();
      string configurationSetting = AzureRoleUtil.GetOverridableConfigurationSetting(ServicingTokenConstants.AzureSubscriptionId);
      if (string.IsNullOrWhiteSpace(configurationSetting))
        return !requestContext.ExecutionEnvironment.IsDevFabricDeployment ? new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Azure Subscription Id is not set or was not found.") : new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Azure Subscription Id is not set. If on Devfabric - need to manually set the value to the ArtifactsCore_Eng subscriptionId on system. See comments in job.");
      StorageMetricsTransactionHelper storageMetricsTransactionHelper = new StorageMetricsTransactionHelper();
      (StorageMetricsTransactionHelper.AuthenticationType? nullable, HashSet<GenericResource> hashSet) = await storageMetricsTransactionHelper.GetStorageAccounts(requestContext, configurationSetting);
      if (!nullable.HasValue)
      {
        requestContext.TraceAlways(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "All authentication types failed.");
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Failed to authenticate with any of the available methods.");
      }
      MetricsQueryClient metricsClient = storageMetricsTransactionHelper.CreateMetricsClient(requestContext, nullable.Value);
      int storageAccountsSuccessfullyProcessed = 0;
      int metricsProcessedCount = 0;
      requestContext.TraceAlways(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, string.Format("Info: Found {0} resources of type {1}", (object) hashSet.Count<GenericResource>(), (object) "Microsoft.Storage/storageAccounts"));
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        hashSet = hashSet.Take<GenericResource>(10).ToHashSet<GenericResource>();
      Dictionary<string, Uri> storageAccountEndpointMappings = await storageMetricsTransactionHelper.GetStorageAccountEndpointMappings(requestContext);
      foreach (GenericResource storageResource in hashSet)
      {
        try
        {
          Dictionary<string, MetricTimeSeriesElement> timeSeriesElements = new Dictionary<string, MetricTimeSeriesElement>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          QueryTimeRange queryTimeRange;
          // ISSUE: explicit constructor call
          ((QueryTimeRange) ref queryTimeRange).\u002Ector(timeRangeStart, timeRangeEnd);
          timeSeriesElements = await storageMetricsTransactionHelper.GetApiMetricsByResponseType(metricsClient, storageResource, queryTimeRange);
          MetricsQueryOptions overallQueryOptions = new MetricsQueryOptions()
          {
            TimeRange = new QueryTimeRange?(new QueryTimeRange(timeRangeStart, timeRangeEnd)),
            Filter = "ApiName eq '*'"
          };
          Response<MetricsQueryResult> IngressEgressMetrics = await metricsClient.QueryResourceAsync(((ArmResource) storageResource).Id.ToString(), (IEnumerable<string>) new \u003C\u003Ez__ReadOnlyArray<string>(new string[2]
          {
            "ingress",
            "egress"
          }), overallQueryOptions, new CancellationToken());
          foreach (MetricResult metric in (IEnumerable<MetricResult>) ((NullableResponse<MetricsQueryResult>) IngressEgressMetrics).Value.Metrics)
          {
            foreach (MetricTimeSeriesElement timeSeriesElement in ((NullableResponse<MetricsQueryResult>) IngressEgressMetrics).Value.Metrics.SelectMany<MetricResult, MetricTimeSeriesElement>((Func<MetricResult, IEnumerable<MetricTimeSeriesElement>>) (m => (IEnumerable<MetricTimeSeriesElement>) m.TimeSeries)))
            {
              string str;
              if (timeSeriesElement.Metadata.TryGetValue("apiname", out str))
                timeSeriesElements[str + "/" + metric.Name] = timeSeriesElement;
            }
          }
          Response<MetricsQueryResult> response = await metricsClient.QueryResourceAsync(((ArmResource) storageResource).Id.ToString(), (IEnumerable<string>) new \u003C\u003Ez__ReadOnlyArray<string>(new string[3]
          {
            "availability",
            "SuccessE2ELatency",
            "SuccessServerLatency"
          }), overallQueryOptions, new CancellationToken());
          foreach (MetricResult metric in (IEnumerable<MetricResult>) ((NullableResponse<MetricsQueryResult>) IngressEgressMetrics).Value.Metrics)
          {
            foreach (MetricTimeSeriesElement timeSeriesElement in ((NullableResponse<MetricsQueryResult>) IngressEgressMetrics).Value.Metrics.SelectMany<MetricResult, MetricTimeSeriesElement>((Func<MetricResult, IEnumerable<MetricTimeSeriesElement>>) (m => (IEnumerable<MetricTimeSeriesElement>) m.TimeSeries)))
            {
              string str;
              if (timeSeriesElement.Metadata.TryGetValue("apiname", out str))
                timeSeriesElements[str + "/" + metric.Name] = timeSeriesElement;
            }
          }
          List<AzureMonitorStorageMetrics> aggregateByApiName = storageMetricsTransactionHelper.CreateMetricsRowsAggregateByApiName(timeSeriesElements);
          metricsProcessedCount += aggregateByApiName.Count;
          if (requestContext.IsFeatureEnabled("BlobStore.Features.EnableSMTPopulatorTelemetryPublishing"))
          {
            Guid executionId = Guid.NewGuid();
            TeamFoundationTracingService service = requestContext.GetService<TeamFoundationTracingService>();
            string clusterIfPossible = StorageMetricsTransactionPopulatorJob.GetStorageClusterIfPossible(requestContext, storageMetricsTransactionHelper, storageAccountEndpointMappings, storageResource);
            requestContext.TraceAlways(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, string.Format("Info: Publishing {0} metrics rows for {1} spanning {2} to {3}", (object) aggregateByApiName.Count, (object) ((ArmResource) storageResource).Id.Name, (object) timeRangeStart, (object) timeRangeEnd));
            foreach (AzureMonitorStorageMetrics monitorStorageMetrics in aggregateByApiName)
              monitorStorageMetrics.LogToTracer(service, executionId, ((ArmResource) storageResource).Id.Name, clusterIfPossible);
            ++storageAccountsSuccessfullyProcessed;
            if (storageAccountsSuccessfullyProcessed == 1)
            {
              requestContext.TraceAlways(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, string.Format("Info: One storage account successfully processed. Setting registry setting to {0}", (object) timeRangeEnd));
              registryService.SetValue<DateTimeOffset>(requestContext, "/Configuration/BlobStore/StorageMetricsTransactionPopulatorJob/LastRunTime", timeRangeEnd);
            }
          }
          timeSeriesElements = (Dictionary<string, MetricTimeSeriesElement>) null;
          overallQueryOptions = (MetricsQueryOptions) null;
          IngressEgressMetrics = (Response<MetricsQueryResult>) null;
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(ContentTracePoints.StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorTrace, "Error: Failed to process metrics for " + ((ArmResource) storageResource).Id.Name + " due to error " + ex.Message);
        }
      }
      StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorJobResult dataContractObject = new StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorJobResult()
      {
        UsedAuthenticationType = nullable.Value,
        TotalStorageAccountsFound = hashSet.Count,
        NumStorageAccountsSuccessfullyProcessed = storageAccountsSuccessfullyProcessed,
        TotalMetricsProcessed = metricsProcessedCount,
        TimeRangeStart = timeRangeStart,
        TimeRangeEnd = timeRangeEnd,
        PublishingEnabled = requestContext.IsFeatureEnabled("BlobStore.Features.EnableSMTPopulatorTelemetryPublishing")
      };
      return storageAccountsSuccessfullyProcessed > 0 ? new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize<StorageMetricsTransactionPopulatorJob.StorageMetricsTransactionPopulatorJobResult>(dataContractObject)) : new VssJobResult(TeamFoundationJobExecutionResult.Failed, "Job failed to process metrics for any storage account.");
    }

    private static string GetStorageClusterIfPossible(
      IVssRequestContext requestContext,
      StorageMetricsTransactionHelper storageMetricsTransactionHelper,
      Dictionary<string, Uri> storageAccountEndpointMappings,
      GenericResource storageResource)
    {
      Uri blobEndpoint;
      return storageAccountEndpointMappings.TryGetValue(((ArmResource) storageResource).Id.Name, out blobEndpoint) ? storageMetricsTransactionHelper.GetStorageCluster(requestContext, blobEndpoint, ((ArmResource) storageResource).Id.Name) : "Unknown";
    }

    public class StorageMetricsTransactionPopulatorJobResult
    {
      public StorageMetricsTransactionHelper.AuthenticationType UsedAuthenticationType { get; set; }

      public int TotalStorageAccountsFound { get; set; }

      public int NumStorageAccountsSuccessfullyProcessed { get; set; }

      public int TotalMetricsProcessed { get; set; }

      public bool PublishingEnabled { get; set; }

      public DateTimeOffset TimeRangeStart { get; set; }

      public DateTimeOffset TimeRangeEnd { get; set; }
    }
  }
}
