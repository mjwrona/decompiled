// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.StorageContentProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Collections.Generic;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public class StorageContentProvider : IStorageContentProvider
  {
    private readonly IReadOnlyList<ISpecificStorageContentProvider> plainProviders;
    private readonly IReadOnlyList<ISpecificExtractingStorageContentProvider> extractingProviders;

    public StorageContentProvider(
      IReadOnlyList<ISpecificStorageContentProvider> plainProviders,
      IReadOnlyList<ISpecificExtractingStorageContentProvider> extractingProviders)
    {
      this.plainProviders = plainProviders;
      this.extractingProviders = extractingProviders;
    }

    public async Task<ContentResult> GetContent(IPackageFileRequest request, IStorageId storageId)
    {
      foreach (ISpecificStorageContentProvider plainProvider in (IEnumerable<ISpecificStorageContentProvider>) this.plainProviders)
      {
        ContentResult contentOrDefault = await plainProvider.GetContentOrDefault(request, storageId);
        if (contentOrDefault != null)
          return contentOrDefault;
      }
      if (storageId is IExtractingStorageId extractingStorageId)
      {
        if (!extractingStorageId.ExtractFrom.IsLocal)
          throw new PackageNotFoundException(request is IPackageInnerFileRequest innerFileRequest ? Resources.Error_CannotExtractFromNonLocalContentInnerFile((object) innerFileRequest.InnerFilePath, (object) innerFileRequest.FilePath, (object) innerFileRequest.PackageId.DisplayStringForMessages, (object) innerFileRequest.Feed.FullyQualifiedName, (object) extractingStorageId.NonLegacyValueString) : Resources.Error_CannotExtractFromNonLocalContent((object) request.FilePath, (object) request.PackageId.DisplayStringForMessages, (object) request.Feed.FullyQualifiedName, (object) extractingStorageId.NonLegacyValueString));
        ContentResult containerContent = await this.GetContent(request, extractingStorageId.ExtractFrom);
        foreach (ISpecificExtractingStorageContentProvider extractingProvider in (IEnumerable<ISpecificExtractingStorageContentProvider>) this.extractingProviders)
        {
          ContentResult contentOrDefault = await extractingProvider.GetContentOrDefault(request, extractingStorageId, containerContent);
          if (contentOrDefault != null)
            return contentOrDefault;
        }
        containerContent = (ContentResult) null;
      }
      bool? nullable = !(storageId is DropStorageId) ? storageId.RepresentsSingleFile : throw new InvalidUserRequestException(Resources.Error_NoDirectDownloadForDropBackedPackage());
      bool flag = false;
      if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        throw new InvalidUserRequestException(request is IPackageInnerFileRequest innerFileRequest1 ? Resources.Error_NoDirectDownloadNotSingleFileInnerFile((object) innerFileRequest1.InnerFilePath, (object) innerFileRequest1.FilePath, (object) innerFileRequest1.PackageId.DisplayStringForMessages, (object) innerFileRequest1.Feed.FullyQualifiedName, (object) storageId.NonLegacyValueString) : Resources.Error_NoDirectDownloadNotSingleFile((object) request.FilePath, (object) request.PackageId.DisplayStringForMessages, (object) request.Feed.FullyQualifiedName, (object) storageId.NonLegacyValueString));
      if (!storageId.IsLocal)
        throw new PackageNotFoundException(request is IPackageInnerFileRequest innerFileRequest2 ? Resources.Error_PackageFileNotFoundTryIngestingInnerFile((object) innerFileRequest2.Feed.FullyQualifiedName, (object) innerFileRequest2.PackageId.DisplayStringForMessages, (object) innerFileRequest2.FilePath, (object) innerFileRequest2.FilePath, (object) innerFileRequest2.InnerFilePath) : Resources.Error_PackageFileNotFoundTryIngesting((object) request.Feed.FullyQualifiedName, (object) request.PackageId.DisplayStringForMessages, (object) request.FilePath, (object) request.FilePath));
      throw new InvalidHandlerException(Resources.Error_NoContentProviderForId((object) storageId.NonLegacyValueString));
    }
  }
}
