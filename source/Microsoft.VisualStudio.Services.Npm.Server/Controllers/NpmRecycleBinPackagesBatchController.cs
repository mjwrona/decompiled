// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmRecycleBinPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.UpdatePackageVersions;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [ClientGroupByResource("Npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "recyclebinpackagesbatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class NpmRecycleBinPackagesBatchController : NpmApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ControllerMethodTraceFilter(12001500)]
    [ValidateModel]
    public async Task<HttpResponseMessage> UpdateRecycleBinPackagesAsync(
      string feedId,
      [FromBody] NpmPackagesBatchRequest batchRequest)
    {
      NpmRecycleBinPackagesBatchController packagesBatchController = this;
      IValidator<FeedCore> readOnlyValidator = FeedValidator.GetFeedIsNotReadOnlyValidator();
      IFeedRequest feedRequest = packagesBatchController.GetFeedRequest(feedId, readOnlyValidator);
      return await new NpmRecycleBinPackagesBatchBootstrapper(packagesBatchController.TfsRequestContext).Bootstrap().Handle(new BatchRawRequest(feedRequest, (IPackagesBatchRequest) batchRequest));
    }
  }
}
