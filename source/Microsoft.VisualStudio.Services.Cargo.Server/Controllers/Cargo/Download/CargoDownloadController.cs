// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Download.CargoDownloadController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Aggregations;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamVersionList;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Cargo.Download
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "download")]
  [ErrorInReasonPhraseExceptionFilter]
  public class CargoDownloadController : CargoApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "application/octet-stream")]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<HttpResponseMessage> GetCrateFileAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string? extract = null)
    {
      CargoDownloadController downloadController = this;
      // ISSUE: reference to a compiler-generated method
      return await CargoAggregationResolver.Bootstrap(downloadController.TfsRequestContext).HandlerFor<IPackageFileRequest<CargoPackageIdentity>, HttpResponseMessage, IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IMetadataCacheService, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>>(new Func<IMetadataDocumentService<CargoPackageIdentity, ICargoMetadataEntry>, IMetadataCacheService, IUpstreamVersionListService<CargoPackageName, CargoPackageVersion>, IAsyncHandler<IPackageFileRequest<CargoPackageIdentity>, HttpResponseMessage>>(downloadController.\u003CGetCrateFileAsync\u003Eb__0_0)).Handle(string.IsNullOrWhiteSpace(extract) ? downloadController.GetPackageFileRequest(feedId, packageName, packageVersion) : (IPackageFileRequest<CargoPackageIdentity>) downloadController.GetPackageInnerFileRequest(feedId, packageName, packageVersion, extract));
    }
  }
}
