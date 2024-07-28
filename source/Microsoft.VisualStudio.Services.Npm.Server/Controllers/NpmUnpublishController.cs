// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmUnpublishController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Parsing;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Metadata;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Npm.Server.NpmPackageMetadata;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "-rev", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  [ClientIgnore]
  [RequestContentTypeRestriction(AllowJson = true, AllowJsonPatch = true)]
  public class NpmUnpublishController : NpmApiController
  {
    [HttpDelete]
    [ControllerMethodTraceFilter(12000800)]
    public async Task<HttpResponseMessage> DeletePackageAsync(
      string feedId,
      string packageName,
      string clientRevision)
    {
      return await this.DeleteScopedPackageAsync(feedId, (string) null, packageName, clientRevision);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12000810)]
    public async Task<HttpResponseMessage> DeleteScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string clientRevision)
    {
      return await this.UnpublishScopedPackageAsync(feedId, packageScope, unscopedPackageName, clientRevision, (Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata) null);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(12000800)]
    public async Task<HttpResponseMessage> UnpublishPackageAsync(
      string feedId,
      string packageName,
      string clientRevision,
      [FromBody] Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata metadata)
    {
      return await this.UnpublishScopedPackageAsync(feedId, (string) null, packageName, clientRevision, metadata);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(12000810)]
    public async Task<HttpResponseMessage> UnpublishScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string clientRevision,
      [FromBody] Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata metadata)
    {
      NpmUnpublishController unpublishController = this;
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = unpublishController.GetFeedRequest(feedId, readOnlyValidator);
      FeedCore feed = feedRequest.Feed;
      FeedSecurityHelper.CheckDeletePackagePermissions(unpublishController.TfsRequestContext, feed);
      NpmPackageName packageName = new NpmPackageName(packageScope, unscopedPackageName);
      unpublishController.TfsRequestContext.SetPackageNameForPackagingTraces((IPackageName) packageName);
      IAsyncHandler<PackageNameRequest<NpmPackageName, RevisionAndVersions>, RawPackageRequest> currentHandler1 = new IdentityResolverForDeleteHandlerBootstrapper(unpublishController.TfsRequestContext).Bootstrap();
      IAsyncHandler<IRawPackageRequest, NullResult> currentHandler2 = NpmAggregationResolver.Bootstrap(unpublishController.TfsRequestContext).HandlerFor<IRawPackageRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new IngestRawPackageIfNotAlreadyIngestedBootstrapper(unpublishController.TfsRequestContext, BlockedIdentityContext.Delete));
      PackageNameRequest<NpmPackageName> packageNameRequest = new PackageNameRequest<NpmPackageName>(feedRequest, packageName);
      RevisionAndVersions data = new RevisionAndVersions();
      data.Revision = clientRevision;
      Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata packageMetadata = metadata;
      List<string> stringList;
      if (packageMetadata == null)
      {
        stringList = (List<string>) null;
      }
      else
      {
        IDictionary<string, VersionMetadata> versions = packageMetadata.Versions;
        if (versions == null)
        {
          stringList = (List<string>) null;
        }
        else
        {
          ICollection<string> keys = versions.Keys;
          stringList = keys != null ? keys.ToList<string>() : (List<string>) null;
        }
      }
      if (stringList == null)
        stringList = new List<string>();
      data.Versions = stringList;
      PackageNameRequest<NpmPackageName, RevisionAndVersions> request = packageNameRequest.WithData<NpmPackageName, RevisionAndVersions>(data);
      return await currentHandler1.ThenDelegateTo<PackageNameRequest<NpmPackageName, RevisionAndVersions>, RawPackageRequest, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>(((IAsyncHandler<RawPackageRequest, NullResult>) currentHandler2).ThenActuallyHandleWith<RawPackageRequest, NullResult, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>((IAsyncHandler<RawPackageRequest, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>) new PropagateNullHandler<RawPackageRequest, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>(new NpmDeleteRequestHandlerBootstrapper(unpublishController.TfsRequestContext).Bootstrap()))).TaskYieldOnException<PackageNameRequest<NpmPackageName, RevisionAndVersions>, Microsoft.VisualStudio.Services.Npm.WebApi.Types.API.Package>().Handle(request) != null ? unpublishController.Request.CreateResponse(HttpStatusCode.Accepted) : unpublishController.Request.CreateResponse(HttpStatusCode.OK);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12000820)]
    public async Task<HttpResponseMessage> DeleteTarballAsync(
      string feedId,
      string packageName,
      string packageFileName,
      string clientRevision,
      [FromBody] Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata metadata)
    {
      return await this.DeleteScopedTarballAsync(feedId, (string) null, packageName, packageFileName, clientRevision, metadata);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12000830)]
    public async Task<HttpResponseMessage> DeleteScopedTarballAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageFileName,
      string clientRevision,
      [FromBody] Microsoft.VisualStudio.Services.Npm.WebApi.Types.PackageMetadata metadata)
    {
      NpmUnpublishController unpublishController = this;
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = unpublishController.GetFeedRequest(feedId, readOnlyValidator);
      FeedCore feed = feedRequest.Feed;
      FeedSecurityHelper.CheckDeletePackagePermissions(unpublishController.TfsRequestContext, feed);
      RawNpmPackageNameWithFileRequestConverter requestConverter = new RawNpmPackageNameWithFileRequestConverter();
      PackageRequest<NpmPackageIdentity> request = new NpmRawPackageRequestToRequestConverterBootstrapper(unpublishController.TfsRequestContext).Bootstrap().Convert((IRawPackageRequest) requestConverter.Convert(new RawNpmPackageNameWithFileRequest(feedRequest, packageScope, unscopedPackageName, packageFileName)));
      NpmPackageIdentity packageIdentity = request.PackageId;
      INpmMetadataEntry npmMetadataEntry = await new NpmMetadataHandlerBootstrapper(unpublishController.TfsRequestContext).Bootstrap().Handle((IPackageRequest<NpmPackageIdentity>) request);
      if (npmMetadataEntry == null)
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_PackageVersionNotFound((object) packageIdentity.Name.FullName, (object) packageIdentity.Version.DisplayVersion, (object) feedId));
      HttpResponseMessage httpResponseMessage = !npmMetadataEntry.DeletedDate.HasValue ? unpublishController.Request.CreateResponse(HttpStatusCode.Accepted) : unpublishController.Request.CreateResponse(HttpStatusCode.NoContent);
      packageIdentity = (NpmPackageIdentity) null;
      return httpResponseMessage;
    }
  }
}
