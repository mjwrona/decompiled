// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.PackageSelector
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class PackageSelector
  {
    public PackageNameSelector Name { get; }

    public PackageVersionSelector Version { get; }

    public PackageSelector(PackageNameSelector name, PackageVersionSelector version)
    {
      this.Name = name ?? throw new ArgumentNullException(nameof (name));
      this.Version = version ?? throw new ArgumentNullException(nameof (version));
      if (name.MatchType != NameMatchType.Exact && version.MatchType != VersionMatchType.AllVersions)
        throw new ArgumentException("Inexact name match can only be combined with all-versions version match");
    }

    public static PackageSelector AllPackages { get; } = new PackageSelector(PackageNameSelector.AllPackageNames, PackageVersionSelector.AllVersions);
  }
}
