// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Internal.NpmContentInternalController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.Controllers;
using Microsoft.VisualStudio.Services.Npm.Server.PackageDownload;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Internal
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "contentInternal", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  public class NpmContentInternalController : NpmApiController
  {
    protected virtual IUpstreamVerificationHelper UpstreamVerificationHelper => new UpstreamVerificationHelperBootstrapper(this.TfsRequestContext).Bootstrap();

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientLocationId("A498B0E6-CD4D-483E-B0C4-C8B8E98D9309")]
    [ControllerMethodTraceFilter(12001400)]
    public Task<HttpResponseMessage> GetContentUnscopedPackageInternalAsync(
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId)
    {
      return this.GetContentScopedPackageInternalAsync(feedId, (string) null, packageName, packageVersion, aadTenantId);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientLocationId("AFF4C6C3-A7FC-4793-BA76-5F21C714FCD3")]
    [ControllerMethodTraceFilter(12001400)]
    public virtual async Task<HttpResponseMessage> GetContentScopedPackageInternalAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      Guid aadTenantId)
    {
      NpmContentInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feedId);
      internalController.UpstreamVerificationHelper.ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      string fileName = unscopedPackageName + "-" + packageVersion + ".tgz";
      HttpResponseMessage packageInternalAsync = await NpmAggregationResolver.Bootstrap(internalController.TfsRequestContext).HandlerFor<RawNpmPackageNameWithFileRequest, HttpResponseMessage>((IRequireAggBootstrapper<IAsyncHandler<RawNpmPackageNameWithFileRequest, HttpResponseMessage>>) new NpmDownloadPackageFileHandlerBootstrapper(internalController.TfsRequestContext)).TaskYieldOnException<RawNpmPackageNameWithFileRequest, HttpResponseMessage>().Handle(new RawNpmPackageNameWithFileRequest(feedRequest, packageScope, unscopedPackageName, fileName));
      internalController.TfsRequestContext.UpdateTimeToFirstPage();
      return packageInternalAsync;
    }
  }
}
