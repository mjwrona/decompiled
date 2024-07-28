// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion.GetPackageVersionHandler`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion
{
  public class GetPackageVersionHandler<TPackageId, TMetadataEntry, TPackage> : 
    IAsyncHandler<PackageRequest<TPackageId, ShowDeletedBool>, TPackage>,
    IHaveInputType<PackageRequest<TPackageId, ShowDeletedBool>>,
    IHaveOutputType<TPackage>
    where TPackageId : IPackageIdentity
    where TMetadataEntry : IMetadataEntry
  {
    private readonly IConverter<TMetadataEntry, TPackage> packageConverter;
    private readonly IAsyncHandler<PackageRequest<TPackageId>, TMetadataEntry> metadataService;

    public GetPackageVersionHandler(
      IConverter<TMetadataEntry, TPackage> packageConverter,
      IAsyncHandler<PackageRequest<TPackageId>, TMetadataEntry> metadataService)
    {
      this.packageConverter = packageConverter;
      this.metadataService = metadataService;
    }

    public async Task<TPackage> Handle(
      PackageRequest<TPackageId, ShowDeletedBool> request)
    {
      TMetadataEntry metadataEntry = await this.metadataService.Handle((PackageRequest<TPackageId>) request);
      ShowDeletedBool additionalData = request.AdditionalData;
      if ((object) metadataEntry == null || metadataEntry.IsDeleted() && !additionalData.Value)
        throw ExceptionHelper.PackageNotFound((IPackageIdentity) request.PackageId, request.Feed);
      return this.packageConverter.Convert(metadataEntry);
    }
  }
}
