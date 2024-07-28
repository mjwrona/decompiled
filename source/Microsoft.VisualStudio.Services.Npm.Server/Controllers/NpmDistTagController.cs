// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmDistTagController
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
using Microsoft.VisualStudio.Services.Npm.Server.DistTag;
using Microsoft.VisualStudio.Services.Npm.Server.Upstreams;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [FeatureEnabled("Packaging.Npm.Service")]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "dist-tags", ResourceVersion = 1)]
  [ClientIgnore]
  public class NpmDistTagController : NpmApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12000500)]
    public async Task<HttpResponseMessage> GetDistTags(string feedId, string packageName) => await this.GetDistTags(feedId, (string) null, packageName);

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12000510)]
    public async Task<HttpResponseMessage> GetDistTags(
      string feedId,
      string packageScope,
      string unscopedPackageName)
    {
      NpmDistTagController distTagController = this;
      IFeedRequest feedRequest = distTagController.GetFeedRequest(feedId);
      RawPackageNameRequest request = new RawPackageNameRequest(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName));
      IDictionary<string, string> source = await new GetDistTagsHandlerBootstrapper(distTagController.TfsRequestContext).Bootstrap().Handle(request);
      SecuredDictionary<string, string> securedDictionary = new SecuredDictionary<string, string>(FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feedRequest.Feed));
      SecuredDictionary<string, string> dest = securedDictionary;
      source.Copy<string, string>((IDictionary<string, string>) dest);
      HttpResponseMessage distTags = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) new ObjectContent(typeof (SecuredDictionary<string, string>), (object) securedDictionary, (MediaTypeFormatter) new ServerVssJsonMediaTypeFormatter(true), "application/json")
      };
      feedRequest = (IFeedRequest) null;
      return distTags;
    }

    [HttpPut]
    [ControllerMethodTraceFilter(12000520)]
    public async Task<HttpResponseMessage> PutDistTag(
      string feedId,
      string packageName,
      string tag,
      [FromBody] string packageVersion)
    {
      return await this.PutDistTag(feedId, (string) null, packageName, tag, packageVersion);
    }

    [HttpPut]
    [ControllerMethodTraceFilter(12000530)]
    public async Task<HttpResponseMessage> PutDistTag(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string tag,
      [FromBody] string packageVersion)
    {
      NpmDistTagController distTagController = this;
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = distTagController.GetFeedRequest(feedId, readOnlyValidator);
      FeedSecurityHelper.CheckAddPackagePermissions(distTagController.TfsRequestContext, feedRequest.Feed);
      NpmPackageName name = new NpmPackageName(packageScope, unscopedPackageName);
      SemanticVersion version;
      if (!NpmVersionUtils.TryParseNpmPackageVersion(packageVersion, out version))
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_InvalidPackageVersion((object) packageVersion));
      distTagController.TfsRequestContext.SetPackageIdentityForPackagingTraces((IPackageIdentity) new NpmPackageIdentity(name, version));
      HttpResponseMessage httpResponseMessage = await ((IAsyncHandler<RawPackageRequest<string>, NullResult>) NpmAggregationResolver.Bootstrap(distTagController.TfsRequestContext).HandlerFor<IRawPackageRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<IRawPackageRequest, NullResult>>) new IngestRawPackageIfNotAlreadyIngestedBootstrapper(distTagController.TfsRequestContext, BlockedIdentityContext.Update))).ThenActuallyHandleWith<RawPackageRequest<string>, NullResult, HttpResponseMessage>(new SetDistTagRequestHandlerBootstrapper(distTagController.TfsRequestContext).Bootstrap()).TaskYieldOnException<RawPackageRequest<string>, HttpResponseMessage>().Handle(new RawPackageRequest<string>(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion, tag));
      return new HttpResponseMessage(HttpStatusCode.Accepted)
      {
        Content = distTagController.GetNpmSuccessMessageContent()
      };
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12000540)]
    public async Task<HttpResponseMessage> DeleteDistTag(
      string feedId,
      string packageName,
      string tag)
    {
      return await this.DeleteDistTag(feedId, (string) null, packageName, tag);
    }

    [HttpDelete]
    [ControllerMethodTraceFilter(12000550)]
    public async Task<HttpResponseMessage> DeleteDistTag(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string tag)
    {
      NpmDistTagController distTagController = this;
      IValidator<FeedCore> readOnlyValidator = NpmElementalFeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = distTagController.GetFeedRequest(feedId, readOnlyValidator);
      HttpResponseMessage httpResponseMessage = await new RemoveDistTagRequestHandlerBootstrapper(distTagController.TfsRequestContext).Bootstrap().TaskYieldOnException<RawPackageNameRequest<string>, HttpResponseMessage>().Handle(new RawPackageNameRequest<string>(feedRequest, RawNpmPackageName.Create(packageScope, unscopedPackageName), tag));
      return new HttpResponseMessage(HttpStatusCode.Accepted)
      {
        Content = distTagController.GetNpmSuccessMessageContent()
      };
    }
  }
}
