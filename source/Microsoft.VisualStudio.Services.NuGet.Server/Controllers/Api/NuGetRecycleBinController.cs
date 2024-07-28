// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetRecycleBinController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("NuGet")]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "RecycleBinVersions", ResourceVersion = 1)]
  public class NuGetRecycleBinController : NuGetApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public async Task<HttpResponseMessage> DeletePackageVersionFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      NuGetRecycleBinController recycleBinController = this;
      HttpResponseMessage httpResponseMessage;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(recycleBinController.TfsRequestContext, NuGetTracePoints.NuGetRecycleBinController.TraceData, 5720200, nameof (DeletePackageVersionFromRecycleBin)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId, readOnlyValidator);
        httpResponseMessage = await new NuGetPermanentDeleteRequestHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().Handle(new RawPackageRequest(feedRequest, packageName, packageVersion));
      }
      return httpResponseMessage;
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> RestorePackageVersionFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion,
      NuGetRecycleBinPackageVersionDetails packageVersionDetails)
    {
      NuGetRecycleBinController recycleBinController = this;
      HttpResponseMessage httpResponseMessage;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(recycleBinController.TfsRequestContext, NuGetTracePoints.NuGetRecycleBinController.TraceData, 5720210, nameof (RestorePackageVersionFromRecycleBin)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId, readOnlyValidator);
        httpResponseMessage = await new NuGetRestoreToFeedHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().Handle(new RawPackageRequest<IRecycleBinPackageVersionDetails>(feedRequest, packageName, packageVersion, (IRecycleBinPackageVersionDetails) packageVersionDetails));
      }
      return httpResponseMessage;
    }

    [HttpGet]
    [ClientResponseType(typeof (NuGetPackageVersionDeletionState), null, null)]
    [ClientSwaggerOperationId("GetPackageVersionFromRecycleBin")]
    public async Task<IPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBin(
      string feedId,
      string packageName,
      string packageVersion)
    {
      NuGetRecycleBinController recycleBinController = this;
      IPackageVersionDeletionState versionDeletionState;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(recycleBinController.TfsRequestContext, NuGetTracePoints.NuGetRecycleBinController.TraceData, 5720220, nameof (GetPackageVersionMetadataFromRecycleBin)))
      {
        IFeedRequest feedRequest = recycleBinController.GetFeedRequest(feedId);
        Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.Package package = await new GetPackageVersionHandlerBootstrapper(recycleBinController.TfsRequestContext).Bootstrap().Handle(new RawPackageRequest<ShowDeletedBool>(feedRequest, packageName, packageVersion, (ShowDeletedBool) true));
        if (!package.DeletedDate.HasValue || package.PermanentlyDeletedDate.HasValue)
          throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageVersionNotFoundInRecycleBin((object) (package.Name + " " + package.Version)));
        versionDeletionState = (IPackageVersionDeletionState) package.ToNuGetPackageVersionDeletionState();
      }
      return versionDeletionState;
    }
  }
}
