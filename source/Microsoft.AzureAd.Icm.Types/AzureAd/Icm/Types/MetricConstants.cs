// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.MetricConstants
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types
{
  public static class MetricConstants
  {
    public const string CacheRefreshMetricGroupName = "Cache Refresh";
    public const string TaskMetricGroupName = "Scheduled Tasks";
    public const string ConnectorGroupName = "Connector";
    public const string SelfMonitoringMetricGroupName = "Self Monitoring";
    public const string WarehouseGroupName = "Warehouse";
    public const string DataAccessGroupName = "DataAccess";
    public const string DatataxiGroupName = "Data Taxi";
    public const string NotifierGroupName = "Notifier";
    public const string TaskNameDimension = "Task Name";
    public const string ODataGroupName = "OData";
    public const string KustoDatapushGroupName = "Kusto Data Push";
    public const string IncidentSimiarityGroupName = "Similar Incidents";
    public const string AirServiceGroupName = "AIR Service";
    public const string SanitizerGroupName = "Sanitizer";
    public const string LiveFeedServiceGroupName = "LiveFeedService";
    public static readonly Tuple<MetricCategory, MetricNamePrefix>[] AllowedOperationMetricConfigs = new Tuple<MetricCategory, MetricNamePrefix>[14]
    {
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.QoS, MetricNamePrefix.Portal),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.QoS, MetricNamePrefix.Odata),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.QoS, MetricNamePrefix.Connector),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.Operation, MetricNamePrefix.Portal),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.Operation, MetricNamePrefix.Odata),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.Operation, MetricNamePrefix.Connector),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.Operation, MetricNamePrefix.OncallSync),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.TfsConnector, MetricNamePrefix.TfsConnectorRoot),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.TfsConnector, MetricNamePrefix.TfsConnectorGetConfig),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.TfsConnector, MetricNamePrefix.TfsConnectorInit),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.TfsConnector, MetricNamePrefix.TfsConnectorGetFromTfs),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.TfsConnector, MetricNamePrefix.TfsConnectorSyncToIcm),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.TfsConnector, MetricNamePrefix.TfsConnectorUpdateConfig),
      Tuple.Create<MetricCategory, MetricNamePrefix>(MetricCategory.TfsConnector, MetricNamePrefix.TfsConnectorLogStatusToIcm)
    };

    public static class CacheRefreshMetrics
    {
      public const string CacheRefreshMetric = "Cache Refresh Metric";
      public const string CacheNameDimension = "Name";
      public const string CacheRefreshResultDimension = "Result";
    }

    public static class TaskMetrics
    {
      public const string FetchNextItemMetricName = "Fetch Item";
      public const string ExecuteItemMetricName = "Process Item";
      public const string UnhandledExceptionMetricName = "Unhandled Exception";
      public const string TaskExecutionMetric = "Task Execution Metric";
      public const string TaskResultDimension = "Result";
    }

    public static class ConnectorMetrics
    {
      public const string AddOrUpdate = "Add or Update";
    }

    public static class ComponentMetrics
    {
      public const string ComponentLastRunAge = "Component Last Run Age";
      public const string ComponentLastSuccessRunAge = "Component Last Successful Run Age";
      public const string ComponentMessageCount = "Component Message Count";
    }

    public static class DataAccessMetrics
    {
      public const string EnumFilteredIncidentSummaries = "EnumFilteredIncidentSummaries";
    }

    public static class ODataMetrics
    {
      public const string GetFavoriteQueries = "GetFavoriteQueries";
      public const string GetIncidents = "GetIncidents";
      public const string CannedQueryIncidentsReturnedCount = "Count of incidents returned for canned query";
      public const string EfQueryIncidentsReturnedCount = "Count of incidents returned for EF query";
      public const string DBTimeToGetIncidentDetailsUsingDapper = "DB-Time to get incident details using dapper";
      public const string DBTimeToGetPostmortemUsingDapper = "DB-Time to get postmortem using dapper";

      public static class Throttle
      {
        public const string MetricName = "Request Throttle Status";
        public const string SourceDimension = "Source";
        public const string MethodDImension = "Method";
        public const string ThrottledDimension = "Throttled";
        public const string EnabledDimension = "Enabled";
        public const string GlobalThrottledDimension = "GlobalThrottled";
      }
    }

    public static class QueueMetrics
    {
      public const string QueueCountMetricName = "Queue Length";
      public const string QueueOldestItemMetricName = "Oldest Unprocessed Item Age";
      public const string QueueInProgressCountMetricName = "Queue In Progress Length";
      public const string QueueOldestInProgressItemMetricName = "Oldest In Progress Item Age";
    }

    public static class SqlNodeMetrics
    {
      public const string SqlNodeIsHealthy = "SQL Node Is Healthy";
      public const string SqlNodeName = "SQL Node Name";
      public const string SqlNodeRole = "SQL Node Role";
      public const string SqlNodeAvailabilityMode = "SQL Node Availability Mode";
      public const string SqlNodeHealth = "SQL Node Health";
      public const string LogSizeIsHealthy = "Log Size Is Healthy";
      public const string Database = "SQL Database Name";
      public const string LogFileSize = "Log File Size";
      public const string PercentLogUsed = "Percent Log Usage";
    }

    public static class DataCleanerMetrics
    {
      public const string DataCleanerIsHealthy = "DataCleaner Is Healthy";
      public const string Status = "Status";
      public const string DeletedIncidentCount = "DeletedIncidentCount";
    }

    public static class WarehouseMetrics
    {
      public const string WarehouseMetricName = "Warehouse Delay (min)";
    }

    public static class DatataxiMetrics
    {
      public const string DatataxiMetricName = "Data Taxi Delay (min)";
    }

    public static class KustoDataPushMetrics
    {
      public const string KustoDataPushDelayInternalInMinutes = "Kusto Data Push Delay (min)";
      public const string KustoDataPushDelayExternalInMinutes = "Kusto Data Push Delay External (min)";
    }

    public static class NotifierMetrics
    {
      public const string NotificationAgeMetricName = "Notification Age";
      public const string NoticiationProcessedDurationMetricName = "Notification Processed Duration";
      public const string SkippedOnCallNotificationsMetricName = "Skipped OnCall Notifications";
      public const string TenantName = "Tenant Name";
      public const string IsIncidentNotification = "Is Incident Notification";
      public const string ServiceType = "Service Type";
      public const string NotificationType = "Notification Type";
      public const string NotificationStatus = "Notification Status";
      public const string SkipReason = "SkipReason";
    }

    public static class IncidentSimilarityMetrics
    {
      public const string AmlServiceCallDuration = "Service Call Duration";
      public const string AmlServiceCallStatus = "Service Call Status";
      public const string FailedAmlServiceCalls = "Failed Service Call";
      public const string ServiceResponseCount = "Number of Output";
      public const string ServiceName = "ServiceName";
      public const string SourceQuery = "SourceQuery";
      public const string StatusCode = "StatusCode";
    }

    public static class LiveFeedMetrics
    {
      public const string EventProcessingStatusMetricName = "Event Processing Status";
      public const string EventPublishStatusMetricName = "Event Publish Status";
      public const string EventType = "EventType";
      public const string IncidentId = "IncidentId";
      public const string Channel = "Channel";
      public const string Status = "Status";
    }

    public static class AirServiceMetrics
    {
      public const string SimilaritySignal = "SimilaritySignal";
    }
  }
}
