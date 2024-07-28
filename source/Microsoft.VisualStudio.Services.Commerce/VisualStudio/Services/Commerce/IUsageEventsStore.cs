// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IUsageEventsStore
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal interface IUsageEventsStore
  {
    void Initialize(IVssRequestContext requestContext);

    string SaveUsageEvent(IVssRequestContext requestContext, UsageEvent usageEvent);

    IEnumerable<UsageEvent> GetUnprocessedEvents(IVssRequestContext requestContext);

    IEnumerable<AzureBillableEvent2> GetRawBillingEventsForProcessing(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime);

    IEnumerable<AzureBillableEvent2> GetBillingEventsFromPushAgentTable(
      IVssRequestContext requestContext,
      string partitionKey);

    IEnumerable<AzureBillableEvent2> GetBillingEventsFromPushAgentTable(
      IVssRequestContext requestContext,
      string partitionKey,
      string rowKey);

    IEnumerable<AzureBillableEvent2> GetBillingEventsFromBillableTable(
      IVssRequestContext requestContext,
      string partitionKey);

    IEnumerable<UsageEvent> GetProcessedEvents(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      out TableContinuationToken continuationTokenOut,
      TableContinuationToken continuationTokenIn = null,
      Guid serviceHostId = default (Guid));

    void SaveBillableEvent<T>(IVssRequestContext requestContext, T billableEvent) where T : class, IAzureBillableEvent;

    void SaveBillingEventsToPushAgentTable(
      IVssRequestContext requestContext,
      AzureBillableEvent2 billableEvent);

    IEnumerable<ErrorReportingQueue> DequeueErrorReportingTrigger(IVssRequestContext requestContext);

    IEnumerable<ErrorReportingTable> GetErrorRecords(
      IVssRequestContext requestContext,
      string partitionKey);

    void EnqueueNotificationForPushAgent(
      IVssRequestContext requestContext,
      AzureBillingQueue billingQueue,
      int count = 0,
      int delayInMilliSeconds = 2);

    void SaveWatermark<T>(IVssRequestContext requestContext, T watermark);

    AggregationTotal GetAggregationTotal(
      IVssRequestContext requestContext,
      string resourceName,
      DateTime date,
      AggregationInterval interval,
      Guid serviceHostId = default (Guid));

    IEnumerable<AggregationTotal> GetAggregationTotals(
      IVssRequestContext requestContext,
      string resourceName,
      DateTime startDateInclusive,
      DateTime endDateExclusive,
      AggregationInterval interval);

    void SetAggregationTotal(
      IVssRequestContext requestContext,
      string resourceName,
      DateTime date,
      AggregationInterval interval,
      int value,
      Guid serviceHostId = default (Guid));

    T GetWatermark<T>(IVssRequestContext requestContext);

    void SetMaxRetryAttempts(int retries);

    int PeekQueueForUnprocessedNotifications(IVssRequestContext requestContext);

    void CleanTables(
      IVssRequestContext requestContext,
      string tableName,
      DateTime latestTimeToCleanup);

    void DequeueUsageTrigger(IVssRequestContext requestContext);

    void SaveErrorRecord(IVssRequestContext requestContext, ErrorReportingTable errorEvent);

    void EnqueueErrorNotification(IVssRequestContext requestContext, ErrorReportingQueue errorQueue);

    void Cleanup(IVssRequestContext requestContext);

    IEnumerable<ReQueuePartitionStore> GetPartitionIdsFromPushAgentTableForReQueuing(
      IVssRequestContext requestContext,
      string startPartitionKey,
      string endPartitionKey);
  }
}
