// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmPackagesBatchController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.Npm;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [ClientGroupByResource("npm")]
  [VersionedApiControllerCustomName(Area = "npm", ResourceName = "packagesbatch", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowJson = true)]
  public class NpmPackagesBatchController : NpmApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ControllerMethodTraceFilter(12001500)]
    public async Task<HttpResponseMessage> UpdatePackagesAsync(
      string feedId,
      [FromBody] NpmPackagesBatchRequest batchRequest)
    {
      NpmPackagesBatchController packagesBatchController = this;
      FeedCore feed = packagesBatchController.GetFeedRequest(feedId).Feed;
      if (batchRequest == null)
        throw new InvalidUserRequestException(Microsoft.VisualStudio.Services.Npm.Server.Resources.Error_ParameterCannotBeNull((object) nameof (batchRequest)));
      if (batchRequest.Operation == NpmBatchOperationType.UpgradeCachedPackages)
        throw new ArgumentException("The service does not support V1 Feeds. This code path should not be hit anymore. Please reach out to support for help.");
      await packagesBatchController.TfsRequestContext.GetService<INpmVersionsService>().UpdatePackageVersions(packagesBatchController.TfsRequestContext, feed, batchRequest);
      return packagesBatchController.Request.CreateResponse(HttpStatusCode.Accepted);
    }
  }
}
