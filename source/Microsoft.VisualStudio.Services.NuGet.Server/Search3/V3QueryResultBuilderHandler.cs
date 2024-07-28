// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.V3QueryResultBuilderHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class V3QueryResultBuilderHandler : 
    IAsyncHandler<IFeedRequest<NuGetSearchResultsInfo>, QueryResult>,
    IHaveInputType<IFeedRequest<NuGetSearchResultsInfo>>,
    IHaveOutputType<QueryResult>
  {
    private readonly INuGetIconUriCalculator iconUriCalculator;
    private readonly INuGetLicenseUriCalculator licenseUriCalculator;

    public V3QueryResultBuilderHandler(
      INuGetIconUriCalculator iconUriCalculator,
      INuGetLicenseUriCalculator licenseUriCalculator)
    {
      this.iconUriCalculator = iconUriCalculator;
      this.licenseUriCalculator = licenseUriCalculator;
    }

    public Task<QueryResult> Handle(IFeedRequest<NuGetSearchResultsInfo> request) => Task.FromResult<QueryResult>(this.BuildIndexJson((IFeedRequest) request, request.AdditionalData.Results));

    private QueryResult BuildIndexJson(
      IFeedRequest request,
      IEnumerable<NuGetSearchResult> packages)
    {
      return new QueryResult()
      {
        TotalHits = "0",
        Index = "PackageIndex",
        LastReopen = DateTime.UtcNow.ToString("o"),
        Data = packages.Select<NuGetSearchResult, QueryResultPackage>((Func<NuGetSearchResult, QueryResultPackage>) (result => this.BuildIndexPackageJson(request, result))),
        Context = new Context()
        {
          Vocab = new Uri("http://schema.nuget.org/schema#")
        }
      };
    }

    private QueryResultPackage BuildIndexPackageJson(
      IFeedRequest request,
      NuGetSearchResult package)
    {
      NuGetPackageMetadata metadata = package.Metadata;
      PackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata> request1 = request.WithPackage<VssNuGetPackageIdentity>(metadata.Identity).WithData<VssNuGetPackageIdentity, NuGetPackageMetadata>(metadata);
      QueryResultPackage queryResultPackage = new QueryResultPackage();
      queryResultPackage.Aid = string.Empty;
      queryResultPackage.Atype = "Package";
      queryResultPackage.Id = package.PackageIdentity.Name.DisplayName;
      queryResultPackage.Version = package.PackageIdentity.Version.NormalizedOriginalCaseVersion;
      queryResultPackage.Description = metadata.Description;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      queryResultPackage.Versions = package.AllMatchingVersionsOfPackage.Select<NuGetSearchResultVersionSummary, QueryResultPackageVersion>(V3QueryResultBuilderHandler.\u003C\u003EO.\u003C0\u003E__BuildIndexVersionJson ?? (V3QueryResultBuilderHandler.\u003C\u003EO.\u003C0\u003E__BuildIndexVersionJson = new Func<NuGetSearchResultVersionSummary, QueryResultPackageVersion>(V3QueryResultBuilderHandler.BuildIndexVersionJson)));
      queryResultPackage.Authors = new List<string>()
      {
        metadata.Authors
      };
      queryResultPackage.IconUrl = this.iconUriCalculator.GetIconUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request1);
      queryResultPackage.LicenseUrl = this.licenseUriCalculator.GetLicenseUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request1);
      queryResultPackage.ProjectUrl = metadata.ProjectUrl;
      queryResultPackage.Registration = string.Empty;
      queryResultPackage.Summary = metadata.Summary;
      ImmutableList<string> tags = metadata.Tags;
      queryResultPackage.Tags = (tags != null ? tags.ToList<string>() : (List<string>) null) ?? new List<string>();
      queryResultPackage.Title = metadata.Title;
      queryResultPackage.TotalDownloadsLong = new long?();
      queryResultPackage.Verified = new bool?();
      return queryResultPackage;
    }

    private static QueryResultPackageVersion BuildIndexVersionJson(
      NuGetSearchResultVersionSummary versionSummary)
    {
      return new QueryResultPackageVersion()
      {
        Aid = versionSummary.PackageIdentity.Name.DisplayName,
        Downloads = 0,
        Version = versionSummary.PackageIdentity.Version.NormalizedOriginalCaseVersion
      };
    }
  }
}
