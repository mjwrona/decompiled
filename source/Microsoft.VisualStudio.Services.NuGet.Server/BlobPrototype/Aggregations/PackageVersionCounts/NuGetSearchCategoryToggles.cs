// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.NuGetSearchCategoryToggles
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public record NuGetSearchCategoryToggles(
    bool VersionsWithBuildMetadataAppearToExist,
    bool IncludePrereleaseVersions,
    bool IncludeDelistedVersions,
    bool NonLocalVersionsAppearToExist)
  {
    public static 
    #nullable disable
    NuGetSearchCategoryToggles FromQuery(NuGetSearchQuery query) => new NuGetSearchCategoryToggles(query.VersionsWithBuildMetadataAreVisible, query.IncludePrereleaseVersions, query.IncludeDelistedVersions, query.NonLocalVersionsAppearToExist);
  }
}
