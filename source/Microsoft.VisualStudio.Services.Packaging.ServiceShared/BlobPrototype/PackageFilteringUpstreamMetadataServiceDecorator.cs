// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PackageFilteringUpstreamMetadataServiceDecorator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class PackageFilteringUpstreamMetadataServiceDecorator
  {
    public static IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> Create<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>(
      IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry> upstreamMetadataService,
      IConverter<TPackageIdentity, Exception> packageIdentityToExceptionConverter,
      IConverter<(TPackageName, TPackageVersion), TPackageIdentity> nameAndVersionToPackageIdentityConverter)
      where TPackageName : IPackageName
      where TPackageVersion : IPackageVersion
      where TPackageIdentity : IPackageIdentity<TPackageName, TPackageVersion>
      where TMetadataEntry : IMetadataEntry<TPackageIdentity>
    {
      return (IUpstreamMetadataService<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>) new PackageFilteringUpstreamMetadataServiceDecorator<TPackageName, TPackageVersion, TPackageIdentity, TMetadataEntry>(upstreamMetadataService, packageIdentityToExceptionConverter, nameAndVersionToPackageIdentityConverter);
    }
  }
}
