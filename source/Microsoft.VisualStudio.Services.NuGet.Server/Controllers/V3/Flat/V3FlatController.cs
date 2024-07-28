// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3.Flat.V3FlatController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V3.Flat
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "flat2")]
  public class V3FlatController : NuGetApiController
  {
    [HttpGet]
    [ClientLocationId("A8C7FA0A-36CF-4D22-8516-9506FC229F5C")]
    [PackagingPublicProjectRequestRestrictions]
    [ControllerMethodTraceFilter(5722500)]
    public async Task<PackageVersionsResponse> GetIndexAsync(string feedId, string id)
    {
      V3FlatController v3FlatController = this;
      IFeedRequest feedRequest = v3FlatController.GetFeedRequest(feedId);
      return await NuGetAggregationResolver.Bootstrap(v3FlatController.TfsRequestContext).HandlerFor<RawPackageNameRequest, PackageVersionsResponse>((IRequireAggBootstrapper<IAsyncHandler<RawPackageNameRequest, PackageVersionsResponse>>) new V3PackageVersionsHandlerBootstrapper(v3FlatController.TfsRequestContext)).Handle(new RawPackageNameRequest(feedRequest, id));
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetNuspec", MediaType = "text/xml")]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetNupkg", MediaType = "application/octet-stream")]
    [PackagingPublicProjectRequestRestrictions]
    [ClientLocationId("76871FBD-8952-415B-9931-A80BC784EBCF")]
    [ControllerMethodTraceFilter(5722510)]
    public async Task<HttpResponseMessage> GetFileAsync(
      string feedId,
      string id,
      string version,
      string file,
      string sourceProtocolVersion = null,
      string extract = null)
    {
      V3FlatController v3FlatController = this;
      IFeedRequest feedRequest = v3FlatController.GetFeedRequest(feedId);
      return await new GetFileAsyncBootstrapper(v3FlatController.TfsRequestContext).Bootstrap().Handle((IRawPackageInnerFileRequest<NuGetGetFileData>) new RawPackageInnerFileRequest<NuGetGetFileData>(feedRequest, id, version, file, extract, new NuGetGetFileData(sourceProtocolVersion)));
    }
  }
}
