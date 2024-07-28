// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api.PyPiRecycleBinController
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.GetPackageVersion;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.PermanentDelete;
using Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.RestoreToFeed;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Python")]
  [VersionedApiControllerCustomName(Area = "pypi", ResourceName = "RecycleBinVersions", ResourceVersion = 1)]
  public class PyPiRecycleBinController : PyPiApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public async Task<HttpResponseMessage> DeletePackageVersionFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      PyPiRecycleBinController recycleBinController = this;
      HttpResponseMessage httpResponseMessage;
      using (recycleBinController.EnterTracer(nameof (DeletePackageVersionFromRecycleBin)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId, readOnlyValidator);
        httpResponseMessage = await new PyPiPermanentDeleteRequestHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().Handle(new RawPackageRequest(feedRequest, packageName, packageVersion));
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
      PyPiRecycleBinPackageVersionDetails packageVersionDetails)
    {
      using (this.EnterTracer(nameof (RestorePackageVersionFromRecycleBin)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = this.GetFeedRequest(feedId, readOnlyValidator);
        return new PyPiRestoreToFeedHandlerBootstrapper(this.TfsRequestContext).Bootstrap().Handle(new RawPackageRequest<IRecycleBinPackageVersionDetails>(feedRequest, packageName, packageVersion, (IRecycleBinPackageVersionDetails) packageVersionDetails));
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (PyPiPackageVersionDeletionState), null, null)]
    [ClientSwaggerOperationId("GetPackageVersionFromRecycleBin")]
    public async Task<PyPiPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      PyPiRecycleBinController recycleBinController = this;
      PyPiPackageVersionDeletionState metadataFromRecycleBin;
      using (recycleBinController.EnterTracer(nameof (GetPackageVersionMetadataFromRecycleBin)))
      {
        IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId);
        Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API.Package package = await new GetPackageVersionHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().Handle(new RawPackageRequest<ShowDeletedBool>(feedRequest, packageName, packageVersion, (ShowDeletedBool) true));
        if (!package.DeletedDate.HasValue || package.PermanentlyDeletedDate.HasValue)
          throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageVersionNotFoundInRecycleBin((object) (package.Name + " " + package.Version)));
        metadataFromRecycleBin = new PyPiPackageVersionDeletionState(package.Name, package.Version, package.DeletedDate.Value);
      }
      return metadataFromRecycleBin;
    }
  }
}
