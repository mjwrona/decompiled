// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageBadgeController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Artifact Details")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Badge")]
  public class PackageBadgeController : FeedApiController
  {
    private const string Layer = "PackageBadgeController";

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "image/svg+xml")]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [AllowAnonymousProjectLevelRequests]
    public HttpResponseMessage GetBadge(string feedId, Guid packageId)
    {
      IVssRequestContext requestContext1 = this.TfsRequestContext.Elevate();
      StringContent stringContent;
      try
      {
        IFeedService feedService = this.FeedService;
        IVssRequestContext requestContext2 = requestContext1;
        string feedId1 = feedId;
        ProjectInfo projectInfo = this.ProjectInfo;
        ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(requestContext2, feedId1, projectReference);
        if (!feed.BadgesEnabled)
          return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.Error_PackageBadgeNotFound());
        Package latestReleaseVersion = PackageBadgeHelper.GetLatestReleaseVersion(requestContext1, feed, packageId.ToString());
        stringContent = new StringContent(PackageBadgeHelper.GetPackageBadgeSvg(this.TfsRequestContext, latestReleaseVersion.ProtocolType, latestReleaseVersion.Versions.First<MinimalPackageVersion>().Version).ToString(), Encoding.UTF8, "image/svg+xml");
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case FeedIdNotFoundException _:
          case FeedViewNotFoundException _:
          case PackageNotFoundException _:
          case InvalidUserInputException _:
label_6:
            return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, Resources.Error_PackageBadgeNotFound());
          default:
            this.TfsRequestContext.TraceCatch(10019088, "Feed", nameof (PackageBadgeController), ex);
            goto label_6;
        }
      }
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) stringContent;
      return response;
    }
  }
}
