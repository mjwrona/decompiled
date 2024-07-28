// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Internal.NpmVersionsInternalController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Controllers;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.PackageVersionsExposedToDownstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Internal
{
  [ControllerApiVersion(2.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "VersionsInternal", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  [RequestContentTypeRestriction(AllowJson = true, AllowJsonPatch = true)]
  public class NpmVersionsInternalController : NpmApiController
  {
    [HttpGet]
    [ControllerMethodTraceFilter(12000940)]
    [ClientLocationId("74B696CD-6FB6-43E9-B049-D4B39FA47B2F")]
    [ClientResponseType(typeof (Package), null, null)]
    public async Task<HttpResponseMessage> GetPackageInfoInternalAsync(
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId)
    {
      return await this.GetScopedPackageInfoInternalAsync(feedId, (string) null, packageName, packageVersion, aadTenantId);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000950)]
    [ClientLocationId("DFC3B38F-B4A2-490A-B388-214DA92FE11C")]
    [ClientResponseType(typeof (Package), null, null)]
    public async Task<HttpResponseMessage> GetScopedPackageInfoInternalAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      Guid aadTenantId)
    {
      NpmVersionsInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feedId);
      new UpstreamVerificationHelperBootstrapper(internalController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      Package package = await new GetPackageVersionHandlerBootstrapper(internalController.TfsRequestContext).Bootstrap().TaskYieldOnException<RawPackageRequest, Package>().Handle(new RawPackageRequest(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion));
      return internalController.Request.CreateResponse<Package>(HttpStatusCode.OK, package);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000960)]
    [ClientLocationId("74B696CD-6FB6-43E9-B049-D4B39FA47B2F")]
    public async Task<VersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      string feedId,
      string packageName,
      Guid aadTenantId)
    {
      return await this.GetScopedPackageVersionsExposedToDownstreamsAsync(feedId, (string) null, packageName, aadTenantId);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000970)]
    [ClientIgnore]
    public async Task<VersionsExposedToDownstreamsResponse> GetScopedPackageVersionsExposedToDownstreamsAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      Guid aadTenantId)
    {
      NpmVersionsInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feedId);
      new UpstreamVerificationHelperBootstrapper(internalController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed);
      GetPackageVersionsExposedToDownstreamForApiHandlerBootstrapper<NpmPackageIdentity, NpmPackageName, SemanticVersion, INpmMetadataEntry> singleBootstrapper = new GetPackageVersionsExposedToDownstreamForApiHandlerBootstrapper<NpmPackageIdentity, NpmPackageName, SemanticVersion, INpmMetadataEntry>(new NpmRawPackageNameRequestToRequestConverterBootstrapper(internalController.TfsRequestContext).Bootstrap());
      VersionsExposedToDownstreamsResponse downstreamsResponse = await NpmAggregationResolver.Bootstrap(internalController.TfsRequestContext).HandlerFor<IRawPackageNameRequest, VersionsExposedToDownstreamsResponse>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageNameRequest, VersionsExposedToDownstreamsResponse>>) singleBootstrapper).Handle((IRawPackageNameRequest) new RawPackageNameRequest(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName)));
      downstreamsResponse.SetSecuredObject(securedObject);
      VersionsExposedToDownstreamsResponse downstreamsAsync = downstreamsResponse;
      securedObject = (ISecuredObject) null;
      return downstreamsAsync;
    }
  }
}
