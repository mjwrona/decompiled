// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.MetadataVersionComparer`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class MetadataVersionComparer<TPackageIdentity, TMetadataEntry> : IComparer<TMetadataEntry>
    where TPackageIdentity : IPackageIdentity
    where TMetadataEntry : IMetadataEntry<TPackageIdentity>
  {
    private readonly IComparer<IPackageVersion> versionComparer;

    public MetadataVersionComparer(IComparer<IPackageVersion> versionComparer) => this.versionComparer = versionComparer;

    public int Compare(TMetadataEntry x, TMetadataEntry y)
    {
      IComparer<IPackageVersion> versionComparer = this.versionComparer;
      TPackageIdentity packageIdentity = x.PackageIdentity;
      IPackageVersion version1 = packageIdentity.Version;
      packageIdentity = y.PackageIdentity;
      IPackageVersion version2 = packageIdentity.Version;
      return versionComparer.Compare(version1, version2);
    }
  }
}
