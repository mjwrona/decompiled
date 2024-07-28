// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmVersionsController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "Versions", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  [RequestContentTypeRestriction(AllowJson = true, AllowJsonPatch = true)]
  public class NpmVersionsController : NpmApiController
  {
    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ControllerMethodTraceFilter(12000920)]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package), null, null)]
    public async Task<HttpResponseMessage> UpdatePackageAsync(
      string feedId,
      string packageName,
      string packageVersion,
      [FromBody] PackageVersionDetails packageVersionDetails)
    {
      return await this.UpdateScopedPackageAsync(feedId, (string) null, packageName, packageVersion, packageVersionDetails);
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ControllerMethodTraceFilter(12000930)]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package), null, null)]
    public async Task<HttpResponseMessage> UpdateScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      [FromBody] PackageVersionDetails packageVersionDetails)
    {
      NpmVersionsController versionsController = this;
      ArgumentUtility.CheckForNull<PackageVersionDetails>(packageVersionDetails, nameof (packageVersionDetails));
      int num = packageVersionDetails.DeprecateMessage != null ? 1 : 0;
      bool flag = !string.IsNullOrWhiteSpace(ViewUtils.GetViewFromPatchOperation(packageVersionDetails.Views));
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = versionsController.GetFeedRequest(feedId, readOnlyValidator);
      if (num != 0)
        NpmSecurityHelper.CheckDeprecatePackagePermissions(versionsController.TfsRequestContext, feedRequest.Feed);
      if (flag)
        FeedSecurityHelper.CheckAddPackagePermissions(versionsController.TfsRequestContext, feedRequest.Feed);
      SemanticVersion version;
      if (!NpmVersionUtils.TryParseNpmPackageVersion(packageVersion, out version))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageVersion((object) packageVersion));
      NpmPackageIdentity npmPackageIdentity = new NpmPackageIdentity(new NpmPackageName(packageScope, unscopedPackageName), version);
      versionsController.TfsRequestContext.SetPackageIdentityForPackagingTraces((IPackageIdentity) npmPackageIdentity);
      if ((num | (flag ? 1 : 0)) != 0)
      {
        NullResult nullResult = await ((IAsyncHandler<RawPackageRequest<PackageVersionDetails>, NullResult>) NpmAggregationResolver.Bootstrap(versionsController.TfsRequestContext).HandlerFor<IRawPackageRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new IngestRawPackageIfNotAlreadyIngestedBootstrapper(versionsController.TfsRequestContext, BlockedIdentityContext.Update))).ThenActuallyHandleWith<RawPackageRequest<PackageVersionDetails>, NullResult, NullResult>(new UpdatePackageVersionHandlerBootstrapper(versionsController.TfsRequestContext).Bootstrap()).TaskYieldOnException<RawPackageRequest<PackageVersionDetails>, NullResult>().Handle(new RawPackageRequest<PackageVersionDetails>(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion, packageVersionDetails));
      }
      return versionsController.Request.CreateResponse(HttpStatusCode.Accepted);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12000900)]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package), null, null)]
    public async Task<HttpResponseMessage> UnpublishPackageAsync(
      string feedId,
      string packageName,
      string packageVersion)
    {
      return await this.UnpublishScopedPackageAsync(feedId, (string) null, packageName, packageVersion);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12000910)]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package), null, null)]
    public async Task<HttpResponseMessage> UnpublishScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion)
    {
      NpmVersionsController versionsController = this;
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = versionsController.GetFeedRequest(feedId, readOnlyValidator);
      FeedSecurityHelper.CheckDeletePackagePermissions(versionsController.TfsRequestContext, feedRequest.Feed);
      NpmPackageName name = new NpmPackageName(packageScope, unscopedPackageName);
      SemanticVersion version;
      if (!NpmVersionUtils.TryParseNpmPackageVersion(packageVersion, out version))
        throw new Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions.InvalidPackageException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageVersion((object) packageVersion));
      NpmPackageIdentity packageIdentity = new NpmPackageIdentity(name, version);
      versionsController.TfsRequestContext.SetPackageIdentityForPackagingTraces((IPackageIdentity) packageIdentity);
      HttpResponseMessage response = versionsController.Request.CreateResponse<Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>(HttpStatusCode.OK, await ((IAsyncHandler<RawPackageRequest, NullResult>) NpmAggregationResolver.Bootstrap(versionsController.TfsRequestContext).HandlerFor<IRawPackageRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new IngestRawPackageIfNotAlreadyIngestedBootstrapper(versionsController.TfsRequestContext, BlockedIdentityContext.Delete))).ThenActuallyHandleWith<RawPackageRequest, NullResult, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>(new NpmDeleteRequestHandlerBootstrapper(versionsController.TfsRequestContext).Bootstrap()).TaskYieldOnException<RawPackageRequest, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>().Handle(new RawPackageRequest(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion)) ?? throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageVersionNotFound((object) packageIdentity.Name.FullName, (object) packageIdentity.Version.DisplayVersion, (object) feedId)));
      packageIdentity = (NpmPackageIdentity) null;
      return response;
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000940)]
    [ClientSwaggerOperationId("GetPackageVersion")]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetPackageInfoAsync(
      string feedId,
      string packageName,
      string packageVersion)
    {
      return await this.GetScopedPackageInfoAsync(feedId, (string) null, packageName, packageVersion);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000950)]
    [ClientSwaggerOperationId("GetScopedPackageVersion")]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetScopedPackageInfoAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion)
    {
      NpmVersionsController versionsController = this;
      IFeedRequest feedRequest = versionsController.GetFeedRequest(feedId);
      ISecuredObject securedObject = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed);
      Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package package = await new GetPackageVersionHandlerBootstrapper(versionsController.TfsRequestContext).Bootstrap().TaskYieldOnException<RawPackageRequest, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>().Handle(new RawPackageRequest(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion));
      package.SetSecuredObject(securedObject);
      HttpResponseMessage response = versionsController.Request.CreateResponse<Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>(HttpStatusCode.OK, package);
      securedObject = (ISecuredObject) null;
      return response;
    }
  }
}
