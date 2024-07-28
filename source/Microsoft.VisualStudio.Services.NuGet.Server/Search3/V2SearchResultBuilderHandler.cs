// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Search3.V2SearchResultBuilderHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Search3
{
  public class V2SearchResultBuilderHandler : 
    IAsyncHandler<IFeedRequest<NuGetSearchResultsInfo>, IEnumerable<ServerV2FeedPackage>>,
    IHaveInputType<IFeedRequest<NuGetSearchResultsInfo>>,
    IHaveOutputType<IEnumerable<ServerV2FeedPackage>>
  {
    private static readonly DateTime UnlistedPackagePublishDate = DateTime.MinValue.AddDays(1.0);
    private readonly INuGetLicenseUriCalculator licenseUriCalculator;
    private readonly INuGetIconUriCalculator iconUriCalculator;

    public Task<IEnumerable<ServerV2FeedPackage>> Handle(
      IFeedRequest<NuGetSearchResultsInfo> request)
    {
      return Task.FromResult<IEnumerable<ServerV2FeedPackage>>(request.AdditionalData.Results.Select<NuGetSearchResult, ServerV2FeedPackage>((Func<NuGetSearchResult, ServerV2FeedPackage>) (r => this.ToServerV2FeedPackage((IFeedRequest) request, r))));
    }

    public V2SearchResultBuilderHandler(
      INuGetLicenseUriCalculator licenseUriCalculator,
      INuGetIconUriCalculator iconUriCalculator)
    {
      this.licenseUriCalculator = licenseUriCalculator;
      this.iconUriCalculator = iconUriCalculator;
    }

    private ServerV2FeedPackage ToServerV2FeedPackage(
      IFeedRequest request,
      NuGetSearchResult searchResult)
    {
      NuGetPackageMetadata metadata = searchResult.Metadata;
      DateTime dateTime = metadata.Listed ? searchResult.Published : V2SearchResultBuilderHandler.UnlistedPackagePublishDate;
      PackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata> request1 = request.WithPackage<VssNuGetPackageIdentity>(metadata.Identity).WithData<VssNuGetPackageIdentity, NuGetPackageMetadata>(metadata);
      ServerV2FeedPackage serverV2FeedPackage = new ServerV2FeedPackage();
      serverV2FeedPackage.PackageIdentity = searchResult.PackageIdentity;
      serverV2FeedPackage.PackageStorageId = searchResult.PackageStorageId;
      serverV2FeedPackage.Id = searchResult.PackageIdentity.Name.DisplayName;
      serverV2FeedPackage.Version = searchResult.PackageIdentity.Version.DisplayVersion;
      serverV2FeedPackage.NormalizedVersion = searchResult.PackageIdentity.Version.NormalizedOriginalCaseVersion;
      serverV2FeedPackage.Authors = metadata.Authors;
      serverV2FeedPackage.Copyright = metadata.Copyright;
      serverV2FeedPackage.Created = searchResult.Created;
      serverV2FeedPackage.Dependencies = NuGetV2DependencyUtils.FlattenDependencies((IReadOnlyList<NuGetDependencyGroup>) metadata.DependencyGroups);
      serverV2FeedPackage.Description = metadata.Description;
      serverV2FeedPackage.DownloadCount = searchResult.DownloadCount;
      serverV2FeedPackage.IconUrl = this.iconUriCalculator.GetIconUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request1);
      serverV2FeedPackage.IsLatestVersion = searchResult.IsLatestVersion;
      serverV2FeedPackage.IsAbsoluteLatestVersion = searchResult.IsAbsoluteLatestVersion;
      serverV2FeedPackage.IsPrerelease = searchResult.PackageIdentity.Version.NuGetVersion.IsPrerelease;
      serverV2FeedPackage.Language = metadata.Language;
      serverV2FeedPackage.LastUpdated = searchResult.LastUpdated;
      serverV2FeedPackage.Published = dateTime;
      serverV2FeedPackage.PackageSize = searchResult.PackageSize;
      serverV2FeedPackage.ProjectUrl = metadata.ProjectUrl;
      serverV2FeedPackage.ReleaseNotes = metadata.ReleaseNotes;
      serverV2FeedPackage.RequireLicenseAcceptance = metadata.RequireLicenseAcceptance.GetValueOrDefault();
      serverV2FeedPackage.Summary = metadata.Summary;
      serverV2FeedPackage.Tags = metadata.Tags != null ? string.Join(" ", (IEnumerable<string>) metadata.Tags) : (string) null;
      serverV2FeedPackage.Title = metadata.Title;
      serverV2FeedPackage.MinClientVersion = metadata.MinClientVersion;
      serverV2FeedPackage.LastEdited = searchResult.LastEdited;
      serverV2FeedPackage.LicenseUrl = this.licenseUriCalculator.GetLicenseUriString((IPackageRequest<VssNuGetPackageIdentity, NuGetPackageMetadata>) request1);
      serverV2FeedPackage.Listed = metadata.Listed;
      return serverV2FeedPackage;
    }
  }
}
