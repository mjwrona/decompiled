// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureStorageTablesQueueMessagesUtilService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Queue;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class AzureStorageTablesQueueMessagesUtilService : 
    IAzureStorageTablesQueueMessagesUtilService,
    IVssFrameworkService
  {
    private int maxNumQueues;
    private TableReplicationStatus? tableReplicationStatus;
    private List<CloudQueue> cloudQueues = new List<CloudQueue>();
    private const int c_waitForStatusChange = 5;
    private const string c_area = "ScaleUnitMove";
    private const string c_layer = "AzureStorageTablesQueueMessagesUtilService";
    private const int c_maxMessageSize = 30000;
    private const string c_insertionTimeMessage = "Insertion time is missing after attempting to add message to the queue. Need to retry";
    private const int c_traceAlways = 40001;
    private const int c_traceFailedQueueMessageWrite = 40005;
    private const int c_traceFailedDeleteRoleInstancesPostFailover = 40006;
    private const int c_traceFailedToSplitMessage = 40007;
    private const int c_traceOnSettingsChanged = 40014;
    private const int c_traceTableReplicationStatus = 40015;
    private const int c_traceAddedCloudQueue = 40016;
    private const int c_tracePublishTableOperationMessage = 40017;
    private const int c_tracePublishTableBatchOperationMessage = 40018;
    private const int c_traceProcess = 40019;
    private const int c_tracePublishBatchQueueMessage = 40020;
    private const int c_traceMessageQueueNo = 40021;
    private const int c_traceMessagePublishCount = 40022;
    private const int c_traceAddingMessage = 40023;
    private const int c_traceQueueMessageContents = 40024;
    private const int c_tracePollUntilChanged = 40025;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.GetScaleUnitMoveTableRegistryEntries(systemRequestContext);
      this.RegisterNotification(systemRequestContext, FrameworkServerConstants.ScaleUnitMoveRoot + "**");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.UnregisterNotification(systemRequestContext);

    protected virtual void RegisterNotification(
      IVssRequestContext systemRequestContext,
      string registryPath)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, registryPath + "/*");
    }

    protected virtual void UnregisterNotification(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    private void OnSettingsChanged(
      IVssRequestContext systemRequestContext,
      RegistryEntryCollection changedEntries)
    {
      if (!systemRequestContext.IsFeatureEnabled(FrameworkServerConstants.ScaleUnitMoveTableStorageEnabledFeatureName))
        return;
      systemRequestContext.Trace(40014, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), nameof (OnSettingsChanged));
      this.GetScaleUnitMoveTableRegistryEntries(systemRequestContext);
      using (SUMoveComponent component = systemRequestContext.CreateComponent<SUMoveComponent>())
      {
        RoleInstanceSynchronization roleInstanceSynchronization = component.GetAllRoleInstanceSynchronization(new Guid?()).Where<RoleInstanceSynchronization>((Func<RoleInstanceSynchronization, bool>) (x => x.RoleInstance == AzureRoleUtil.Environment.CurrentRoleInstanceId)).FirstOrDefault<RoleInstanceSynchronization>();
        if (roleInstanceSynchronization == null)
        {
          component.CreateRoleInstanceSynchronization(new RoleInstanceSynchronization()
          {
            RoleInstance = AzureRoleUtil.Environment.CurrentRoleInstanceId,
            CurrentStatus = this.tableReplicationStatus.ToString()
          });
        }
        else
        {
          roleInstanceSynchronization.CurrentStatus = this.tableReplicationStatus.ToString();
          component.UpdateRoleInstanceSynchronization(roleInstanceSynchronization);
        }
      }
    }

    private void GetScaleUnitMoveTableRegistryEntries(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service1 = systemRequestContext.GetService<IVssRegistryService>();
      this.maxNumQueues = service1.GetValue<int>(systemRequestContext, (RegistryQuery) FrameworkServerConstants.MaxNumOfQueues, 0);
      string str = service1.GetValue(systemRequestContext, (RegistryQuery) FrameworkServerConstants.FailoverStatus, (string) null);
      if (str != null)
      {
        TableReplicationStatus result;
        if (Enum.TryParse<TableReplicationStatus>(str, true, out result))
        {
          this.tableReplicationStatus = new TableReplicationStatus?(result);
          systemRequestContext.Trace(40015, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "Set TableReplicationStatus: {0}");
        }
        else
        {
          this.tableReplicationStatus = new TableReplicationStatus?();
          systemRequestContext.Trace(40015, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "TableReplicationStatus is null");
        }
      }
      else
        this.tableReplicationStatus = new TableReplicationStatus?();
      try
      {
        ITeamFoundationStrongBoxService service2 = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service2.UnlockDrawer(systemRequestContext, "ScaleUnitMoveTableTokens", false);
        if (!(drawerId != Guid.Empty))
          return;
        service2.GetDrawerContents(systemRequestContext, drawerId);
        CloudQueueClient cloudQueueClient = Microsoft.Azure.Storage.CloudStorageAccount.Parse(service2.GetString(systemRequestContext, drawerId, "QueueStorageConnectionString")).CreateCloudQueueClient();
        this.cloudQueues.Clear();
        for (int index = 0; index < this.maxNumQueues; ++index)
        {
          CloudQueue queueReference = cloudQueueClient.GetQueueReference(string.Format("{0}{1}", (object) "sumovequeue", (object) index));
          this.cloudQueues.Add(queueReference);
          systemRequestContext.Trace(40016, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "Added cloud queue: {0}", (object) queueReference.Name);
        }
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        systemRequestContext.TraceAlways(40001, TraceLevel.Error, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "QueueStorageConnectionString has not been set");
      }
    }

    public TableReplicationStatus? PublishTableOperationMessage(
      IVssRequestContext requestContext,
      string storageAccountName,
      string tableName,
      TableOperation tableOperation)
    {
      if (requestContext.IsFeatureEnabled(FrameworkServerConstants.ScaleUnitMoveTableStorageEnabledFeatureName) && this.maxNumQueues > 0)
      {
        requestContext.Trace(40017, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), nameof (PublishTableOperationMessage));
        StorageQueueMessage storageQueueMessage = new StorageQueueMessage()
        {
          StorageAccountName = storageAccountName,
          PrimaryId = tableName,
          SubsetId = tableOperation.Entity.PartitionKey,
          ItemId = tableOperation.Entity.RowKey,
          Timestamp = DateTime.Now
        };
        this.Process(requestContext, new StorageQueueMessageBatch()
        {
          StorageQueueMessages = {
            storageQueueMessage
          }
        });
      }
      return this.tableReplicationStatus;
    }

    public TableReplicationStatus? PublishTableBatchOperationMessage(
      IVssRequestContext requestContext,
      string storageAccountName,
      string tableName,
      TableBatchOperation tableBatchOperation)
    {
      if (requestContext.IsFeatureEnabled(FrameworkServerConstants.ScaleUnitMoveTableStorageEnabledFeatureName) && this.maxNumQueues > 0)
      {
        requestContext.Trace(40018, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), nameof (PublishTableBatchOperationMessage));
        StorageQueueMessageBatch storageQueueMessageBatch = new StorageQueueMessageBatch();
        foreach (TableOperation tableOperation in tableBatchOperation)
        {
          StorageQueueMessage storageQueueMessage = new StorageQueueMessage()
          {
            StorageAccountName = storageAccountName,
            PrimaryId = tableName,
            SubsetId = tableOperation.Entity.PartitionKey,
            ItemId = tableOperation.Entity.RowKey,
            Timestamp = DateTime.UtcNow
          };
          storageQueueMessageBatch.StorageQueueMessages.Add(storageQueueMessage);
        }
        this.Process(requestContext, storageQueueMessageBatch);
      }
      return this.tableReplicationStatus;
    }

    private void Process(
      IVssRequestContext requestContext,
      StorageQueueMessageBatch storageQueueMessageBatch)
    {
      requestContext.Trace(40019, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), nameof (Process));
      if (!this.tableReplicationStatus.HasValue)
        return;
      TableReplicationStatus? replicationStatus = this.tableReplicationStatus;
      if (!replicationStatus.HasValue)
        return;
      switch (replicationStatus.GetValueOrDefault())
      {
        case TableReplicationStatus.Replicating:
          this.PublishBatchQueueMessage(requestContext, storageQueueMessageBatch);
          break;
        case TableReplicationStatus.FailingOver:
          this.PollUntilChanged(requestContext);
          break;
      }
    }

    internal void PublishBatchQueueMessage(
      IVssRequestContext requestContext,
      StorageQueueMessageBatch storageQueueMessageBatch)
    {
      requestContext.Trace(40020, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), nameof (PublishBatchQueueMessage));
      StorageQueueMessage storageQueueMessage = storageQueueMessageBatch.StorageQueueMessages.First<StorageQueueMessage>();
      int queueNo = Math.Abs(string.Format("{0}-{1}-{2}", (object) storageQueueMessage.StorageAccountName, (object) storageQueueMessage.PrimaryId, (object) storageQueueMessage.SubsetId).GetHashCode() % this.maxNumQueues);
      requestContext.Trace(40021, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "Message is being published to queue no {0}", (object) queueNo);
      RetryManager retryManager = new RetryManager(3, TimeSpan.FromSeconds(10.0), (Action<Exception>) (ex => requestContext.Trace(40005, TraceLevel.Error, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), ex.Message)), (Func<Exception, bool>) (ex => ex != null && ex.Message == "Insertion time is missing after attempting to add message to the queue. Need to retry"));
      List<CloudQueueMessage> cloudQueueMessageList1 = new List<CloudQueueMessage>();
      List<StorageQueueMessageBatch> fitQueueMessage = this.SplitStorageQueueMessageBatchToFitQueueMessage(storageQueueMessageBatch, requestContext);
      List<CloudQueueMessage> cloudQueueMessageList2 = new List<CloudQueueMessage>();
      foreach (object obj in fitQueueMessage)
      {
        string content = JsonConvert.SerializeObject(obj);
        cloudQueueMessageList2.Add(new CloudQueueMessage(content));
      }
      requestContext.Trace(40022, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "Message count to publish: {0}", (object) cloudQueueMessageList2.Count);
      foreach (CloudQueueMessage cloudQueueMessage1 in cloudQueueMessageList2)
      {
        CloudQueueMessage cloudQueueMessage = cloudQueueMessage1;
        try
        {
          retryManager.Invoke((Action) (() =>
          {
            lock (this.cloudQueues[queueNo])
            {
              requestContext.Trace(40023, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "Adding message to queue");
              requestContext.Trace(40024, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), "Queue message contents : {0}", (object) cloudQueueMessage.AsString);
              CloudQueue cloudQueue = this.cloudQueues[queueNo];
              CloudQueueMessage message = cloudQueueMessage;
              TimeSpan? nullable = new TimeSpan?();
              TimeSpan? timeToLive = new TimeSpan?();
              TimeSpan? initialVisibilityDelay = nullable;
              cloudQueue.AddMessage(message, timeToLive, initialVisibilityDelay);
              if (!cloudQueueMessage.InsertionTime.HasValue)
                throw new Exception(string.Format("Insertion time is missing after attempting to add message to the queue. Need to retry"));
            }
          }));
        }
        catch (Exception ex)
        {
          requestContext.TraceAlways(40005, TraceLevel.Error, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), string.Format("Failed to write queue message to queue no :{0}", (object) queueNo));
        }
      }
    }

    private List<StorageQueueMessageBatch> SplitStorageQueueMessageBatchToFitQueueMessage(
      StorageQueueMessageBatch storageQueueMessageBatch,
      IVssRequestContext requestContext)
    {
      List<StorageQueueMessageBatch> fitQueueMessage = new List<StorageQueueMessageBatch>();
      try
      {
        if (JsonConvert.SerializeObject((object) storageQueueMessageBatch).Length > 30000)
        {
          List<StorageQueueMessage> range = storageQueueMessageBatch.StorageQueueMessages.GetRange(0, storageQueueMessageBatch.StorageQueueMessages.Count / 2);
          storageQueueMessageBatch.StorageQueueMessages.RemoveRange(0, storageQueueMessageBatch.StorageQueueMessages.Count / 2);
          StorageQueueMessageBatch storageQueueMessageBatch1 = new StorageQueueMessageBatch()
          {
            StorageQueueMessages = range
          };
          fitQueueMessage.AddRange((IEnumerable<StorageQueueMessageBatch>) this.SplitStorageQueueMessageBatchToFitQueueMessage(storageQueueMessageBatch, requestContext));
          fitQueueMessage.AddRange((IEnumerable<StorageQueueMessageBatch>) this.SplitStorageQueueMessageBatchToFitQueueMessage(storageQueueMessageBatch1, requestContext));
        }
        else
          fitQueueMessage.Add(storageQueueMessageBatch);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(40007, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), ex);
      }
      return fitQueueMessage;
    }

    private void PollUntilChanged(IVssRequestContext requestContext)
    {
      requestContext.Trace(40025, TraceLevel.Verbose, "ScaleUnitMove", nameof (AzureStorageTablesQueueMessagesUtilService), nameof (PollUntilChanged));
      using (SUMoveComponent component = requestContext.CreateComponent<SUMoveComponent>())
        component.UpdateRoleInstanceSynchronization(AzureStorageTablesQueueMessagesUtilService.CreateRoleInstanceSynchronizationFailingOverObject());
      bool flag = false;
      do
      {
        Thread.Sleep(5000);
        TableReplicationStatus? replicationStatus1 = this.tableReplicationStatus;
        TableReplicationStatus replicationStatus2 = TableReplicationStatus.FailingOver;
        if (!(replicationStatus1.GetValueOrDefault() == replicationStatus2 & replicationStatus1.HasValue))
          flag = true;
      }
      while (!flag);
    }

    private static RoleInstanceSynchronization CreateRoleInstanceSynchronizationFailingOverObject() => new RoleInstanceSynchronization()
    {
      RoleInstance = AzureRoleUtil.Environment.CurrentRoleInstanceId,
      HostId = new Guid?(),
      CurrentStatus = "FailingOver"
    };

    public void SetQueueCount(int queueCount) => this.maxNumQueues = queueCount;

    public void SetTableReplicationStatus(TableReplicationStatus tableReplicationStatus) => this.tableReplicationStatus = new TableReplicationStatus?(tableReplicationStatus);
  }
}
