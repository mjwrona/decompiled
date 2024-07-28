// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmReadmeController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.Readme;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "readme", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  public class NpmReadmeController : NpmApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ClientSwaggerOperationId("GetPackageReadme")]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12001300)]
    public Task<HttpResponseMessage> GetReadmeUnscopedPackageAsync(
      string feedId,
      string packageName,
      string packageVersion)
    {
      return this.GetReadmeScopedPackageAsync(feedId, (string) null, packageName, packageVersion);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    [ClientSwaggerOperationId("GetScopedPackageReadme")]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12001300)]
    public virtual async Task<HttpResponseMessage> GetReadmeScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion)
    {
      NpmReadmeController readmeController = this;
      ArgumentUtility.CheckForNull<string>(feedId, nameof (feedId));
      ArgumentUtility.CheckForNull<string>(unscopedPackageName, nameof (unscopedPackageName));
      ArgumentUtility.CheckForNull<string>(packageVersion, nameof (packageVersion));
      RawPackageRequest packageRequest = new RawPackageRequest(readmeController.GetFeedRequest(feedId), RawNpmPackageName.Create(packageScope, unscopedPackageName), packageVersion);
      Stream content = await NpmAggregationResolver.Bootstrap(readmeController.TfsRequestContext).HandlerFor<RawPackageRequest, Stream>((IRequireAggBootstrapper<IAsyncHandler<RawPackageRequest, Stream>>) new GetReadmeHandlerBootstrapper(readmeController.TfsRequestContext)).TaskYieldOnException<RawPackageRequest, Stream>().Handle(packageRequest);
      string fileName = Path.GetFileName("/readme.md");
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(packageRequest.Feed);
      VssServerStreamContent serverStreamContent = new VssServerStreamContent(content, (object) securedObjectReadOnly);
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) serverStreamContent
      };
      httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
      httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = fileName
      };
      HttpResponseMessage scopedPackageAsync = httpResponseMessage;
      packageRequest = (RawPackageRequest) null;
      return scopedPackageAsync;
    }
  }
}
