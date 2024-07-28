// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.NumVersionsOnPublishValidatingHandler`3
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion
{
  public class NumVersionsOnPublishValidatingHandler<TPackageId, TMetadataEntry, TStorable> : 
    IAsyncHandler<
    #nullable disable
    TStorable>,
    IAsyncHandler<TStorable, NullResult>,
    IHaveInputType<TStorable>,
    IHaveOutputType<NullResult>
    where TPackageId : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageId>
    where TStorable : IPackageRequest<TPackageId>
  {
    private readonly IReadMetadataService<TPackageId, TMetadataEntry> metadataService;
    private readonly IFrotocolLevelPackagingSetting<int> maxVersionsPerPackageSetting;

    public NumVersionsOnPublishValidatingHandler(
      IReadMetadataService<TPackageId, TMetadataEntry> metadataService,
      IFrotocolLevelPackagingSetting<int> maxVersionsPerPackageSetting)
    {
      this.metadataService = metadataService;
      this.maxVersionsPerPackageSetting = maxVersionsPerPackageSetting;
    }

    public async Task<NullResult> Handle(TStorable request)
    {
      int maxVersions = this.maxVersionsPerPackageSetting.Get((IFeedRequest) request);
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest = new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) new PackageNameRequest<IPackageName>((IFeedRequest) request, request.PackageId.Name));
      packageNameQueryRequest.Options = new QueryOptions<TMetadataEntry>().OnlyProjecting((Expression<Func<TMetadataEntry, object>>) (e => e.PackageStorageId)).OnlyProjecting((Expression<Func<TMetadataEntry, object>>) (e => (object) e.DeletedDate)).WithFilter((Func<TMetadataEntry, bool>) (e => !e.PackageIdentity.Equals((object) request.PackageId))).WithFilter((Func<TMetadataEntry, bool>) (e =>
      {
        if (e.IsDeleted())
          return false;
        return e.PackageStorageId == null || e.PackageStorageId.IsLocal;
      }));
      if ((await this.metadataService.GetPackageVersionStatesAsync(packageNameQueryRequest)).Count >= maxVersions)
        throw new TooManyVersionsException(Resources.Error_PackageTooManyVersions((object) request.PackageId.Name.DisplayName, (object) maxVersions));
      return (NullResult) null;
    }
  }
}
