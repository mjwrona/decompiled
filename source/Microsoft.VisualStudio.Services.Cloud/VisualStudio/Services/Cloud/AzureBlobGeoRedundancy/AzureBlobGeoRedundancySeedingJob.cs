// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancySeedingJob
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancySeedingJob : VssAsyncJobExtension
  {
    private TimeSpan m_statusUpdateInterval = AzureBlobGeoRedundancySeedingJob.s_defaultStatusUpdateInterval;
    private int m_maxActiveTasks = 8;
    private bool m_applyOptimizations = true;
    private int m_retries = 3;
    internal static readonly string s_baseRegistryPath = "/Service/AzureBlobGeoRedundancy/Settings/Seeding";
    internal static readonly string s_seedingSnapshotStateRegistryPath = "/AzureBlobGeoRedundancy/SnapshotState";
    internal static readonly string s_maxActiveTasksRegistryPath = AzureBlobGeoRedundancySeedingJob.s_baseRegistryPath + "/MaxActiveTasks";
    internal static readonly string s_statusUpdateIntervalRegistryPath = AzureBlobGeoRedundancySeedingJob.s_baseRegistryPath + "/StatusUpdateInterval";
    internal static readonly string s_applyOptimizationsRegistryPath = AzureBlobGeoRedundancySeedingJob.s_baseRegistryPath + "/ApplyOptimizations";
    internal static readonly string s_retriesRegistryPath = AzureBlobGeoRedundancySeedingJob.s_baseRegistryPath + "/Retries";
    private static readonly TimeSpan s_defaultStatusUpdateInterval = TimeSpan.FromMinutes(1.0);
    private const int c_defaultMaxActiveTasks = 8;
    private const bool c_defaultApplyOptimizations = true;
    private const int c_defaultRetries = 3;
    private const int c_maxBatchSize = 100;
    private const int c_maxStringPropertyLength = 32768;
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancySeedingJob";

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      AzureBlobGeoRedundancySeedingJobData redundancySeedingJobData = TeamFoundationSerializationUtility.Deserialize<AzureBlobGeoRedundancySeedingJobData>(jobDefinition.Data);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string stateRegistryPath = AzureBlobGeoRedundancySeedingJob.GetSnapshotStateRegistryPath(redundancySeedingJobData.PrimaryLookupKey);
      SnapshotState snapshotState = service.GetValue<SnapshotState>(requestContext, (RegistryQuery) stateRegistryPath, SnapshotState.None);
      if (!Enum.GetValues(typeof (SnapshotState)).Cast<SnapshotState>().Contains<SnapshotState>(snapshotState))
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, string.Format("Unknown snapshot state '{0}'! See '{1}'.", (object) snapshotState, (object) stateRegistryPath));
      this.m_maxActiveTasks = service.GetValue<int>(requestContext, (RegistryQuery) AzureBlobGeoRedundancySeedingJob.s_maxActiveTasksRegistryPath, 8);
      this.m_statusUpdateInterval = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) AzureBlobGeoRedundancySeedingJob.s_statusUpdateIntervalRegistryPath, AzureBlobGeoRedundancySeedingJob.s_defaultStatusUpdateInterval);
      this.m_applyOptimizations = service.GetValue<bool>(requestContext, (RegistryQuery) AzureBlobGeoRedundancySeedingJob.s_applyOptimizationsRegistryPath, true);
      this.m_retries = service.GetValue<int>(requestContext, (RegistryQuery) AzureBlobGeoRedundancySeedingJob.s_retriesRegistryPath, 3);
      string drawerName = redundancySeedingJobData.DrawerName;
      string primaryLookupKey = redundancySeedingJobData.PrimaryLookupKey;
      string secondaryLookupKey = redundancySeedingJobData.SecondaryLookupKey;
      AzureBlobGeoRedundancySeedingJob.StorageAccountCopySettings storageAccountCopySettings = new AzureBlobGeoRedundancySeedingJob.StorageAccountCopySettings()
      {
        Id = primaryLookupKey,
        PrimaryConnectionString = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, drawerName, primaryLookupKey),
        SecondaryConnectionString = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, drawerName, secondaryLookupKey),
        OverwriteExisting = redundancySeedingJobData.OverwriteExisting,
        ContinueOnError = redundancySeedingJobData.ContinueOnError
      };
      try
      {
        if (snapshotState == SnapshotState.SnapshotStarted)
        {
          snapshotState = SnapshotState.None;
          requestContext.Trace(15301006, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancySeedingJob), "Detected an incomplete snapshot. Deleting existing snapshot and restarting.");
        }
        if (snapshotState == SnapshotState.None)
          await this.CreateSnapshotAsync(requestContext, storageAccountCopySettings);
        return !await this.CopyContainersAsync(requestContext, storageAccountCopySettings) ? new VssJobResult(TeamFoundationJobExecutionResult.PartiallySucceeded, "Job finished but not all containers completed!") : new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, "Successfully copied all containers!");
      }
      catch (Exception ex) when (AzureBlobGeoRedundancySeedingJob.IsCancelledException(ex))
      {
        return new VssJobResult(TeamFoundationJobExecutionResult.PartiallySucceeded, "Job was cancelled!");
      }
    }

    private async Task CreateSnapshotAsync(
      IVssRequestContext requestContext,
      AzureBlobGeoRedundancySeedingJob.StorageAccountCopySettings storageAccountCopySettings)
    {
      IVssRegistryService registryService = requestContext.GetService<IVssRegistryService>();
      registryService.SetValue<SnapshotState>(requestContext, AzureBlobGeoRedundancySeedingJob.GetSnapshotStateRegistryPath(storageAccountCopySettings.Id), SnapshotState.SnapshotStarted);
      Microsoft.Azure.Storage.CloudStorageAccount account = Microsoft.Azure.Storage.CloudStorageAccount.Parse(storageAccountCopySettings.PrimaryConnectionString);
      CloudTableClient cloudTableClient = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(storageAccountCopySettings.PrimaryConnectionString).CreateCloudTableClient();
      CloudBlobClient blobClient = account.CreateCloudBlobClient();
      if (this.m_applyOptimizations)
      {
        AzureBlobGeoRedundancyUtils.OptimizeTableServiceEndpoint(requestContext, account);
        AzureBlobGeoRedundancyUtils.OptimizeBlobServiceEndpoint(requestContext, account);
      }
      CloudTable queuedSnapshotTable = cloudTableClient.GetTableReference("AzureBlobGeoRedundancyQueued");
      CloudTable completedSnapshotTable = cloudTableClient.GetTableReference("AzureBlobGeoRedundancyCompleted");
      CloudBlobContainer checkpointContainer = blobClient.GetContainerReference("azureblobgeoredundancycheckpoints");
      int num1 = await queuedSnapshotTable.DeleteIfExistsAsync(requestContext.CancellationToken) ? 1 : 0;
      int num2 = await completedSnapshotTable.DeleteIfExistsAsync(requestContext.CancellationToken) ? 1 : 0;
      int num3 = await checkpointContainer.DeleteIfExistsAsync(requestContext.CancellationToken) ? 1 : 0;
      BackoffRetryManager backoffRetryManager = new BackoffRetryManager(BackoffRetryManager.ExponentialDelay(10, TimeSpan.FromMinutes(1.0)), (BackoffRetryManager.OnExceptionHandler) (retryContext =>
      {
        if (retryContext.Exception is Microsoft.Azure.Storage.StorageException exception3)
        {
          Microsoft.Azure.Storage.RequestResult requestInformation = exception3.RequestInformation;
          if ((requestInformation != null ? (requestInformation.HttpStatusCode == 409 ? 1 : 0) : 0) != 0)
            return true;
        }
        if (!(retryContext.Exception is Microsoft.Azure.Cosmos.Table.StorageException exception4))
          return false;
        Microsoft.Azure.Cosmos.Table.RequestResult requestInformation1 = exception4.RequestInformation;
        return requestInformation1 != null && requestInformation1.HttpStatusCode == 409;
      }));
      backoffRetryManager.Invoke((Action) (() => queuedSnapshotTable.CreateIfNotExists()));
      backoffRetryManager.Invoke((Action) (() => completedSnapshotTable.CreateIfNotExists()));
      backoffRetryManager.Invoke((Action) (() => checkpointContainer.CreateIfNotExists()));
      BlobContinuationToken continuationToken = (BlobContinuationToken) null;
      TableBatchOperation batchOperation = new TableBatchOperation();
      string currentPartitionKey = (string) null;
      do
      {
        ContainerResultSegment segment = await blobClient.ListContainersSegmentedAsync(continuationToken, requestContext.CancellationToken);
        foreach (CloudBlobContainer result in segment.Results)
        {
          if (!string.Equals(result.Name, "azureblobgeoredundancycheckpoints", StringComparison.Ordinal))
          {
            QueuedContainerEntity containerEntity = new QueuedContainerEntity(result.Name);
            containerEntity.Status = "Created entry.";
            containerEntity.QueuedDate = new DateTime?(DateTime.UtcNow);
            if ((string.Equals(currentPartitionKey, containerEntity.PartitionKey, StringComparison.Ordinal) ? 0 : (batchOperation.Count > 0 ? 1 : 0)) != 0 || batchOperation.Count >= 100)
            {
              TableBatchResult tableBatchResult = await queuedSnapshotTable.ExecuteBatchAsync(batchOperation, requestContext.CancellationToken);
              batchOperation = new TableBatchOperation();
            }
            batchOperation.Insert((ITableEntity) containerEntity);
            currentPartitionKey = containerEntity.PartitionKey;
            containerEntity = (QueuedContainerEntity) null;
          }
        }
        continuationToken = segment.ContinuationToken;
        segment = (ContainerResultSegment) null;
      }
      while (continuationToken != null);
      if (batchOperation.Count > 0)
      {
        TableBatchResult tableBatchResult1 = await queuedSnapshotTable.ExecuteBatchAsync(batchOperation, requestContext.CancellationToken);
      }
      registryService.SetValue<SnapshotState>(requestContext, AzureBlobGeoRedundancySeedingJob.GetSnapshotStateRegistryPath(storageAccountCopySettings.Id), SnapshotState.SnapshotCompleted);
      registryService = (IVssRegistryService) null;
      blobClient = (CloudBlobClient) null;
      batchOperation = (TableBatchOperation) null;
      currentPartitionKey = (string) null;
    }

    private async Task<bool> CopyContainersAsync(
      IVssRequestContext requestContext,
      AzureBlobGeoRedundancySeedingJob.StorageAccountCopySettings storageAccountCopySettings)
    {
      Microsoft.Azure.Storage.CloudStorageAccount account1 = Microsoft.Azure.Storage.CloudStorageAccount.Parse(storageAccountCopySettings.PrimaryConnectionString);
      Microsoft.Azure.Storage.CloudStorageAccount account2 = Microsoft.Azure.Storage.CloudStorageAccount.Parse(storageAccountCopySettings.SecondaryConnectionString);
      CloudBlobClient cloudBlobClient1 = account1.CreateCloudBlobClient();
      CloudBlobClient cloudBlobClient2 = account2.CreateCloudBlobClient();
      CloudTableClient cloudTableClient = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(storageAccountCopySettings.PrimaryConnectionString).CreateCloudTableClient();
      if (this.m_applyOptimizations)
      {
        AzureBlobGeoRedundancyUtils.OptimizeBlobServiceEndpoint(requestContext, account1);
        AzureBlobGeoRedundancyUtils.OptimizeBlobServiceEndpoint(requestContext, account2);
        AzureBlobGeoRedundancyUtils.OptimizeTableServiceEndpoint(requestContext, account1);
      }
      return this.m_maxActiveTasks <= 1 ? await this.ExecuteCopySerialAsync(requestContext, cloudTableClient, cloudBlobClient1, cloudBlobClient2, storageAccountCopySettings) : await this.ExecuteCopyParallelAsync(requestContext, cloudTableClient, cloudBlobClient1, cloudBlobClient2, storageAccountCopySettings, this.m_maxActiveTasks);
    }

    private async Task<bool> ExecuteCopySerialAsync(
      IVssRequestContext requestContext,
      CloudTableClient tableClient,
      CloudBlobClient primaryBlobClient,
      CloudBlobClient secondaryBlobClient,
      AzureBlobGeoRedundancySeedingJob.StorageAccountCopySettings storageAccountCopySettings)
    {
      CloudTable queuedSnapshotTable = tableClient.GetTableReference("AzureBlobGeoRedundancyQueued");
      bool isComplete = true;
      TableContinuationToken token = (TableContinuationToken) null;
      TableQuery<QueuedContainerEntity> tableQuery = new TableQuery<QueuedContainerEntity>();
      do
      {
        TableQuerySegment<QueuedContainerEntity> tableQueryResult = await queuedSnapshotTable.ExecuteQuerySegmentedAsync<QueuedContainerEntity>(tableQuery, token, requestContext.CancellationToken);
        foreach (QueuedContainerEntity result in tableQueryResult.Results)
        {
          requestContext.CancellationToken.ThrowIfCancellationRequested();
          bool flag = isComplete;
          isComplete = flag & await this.CopyContainerAsync(tableClient, primaryBlobClient, secondaryBlobClient, result, storageAccountCopySettings.OverwriteExisting, storageAccountCopySettings.ContinueOnError, new TraceMethod(((VssRequestContextExtensions) requestContext).Trace), requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.EnableServiceSideCopy"), requestContext.CancellationToken);
        }
        token = tableQueryResult.ContinuationToken;
        tableQueryResult = (TableQuerySegment<QueuedContainerEntity>) null;
      }
      while (token != null);
      bool flag1 = isComplete;
      queuedSnapshotTable = (CloudTable) null;
      tableQuery = (TableQuery<QueuedContainerEntity>) null;
      return flag1;
    }

    private async Task<bool> ExecuteCopyParallelAsync(
      IVssRequestContext requestContext,
      CloudTableClient tableClient,
      CloudBlobClient primaryBlobClient,
      CloudBlobClient secondaryBlobClient,
      AzureBlobGeoRedundancySeedingJob.StorageAccountCopySettings storageAccountCopySettings,
      int maxActiveTasks)
    {
      CloudTable queuedSnapshotTable = tableClient.GetTableReference("AzureBlobGeoRedundancyQueued");
      bool isComplete = true;
      TableContinuationToken token = (TableContinuationToken) null;
      TableQuery<QueuedContainerEntity> tableQuery = new TableQuery<QueuedContainerEntity>();
      List<Task<bool>> activeCopyTasks = new List<Task<bool>>();
      using (CancellationTokenSource compositeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(requestContext.CancellationToken))
      {
        CancellationToken cancellationToken = compositeCancellationTokenSource.Token;
        try
        {
          bool flag;
          do
          {
            TableQuerySegment<QueuedContainerEntity> tableQueryResult = await queuedSnapshotTable.ExecuteQuerySegmentedAsync<QueuedContainerEntity>(tableQuery, token, cancellationToken);
            IEnumerator<QueuedContainerEntity> resultEnumerator = (IEnumerator<QueuedContainerEntity>) tableQueryResult.Results.GetEnumerator();
            while (resultEnumerator.MoveNext())
            {
              cancellationToken.ThrowIfCancellationRequested();
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              activeCopyTasks.Add(this.CopyContainerAsync(tableClient, primaryBlobClient, secondaryBlobClient, resultEnumerator.Current, storageAccountCopySettings.OverwriteExisting, storageAccountCopySettings.ContinueOnError, AzureBlobGeoRedundancySeedingJob.\u003C\u003EO.\u003C0\u003E__TraceRaw ?? (AzureBlobGeoRedundancySeedingJob.\u003C\u003EO.\u003C0\u003E__TraceRaw = new TraceMethod(TeamFoundationTracingService.TraceRaw)), requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.EnableServiceSideCopy"), cancellationToken));
              if (activeCopyTasks.Count >= maxActiveTasks)
              {
                Task<bool> task = await Task.WhenAny<bool>((IEnumerable<Task<bool>>) activeCopyTasks);
                activeCopyTasks.Remove(task);
                flag = isComplete;
                isComplete = flag & await task;
              }
            }
            token = tableQueryResult.ContinuationToken;
            tableQueryResult = (TableQuerySegment<QueuedContainerEntity>) null;
            resultEnumerator = (IEnumerator<QueuedContainerEntity>) null;
          }
          while (token != null);
          while (activeCopyTasks.Count > 0)
          {
            cancellationToken.ThrowIfCancellationRequested();
            Task<bool> task = await Task.WhenAny<bool>((IEnumerable<Task<bool>>) activeCopyTasks);
            activeCopyTasks.Remove(task);
            flag = isComplete;
            isComplete = flag & await task;
          }
        }
        catch (Exception ex1)
        {
          compositeCancellationTokenSource.Cancel();
          try
          {
            bool[] flagArray = await Task.WhenAll<bool>((IEnumerable<Task<bool>>) activeCopyTasks);
          }
          catch (Exception ex2)
          {
          }
          throw;
        }
        cancellationToken = new CancellationToken();
      }
      bool flag1 = isComplete;
      queuedSnapshotTable = (CloudTable) null;
      tableQuery = (TableQuery<QueuedContainerEntity>) null;
      activeCopyTasks = (List<Task<bool>>) null;
      return flag1;
    }

    private async Task<bool> CopyContainerAsync(
      CloudTableClient tableClient,
      CloudBlobClient primaryBlobClient,
      CloudBlobClient secondaryBlobClient,
      QueuedContainerEntity containerEntity,
      bool overwriteExisting,
      bool continueOnError,
      TraceMethod traceMethod,
      bool useServiceSideCopy,
      CancellationToken cancellationToken)
    {
      int attempt = 1;
      bool successful;
      for (successful = false; !successful && attempt <= this.m_retries; ++attempt)
      {
        cancellationToken.ThrowIfCancellationRequested();
        try
        {
          successful = await this.CopyContainerInternalAsync(tableClient, primaryBlobClient, secondaryBlobClient, containerEntity, overwriteExisting, attempt, traceMethod, useServiceSideCopy, cancellationToken);
        }
        catch (Exception ex) when (!AzureBlobGeoRedundancySeedingJob.IsForbiddenException(ex))
        {
          traceMethod(15301003, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancySeedingJob), ex.ToReadableStackTrace(), Array.Empty<object>());
          if (!cancellationToken.IsCancellationRequested)
            await AzureBlobGeoRedundancySeedingJob.UpdateStatusAsync<QueuedContainerEntity>(tableClient.GetTableReference("AzureBlobGeoRedundancyQueued"), containerEntity.ContainerName, string.Format("Caught exception! Attempt {0} of {1}. Message: {2}", (object) attempt, (object) this.m_retries, (object) ex.ToReadableStackTrace()), cancellationToken);
          if (continueOnError)
            return false;
          throw;
        }
      }
      return successful;
    }

    private async Task<bool> CopyContainerInternalAsync(
      CloudTableClient tableClient,
      CloudBlobClient primaryBlobClient,
      CloudBlobClient secondaryBlobClient,
      QueuedContainerEntity containerEntity,
      bool overwriteExisting,
      int attempt,
      TraceMethod traceMethod,
      bool useServiceSideCopy,
      CancellationToken cancellationToken)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      AzureBlobGeoRedundancySeedingJob.\u003C\u003Ec__DisplayClass6_0 cDisplayClass60 = new AzureBlobGeoRedundancySeedingJob.\u003C\u003Ec__DisplayClass6_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.traceMethod = traceMethod;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.attempt = attempt;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.containerName = containerEntity.ContainerName;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.traceMethod(15301000, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancySeedingJob), "Begin copying container. Container: {0}, Attempt: {1} of {2}.", new object[3]
      {
        (object) cDisplayClass60.containerName,
        (object) cDisplayClass60.attempt,
        (object) this.m_retries
      });
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.queuedSnapshotTable = tableClient.GetTableReference("AzureBlobGeoRedundancyQueued");
      CloudTable completedSnapshotTable = tableClient.GetTableReference("AzureBlobGeoRedundancyCompleted");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.primaryContainer = primaryBlobClient.GetContainerReference(cDisplayClass60.containerName);
      // ISSUE: reference to a compiler-generated field
      CloudBlobContainer secondaryContainer = secondaryBlobClient.GetContainerReference(cDisplayClass60.containerName);
      primaryBlobClient.GetContainerReference("azureblobgeoredundancycheckpoints");
      // ISSUE: reference to a compiler-generated field
      if (!await cDisplayClass60.primaryContainer.ExistsAsync(cancellationToken))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        string deletedContainerStatus = string.Format("Container deleted. Container: {0}. Attempt: {1} of {2}.", (object) cDisplayClass60.containerName, (object) cDisplayClass60.attempt, (object) this.m_retries);
        // ISSUE: reference to a compiler-generated field
        await AzureBlobGeoRedundancySeedingJob.CompleteContainerAsync(cDisplayClass60.queuedSnapshotTable, completedSnapshotTable, containerEntity, deletedContainerStatus, cancellationToken);
        // ISSUE: reference to a compiler-generated field
        cDisplayClass60.traceMethod(15301001, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancySeedingJob), deletedContainerStatus, Array.Empty<object>());
        return true;
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      int num = await AzureBlobGeoRedundancyUtils.SetupSecondaryContainerAsync(cDisplayClass60.primaryContainer, secondaryContainer, cDisplayClass60.traceMethod, cancellationToken) ? 1 : 0;
      // ISSUE: reference to a compiler-generated field
      CloudBlobDirectory primaryDirectory = cDisplayClass60.primaryContainer.GetDirectoryReference("");
      CloudBlobDirectory secondaryDirectory = secondaryContainer.GetDirectoryReference("");
      CopyDirectoryOptions directoryOptions = new CopyDirectoryOptions();
      ((DirectoryOptions) directoryOptions).Recursive = true;
      CopyDirectoryOptions options = directoryOptions;
      TransferCheckpoint checkpoint = (TransferCheckpoint) null;
      // ISSUE: reference to a compiler-generated field
      QueuedContainerEntity entity = await AzureBlobGeoRedundancySeedingJob.QueryContainerAsync<QueuedContainerEntity>(cDisplayClass60.queuedSnapshotTable, containerEntity.ContainerName, cancellationToken);
      // ISSUE: reference to a compiler-generated field
      entity.Status = string.Format(checkpoint == null ? "Starting copy. Attempt {0} of {1}." : "Resuming copy. Attempt {0} of {1}.", (object) cDisplayClass60.attempt, (object) this.m_retries);
      entity.StartedDate = new DateTime?(DateTime.UtcNow);
      // ISSUE: reference to a compiler-generated field
      await AzureBlobGeoRedundancySeedingJob.UpdateAsync<QueuedContainerEntity>(cDisplayClass60.queuedSnapshotTable, entity, cancellationToken);
      DirectoryTransferContext state = new DirectoryTransferContext(checkpoint);
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.numberOfFilesThatAlreadyExist = 0L;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.numberOfFilesDeletedFromSource = 0L;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.numberOfFailedFiles = 0L;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass60.progressRecorder = new AzureBlobGeoRedundancySeedingJob.ProgressRecorder();
      // ISSUE: reference to a compiler-generated field
      ((TransferContext) state).ProgressHandler = (IProgress<TransferStatus>) cDisplayClass60.progressRecorder;
      if (!overwriteExisting)
      {
        // ISSUE: method pointer
        state.ShouldTransferCallbackAsync = new ShouldTransferCallbackAsync((object) cDisplayClass60, __methodptr(\u003CCopyContainerInternalAsync\u003Eb__0));
      }
      // ISSUE: reference to a compiler-generated method
      ((TransferContext) state).FileFailed += new EventHandler<TransferEventArgs>(cDisplayClass60.\u003CCopyContainerInternalAsync\u003Eb__1);
      if (overwriteExisting)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        ((TransferContext) state).ShouldOverwriteCallbackAsync = AzureBlobGeoRedundancySeedingJob.\u003C\u003EO.\u003C1\u003E__ForceOverwrite ?? (AzureBlobGeoRedundancySeedingJob.\u003C\u003EO.\u003C1\u003E__ForceOverwrite = new ShouldOverwriteCallbackAsync((object) null, __methodptr(ForceOverwrite)));
      }
      TransferStatus transferStatus = (TransferStatus) null;
      // ISSUE: reference to a compiler-generated method
      TimerCallback callback = new TimerCallback(cDisplayClass60.\u003CCopyContainerInternalAsync\u003Eb__2);
      CopyMethod copyMethod = useServiceSideCopy ? (CopyMethod) 2 : (CopyMethod) 0;
      using (new Timer(callback, (object) state, this.m_statusUpdateInterval, this.m_statusUpdateInterval))
        transferStatus = await TransferManager.CopyDirectoryAsync(primaryDirectory, secondaryDirectory, copyMethod, options, state, cancellationToken);
      bool cancellationRequested = cancellationToken.IsCancellationRequested;
      // ISSUE: reference to a compiler-generated field
      bool isComplete = cDisplayClass60.numberOfFailedFiles == 0L && !cancellationRequested;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string status = string.Format("Container copy {0}. Transferred: {1}. Skipped: {2}. Exist on target: {3}. Deleted from source: {4}. Failed: {5}. Total bytes: {6}. Container: {7}. Attempt: {8} of {9}.", isComplete ? (object) "completed" : (cancellationRequested ? (object) "canceled" : (object) "failed"), (object) transferStatus.NumberOfFilesTransferred, (object) transferStatus.NumberOfFilesSkipped, (object) cDisplayClass60.numberOfFilesThatAlreadyExist, (object) cDisplayClass60.numberOfFilesDeletedFromSource, (object) cDisplayClass60.numberOfFailedFiles, (object) transferStatus.BytesTransferred, (object) cDisplayClass60.containerName, (object) cDisplayClass60.attempt, (object) this.m_retries);
      if (isComplete)
      {
        // ISSUE: reference to a compiler-generated field
        await AzureBlobGeoRedundancySeedingJob.CompleteContainerAsync(cDisplayClass60.queuedSnapshotTable, completedSnapshotTable, containerEntity, status, cancellationToken);
        // ISSUE: reference to a compiler-generated field
        cDisplayClass60.traceMethod(15301001, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancySeedingJob), status, Array.Empty<object>());
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        await AzureBlobGeoRedundancySeedingJob.UpdateStatusAsync<QueuedContainerEntity>(cDisplayClass60.queuedSnapshotTable, cDisplayClass60.containerName, status, cancellationToken);
        // ISSUE: reference to a compiler-generated field
        cDisplayClass60.traceMethod(15301002, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancySeedingJob), status, Array.Empty<object>());
      }
      return isComplete;
    }

    private static async Task CompleteContainerAsync(
      CloudTable queuedSnapshotTable,
      CloudTable completedSnapshotTable,
      QueuedContainerEntity containerEntity,
      string status,
      CancellationToken cancellationToken)
    {
      CompletedContainerEntity entity = new CompletedContainerEntity(containerEntity.ContainerName);
      entity.Status = AzureBlobGeoRedundancySeedingJob.TrimStatusMessage(status);
      entity.QueuedDate = containerEntity.QueuedDate;
      entity.StartedDate = containerEntity.StartedDate;
      entity.CompletedDate = new DateTime?(DateTime.UtcNow);
      await AzureBlobGeoRedundancySeedingJob.InsertOrReplaceAsync<CompletedContainerEntity>(completedSnapshotTable, entity, cancellationToken);
      await AzureBlobGeoRedundancySeedingJob.DeleteAsync<QueuedContainerEntity>(queuedSnapshotTable, containerEntity.ContainerName, cancellationToken);
    }

    private static async Task<T> QueryContainerAsync<T>(
      CloudTable table,
      string containerName,
      CancellationToken cancellationToken)
      where T : ITableEntity
    {
      return (T) (await table.ExecuteAsync(TableOperation.Retrieve<T>(ContainerEntity.GetPartitionKey(containerName), containerName), cancellationToken)).Result;
    }

    private static T QueryContainer<T>(CloudTable table, string containerName) where T : ITableEntity
    {
      TableOperation operation = TableOperation.Retrieve<T>(ContainerEntity.GetPartitionKey(containerName), containerName);
      return (T) table.Execute(operation).Result;
    }

    private static async Task UpdateAsync<T>(
      CloudTable table,
      T entity,
      CancellationToken cancellationToken)
      where T : ITableEntity
    {
      TableResult tableResult = await table.ExecuteAsync(TableOperation.Replace((ITableEntity) entity), cancellationToken);
    }

    private static void Update<T>(CloudTable table, T entity) where T : ITableEntity
    {
      TableOperation operation = TableOperation.Replace((ITableEntity) entity);
      table.Execute(operation);
    }

    private static async Task UpdateStatusAsync<T>(
      CloudTable table,
      string containerName,
      string status,
      CancellationToken cancellationToken)
      where T : ContainerEntity
    {
      T entity = await AzureBlobGeoRedundancySeedingJob.QueryContainerAsync<T>(table, containerName, cancellationToken);
      entity.Status = AzureBlobGeoRedundancySeedingJob.TrimStatusMessage(status);
      await AzureBlobGeoRedundancySeedingJob.UpdateAsync<T>(table, entity, cancellationToken);
    }

    private static void UpdateStatus<T>(CloudTable table, string containerName, string status) where T : ContainerEntity
    {
      T entity = AzureBlobGeoRedundancySeedingJob.QueryContainer<T>(table, containerName);
      entity.Status = AzureBlobGeoRedundancySeedingJob.TrimStatusMessage(status);
      AzureBlobGeoRedundancySeedingJob.Update<T>(table, entity);
    }

    private static async Task InsertOrReplaceAsync<T>(
      CloudTable table,
      T entity,
      CancellationToken cancellationToken)
      where T : ITableEntity
    {
      TableResult tableResult = await table.ExecuteAsync(TableOperation.InsertOrReplace((ITableEntity) entity), cancellationToken);
    }

    private static async Task DeleteAsync<T>(
      CloudTable table,
      T entity,
      CancellationToken cancellationToken)
      where T : ITableEntity
    {
      TableResult tableResult = await table.ExecuteAsync(TableOperation.Delete((ITableEntity) entity), cancellationToken);
    }

    private static async Task DeleteAsync<T>(
      CloudTable table,
      string containerName,
      CancellationToken cancellationToken)
      where T : ContainerEntity
    {
      await AzureBlobGeoRedundancySeedingJob.DeleteAsync<T>(table, await AzureBlobGeoRedundancySeedingJob.QueryContainerAsync<T>(table, containerName, cancellationToken), cancellationToken);
    }

    private static string TrimStatusMessage(string status)
    {
      if (status.Length > 32768)
        status = status.Substring(0, 32765) + "...";
      return status;
    }

    internal static string GetSnapshotStateRegistryPath(string storageAccountName) => AzureBlobGeoRedundancySeedingJob.s_seedingSnapshotStateRegistryPath + "/" + storageAccountName;

    private static bool IsCancelledException(Exception e)
    {
      switch (e)
      {
        case TaskCanceledException _:
        case OperationCanceledException _:
          return true;
        case AggregateException aggregateException:
          return aggregateException.InnerExceptions.Any<Exception>((Func<Exception, bool>) (ie => ie is TaskCanceledException || ie is OperationCanceledException));
        default:
          return false;
      }
    }

    private static bool IsForbiddenException(Exception e)
    {
      if (e is Microsoft.Azure.Storage.StorageException storageException1)
      {
        Microsoft.Azure.Storage.RequestResult requestInformation = storageException1.RequestInformation;
        if ((requestInformation != null ? (requestInformation.HttpStatusCode == 403 ? 1 : 0) : 0) != 0)
          return true;
      }
      if (!(e is Microsoft.Azure.Cosmos.Table.StorageException storageException2))
        return false;
      Microsoft.Azure.Cosmos.Table.RequestResult requestInformation1 = storageException2.RequestInformation;
      return requestInformation1 != null && requestInformation1.HttpStatusCode == 403;
    }

    private class StorageAccountCopySettings
    {
      public string Id { get; internal set; }

      public string PrimaryConnectionString { get; internal set; }

      public string SecondaryConnectionString { get; internal set; }

      public bool OverwriteExisting { get; internal set; }

      public bool ContinueOnError { get; internal set; }
    }

    private class ProgressRecorder : IProgress<TransferStatus>
    {
      private long m_latestBytesTransferred;
      private long m_latestNumberOfFilesTransferred;
      private long m_latestNumberOfFilesSkipped;
      private long m_latestNumberOfFilesFailed;

      public void Report(TransferStatus progress)
      {
        this.m_latestBytesTransferred = progress.BytesTransferred;
        this.m_latestNumberOfFilesTransferred = progress.NumberOfFilesTransferred;
        this.m_latestNumberOfFilesSkipped = progress.NumberOfFilesSkipped;
        this.m_latestNumberOfFilesFailed = progress.NumberOfFilesFailed;
      }

      public long LatestBytesTransferred => this.m_latestBytesTransferred;

      public long LatestNumberOfFilesTransferred => this.m_latestNumberOfFilesTransferred;

      public long LatestNumberOfFilesSkipped => this.m_latestNumberOfFilesSkipped;

      public long LatestNumberOfFilesFailed => this.m_latestNumberOfFilesFailed;
    }
  }
}
