// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureUsageEventStore
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Queue;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ExcludeFromCodeCoverage]
  internal class AzureUsageEventStore : IUsageEventsStore, IDisposable
  {
    private bool isFurtherProcessingRequired;
    private const string lastProcessedTickRegistryPath = "/Service/Commerce/Metering/LastProcessedTick";
    private readonly IAzureTableProvider azureTableProvider;
    private CloudQueue batchNotificationqueue;
    private CloudQueue errorQueue;
    private bool dispose;
    private const string StorageKey = "CommerceEventsStoreConnectionString";
    private const string Layer = "AzureUsageEventStore";
    private const string Area = "Commerce";
    private int maxRetries = 4;
    private const int MaxBatchSize = 100;
    internal string storageConnectionString;
    private readonly IVssRequestContext RequestContext;

    public AzureUsageEventStore(IVssRequestContext requestContext)
      : this(requestContext, (IAzureTableProvider) new AzureTableProvider())
    {
    }

    internal AzureUsageEventStore(
      IVssRequestContext requestContext,
      IAzureTableProvider tableProvider)
    {
      this.RequestContext = requestContext;
      requestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), FrameworkServerConstants.ConfigurationSecretsDrawerName, (IEnumerable<string>) new string[1]
      {
        "CommerceEventsStoreConnectionString"
      });
      this.azureTableProvider = tableProvider;
      this.Initialize(requestContext);
    }

    public void Initialize(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.storageConnectionString = this.GetStrongBoxConnectionString(requestContext.To(TeamFoundationHostType.Deployment).Elevate());
      if (string.IsNullOrEmpty(this.storageConnectionString))
        this.storageConnectionString = service.GetValue<string>(requestContext, (RegistryQuery) "/Service/Commerce/Metering/StorageAccountConnectionString", string.Empty);
      try
      {
        IAzureTableProvider azureTableProvider = this.azureTableProvider;
        IVssRequestContext requestContext1 = requestContext;
        string connectionString = this.storageConnectionString;
        List<string> tableNames = new List<string>();
        tableNames.Add("CommerceMasterEventsTable");
        tableNames.Add("CommercePublisherEventsTable");
        tableNames.Add("CommerceUsageEventsTable");
        tableNames.Add("CommerceBillableEventsTable");
        tableNames.Add("CommerceUsageEventsHourlyAggregationTable");
        tableNames.Add("CommerceUsageEventsDailyAggregationTable");
        tableNames.Add("CommerceBillableEventsTable2");
        tableNames.Add("CommerceUsageRecordsTable");
        tableNames.Add("CommerceUsageProcessingErrors");
        Func<bool> fallback = (Func<bool>) (() => this.InitializedFallback(requestContext));
        azureTableProvider.Initialize(requestContext1, connectionString, (IEnumerable<string>) tableNames, fallback);
        this.InitializeQueues(requestContext);
      }
      catch (Microsoft.Azure.Cosmos.Table.StorageException ex)
      {
        requestContext.TraceException(5106600, "Commerce", nameof (AzureUsageEventStore), (Exception) ex);
        throw;
      }
    }

    internal virtual void InitializeQueues(IVssRequestContext requestContext)
    {
      if (string.IsNullOrEmpty(this.storageConnectionString))
      {
        requestContext.Trace(5108800, TraceLevel.Error, "Commerce", nameof (AzureUsageEventStore), "Azure storage queue initialization failed due to null or empty connection string");
      }
      else
      {
        this.batchNotificationqueue = this.InitializeQueueByName(requestContext, "commerce-usage-trigger");
        this.errorQueue = this.InitializeQueueByName(requestContext, "commerce-usage-error-trigger");
      }
    }

    private CloudQueue InitializeQueueByName(IVssRequestContext requestContext, string queueNameStr)
    {
      try
      {
        CloudQueue queue = Microsoft.Azure.Storage.CloudStorageAccount.Parse(this.storageConnectionString).CreateCloudQueueClient().GetQueueReference(queueNameStr);
        this.ExecuteInCircuitBreaker(requestContext, queue.Name, TimeSpan.FromSeconds(20.0), (Action) (() => queue.CreateIfNotExists()));
        return queue;
      }
      catch
      {
        requestContext.Trace(5108801, TraceLevel.Error, "Commerce", nameof (AzureUsageEventStore), "Azure storage commerce-usage-trigger queue initialization failed");
        throw;
      }
    }

    private void InitializeErrorQueue(IVssRequestContext requestContext)
    {
      try
      {
        this.errorQueue = Microsoft.Azure.Storage.CloudStorageAccount.Parse(this.storageConnectionString).CreateCloudQueueClient().GetQueueReference("commerce-usage-error-trigger");
        this.ExecuteInCircuitBreaker(requestContext, this.errorQueue.Name, TimeSpan.FromSeconds(20.0), (Action) (() => this.errorQueue.CreateIfNotExists()));
      }
      catch
      {
        requestContext.Trace(5108801, TraceLevel.Error, "Commerce", nameof (AzureUsageEventStore), "Azure storage commerce-usage-error-trigger queue initialization failed");
        throw;
      }
    }

    public void SaveBillingEventsToPushAgentTable(
      IVssRequestContext requestContext,
      AzureBillableEvent2 billableEvent)
    {
      TableOperation tableOperation = TableOperation.InsertOrMerge((ITableEntity) billableEvent);
      this.azureTableProvider.ExecuteTableOperation(requestContext, "CommerceUsageRecordsTable", tableOperation, commandKey: nameof (SaveBillingEventsToPushAgentTable));
    }

    public IEnumerable<AzureBillableEvent2> GetRawBillingEventsForProcessing(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime)
    {
      TableQuery<AzureBillableEvent2> tableQuery = new TableQuery<AzureBillableEvent2>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "gt", startTime.Ticks.ToString()), "and", TableQuery.GenerateFilterCondition("PartitionKey", "le", endTime.Ticks.ToString())));
      return (IEnumerable<AzureBillableEvent2>) this.azureTableProvider.ExecuteTableQuery<AzureBillableEvent2>(requestContext, "CommerceBillableEventsTable2", tableQuery, commandKey: nameof (GetRawBillingEventsForProcessing)).OrderBy<AzureBillableEvent2, DateTime>((Func<AzureBillableEvent2, DateTime>) (x => x.EventDateTime));
    }

    internal IEnumerable<AzureBillableEvent2> GetBillingEventsForValidationFromPushAgentTable(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime)
    {
      TableQuery<AzureBillableEvent2> tableQuery = new TableQuery<AzureBillableEvent2>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "gt", startTime.Ticks.ToString()), "and", TableQuery.GenerateFilterCondition("PartitionKey", "le", endTime.Ticks.ToString())));
      return (IEnumerable<AzureBillableEvent2>) this.azureTableProvider.ExecuteTableQuery<AzureBillableEvent2>(requestContext, "CommerceUsageRecordsTable", tableQuery, commandKey: nameof (GetBillingEventsForValidationFromPushAgentTable)).OrderBy<AzureBillableEvent2, DateTime>((Func<AzureBillableEvent2, DateTime>) (x => x.EventDateTime));
    }

    public IEnumerable<AzureBillableEvent2> GetBillingEventsFromPushAgentTable(
      IVssRequestContext requestContext,
      string partitionKey)
    {
      TableQuery<AzureBillableEvent2> tableQuery = new TableQuery<AzureBillableEvent2>().Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", partitionKey));
      return this.azureTableProvider.ExecuteTableQuery<AzureBillableEvent2>(requestContext, "CommerceUsageRecordsTable", tableQuery, commandKey: nameof (GetBillingEventsFromPushAgentTable));
    }

    public IEnumerable<AzureBillableEvent2> GetBillingEventsFromPushAgentTable(
      IVssRequestContext requestContext,
      string partitionKey,
      string rowKey)
    {
      TableQuery<AzureBillableEvent2> tableQuery = new TableQuery<AzureBillableEvent2>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "eq", partitionKey), "and", TableQuery.GenerateFilterCondition("RowKey", "eq", rowKey)));
      return this.azureTableProvider.ExecuteTableQuery<AzureBillableEvent2>(requestContext, "CommerceUsageRecordsTable", tableQuery, commandKey: nameof (GetBillingEventsFromPushAgentTable));
    }

    public IEnumerable<AzureBillableEvent2> GetBillingEventsFromBillableTable(
      IVssRequestContext requestContext,
      string partitionKey)
    {
      TableQuery<AzureBillableEvent2> tableQuery = new TableQuery<AzureBillableEvent2>().Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", partitionKey));
      return this.azureTableProvider.ExecuteTableQuery<AzureBillableEvent2>(requestContext, "CommerceBillableEventsTable2", tableQuery, commandKey: nameof (GetBillingEventsFromBillableTable));
    }

    public IEnumerable<ErrorReportingQueue> DequeueErrorReportingTrigger(
      IVssRequestContext requestContext)
    {
      List<ErrorReportingQueue> errorReportingQueueList = new List<ErrorReportingQueue>();
      try
      {
        for (CloudQueueMessage message = this.errorQueue.GetMessage(); message != null; message = this.errorQueue.GetMessage())
        {
          if (!string.IsNullOrEmpty(message.AsString))
            errorReportingQueueList.Add(new ErrorReportingQueue(message.AsString));
        }
        return (IEnumerable<ErrorReportingQueue>) errorReportingQueueList;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108811, "Commerce", nameof (AzureUsageEventStore), ex);
        return (IEnumerable<ErrorReportingQueue>) errorReportingQueueList;
      }
    }

    public IEnumerable<ErrorReportingTable> GetErrorRecords(
      IVssRequestContext requestContext,
      string partitionKey)
    {
      TableQuery<ErrorReportingTable> tableQuery = !string.IsNullOrEmpty(partitionKey) ? new TableQuery<ErrorReportingTable>().Where(TableQuery.GenerateFilterCondition("PartitionKey", "eq", partitionKey)) : throw new ArgumentNullException("Error records retrieval failed due to null or empty partition key");
      return this.azureTableProvider.ExecuteTableQuery<ErrorReportingTable>(requestContext, "CommerceUsageProcessingErrors", tableQuery, commandKey: nameof (GetErrorRecords));
    }

    public void EnqueueNotificationForPushAgent(
      IVssRequestContext requestContext,
      AzureBillingQueue billingQueue,
      int count = 0,
      int delayInMilliSeconds = 2)
    {
      try
      {
        requestContext.TraceEnter(5108785, "Commerce", nameof (AzureUsageEventStore), nameof (EnqueueNotificationForPushAgent));
        string str = billingQueue.Serialize<AzureBillingQueue>();
        if (count > this.maxRetries)
          throw new Exception(string.Format("Enqueuing the job for the batch ID failed. Queue Data: {0}", (object) billingQueue));
        CloudQueueMessage message = new CloudQueueMessage(str.ToString());
        this.ExecuteInCircuitBreaker(requestContext, this.batchNotificationqueue.Name, TimeSpan.FromSeconds(20.0), (Action) (() => this.batchNotificationqueue.AddMessage(message)));
      }
      catch (Exception ex)
      {
        if (!(ex is CircuitBreakerException) && !(ex is CircuitBreakerShortCircuitException))
        {
          if (count > this.maxRetries)
            return;
          Thread.Sleep(delayInMilliSeconds);
          this.EnqueueNotificationForPushAgent(requestContext, billingQueue, count + 1, 2 * delayInMilliSeconds);
        }
        else
        {
          requestContext.TraceException(5108810, "Commerce", nameof (AzureUsageEventStore), ex);
          throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(5108786, "Commerce", nameof (AzureUsageEventStore), nameof (EnqueueNotificationForPushAgent));
      }
    }

    public int PeekQueueForUnprocessedNotifications(IVssRequestContext requestContext)
    {
      try
      {
        int ret = 0;
        Action run = (Action) (() =>
        {
          this.batchNotificationqueue.FetchAttributes();
          ret = this.batchNotificationqueue.ApproximateMessageCount.GetValueOrDefault();
        });
        this.ExecuteInCircuitBreaker(requestContext, this.batchNotificationqueue.Name, TimeSpan.FromSeconds(20.0), run);
        return ret;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108787, "Commerce", nameof (AzureUsageEventStore), ex);
        throw;
      }
    }

    internal IEnumerable<CloudQueueMessage> GetUnprocessedNotificationsFromQueue(
      IVssRequestContext requestContext,
      int n = 1)
    {
      try
      {
        IEnumerable<CloudQueueMessage> ret = (IEnumerable<CloudQueueMessage>) null;
        Action run = (Action) (() => ret = this.batchNotificationqueue.GetMessages(n));
        this.ExecuteInCircuitBreaker(requestContext, this.batchNotificationqueue.Name, TimeSpan.FromSeconds(20.0), run);
        return ret;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108866, "Commerce", nameof (AzureUsageEventStore), ex);
        throw;
      }
    }

    public void CleanTables(
      IVssRequestContext requestContext,
      string tableName,
      DateTime latestTimeToCleanup)
    {
      try
      {
        requestContext.TraceEnter(5108789, "Commerce", nameof (AzureUsageEventStore), nameof (CleanTables));
        CloudTable cloudTable = !string.IsNullOrEmpty(tableName) ? Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(this.storageConnectionString).CreateCloudTableClient().GetTableReference(tableName) : throw new ArgumentNullException("Cleaning Table failed due to null or empty table name");
        TableBatchOperation tableBatchOperation = new TableBatchOperation();
        TableQuery<AzureBillableEvent2> tableQuery = new TableQuery<AzureBillableEvent2>().Where(TableQuery.GenerateFilterCondition("PartitionKey", "le", latestTimeToCleanup.Ticks.ToString()));
        foreach (AzureBillableEvent2 entity in this.azureTableProvider.ExecuteTableQuery<AzureBillableEvent2>(requestContext, tableName, tableQuery, commandKey: nameof (CleanTables)))
          cloudTable.Execute(TableOperation.Delete((ITableEntity) entity));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108792, "Commerce", nameof (AzureUsageEventStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5108790, "Commerce", nameof (AzureUsageEventStore), nameof (CleanTables));
      }
    }

    public void DequeueUsageTrigger(IVssRequestContext requestContext)
    {
      try
      {
        this.batchNotificationqueue.Clear();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108793, "Commerce", nameof (AzureUsageEventStore), ex);
        throw;
      }
    }

    public void SaveErrorRecord(IVssRequestContext requestContext, ErrorReportingTable errorEvent)
    {
      TableOperation tableOperation = TableOperation.InsertOrMerge((ITableEntity) errorEvent);
      this.azureTableProvider.ExecuteTableOperation(requestContext, "CommerceUsageProcessingErrors", tableOperation, commandKey: nameof (SaveErrorRecord));
    }

    public void EnqueueErrorNotification(
      IVssRequestContext requestContext,
      ErrorReportingQueue errorQueue)
    {
      try
      {
        string mitigationName = errorQueue.Serialize<ErrorReportingQueue>();
        Action run = (Action) (() => this.batchNotificationqueue.AddMessage(new CloudQueueMessage(mitigationName.ToString())));
        this.ExecuteInCircuitBreaker(requestContext, this.batchNotificationqueue.Name, TimeSpan.FromSeconds(20.0), run);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108797, "Commerce", nameof (AzureUsageEventStore), ex);
        throw;
      }
    }

    public IEnumerable<ReQueuePartitionStore> GetPartitionIdsFromPushAgentTableForReQueuing(
      IVssRequestContext requestContext,
      string startPartitionKey,
      string endPartitionKey)
    {
      TableQuery<ReQueuePartitionStore> tableQuery = new TableQuery<ReQueuePartitionStore>().Select((IList<string>) new string[2]
      {
        "PartitionKey",
        "RowKey"
      }).Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "ge", startPartitionKey), "and", TableQuery.GenerateFilterCondition("PartitionKey", "le", endPartitionKey)));
      return this.azureTableProvider.ExecuteTableQuery<ReQueuePartitionStore>(requestContext, "CommerceUsageRecordsTable", tableQuery, commandKey: nameof (GetPartitionIdsFromPushAgentTableForReQueuing));
    }

    [ExcludeFromCodeCoverage]
    internal virtual string GetStrongBoxConnectionString(IVssRequestContext requestContext)
    {
      try
      {
        TeamFoundationStrongBoxService service = requestContext.GetService<TeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, true);
        return service.GetString(requestContext, drawerId, "CommerceEventsStoreConnectionString");
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        requestContext.Trace(5109021, TraceLevel.Error, "Commerce", nameof (AzureUsageEventStore), "Unable to retrieve the storage from the strongbox using the CommerceEventsStoreConnectionString key so returning empty string.");
        requestContext.TraceException(5109021, "Commerce", nameof (AzureUsageEventStore), (Exception) ex);
        TeamFoundationEventLog.Default.Log(requestContext, "StrongBoxFailedEvent", TeamFoundationEventId.StrongBoxFailedEvent, EventLogEntryType.Error, (object) "CommerceEventsStoreConnectionString");
        throw;
      }
    }

    public string SaveUsageEvent(IVssRequestContext requestContext, UsageEvent usageEvent)
    {
      AzureStorageFallback<UsageEvent> azureStorageFallback = new AzureStorageFallback<UsageEvent>();
      AzureUsageEvent azureUsageEvent = usageEvent.ToAzureUsageEvent(false);
      TableOperation tableOperation = TableOperation.InsertOrMerge((ITableEntity) azureUsageEvent);
      this.azureTableProvider.ExecuteTableOperation(requestContext, "CommerceUsageEventsTable", tableOperation, (Func<TableResult>) (() => azureStorageFallback.ReportEventFallback(requestContext, usageEvent)), commandKey: nameof (SaveUsageEvent));
      return azureUsageEvent.RowKey;
    }

    public void SaveBillableEvent<T>(IVssRequestContext requestContext, T billableEvent) where T : class, IAzureBillableEvent
    {
      if (requestContext.IsSpsService() && CommerceDeploymentHelper.IsBillingDisabled(requestContext, requestContext.UserAgent))
      {
        requestContext.TraceAlways(5106602, TraceLevel.Info, "Commerce", nameof (AzureUsageEventStore), "Billing events are not posted to Azure Storage. Payload: " + billableEvent.ToString());
      }
      else
      {
        IList<string> failedProperties;
        if (!billableEvent.Validate(out failedProperties))
        {
          string message = string.Join("|", (IEnumerable<string>) failedProperties);
          requestContext.Trace(5106602, TraceLevel.Error, "Commerce", nameof (AzureUsageEventStore), message);
          throw new ArgumentException("Incorrect data in " + message, nameof (billableEvent));
        }
        AzureStorageFallback<T> azureStorageFallback = new AzureStorageFallback<T>();
        TableOperation tableOperation = TableOperation.InsertOrMerge((ITableEntity) billableEvent);
        this.azureTableProvider.ExecuteTableOperation(requestContext, billableEvent.GetTableName(), tableOperation, (Func<TableResult>) (() => azureStorageFallback.ReportFallback(requestContext, billableEvent)), commandKey: nameof (SaveBillableEvent));
      }
    }

    public IEnumerable<UsageEvent> GetUnprocessedEvents(IVssRequestContext requestContext)
    {
      DateTime utcNow = DateTime.UtcNow;
      long num = this.GetWatermark<long>(requestContext);
      if (num > utcNow.Ticks)
      {
        num = utcNow.Ticks;
        this.SaveWatermark<long>(requestContext, num);
      }
      TableQuery<AzureUsageEvent> tableQuery = new TableQuery<AzureUsageEvent>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "ge", utcNow.AddHours(-1.0).ToString("yyyy-MM-dd-HH")), "and", TableQuery.CombineFilters(TableQuery.GenerateFilterConditionForLong("EventTicks", "gt", num), "and", TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("ResourceBillingMode", "eq", "PayAsYouGo"), "and", TableQuery.GenerateFilterConditionForGuid("AccountId", "eq", requestContext.ServiceHost.InstanceId))))).Take(new int?(1000));
      List<UsageEvent> list = this.azureTableProvider.ExecuteTableQuery<AzureUsageEvent>(requestContext, "CommerceUsageEventsTable", tableQuery, commandKey: nameof (GetUnprocessedEvents)).Select<AzureUsageEvent, UsageEvent>((Func<AzureUsageEvent, UsageEvent>) (x => x.ToUsageEvent())).ToList<UsageEvent>();
      if (list.Count != 1000)
        return (IEnumerable<UsageEvent>) list;
      this.isFurtherProcessingRequired = true;
      return (IEnumerable<UsageEvent>) list;
    }

    public IEnumerable<UsageEvent> GetProcessedEvents(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      out TableContinuationToken continuationTokenOut,
      TableContinuationToken continuationTokenIn = null,
      Guid serviceHostId = default (Guid))
    {
      DateTime startTimeUniversal = startTime.ToUniversalTime();
      DateTime endTimeUniversal = endTime.ToUniversalTime();
      TableQuery<AzureUsageEvent> tableQuery = new TableQuery<AzureUsageEvent>().Where(TableQuery.CombineFilters(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "ge", startTimeUniversal.ToString("yyyy-MM-dd-HH")), "and", TableQuery.GenerateFilterCondition("PartitionKey", "lt", endTimeUniversal.ToString("yyyy-MM-dd-HH"))), "and", TableQuery.GenerateFilterConditionForGuid("AccountId", "eq", serviceHostId == new Guid() ? requestContext.ServiceHost.InstanceId : serviceHostId))).Take(new int?(1000));
      TableQuerySegment<AzureUsageEvent> source = this.azureTableProvider.ExecuteTableQuerySegmented<AzureUsageEvent>(requestContext, "CommerceUsageEventsTable", tableQuery, continuationTokenIn, commandKey: nameof (GetProcessedEvents));
      List<UsageEvent> list = source.Select<AzureUsageEvent, UsageEvent>((Func<AzureUsageEvent, UsageEvent>) (x => x.ToUsageEvent())).Where<UsageEvent>((Func<UsageEvent, bool>) (x => x.EventTimestamp >= startTimeUniversal && x.EventTimestamp <= endTimeUniversal && x.ResourceBillingMode == ResourceBillingMode.PayAsYouGo)).ToList<UsageEvent>();
      continuationTokenOut = source.ContinuationToken;
      return (IEnumerable<UsageEvent>) list;
    }

    public void SaveWatermark<T>(IVssRequestContext requestContext, T watermark) => requestContext.GetService<IVssRegistryService>().SetValue<T>(requestContext, "/Service/Commerce/Metering/LastProcessedTick", watermark);

    public T GetWatermark<T>(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<T>(requestContext, (RegistryQuery) "/Service/Commerce/Metering/LastProcessedTick", default (T));

    public AggregationTotal GetAggregationTotal(
      IVssRequestContext requestContext,
      string resourceName,
      DateTime date,
      AggregationInterval interval,
      Guid serviceHostId = default (Guid))
    {
      Guid guid = serviceHostId == new Guid() ? requestContext.ServiceHost.InstanceId : serviceHostId;
      TableQuery<AggregationTotal> tableQuery = new TableQuery<AggregationTotal>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "eq", AggregationTotal.FormatPartitionKey(guid, resourceName)), "and", TableQuery.GenerateFilterCondition("RowKey", "eq", AggregationTotal.FormatRowKey(date, interval)))).Take(new int?(1));
      return this.azureTableProvider.ExecuteTableQuery<AggregationTotal>(requestContext, AzureUsageEventStore.GetTable(interval), tableQuery, commandKey: nameof (GetAggregationTotal)).FirstOrDefault<AggregationTotal>() ?? new AggregationTotal(date, resourceName, guid, interval);
    }

    public IEnumerable<AggregationTotal> GetAggregationTotals(
      IVssRequestContext requestContext,
      string resourceName,
      DateTime startDateInclusive,
      DateTime endDateInclusive,
      AggregationInterval interval)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      if (startDateInclusive.Kind == DateTimeKind.Local)
        startDateInclusive = startDateInclusive.ToUniversalTime();
      if (endDateInclusive.Kind == DateTimeKind.Local)
        endDateInclusive = endDateInclusive.ToUniversalTime();
      TableQuery<AggregationTotal> tableQuery = new TableQuery<AggregationTotal>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "eq", AggregationTotal.FormatPartitionKey(instanceId, resourceName)), "and", TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", "ge", AggregationTotal.FormatRowKey(startDateInclusive, interval)), "and", TableQuery.GenerateFilterCondition("RowKey", "le", AggregationTotal.FormatRowKey(endDateInclusive, interval)))));
      return this.azureTableProvider.ExecuteTableQuery<AggregationTotal>(requestContext, AzureUsageEventStore.GetTable(interval), tableQuery, commandKey: nameof (GetAggregationTotals));
    }

    public void SetAggregationTotal(
      IVssRequestContext requestContext,
      string resourceName,
      DateTime date,
      AggregationInterval interval,
      int value,
      Guid serviceHostId = default (Guid))
    {
      Guid accountId = serviceHostId == new Guid() ? requestContext.ServiceHost.InstanceId : serviceHostId;
      TableOperation tableOperation = TableOperation.InsertOrReplace((ITableEntity) new AggregationTotal(date, resourceName, accountId, interval)
      {
        Value = value
      });
      this.azureTableProvider.ExecuteTableOperation(requestContext, AzureUsageEventStore.GetTable(interval), tableOperation, commandKey: nameof (SetAggregationTotal));
    }

    private static string GetTable(AggregationInterval interval)
    {
      string table = (string) null;
      switch (interval)
      {
        case AggregationInterval.Hourly:
          table = "CommerceUsageEventsHourlyAggregationTable";
          break;
        case AggregationInterval.Daily:
          table = "CommerceUsageEventsDailyAggregationTable";
          break;
      }
      return table;
    }

    public bool InitializedFallback(IVssRequestContext requestContext)
    {
      requestContext.Trace(5108418, TraceLevel.Error, "Commerce", nameof (AzureUsageEventStore), "Azure table storage initialization failed");
      return true;
    }

    public bool IsFurtherProcessingRequired => this.isFurtherProcessingRequired;

    public void SetMaxRetryAttempts(int retries) => this.maxRetries = retries;

    public void Cleanup(IVssRequestContext requestContext)
    {
      if (!this.dispose)
        this.UnRegisterNotification(requestContext);
      this.dispose = true;
    }

    private void UnRegisterNotification(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      this.Initialize(requestContext);
    }

    private void ExecuteInCircuitBreaker(
      IVssRequestContext requestContext,
      string commandKey,
      TimeSpan executionIsolationThreadTimeout,
      Action run,
      Action fallback = null)
    {
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKey).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(executionIsolationThreadTimeout));
      new CommandService(requestContext, setter, run, fallback).Execute();
    }

    public void Dispose()
    {
      if (this.RequestContext == null)
        return;
      this.Cleanup(this.RequestContext);
    }
  }
}
