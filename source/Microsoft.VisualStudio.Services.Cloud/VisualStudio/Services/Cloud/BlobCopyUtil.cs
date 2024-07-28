// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobCopyUtil
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.HostMigration;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using Microsoft.VisualStudio.Services.Cloud.HostMigration;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class BlobCopyUtil
  {
    private static readonly string s_area = "AzureBlobProvider";
    private static readonly string s_layer = nameof (BlobCopyUtil);
    internal static readonly string s_testFailOnQueue = "/Testing/BlobCopyUtil/FailOnQueue";
    internal static readonly string s_testInvalidateSasTokens = "/Testing/BlobCopyUtil/InvalidateSASTokens";
    internal static readonly string s_testInjectMissingBlobs = "/Testing/BlobCopyUtil/InjectMissingBlobs";
    internal static readonly string s_testFailingBlobCopy = "/Testing/BlobCopyUtil/FailingBlobCopy";
    internal static readonly string s_testBlobCopyDelay = "/Testing/BlobCopyUtil/BlobCopyDelay";
    internal static readonly string s_testCreateFileServiceTaggedFilesOnTargetHost = "/Testing/BlobCopyUtil/CreateFileServiceTaggedFilesOnTargetHost";
    internal static readonly string s_queueBlobCopyOperationName = "QueueBlobCopy";
    internal static readonly string s_disableBlobCopyContinuationTokenStorage = "Microsoft.VisualStudio.Services.Cloud.HostMigration.DisableBlobCopyContinuationTokenStorage";
    private static readonly string s_nameSeparator = "/";
    public static readonly string[] s_defaultContainerPrefixes = new string[36]
    {
      "0",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "a",
      "b",
      "c",
      "d",
      "e",
      "f",
      "g",
      "h",
      "i",
      "j",
      "k",
      "l",
      "m",
      "n",
      "o",
      "p",
      "q",
      "r",
      "s",
      "t",
      "u",
      "v",
      "w",
      "x",
      "y",
      "z"
    };
    public static readonly string[] s_defaultBlobPrefixes = new string[65]
    {
      "A",
      "B",
      "C",
      "D",
      "E",
      "F",
      "G",
      "H",
      "I",
      "J",
      "K",
      "L",
      "M",
      "N",
      "O",
      "P",
      "Q",
      "R",
      "S",
      "T",
      "U",
      "V",
      "W",
      "X",
      "Y",
      "Z",
      "a",
      "b",
      "c",
      "d",
      "e",
      "f",
      "g",
      "h",
      "i",
      "j",
      "k",
      "l",
      "m",
      "n",
      "o",
      "p",
      "q",
      "r",
      "s",
      "t",
      "u",
      "v",
      "w",
      "x",
      "y",
      "z",
      "0",
      "1",
      "2",
      "3",
      "4",
      "5",
      "6",
      "7",
      "8",
      "9",
      "/",
      "\\",
      "$"
    };

    public static long QueueBlobCopy(
      IVssRequestContext requestContext,
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobContainerWrapper targetContainer,
      int maxDegreeOfParallelism,
      bool skipNotFound,
      CancellationToken cancellationToken,
      Action<string> log,
      ICloudStorageAccountWrapper accountWrapper = null,
      string prefix = null,
      string startWithInclusive = null,
      string endWithInclusive = null,
      int maxResultsPerBatch = 5000,
      bool useCachedBlobContinuationToken = false,
      bool failIfUnexpectedBlobsOnTarget = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ICloudBlobContainerWrapper>(sourceContainer, nameof (sourceContainer));
      ArgumentUtility.CheckForNull<ICloudBlobContainerWrapper>(targetContainer, nameof (targetContainer));
      ArgumentUtility.CheckForOutOfRange(maxResultsPerBatch, nameof (maxResultsPerBatch), 0, 5000);
      if (log == null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        log = BlobCopyUtil.\u003C\u003EO.\u003C0\u003E__NoLog ?? (BlobCopyUtil.\u003C\u003EO.\u003C0\u003E__NoLog = new Action<string>(BlobCopyUtil.NoLog));
      }
      if (accountWrapper == null)
        accountWrapper = (ICloudStorageAccountWrapper) new CloudStorageAccountWrapper();
      useCachedBlobContinuationToken &= BlobCopyUtil.IsBlobContinuationTokenStorageEnabled(requestContext);
      TimeSpan[] retryDelays = BlobCopyUtil.GetRetryDelays(requestContext);
      bool failOnQueue = HostMigrationUtil.GetTestRegistryFlag(requestContext, BlobCopyUtil.s_testFailOnQueue, log);
      bool invalidateSasTokens = HostMigrationUtil.GetTestRegistryFlag(requestContext, BlobCopyUtil.s_testInvalidateSasTokens, log);
      bool failBlobCopy = HostMigrationUtil.GetTestRegistryFlag(requestContext, BlobCopyUtil.s_testFailingBlobCopy, log);
      if (failBlobCopy)
      {
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) BlobCopyUtil.s_testBlobCopyDelay, 120);
        log(string.Format("Sleeping for {0} seconds to allow target hosts to complete database migrations.  This should only occur in test environments.", (object) num));
        Thread.Sleep(TimeSpan.FromSeconds((double) num));
        retryDelays = Array.Empty<TimeSpan>();
      }
      BlockingCollection<BlobCopyRequest> blockingCollection = new BlockingCollection<BlobCopyRequest>(5000);
      long num1 = 0;
      long totalErrors = 0;
      long totalCopies = 0;
      long totalSkipped = 0;
      Task task = Task.Run((Action) (() =>
      {
        ParallelOptions parallelOptions = new ParallelOptions()
        {
          MaxDegreeOfParallelism = maxDegreeOfParallelism,
          CancellationToken = cancellationToken
        };
        Parallel.ForEach<BlobCopyRequest>(blockingCollection.GetConsumingEnumerable(), parallelOptions, (Action<BlobCopyRequest>) (blobCopyRequest =>
        {
          if (failOnQueue)
            throw new InvalidOperationException("Test registry setting at path '" + BlobCopyUtil.s_testFailOnQueue + "' is configured to block any new blobs from being queued for copy.  This value should only be set in test environments.");
          if (invalidateSasTokens | failBlobCopy)
            HostMigrationUtil.InvalidateCredentials(sourceContainer, log);
          BlobCopyUtil.CopyBlob(blobCopyRequest, skipNotFound, ref totalCopies, ref totalErrors, ref totalSkipped, retryDelays, log, nameof (QueueBlobCopy));
        }));
      }));
      IBlobCopyContext copyContext = (IBlobCopyContext) new BlobContainerCopyContext(sourceContainer, targetContainer, blockingCollection, failIfUnexpectedBlobsOnTarget, cancellationToken);
      TimeSpan period = TimeSpan.FromMinutes(1.0);
      BlobContainerEnumerationContinuationSettings continuationSettings = (BlobContainerEnumerationContinuationSettings) null;
      if (useCachedBlobContinuationToken)
        continuationSettings = new BlobContainerEnumerationContinuationSettings(BlobCopyUtil.s_queueBlobCopyOperationName);
      bool shouldLog = true;
      using (BlobContainerEnumerator sourceBlobEnumerator = new BlobContainerEnumerator(requestContext, accountWrapper, sourceContainer, BlobListingDetails.None, maxResultsPerBatch, prefix, retryDelays, continuationSettings, log))
      {
        using (BlobContainerEnumerator targetEnumerator = new BlobContainerEnumerator(requestContext, accountWrapper, targetContainer, BlobListingDetails.None, maxResultsPerBatch, prefix, retryDelays, continuationSettings, log))
        {
          using (new System.Threading.Timer((TimerCallback) (_ =>
          {
            if (!shouldLog)
              return;
            log(string.Format("Blob copy progress: {0} blobs processed, {1} blobs copied, {2} blobs skipped, {3} total errors (errors will be retried).", (object) sourceBlobEnumerator.BlobsEnumerated, (object) Interlocked.Read(ref totalCopies), (object) Interlocked.Read(ref totalSkipped), (object) Interlocked.Read(ref totalErrors)));
          }), (object) null, TimeSpan.FromSeconds(0.0), period))
          {
            BlobCopyUtil.IterateBlobs(requestContext, copyContext, (IVsoBlobEnumerator) sourceBlobEnumerator, (IVsoBlobEnumerator) targetEnumerator, startWithInclusive, endWithInclusive);
            num1 = sourceBlobEnumerator.BlobsEnumerated;
            shouldLog = false;
          }
        }
      }
      string str1;
      if (prefix == null)
        str1 = "";
      else
        str1 = " (prefix=" + prefix + "; source=" + sourceContainer.StorageAccountName + "; target=" + targetContainer.StorageAccountName + ")";
      string str2 = str1;
      blockingCollection.CompleteAdding();
      log("Enumeration completed" + str2 + ".  Waiting for copy requests to drain.");
      task.Wait(cancellationToken);
      log(string.Format("Copy threads have finished{0}. Total blobs in source: {1}. Total copies: {2}. Total errors: {3}. Total skipped {4}.", (object) str2, (object) num1, (object) totalCopies, (object) totalErrors, (object) totalSkipped));
      return totalCopies;
    }

    public static bool HasNotFoundError(Exception exception) => BlobCopyUtil.HasHttpStatusCode(exception, HttpStatusCode.NotFound) || BlobCopyUtil.HasExtendedError(exception, BlobErrorCodeStrings.BlobNotFound);

    public static bool HasPendingCopyOperation(Exception exception) => BlobCopyUtil.HasExtendedError(exception, BlobErrorCodeStrings.PendingCopyOperation);

    public static bool HasHttpStatusCode(Exception exception, HttpStatusCode statusCode) => exception is StorageException storageException && storageException.RequestInformation != null && (HttpStatusCode) storageException.RequestInformation.HttpStatusCode == statusCode;

    public static bool HasExtendedError(Exception exception, string errorCode) => exception is StorageException storageException && storageException.RequestInformation != null && storageException.RequestInformation.ExtendedErrorInformation != null && storageException.RequestInformation.ExtendedErrorInformation.ErrorCode == errorCode;

    private static void CopyBlob(
      BlobCopyRequest blobCopyRequest,
      bool skipNotFound,
      ref long totalCopies,
      ref long totalErrors,
      ref long totalSkipped,
      TimeSpan[] retryDelays,
      Action<string> log,
      [CallerMemberName] string caller = null)
    {
      BackoffRetryManager backoffRetryManager = new BackoffRetryManager(retryDelays, (BackoffRetryManager.OnExceptionHandler) (context =>
      {
        log(string.Format("Exception occurred when in CopyBlob. Source blob: {0}, Attempt: {1}, Caller: {2}, Exception: {3}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) (context.CurrentRetryCount + 1), (object) caller, (object) context.Exception));
        if (context.RemainingRetries > 0)
        {
          if (BlobCopyUtil.HasHttpStatusCode(context.Exception, HttpStatusCode.Forbidden))
          {
            log(string.Format("Warning: Access to blob forbidden.  Updating credentials and trying again.  Source blob: {0}, Attempt: {1}, Caller: {2}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) (context.CurrentRetryCount + 1), (object) caller));
            if (blobCopyRequest.SourceContainer.UpdateCredentials())
              return true;
            log(string.Format("Warning: Unable to retrieve new credentials.  Source blob: {0}, Attempt: {1}, Caller: {2}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) (context.CurrentRetryCount + 1), (object) caller));
            return false;
          }
          if (BlobCopyUtil.HasHttpStatusCode(context.Exception, HttpStatusCode.ServiceUnavailable))
          {
            log(string.Format("Warning: 503 Service Unavailable.  Retrying on transient error.  Source blob: {0}, Attempt: {1}, Caller: {2}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) (context.CurrentRetryCount + 1), (object) caller));
            return true;
          }
          if (BlobCopyUtil.HasHttpStatusCode(context.Exception, HttpStatusCode.InternalServerError))
          {
            log(string.Format("Warning: 500 Internal Server Error.  Retrying on transient error.  Source blob: {0}, Attempt: {1}, Caller: {2}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) (context.CurrentRetryCount + 1), (object) caller));
            return true;
          }
        }
        return false;
      }));
      try
      {
        Interlocked.Increment(ref totalCopies);
        backoffRetryManager.Invoke((Action) (() =>
        {
          CopyState copyState = blobCopyRequest.TargetBlob.CopyState;
          if ((copyState != null ? (copyState.Status == CopyStatus.Pending ? 1 : 0) : 0) != 0)
            return;
          blobCopyRequest.TargetBlob.StartCopyFromBlob(blobCopyRequest.SourceUri);
        }));
      }
      catch (Exception ex)
      {
        Interlocked.Increment(ref totalErrors);
        if (BlobCopyUtil.HasPendingCopyOperation(ex))
          log(string.Format("Warning: Encountered exception while queueing a blob copy, the target blob '{0}' already has a pending copy: {1}, Caller: {2}", (object) blobCopyRequest.TargetBlob.Uri.AbsoluteUri, (object) ex, (object) caller));
        else if (BlobCopyUtil.HasHttpStatusCode(ex, HttpStatusCode.Forbidden))
        {
          log(string.Format("Error: Access to blob forbidden and unable to obtain valid credentials. Source blob: {0}, Target blob: {1}, Exception details: {2}, Caller: {3}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) blobCopyRequest.TargetBlob.Uri.AbsoluteUri, (object) ex, (object) caller));
          throw;
        }
        else if (BlobCopyUtil.HasNotFoundError(ex) & skipNotFound)
        {
          log(string.Format("Warning: A blob has been deleted since we listed blobs for copy.  This blob will be skipped. Source blob: {0}, Target blob: {1}, Exception details: {2}, Caller: {3}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) blobCopyRequest.TargetBlob.Uri.AbsoluteUri, (object) ex, (object) caller));
          Interlocked.Increment(ref totalSkipped);
        }
        else
        {
          log(string.Format("Error: Unhandled exception encountered setting up blob copy. Source blob: {0}, Target blob: {1}, Exception details: {2}, Caller: {3}", (object) blobCopyRequest.SourceBlob.Uri.AbsoluteUri, (object) blobCopyRequest.TargetBlob.Uri.AbsoluteUri, (object) ex, (object) caller));
          throw;
        }
      }
    }

    public static void IterateBlobs(
      IVssRequestContext requestContext,
      IBlobCopyContext copyContext,
      IVsoBlobEnumerator sourceEnumerator,
      IVsoBlobEnumerator targetEnumerator,
      string startWithInclusive = null,
      string endWithInclusive = null)
    {
      bool flag1 = sourceEnumerator.MoveNext();
      bool flag2 = targetEnumerator.MoveNext();
      NameOnlyBlockBlobWrapper blockBlobWrapper = startWithInclusive == null ? (NameOnlyBlockBlobWrapper) null : new NameOnlyBlockBlobWrapper(startWithInclusive);
      NameOnlyBlockBlobWrapper targetBlob = endWithInclusive == null ? (NameOnlyBlockBlobWrapper) null : new NameOnlyBlockBlobWrapper(endWithInclusive);
      if (blockBlobWrapper != null && targetBlob != null && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) blockBlobWrapper, (ICloudBlobReadOnlyInfo) targetBlob) > 0)
        throw new ArgumentException("The starting blob is larger than the ending blob.");
      if (blockBlobWrapper != null)
      {
        for (; flag1; flag1 = sourceEnumerator.MoveNext())
        {
          if (requestContext.IsCanceled())
          {
            requestContext.Trace(10008500, TraceLevel.Warning, BlobCopyUtil.s_area, BlobCopyUtil.s_layer, "The request has been cancelled.  Exiting BlobCopyUtil.IterateBlobs");
            return;
          }
          if (copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) sourceEnumerator.Current, (ICloudBlobReadOnlyInfo) blockBlobWrapper) >= 0)
            break;
        }
        for (; flag2; flag2 = targetEnumerator.MoveNext())
        {
          if (requestContext.IsCanceled())
          {
            requestContext.Trace(10008500, TraceLevel.Warning, BlobCopyUtil.s_area, BlobCopyUtil.s_layer, "The request has been cancelled.  Exiting BlobCopyUtil.IterateBlobs");
            return;
          }
          if (copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) targetEnumerator.Current, (ICloudBlobReadOnlyInfo) blockBlobWrapper) >= 0)
            break;
        }
      }
      if (targetBlob != null)
      {
        flag1 = flag1 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) sourceEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
        flag2 = flag2 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) targetEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
      }
      while (flag1 & flag2)
      {
        if (requestContext.IsCanceled())
        {
          requestContext.Trace(10008500, TraceLevel.Warning, BlobCopyUtil.s_area, BlobCopyUtil.s_layer, "The request has been cancelled.  Exiting BlobCopyUtil.IterateBlobs");
          return;
        }
        copyContext.StartIteration(requestContext, sourceEnumerator.CurrentContainer, targetEnumerator.CurrentContainer, sourceEnumerator.Current, targetEnumerator.Current);
        int num = copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) sourceEnumerator.Current, (ICloudBlobReadOnlyInfo) targetEnumerator.Current);
        if (num > 0)
        {
          copyContext.OnTargetOnly(requestContext, targetEnumerator.Current);
          flag2 = targetEnumerator.MoveNext();
          if (targetBlob != null)
            flag2 = flag2 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) targetEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
        }
        else if (num < 0)
        {
          copyContext.OnSourceOnly(requestContext, sourceEnumerator.Current);
          flag1 = sourceEnumerator.MoveNext();
          if (targetBlob != null)
            flag1 = flag1 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) sourceEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
        }
        else if (num == 0)
        {
          copyContext.OnSourceAndTarget(requestContext, sourceEnumerator.Current, targetEnumerator.Current);
          flag1 = sourceEnumerator.MoveNext();
          flag2 = targetEnumerator.MoveNext();
          if (targetBlob != null)
          {
            flag1 = flag1 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) sourceEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
            flag2 = flag2 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) targetEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
          }
        }
      }
      while (flag1)
      {
        copyContext.StartIteration(requestContext, sourceEnumerator.CurrentContainer, targetEnumerator.CurrentContainer, sourceEnumerator.Current, targetEnumerator.Current);
        copyContext.OnSourceOnly(requestContext, sourceEnumerator.Current);
        flag1 = sourceEnumerator.MoveNext();
        if (targetBlob != null)
          flag1 = flag1 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) sourceEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
      }
      while (flag2)
      {
        copyContext.StartIteration(requestContext, sourceEnumerator.CurrentContainer, targetEnumerator.CurrentContainer, sourceEnumerator.Current, targetEnumerator.Current);
        copyContext.OnTargetOnly(requestContext, targetEnumerator.Current);
        flag2 = targetEnumerator.MoveNext();
        if (targetBlob != null)
          flag2 = flag2 && copyContext.Compare(requestContext, (ICloudBlobReadOnlyInfo) targetEnumerator.Current, (ICloudBlobReadOnlyInfo) targetBlob) <= 0;
      }
    }

    public static int CompareContainerAndBlobs(ICloudBlob sourceBlob, ICloudBlob targetBlob) => string.Compare(sourceBlob.Container.Name + BlobCopyUtil.s_nameSeparator + sourceBlob.Name, targetBlob.Container.Name + BlobCopyUtil.s_nameSeparator + targetBlob.Name, StringComparison.Ordinal);

    public static void CopyTargetBlobsToSource(
      IVssRequestContext requestContext,
      ICloudBlobContainerWrapper sourceContainerWrapper,
      ICloudBlobContainerWrapper targetContainerWrapper,
      IEnumerable<TaggedBlobItem> targetTaggedBlobs,
      int maxDegreeOfParallelism,
      CancellationToken cancellationToken)
    {
      ParallelOptions parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = maxDegreeOfParallelism,
        CancellationToken = cancellationToken
      };
      Parallel.ForEach<TaggedBlobItem>(targetTaggedBlobs, parallelOptions, (Action<TaggedBlobItem>) (targetTaggedBlob =>
      {
        ICloudBlobWrapper referenceFromServer = targetContainerWrapper.GetBlobReferenceFromServer(targetTaggedBlob.BlobName);
        sourceContainerWrapper.GetBlockBlobReference(targetTaggedBlob.BlobName).StartCopyFromBlob(targetContainerWrapper.GetUriWithCredentials(referenceFromServer));
      }));
    }

    public static TaggedBlobTransferStats MonitorTargetBlobsToSource(
      IVssRequestContext requestContext,
      ICloudBlobContainerWrapper sourceContainerWrapper,
      ICloudBlobContainerWrapper targetContainerWrapper,
      IEnumerable<TaggedBlobItem> targetTaggedBlobs,
      int maxDegreeOfParallelism,
      CancellationToken cancellationToken)
    {
      TaggedBlobTransferStats result = new TaggedBlobTransferStats();
      result.CompletedAllBlobTransfers = true;
      ParallelOptions parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = maxDegreeOfParallelism,
        CancellationToken = cancellationToken
      };
      int copiedBlobs = 0;
      int pendingBlobs = 0;
      int copyFailures = 0;
      int newQueuedBlobs = 0;
      Parallel.ForEach<TaggedBlobItem>(targetTaggedBlobs, parallelOptions, (Action<TaggedBlobItem>) (targetTaggedBlob =>
      {
        ICloudBlobWrapper referenceFromServer = sourceContainerWrapper.GetBlobReferenceFromServer(targetTaggedBlob.BlobName);
        if (referenceFromServer != null)
        {
          if (referenceFromServer.CopyState == null)
            return;
          switch (referenceFromServer.CopyState.Status)
          {
            case CopyStatus.Pending:
              Interlocked.Add(ref pendingBlobs, 1);
              result.CompletedAllBlobTransfers = false;
              break;
            case CopyStatus.Success:
              Interlocked.Add(ref copiedBlobs, 1);
              break;
            case CopyStatus.Failed:
              Interlocked.Add(ref copyFailures, 1);
              BlobCopyUtil.AttemptCopy(sourceContainerWrapper, targetContainerWrapper, targetTaggedBlob);
              Interlocked.Add(ref pendingBlobs, 1);
              break;
          }
        }
        else
        {
          BlobCopyUtil.AttemptCopy(sourceContainerWrapper, targetContainerWrapper, targetTaggedBlob);
          Interlocked.Add(ref pendingBlobs, 1);
          Interlocked.Add(ref newQueuedBlobs, 1);
        }
      }));
      if (pendingBlobs > 0 || copyFailures > 0)
        result.CompletedAllBlobTransfers = false;
      result.CopiedBlobs = copiedBlobs;
      result.PendingBlobs = pendingBlobs;
      result.CopyFailures = copyFailures;
      result.NewQueuedBlobs = newQueuedBlobs;
      return result;
    }

    public static void AttemptCopy(
      ICloudBlobContainerWrapper sourceContainerWrapper,
      ICloudBlobContainerWrapper targetContainerWrapper,
      TaggedBlobItem targetTaggedBlob)
    {
      ICloudBlobWrapper referenceFromServer = targetContainerWrapper.GetBlobReferenceFromServer(targetTaggedBlob.BlobName);
      sourceContainerWrapper.GetBlockBlobReference(targetTaggedBlob.BlobName).StartCopyFromBlob(targetContainerWrapper.GetUriWithCredentials(referenceFromServer));
    }

    private static void NoLog(string message)
    {
    }

    public static BlobCopyStats MonitorBlobCopy(
      IVssRequestContext deploymentContext,
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlobContainerWrapper targetContainer,
      Action<string> log,
      CancellationToken cancellationToken,
      MigrationTracer tracer,
      bool stopOnPendingCopy = false,
      BlobCopyStats copyStats = null,
      int maxResultsPerBatch = 5000,
      bool isSharded = false,
      string prefix = null)
    {
      tracer = tracer == null ? new MigrationTracer(deploymentContext, (IMigrationData) new BlobMigrationJobData(), string.Empty, nameof (BlobCopyUtil), nameof (MonitorBlobCopy)) : tracer.Enter(nameof (BlobCopyUtil), nameof (MonitorBlobCopy));
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      ArgumentUtility.CheckForNull<ICloudBlobContainerWrapper>(targetContainer, nameof (targetContainer));
      ArgumentUtility.CheckForOutOfRange(maxResultsPerBatch, nameof (maxResultsPerBatch), 0, 5000);
      if (!isSharded)
        ArgumentUtility.CheckForNull<ICloudBlobContainerWrapper>(sourceContainer, nameof (sourceContainer));
      if (log == null)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        log = BlobCopyUtil.\u003C\u003EO.\u003C0\u003E__NoLog ?? (BlobCopyUtil.\u003C\u003EO.\u003C0\u003E__NoLog = new Action<string>(BlobCopyUtil.NoLog));
      }
      if (copyStats == null)
        copyStats = new BlobCopyStats();
      TimeSpan[] retryDelays = BlobCopyUtil.GetRetryDelays(deploymentContext);
      bool testRegistryFlag1 = HostMigrationUtil.GetTestRegistryFlag(deploymentContext, BlobCopyUtil.s_testInvalidateSasTokens, log);
      bool testRegistryFlag2 = HostMigrationUtil.GetTestRegistryFlag(deploymentContext, BlobCopyUtil.s_testInjectMissingBlobs, log);
      if (HostMigrationInjectionUtil.CheckInjection(deploymentContext, FrameworkServerConstants.PendingBlobCopyIndefinitelyInjection))
      {
        copyStats.HasPendingCopies = true;
        return copyStats;
      }
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(deploymentContext, (RegistryQuery) FrameworkServerConstants.BlobCopyLogNumPendingBlobCopiesMaxThreshold, 10);
      IVssRegistryService registryService1 = service;
      IVssRequestContext requestContext1 = deploymentContext;
      RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.BlobCopyRequeueNumPendingBlobCopiesMax;
      ref RegistryQuery local1 = ref registryQuery;
      int num2 = registryService1.GetValue<int>(requestContext1, in local1, 1000);
      IVssRegistryService registryService2 = service;
      IVssRequestContext requestContext2 = deploymentContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.BlobCopyEnableNoCopyStateLogging;
      ref RegistryQuery local2 = ref registryQuery;
      bool flag = registryService2.GetValue<bool>(requestContext2, in local2, true);
      copyStats.HasPendingCopies = false;
      copyStats.ReissuedCopies = false;
      DateTimeOffset? nullable1 = new DateTimeOffset?();
      List<ICloudBlob> cloudBlobList = new List<ICloudBlob>();
      IBlobResultSegmentWrapper resultSegmentWrapper;
      do
      {
        cancellationToken.ThrowIfCancellationRequested();
        BlobCopyStats blobCopyStats = BlobCopyStats.Clone(copyStats);
        try
        {
          resultSegmentWrapper = targetContainer.ListBlobsSegmented(prefix, true, BlobListingDetails.Copy, new int?(maxResultsPerBatch), copyStats.ContinuationToken, (BlobRequestOptions) null, (OperationContext) null);
        }
        catch (StorageException ex)
        {
          RequestResult requestInformation = ex.RequestInformation;
          if ((requestInformation != null ? (requestInformation.HttpStatusCode == 404 ? 1 : 0) : 0) != 0)
          {
            tracer.Info(deploymentContext, 197001, "MonitorBlobCopy: Breaking container monitoring loop due to " + ex.Message);
            break;
          }
          tracer.Error(deploymentContext, 197101, (Func<string>) (() => "MonitorBlobCopy: Failed with storage exception " + ex.Message));
          throw;
        }
        copyStats.ContinuationToken = resultSegmentWrapper.ContinuationToken;
        foreach (IListBlobItem result in resultSegmentWrapper.Results)
        {
          if (result is ICloudBlob blob)
          {
            ++copyStats.TotalBlobs;
            if (blob.CopyState == null)
            {
              if (flag)
                log(string.Format("There is no copy state on this ICloudBlob, uri = {0}", (object) blob.Uri));
              ++copyStats.NoCopyInfo;
            }
            else if (((blob.CopyState.Status == CopyStatus.Aborted ? 1 : (blob.CopyState.Status == CopyStatus.Failed ? 1 : 0)) | (testRegistryFlag2 ? 1 : 0)) != 0)
            {
              if (testRegistryFlag2)
                log(string.Format("Injected a failed blob copy for testing for blob with uri: {0}.  This should only occur in test environments.", (object) blob.Uri));
              else
                log(string.Format("Detected a failed blob copy. State: {0}, StatusDescription: '{1}' for blob with uri: {2}", (object) blob.CopyState.Status, (object) blob.CopyState.StatusDescription, (object) blob.Uri));
              if (string.Equals(blob.CopyState.StatusDescription, "404 BlobNotFound \"Copy failed when reading the source.\"", StringComparison.Ordinal) || string.Equals(blob.CopyState.StatusDescription, "404 NotFound \"Copy failed when reading the source.\"", StringComparison.Ordinal))
              {
                log("The source blob no longer exists, deleting the target blob.");
                blob.Delete();
              }
              else
              {
                if (string.Equals(blob.CopyState.StatusDescription, "412 PreconditionFailed \"Copy failed when reading the source.\"", StringComparison.Ordinal))
                {
                  string str = "The source blob landed in a non-recoverable state: " + blob.CopyState.StatusDescription;
                  log(str);
                }
                copyStats.HasPendingCopies = true;
                BlobCopyUtil.ReattemptBlobCopy(deploymentContext, sourceContainer, log, isSharded, retryDelays, testRegistryFlag1, blob);
              }
            }
            else if (blob.CopyState.Status == CopyStatus.Pending)
            {
              if (cloudBlobList.Count < num2)
                cloudBlobList.Add(blob);
              if (copyStats.BlobsPendingCopy.Count < num1)
                copyStats.BlobsPendingCopy.Add(blob.Name);
              if (stopOnPendingCopy)
              {
                blobCopyStats.HasPendingCopies = true;
                return blobCopyStats;
              }
              copyStats.HasPendingCopies = true;
              ++copyStats.PendingCopies;
            }
            else
            {
              if (nullable1.HasValue)
              {
                DateTimeOffset? completionTime = blob.CopyState.CompletionTime;
                DateTimeOffset? nullable2 = nullable1;
                if ((completionTime.HasValue & nullable2.HasValue ? (completionTime.GetValueOrDefault() > nullable2.GetValueOrDefault() ? 1 : 0) : 0) == 0)
                  goto label_41;
              }
              nullable1 = blob.CopyState.CompletionTime;
label_41:
              ++copyStats.BlobsCompleted;
              if (((copyStats.BlobsCompleted | 131072) & 131071) == 0)
                log(string.Format("Monitoring blob copies update: {0} blobs verified in container: {1}.", (object) copyStats.BlobsCompleted, (object) targetContainer.Name));
            }
          }
        }
        tracer.Verbose(deploymentContext, 197101, "MonitorBlobCopy: total = {0}, completed = {1}, pending = {2}", (object) copyStats.TotalBlobs, (object) copyStats.BlobsCompleted, (object) copyStats.PendingCopies);
      }
      while (resultSegmentWrapper.ContinuationToken != null);
      int num3 = service.GetValue<int>(deploymentContext, (RegistryQuery) FrameworkServerConstants.RequeueNoBlobCompletionAfterMaxHoursThreshold, 12);
      if (nullable1.HasValue && DateTime.UtcNow.Subtract(nullable1.Value.DateTime.ToUniversalTime()) > TimeSpan.FromHours((double) num3))
      {
        log(string.Format("Threshold of {0} hour(s) for no blob completion activity has been hit, cancelling existing copy requests and reissueing them for {1} blob(s) pending", (object) num3, (object) cloudBlobList.Count));
        foreach (ICloudBlob blob in cloudBlobList)
        {
          blob.AbortCopy(blob.CopyState.CopyId);
          BlobCopyUtil.ReattemptBlobCopy(deploymentContext, sourceContainer, log, isSharded, retryDelays, false, blob);
          copyStats.ReissuedCopies = true;
        }
        log(string.Format("Reissued copy requests for {0} blob(s) that are in the pending state", (object) cloudBlobList.Count));
      }
      log(string.Format("{0} of {1} blobs copied.", (object) copyStats.BlobsCompleted, (object) (copyStats.TotalBlobs - copyStats.NoCopyInfo)));
      return copyStats;
    }

    private static void ReattemptBlobCopy(
      IVssRequestContext deploymentContext,
      ICloudBlobContainerWrapper sourceContainer,
      Action<string> log,
      bool isSharded,
      TimeSpan[] retryDelays,
      bool invalidateSasTokens,
      ICloudBlob blob)
    {
      if (!isSharded)
      {
        ICloudBlobWrapper blobOrDeleteTarget = BlobCopyUtil.GetReferenceToSourceBlobOrDeleteTarget(deploymentContext, sourceContainer, blob, retryDelays, log, nameof (ReattemptBlobCopy));
        if (blobOrDeleteTarget != null)
        {
          ICloudBlobWrapper targetBlob = (ICloudBlobWrapper) new CloudBlobWrapper(blob);
          BlobCopyRequest blobCopyRequest = new BlobCopyRequest(sourceContainer, blobOrDeleteTarget, targetBlob);
          log(string.Format("Retrying copy of {0}", (object) blobOrDeleteTarget.Uri));
          long totalCopies = 0;
          long totalErrors = 0;
          long totalSkipped = 0;
          if (invalidateSasTokens)
            HostMigrationUtil.InvalidateCredentials(blobCopyRequest.SourceContainer, log);
          BlobCopyUtil.CopyBlob(blobCopyRequest, true, ref totalCopies, ref totalErrors, ref totalSkipped, retryDelays, log, nameof (ReattemptBlobCopy));
        }
        else
          log(string.Format("Will not retry copy of {0}, blob not found on source", (object) blob.Uri));
      }
      else
        new BackoffRetryManager(retryDelays).Invoke((Action) (() =>
        {
          try
          {
            if (blob.CopyState.Status == CopyStatus.Pending)
              return;
            ((CloudBlob) blob).StartCopy(blob.CopyState.Source);
          }
          catch (StorageException ex)
          {
            if (ex.RequestInformation != null && ex.RequestInformation.ExtendedErrorInformation != null)
            {
              StorageExtendedErrorInformation errorInformation = ex.RequestInformation.ExtendedErrorInformation;
              log("Storage exception during re-copy attempt. Error Code: " + errorInformation.ErrorCode + " Error message: " + errorInformation.ErrorMessage);
              if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.PendingCopyOperation)
              {
                log(string.Format("Blob with uri: {0} already has a pending copy request. Likely a transient error originally reported copy request failed.", (object) blob.Uri));
                return;
              }
              if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.CannotVerifyCopySource)
              {
                log(string.Format("Deleting target blob with uri: {0}, the blob no longer exists on the source.", (object) blob.Uri));
                blob.Delete();
                return;
              }
            }
            throw;
          }
        }));
    }

    private static ICloudBlobWrapper GetReferenceToSourceBlobOrDeleteTarget(
      IVssRequestContext deploymentContext,
      ICloudBlobContainerWrapper sourceContainer,
      ICloudBlob blob,
      TimeSpan[] retryDelays,
      Action<string> log,
      [CallerMemberName] string caller = null)
    {
      bool invalidateSasTokens = HostMigrationUtil.GetTestRegistryFlag(deploymentContext, BlobCopyUtil.s_testInvalidateSasTokens, log);
      bool injectMissingBlobs = HostMigrationUtil.GetTestRegistryFlag(deploymentContext, BlobCopyUtil.s_testInjectMissingBlobs, log);
      BackoffRetryManager backoffRetryManager = new BackoffRetryManager(retryDelays, (BackoffRetryManager.OnExceptionHandler) (context =>
      {
        if (context.RemainingRetries > 0)
        {
          if (BlobCopyUtil.HasHttpStatusCode(context.Exception, HttpStatusCode.Forbidden))
          {
            log(string.Format("Warning: Access to blob forbidden.  Updating credentials and trying again.  Source blob: {0}, Attempt: {1}, Caller: {2}", (object) blob.CopyState.Source, (object) (context.CurrentRetryCount + 1), (object) caller));
            if (sourceContainer.UpdateCredentials())
              return true;
            log(string.Format("Warning: Unable to retrieve new credentials.  Source blob: {0}, Attempt: {1}, Caller: {2}", (object) blob.CopyState.Source, (object) (context.CurrentRetryCount + 1), (object) caller));
            return false;
          }
          if (BlobCopyUtil.HasHttpStatusCode(context.Exception, HttpStatusCode.ServiceUnavailable))
          {
            log(string.Format("Warning: 503 Service Unavailable.  Retrying on transient error.  Source blob: {0}, Attempt: {1}, Caller: {2}", (object) blob.CopyState.Source, (object) (context.CurrentRetryCount + 1), (object) caller));
            return true;
          }
        }
        return false;
      }));
      ICloudBlobWrapper sourceBlob = (ICloudBlobWrapper) null;
      Action action = (Action) (() =>
      {
        try
        {
          if (invalidateSasTokens)
            HostMigrationUtil.InvalidateCredentials(sourceContainer, log);
          if (injectMissingBlobs)
            throw new StorageException(new RequestResult()
            {
              HttpStatusCode = 404
            }, "Test Not Found", new Exception("Injected a test failure"));
          sourceBlob = sourceContainer.GetBlobReferenceFromServer(blob.Name);
        }
        catch (StorageException ex)
        {
          if (BlobCopyUtil.HasNotFoundError((Exception) ex))
          {
            log(string.Format("Deleting target blob with uri: {0}, the blob no longer exists on the source.", (object) blob.Uri));
            blob.DeleteIfExists();
          }
          else
            throw;
        }
      });
      backoffRetryManager.Invoke(action);
      return sourceBlob;
    }

    private static TimeSpan[] GetRetryDelays(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return BackoffRetryManager.ExponentialDelay(service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.BlobCopyRetryAttempts, 10), TimeSpan.FromSeconds((double) service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.BlobCopyRetryMaxDelayInSeconds, 120)));
    }

    private static bool IsBlobContinuationTokenStorageEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled(BlobCopyUtil.s_disableBlobCopyContinuationTokenStorage);
  }
}
