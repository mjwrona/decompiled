// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.DirectDownloadUriCalculator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class DirectDownloadUriCalculator : 
    IAsyncHandler<IPackageFileRequest<IPackageIdentity, BlobStorageId>, Uri>,
    IHaveInputType<IPackageFileRequest<IPackageIdentity, BlobStorageId>>,
    IHaveOutputType<Uri>
  {
    private readonly IContentBlobStore blobStore;
    private readonly IFactory<IPackageFileRequest, DownloadUriSettings> downloadUriSettingsFactory;
    private readonly IFactory<DateTimeOffset> sasTokenExpiryFactory;

    public DirectDownloadUriCalculator(
      IContentBlobStore blobStore,
      IFactory<IPackageFileRequest, DownloadUriSettings> downloadUriSettingsFactory,
      IFactory<DateTimeOffset> sasTokenExpiryFactory)
    {
      this.blobStore = blobStore;
      this.downloadUriSettingsFactory = downloadUriSettingsFactory;
      this.sasTokenExpiryFactory = sasTokenExpiryFactory;
    }

    public async Task<Uri> Handle(
      IPackageFileRequest<IPackageIdentity, BlobStorageId> request)
    {
      DateTimeOffset dateTimeOffset = this.sasTokenExpiryFactory.Get();
      string filename = this.GetFilename(this.downloadUriSettingsFactory.Get((IPackageFileRequest) request).DownloadFilenameMode, (IPackageFileRequest) request)?.Replace('+', '-');
      return await this.blobStore.GetDownloadUriAsync(new BlobIdWithHeaders(request.AdditionalData.BlobId, EdgeCache.NotAllowed, filename, expiryTime: new DateTimeOffset?(dateTimeOffset)));
    }

    private string GetFilename(DownloadFilenameMode mode, IPackageFileRequest request) => mode == DownloadFilenameMode.None ? (string) null : Path.GetFileName(request.FilePath);
  }
}
