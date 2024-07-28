// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPageCommentReactionsController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.Azure.DevOps.Comments.WebApi;
using Microsoft.Azure.DevOps.Comments.WebApi.Controllers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.Web.Helpers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pageCommentReactions")]
  [FeatureEnabled("Wiki.WikiPageCommentReactions")]
  public class WikiPageCommentReactionsController : CommentsReactionBaseController
  {
    [HttpPut]
    [ClientResourceOperation(ClientResourceOperationName.Add)]
    [ClientResponseType(typeof (CommentReaction), null, null)]
    [ClientLocationId("7A5BC693-AAB7-4D48-8F34-36F373022063")]
    [TraceFilter(15251100, 15251199)]
    [ClientInternalUseOnly(true)]
    public CommentReaction AddCommentReaction(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int pageId,
      [FromUri] int commentId,
      [FromUri] CommentReactionType type)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.CreateCommentReaction(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), commentId, type);
    }

    [HttpDelete]
    [ClientResourceOperation(ClientResourceOperationName.Delete)]
    [ClientResponseType(typeof (CommentReaction), null, null)]
    [ClientLocationId("7A5BC693-AAB7-4D48-8F34-36F373022063")]
    [TraceFilter(15251100, 15251199)]
    [ClientInternalUseOnly(true)]
    public CommentReaction DeleteCommentReaction(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int pageId,
      [FromUri] int commentId,
      [FromUri] CommentReactionType type)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.DeleteCommentReaction(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), commentId, type);
    }
  }
}
