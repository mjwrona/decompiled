// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureReportingEventStore
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureReportingEventStore
  {
    private readonly IAzureTableProvider azureTableProvider;
    private readonly IVssDateTimeProvider dateTimeProvider;
    private IEnumerable<string> tables;
    private const string Layer = "AzureReportingEventStore";
    private const string Area = "Commerce";
    private const int TableStorageMaxRowsPerQuery = 1000;
    private string secretsDrawer;
    private string storageKey;
    private bool dispose;

    public AzureReportingEventStore(
      IVssRequestContext requestContext,
      IEnumerable<string> tableNames,
      AzureReportingEventStoreContext context)
      : this(requestContext, tableNames, context, (IAzureTableProvider) new AzureTableProvider(), VssDateTimeProvider.DefaultProvider)
    {
    }

    internal AzureReportingEventStore(
      IVssRequestContext requestContext,
      IEnumerable<string> tableNames,
      AzureReportingEventStoreContext context,
      IAzureTableProvider tableProvider,
      IVssDateTimeProvider dateTimeProvider)
    {
      this.azureTableProvider = tableProvider;
      this.dateTimeProvider = dateTimeProvider;
      this.EventProcessingDelay = context.EventProcessingDelay;
      this.tables = tableNames;
      this.secretsDrawer = context.ConnectionStringDrawer;
      this.storageKey = context.StorageKey;
      requestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), context.ConnectionStringDrawer, (IEnumerable<string>) new string[1]
      {
        context.StorageKey
      });
      string connectionString = this.GetStrongBoxConnectionString(requestContext, context.ConnectionStringDrawer, context.StorageKey);
      this.Initialize(requestContext, connectionString);
    }

    public virtual void SaveProcessedEvents(
      IVssRequestContext requestContext,
      string tableName,
      IList<ITableEntity> reportingEvents)
    {
      requestContext.CheckDeploymentRequestContext();
      foreach (TableBatchOperation tableBatchOperation in reportingEvents.ToTableBatchOperations(TableOperationType.InsertOrMerge))
        this.azureTableProvider.ExecuteTableBatchOperationAsync(tableName, tableBatchOperation, commandKey: nameof (SaveProcessedEvents)).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    public virtual void SaveUnprocessedEvent(
      IVssRequestContext requestContext,
      ReportingEvent reportingEvent)
    {
      requestContext.CheckDeploymentRequestContext();
      try
      {
        AzureStorageFallback<ReportingEvent> azureStorageFallback = new AzureStorageFallback<ReportingEvent>();
        TableOperation tableOperation = TableOperation.InsertOrMerge((ITableEntity) reportingEvent.ToAzureReportingEvent());
        this.azureTableProvider.ExecuteTableOperation(requestContext, "CommerceMasterEventsTable", tableOperation, (Func<TableResult>) (() => azureStorageFallback.ReportEventFallback(requestContext, reportingEvent)), commandKey: nameof (SaveUnprocessedEvent));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108940, "Commerce", nameof (AzureReportingEventStore), ex);
      }
    }

    public virtual IEnumerable<T> GetProcessedEvents<T>(
      IVssRequestContext requestContext,
      string tableName,
      string resourceName,
      DateTime startDateInclusive,
      DateTime endDateInclusive,
      string filter)
      where T : ITableEntity, new()
    {
      string filterA = TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "eq", resourceName), "and", TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("RowKey", "ge", startDateInclusive.ToUniversalTime().Ticks.ToString("D19")), "and", TableQuery.GenerateFilterCondition("RowKey", "le", endDateInclusive.ToUniversalTime().Ticks.ToString("D19"))));
      string filter1 = filterA;
      if (!string.IsNullOrEmpty(filter))
        filter1 = TableQuery.CombineFilters(filterA, "and", filter);
      TableQuery<T> tableQuery = new TableQuery<T>().Where(filter1);
      return this.azureTableProvider.ExecuteTableQuery<T>(requestContext, tableName, tableQuery, commandKey: nameof (GetProcessedEvents));
    }

    public virtual AzureTableQuerySegment<AzureReportingEvent> GetUnprocessedEvents(
      IVssRequestContext requestContext,
      TableContinuationToken continuationToken,
      int batchSize,
      AzureReportingWatermark watermark)
    {
      if (batchSize > 1000)
        throw new ArgumentOutOfRangeException(nameof (batchSize));
      TableQuery<AzureReportingEvent> eventsQuery = this.GetEventsQuery(watermark);
      TableQuerySegment<AzureReportingEvent> tableQuerySegment = this.azureTableProvider.ExecuteTableQuerySegmented<AzureReportingEvent>(requestContext, "CommerceMasterEventsTable", eventsQuery.Take(new int?(batchSize)), continuationToken, commandKey: nameof (GetUnprocessedEvents));
      return tableQuerySegment == null ? (AzureTableQuerySegment<AzureReportingEvent>) null : tableQuerySegment.ToAzureTableQuerySegment<AzureReportingEvent>();
    }

    internal void CleanUnprocessedEvents(
      IVssRequestContext requestContext,
      AzureReportingWatermark watermark)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableReportingEventCleanup"))
        return;
      TableQuery<AzureReportingEvent> eventsQuery = this.GetEventsQuery(watermark);
      foreach (AzureReportingEvent entity in this.azureTableProvider.ExecuteTableQuery<AzureReportingEvent>(requestContext, "CommerceMasterEventsTable", eventsQuery, commandKey: nameof (CleanUnprocessedEvents)).Where<AzureReportingEvent>((Func<AzureReportingEvent, bool>) (e => e.PartitionKey != e.EventTime.ToString("yyyy-MM-dd-HH", (IFormatProvider) CultureInfo.InvariantCulture))).ToList<AzureReportingEvent>())
      {
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add("TableEntity", JsonConvert.SerializeObject((object) entity));
        CustomerIntelligence.PublishEvent(requestContext, "InvalidReportingEvent", eventData);
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.RepairReportingEvents"))
        {
          this.azureTableProvider.ExecuteTableOperation(requestContext, "CommerceMasterEventsTable", TableOperation.Delete((ITableEntity) entity), commandKey: nameof (CleanUnprocessedEvents));
          entity.PartitionKey = entity.EventTime.ToString("yyyy-MM-dd-HH", (IFormatProvider) CultureInfo.InvariantCulture);
          this.azureTableProvider.ExecuteTableOperation(requestContext, "CommerceMasterEventsTable", TableOperation.InsertOrMerge((ITableEntity) entity), commandKey: nameof (CleanUnprocessedEvents));
        }
      }
    }

    private TableQuery<AzureReportingEvent> GetEventsQuery(AzureReportingWatermark watermark)
    {
      TableQuery<AzureReportingEvent> tableQuery = new TableQuery<AzureReportingEvent>();
      return watermark == null ? tableQuery.Where(TableQuery.GenerateFilterCondition("RowKey", "lt", this.GetUpperBoundRowKey())) : tableQuery.Where(TableQuery.CombineFilters(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("PartitionKey", "ge", watermark.PartitionKey), "and", TableQuery.GenerateFilterCondition("RowKey", "gt", watermark.RowKey)), "and", TableQuery.GenerateFilterCondition("RowKey", "lt", this.GetUpperBoundRowKey())));
    }

    private string GetUpperBoundRowKey() => string.Format("{0}_{1}", (object) this.GetUtcNow().AddMinutes((double) -this.EventProcessingDelay).Ticks.ToString("D19"), (object) Guid.Empty);

    private void Initialize(
      IVssRequestContext requestContext,
      string storageAccountConnectionString)
    {
      requestContext.CheckDeploymentRequestContext();
      if (string.IsNullOrEmpty(storageAccountConnectionString))
        storageAccountConnectionString = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/Commerce/Metering/StorageAccountConnectionString", string.Empty);
      try
      {
        this.azureTableProvider.Initialize(requestContext, storageAccountConnectionString, this.tables, (Func<bool>) (() => this.InitializedFallback(requestContext)));
      }
      catch (StorageException ex)
      {
        requestContext.TraceException(5108940, "Commerce", nameof (AzureReportingEventStore), (Exception) ex);
        throw;
      }
    }

    private bool InitializedFallback(IVssRequestContext requestContext)
    {
      requestContext.Trace(5108418, TraceLevel.Error, "Commerce", nameof (AzureReportingEventStore), "Azure table storage initialization failed");
      return true;
    }

    internal virtual string GetStrongBoxConnectionString(
      IVssRequestContext requestContext,
      string drawer,
      string storageKey)
    {
      try
      {
        TeamFoundationStrongBoxService service = requestContext.GetService<TeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(requestContext, drawer, true);
        return service.GetString(requestContext, drawerId, storageKey);
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        TeamFoundationEventLog.Default.Log(requestContext, "StrongBoxFailedEvent", TeamFoundationEventId.StrongBoxFailedEvent, EventLogEntryType.Error, (object) storageKey);
        return string.Empty;
      }
    }

    [ExcludeFromCodeCoverage]
    internal virtual DateTime GetUtcNow() => this.dateTimeProvider.UtcNow;

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      string connectionString = this.GetStrongBoxConnectionString(requestContext, this.secretsDrawer, this.storageKey);
      this.Initialize(requestContext, connectionString);
    }

    public void Cleanup(IVssRequestContext requestContext)
    {
      if (!this.dispose)
        this.UnRegisterNotification(requestContext);
      this.dispose = true;
    }

    private void UnRegisterNotification(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));

    public int EventProcessingDelay { get; set; }
  }
}
