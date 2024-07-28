// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NuGetLatestVersionsFinder
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NuGetLatestVersionsFinder : INuGetLatestVersionsFinder
  {
    public LatestVersions FindLatestVersions(
      IReadOnlyCollection<NuGetSearchResultVersionSummary> entries)
    {
      NuGetSearchResultVersionSummary resultVersionSummary = entries.FirstOrDefault<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (x => x.IsListed));
      return new LatestVersions(entries.FirstOrDefault<NuGetSearchResultVersionSummary>((Func<NuGetSearchResultVersionSummary, bool>) (x => x.IsListed && !x.IsPrerelease))?.PackageIdentity.Version, resultVersionSummary?.PackageIdentity.Version);
    }
  }
}
