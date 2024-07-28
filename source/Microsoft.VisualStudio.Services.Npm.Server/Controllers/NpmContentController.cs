// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmContentController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.Aggregations;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.PackageDownload;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "content", ResourceVersion = 1)]
  [FeatureEnabled("Packaging.Npm.Service")]
  public class NpmContentController : NpmApiController
  {
    [HttpHead]
    [ClientIgnore]
    public async Task<HttpResponseMessage> CheckPackageExists(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion)
    {
      NpmContentController contentController = this;
      ArgumentUtility.CheckForNull<string>(feedId, nameof (feedId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(unscopedPackageName, nameof (unscopedPackageName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageVersion, nameof (packageVersion));
      NpmPackageIdentity data = new NpmPackageIdentity(new NpmPackageName(packageScope, unscopedPackageName), SemanticVersion.Parse(packageVersion));
      IFeedRequest feedRequest = contentController.GetFeedRequest(feedId);
      NullResult nullResult = await NpmAggregationResolver.Bootstrap(contentController.TfsRequestContext).HandlerFor<RawPackageRequest, NullResult>((IRequireAggBootstrapper<IAsyncHandler<RawPackageRequest, NullResult>>) new NpmFindPackageVersionHandlerBootstrapper(contentController.TfsRequestContext)).TaskYieldOnException<RawPackageRequest, NullResult>().Handle((RawPackageRequest) new RawPackageRequest<NpmPackageIdentity>(feedRequest, data.Name.FullName, packageVersion, data));
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    [HttpGet]
    [HttpHead]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientSwaggerOperationId("DownloadPackage")]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12001400)]
    public async Task<HttpResponseMessage> GetContentUnscopedPackageAsync(
      string feedId,
      string packageName,
      string packageVersion)
    {
      NpmContentController contentController = this;
      return contentController.TfsRequestContext.HttpMethod() == HttpMethod.Head.ToString() ? await contentController.CheckPackageExists(feedId, (string) null, packageName, packageVersion) : await contentController.GetContentScopedPackageAsync(feedId, (string) null, packageName, packageVersion);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [ClientSwaggerOperationId("DownloadScopedPackage")]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(12001400)]
    public virtual async Task<HttpResponseMessage> GetContentScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion)
    {
      NpmContentController contentController = this;
      ArgumentUtility.CheckStringForNullOrEmpty(unscopedPackageName, nameof (unscopedPackageName));
      ArgumentUtility.CheckForNull<string>(packageVersion, nameof (packageVersion));
      IFeedRequest feedRequest = contentController.GetFeedRequest(feedId);
      string fileName = unscopedPackageName + "-" + packageVersion + ".tgz";
      return await NpmAggregationResolver.Bootstrap(contentController.TfsRequestContext).HandlerFor<RawNpmPackageNameWithFileRequest, HttpResponseMessage>((IRequireAggBootstrapper<IAsyncHandler<RawNpmPackageNameWithFileRequest, HttpResponseMessage>>) new NpmDownloadPackageFileHandlerBootstrapper(contentController.TfsRequestContext)).TaskYieldOnException<RawNpmPackageNameWithFileRequest, HttpResponseMessage>().Handle(new RawNpmPackageNameWithFileRequest(feedRequest, packageScope, unscopedPackageName, fileName));
    }
  }
}
