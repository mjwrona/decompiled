// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DownloadBlobPackageFileAsContentResultHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DownloadBlobPackageFileAsContentResultHandler : ISpecificStorageContentProvider
  {
    private readonly IExecutionEnvironment executionEnvironment;
    private readonly IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> blobIdToUriHandler;
    private readonly IContentBlobStore contentBlobStore;

    public DownloadBlobPackageFileAsContentResultHandler(
      IExecutionEnvironment executionEnvironment,
      IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri> blobIdToUriHandler,
      IContentBlobStore contentBlobStore)
    {
      this.executionEnvironment = executionEnvironment;
      this.blobIdToUriHandler = blobIdToUriHandler;
      this.contentBlobStore = contentBlobStore;
    }

    public async Task<ContentResult> GetContentOrDefault(
      IPackageFileRequest request,
      IStorageId storageId)
    {
      if (!(storageId is BlobStorageId data))
        return (ContentResult) null;
      return this.executionEnvironment.IsHosted() ? new ContentResult(await this.blobIdToUriHandler.Handle(request.WithPackage<IPackageIdentity>(request.PackageId).WithFile<IPackageIdentity>(request.FilePath).WithData<IPackageIdentity, BlobStorageId>(data))) : new ContentResult(await this.contentBlobStore.GetBlobAsync(data.BlobId), request.FilePath);
    }
  }
}
