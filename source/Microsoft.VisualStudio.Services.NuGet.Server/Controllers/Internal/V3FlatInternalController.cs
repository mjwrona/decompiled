// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Internal.V3FlatInternalController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Internal
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "flatInternal")]
  public class V3FlatInternalController : NuGetApiController
  {
    [HttpGet]
    [ClientLocationId("5CDF277C-D60B-4259-945C-83144FFA814C")]
    [ControllerMethodTraceFilter(5722500)]
    public async 
    #nullable disable
    Task<NuGetVersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      string feedId,
      string id,
      Guid aadTenantId)
    {
      V3FlatInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feedId);
      new UpstreamVerificationHelperBootstrapper(internalController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      NuGetVersionsExposedToDownstreamsResponse downstreamsAsync = await (await NuGetAggregationResolver.Bootstrap(internalController.TfsRequestContext).FactoryFor<IAsyncHandler<RawPackageNameRequest, NuGetVersionsExposedToDownstreamsResponse>>((IRequireAggBootstrapper<IAsyncHandler<RawPackageNameRequest, NuGetVersionsExposedToDownstreamsResponse>>) new InternalPackageVersionsHandlerBootstrapper(internalController.TfsRequestContext)).Get(feedRequest)).Handle(new RawPackageNameRequest(feedRequest, id));
      feedRequest = (IFeedRequest) null;
      return downstreamsAsync;
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetNuspecInternal", MediaType = "text/xml")]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetNupkgInternal", MediaType = "application/octet-stream")]
    [ClientLocationId("FA0B8197-5517-47A7-8992-F6F9CE86CD05")]
    [ControllerMethodTraceFilter(5722510)]
    public async Task<HttpResponseMessage> GetFileInternalAsync(
      string feedId,
      string id,
      string version,
      string file,
      Guid aadTenantId,
      string sourceProtocolVersion = null)
    {
      V3FlatInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feedId);
      new UpstreamVerificationHelperBootstrapper(internalController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      return await new GetFileAsyncBootstrapper(internalController.TfsRequestContext).Bootstrap().Handle((IRawPackageInnerFileRequest<NuGetGetFileData>) new RawPackageInnerFileRequest<NuGetGetFileData>(feedRequest, id, version, file, (string) null, new NuGetGetFileData(sourceProtocolVersion)));
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<RawPackageNameEntry>), null, null)]
    [ClientLocationId("52156C00-7656-4768-8E49-5F12E9347204")]
    [ControllerMethodTraceFilter(5722530)]
    public async Task<IEnumerable<RawPackageNameEntry>> GetNamesInternalAsync(
      string feedId,
      Guid aadTenantId)
    {
      V3FlatInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feedId);
      new UpstreamVerificationHelperBootstrapper(internalController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      IEnumerable<RawPackageNameEntry> list = (IEnumerable<RawPackageNameEntry>) (await (await NuGetAggregationResolver.Bootstrap(internalController.TfsRequestContext).FactoryFor<INuGetNamesService>().Get(feedRequest)).GetPackageNamesAsync(feedRequest)).Select<IPackageNameEntry<VssNuGetPackageName>, RawPackageNameEntry>((Func<IPackageNameEntry<VssNuGetPackageName>, RawPackageNameEntry>) (n => new RawPackageNameEntry()
      {
        Name = n.Name.NormalizedName,
        LastUpdatedDateTime = n.LastUpdatedDateTime
      })).ToList<RawPackageNameEntry>();
      feedRequest = (IFeedRequest) null;
      return list;
    }

    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<GetNuspecsInternalResponseNuspec>), null, null)]
    [ClientLocationId("387CDCB7-3FCF-4DFC-B8BC-1ABB5B4F4354")]
    [ControllerMethodTraceFilter(5722520)]
    public async Task<IEnumerable<GetNuspecsInternalResponseNuspec>> GetNuspecsInternalAsync(
      string feedId,
      string id,
      [FromBody] List<string> versions,
      Guid aadTenantId)
    {
      V3FlatInternalController internalController = this;
      IFeedRequest feedRequest = internalController.GetFeedRequest(feedId);
      new UpstreamVerificationHelperBootstrapper(internalController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(internalController.TfsRequestContext, feedRequest.Feed, aadTenantId);
      IConverter<string, VssNuGetPackageName> converter = new NuGetPackageNameParsingConverterBootstrapper(internalController.TfsRequestContext).Bootstrap();
      NuGetIdentityResolver identityResolver = NuGetIdentityResolver.Instance;
      return await new GetNuspecsInternalBootstrapper(internalController.TfsRequestContext).Bootstrap().ThenDelegateTo<NuGetGetNuspecsRequest, IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>, IEnumerable<GetNuspecsInternalResponseNuspec>>(ByFuncConverter.Create<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>, IEnumerable<GetNuspecsInternalResponseNuspec>>((Func<IReadOnlyDictionary<VssNuGetPackageVersion, ContentBytes>, IEnumerable<GetNuspecsInternalResponseNuspec>>) (returnedVersions => returnedVersions.Select<KeyValuePair<VssNuGetPackageVersion, ContentBytes>, GetNuspecsInternalResponseNuspec>((Func<KeyValuePair<VssNuGetPackageVersion, ContentBytes>, GetNuspecsInternalResponseNuspec>) (pair => new GetNuspecsInternalResponseNuspec()
      {
        DisplayVersion = pair.Key.DisplayVersion,
        Content = Convert.ToBase64String(pair.Value.Content),
        AreBytesCompressed = pair.Value.AreBytesCompressed
      }))))).Handle(new NuGetGetNuspecsRequest((IPackageNameRequest<VssNuGetPackageName>) new PackageNameRequest<VssNuGetPackageName>(feedRequest, converter.Convert(id)), versions.Select<string, VssNuGetPackageVersion>((Func<string, VssNuGetPackageVersion>) (x => identityResolver.ResolvePackageVersion(x)))));
    }
  }
}
