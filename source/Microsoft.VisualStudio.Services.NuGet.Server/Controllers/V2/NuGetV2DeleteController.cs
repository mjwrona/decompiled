// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.NuGetV2DeleteController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.NonProtocolApis.ListUnlistPackageVersion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Validation.BlockedPackageIdentities;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "v2", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowAllContentTypes = true)]
  public class NuGetV2DeleteController : NuGetApiController
  {
    [HttpDelete]
    [RequiresAnyNuGetApiKey]
    public async Task<HttpResponseMessage> DeletePackage(
      string feedId,
      string packageName,
      string version)
    {
      NuGetV2DeleteController deleteController = this;
      HttpResponseMessage httpResponseMessage;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(deleteController.TfsRequestContext, NuGetTracePoints.DeleteController.TraceData, 5721500, nameof (DeletePackage)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = deleteController.GetFeedRequest(feedId, readOnlyValidator);
        httpResponseMessage = await ((IAsyncHandler<RawPackageRequest<ListingOperationRequestAdditionalData>, ContentResult>) IngestRawPackageIfNotAlreadyIngestedBootstrapper.Create(deleteController.TfsRequestContext, BlockedIdentityContext.Update).Bootstrap()).ThenActuallyHandleWith<RawPackageRequest<ListingOperationRequestAdditionalData>, ContentResult, HttpResponseMessage>(new NuGetV2DelistRequestHandlerBootstrapper(deleteController.TfsRequestContext).Bootstrap()).Handle(new RawPackageRequest<ListingOperationRequestAdditionalData>(feedRequest, packageName, version, new ListingOperationRequestAdditionalData(ListingDirection.Delist)));
      }
      return httpResponseMessage;
    }
  }
}
