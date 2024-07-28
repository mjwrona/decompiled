// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobContainerEnumerator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class BlobContainerEnumerator : 
    IVsoBlobEnumerator,
    IEnumerator<ICloudBlobWrapper>,
    IDisposable,
    IEnumerator
  {
    private readonly IVssRequestContext m_requestContext;
    private readonly ICloudStorageAccountWrapper m_account;
    private readonly BlobContainerEnumerationContinuationSettings m_continuationSettings;
    private readonly IBlobContinuationTokenStorageService m_continuationTokenStorageService;
    private readonly Action<string> m_log;
    private IEnumerator<IListBlobItem> m_blobEnumerator;
    private IEnumerable<IListBlobItem> m_blobList;
    private BlobContinuationToken m_blobContinuationToken;
    private BlobContinuationToken m_lastContinuationToken;
    private long m_lastStoredContinuationTokenCount;
    private long m_continuationTokenStorageFrequency;
    private long m_blobsEnumerated;
    private BlobListingDetails m_blobListingDetails;
    private readonly int m_maxBlobResults;
    private readonly string m_blobPrefix;
    private readonly BackoffRetryManager m_retryManager;
    private static readonly int s_defaultRetryCount = 5;
    private static readonly TimeSpan s_defaultRetryDelay = TimeSpan.FromSeconds(5.0);
    private bool disposedValue;

    public BlobContainerEnumerator(
      IVssRequestContext requestContext,
      ICloudStorageAccountWrapper account,
      ICloudBlobContainerWrapper container,
      BlobListingDetails blobListingDetails,
      int blobBatchSize,
      string blobPrefix = null,
      TimeSpan[] retryDelays = null,
      BlobContainerEnumerationContinuationSettings continuationSettings = null,
      Action<string> log = null)
    {
      this.m_requestContext = requestContext;
      this.m_account = account;
      this.CurrentContainer = container;
      this.m_blobListingDetails = blobListingDetails;
      this.m_maxBlobResults = blobBatchSize;
      this.m_blobPrefix = blobPrefix;
      this.m_blobsEnumerated = 0L;
      this.m_continuationSettings = continuationSettings;
      this.m_log = log ?? (Action<string>) (_ => { });
      if (this.m_continuationSettings != null)
      {
        this.m_continuationTokenStorageService = requestContext.GetService<IBlobContinuationTokenStorageService>();
        this.m_continuationTokenStorageFrequency = this.m_continuationTokenStorageService.GetStorageFrequency(this.m_requestContext);
        this.m_blobContinuationToken = this.m_continuationTokenStorageService.GetContinuationToken(this.m_requestContext, this.m_continuationSettings.OperationName, container, this.m_log);
        if (this.m_blobContinuationToken != null)
          this.m_log(string.Format("Resuming blob container enumeration from stored continuation token.  Storage account: {0}, Container: {1}", (object) container.StorageAccountName, (object) container.Name));
      }
      if (retryDelays == null)
        retryDelays = BackoffRetryManager.ConstantDelay(BlobContainerEnumerator.s_defaultRetryCount, BlobContainerEnumerator.s_defaultRetryDelay);
      this.m_retryManager = new BackoffRetryManager(retryDelays, (BackoffRetryManager.OnExceptionHandler) (context => !BlobCopyUtil.HasHttpStatusCode(context.Exception, HttpStatusCode.Forbidden) || context.RemainingRetries <= 0 || container.UpdateCredentials()));
      this.FetchBlobs();
    }

    public ICloudBlobContainerWrapper CurrentContainer { get; private set; }

    public ICloudBlobWrapper Current { get; private set; }

    object IEnumerator.Current => (object) this.Current;

    public bool MoveNext()
    {
      while (this.m_blobEnumerator != null && !this.m_blobEnumerator.MoveNext() && !this.m_account.IsValidBlobType(this.m_blobEnumerator.Current))
      {
        if (this.m_blobContinuationToken == null)
        {
          if (this.m_continuationSettings != null)
            this.m_continuationTokenStorageService.ClearContinuationToken(this.m_requestContext, this.m_continuationSettings.OperationName, this.CurrentContainer, this.m_log);
          return false;
        }
        this.FetchBlobs();
      }
      if (this.m_blobEnumerator != null && this.m_blobEnumerator.Current != null)
      {
        this.Current = this.m_account.CreateBlobWrapper(this.m_blobEnumerator.Current);
        ++this.m_blobsEnumerated;
        if (this.m_continuationSettings != null && this.m_lastContinuationToken != null && this.m_lastStoredContinuationTokenCount + this.m_continuationTokenStorageFrequency < this.m_blobsEnumerated)
        {
          this.m_continuationTokenStorageService.SetContinuationToken(this.m_requestContext, this.m_continuationSettings.OperationName, this.CurrentContainer, this.m_lastContinuationToken, this.m_log);
          this.m_continuationTokenStorageFrequency = this.m_continuationTokenStorageService.GetStorageFrequency(this.m_requestContext);
          this.m_lastStoredContinuationTokenCount = this.m_blobsEnumerated;
        }
        return true;
      }
      this.Current = (ICloudBlobWrapper) null;
      if (this.m_continuationSettings != null)
        this.m_continuationTokenStorageService.ClearContinuationToken(this.m_requestContext, this.m_continuationSettings.OperationName, this.CurrentContainer, this.m_log);
      return false;
    }

    public void Reset() => throw new NotSupportedException();

    private bool FetchBlobs()
    {
      if (this.m_blobEnumerator != null)
      {
        this.m_blobEnumerator.Dispose();
        this.m_blobEnumerator = (IEnumerator<IListBlobItem>) null;
      }
      this.m_lastContinuationToken = this.m_blobContinuationToken;
      if (this.CurrentContainer == null)
        return false;
      IBlobResultSegmentWrapper results = (IBlobResultSegmentWrapper) null;
      this.m_retryManager.Invoke((Action) (() => results = this.CurrentContainer.ListBlobsSegmented(this.m_blobPrefix, true, this.m_blobListingDetails, new int?(this.m_maxBlobResults), this.m_blobContinuationToken, (BlobRequestOptions) null, (OperationContext) null)));
      this.m_blobContinuationToken = results.ContinuationToken;
      this.m_blobList = results.Results;
      this.m_blobEnumerator = this.m_blobList.GetEnumerator();
      return true;
    }

    public long BlobsEnumerated => Interlocked.Read(ref this.m_blobsEnumerated);

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing && this.m_blobEnumerator != null)
        this.m_blobEnumerator.Dispose();
      this.disposedValue = true;
    }

    public void Dispose() => this.Dispose(true);
  }
}
