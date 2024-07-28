// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.V2PackagesController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3;
using Microsoft.VisualStudio.Services.NuGet.Server.Search3.QueryRepresentation;
using Microsoft.VisualStudio.Services.NuGet.Server.Telemetry;
using Microsoft.VisualStudio.Services.NuGet.Server.Utils;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  [ClientIgnore]
  [ODataRouting]
  [ODataFormatting]
  [SecureNuGetODataFormatting]
  [ValidateModel]
  public class V2PackagesController : NuGetApiController
  {
    private const string ProtocolVersion = "v2";

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722300)]
    public async Task<V2FeedPackage> GetV2FeedPackage(
      ODataQueryOptions<V2FeedPackage> options,
      string feedId,
      string id,
      string version)
    {
      V2PackagesController packagesController = this;
      IFeedRequest feedRequest = packagesController.GetFeedRequest(feedId);
      return await NuGetAggregationResolver.Bootstrap(packagesController.TfsRequestContext).HandlerFor<RawV2PackageRequest, V2FeedPackage>((IRequireAggBootstrapper<IAsyncHandler<RawV2PackageRequest, V2FeedPackage>>) new V2PackageBlobHandlerBootstrapper(packagesController.TfsRequestContext)).Handle(new RawV2PackageRequest(feedRequest, id, version, options));
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722310)]
    public async Task<IEnumerable<V2FeedPackage>> FindPackagesById(
      ODataQueryOptions<V2FeedPackage> options,
      string feedId,
      [FromODataUri] string id)
    {
      V2PackagesController packagesController = this;
      IFeedRequest feedRequest = packagesController.GetFeedRequest(feedId);
      return await NuGetAggregationResolver.Bootstrap(packagesController.TfsRequestContext).HandlerFor<RawV2PackageNameRequest, IEnumerable<V2FeedPackage>>((IRequireAggBootstrapper<IAsyncHandler<RawV2PackageNameRequest, IEnumerable<V2FeedPackage>>>) new V2FindPackagesByIdBlobHandlerBootstrapper(packagesController.TfsRequestContext)).Handle(new RawV2PackageNameRequest(feedRequest, id, options));
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722330)]
    public async Task<IHttpActionResult> FindPackagesByIdCount(
      ODataQueryOptions<V2FeedPackage> options,
      string feedId,
      [FromODataUri] string id)
    {
      V2PackagesController packagesController = this;
      IEnumerable<V2FeedPackage> packagesById = await packagesController.FindPackagesById(options, feedId, id);
      ISecuredObject objectFromResult = packagesController.GetSecureObjectFromResult(packagesById, feedId);
      return (IHttpActionResult) new PlainTextResult(packagesById.Count<V2FeedPackage>().ToString(), packagesController.Request, objectFromResult);
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722320)]
    public async Task<IEnumerable<V2FeedPackage>> Search(
      ODataQueryOptions<V2FeedPackage> options,
      string feedId,
      [FromODataUri] string searchTerm = "",
      [FromODataUri] string targetFramework = "",
      [FromODataUri] bool includePrerelease = false,
      string semVerLevel = null)
    {
      V2PackagesController packagesController = this;
      IFeedRequest feedRequest = packagesController.GetFeedRequest(feedId);
      IReadOnlyList<ServerV2FeedPackage> packages = await packagesController.SearchCore(feedRequest, options, searchTerm, includePrerelease, false, semVerLevel);
      IEnumerable<V2FeedPackage> v2FeedPackages = (IEnumerable<V2FeedPackage>) packagesController.PrepareRegularResult(feedRequest, packages);
      feedRequest = (IFeedRequest) null;
      return v2FeedPackages;
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722340)]
    public async Task<IHttpActionResult> SearchCount(
      ODataQueryOptions<V2FeedPackage> options,
      string feedId,
      [FromODataUri] string searchTerm = "",
      [FromODataUri] string targetFramework = "",
      [FromODataUri] bool includePrerelease = false,
      string semVerLevel = null)
    {
      V2PackagesController packagesController = this;
      IFeedRequest feedRequest = packagesController.GetFeedRequest(feedId);
      IReadOnlyList<ServerV2FeedPackage> packages = await packagesController.SearchCore(feedRequest, options, searchTerm, includePrerelease, false, semVerLevel);
      IHttpActionResult httpActionResult = packagesController.PrepareCountResult(feedRequest, packages);
      feedRequest = (IFeedRequest) null;
      return httpActionResult;
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722350)]
    public async Task<IEnumerable<V2FeedPackage>> GetPackages(
      ODataQueryOptions<V2FeedPackage> options,
      string feedId,
      string semVerLevel = null)
    {
      V2PackagesController packagesController = this;
      IFeedRequest feedRequest = packagesController.GetFeedRequest(feedId);
      IReadOnlyList<ServerV2FeedPackage> packages1 = await packagesController.SearchCore(feedRequest, options, (string) null, true, true, semVerLevel);
      IEnumerable<V2FeedPackage> packages2 = (IEnumerable<V2FeedPackage>) packagesController.PrepareRegularResult(feedRequest, packages1);
      feedRequest = (IFeedRequest) null;
      return packages2;
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722360)]
    public async Task<IHttpActionResult> GetCount(
      ODataQueryOptions<V2FeedPackage> options,
      string feedId,
      string semVerLevel = null)
    {
      V2PackagesController packagesController = this;
      IFeedRequest feedRequest = packagesController.GetFeedRequest(feedId);
      IReadOnlyList<ServerV2FeedPackage> packages = await packagesController.SearchCore(feedRequest, options, (string) null, true, true, semVerLevel);
      IHttpActionResult count = packagesController.PrepareCountResult(feedRequest, packages);
      feedRequest = (IFeedRequest) null;
      return count;
    }

    private async Task<IReadOnlyList<ServerV2FeedPackage>> SearchCore(
      IFeedRequest feedRequest,
      ODataQueryOptions<V2FeedPackage> options,
      string searchTerm,
      bool includePrerelease,
      bool includeDelisted,
      string semVerLevel)
    {
      V2PackagesController packagesController = this;
      SkipQueryOption skip1 = options.Skip;
      int skip2 = skip1 != null ? skip1.Value : 0;
      TopQueryOption top = options.Top;
      int take = top != null ? top.Value : 100;
      if (skip2 < 0)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_ArgumentOutOfRange((object) "$skip"));
      if (take < 0 || take > 10000)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_ArgumentOutOfRange((object) "$top"));
      NuGetSearchQueryInputs data = new NuGetSearchQueryInputs(searchTerm, options.Filter?.RawValue, options.OrderBy?.RawValue, includeDelisted, includePrerelease, semVerLevel, skip2, take, NuGetSearchResultShape.Versions, false);
      NuGetExtractInnerFileFromNupkgUriCalculator innerFileUriCalculator = new NuGetExtractInnerFileFromNupkgUriCalculator(packagesController.TfsRequestContext.GetLocationFacade());
      packagesController.TfsRequestContext.GetFeatureFlagFacade();
      IAsyncHandler<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo> handler1 = new NuGetPackageSearch3Bootstrapper(packagesController.TfsRequestContext, false).Bootstrap();
      V2SearchResultBuilderHandler handler2 = new V2SearchResultBuilderHandler((INuGetLicenseUriCalculator) new NuGetLicenseUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator), (INuGetIconUriCalculator) new NuGetIconUriCalculator((INuGetExtractInnerFileFromNupkgUriCalculator) innerFileUriCalculator));
      List<ServerV2FeedPackage> list = (await handler1.KeepInput<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo, IFeedRequest<NuGetSearchResultsInfo>>((Func<FeedRequest<NuGetSearchQueryInputs>, NuGetSearchResultsInfo, IFeedRequest<NuGetSearchResultsInfo>>) ((searchRequest, response) => searchRequest.WithData<NuGetSearchResultsInfo>(response))).ThenDelegateTo<FeedRequest<NuGetSearchQueryInputs>, IFeedRequest<NuGetSearchResultsInfo>, IEnumerable<ServerV2FeedPackage>>((IAsyncHandler<IFeedRequest<NuGetSearchResultsInfo>, IEnumerable<ServerV2FeedPackage>>) handler2).Handle(new FeedRequest<NuGetSearchQueryInputs>(feedRequest, data))).ToList<ServerV2FeedPackage>();
      packagesController.PublishSearchTelemetry(options, feedRequest.Feed);
      return (IReadOnlyList<ServerV2FeedPackage>) list;
    }

    private IEnumerable<ServerV2FeedPackage> PrepareRegularResult(
      IFeedRequest feedRequest,
      IReadOnlyList<ServerV2FeedPackage> packages)
    {
      V2PackagesController.PopulateDownloadUrls(this.TfsRequestContext, feedRequest, packages);
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed);
      IEnumerable<ServerV2FeedPackage> securedObject = packages.ToSecuredObject<ServerV2FeedPackage>(securedObjectReadOnly);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return securedObject;
    }

    private IHttpActionResult PrepareCountResult(
      IFeedRequest feedRequest,
      IReadOnlyList<ServerV2FeedPackage> packages)
    {
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed);
      return (IHttpActionResult) new PlainTextResult(packages.Count.ToString(), this.Request, securedObjectReadOnly);
    }

    protected virtual void PublishSearchTelemetry(
      ODataQueryOptions<V2FeedPackage> options,
      FeedCore feed)
    {
      QueryPackageCiData nuGetQueryCiData = NuGetCiDataFactory.GetNuGetQueryCiData(this.TfsRequestContext, "v2", feed);
      new NuGetPackagingTelemetryBuilder().Build(this.TfsRequestContext).Publish(this.TfsRequestContext, (ICiData) nuGetQueryCiData);
    }

    private static void PopulateDownloadUrls(
      IVssRequestContext requestContext,
      IFeedRequest feedRequest,
      IReadOnlyList<ServerV2FeedPackage> packages)
    {
      IDictionary<IPackageIdentity, Uri> redirectSourceUris = NuGetUriBuilder.GetPackageContentRedirectSourceUris(requestContext, new PackageIdentityBatchRequest(feedRequest, (ICollection<IPackageIdentity>) packages.Select<ServerV2FeedPackage, VssNuGetPackageIdentity>((Func<ServerV2FeedPackage, VssNuGetPackageIdentity>) (p => p.PackageIdentity)).ToArray<VssNuGetPackageIdentity>()), "v2");
      foreach (ServerV2FeedPackage package in (IEnumerable<ServerV2FeedPackage>) packages)
        package.DownloadUrl = redirectSourceUris[(IPackageIdentity) package.PackageIdentity];
    }

    private ISecuredObject GetSecureObjectFromResult(
      IEnumerable<V2FeedPackage> packageList,
      string feedId)
    {
      return (ISecuredObject) packageList.FirstOrDefault<V2FeedPackage>() ?? FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(this.GetFeedRequest(feedId).Feed);
    }
  }
}
