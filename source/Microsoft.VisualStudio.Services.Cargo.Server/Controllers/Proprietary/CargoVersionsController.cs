// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Proprietary.CargoVersionsController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.CiData;
using Microsoft.VisualStudio.Services.Cargo.Server.Metadata;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.Converters;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersion;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.Server.Telemetry;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.DeletePackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Proprietary
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Cargo")]
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "versions", ResourceVersion = 1)]
  public class CargoVersionsController : CargoApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Package), null, null)]
    [PackagingPublicProjectRequestRestrictions]
    public async Task<Package> GetPackageVersion(
      string feedId,
      string packageName,
      string packageVersion,
      bool showDeleted = false)
    {
      CargoVersionsController versionsController = this;
      Package packageVersion1;
      using (versionsController.EnterTracer(nameof (GetPackageVersion)))
      {
        GetPackageVersionHandler<CargoPackageIdentity, ICargoMetadataEntry, Package> packageVersionHandler = new GetPackageVersionHandler<CargoPackageIdentity, ICargoMetadataEntry, Package>((IConverter<ICargoMetadataEntry, Package>) new CargoMetadataEntryToPackageConverter(), new CargoMetadataHandlerBootstrapper(versionsController.TfsRequestContext).Bootstrap());
        PackageRequest<CargoPackageIdentity, ShowDeletedBool> packageRequest = versionsController.GetPackageRequest(feedId, packageName, packageVersion).WithData<CargoPackageIdentity, ShowDeletedBool>(new ShowDeletedBool(showDeleted));
        PackageRequest<CargoPackageIdentity, ShowDeletedBool> request = packageRequest;
        Package package = await packageVersionHandler.Handle(request);
        package.SetSecuredObject(FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(packageRequest.Feed));
        packageVersion1 = package;
      }
      return packageVersion1;
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientSwaggerOperationId("Update Package Version")]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdatePackageVersion(
      string feedId,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails)
    {
      CargoVersionsController versionsController = this;
      HttpResponseMessage response;
      using (versionsController.EnterTracer(nameof (UpdatePackageVersion)))
      {
        PackageRequest<CargoPackageIdentity, PackageVersionDetails> request = versionsController.GetPackageRequest(feedId, packageName, packageVersion).WithData<CargoPackageIdentity, PackageVersionDetails>(packageVersionDetails);
        IRequireAggHandlerBootstrapper<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData> requestToOpHandler = NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData>(new UpdatePackageVersionValidatingHandlerBootstrapper(versionsController.TfsRequestContext).Bootstrap());
        CargoUpdatePackageVersionCiDataHandler requestToCiHandler = new CargoUpdatePackageVersionCiDataHandler(versionsController.TfsRequestContext);
        NullResult nullResult = await new CargoWriteBootstrapper<PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData, NullResult>(versionsController.TfsRequestContext, requestToOpHandler, (IAsyncHandler<ICommitLogEntry, NullResult>) new ByFuncAsyncHandler<ICommitLogEntry, NullResult>((Func<ICommitLogEntry, NullResult>) (c => (NullResult) null)), (IAsyncHandler<(PackageRequest<CargoPackageIdentity, PackageVersionDetails>, ICommitOperationData), ICiData>) requestToCiHandler, false).Bootstrap().Handle(request);
        response = versionsController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }

    [HttpDelete]
    [ClientResponseType(typeof (Package), null, null)]
    public async Task<HttpResponseMessage> DeletePackageVersion(
      string feedId,
      string packageName,
      string packageVersion)
    {
      CargoVersionsController versionsController = this;
      NullResult nullResult = await new CargoWriteBootstrapper<IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData, NullResult>(versionsController.TfsRequestContext, (IRequireAggHandlerBootstrapper<IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData>) new CargoDeleteOpGeneratorBootstrapper(versionsController.TfsRequestContext), (IAsyncHandler<ICommitLogEntry, NullResult>) new DoNothingHandler<ICommitLogEntry>(), (IAsyncHandler<(IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>, IDeleteOperationData), ICiData>) new CargoDeletePackageCiDataFacadeHandler(versionsController.TfsRequestContext), false).Bootstrap().Handle((IPackageRequest<CargoPackageIdentity, DeleteRequestAdditionalData>) versionsController.GetPackageRequest(feedId, packageName, packageVersion).WithData<CargoPackageIdentity, DeleteRequestAdditionalData>(new DeleteRequestAdditionalData(DateTime.UtcNow)));
      return versionsController.Request.CreateResponse(HttpStatusCode.Accepted);
    }
  }
}
