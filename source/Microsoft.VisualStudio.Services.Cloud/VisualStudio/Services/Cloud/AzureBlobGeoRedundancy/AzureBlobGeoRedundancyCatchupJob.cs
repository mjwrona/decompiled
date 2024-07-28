// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyCatchupJob
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage.Queue.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyCatchupJob : VssAsyncJobExtension
  {
    private TimeSpan m_expirationTime = AzureBlobGeoRedundancyCatchupJob.s_defaultExpirationTime;
    private TimeSpan m_keepAliveInterval = AzureBlobGeoRedundancyCatchupJob.s_defaultKeepAliveInterval;
    private TimeSpan m_visibilityTimeout = AzureBlobGeoRedundancyCatchupJob.s_defaultVisibilityTimeout;
    private TimeSpan m_maxVisibilityTimeout = AzureBlobGeoRedundancyCatchupJob.s_defaultMaxVisibilityTimeout;
    private bool m_applyOptimizations = true;
    private int m_batchSize = 32;
    private int m_maxBatchesPerQueue = 1;
    private TimeSpan m_maxRuntime = AzureBlobGeoRedundancyCatchupJob.s_defaultMaxRuntime;
    private static readonly TimeSpan s_defaultExpirationTime = TimeSpan.FromHours(8.0);
    private static readonly TimeSpan s_defaultKeepAliveInterval = TimeSpan.FromMinutes(1.0);
    private static readonly TimeSpan s_defaultVisibilityTimeout = TimeSpan.FromMinutes(2.0);
    internal static readonly TimeSpan s_defaultMaxVisibilityTimeout = TimeSpan.FromMinutes(16.0);
    private static readonly TimeSpan s_defaultMaxRuntime = TimeSpan.FromMinutes(10.0);
    private const bool c_defaultApplyOptimizations = true;
    private const int c_defaultBatchSize = 32;
    private const int c_defaultMaxBatchesPerQueue = 1;
    internal static readonly string s_baseRegistryPath = "/Service/AzureBlobGeoRedundancy/Settings/Catchup";
    internal static readonly string s_expirationTimeRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/ExpirationTime";
    internal static readonly string s_keepAliveIntervalRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/KeepAliveInterval";
    internal static readonly string s_visibilityTimeoutRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/VisibilityTimeout";
    internal static readonly string s_maxVisibilityTimeoutRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/MaxVisibilityTimeout";
    internal static readonly string s_applyOptimizationsRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/ApplyOptimizations";
    internal static readonly string s_batchSizeRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/BatchSize";
    internal static readonly string s_maxBatchesPerQueueRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/MaxBatchesPerQueue";
    internal static readonly string s_maxRuntimeRegistryPath = AzureBlobGeoRedundancyCatchupJob.s_baseRegistryPath + "/MaxRuntime";
    private const int c_maxQueueRequestSize = 32;
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancyCatchupJob";

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IAzureBlobGeoRedundancyService service1 = requestContext.GetService<IAzureBlobGeoRedundancyService>();
      if (!service1.IsEnabled(requestContext))
        return new VssJobResult(TeamFoundationJobExecutionResult.Blocked, "Geo-redundancy feature flag is not enabled.");
      IVssRegistryService service2 = requestContext.GetService<IVssRegistryService>();
      this.m_expirationTime = service2.GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_expirationTimeRegistryPath, AzureBlobGeoRedundancyCatchupJob.s_defaultExpirationTime);
      this.m_keepAliveInterval = service2.GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_keepAliveIntervalRegistryPath, AzureBlobGeoRedundancyCatchupJob.s_defaultKeepAliveInterval);
      this.m_visibilityTimeout = service2.GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_visibilityTimeoutRegistryPath, AzureBlobGeoRedundancyCatchupJob.s_defaultVisibilityTimeout);
      this.m_maxVisibilityTimeout = service2.GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_maxVisibilityTimeoutRegistryPath, AzureBlobGeoRedundancyCatchupJob.s_defaultMaxVisibilityTimeout);
      this.m_applyOptimizations = service2.GetValue<bool>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_applyOptimizationsRegistryPath, true);
      this.m_batchSize = service2.GetValue<int>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_batchSizeRegistryPath, 32);
      this.m_maxBatchesPerQueue = service2.GetValue<int>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_maxBatchesPerQueueRegistryPath, 1);
      this.m_maxRuntime = service2.GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancyCatchupJob.s_maxRuntimeRegistryPath, AzureBlobGeoRedundancyCatchupJob.s_defaultMaxRuntime);
      IEnumerable<GeoRedundantStorageAccountSettings> storageAccountSettings = requestContext.GetService<IAzureBlobGeoRedundancyManagementService>().GetGeoRedundantStorageAccounts(requestContext);
      List<AzureBlobGeoRedundancyCatchupJob.StorageAccountClients> list = storageAccountSettings.Select<GeoRedundantStorageAccountSettings, AzureBlobGeoRedundancyCatchupJob.StorageAccountClients>((Func<GeoRedundantStorageAccountSettings, AzureBlobGeoRedundancyCatchupJob.StorageAccountClients>) (s =>
      {
        string connectionString1 = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, s.DrawerName, s.PrimaryLookupKey);
        string connectionString2 = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, s.DrawerName, s.SecondaryLookupKey);
        CloudStorageAccount account1 = CloudStorageAccount.Parse(connectionString1);
        CloudStorageAccount account2 = CloudStorageAccount.Parse(connectionString2);
        AzureBlobGeoRedundancyCatchupJob.StorageAccountClients storageAccountClients = new AzureBlobGeoRedundancyCatchupJob.StorageAccountClients();
        storageAccountClients.PrimaryBlobClient = account1.CreateCloudBlobClient();
        storageAccountClients.SecondaryBlobClient = account2.CreateCloudBlobClient();
        storageAccountClients.PrimaryQueueClient = account1.CreateCloudQueueClient();
        storageAccountClients.SecondaryQueueClient = account2.CreateCloudQueueClient();
        if (!this.m_applyOptimizations)
          return storageAccountClients;
        AzureBlobGeoRedundancyUtils.OptimizeBlobServiceEndpoint(requestContext, account1);
        AzureBlobGeoRedundancyUtils.OptimizeBlobServiceEndpoint(requestContext, account2);
        AzureBlobGeoRedundancyUtils.OptimizeQueueServiceEndpoint(requestContext, account1);
        AzureBlobGeoRedundancyUtils.OptimizeQueueServiceEndpoint(requestContext, account2);
        return storageAccountClients;
      })).ToList<AzureBlobGeoRedundancyCatchupJob.StorageAccountClients>();
      AzureBlobGeoRedundancyCatchupJob.ProcessingStats stats = new AzureBlobGeoRedundancyCatchupJob.ProcessingStats();
      DateTime timeStarted = DateTime.Now;
      StringBuilder resultMessageBuilder = new StringBuilder();
      using (CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        linkedTokenSource.CancelAfter(this.m_maxRuntime + TimeSpan.FromSeconds(300.0));
        CancellationToken cancellationToken = linkedTokenSource.Token;
        try
        {
          int numberOfQueues = service1.GetNumberOfQueues(requestContext);
          resultMessageBuilder.AppendLine(string.Format("Processing {0} queues for {1} clients.", (object) numberOfQueues, (object) list.Count));
          foreach (AzureBlobGeoRedundancyCatchupJob.StorageAccountClients clients in list)
          {
            for (int i = 0; i < numberOfQueues; ++i)
            {
              string queueName = AzureBlobGeoRedundancyUtils.GetQueueName(i);
              await this.ProcessMultiBatchAsync(clients.PrimaryQueueClient.GetQueueReference(queueName), clients.PrimaryBlobClient, clients.SecondaryBlobClient, stats, cancellationToken);
              await this.ProcessMultiBatchAsync(clients.SecondaryQueueClient.GetQueueReference(queueName), clients.SecondaryBlobClient, clients.PrimaryBlobClient, stats, cancellationToken);
              queueName = (string) null;
            }
          }
          if (DateTime.Now - timeStarted > this.m_maxRuntime)
          {
            requestContext.Trace(15302008, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Job exceeded max runtime of {0}, terminating.", (object) this.m_maxRuntime);
            resultMessageBuilder.AppendLine(string.Format("Job exceeded max runtime of {0}, terminating.", (object) this.m_maxRuntime));
          }
        }
        catch (Exception ex)
        {
          resultMessageBuilder.AppendLine(string.Format("Caught Exception: {0}. Accounts: {1}, Successful: {2}, BlockedCreateBlob: {3}, BlockedDeleteBlob: {4}, BlockedCreateContainer: {5}, BlockedDeleteContainer: {6}, Expired: {7}, Failed: {8}, Batches: {9}, BatchProcessingTime: {10}ms, TotalBytesTransferred: {11}.", (object) ex, (object) storageAccountSettings.Count<GeoRedundantStorageAccountSettings>(), (object) stats.SuccessfulMessages, (object) stats.BlockedCreateBlobMessages, (object) stats.BlockedDeleteBlobMessages, (object) stats.BlockedCreateContainerMessages, (object) stats.BlockedDeleteContainerMessages, (object) stats.ExpiredMessages, (object) stats.FailedMessages, (object) stats.Batches, (object) stats.BatchProcessingTime, (object) stats.TotalBytesTransferred));
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, resultMessageBuilder.ToString());
        }
        cancellationToken = new CancellationToken();
      }
      resultMessageBuilder.AppendLine(string.Format("Processed contents of queues. Accounts: {0}, Successful: {1}, BlockedCreateBlob: {2}, BlockedDeleteBlob: {3}, BlockedCreateContainer: {4}, BlockedDeleteContainer: {5}, Expired: {6}, Failed: {7}, Batches: {8}, BatchProcessingTime: {9}ms, TotalBytesTransferred: {10}.", (object) storageAccountSettings.Count<GeoRedundantStorageAccountSettings>(), (object) stats.SuccessfulMessages, (object) stats.BlockedCreateBlobMessages, (object) stats.BlockedDeleteBlobMessages, (object) stats.BlockedCreateContainerMessages, (object) stats.BlockedDeleteContainerMessages, (object) stats.ExpiredMessages, (object) stats.FailedMessages, (object) stats.Batches, (object) stats.BatchProcessingTime, (object) stats.TotalBytesTransferred));
      return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, resultMessageBuilder.ToString());
    }

    protected async Task<List<CloudQueueMessage>> FetchBatchAsync(
      CloudQueue queue,
      int size,
      CancellationToken cancellationToken)
    {
      List<CloudQueueMessage> result = new List<CloudQueueMessage>();
      while (result.Count < size)
      {
        try
        {
          int num = size - result.Count;
          IEnumerable<CloudQueueMessage> messagesAsync = await queue.GetMessagesAsync(num > 32 ? 32 : num, new TimeSpan?(this.m_visibilityTimeout), (QueueRequestOptions) null, (OperationContext) null, cancellationToken);
          if (messagesAsync.Any<CloudQueueMessage>())
            result.AddRange(messagesAsync);
          else
            break;
        }
        catch (Exception ex) when (AzureBlobGeoRedundancyCatchupJob.IsQueueNotFound(ex))
        {
          break;
        }
      }
      List<CloudQueueMessage> cloudQueueMessageList = result;
      result = (List<CloudQueueMessage>) null;
      return cloudQueueMessageList;
    }

    internal async Task ProcessMultiBatchAsync(
      CloudQueue queue,
      CloudBlobClient localBlobClient,
      CloudBlobClient remoteBlobClient,
      AzureBlobGeoRedundancyCatchupJob.ProcessingStats stats,
      CancellationToken cancellationToken)
    {
      bool flag = true;
      while (flag)
      {
        for (int i = 0; i < this.m_maxBatchesPerQueue & flag; ++i)
          flag = await this.ProcessBatchAsync(queue, localBlobClient, remoteBlobClient, stats, cancellationToken);
      }
    }

    internal async Task<bool> ProcessBatchAsync(
      CloudQueue queue,
      CloudBlobClient localBlobClient,
      CloudBlobClient remoteBlobClient,
      AzureBlobGeoRedundancyCatchupJob.ProcessingStats stats,
      CancellationToken cancellationToken)
    {
      Stopwatch processingTime = Stopwatch.StartNew();
      List<CloudQueueMessage> queueMessages = await this.FetchBatchAsync(queue, this.m_batchSize, cancellationToken);
      if (queueMessages.Count == 0)
        return false;
      ConcurrentDictionary<Task, CloudQueueMessage> activeTasks = new ConcurrentDictionary<Task, CloudQueueMessage>();
      using (new Timer((TimerCallback) (state => this.KeepAliveCallback(queue, (IEnumerable<CloudQueueMessage>) activeTasks.Values)), (object) null, this.m_keepAliveInterval, this.m_keepAliveInterval))
      {
        foreach (CloudQueueMessage queueMessage in queueMessages)
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          activeTasks.TryAdd(this.ProcessMessageAsync(queue, localBlobClient, remoteBlobClient, queueMessage, stats, AzureBlobGeoRedundancyCatchupJob.\u003C\u003EO.\u003C0\u003E__TraceRaw ?? (AzureBlobGeoRedundancyCatchupJob.\u003C\u003EO.\u003C0\u003E__TraceRaw = new TraceMethod(TeamFoundationTracingService.TraceRaw)), cancellationToken), queueMessage);
        }
        while (activeTasks.Count > 0)
        {
          Task completedTask = await Task.WhenAny((IEnumerable<Task>) activeTasks.Keys);
          await completedTask;
          activeTasks.TryRemove(completedTask, out CloudQueueMessage _);
          completedTask = (Task) null;
        }
      }
      stats.ProcessedBatch(processingTime.Elapsed);
      return queueMessages.Count == this.m_batchSize;
    }

    private void KeepAliveCallback(CloudQueue queue, IEnumerable<CloudQueueMessage> activeMessages)
    {
      foreach (CloudQueueMessage activeMessage in activeMessages)
      {
        try
        {
          queue.UpdateMessage(activeMessage, this.m_visibilityTimeout, MessageUpdateFields.Visibility);
        }
        catch (Exception ex)
        {
          BlobActionMessage blobActionMessage = BlobActionMessage.FromJson(activeMessage.AsString);
          if (AzureBlobGeoRedundancyCatchupJob.IsQueueMessageNotFound(ex))
            TeamFoundationTracingService.TraceRaw(15302012, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Attempted to update visibility but message not found. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, MessageId: {5}", (object) queue.ServiceClient.Credentials?.AccountName, (object) queue.Name, (object) blobActionMessage.Action, (object) blobActionMessage.ContainerName, (object) blobActionMessage.BlobName, (object) activeMessage.Id);
          else
            TeamFoundationTracingService.TraceRaw(15302005, TraceLevel.Warning, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Caught exception while updating visibility timeout. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, MessageId: {5}, Exception: {6}", (object) queue.ServiceClient.Credentials?.AccountName, (object) queue.Name, (object) blobActionMessage.Action, (object) blobActionMessage.ContainerName, (object) blobActionMessage.BlobName, (object) activeMessage.Id, (object) ex);
        }
      }
    }

    internal async Task ProcessMessageAsync(
      CloudQueue queue,
      CloudBlobClient localBlobClient,
      CloudBlobClient remoteBlobClient,
      CloudQueueMessage queueMessage,
      AzureBlobGeoRedundancyCatchupJob.ProcessingStats stats,
      TraceMethod traceMethod,
      CancellationToken cancellationToken)
    {
      BlobActionMessage decodedMessage = BlobActionMessage.FromJson(queueMessage.AsString);
      traceMethod(15302009, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Processing message. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, DequeueCount: {5}, MessageId: {6}", new object[7]
      {
        (object) queue.ServiceClient.Credentials?.AccountName,
        (object) queue.Name,
        (object) decodedMessage.Action,
        (object) decodedMessage.ContainerName,
        (object) decodedMessage.BlobName,
        (object) queueMessage.DequeueCount,
        (object) queueMessage.Id
      });
      AzureBlobGeoRedundancyCatchupJob.MessageResult result;
      switch (decodedMessage.Action)
      {
        case BlobAction.CreateBlob:
          result = await AzureBlobGeoRedundancyCatchupJob.ProcessCreateBlobAsync(localBlobClient, remoteBlobClient, decodedMessage.ContainerName, decodedMessage.BlobName, stats, traceMethod, cancellationToken);
          break;
        case BlobAction.DeleteBlob:
          result = await AzureBlobGeoRedundancyCatchupJob.ProcessDeleteBlobAsync(localBlobClient, remoteBlobClient, decodedMessage.ContainerName, decodedMessage.BlobName, traceMethod, cancellationToken);
          break;
        case BlobAction.CreateContainer:
          result = await AzureBlobGeoRedundancyCatchupJob.ProcessCreateContainerAsync(localBlobClient, remoteBlobClient, decodedMessage.ContainerName, traceMethod, cancellationToken);
          break;
        case BlobAction.DeleteContainer:
          result = await AzureBlobGeoRedundancyCatchupJob.ProcessDeleteContainerAsync(localBlobClient, remoteBlobClient, decodedMessage.ContainerName, traceMethod, cancellationToken);
          break;
        default:
          throw new InvalidOperationException(string.Format("{0} with value {1} is not supported", (object) "BlobAction", (object) decodedMessage.Action));
      }
      if (result == AzureBlobGeoRedundancyCatchupJob.MessageResult.Successful)
      {
        try
        {
          await queue.DeleteMessageAsync(queueMessage, cancellationToken);
        }
        catch (Exception ex) when (AzureBlobGeoRedundancyCatchupJob.IsQueueMessageNotFound(ex))
        {
          traceMethod(15302012, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Attempted to delete message but message not found. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, DequeueCount: {5}, MessageId: {6}", new object[7]
          {
            (object) queue.ServiceClient.Credentials?.AccountName,
            (object) queue.Name,
            (object) decodedMessage.Action,
            (object) decodedMessage.ContainerName,
            (object) decodedMessage.BlobName,
            (object) queueMessage.DequeueCount,
            (object) queueMessage.Id
          });
        }
        stats.IncrementSuccessful();
        traceMethod(15302010, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Successfully processed message. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, DequeueCount: {5}, MessageId: {6}", new object[7]
        {
          (object) queue.ServiceClient.Credentials?.AccountName,
          (object) queue.Name,
          (object) decodedMessage.Action,
          (object) decodedMessage.ContainerName,
          (object) decodedMessage.BlobName,
          (object) queueMessage.DequeueCount,
          (object) queueMessage.Id
        });
        decodedMessage = (BlobActionMessage) null;
      }
      else if (result == AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked && queueMessage.InsertionTime.HasValue && DateTimeOffset.UtcNow - queueMessage.InsertionTime.Value > this.m_expirationTime)
      {
        traceMethod(15302007, TraceLevel.Warning, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Message expired. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, DequeueCount: {5}, MessageId: {6}", new object[7]
        {
          (object) queue.ServiceClient.Credentials?.AccountName,
          (object) queue.Name,
          (object) decodedMessage.Action,
          (object) decodedMessage.ContainerName,
          (object) decodedMessage.BlobName,
          (object) queueMessage.DequeueCount,
          (object) queueMessage.Id
        });
        try
        {
          await queue.DeleteMessageAsync(queueMessage, cancellationToken);
        }
        catch (Exception ex) when (AzureBlobGeoRedundancyCatchupJob.IsQueueMessageNotFound(ex))
        {
          traceMethod(15302012, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Attempted to delete expired message but message not found. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, DequeueCount: {5}, MessageId: {6}", new object[7]
          {
            (object) queue.ServiceClient.Credentials?.AccountName,
            (object) queue.Name,
            (object) decodedMessage.Action,
            (object) decodedMessage.ContainerName,
            (object) decodedMessage.BlobName,
            (object) queueMessage.DequeueCount,
            (object) queueMessage.Id
          });
        }
        stats.IncrementExpired();
        decodedMessage = (BlobActionMessage) null;
      }
      else
      {
        if (queueMessage.DequeueCount > 1)
        {
          TimeSpan visibilityTimeout = TimeSpan.FromTicks(this.m_visibilityTimeout.Ticks * (long) queueMessage.DequeueCount);
          if (visibilityTimeout > this.m_maxVisibilityTimeout)
            visibilityTimeout = this.m_maxVisibilityTimeout;
          traceMethod(15302006, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Message failed to process multiple times. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, DequeueCount: {5}, VisibilityTimeout: {6}", new object[7]
          {
            (object) queue.ServiceClient.Credentials?.AccountName,
            (object) queue.Name,
            (object) decodedMessage.Action,
            (object) decodedMessage.ContainerName,
            (object) decodedMessage.BlobName,
            (object) queueMessage.DequeueCount,
            (object) visibilityTimeout
          });
          await queue.UpdateMessageAsync(queueMessage, visibilityTimeout, MessageUpdateFields.Visibility, cancellationToken);
        }
        if (result == AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked)
        {
          switch (decodedMessage.Action)
          {
            case BlobAction.CreateBlob:
              stats.IncrementBlockedCreateBlob();
              break;
            case BlobAction.DeleteBlob:
              stats.IncrementBlockedDeleteBlob();
              break;
            case BlobAction.CreateContainer:
              stats.IncrementBlockedCreateContainer();
              break;
            case BlobAction.DeleteContainer:
              stats.IncrementBlockedDeleteContainer();
              break;
          }
        }
        else
          stats.IncrementFailed();
        traceMethod(15302011, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Failed to process message. Account: {0}, Queue: {1}, Action: {2}, Container: {3}, Blob: {4}, DequeueCount: {5}, MessageId: {6}, Result: {7}", new object[8]
        {
          (object) queue.ServiceClient.Credentials?.AccountName,
          (object) queue.Name,
          (object) decodedMessage.Action,
          (object) decodedMessage.ContainerName,
          (object) decodedMessage.BlobName,
          (object) queueMessage.DequeueCount,
          (object) queueMessage.Id,
          (object) result
        });
        decodedMessage = (BlobActionMessage) null;
      }
    }

    internal static async Task<AzureBlobGeoRedundancyCatchupJob.MessageResult> ProcessCreateBlobAsync(
      CloudBlobClient localBlobClient,
      CloudBlobClient remoteBlobClient,
      string containerName,
      string blobName,
      AzureBlobGeoRedundancyCatchupJob.ProcessingStats stats,
      TraceMethod traceMethod,
      CancellationToken cancellationToken)
    {
      int num1;
      try
      {
        CloudBlobContainer localContainer = localBlobClient.GetContainerReference(containerName);
        CloudBlobContainer remoteContainer = remoteBlobClient.GetContainerReference(containerName);
        int num2 = await AzureBlobGeoRedundancyUtils.SetupSecondaryContainerAsync(localContainer, remoteContainer, traceMethod, cancellationToken) ? 1 : 0;
        CloudBlob localBlob = localContainer.GetBlobReference(blobName);
        if (!await localBlob.ExistsAsync(cancellationToken))
          return AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked;
        long sizeInBytes = localBlob.Properties.Length;
        CloudBlob cloudBlob;
        switch (localBlob.BlobType)
        {
          case BlobType.PageBlob:
            localBlob = (CloudBlob) localContainer.GetPageBlobReference(blobName);
            cloudBlob = (CloudBlob) remoteContainer.GetPageBlobReference(blobName);
            break;
          case BlobType.BlockBlob:
            localBlob = (CloudBlob) localContainer.GetBlockBlobReference(blobName);
            cloudBlob = (CloudBlob) remoteContainer.GetBlockBlobReference(blobName);
            break;
          case BlobType.AppendBlob:
            localBlob = (CloudBlob) localContainer.GetAppendBlobReference(blobName);
            cloudBlob = (CloudBlob) remoteContainer.GetAppendBlobReference(blobName);
            break;
          default:
            throw new InvalidOperationException(string.Format("Blob type '{0}' is not supported", (object) localBlob.BlobType));
        }
        SingleTransferContext context = new SingleTransferContext();
        EventHandler<TransferEventArgs> addBytesDelegate = (EventHandler<TransferEventArgs>) ((sender, args) => stats.AddTransferredBytes(sizeInBytes));
        ((TransferContext) context).FileTransferred += addBytesDelegate;
        await TransferManager.CopyAsync(localBlob, cloudBlob, (CopyMethod) 0, (CopyOptions) null, context, cancellationToken);
        ((TransferContext) context).FileTransferred -= addBytesDelegate;
        return AzureBlobGeoRedundancyCatchupJob.MessageResult.Successful;
      }
      catch (Exception ex)
      {
        num1 = 1;
      }
      if (num1 == 1)
      {
        Exception e = ex;
        if (e is TransferException transferException1 && transferException1.ErrorCode == 14)
          return AzureBlobGeoRedundancyCatchupJob.MessageResult.Successful;
        if (AzureBlobGeoRedundancyUtils.IsDualWriteBlockIdError(e))
        {
          int num3 = await AzureBlobGeoRedundancyUtils.TryRecoverDualWriteBlockIdError(remoteBlobClient.GetContainerReference(containerName).GetBlockBlobReference(blobName), traceMethod, cancellationToken) ? 1 : 0;
          return AzureBlobGeoRedundancyCatchupJob.MessageResult.Failed;
        }
        if (e is TransferException transferException2 && ((Exception) transferException2).InnerException != null)
          traceMethod(15302000, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Caught exception while processing {0} message. Account: {1}, Container: {2}, Blob: {3}, InnerException: {4}", new object[5]
          {
            (object) "CreateBlob",
            (object) localBlobClient.Credentials?.AccountName,
            (object) containerName,
            (object) blobName,
            (object) ((Exception) transferException2).InnerException
          });
        traceMethod(15302000, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Caught exception while processing {0} message. Account: {1}, Container: {2}, Blob: {3}, Exception: {4}", new object[5]
        {
          (object) "CreateBlob",
          (object) localBlobClient.Credentials?.AccountName,
          (object) containerName,
          (object) blobName,
          (object) e
        });
        return AzureBlobGeoRedundancyCatchupJob.MessageResult.Failed;
      }
      AzureBlobGeoRedundancyCatchupJob.MessageResult blobAsync;
      return blobAsync;
    }

    internal static async Task<AzureBlobGeoRedundancyCatchupJob.MessageResult> ProcessDeleteBlobAsync(
      CloudBlobClient localBlobClient,
      CloudBlobClient remoteBlobClient,
      string containerName,
      string blobName,
      TraceMethod traceMethod,
      CancellationToken cancellationToken)
    {
      try
      {
        return await localBlobClient.GetContainerReference(containerName).GetBlobReference(blobName).ExistsAsync(cancellationToken) ? AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked : (await remoteBlobClient.GetContainerReference(containerName).GetBlobReference(blobName).DeleteIfExistsAsync(cancellationToken) ? AzureBlobGeoRedundancyCatchupJob.MessageResult.Successful : AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked);
      }
      catch (Exception ex)
      {
        traceMethod(15302001, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Caught exception while processing {0} message. Account: {1}, Container: {2}, Blob: {3}, Exception: {4}", new object[5]
        {
          (object) "DeleteBlob",
          (object) localBlobClient.Credentials?.AccountName,
          (object) containerName,
          (object) blobName,
          (object) ex
        });
        return AzureBlobGeoRedundancyCatchupJob.MessageResult.Failed;
      }
    }

    internal static async Task<AzureBlobGeoRedundancyCatchupJob.MessageResult> ProcessCreateContainerAsync(
      CloudBlobClient localBlobClient,
      CloudBlobClient remoteBlobClient,
      string containerName,
      TraceMethod traceMethod,
      CancellationToken cancellationToken)
    {
      try
      {
        CloudBlobContainer localContainer = localBlobClient.GetContainerReference(containerName);
        if (!await localContainer.ExistsAsync(cancellationToken))
          return AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked;
        int num = await AzureBlobGeoRedundancyUtils.SetupSecondaryContainerAsync(localContainer, remoteBlobClient.GetContainerReference(containerName), traceMethod, cancellationToken) ? 1 : 0;
        return AzureBlobGeoRedundancyCatchupJob.MessageResult.Successful;
      }
      catch (Exception ex)
      {
        traceMethod(15302003, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Caught exception while processing {0} message. Account: {1}, Container: {2}, Exception: {3}", new object[4]
        {
          (object) "CreateContainer",
          (object) localBlobClient.Credentials?.AccountName,
          (object) containerName,
          (object) ex
        });
        return AzureBlobGeoRedundancyCatchupJob.MessageResult.Failed;
      }
    }

    internal static async Task<AzureBlobGeoRedundancyCatchupJob.MessageResult> ProcessDeleteContainerAsync(
      CloudBlobClient localBlobClient,
      CloudBlobClient remoteBlobClient,
      string containerName,
      TraceMethod traceMethod,
      CancellationToken cancellationToken)
    {
      try
      {
        return await localBlobClient.GetContainerReference(containerName).ExistsAsync(cancellationToken) ? AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked : (await remoteBlobClient.GetContainerReference(containerName).DeleteIfExistsAsync(cancellationToken) ? AzureBlobGeoRedundancyCatchupJob.MessageResult.Successful : AzureBlobGeoRedundancyCatchupJob.MessageResult.Blocked);
      }
      catch (Exception ex)
      {
        traceMethod(15302004, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyCatchupJob), "Caught exception while processing {0} message. Account: {1}, Container: {2}, Exception: {3}", new object[4]
        {
          (object) "DeleteContainer",
          (object) localBlobClient.Credentials?.AccountName,
          (object) containerName,
          (object) ex
        });
        return AzureBlobGeoRedundancyCatchupJob.MessageResult.Failed;
      }
    }

    private static bool IsQueueMessageNotFound(Exception e)
    {
      if (e is StorageException storageException)
      {
        RequestResult requestInformation = storageException.RequestInformation;
        if ((requestInformation != null ? (requestInformation.HttpStatusCode == 404 ? 1 : 0) : 0) != 0)
          return storageException.RequestInformation?.ExtendedErrorInformation?.ErrorCode == QueueErrorCodeStrings.MessageNotFound;
      }
      return false;
    }

    private static bool IsQueueNotFound(Exception e)
    {
      if (e is StorageException storageException)
      {
        RequestResult requestInformation = storageException.RequestInformation;
        if ((requestInformation != null ? (requestInformation.HttpStatusCode == 404 ? 1 : 0) : 0) != 0)
          return storageException.RequestInformation?.ExtendedErrorInformation?.ErrorCode == QueueErrorCodeStrings.QueueNotFound;
      }
      return false;
    }

    private class StorageAccountClients
    {
      public CloudBlobClient PrimaryBlobClient { get; internal set; }

      public CloudBlobClient SecondaryBlobClient { get; internal set; }

      public CloudQueueClient PrimaryQueueClient { get; internal set; }

      public CloudQueueClient SecondaryQueueClient { get; internal set; }
    }

    internal enum MessageResult
    {
      Successful,
      Blocked,
      Failed,
    }

    internal class ProcessingStats
    {
      private long m_batches;
      private long m_batchProcessingTime;
      private long m_successful;
      private long m_blockedCreateBlob;
      private long m_blockedDeleteBlob;
      private long m_blockedCreateContainer;
      private long m_blockedDeleteContainer;
      private long m_expired;
      private long m_failed;
      private long m_totalBytesTransferred;

      public long Batches => Interlocked.Read(ref this.m_batches);

      public long BatchProcessingTime => Interlocked.Read(ref this.m_batchProcessingTime);

      public long SuccessfulMessages => Interlocked.Read(ref this.m_successful);

      public long BlockedCreateBlobMessages => Interlocked.Read(ref this.m_blockedCreateBlob);

      public long BlockedDeleteBlobMessages => Interlocked.Read(ref this.m_blockedDeleteBlob);

      public long BlockedCreateContainerMessages => Interlocked.Read(ref this.m_blockedCreateContainer);

      public long BlockedDeleteContainerMessages => Interlocked.Read(ref this.m_blockedDeleteContainer);

      public long ExpiredMessages => Interlocked.Read(ref this.m_expired);

      public long FailedMessages => Interlocked.Read(ref this.m_failed);

      public long TotalBytesTransferred => Interlocked.Read(ref this.m_totalBytesTransferred);

      public void ProcessedBatch(TimeSpan processingTime)
      {
        Interlocked.Increment(ref this.m_batches);
        Interlocked.Add(ref this.m_batchProcessingTime, (long) processingTime.TotalMilliseconds);
      }

      public void IncrementSuccessful() => Interlocked.Increment(ref this.m_successful);

      public void AddTransferredBytes(long sizeInBytes) => Interlocked.Add(ref this.m_totalBytesTransferred, sizeInBytes);

      public void IncrementBlockedCreateBlob() => Interlocked.Increment(ref this.m_blockedCreateBlob);

      public void IncrementBlockedDeleteBlob() => Interlocked.Increment(ref this.m_blockedDeleteBlob);

      public void IncrementBlockedCreateContainer() => Interlocked.Increment(ref this.m_blockedCreateContainer);

      public void IncrementBlockedDeleteContainer() => Interlocked.Increment(ref this.m_blockedDeleteContainer);

      public void IncrementExpired() => Interlocked.Increment(ref this.m_expired);

      public void IncrementFailed() => Interlocked.Increment(ref this.m_failed);
    }
  }
}
