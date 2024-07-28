// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageVersionComparer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class PackageVersionComparer : IEqualityComparer<IPackageVersion>
  {
    private PackageVersionComparer()
    {
    }

    public static PackageVersionComparer NormalizedVersion { get; } = new PackageVersionComparer();

    public bool Equals(IPackageVersion? x, IPackageVersion? y) => x == y || StringComparer.Ordinal.Equals(x?.NormalizedVersion, y?.NormalizedVersion);

    public int GetHashCode(IPackageVersion? obj) => obj == null ? 0 : StringComparer.Ordinal.GetHashCode(obj.NormalizedVersion);
  }
}
