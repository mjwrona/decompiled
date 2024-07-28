// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmRecycleBinController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.RestoreToFeed;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "RecycleBinVersions", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  public class NpmRecycleBinController : NpmApiController
  {
    [HttpDelete]
    [ControllerMethodTraceFilter(12001700)]
    [ClientResponseType(typeof (void), null, null)]
    public async Task<HttpResponseMessage> DeletePackageVersionFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      return await this.DeleteScopedPackageVersionFromRecycleBin(feedId, (string) null, packageName, packageVersion);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12001710)]
    [ClientResponseType(typeof (void), null, null)]
    public async Task<HttpResponseMessage> DeleteScopedPackageVersionFromRecycleBin(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion)
    {
      NpmRecycleBinController recycleBinController = this;
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId, readOnlyValidator);
      HttpResponseMessage httpResponseMessage = await new NpmPermanentDeleteRequestHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().TaskYieldOnException<RawPackageRequest, HttpResponseMessage>().Handle(new RawPackageRequest(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion));
      return new HttpResponseMessage(HttpStatusCode.Accepted);
    }

    [HttpPatch]
    [ControllerMethodTraceFilter(12001720)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> RestorePackageVersionFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion,
      NpmRecycleBinPackageVersionDetails packageVersionDetails)
    {
      return await this.RestoreScopedPackageVersionFromRecycleBin(feedId, (string) null, packageName, packageVersion, packageVersionDetails);
    }

    [HttpPatch]
    [ControllerMethodTraceFilter(12001730)]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> RestoreScopedPackageVersionFromRecycleBin(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      NpmRecycleBinPackageVersionDetails packageVersionDetails)
    {
      NpmRecycleBinController recycleBinController = this;
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId, readOnlyValidator);
      return await new NpmRestoreToFeedHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().TaskYieldOnException<RawPackageRequest<IRecycleBinPackageVersionDetails>, HttpResponseMessage>().Handle(new RawPackageRequest<IRecycleBinPackageVersionDetails>(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion, (IRecycleBinPackageVersionDetails) packageVersionDetails));
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12001740)]
    [ClientSwaggerOperationId("GetPackageVersionFromRecycleBin")]
    [ClientResponseType(typeof (NpmPackageVersionDeletionState), null, null)]
    public async Task<NpmPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      return await this.GetScopedPackageVersionMetadataFromRecycleBin(feedId, (string) null, packageName, packageVersion);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12001750)]
    [ClientSwaggerOperationId("GetScopedPackageVersionFromRecycleBin")]
    [ClientResponseType(typeof (NpmPackageVersionDeletionState), null, null)]
    public async Task<NpmPackageVersionDeletionState> GetScopedPackageVersionMetadataFromRecycleBin(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion)
    {
      NpmRecycleBinController recycleBinController = this;
      IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId);
      Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package package = await new GetPackageVersionHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().TaskYieldOnException<RawPackageRequest, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>().Handle(new RawPackageRequest(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion));
      if (!package.UnpublishedDate.HasValue || package.PermanentlyDeletedDate.HasValue)
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageVersionNotFoundInRecycleBin((object) (package.Name + " " + package.Version)));
      return package.ToNpmPackageVersionDeletionState();
    }
  }
}
