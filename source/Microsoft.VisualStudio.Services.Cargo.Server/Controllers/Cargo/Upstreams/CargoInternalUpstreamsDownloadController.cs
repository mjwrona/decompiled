// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Upstreams.CargoInternalUpstreamsDownloadController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Upstreams
{
  [ControllerApiVersion(2.0)]
  [ClientGroupByResource("Cargo")]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "internalCargoUpstreamsDownload", ResourceVersion = 1)]
  public class CargoInternalUpstreamsDownloadController : CargoApiController
  {
    [HttpGet]
    [ClientLocationId("92BA229F-C577-4A76-97A0-0205CE939B99")]
    [ClientResponseType(typeof (Stream), null, null)]
    public async Task<HttpResponseMessage> GetPackageContentStreamAsync(
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId)
    {
      CargoInternalUpstreamsDownloadController downloadController = this;
      IPackageFileRequest<CargoPackageIdentity> packageFileRequest = downloadController.GetPackageFileRequest(feedId, packageName, packageVersion);
      new UpstreamVerificationHelperBootstrapper(downloadController.TfsRequestContext).Bootstrap().ThrowIfFeedIsNotWidelyVisible(downloadController.TfsRequestContext, packageFileRequest.Feed, aadTenantId);
      // ISSUE: reference to a compiler-generated method
      HttpResponseMessage contentStreamAsync = await (await CargoAggregationResolver.Bootstrap(downloadController.TfsRequestContext).FactoryFor<IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, HttpResponseMessage>, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IMetadataCacheService, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>(new Func<IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IMetadataCacheService, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>, IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, HttpResponseMessage>>(downloadController.\u003CGetPackageContentStreamAsync\u003Eb__0_0)).Get((IFeedRequest) packageFileRequest)).Handle(packageFileRequest);
      packageFileRequest = (IPackageFileRequest<CargoPackageIdentity>) null;
      return contentStreamAsync;
    }
  }
}
