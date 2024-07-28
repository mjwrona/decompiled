// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRetentionPolicyController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(2.0)]
  [ClientGroupByResource("Retention Policies")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "RetentionPolicies")]
  public class FeedRetentionPolicyController : FeedApiController
  {
    private IPackageRetentionPolicyStore policyStore = (IPackageRetentionPolicyStore) new PackageRetentionPolicyStore();

    [HttpGet]
    [ClientSwaggerOperationId("Get Retention Policy")]
    [PublicProjectRequestRestrictions]
    public FeedRetentionPolicy GetFeedRetentionPolicies(string feedId)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation();
      return this.policyStore.GetPolicy(this.TfsRequestContext, feed).SetSecuredObject(feed);
    }

    [HttpPut]
    [ClientSwaggerOperationId("Set Retention Policy")]
    [ClientResponseType(typeof (FeedRetentionPolicy), null, null)]
    public HttpResponseMessage SetFeedRetentionPolicies(string feedId, FeedRetentionPolicy policy)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation();
      int num = this.policyStore.GetPolicy(this.TfsRequestContext, feed) != null ? 1 : 0;
      this.policyStore.SetPolicy(this.TfsRequestContext, feed, policy);
      return num == 0 ? this.Request.CreateResponse<FeedRetentionPolicy>(HttpStatusCode.Created, policy) : this.Request.CreateResponse<FeedRetentionPolicy>(HttpStatusCode.OK, policy);
    }

    [HttpDelete]
    [ClientSwaggerOperationId("Delete Retention Policy")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteFeedRetentionPolicies(string feedId)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      IFeedService feedService = this.FeedService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feedId1 = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      this.policyStore.DeletePolicy(this.TfsRequestContext, feedService.GetFeed(tfsRequestContext, feedId1, projectReference).ThrowIfViewNotation());
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
