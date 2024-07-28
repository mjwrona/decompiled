// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IBillingPipelineService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (PlatformBillingPipelineService))]
  public interface IBillingPipelineService : IVssFrameworkService
  {
    DateTime GetLastSuccessfulRecordEventTimeRegistryEntry(IVssRequestContext requestContext);

    void SetLastSuccessfulRecordEventTimeRegistryEntry(
      IVssRequestContext requestContext,
      DateTime updatedEventTime);

    IEnumerable<AzureBillableEvent2> GetBillingEventsForProcessing(
      IVssRequestContext requestContext,
      DateTime lastSuccessfullyProcessedEventTime);

    IEnumerable<AzureBillableEvent2> GetBillingEventsFromPushAgentTableByPartitionAndRowKey(
      IVssRequestContext requestContext,
      string partitionKey,
      string rowKey = null);

    void UpdateStorageTableWithNewBillingEvents(
      IVssRequestContext requestContext,
      IEnumerable<AzureBillableEvent2> billableEvents,
      out AzureBillingQueue billingQueue);

    void EnqueueUsageBillingBatchMessageForProcessing(
      IVssRequestContext requestContext,
      AzureBillingQueue billingQueue);

    IEnumerable<ErrorReportingQueue> PollErrorReportingQueue(IVssRequestContext requestContext);

    IEnumerable<ErrorReportingTable> GetErrorRecords(
      IVssRequestContext requestContext,
      string partitionKey);

    void SetMaxRetryAttempts(int retries);

    int PeekQueueForUnprocessedNotifications(IVssRequestContext requestContext);

    void CleanPushAgentBillingTable(IVssRequestContext requestContext);

    void CleanUsageEventTable(IVssRequestContext requestContext);

    string PrintErrorRecord(IVssRequestContext requestContext, ErrorReportingTable error);

    IEnumerable<ReQueuePartitionStore> GetPartitionIdsFromPushAgentTable(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime);
  }
}
