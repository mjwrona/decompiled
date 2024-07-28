// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.PackageVersionContentController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.PackagingApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("NuGet")]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "content", ResourceVersion = 1)]
  public class PackageVersionContentController : NuGetApiController
  {
    [HttpHead]
    [ClientIgnore]
    public async Task<HttpResponseMessage> CheckPackageExists(
      string feedId,
      string packageName,
      string packageVersion,
      string sourceProtocolVersion = null)
    {
      PackageVersionContentController contentController = this;
      ArgumentUtility.CheckForNull<string>(feedId, nameof (feedId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageName, nameof (packageName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(packageVersion, nameof (packageVersion));
      IFeedRequest feedRequest = contentController.GetFeedRequest(feedId);
      NullResult nullResult = await new NuGetFindPackageVersionHandlerBootstrapper(contentController.TfsRequestContext).Bootstrap().TaskYieldOnException<RawPackageRequest, NullResult>().Handle(new RawPackageRequest(feedRequest, packageName, packageVersion));
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> DownloadPackage(
      string feedId,
      string packageName,
      string packageVersion,
      string sourceProtocolVersion = null)
    {
      PackageVersionContentController contentController = this;
      IFeedRequest feedRequest = contentController.GetFeedRequest(feedId);
      HttpResponseMessage httpResponseMessage = await NuGetAggregationResolver.Bootstrap(contentController.TfsRequestContext).HandlerFor<RawPackageFileRequest<NuGetGetFileData>, HttpResponseMessage>((IRequireAggBootstrapper<IAsyncHandler<RawPackageFileRequest<NuGetGetFileData>, HttpResponseMessage>>) new PackageVersionDownloadPackageMetadataHandlerBootstrapper(contentController.TfsRequestContext)).Handle(new RawPackageFileRequest<NuGetGetFileData>(feedRequest, packageName, packageVersion, packageName + "." + packageVersion + ".nupkg", new NuGetGetFileData("none")));
      if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
        contentController.TfsRequestContext.UpdateTimeToFirstPage();
      return httpResponseMessage;
    }
  }
}
