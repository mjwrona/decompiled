// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.IContainerStorageAccessStatistics
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public interface IContainerStorageAccessStatistics
  {
    long ListBlobsSegmentedAsyncCount { get; }

    long GetBlockBlobReferenceCount { get; }

    long CreateIfNotExistsAsyncCount { get; }

    long DeleteIfExistsAsyncCount { get; }

    long GetSharedAccessSignatureCount { get; }

    long GetPermissionsAsyncCount { get; }

    long SetPermissionsAsyncCount { get; }

    BlobStorageAccessStatistics BlobStatistics { get; }
  }
}
