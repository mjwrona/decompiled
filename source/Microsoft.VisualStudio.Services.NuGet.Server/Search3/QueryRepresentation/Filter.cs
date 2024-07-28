// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using System;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class Filter
  {
    public static readonly Filter Empty = new Filter(ImmutableList<PackageSelector>.Empty, ImmutableList<VersionCategorySelector>.Empty);

    public ImmutableList<PackageSelector> PackageSelectors { get; }

    public ImmutableList<VersionCategorySelector> VersionCategorySelectors { get; }

    public Filter(
      ImmutableList<PackageSelector> packageSelectors,
      ImmutableList<VersionCategorySelector> versionCategorySelectors)
    {
      this.PackageSelectors = packageSelectors ?? throw new ArgumentNullException(nameof (packageSelectors));
      this.VersionCategorySelectors = versionCategorySelectors ?? throw new ArgumentNullException(nameof (versionCategorySelectors));
    }
  }
}
