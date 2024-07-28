// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPageCommentsController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.Azure.DevOps.Comments.Web.Controllers;
using Microsoft.Azure.DevOps.Comments.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.Server.Utils;
using Microsoft.TeamFoundation.Wiki.Web.Helpers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pageComments")]
  public class WikiPageCommentsController : CommentsBaseController
  {
    [HttpGet]
    [ClientResourceOperation(ClientResourceOperationName.Get)]
    [ClientResponseType(typeof (Comment), null, null)]
    [ClientLocationId("9B394E93-7DB5-46CB-9C26-09A36AA5C895")]
    [TraceFilter(15250900, 15250999)]
    [FeatureEnabled("Wiki.WikiPageComments")]
    [ClientInternalUseOnly(true)]
    [PublicProjectRequestRestrictions]
    public Comment GetComment(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int pageId,
      [FromUri] int id,
      [ClientQueryParameter] bool? excludeDeleted = null,
      [FromUri(Name = "$expand")] CommentExpandOptions expandOptions = CommentExpandOptions.None)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.GetComment<CommentList, Comment>(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), id, new bool?(excludeDeleted.GetValueOrDefault()), expandOptions);
    }

    [HttpGet]
    [ClientResourceOperation(ClientResourceOperationName.List)]
    [ClientResponseType(typeof (CommentList), null, null)]
    [ClientLocationId("9B394E93-7DB5-46CB-9C26-09A36AA5C895")]
    [TraceFilter(15250900, 15250999)]
    [FeatureEnabled("Wiki.WikiPageComments")]
    [ClientInternalUseOnly(true)]
    [PublicProjectRequestRestrictions]
    public CommentList ListComments(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int pageId,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri] string continuationToken = null,
      [ClientQueryParameter] bool? excludeDeleted = null,
      [FromUri(Name = "$expand")] CommentExpandOptions expandOptions = CommentExpandOptions.None,
      [FromUri(Name = "order")] CommentSortOrder? order = null,
      [ClientQueryParameter] int? parentId = null)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.GetComments<CommentList, Comment>(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), top, continuationToken, new bool?(excludeDeleted.GetValueOrDefault()), expandOptions, order, parentId);
    }

    [HttpPost]
    [ClientResourceOperation(ClientResourceOperationName.Add)]
    [ClientResponseType(typeof (Comment), null, null)]
    [ClientLocationId("9B394E93-7DB5-46CB-9C26-09A36AA5C895")]
    [TraceFilter(15250900, 15250999)]
    [FeatureEnabled("Wiki.WikiPageComments")]
    [ClientInternalUseOnly(true)]
    public Comment AddComment([FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier, [FromUri] int pageId, [FromBody] CommentCreateParameters request)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.AddComment<CommentList, Comment>(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), request);
    }

    [HttpPatch]
    [ClientResourceOperation(ClientResourceOperationName.Update)]
    [ClientResponseType(typeof (Comment), null, null)]
    [ClientLocationId("9B394E93-7DB5-46CB-9C26-09A36AA5C895")]
    [TraceFilter(15250900, 15250999)]
    [FeatureEnabled("Wiki.WikiPageComments")]
    [ClientInternalUseOnly(true)]
    public Comment UpdateComment(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int pageId,
      [FromUri] int id,
      [FromBody] CommentUpdateParameters comment)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.UpdateComment<CommentList, Comment>(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), id, comment);
    }

    [HttpDelete]
    [ClientResourceOperation(ClientResourceOperationName.Delete)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("9B394E93-7DB5-46CB-9C26-09A36AA5C895")]
    [TraceFilter(15250900, 15250999)]
    [FeatureEnabled("Wiki.WikiPageComments")]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage DeleteComment([FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier, [FromUri] int pageId, [FromUri] int id)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.DeleteComment(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), id);
    }

    protected override void AddUrlToComments(CommentList commentList)
    {
      if (commentList.Comments.Count<Comment>() <= 0)
        return;
      Guid projectId;
      Guid wikiId;
      int pageId;
      WikiPageIdHelper.GetIDsFromArtifactiId(commentList.Comments.FirstOrDefault<Comment>().ArtifactId, out projectId, out wikiId, out pageId);
      WikiPageCommentUrlBuilder commentUrlBuilder = new WikiPageCommentUrlBuilder(this.TfsRequestContext, projectId, wikiId, pageId);
      foreach (Comment comment in commentList.Comments)
        comment.Url = commentUrlBuilder.getPageCommentResourceUrl(comment.Id);
    }

    protected override void AddUrlToComment(Comment comment)
    {
      Guid projectId;
      Guid wikiId;
      int pageId;
      WikiPageIdHelper.GetIDsFromArtifactiId(comment.ArtifactId, out projectId, out wikiId, out pageId);
      WikiPageCommentUrlBuilder commentUrlBuilder = new WikiPageCommentUrlBuilder(this.TfsRequestContext, projectId, wikiId, pageId);
      comment.Url = commentUrlBuilder.getPageCommentResourceUrl(comment.Id);
    }
  }
}
