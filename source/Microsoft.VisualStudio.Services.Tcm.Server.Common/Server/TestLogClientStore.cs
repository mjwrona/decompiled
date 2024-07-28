// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestLogClientStore
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestLogClientStore : ITestLogClientStore
  {
    private ILogStoreConnectionEndpoint _logStoreConnectionEndpoint;
    private LogStorePathFormatter _logStorePathFormatter;
    private CloudBlobContainer _cloudBlobContainer;
    private string _containerName;

    public TestLogClientStore(
      IVssRequestContext requestContext,
      ILogStoreConnectionEndpoint logStoreConnectionEndpoint,
      string containerName)
    {
      this._containerName = LogStoreHelper.SanatizeContainerName(requestContext, containerName);
      this._logStoreConnectionEndpoint = logStoreConnectionEndpoint;
      this._logStorePathFormatter = new LogStorePathFormatter();
      this._initializeContainerReference(requestContext);
      this._configureParallelOperations(requestContext);
    }

    public async Task<TestLogStatus> UploadAsync(
      IVssRequestContext requestContext,
      string logFileSourcePath,
      string blobName,
      bool allowDuplicate,
      Dictionary<string, string> metaData,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogUploadOptions testLogUploadOptions,
      CancellationToken cancellationToken)
    {
      TestLogStatus logStatus = new TestLogStatus();
      logStatus.Status = TestLogStatusCode.Success;
      try
      {
        await this._startUploadAsync(requestContext, logFileSourcePath, blobName, false, metaData, testLogSingleTransferContext, testLogUploadOptions, cancellationToken).ConfigureAwait(false);
      }
      catch (TransferSkippedException ex)
      {
        if (allowDuplicate)
        {
          await this._startUploadAsync(requestContext, logFileSourcePath, blobName, true, metaData, testLogSingleTransferContext, testLogUploadOptions, cancellationToken).ConfigureAwait(false);
        }
        else
        {
          requestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "FileAlreadyExists - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ((Exception) ex).Message);
          return this._getTestLogReturnStatus(TestLogStatusCode.FileAlreadyExists, (Exception) ex);
        }
      }
      catch (TransferException ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "UploadFailedTransferException - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ((Exception) ex).Message);
        return this._getTestLogReturnStatus(TestLogStatusCode.TransferFailed, (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "UploadBlobFailed - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ex.Message);
        return this._getTestLogReturnStatus(TestLogStatusCode.Failed, ex);
      }
      return logStatus;
    }

    public async Task<TestLogStatus> UploadAsync(
      IVssRequestContext requestContext,
      Stream logFileStream,
      string blobName,
      bool allowDuplicate,
      Dictionary<string, string> metaData,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogUploadOptions testLogUploadOptions,
      CancellationToken cancellationToken)
    {
      TestLogStatus logStatus = new TestLogStatus();
      logStatus.Status = TestLogStatusCode.Success;
      try
      {
        await this._startUploadAsync(requestContext, logFileStream, blobName, false, metaData, testLogSingleTransferContext, testLogUploadOptions, cancellationToken).ConfigureAwait(false);
      }
      catch (TransferSkippedException ex)
      {
        if (allowDuplicate)
        {
          await this._startUploadAsync(requestContext, logFileStream, blobName, true, metaData, testLogSingleTransferContext, testLogUploadOptions, cancellationToken).ConfigureAwait(false);
        }
        else
        {
          requestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "FileAlreadyExists - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ((Exception) ex).Message);
          return this._getTestLogReturnStatus(TestLogStatusCode.FileAlreadyExists, (Exception) ex);
        }
      }
      catch (TransferException ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "UploadFailedTransferException - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ((Exception) ex).Message);
        return this._getTestLogReturnStatus(TestLogStatusCode.TransferFailed, (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "UploadBlobFailed - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ex.Message);
        return this._getTestLogReturnStatus(TestLogStatusCode.Failed, ex);
      }
      return logStatus;
    }

    public async Task<TestLogStatus> DownloadAsync(
      IVssRequestContext requestContext,
      string blobName,
      Stream stream,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogDownloadOptions testLogDownloadOptions,
      CancellationToken cancellationToken)
    {
      new TestLogStatus().Status = TestLogStatusCode.Success;
      TestLogStatus testLogStatus;
      try
      {
        testLogStatus = await this._startDownloadAsync(requestContext, blobName, stream, testLogSingleTransferContext, testLogDownloadOptions, cancellationToken).ConfigureAwait(false);
        stream.Position = 0L;
      }
      catch (TransferException ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "DownloadFailedTransferException - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ((Exception) ex).Message);
        return this._getTestLogReturnStatus(TestLogStatusCode.TransferFailed, (Exception) ex);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "DownloadBlobFailed - For blob {0}. Exception Hit: {1}", (object) blobName, (object) ex.Message);
        return this._getTestLogReturnStatus(TestLogStatusCode.Failed, ex);
      }
      return testLogStatus;
    }

    private TestLogStatus _getTestLogReturnStatus(TestLogStatusCode testLogStatusCode, Exception ex)
    {
      TestLogStatus testLogReturnStatus = new TestLogStatus();
      testLogReturnStatus.Exception = ex.ToString();
      testLogReturnStatus.Status = testLogStatusCode;
      if (ex is TransferException)
        testLogReturnStatus.TransferFailureType = ((TransferException) ex).ErrorCode.ToString();
      return testLogReturnStatus;
    }

    private async Task _startUploadAsync(
      IVssRequestContext requestContext,
      string logFileSourcePath,
      string blobName,
      bool isDuplicate,
      Dictionary<string, string> metaData,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogUploadOptions testLogUploadOptions,
      CancellationToken cancellationToken)
    {
      await TransferManager.UploadAsync(logFileSourcePath, (CloudBlob) this._getBlobReference(blobName, isDuplicate, metaData), testLogUploadOptions.GetUploadOptions(), testLogSingleTransferContext.GetSingleTransferContext(), cancellationToken).ConfigureAwait(false);
    }

    private async Task _startUploadAsync(
      IVssRequestContext requestContext,
      Stream stream,
      string blobName,
      bool isDuplicate,
      Dictionary<string, string> metaData,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogUploadOptions testLogUploadOptions,
      CancellationToken cancellationToken)
    {
      await TransferManager.UploadAsync(stream, (CloudBlob) this._getBlobReference(blobName, isDuplicate, metaData), testLogUploadOptions.GetUploadOptions(), testLogSingleTransferContext.GetSingleTransferContext(), cancellationToken).ConfigureAwait(false);
    }

    private async Task<TestLogStatus> _startDownloadAsync(
      IVssRequestContext requestContext,
      string blobName,
      Stream stream,
      ITestLogSingleTransferContext testLogSingleTransferContext,
      ITestLogDownloadOptions testLogDownloadOptions,
      CancellationToken cancellationToken)
    {
      TestLogStatus testLogStatus = new TestLogStatus();
      testLogStatus.Status = TestLogStatusCode.Success;
      CloudBlockBlob sourceBlob = this._getBlobReference(blobName, false, (Dictionary<string, string>) null);
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(sourceBlob.Uri);
      int num = servicePoint != null ? servicePoint.CurrentConnections : -1;
      requestContext.Trace(1015684, TraceLevel.Info, "TestManagement", "LogStorage", string.Format("_startDownloadAsync: URI: {0}, DefaultConnectionLimit: {1}, CurrentConnections: {2}, Expect100Continue: {3}", (object) sourceBlob.Uri, (object) ServicePointManager.DefaultConnectionLimit, (object) num, (object) ServicePointManager.Expect100Continue));
      if (!await this._isValidFileSize(requestContext, sourceBlob).ConfigureAwait(false))
      {
        testLogStatus.Status = TestLogStatusCode.FileSizeExceeds;
        return testLogStatus;
      }
      await TransferManager.DownloadAsync((CloudBlob) sourceBlob, stream, testLogDownloadOptions.GetDownloadOptions(), testLogSingleTransferContext.GetSingleTransferContext(), cancellationToken).ConfigureAwait(false);
      return testLogStatus;
    }

    private async Task<bool> _isValidFileSize(
      IVssRequestContext requestContext,
      CloudBlockBlob cloudBlockBlob)
    {
      long fileSizeLimit = (long) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreStreamSizeLimitInBytes", 100000000);
      await cloudBlockBlob.FetchAttributesAsync().ConfigureAwait(false);
      if (cloudBlockBlob.Properties.Length <= fileSizeLimit)
        return true;
      requestContext.Trace(1015676, TraceLevel.Warning, "TestManagement", "LogStorage", "_isValidFileSize - File size greater than the limit set ");
      return false;
    }

    private CloudBlockBlob _getBlobReference(
      string blobName,
      bool isDuplicate,
      Dictionary<string, string> metaData)
    {
      if (isDuplicate)
        blobName = this._logStorePathFormatter.GetNameWhenDuplicate(blobName);
      CloudBlockBlob blockBlobReference = this._cloudBlobContainer.GetBlockBlobReference(blobName);
      this._setMetaData((CloudBlob) blockBlobReference, metaData);
      return blockBlobReference;
    }

    private void _setMetaData(CloudBlob destinationBlob, Dictionary<string, string> metaData)
    {
      if (metaData == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in metaData)
      {
        string str = (string) null;
        if (!destinationBlob.Metadata.TryGetValue(keyValuePair.Key, out str))
          destinationBlob.Metadata.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }

    private void _initializeContainerReference(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<string>(this._containerName, "_containerName", "LogStorage");
      ArgumentUtility.CheckForNull<CloudBlobClient>(this._logStoreConnectionEndpoint.GetCloudBlobClient(), "_logStoreConnectionEndpoint.GetCloudBlobClient()", "LogStorage");
      this._cloudBlobContainer = this._logStoreConnectionEndpoint.GetCloudBlobClient().GetContainerReference(this._containerName);
      if (!this._cloudBlobContainer.Exists() && !this._createContainerIfDoesNotExists(requestContext))
        throw new TestObjectNotFoundException("Container not found", ObjectTypes.Other);
    }

    private bool _createContainerIfDoesNotExists(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int serverTimeoutInSeconds = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreServerTimeoutInSeconds", 60);
      int maximumExecutionTimeInSeconds = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreServerMaxExecutionTimeInSeconds", 900);
      int maxAttempts = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreMaxAttemptsRetry", 10);
      int num = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreDeltaBackOffInSeconds", 3);
      LogStoreExponentialRetryPolicy logStoreExponentialRetryPolicy = new LogStoreExponentialRetryPolicy(requestContext, TimeSpan.FromSeconds((double) num), maxAttempts);
      this._cloudBlobContainer.CreateIfNotExistsAsync(new LogStoreContainerAccessPolicy().GetBlobContainerPublicAccessType(), new LogStoreRequestOption(serverTimeoutInSeconds, maximumExecutionTimeInSeconds, logStoreExponentialRetryPolicy).GetBlobRequestOptions(), new LogStoreOperationContext(DateTime.UtcNow.Date, requestContext.ActivityId.ToString()).GetOperationContext(), requestContext.CancellationToken).Wait();
      return this._cloudBlobContainer.Exists();
    }

    private void _configureParallelOperations(IVssRequestContext requestContext)
    {
      TransferManager.Configurations.ParallelOperations = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogBlobStoreParallelOperationCount", 8);
      TransferManager.Configurations.UserAgentPrefix = "TCMLogStoreJobAgent";
    }
  }
}
