// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NuGetPackageMetadataSearchVersionFilteringStrategy
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NuGetPackageMetadataSearchVersionFilteringStrategy : 
    INuGetPackageMetadataSearchVersionFilteringStrategy
  {
    public bool DoesVersionAppearToExist(
      NuGetSearchCategoryToggles query,
      Guid viewId,
      NuGetSearchResultVersionSummary versionInfo)
    {
      bool flag1 = viewId == Guid.Empty || versionInfo.Views.Contains(viewId);
      bool flag2 = query.VersionsWithBuildMetadataAppearToExist || !versionInfo.IsSemVer2;
      if (!(!versionInfo.IsDeleted & flag1 & flag2))
        return false;
      return versionInfo.IsLocal || query.NonLocalVersionsAppearToExist;
    }

    public bool IsVersionSelectable(
      NuGetSearchCategoryToggles query,
      NuGetSearchResultVersionSummary versionInfo)
    {
      return (query.IncludeDelistedVersions || versionInfo.IsListed) && (query.IncludePrereleaseVersions || !versionInfo.IsPrerelease);
    }
  }
}
