// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformBillingPipelineService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformBillingPipelineService : IBillingPipelineService, IVssFrameworkService
  {
    private const string lastSuccessfulRecordProcessedEventTimeRegistryKey = "/Service/Commerce/AzureBilling/LastSuccessfulRecordEventTime";
    private IUsageEventsStore usageEventStore;
    private DateTime lastSuccessfulRecordProcessedEventTime = DateTime.MinValue;
    private const string Layer = "PlatformBillingPipelineService";
    private const string Area = "Commerce";
    private int maxRetries = 4;
    private const string BILLING_TABLE_NAME = "CommerceBillableEventsTable2";
    private const string PUSHAGENT_TABLE_NAME = "CommerceUsageRecordsTable";

    public void EnqueueUsageBillingBatchMessageForProcessing(
      IVssRequestContext requestContext,
      AzureBillingQueue billingQueue)
    {
      ArgumentUtility.CheckForNull<AzureBillingQueue>(billingQueue, nameof (billingQueue));
      try
      {
        requestContext.TraceEnter(5108808, "Commerce", nameof (PlatformBillingPipelineService), new object[1]
        {
          (object) billingQueue
        }, nameof (EnqueueUsageBillingBatchMessageForProcessing));
        this.PermissionChecker(requestContext, 88);
        this.usageEventStore.EnqueueNotificationForPushAgent(requestContext, billingQueue);
        this.SetLastSuccessfulRecordEventTimeRegistryEntry(requestContext, this.lastSuccessfulRecordProcessedEventTime);
        CustomerIntelligenceData eventData = new CustomerIntelligenceData();
        eventData.Add(CustomerIntelligenceProperty.AccountId, (object) requestContext.ServiceHost.InstanceId);
        eventData.Add(CustomerIntelligenceProperty.ActivityId, (object) requestContext.ActivityId);
        eventData.Add("LastSucessfulRecordProcessedTime", this.lastSuccessfulRecordProcessedEventTime.ToString());
        eventData.Add("BillingQueue", billingQueue.ToString());
        CustomerIntelligence.PublishEvent(requestContext, "EnqueueUsageBillingBatchMessage", eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108804, "Commerce", nameof (PlatformBillingPipelineService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5108809, "Commerce", nameof (PlatformBillingPipelineService), nameof (EnqueueUsageBillingBatchMessageForProcessing));
      }
    }

    public IEnumerable<AzureBillableEvent2> GetBillingEventsForProcessing(
      IVssRequestContext requestContext,
      DateTime lastSuccessfullyProcessedEventTime)
    {
      this.PermissionChecker(requestContext, 1);
      return this.usageEventStore.GetRawBillingEventsForProcessing(requestContext, lastSuccessfullyProcessedEventTime, DateTime.UtcNow.AddMinutes(-5.0));
    }

    public DateTime GetLastSuccessfulRecordEventTimeRegistryEntry(IVssRequestContext requestContext)
    {
      try
      {
        this.PermissionChecker(requestContext, 4);
        return new DateTime(requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, (RegistryQuery) "/Service/Commerce/AzureBilling/LastSuccessfulRecordEventTime", 0L));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108817, "Commerce", nameof (PlatformBillingPipelineService), ex);
        throw;
      }
    }

    public void SetLastSuccessfulRecordEventTimeRegistryEntry(
      IVssRequestContext requestContext,
      DateTime updatedEventTime)
    {
      requestContext.TraceEnter(5108806, "Commerce", nameof (PlatformBillingPipelineService), new object[1]
      {
        (object) updatedEventTime
      }, nameof (SetLastSuccessfulRecordEventTimeRegistryEntry));
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      try
      {
        this.PermissionChecker(requestContext, 64);
        service.SetValue<string>(requestContext, "/Service/Commerce/AzureBilling/LastSuccessfulRecordEventTime", updatedEventTime.Ticks.ToString());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108805, TraceLevel.Info, "Commerce", nameof (PlatformBillingPipelineService), ex);
        this.SetLastSuccessfulRecordEventTimeRegistryEntry(requestContext, updatedEventTime);
      }
      finally
      {
        requestContext.TraceLeave(5108807, "Commerce", nameof (PlatformBillingPipelineService), nameof (SetLastSuccessfulRecordEventTimeRegistryEntry));
      }
    }

    public void UpdateStorageTableWithNewBillingEvents(
      IVssRequestContext requestContext,
      IEnumerable<AzureBillableEvent2> billableEvents,
      out AzureBillingQueue billingQueue)
    {
      this.PermissionChecker(requestContext, 8);
      if (!billableEvents.Any<AzureBillableEvent2>())
      {
        requestContext.Trace(5108802, TraceLevel.Info, "Commerce", nameof (PlatformBillingPipelineService), "No billable events present, so skipping the update.");
        billingQueue = (AzureBillingQueue) null;
      }
      else
      {
        requestContext.TraceEnter(5108798, "Commerce", nameof (PlatformBillingPipelineService), new object[1]
        {
          (object) billableEvents
        }, nameof (UpdateStorageTableWithNewBillingEvents));
        string partitionKey = this.GeneratePartitionKey(DateTime.UtcNow);
        billingQueue = new AzureBillingQueue().UpdatePartitionKeyAndBatchId(partitionKey, Guid.NewGuid());
        foreach (AzureBillableEvent2 billableEvent in billableEvents)
        {
          try
          {
            this.UpdateEventsToBillingTable(requestContext, billableEvent.UpdatePartitionKey(partitionKey));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(5108802, "Commerce", nameof (PlatformBillingPipelineService), ex);
          }
        }
        requestContext.Trace(5108803, TraceLevel.Info, "Commerce", nameof (PlatformBillingPipelineService), string.Format("Partition Key: {0} Batch ID: {1} Last Successful Record Processed Time: {2}", (object) billingQueue.PartitionKey, (object) billingQueue.BatchId.ToString(), (object) this.lastSuccessfulRecordProcessedEventTime));
        billingQueue.LastSuccessfullyProcessedEventTime = new DateTime?(this.lastSuccessfulRecordProcessedEventTime);
        requestContext.TraceLeave(5108799, "Commerce", nameof (PlatformBillingPipelineService), nameof (UpdateStorageTableWithNewBillingEvents));
      }
    }

    public IEnumerable<AzureBillableEvent2> GetBillingEventsFromPushAgentTableByPartitionAndRowKey(
      IVssRequestContext requestContext,
      string partitionKey,
      string rowKey = null)
    {
      this.PermissionChecker(requestContext, 1);
      return string.IsNullOrEmpty(rowKey) ? this.usageEventStore.GetBillingEventsFromPushAgentTable(requestContext, partitionKey) : this.usageEventStore.GetBillingEventsFromPushAgentTable(requestContext, partitionKey, rowKey);
    }

    public IEnumerable<ErrorReportingQueue> PollErrorReportingQueue(
      IVssRequestContext requestContext)
    {
      this.PermissionChecker(requestContext, 2);
      return this.usageEventStore.DequeueErrorReportingTrigger(requestContext);
    }

    public IEnumerable<ErrorReportingTable> GetErrorRecords(
      IVssRequestContext requestContext,
      string partitionKey)
    {
      this.PermissionChecker(requestContext, 1);
      return this.usageEventStore.GetErrorRecords(requestContext, partitionKey);
    }

    public void SetMaxRetryAttempts(int retries) => this.maxRetries = retries;

    public int PeekQueueForUnprocessedNotifications(IVssRequestContext requestContext)
    {
      this.PermissionChecker(requestContext, 2);
      return this.usageEventStore.PeekQueueForUnprocessedNotifications(requestContext);
    }

    public void CleanUsageEventTable(IVssRequestContext requestContext)
    {
      this.PermissionChecker(requestContext, 128);
      this.usageEventStore.CleanTables(requestContext, "CommerceBillableEventsTable2", DateTime.UtcNow.AddYears(-1));
    }

    public void CleanPushAgentBillingTable(IVssRequestContext requestContext)
    {
      this.PermissionChecker(requestContext, 128);
      this.usageEventStore.CleanTables(requestContext, "CommerceUsageRecordsTable", DateTime.UtcNow.AddYears(-1));
    }

    public string PrintErrorRecord(IVssRequestContext requestContext, ErrorReportingTable error)
    {
      string str = string.Empty;
      IEnumerable<AzureBillableEvent2> partitionAndRowKey = this.GetBillingEventsFromPushAgentTableByPartitionAndRowKey(requestContext, error.UsageRecordPartitionKey, error.UsageRecordRowKey);
      if (partitionAndRowKey != null && partitionAndRowKey.Any<AzureBillableEvent2>())
        str = partitionAndRowKey.FirstOrDefault<AzureBillableEvent2>().ToJsonStringValue();
      return str;
    }

    public IEnumerable<ReQueuePartitionStore> GetPartitionIdsFromPushAgentTable(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime)
    {
      return this.usageEventStore.GetPartitionIdsFromPushAgentTableForReQueuing(requestContext, this.GeneratePartitionKey(startTime), this.GeneratePartitionKey(endTime));
    }

    internal string GeneratePartitionKey(DateTime eventTimeStamp) => eventTimeStamp.Ticks.ToString();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.usageEventStore == null)
        return;
      this.usageEventStore.Cleanup(systemRequestContext);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.usageEventStore = Activator.CreateInstance(Type.GetType(systemRequestContext.GetService<IVssRegistryService>().GetValue<string>(systemRequestContext, (RegistryQuery) "/Service/Commerce/Metering/UsageEventStorageProvider", string.Empty), true), (object) systemRequestContext) as IUsageEventsStore;
    }

    private void PermissionChecker(IVssRequestContext requestContext, int permissionToLookFor) => requestContext.GetService<IPermissionCheckerService>().CheckPermission(requestContext, permissionToLookFor, BillingPipelineSecurity.CommerceBillingPipelineSecurityNamespaceId, BillingPipelineSecurity.CommerceBillingPipelineSecurityNamespaceToken);

    private void ProcessErrorRecords(IVssRequestContext requestContext)
    {
      try
      {
        foreach (ErrorReportingQueue pollErrorReporting in this.PollErrorReportingQueue(requestContext))
        {
          foreach (ErrorReportingTable errorRecord in this.GetErrorRecords(requestContext, pollErrorReporting.PartitionKey))
            requestContext.Trace(5108812, TraceLevel.Error, "Commerce", nameof (PlatformBillingPipelineService), "An error reported by PAV2 for the corresponding partition key: " + errorRecord.PartitionKey + " row key: " + errorRecord.RowKey + " with an error code: " + errorRecord.Code + " and message " + errorRecord.Message);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5108813, "Commerce", nameof (PlatformBillingPipelineService), ex);
        throw;
      }
    }

    private void UpdateEventsToBillingTable(
      IVssRequestContext requestContext,
      AzureBillableEvent2 billableEvent,
      int count = 0)
    {
      try
      {
        if (count > this.maxRetries)
          throw new Exception("Retry failure for the record " + billableEvent.ToStringValue());
        this.usageEventStore.SaveBillingEventsToPushAgentTable(requestContext, billableEvent);
        this.lastSuccessfulRecordProcessedEventTime = billableEvent.EventDateTime;
      }
      catch
      {
        if (count <= this.maxRetries)
          this.UpdateEventsToBillingTable(requestContext, billableEvent, count + 1);
        else
          throw;
      }
    }
  }
}
