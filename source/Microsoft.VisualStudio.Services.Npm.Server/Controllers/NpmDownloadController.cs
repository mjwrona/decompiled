// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmDownloadController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.PackageDownload;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "-", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  [ClientIgnore]
  public class NpmDownloadController : NpmApiController
  {
    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12000010)]
    public Task<HttpResponseMessage> GetPackageAsync(
      string feedId,
      string packageName,
      string packageFileName)
    {
      return this.GetPackageAsync(feedId, (string) null, packageName, packageFileName);
    }

    [HttpGet]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12000010)]
    public virtual async Task<HttpResponseMessage> GetPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageFileName)
    {
      NpmDownloadController downloadController = this;
      IFeedRequest feedRequest = downloadController.GetFeedRequest(feedId);
      return await NpmAggregationResolver.Bootstrap(downloadController.TfsRequestContext).HandlerFor<RawNpmPackageNameWithFileRequest, HttpResponseMessage>((IRequireAggBootstrapper<IAsyncHandler<RawNpmPackageNameWithFileRequest, HttpResponseMessage>>) new NpmDownloadPackageFileHandlerBootstrapper(downloadController.TfsRequestContext)).TaskYieldOnException<RawNpmPackageNameWithFileRequest, HttpResponseMessage>().Handle(new RawNpmPackageNameWithFileRequest(feedRequest, packageScope, unscopedPackageName, packageFileName));
    }
  }
}
