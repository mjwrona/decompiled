// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.IBlobStorageAccessStatistics
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public interface IBlobStorageAccessStatistics
  {
    long GetSharedAccessSignatureCount { get; }

    long PutBlockAsyncCount { get; }

    long PutBlockListAsyncCount { get; }

    long DeleteIfExistsAsyncCount { get; }

    long UndeleteAsyncCount { get; }

    long DownloadBlockListAsyncCount { get; }

    long UploadFromStreamAsyncCount { get; }

    long OpenReadNeedsRetryAsyncCount { get; }

    long DownloadToStreamNeedsRetryAsyncCount { get; }

    long ExistsAsyncCount { get; }

    long UploadFromByteArrayAsyncCount { get; }

    long FetchAttributesAsyncCount { get; }

    long SetMetadataAsyncCount { get; }
  }
}
