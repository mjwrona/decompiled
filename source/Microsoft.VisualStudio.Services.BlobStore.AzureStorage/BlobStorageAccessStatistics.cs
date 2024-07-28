// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.BlobStorageAccessStatistics
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class BlobStorageAccessStatistics : IBlobStorageAccessStatistics
  {
    private long getSharedAccessSignatureCount;
    private long putBlockAsyncCount;
    private long putBlockListAsyncCount;
    private long deleteIfExistsAsyncCount;
    private long undeleteAsyncCount;
    private long downloadBlockListAsyncCount;
    private long uploadFromStreamAsyncCount;
    private long openReadNeedsRetryAsyncCount;
    private long downloadToStreamNeedsRetryAsyncCount;
    private long existsAsyncCount;
    private long uploadFromByteArrayAsyncCount;
    private long fetchAttributesAsyncCount;
    private long setMetadataAsyncCount;

    public BlobStorageAccessStatistics(IBlobStorageAccessStatistics statistics = null)
    {
      if (statistics == null)
        return;
      this.Initialize(statistics.GetSharedAccessSignatureCount, statistics.PutBlockAsyncCount, statistics.PutBlockListAsyncCount, statistics.DeleteIfExistsAsyncCount, statistics.UndeleteAsyncCount, statistics.DownloadBlockListAsyncCount, statistics.UploadFromStreamAsyncCount, statistics.OpenReadNeedsRetryAsyncCount, statistics.DownloadToStreamNeedsRetryAsyncCount, statistics.ExistsAsyncCount, statistics.UploadFromByteArrayAsyncCount, statistics.FetchAttributesAsyncCount, statistics.SetMetadataAsyncCount);
    }

    public BlobStorageAccessStatistics(
      long getSharedAccessSignatureCount,
      long putBlockAsyncCount,
      long putBlockListAsyncCount,
      long deleteIfExistsAsyncCount,
      long undeleteAsyncCount,
      long downloadBlockListAsyncCount,
      long uploadFromStreamAsyncCount,
      long openReadNeedsRetryAsyncCount,
      long downloadToStreamNeedsRetryAsyncCount,
      long existsAsyncCount,
      long uploadFromByteArrayAsyncCount,
      long fetchAttributesAsyncCount,
      long setMetadataAsyncCount)
    {
      this.Initialize(getSharedAccessSignatureCount, putBlockAsyncCount, putBlockListAsyncCount, deleteIfExistsAsyncCount, undeleteAsyncCount, downloadBlockListAsyncCount, uploadFromStreamAsyncCount, openReadNeedsRetryAsyncCount, downloadToStreamNeedsRetryAsyncCount, existsAsyncCount, uploadFromByteArrayAsyncCount, fetchAttributesAsyncCount, setMetadataAsyncCount);
    }

    private void Initialize(
      long getSharedAccessSignatureCount,
      long putBlockAsyncCount,
      long putBlockListAsyncCount,
      long deleteIfExistsAsyncCount,
      long undeleteAsyncCount,
      long downloadBlockListAsyncCount,
      long uploadFromStreamAsyncCount,
      long openReadNeedsRetryAsyncCount,
      long downloadToStreamNeedsRetryAsyncCount,
      long existsAsyncCount,
      long uploadFromByteArrayAsyncCount,
      long fetchAttributesAsyncCount,
      long setMetadataAsyncCount)
    {
      this.getSharedAccessSignatureCount = getSharedAccessSignatureCount;
      this.putBlockAsyncCount = putBlockAsyncCount;
      this.putBlockListAsyncCount = putBlockListAsyncCount;
      this.deleteIfExistsAsyncCount = deleteIfExistsAsyncCount;
      this.undeleteAsyncCount = undeleteAsyncCount;
      this.downloadBlockListAsyncCount = downloadBlockListAsyncCount;
      this.uploadFromStreamAsyncCount = uploadFromStreamAsyncCount;
      this.openReadNeedsRetryAsyncCount = openReadNeedsRetryAsyncCount;
      this.downloadToStreamNeedsRetryAsyncCount = downloadToStreamNeedsRetryAsyncCount;
      this.existsAsyncCount = existsAsyncCount;
      this.uploadFromByteArrayAsyncCount = uploadFromByteArrayAsyncCount;
      this.fetchAttributesAsyncCount = fetchAttributesAsyncCount;
      this.setMetadataAsyncCount = setMetadataAsyncCount;
    }

    public long GetSharedAccessSignatureCount => this.getSharedAccessSignatureCount;

    public long PutBlockAsyncCount => this.putBlockAsyncCount;

    public long PutBlockListAsyncCount => this.putBlockListAsyncCount;

    public long DeleteIfExistsAsyncCount => this.deleteIfExistsAsyncCount;

    public long UndeleteAsyncCount => this.undeleteAsyncCount;

    public long DownloadBlockListAsyncCount => this.downloadBlockListAsyncCount;

    public long UploadFromStreamAsyncCount => this.uploadFromStreamAsyncCount;

    public long OpenReadNeedsRetryAsyncCount => this.openReadNeedsRetryAsyncCount;

    public long DownloadToStreamNeedsRetryAsyncCount => this.downloadToStreamNeedsRetryAsyncCount;

    public long ExistsAsyncCount => this.existsAsyncCount;

    public long UploadFromByteArrayAsyncCount => this.uploadFromByteArrayAsyncCount;

    public long FetchAttributesAsyncCount => this.fetchAttributesAsyncCount;

    public long SetMetadataAsyncCount => this.setMetadataAsyncCount;

    public void Increment(BlobStorageAccessStatistics.Methods property)
    {
      switch (property)
      {
        case BlobStorageAccessStatistics.Methods.GetSharedAccessSignature:
          Interlocked.Increment(ref this.getSharedAccessSignatureCount);
          break;
        case BlobStorageAccessStatistics.Methods.PutBlockAsync:
          Interlocked.Increment(ref this.putBlockAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.PutBlockListAsync:
          Interlocked.Increment(ref this.putBlockListAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.DeleteIfExistsAsync:
          Interlocked.Increment(ref this.deleteIfExistsAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.UndeleteAsync:
          Interlocked.Increment(ref this.undeleteAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.DownloadBlockListAsync:
          Interlocked.Increment(ref this.downloadBlockListAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.UploadFromStreamAsync:
          Interlocked.Increment(ref this.uploadFromStreamAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.OpenReadNeedsRetryAsync:
          Interlocked.Increment(ref this.openReadNeedsRetryAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.DownloadToStreamNeedsRetryAsync:
          Interlocked.Increment(ref this.downloadToStreamNeedsRetryAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.ExistsAsync:
          Interlocked.Increment(ref this.existsAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.UploadFromByteArrayAsync:
          Interlocked.Increment(ref this.uploadFromByteArrayAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.FetchAttributesAsync:
          Interlocked.Increment(ref this.fetchAttributesAsyncCount);
          break;
        case BlobStorageAccessStatistics.Methods.SetMetadataAsync:
          Interlocked.Increment(ref this.setMetadataAsyncCount);
          break;
      }
    }

    public IBlobStorageAccessStatistics Reset() => (IBlobStorageAccessStatistics) new BlobStorageAccessStatistics(Interlocked.Exchange(ref this.getSharedAccessSignatureCount, 0L), Interlocked.Exchange(ref this.putBlockAsyncCount, 0L), Interlocked.Exchange(ref this.putBlockListAsyncCount, 0L), Interlocked.Exchange(ref this.deleteIfExistsAsyncCount, 0L), Interlocked.Exchange(ref this.undeleteAsyncCount, 0L), Interlocked.Exchange(ref this.downloadBlockListAsyncCount, 0L), Interlocked.Exchange(ref this.uploadFromStreamAsyncCount, 0L), Interlocked.Exchange(ref this.openReadNeedsRetryAsyncCount, 0L), Interlocked.Exchange(ref this.downloadToStreamNeedsRetryAsyncCount, 0L), Interlocked.Exchange(ref this.existsAsyncCount, 0L), Interlocked.Exchange(ref this.uploadFromByteArrayAsyncCount, 0L), Interlocked.Exchange(ref this.fetchAttributesAsyncCount, 0L), Interlocked.Exchange(ref this.setMetadataAsyncCount, 0L));

    public long TotalTransactions() => this.getSharedAccessSignatureCount + this.putBlockAsyncCount + this.putBlockListAsyncCount + this.deleteIfExistsAsyncCount + this.undeleteAsyncCount + this.downloadBlockListAsyncCount + this.uploadFromStreamAsyncCount + this.openReadNeedsRetryAsyncCount + this.downloadToStreamNeedsRetryAsyncCount + this.existsAsyncCount + this.uploadFromByteArrayAsyncCount + this.fetchAttributesAsyncCount + this.setMetadataAsyncCount;

    public enum Methods
    {
      GetSharedAccessSignature,
      PutBlockAsync,
      PutBlockListAsync,
      DeleteIfExistsAsync,
      UndeleteAsync,
      DownloadBlockListAsync,
      UploadFromStreamAsync,
      OpenReadNeedsRetryAsync,
      DownloadToStreamNeedsRetryAsync,
      ExistsAsync,
      UploadFromByteArrayAsync,
      FetchAttributesAsync,
      SetMetadataAsync,
    }
  }
}
