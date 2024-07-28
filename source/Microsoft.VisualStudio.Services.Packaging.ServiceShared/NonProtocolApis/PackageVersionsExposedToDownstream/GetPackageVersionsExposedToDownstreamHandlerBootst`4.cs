// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream.GetPackageVersionsExposedToDownstreamHandlerBootstrapper`4
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream
{
  public class GetPackageVersionsExposedToDownstreamHandlerBootstrapper<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry> : 
    RequireAggHandlerBootstrapper<IPackageNameRequest<TPackageName>, IReadOnlyList<VersionWithSourceChain<TPackageVersion>>, IReadMetadataService<TPackageIdentity, TMetadataEntry>>
    where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    protected override IAsyncHandler<IPackageNameRequest<TPackageName>, IReadOnlyList<VersionWithSourceChain<TPackageVersion>>> Bootstrap(
      IReadMetadataService<TPackageIdentity, TMetadataEntry> metadataService)
    {
      return (IAsyncHandler<IPackageNameRequest<TPackageName>, IReadOnlyList<VersionWithSourceChain<TPackageVersion>>>) new GetPackageVersionsExposedToDownstreamHandler<TPackageIdentity, TPackageName, TPackageVersion, TMetadataEntry>(metadataService);
    }
  }
}
