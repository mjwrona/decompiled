// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.NuGetSearchQueryBuilderConverter
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.OData;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class NuGetSearchQueryBuilderConverter : 
    IConverter<NuGetSearchQueryInputs, NuGetSearchQuery>,
    IHaveInputType<NuGetSearchQueryInputs>,
    IHaveOutputType<NuGetSearchQuery>
  {
    private static readonly VersionRange SemVerSupportsBuildMetadataRange = new VersionRange(new NuGetVersion(2, 0, 0));
    private readonly IFilterRecognizer filterRecognizer;
    private readonly IOrderByRecognizer orderByRecognizer;
    private readonly ITracerService tracerService;

    public NuGetSearchQueryBuilderConverter(
      IFilterRecognizer filterRecognizer,
      IOrderByRecognizer orderByRecognizer,
      ITracerService tracerService)
    {
      this.filterRecognizer = filterRecognizer;
      this.orderByRecognizer = orderByRecognizer;
      this.tracerService = tracerService;
    }

    public NuGetSearchQuery Convert(NuGetSearchQueryInputs inputs)
    {
      using (this.tracerService.Enter((object) this, nameof (Convert)))
      {
        RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter> recognizeResult1 = inputs.ODataFilter != null ? this.filterRecognizer.RecognizeFilter(inputs.ODataFilter) : new RecognizeResult<Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter>(Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter.Empty);
        Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter filter = recognizeResult1.Result ?? Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation.Filter.Empty;
        ImmutableList<VssNuGetPackageIdentity>.Builder builder1 = ImmutableList.CreateBuilder<VssNuGetPackageIdentity>();
        ImmutableList<NuGetSearchPackageNameQuery>.Builder builder2 = ImmutableList.CreateBuilder<NuGetSearchPackageNameQuery>();
        foreach (PackageSelector packageSelector in filter.PackageSelectors)
        {
          if (packageSelector.Name.MatchType == NameMatchType.Exact && packageSelector.Version.MatchType == VersionMatchType.Exact)
            builder1.Add(new VssNuGetPackageIdentity(packageSelector.Name.NameString, packageSelector.Version.VersionString));
          else if (packageSelector.Name.MatchType == NameMatchType.AllNames)
          {
            builder2.Add(NuGetSearchPackageNameQuery.AllNames);
          }
          else
          {
            StringComparison stringComparison = packageSelector.Name.CaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            builder2.Add(new NuGetSearchPackageNameQuery(packageSelector.Name.NameString, stringComparison, packageSelector.Name.MatchType));
          }
        }
        if (!string.IsNullOrWhiteSpace(inputs.SearchTerm))
          builder2.Add(new NuGetSearchPackageNameQuery(inputs.SearchTerm, StringComparison.OrdinalIgnoreCase, NameMatchType.Substring));
        if (builder1.Count == 0 && builder2.Count == 0)
          builder2.Add(NuGetSearchPackageNameQuery.AllNames);
        ImmutableList<VersionCategorySelector> categorySelectors = filter.VersionCategorySelectors;
        if (categorySelectors.IsEmpty)
          categorySelectors = ImmutableList.Create<VersionCategorySelector>(VersionCategorySelector.AllVersions);
        NuGetVersion version;
        bool versionsWithBuildMetadataAreVisible = !string.IsNullOrWhiteSpace(inputs.SemVerLevel) && NuGetVersion.TryParse(inputs.SemVerLevel, out version) && NuGetSearchQueryBuilderConverter.SemVerSupportsBuildMetadataRange.Satisfies(version);
        RecognizeResult<object> recognizeResult2 = inputs.ODataOrderBy != null ? this.orderByRecognizer.RecognizeOrderBy(inputs.ODataOrderBy) : new RecognizeResult<object>((object) null);
        return new NuGetSearchQuery(builder1.ToImmutable(), builder2.ToImmutable(), categorySelectors, versionsWithBuildMetadataAreVisible, inputs.IncludePrereleaseVersions, inputs.IncludeDelistedVersions, inputs.ResultShape, inputs.Skip, inputs.Take, (IEnumerable<Exception>) recognizeResult1.Errors, (IEnumerable<Exception>) recognizeResult2.Errors, inputs.NonLocalVersionsAppearToExist);
      }
    }
  }
}
