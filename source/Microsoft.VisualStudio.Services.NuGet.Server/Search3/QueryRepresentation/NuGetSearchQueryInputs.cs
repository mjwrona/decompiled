// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.NuGetSearchQueryInputs
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class NuGetSearchQueryInputs
  {
    public string SearchTerm { get; }

    public string ODataFilter { get; }

    public string ODataOrderBy { get; }

    public bool IncludeDelistedVersions { get; }

    public bool IncludePrereleaseVersions { get; }

    public string SemVerLevel { get; }

    public int Skip { get; }

    public int Take { get; }

    public NuGetSearchResultShape ResultShape { get; }

    public bool NonLocalVersionsAppearToExist { get; }

    public NuGetSearchQueryInputs(
      string searchTerm,
      string oDataFilter,
      string oDataOrderBy,
      bool includeDelistedVersions,
      bool includePrereleaseVersions,
      string semVerLevel,
      int skip,
      int take,
      NuGetSearchResultShape resultShape,
      bool nonLocalVersionsAppearToExist)
    {
      this.SearchTerm = searchTerm;
      this.ODataFilter = oDataFilter;
      this.ODataOrderBy = oDataOrderBy;
      this.IncludeDelistedVersions = includeDelistedVersions;
      this.IncludePrereleaseVersions = includePrereleaseVersions;
      this.SemVerLevel = semVerLevel;
      this.Skip = skip;
      this.Take = take;
      this.ResultShape = resultShape;
      this.NonLocalVersionsAppearToExist = nonLocalVersionsAppearToExist;
    }
  }
}
