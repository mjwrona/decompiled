// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenGetPackageFileContentHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenGetPackageFileContentHandler : 
    IAsyncHandler<MavenFileRequest, MavenPackageFileResponse>,
    IHaveInputType<MavenFileRequest>,
    IHaveOutputType<MavenPackageFileResponse>
  {
    private readonly IMetadataCacheService cacheService;
    private readonly IFactory<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>> metadataFactory;
    private readonly IContentBlobStore contentBlobStore;
    private readonly IValidator<MavenArtifactFileRequest> packageRequestValidator;
    private readonly IConverter<MavenFileRequest, MavenArtifactFileRequest> mavenFileRequestConverter;
    private readonly IFactory<DateTimeOffset> sasTokenExpiryFactory;

    public MavenGetPackageFileContentHandler(
      IMetadataCacheService cacheService,
      IFactory<IPackageRequest<MavenPackageIdentity>, Task<IMavenMetadataEntry>> metadataFactory,
      IContentBlobStore contentBlobStore,
      IValidator<MavenArtifactFileRequest> packageRequestValidator,
      IConverter<MavenFileRequest, MavenArtifactFileRequest> mavenFileRequestConverter,
      IFactory<DateTimeOffset> sasTokenExpiryFactory)
    {
      this.cacheService = cacheService;
      this.metadataFactory = metadataFactory;
      this.contentBlobStore = contentBlobStore;
      this.packageRequestValidator = packageRequestValidator;
      this.mavenFileRequestConverter = mavenFileRequestConverter;
      this.sasTokenExpiryFactory = sasTokenExpiryFactory;
    }

    public async Task<MavenPackageFileResponse> Handle(MavenFileRequest input) => await this.GetPackageContentsAsync(input);

    private async Task<MavenPackageFileResponse> GetPackageContentsAsync(
      MavenFileRequest fileRequest)
    {
      MavenPackageFileResponse response = new MavenPackageFileResponse()
      {
        FileRequest = fileRequest,
        FileName = fileRequest.FilePath.FileName
      };
      MavenArtifactFileRequest artifactRequest = this.mavenFileRequestConverter.Convert(fileRequest);
      this.packageRequestValidator.Validate(artifactRequest);
      ICachablePackageMetadata packageMetadata1;
      IStorageId storageId1;
      if (this.cacheService.TryGetPackageMetadata((IPackageFileRequest) artifactRequest, out packageMetadata1))
      {
        storageId1 = packageMetadata1.StorageId;
        response.ContentSize = packageMetadata1.SizeInBytes;
      }
      else
      {
        IMavenMetadataEntry metadataEntry = (await this.GetMetadataEntryAsync((IPackageRequest<MavenPackageIdentity>) artifactRequest)).ThrowIfNotActive(artifactRequest.Feed);
        (MavenPackageFileNew packageFile, IStorageId storageId2) = MavenGetPackageFileContentHandler.GetPackageFileStorage(artifactRequest, metadataEntry);
        CachablePackageMetadata packageMetadata2 = packageFile == null ? new CachablePackageMetadata((IMetadataEntry) metadataEntry) : new CachablePackageMetadata((IMetadataEntry) metadataEntry, storageId2, packageFile.SizeInBytes);
        this.cacheService.SetPackageMetadata((IPackageFileRequest) artifactRequest, (ICachablePackageMetadata) packageMetadata2);
        storageId1 = storageId2;
        // ISSUE: explicit non-virtual call
        response.ContentSize = packageFile != null ? __nonvirtual (packageFile.SizeInBytes) : -1L;
      }
      if (artifactRequest.RequireContent)
      {
        switch (storageId1)
        {
          case LiteralStringStorageId literalStringStorageId:
            response.Content = (Stream) new MemoryStream(Encoding.UTF8.GetBytes(literalStringStorageId.Value));
            break;
          case BlobStorageId blobStorageId:
            MavenPackageFileResponse packageFileResponse;
            if (artifactRequest.StreamContent)
            {
              packageFileResponse = response;
              packageFileResponse.Content = await this.contentBlobStore.GetBlobAsync(blobStorageId.BlobId);
              packageFileResponse = (MavenPackageFileResponse) null;
              break;
            }
            string filename = artifactRequest.FilePath?.Replace('+', '-');
            packageFileResponse = response;
            packageFileResponse.Uri = await this.contentBlobStore.GetDownloadUriAsync(new BlobIdWithHeaders(blobStorageId.BlobId, EdgeCache.NotAllowed, filename, expiryTime: new DateTimeOffset?(this.sasTokenExpiryFactory.Get())));
            packageFileResponse = (MavenPackageFileResponse) null;
            break;
          case null:
            throw new InvalidOperationException("File had null storage ID");
          default:
            throw new InvalidOperationException("Unknown storage ID type " + storageId1.GetType().Name);
        }
      }
      else
        response.Content = (Stream) null;
      MavenPackageFileResponse packageContentsAsync = response;
      response = (MavenPackageFileResponse) null;
      artifactRequest = (MavenArtifactFileRequest) null;
      return packageContentsAsync;
    }

    private static (MavenPackageFileNew packageFile, IStorageId storageId) GetPackageFileStorage(
      MavenArtifactFileRequest fileRequest,
      IMavenMetadataEntry metadataEntry)
    {
      string filePath = fileRequest.FilePath;
      MavenPackageFileNew packageFileWithPath1 = metadataEntry.GetPackageFileWithPath(filePath);
      if (packageFileWithPath1 != null)
        return (packageFileWithPath1, packageFileWithPath1.StorageId);
      (string str, MavenHashAlgorithmInfo Algorithm) = MavenFileNameUtility.SplitChecksumFileName(filePath);
      if (Algorithm == null)
        throw ExceptionHelper.PackageNotFound(Resources.Error_ArtifactFileMissing((object) filePath, (object) fileRequest.PackageId));
      MavenPackageFileNew packageFileWithPath2 = metadataEntry.GetPackageFileWithPath(str);
      if (packageFileWithPath2 == null)
        throw ExceptionHelper.PackageNotFound(Resources.Error_ArtifactFileMissing((object) filePath, (object) fileRequest.PackageId));
      return (packageFileWithPath2, (IStorageId) new LiteralStringStorageId((packageFileWithPath2.Hashes.FirstOrDefault<HashAndType>((Func<HashAndType, bool>) (x => x.HashType == Algorithm.HashType)) ?? throw ExceptionHelper.PackageNotFound(Resources.Error_ArtifactFileChecksumMissing((object) filePath, (object) fileRequest.PackageId))).Value));
    }

    private async Task<IMavenMetadataEntry> GetMetadataEntryAsync(
      IPackageRequest<MavenPackageIdentity> packageRequest)
    {
      return await this.metadataFactory.Get(packageRequest) ?? throw ExceptionHelper.PackageNotFound((IPackageIdentity) packageRequest.PackageId, packageRequest.Feed);
    }
  }
}
