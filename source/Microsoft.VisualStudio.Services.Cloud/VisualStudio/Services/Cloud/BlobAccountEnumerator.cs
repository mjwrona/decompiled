// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobAccountEnumerator
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class BlobAccountEnumerator : 
    IVsoBlobEnumerator,
    IEnumerator<ICloudBlobWrapper>,
    IDisposable,
    IEnumerator
  {
    private IEnumerator<CloudBlobContainer> m_containerEnumerator;
    private IEnumerable<CloudBlobContainer> m_containerList;
    private BlobContinuationToken m_containerContinuationToken;
    private IEnumerator<IListBlobItem> m_blobEnumerator;
    private IEnumerable<IListBlobItem> m_blobList;
    private BlobContinuationToken m_blobContinuationToken;
    private ICloudStorageAccountWrapper m_account;
    private ICloudBlobClientWrapper m_blobClient;
    private int m_maxContainerResults;
    private int m_maxBlobResults;
    private BlobListingDetails m_blobListingDetails;
    private RetryManager m_retryManager;
    private string m_containerPrefix;
    private string m_blobPrefix;
    private BlobContinuationToken m_currentContainerContinuationToken;
    private BlobContinuationToken m_currentBlobContinuationToken;
    internal static int s_retryCount = 5;
    public static TimeSpan s_retryDelay = TimeSpan.FromMilliseconds(500.0);
    private bool disposedValue;

    public BlobAccountEnumerator(
      ICloudStorageAccountWrapper account,
      BlobListingDetails blobListingDetails,
      int containerBatchSize,
      int blobBatchSize,
      string containerPrefix = null,
      string blobPrefix = null,
      BlobContinuationToken containerContinuationToken = null,
      BlobContinuationToken blobContinuationToken = null)
    {
      this.m_containerPrefix = containerPrefix;
      this.m_blobPrefix = blobPrefix;
      this.m_account = account;
      this.m_blobClient = account.CreateCloudBlobClient();
      this.m_maxContainerResults = containerBatchSize;
      this.m_maxBlobResults = blobBatchSize;
      this.m_blobListingDetails = blobListingDetails;
      this.m_containerContinuationToken = containerContinuationToken;
      this.m_blobContinuationToken = blobContinuationToken;
      this.m_retryManager = new RetryManager(BlobAccountEnumerator.s_retryCount, BlobAccountEnumerator.s_retryDelay, new Action<Exception>(this.OnRetryException));
      this.FetchContainers();
      this.MoveNextContainer();
      this.FetchBlobs();
    }

    public bool MoveNext()
    {
      while (this.m_blobEnumerator != null && !this.m_blobEnumerator.MoveNext() && !this.m_account.IsValidBlobType(this.m_blobEnumerator.Current))
      {
        if (this.m_blobContinuationToken == null && !this.MoveNextContainer())
        {
          this.m_blobEnumerator.Dispose();
          this.m_blobEnumerator = (IEnumerator<IListBlobItem>) null;
          break;
        }
        this.FetchBlobs();
      }
      if (this.m_blobEnumerator != null && this.m_blobEnumerator.Current != null)
      {
        this.Current = this.m_account.CreateBlobWrapper(this.m_blobEnumerator.Current);
        return true;
      }
      this.Current = (ICloudBlobWrapper) null;
      return false;
    }

    public void Reset() => throw new NotSupportedException();

    public ICloudBlobWrapper Current { get; private set; }

    public ICloudBlobContainerWrapper CurrentContainer { get; private set; }

    object IEnumerator.Current => (object) this.Current;

    public ICloudBlobClientWrapper CloudBlobClient => this.m_blobClient;

    internal bool MoveNextContainer()
    {
      while (!this.m_containerEnumerator.MoveNext())
      {
        if (this.m_containerContinuationToken == null)
        {
          this.CurrentContainer = (ICloudBlobContainerWrapper) null;
          return false;
        }
        this.FetchContainers();
      }
      this.CurrentContainer = this.m_account.CreateContainerWrapper(this.m_containerEnumerator.Current);
      return true;
    }

    internal BlobContinuationToken CurrentContainerContinuationToken => this.m_currentContainerContinuationToken;

    internal BlobContinuationToken CurrentBlobContinuationToken => this.m_currentBlobContinuationToken;

    private void FetchContainers()
    {
      if (this.m_containerEnumerator != null)
      {
        this.m_containerEnumerator.Dispose();
        this.m_containerEnumerator = (IEnumerator<CloudBlobContainer>) null;
      }
      this.m_currentContainerContinuationToken = this.m_containerContinuationToken;
      IContainerResultSegmentWrapper result = (IContainerResultSegmentWrapper) null;
      this.m_retryManager.Invoke((Action) (() => result = this.m_blobClient.ListContainersSegmented(this.m_containerPrefix, ContainerListingDetails.Metadata, new int?(this.m_maxContainerResults), this.m_containerContinuationToken)));
      this.m_containerContinuationToken = result.ContinuationToken;
      this.m_containerList = result.Results;
      if (this.m_containerList == null)
        return;
      this.m_containerEnumerator = this.m_containerList.GetEnumerator();
    }

    internal bool FetchBlobs()
    {
      if (this.m_blobEnumerator != null)
      {
        this.m_blobEnumerator.Dispose();
        this.m_blobEnumerator = (IEnumerator<IListBlobItem>) null;
      }
      if (this.CurrentContainer == null)
        return false;
      this.m_currentBlobContinuationToken = this.m_blobContinuationToken;
      IBlobResultSegmentWrapper results = (IBlobResultSegmentWrapper) null;
      this.m_retryManager.Invoke((Action) (() => results = this.CurrentContainer.ListBlobsSegmented(this.m_blobPrefix, true, this.m_blobListingDetails, new int?(this.m_maxBlobResults), this.m_blobContinuationToken, (BlobRequestOptions) null, (OperationContext) null)));
      this.m_blobContinuationToken = results.ContinuationToken;
      this.m_blobList = results.Results;
      this.m_blobEnumerator = this.m_blobList.GetEnumerator();
      return true;
    }

    private void OnRetryException(Exception e)
    {
      if (e is StorageException storageException && storageException.RequestInformation != null && storageException.RequestInformation.HttpStatusCode == 403)
        throw storageException;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposedValue)
        return;
      if (disposing)
      {
        if (this.m_blobEnumerator != null)
        {
          this.m_blobEnumerator.Dispose();
          this.m_blobEnumerator = (IEnumerator<IListBlobItem>) null;
        }
        if (this.m_containerEnumerator != null)
        {
          this.m_containerEnumerator.Dispose();
          this.m_containerEnumerator = (IEnumerator<CloudBlobContainer>) null;
        }
        this.m_containerList = (IEnumerable<CloudBlobContainer>) null;
        this.m_blobList = (IEnumerable<IListBlobItem>) null;
      }
      this.disposedValue = true;
    }

    public void Dispose() => this.Dispose(true);
  }
}
