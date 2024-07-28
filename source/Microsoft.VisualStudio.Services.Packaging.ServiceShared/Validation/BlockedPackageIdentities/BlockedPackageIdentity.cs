// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities.BlockedPackageIdentity
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities
{
  public class BlockedPackageIdentity
  {
    public IPackageName Name { get; }

    public IPackageVersion Version { get; }

    public bool AllVersionsBlocked => this.Version == null;

    public BlockedPackageIdentity(IPackageName name, IPackageVersion version)
    {
      this.Name = name ?? throw new ArgumentNullException(nameof (name));
      this.Version = version;
    }

    public bool Matches(IPackageIdentity packageIdentity)
    {
      if (!PackageNameComparer.NormalizedName.Equals(this.Name, packageIdentity.Name))
        return false;
      return this.AllVersionsBlocked || PackageVersionComparer.NormalizedVersion.Equals(this.Version, packageIdentity.Version);
    }

    public static BlockedPackageIdentity AllVersionsOf(IPackageName name) => new BlockedPackageIdentity(name, (IPackageVersion) null);
  }
}
