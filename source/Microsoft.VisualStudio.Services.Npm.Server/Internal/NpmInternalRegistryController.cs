// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Internal.NpmInternalRegistryController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.Controllers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Internal
{
  [ControllerApiVersion(2.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "registryInternal", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  public class NpmInternalRegistryController : NpmApiController
  {
    [HttpGet]
    [ControllerMethodTraceFilter(12000010)]
    [ClientLocationId("C6647BF1-BF4D-4BBF-AEDC-90136C1FB21A")]
    [ClientResponseType(typeof (JObject), null, null)]
    public async Task<HttpResponseMessage> GetUnscopedPackageInternalRegistrationAsync(
      string feedId,
      string packageName,
      Guid aadTenantId)
    {
      return await this.GetScopedPackageInternalRegistrationAsync(feedId, (string) null, packageName, aadTenantId);
    }

    [HttpGet]
    [ControllerMethodTraceFilter(12000010)]
    [ClientLocationId("D2113CCF-750A-4B45-86E8-198C4A27279D")]
    [ClientResponseType(typeof (JObject), null, null)]
    public async Task<HttpResponseMessage> GetScopedPackageInternalRegistrationAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      Guid aadTenantId)
    {
      NpmInternalRegistryController registryController = this;
      IFeedRequest feed = registryController.GetFeedRequest(feedId);
      new UpstreamVerificationHelperBootstrapper(registryController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(registryController.TfsRequestContext, feed.Feed, aadTenantId);
      string content = await NpmAggregationResolver.Bootstrap(registryController.TfsRequestContext).HandlerFor<RawPackageNameRequest, string>((IRequireAggBootstrapper<IAsyncHandler<RawPackageNameRequest, string>>) new GetPackageRegistrationHandlerBootstrapper(registryController.TfsRequestContext)).TaskYieldOnException<RawPackageNameRequest, string>().Handle(new RawPackageNameRequest(feed, RawNpmPackageName.Create(packageScope, unscopedPackageName)));
      ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(feed.Feed);
      Encoding utF8 = Encoding.UTF8;
      ISecuredObject securedObject = securedObjectReadOnly;
      VssServerStringContent serverStringContent = new VssServerStringContent(content, utF8, "application/json", (object) securedObject);
      registryController.TfsRequestContext.UpdateTimeToFirstPage();
      HttpResponseMessage registrationAsync = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = (HttpContent) serverStringContent
      };
      feed = (IFeedRequest) null;
      return registrationAsync;
    }
  }
}
