// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageNameComparer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public sealed class PackageNameComparer : IComparer<IPackageName>, IEqualityComparer<IPackageName>
  {
    private PackageNameComparer()
    {
    }

    public static PackageNameComparer NormalizedName { get; } = new PackageNameComparer();

    public int Compare(IPackageName x, IPackageName y) => x == y ? 0 : StringComparer.Ordinal.Compare(x?.NormalizedName, y?.NormalizedName);

    public bool Equals(IPackageName x, IPackageName y) => x == y || StringComparer.Ordinal.Equals(x?.NormalizedName, y?.NormalizedName);

    public int GetHashCode(IPackageName obj) => obj == null ? 0 : StringComparer.Ordinal.GetHashCode(obj.NormalizedName);
  }
}
