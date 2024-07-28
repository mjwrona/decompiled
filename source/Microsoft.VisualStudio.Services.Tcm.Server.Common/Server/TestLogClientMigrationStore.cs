// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogClientMigrationStore
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestLogClientMigrationStore : ITestLogClientMigrationStore
  {
    private ILogStoreConnectionEndpoint _sourceLogStoreConnectionEndpoint;
    private ILogStoreConnectionEndpoint _targetLogStoreConnectionEndpoint;

    public TestLogClientMigrationStore(
      IVssRequestContext requestContext,
      ILogStoreConnectionEndpoint sourceLogStoreConnectionEndpoint,
      ILogStoreConnectionEndpoint targetLogStoreConnectionEndpoint)
    {
      this._sourceLogStoreConnectionEndpoint = sourceLogStoreConnectionEndpoint;
      this._targetLogStoreConnectionEndpoint = targetLogStoreConnectionEndpoint;
      this.ConfigureDMLParallelSetting();
    }

    public async Task<LogStoreContainerCopyResponse> CopyContainersInParallelAsync(
      IVssRequestContext requestContext,
      List<string> containerNames,
      bool isServiceCopy,
      int maxDegreeParallelism,
      int retryCount,
      CancellationToken cancellationToken)
    {
      requestContext.Trace(1015662, TraceLevel.Info, "TestManagement", "LogStorage", "Executing Action: TestLogClientMigrationStore.CopyContainersInParallelAsync with maxDegreeParallelism - {0}, retryCount - {1}", (object) maxDegreeParallelism, (object) retryCount);
      LogStoreContainerCopyResponse copyResponse = new LogStoreContainerCopyResponse();
      copyResponse.Status = LogStoreContainerCopyStatus.Completed;
      List<LogStoreContainerCopyDetails> containerDetails = new List<LogStoreContainerCopyDetails>();
      copyResponse.ContainerCopyDetails = containerDetails;
      List<Task> tasksWithThrottler = new List<Task>();
      try
      {
        using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(maxDegreeParallelism))
        {
          foreach (string containerName1 in containerNames)
          {
            string containerName = containerName1;
            await semaphoreSlim.WaitAsync();
            tasksWithThrottler.Add(Task.Run((Func<Task>) (async () =>
            {
              try
              {
                LogStoreContainerCopyDetails containerCopyDetails = await this.CopyContainerAsync(containerName, isServiceCopy, retryCount, cancellationToken).ConfigureAwait(false);
                containerDetails.Add(containerCopyDetails);
              }
              finally
              {
                semaphoreSlim.Release();
              }
            })));
          }
          await Task.WhenAll(tasksWithThrottler.ToArray()).ConfigureAwait(false);
        }
        requestContext.Trace(1015662, TraceLevel.Info, "TestManagement", "LogStorage", "Action: TestLogClientMigrationStore.CopyContainersInParallelAsync has passed");
      }
      catch (Exception ex)
      {
        copyResponse.Status = LogStoreContainerCopyStatus.Failed;
        copyResponse.StatusReason = "Message: " + ex.Message + " InnerMessage: " + ex.InnerException?.Message;
        requestContext.Trace(1015662, TraceLevel.Warning, "TestManagement", "LogStorage", "CopyContainers - Failed, ExceptionHit: {0} ", (object) ex.Message);
      }
      LogStoreContainerCopyResponse containerCopyResponse = copyResponse;
      copyResponse = (LogStoreContainerCopyResponse) null;
      tasksWithThrottler = (List<Task>) null;
      return containerCopyResponse;
    }

    private async Task<LogStoreContainerCopyDetails> CopyContainerAsync(
      string containerName,
      bool isServiceCopy,
      int retryCount,
      CancellationToken cancellationToken)
    {
      int attempt = 1;
      LogStoreContainerCopyDetails copyDetails = new LogStoreContainerCopyDetails();
      copyDetails.ContainerName = containerName;
      for (copyDetails.Status = LogStoreContainerCopyStatus.Started; copyDetails.Status != LogStoreContainerCopyStatus.FailedWithException && copyDetails.Status != LogStoreContainerCopyStatus.Completed && attempt <= retryCount; ++attempt)
      {
        try
        {
          copyDetails = await this.CopyContainerInternalAsync(containerName, isServiceCopy, retryCount, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
          if (ex is StorageException storageException)
          {
            RequestResult requestInformation1 = storageException.RequestInformation;
            if ((requestInformation1 != null ? (requestInformation1.HttpStatusCode == 403 ? 1 : 0) : 0) != 0)
            {
              if (ex.InnerException != null)
              {
                if (ex.InnerException is StorageException innerException)
                {
                  RequestResult requestInformation2 = innerException.RequestInformation;
                  if ((requestInformation2 != null ? (requestInformation2.HttpStatusCode == 403 ? 1 : 0) : 0) != 0)
                    continue;
                }
              }
              else
                continue;
            }
          }
          copyDetails.Status = LogStoreContainerCopyStatus.FailedWithException;
          copyDetails.StatusReason = "Message:" + ex.Message + " InnerMessage: " + ex.InnerException?.Message;
        }
      }
      copyDetails.StatusReason += string.Format(" RetryCount: {0}", (object) (attempt - 1));
      LogStoreContainerCopyDetails containerCopyDetails = copyDetails;
      copyDetails = (LogStoreContainerCopyDetails) null;
      return containerCopyDetails;
    }

    private async Task<LogStoreContainerCopyDetails> CopyContainerInternalAsync(
      string containerName,
      bool isServiceCopy,
      int retryCount,
      CancellationToken cancellationToken)
    {
      LogStoreContainerCopyDetails copyDetails = new LogStoreContainerCopyDetails();
      copyDetails.Status = LogStoreContainerCopyStatus.Started;
      copyDetails.ContainerName = containerName;
      CloudBlobContainer sourceContainer = this._sourceLogStoreConnectionEndpoint.GetCloudBlobClient().GetContainerReference(containerName);
      CloudBlobContainer targetContainer = this._targetLogStoreConnectionEndpoint.GetCloudBlobClient().GetContainerReference(containerName);
      int num1 = await targetContainer.CreateIfNotExistsAsync().ConfigureAwait(false) ? 1 : 0;
      CloudBlobDirectory directoryReference1 = sourceContainer.GetDirectoryReference(string.Empty);
      CloudBlobDirectory directoryReference2 = targetContainer.GetDirectoryReference(string.Empty);
      CopyDirectoryOptions directoryOptions1 = new CopyDirectoryOptions();
      ((DirectoryOptions) directoryOptions1).Recursive = true;
      CopyDirectoryOptions directoryOptions2 = directoryOptions1;
      DirectoryTransferContext directoryTransferContext1 = new DirectoryTransferContext((TransferCheckpoint) null);
      long failedFileCount = 0;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      directoryTransferContext1.ShouldTransferCallbackAsync = TestLogClientMigrationStore.\u003C\u003Ec.\u003C\u003E9__3_0 ?? (TestLogClientMigrationStore.\u003C\u003Ec.\u003C\u003E9__3_0 = new ShouldTransferCallbackAsync((object) TestLogClientMigrationStore.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CCopyContainerInternalAsync\u003Eb__3_0)));
      ((TransferContext) directoryTransferContext1).FileFailed += (EventHandler<TransferEventArgs>) ((sender, arg) =>
      {
        if (arg.Exception is StorageException exception2)
        {
          RequestResult requestInformation = exception2.RequestInformation;
          if ((requestInformation != null ? (requestInformation.HttpStatusCode == 404 ? 1 : 0) : 0) != 0 && exception2.RequestInformation?.ExtendedErrorInformation?.ErrorCode == BlobErrorCodeStrings.BlobNotFound)
            return;
        }
        Interlocked.Increment(ref failedFileCount);
      });
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      ((TransferContext) directoryTransferContext1).ShouldOverwriteCallbackAsync = TestLogClientMigrationStore.\u003C\u003EO.\u003C0\u003E__ForceOverwrite ?? (TestLogClientMigrationStore.\u003C\u003EO.\u003C0\u003E__ForceOverwrite = new ShouldOverwriteCallbackAsync((object) null, __methodptr(ForceOverwrite)));
      this.ConfigureDMLParallelSetting();
      CloudBlobDirectory cloudBlobDirectory = directoryReference2;
      int num2 = isServiceCopy ? 1 : 0;
      CopyDirectoryOptions directoryOptions3 = directoryOptions2;
      DirectoryTransferContext directoryTransferContext2 = directoryTransferContext1;
      CancellationToken cancellationToken1 = cancellationToken;
      TransferStatus transferStatus = await TransferManager.CopyDirectoryAsync(directoryReference1, cloudBlobDirectory, (CopyMethod) num2, directoryOptions3, directoryTransferContext2, cancellationToken1).ConfigureAwait(false);
      copyDetails.Status = failedFileCount != 0L ? LogStoreContainerCopyStatus.Failed : LogStoreContainerCopyStatus.Completed;
      copyDetails.StatusReason = string.Format("Files Transferred: {0}, Failed: {1}, Skipped: {2}", (object) transferStatus.NumberOfFilesTransferred, (object) failedFileCount, (object) transferStatus.NumberOfFilesSkipped);
      LogStoreContainerCopyDetails containerCopyDetails = copyDetails;
      copyDetails = (LogStoreContainerCopyDetails) null;
      sourceContainer = (CloudBlobContainer) null;
      targetContainer = (CloudBlobContainer) null;
      return containerCopyDetails;
    }

    private void ConfigureDMLParallelSetting()
    {
      TransferManager.Configurations.ParallelOperations = Environment.ProcessorCount * 8;
      TransferManager.Configurations.UserAgentPrefix = "TCMLogStoreJobAgent";
    }
  }
}
