// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream.GetPackageVersionsExposedToDownstreamHandler`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream
{
  public class GetPackageVersionsExposedToDownstreamHandler<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry> : 
    IAsyncHandler<
    #nullable disable
    IPackageNameRequest<TPackageName>, IReadOnlyList<VersionWithSourceChain<TPackageVersion>>>,
    IHaveInputType<IPackageNameRequest<TPackageName>>,
    IHaveOutputType<IReadOnlyList<VersionWithSourceChain<TPackageVersion>>>
    where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService;

    public GetPackageVersionsExposedToDownstreamHandler(
      IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService)
    {
      this.metadataService = metadataService;
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<TPackageVersion>>> Handle(
      IPackageNameRequest<TPackageName> request)
    {
      if (request.Feed.View == null)
        throw new InvalidOperationException("Only views provide versions to downstream. Base feeds do not.");
      PackageNameQuery<TMetadataEntry> packageNameQueryRequest = new PackageNameQuery<TMetadataEntry>((IPackageNameRequest) request);
      packageNameQueryRequest.Options = new QueryOptions<TMetadataEntry>().WithFilter((Func<TMetadataEntry, bool>) (x => x.IsLocal && !x.IsDeleted()));
      return (IReadOnlyList<VersionWithSourceChain<TPackageVersion>>) (await this.metadataService.GetPackageVersionStatesAsync(packageNameQueryRequest)).Select<TMetadataEntry, VersionWithSourceChain<TPackageVersion>>((Func<TMetadataEntry, VersionWithSourceChain<TPackageVersion>>) (x => VersionWithSourceChain.FromThisFeed<TPackageVersion>(x.PackageIdentity.Version, x.SourceChain))).ToList<VersionWithSourceChain<TPackageVersion>>();
    }
  }
}
