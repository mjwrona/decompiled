// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.NuGetSearchQuery
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation
{
  public class NuGetSearchQuery
  {
    public int Skip { get; }

    public int Take { get; }

    public NuGetSearchResultShape ResultShape { get; }

    public ImmutableList<VssNuGetPackageIdentity> PointQueries { get; }

    public ImmutableList<NuGetSearchPackageNameQuery> NameQueries { get; }

    public ImmutableList<VersionCategorySelector> VersionCategories { get; }

    public bool VersionsWithBuildMetadataAreVisible { get; }

    public bool IncludePrereleaseVersions { get; }

    public bool IncludeDelistedVersions { get; }

    public bool NonLocalVersionsAppearToExist { get; }

    public ImmutableList<Exception> FilterRecognizerErrors { get; }

    public ImmutableList<Exception> OrderByRecognizerErrors { get; }

    public ImmutableList<Exception> AllErrors { get; }

    public bool CanExecuteQuery => this.AllErrors.IsEmpty;

    public NuGetSearchQuery(
      ImmutableList<VssNuGetPackageIdentity> pointQueries,
      ImmutableList<NuGetSearchPackageNameQuery> nameQueries,
      ImmutableList<VersionCategorySelector> versionCategories,
      bool versionsWithBuildMetadataAreVisible,
      bool includePrereleaseVersions,
      bool includeDelistedVersions,
      NuGetSearchResultShape resultShape,
      int skip,
      int take,
      IEnumerable<Exception> filterRecognizerErrors,
      IEnumerable<Exception> orderByErrors,
      bool nonLocalVersionsAppearToExist)
    {
      if (filterRecognizerErrors == null)
        throw new ArgumentNullException(nameof (filterRecognizerErrors));
      if (orderByErrors == null)
        throw new ArgumentNullException(nameof (orderByErrors));
      this.VersionsWithBuildMetadataAreVisible = versionsWithBuildMetadataAreVisible;
      this.Skip = skip;
      this.Take = take;
      this.NonLocalVersionsAppearToExist = nonLocalVersionsAppearToExist;
      this.PointQueries = pointQueries;
      this.NameQueries = nameQueries;
      this.VersionCategories = versionCategories;
      this.IncludePrereleaseVersions = includePrereleaseVersions;
      this.IncludeDelistedVersions = includeDelistedVersions;
      this.ResultShape = resultShape;
      this.FilterRecognizerErrors = filterRecognizerErrors.ToImmutableList<Exception>();
      this.OrderByRecognizerErrors = orderByErrors.ToImmutableList<Exception>();
      this.AllErrors = this.FilterRecognizerErrors.AddRange((IEnumerable<Exception>) this.OrderByRecognizerErrors);
      if ((pointQueries == null || nameQueries == null || versionCategories == null) && this.FilterRecognizerErrors.IsEmpty)
        throw new Exception("filters must not be null unless errors are specified");
    }
  }
}
