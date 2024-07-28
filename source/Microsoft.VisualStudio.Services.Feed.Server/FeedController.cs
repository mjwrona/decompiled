// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Feed Management")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Feeds")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedController : FeedApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Feed.WebApi.Feed), null, null)]
    public async Task<HttpResponseMessage> CreateFeed(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      FeedController feedController = this;
      feed.ThrowIfNull<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      IFeedService feedService = feedController.FeedService;
      IVssRequestContext tfsRequestContext = feedController.TfsRequestContext;
      string name = feed.Name;
      string description = feed.Description;
      int num1 = feed.UpstreamEnabled ? 1 : 0;
      int num2 = feed.AllowUpstreamNameConflict ? 1 : 0;
      bool? hideDeletedPackageVersions = new bool?(feed.HideDeletedPackageVersions);
      bool? badgesEnabled = new bool?(feed.BadgesEnabled);
      IEnumerable<FeedPermission> permissions = feed.Permissions;
      IList<UpstreamSource> upstreamSources = feed.UpstreamSources;
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (feedController.ProjectInfo);
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = await (await feedService.CreateFeed(tfsRequestContext, name, description, num1 != 0, num2 != 0, hideDeletedPackageVersions, badgesEnabled, permissions, upstreamSources, projectReference)).IncludeUrlsAsync(feedController.TfsRequestContext);
      return feedController.Request.CreateResponse<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(HttpStatusCode.Created, feed1);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public async Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeed(
      string feedId,
      bool includeDeletedUpstreams = false)
    {
      FeedController feedController = this;
      IFeedService feedService = feedController.FeedService;
      IVssRequestContext tfsRequestContext = feedController.TfsRequestContext;
      string feedId1 = feedId;
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (feedController.ProjectInfo);
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      int num = includeDeletedUpstreams ? 1 : 0;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference, includeDeletedUpstreams: num != 0);
      if (!MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(feedController.Request))
        feed = await feed.IncludeUrlsAsync(feedController.TfsRequestContext);
      return feed.SetSecuredObject();
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public async Task<IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeeds(
      FeedRole feedRole = FeedRole.Reader,
      bool includeDeletedUpstreams = false,
      bool includeUrls = true)
    {
      FeedController feedController = this;
      IFeedService feedService = feedController.FeedService;
      IVssRequestContext tfsRequestContext = feedController.TfsRequestContext;
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (feedController.ProjectInfo);
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      int num1 = (int) feedRole;
      int num2 = includeDeletedUpstreams ? 1 : 0;
      IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> feeds = feedService.GetFeeds(tfsRequestContext, projectReference, (FeedRole) num1, num2 != 0);
      if (!MediaTypeFormatUtility.GetExcludeUrlsAcceptHeaderOptionValue(feedController.Request) & includeUrls)
        feeds = (IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>) await feeds.IncludeUrlsAsync(feedController.TfsRequestContext);
      return feeds.SetSecuredObject();
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (Microsoft.VisualStudio.Services.Feed.WebApi.Feed), null, null)]
    public async Task<HttpResponseMessage> UpdateFeed(string feedId, FeedUpdate feed)
    {
      FeedController feedController = this;
      feed.ThrowIfNull<FeedUpdate>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feed))));
      IFeedService feedService = feedController.FeedService;
      IVssRequestContext tfsRequestContext = feedController.TfsRequestContext;
      string feedId1 = feedId;
      FeedUpdate updatedFeed = feed;
      // ISSUE: explicit non-virtual call
      ProjectInfo projectInfo = __nonvirtual (feedController.ProjectInfo);
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = await (await feedService.UpdateFeed(tfsRequestContext, feedId1, updatedFeed, projectReference)).IncludeUrlsAsync(feedController.TfsRequestContext);
      return feedController.Request.CreateResponse<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(HttpStatusCode.OK, feed1);
    }

    [HttpDelete]
    [ClientLocationId("C65009A7-474A-4AD1-8B42-7D852107EF8C")]
    public void DeleteFeed(string feedId)
    {
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      feedService.DeleteFeed(tfsRequestContext, feedId1, projectReference);
    }
  }
}
