// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageIdentityComparer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class PackageIdentityComparer : IEqualityComparer<IPackageIdentity>
  {
    private PackageIdentityComparer()
    {
    }

    public static PackageIdentityComparer NormalizedNameAndVersion { get; } = new PackageIdentityComparer();

    public bool Equals(IPackageIdentity? x, IPackageIdentity? y)
    {
      if (x == y)
        return true;
      return PackageNameComparer.NormalizedName.Equals(x?.Name, y?.Name) && PackageVersionComparer.NormalizedVersion.Equals(x?.Version, y?.Version);
    }

    public int GetHashCode(IPackageIdentity? obj) => obj == null ? 0 : PackageNameComparer.NormalizedName.GetHashCode(obj?.Name) * 397 ^ PackageVersionComparer.NormalizedVersion.GetHashCode(obj?.Version);
  }
}
