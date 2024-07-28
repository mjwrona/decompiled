// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Proprietary.CargoRecycleBinController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.Metadata;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.Converters;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.RestoreToFeed;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
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
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "RecycleBinVersions", ResourceVersion = 1)]
  public class CargoRecycleBinController : CargoApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public async Task<HttpResponseMessage> DeletePackageVersionFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      CargoRecycleBinController recycleBinController = this;
      HttpResponseMessage httpResponseMessage;
      using (recycleBinController.EnterTracer(nameof (DeletePackageVersionFromRecycleBin)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId, readOnlyValidator);
        RawPackageRequestConverter<CargoPackageIdentity> converter = new RawPackageRequestConverter<CargoPackageIdentity>(recycleBinController.IdentityResolver.GetRawPackageRequestToIdentityConverter<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, SimplePackageFileName>(recycleBinController.TfsRequestContext));
        CargoPermanentDeleteValidatingOpGeneratingHandler handler1 = new CargoPermanentDeleteValidatingOpGeneratingHandler(new CargoMetadataHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap());
        IAsyncHandler<PackageRequest<CargoPackageIdentity>, HttpResponseMessage> handler2 = new CargoWriteBootstrapper<PackageRequest<CargoPackageIdentity>, IPermanentDeleteOperationData, HttpResponseMessage>(recycleBinController.TfsRequestContext, NoAggregationRequiredReturnSameInstanceBootstrapper.Create<PackageRequest<CargoPackageIdentity>, IPermanentDeleteOperationData>((IAsyncHandler<PackageRequest<CargoPackageIdentity>, IPermanentDeleteOperationData>) handler1), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<CargoPackageIdentity>, IPermanentDeleteOperationData), ICiData>) new PermanentDeleteCiDataFacadeHandler<CargoPackageIdentity>(recycleBinController.TfsRequestContext), false).Bootstrap();
        httpResponseMessage = await converter.ThenDelegateTo<IRawPackageRequest, PackageRequest<CargoPackageIdentity>, HttpResponseMessage>(handler2).Handle((IRawPackageRequest) new RawPackageRequest(feedRequest, packageName, packageVersion));
      }
      return httpResponseMessage;
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public Task<HttpResponseMessage> RestorePackageVersionFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion,
      CargoRecycleBinPackageVersionDetails packageVersionDetails)
    {
      using (this.EnterTracer(nameof (RestorePackageVersionFromRecycleBin)))
      {
        PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails> request = this.GetPackageRequest(feedId, packageName, packageVersion).WithData<CargoPackageIdentity, IRecycleBinPackageVersionDetails>((IRecycleBinPackageVersionDetails) packageVersionDetails);
        return new CargoWriteBootstrapper<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData, HttpResponseMessage>(this.TfsRequestContext, (IRequireAggHandlerBootstrapper<PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData>) new CargoRestoreToFeedValidatingOpGeneratingHandlerBootstrapper(), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackageRequest<CargoPackageIdentity, IRecycleBinPackageVersionDetails>, IRestoreToFeedOperationData), ICiData>) new RestoreToFeedCiDataFacadeHandler<CargoPackageIdentity>(this.TfsRequestContext), false).Bootstrap().Handle(request);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (CargoPackageVersionDeletionState), null, null)]
    [ClientSwaggerOperationId("GetPackageVersionFromRecycleBin")]
    public async Task<CargoPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      CargoRecycleBinController recycleBinController = this;
      CargoPackageVersionDeletionState metadataFromRecycleBin;
      using (recycleBinController.EnterTracer(nameof (GetPackageVersionMetadataFromRecycleBin)))
      {
        CargoMetadataEntryToPackageConverter packageConverter = new CargoMetadataEntryToPackageConverter();
        IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry> asyncHandler = new CargoMetadataHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap();
        PackageRequest<CargoPackageIdentity, ShowDeletedBool> request = recycleBinController.GetPackageRequest(feedId, packageName, packageVersion).WithData<CargoPackageIdentity, ShowDeletedBool>(new ShowDeletedBool(true));
        IAsyncHandler<PackageRequest<CargoPackageIdentity>, ICargoMetadataEntry> metadataService = asyncHandler;
        Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API.Package package = await new GetPackageVersionHandler<CargoPackageIdentity, ICargoMetadataEntry, Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API.Package>((IConverter<ICargoMetadataEntry, Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API.Package>) packageConverter, metadataService).Handle(request);
        if (!package.DeletedDate.HasValue || package.PermanentlyDeletedDate.HasValue)
          throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageVersionNotFoundInRecycleBin((object) (package.Name + " " + package.Version)));
        metadataFromRecycleBin = new CargoPackageVersionDeletionState(package.Name, package.Version, package.DeletedDate.Value);
      }
      return metadataFromRecycleBin;
    }
  }
}
