// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Controllers.Proprietary.CargoRecycleBinPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cargo.Server.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.Cargo.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.Resolution;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.UPack.Server;
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
  [VersionedApiControllerCustomName(Area = "cargo", ResourceName = "RecycleBinPackagesBatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class CargoRecycleBinPackagesBatchController : CargoApiController
  {
    [HttpPost]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdateRecycleBinPackageVersions(
      string feedId,
      [FromBody] CargoPackagesBatchRequest batchRequest)
    {
      CargoRecycleBinPackagesBatchController packagesBatchController = this;
      HttpResponseMessage response;
      using (packagesBatchController.EnterTracer(nameof (UpdateRecycleBinPackageVersions)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = packagesBatchController.GetFeedRequest(feedId, readOnlyValidator);
        HttpResponseMessage httpResponseMessage = await new RawBatchPackageRequestValidatingConverter<CargoPackageIdentity>(packagesBatchController.IdentityResolver.GetRawPackageRequestToIdentityConverter<CargoPackageName, CargoPackageVersion, CargoPackageIdentity, SimplePackageFileName>(packagesBatchController.TfsRequestContext), (IRegistryService) new RegistryServiceFacade(packagesBatchController.TfsRequestContext)).ThenDelegateTo<IBatchRawRequest, PackagesBatchRequest<CargoPackageIdentity>, HttpResponseMessage>(new CargoWriteBootstrapper<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData, HttpResponseMessage>(packagesBatchController.TfsRequestContext, (IRequireAggHandlerBootstrapper<PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData>) new CargoRecycleBinBatchValidatingOpGeneratingHandlerBootstrapper(packagesBatchController.TfsRequestContext), (IAsyncHandler<ICommitLogEntry, HttpResponseMessage>) new ByFuncAsyncHandler<ICommitLogEntry, HttpResponseMessage>((Func<ICommitLogEntry, HttpResponseMessage>) (c => new HttpResponseMessage(HttpStatusCode.Accepted))), (IAsyncHandler<(PackagesBatchRequest<CargoPackageIdentity>, BatchCommitOperationData), ICiData>) new BatchCiDataFacadeHandler<CargoPackageIdentity>(packagesBatchController.TfsRequestContext), false).Bootstrap()).Handle((IBatchRawRequest) new BatchRawRequest(feedRequest, (IPackagesBatchRequest) batchRequest));
        response = packagesBatchController.Request.CreateResponse(HttpStatusCode.Accepted);
      }
      return response;
    }
  }
}
