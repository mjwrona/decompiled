// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api.NuGetRecyleBinPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype;
using Microsoft.VisualStudio.Services.NuGet.Server.Constants;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.Api
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("NuGet")]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "RecycleBinPackagesBatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class NuGetRecyleBinPackagesBatchController : NuGetApiController
  {
    [HttpPost]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (void), null, null)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdateRecycleBinPackageVersions(
      string feedId,
      [FromBody] NuGetPackagesBatchRequest batchRequest)
    {
      NuGetRecyleBinPackagesBatchController packagesBatchController = this;
      HttpResponseMessage httpResponseMessage;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(packagesBatchController.TfsRequestContext, NuGetTracePoints.NuGetRecyleBinPackagesBatchController.TraceData, 5723700, nameof (UpdateRecycleBinPackageVersions)))
      {
        IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
        IFeedRequest feedRequest = packagesBatchController.GetFeedRequest(feedId, readOnlyValidator);
        httpResponseMessage = await new NuGetRecycleBinPackagesBatchBootstrapper(packagesBatchController.TfsRequestContext).Bootstrap().Handle(new BatchRawRequest(feedRequest, (IPackagesBatchRequest) batchRequest));
      }
      return httpResponseMessage;
    }
  }
}
