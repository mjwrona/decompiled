// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex.NuGetSortableVersionBuilder
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIndex
{
  public class NuGetSortableVersionBuilder : SortableVersionBuilder
  {
    public NuGetSortableVersionBuilder()
      : base(4)
    {
    }

    public override Exception TryGetSortableVersion(
      string normalizedVersion,
      out string sortableVersion)
    {
      return base.TryGetSortableVersion(normalizedVersion, out sortableVersion);
    }

    public override string GetSortableVersion(string normalizedVersion) => base.GetSortableVersion(normalizedVersion);
  }
}
