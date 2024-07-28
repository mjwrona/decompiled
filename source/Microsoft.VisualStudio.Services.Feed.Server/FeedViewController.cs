// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedViewController
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Feed Management")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Views")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedViewController : FeedApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (FeedView), null, null)]
    public HttpResponseMessage CreateFeedView(string feedId, FeedView view)
    {
      view.ThrowIfNull<FeedView>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (view))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation();
      return this.Request.CreateResponse<FeedView>(HttpStatusCode.Created, this.ViewService.CreateView(this.TfsRequestContext, feed, view).IncludeUrls(this.TfsRequestContext, feed));
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public FeedView GetFeedView(string feedId, string viewId)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      viewId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (viewId))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation();
      return FeedModelSecuredObjectExtensions.SetSecuredObject(this.ViewService.GetView(this.TfsRequestContext, feed, viewId).IncludeUrls(this.TfsRequestContext, feed), feed);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    public IEnumerable<FeedView> GetFeedViews(string feedId)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation();
      return this.ViewService.GetViews(this.TfsRequestContext, feed).Select<FeedView, FeedView>((Func<FeedView, FeedView>) (v => v.IncludeUrls(this.TfsRequestContext, feed))).SetSecuredObject(feed);
    }

    [HttpDelete]
    public void DeleteFeedView(string feedId, string viewId)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      viewId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (viewId))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      this.ViewService.DeleteView(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation(), viewId);
    }

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    [ClientResponseType(typeof (FeedView), null, null)]
    public HttpResponseMessage UpdateFeedView(string feedId, string viewId, FeedView view)
    {
      view.ThrowIfNull<FeedView>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (view))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation();
      return this.Request.CreateResponse<FeedView>(HttpStatusCode.OK, this.ViewService.UpdateView(this.TfsRequestContext, feed, viewId, view).IncludeUrls(this.TfsRequestContext, feed));
    }
  }
}
