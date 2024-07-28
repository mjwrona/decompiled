// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.ContainerStorageAccessStatistics
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class ContainerStorageAccessStatistics : IContainerStorageAccessStatistics
  {
    private long listBlobsSegmentedAsyncCount;
    private long getBlockBlobReferenceCount;
    private long createIfNotExistsAsyncCount;
    private long deleteIfExistsAsyncCount;
    private long getSharedAccessSignatureCount;
    private long getPermissionsAsyncCount;
    private long setPermissionsAsyncCount;

    public long ListBlobsSegmentedAsyncCount => this.listBlobsSegmentedAsyncCount;

    public long GetBlockBlobReferenceCount => this.getBlockBlobReferenceCount;

    public long CreateIfNotExistsAsyncCount => this.createIfNotExistsAsyncCount;

    public long DeleteIfExistsAsyncCount => this.deleteIfExistsAsyncCount;

    public long GetSharedAccessSignatureCount => this.getSharedAccessSignatureCount;

    public long GetPermissionsAsyncCount => this.getPermissionsAsyncCount;

    public long SetPermissionsAsyncCount => this.setPermissionsAsyncCount;

    public BlobStorageAccessStatistics BlobStatistics { get; }

    public ContainerStorageAccessStatistics(
      long listBlobsSegmentedAsyncCount = 0,
      long getBlockBlobReferenceCount = 0,
      long createIfNotExistsAsyncCount = 0,
      long deleteIfExistsAsyncCount = 0,
      long getSharedAccessSignatureCount = 0,
      long getPermissionsAsyncCount = 0,
      long setPermissionsAsyncCount = 0,
      IBlobStorageAccessStatistics blobStatistics = null)
    {
      this.listBlobsSegmentedAsyncCount = listBlobsSegmentedAsyncCount;
      this.getBlockBlobReferenceCount = getBlockBlobReferenceCount;
      this.createIfNotExistsAsyncCount = createIfNotExistsAsyncCount;
      this.deleteIfExistsAsyncCount = deleteIfExistsAsyncCount;
      this.getSharedAccessSignatureCount = getSharedAccessSignatureCount;
      this.getPermissionsAsyncCount = getPermissionsAsyncCount;
      this.setPermissionsAsyncCount = setPermissionsAsyncCount;
      this.BlobStatistics = new BlobStorageAccessStatistics(blobStatistics);
    }

    public IContainerStorageAccessStatistics Reset() => (IContainerStorageAccessStatistics) new ContainerStorageAccessStatistics(Interlocked.Exchange(ref this.listBlobsSegmentedAsyncCount, 0L), Interlocked.Exchange(ref this.getBlockBlobReferenceCount, 0L), Interlocked.Exchange(ref this.createIfNotExistsAsyncCount, 0L), Interlocked.Exchange(ref this.deleteIfExistsAsyncCount, 0L), Interlocked.Exchange(ref this.getSharedAccessSignatureCount, 0L), Interlocked.Exchange(ref this.getPermissionsAsyncCount, 0L), Interlocked.Exchange(ref this.setPermissionsAsyncCount, 0L), this.BlobStatistics.Reset());

    public void Increment(ContainerStorageAccessStatistics.Methods property)
    {
      switch (property)
      {
        case ContainerStorageAccessStatistics.Methods.ListBlobsSegmentedAsync:
          Interlocked.Increment(ref this.listBlobsSegmentedAsyncCount);
          break;
        case ContainerStorageAccessStatistics.Methods.GetBlockBlobReference:
          Interlocked.Increment(ref this.getBlockBlobReferenceCount);
          break;
        case ContainerStorageAccessStatistics.Methods.CreateIfNotExistsAsync:
          Interlocked.Increment(ref this.createIfNotExistsAsyncCount);
          break;
        case ContainerStorageAccessStatistics.Methods.DeleteIfExistsAsync:
          Interlocked.Increment(ref this.deleteIfExistsAsyncCount);
          break;
        case ContainerStorageAccessStatistics.Methods.GetSharedAccessSignature:
          Interlocked.Increment(ref this.getSharedAccessSignatureCount);
          break;
        case ContainerStorageAccessStatistics.Methods.GetPermissionsAsync:
          Interlocked.Increment(ref this.getPermissionsAsyncCount);
          break;
        case ContainerStorageAccessStatistics.Methods.SetPermissionsAsync:
          Interlocked.Increment(ref this.setPermissionsAsyncCount);
          break;
      }
    }

    public long TotalTransactions() => this.BlobStatistics.TotalTransactions() + this.listBlobsSegmentedAsyncCount + this.getBlockBlobReferenceCount + this.createIfNotExistsAsyncCount + this.deleteIfExistsAsyncCount + this.getSharedAccessSignatureCount + this.getPermissionsAsyncCount + this.setPermissionsAsyncCount;

    public enum Methods
    {
      ListBlobsSegmentedAsync,
      GetBlockBlobReference,
      CreateIfNotExistsAsync,
      DeleteIfExistsAsync,
      GetSharedAccessSignature,
      GetPermissionsAsync,
      SetPermissionsAsync,
    }
  }
}
