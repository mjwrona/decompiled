// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPageCommentAttachmentsController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.Azure.DevOps.Comments.Web.Controllers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pageCommentAttachments")]
  public class WikiPageCommentAttachmentsController : CommentAttachmentsBaseController
  {
    protected static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static WikiPageCommentAttachmentsController() => WikiPageCommentAttachmentsController.s_httpExceptions.Add(typeof (WikiNotFoundException), HttpStatusCode.NotFound);

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) WikiPageCommentAttachmentsController.s_httpExceptions;

    [HttpPost]
    [ClientResponseType(typeof (Microsoft.Azure.DevOps.Comments.WebApi.CommentAttachment), null, null)]
    [ClientLocationId("5100D976-363D-42E7-A19D-4171ECB44782")]
    [TraceFilter(15251000, 15251099)]
    [FeatureEnabled("Wiki.WikiPageComments")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [RequestContentTypeRestriction(AllowJson = false, AllowJsonPatch = false, AllowStream = true, AllowZip = false)]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientInternalUseOnly(true)]
    public Microsoft.Azure.DevOps.Comments.WebApi.CommentAttachment CreateCommentAttachment(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int pageId)
    {
      string artifactId = WikiPageIdHelper.GetArtifactId(this.ProjectId, (WikiV2Helper.GetWikiByIdentifier(this.TfsRequestContext, this.ProjectId, wikiIdentifier) ?? throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound)).Id, pageId);
      using (Stream result = this.Request.Content.ReadAsStreamAsync().Result)
        return this.CreateAttachment<Microsoft.Azure.DevOps.Comments.WebApi.CommentAttachment>(WikiArtifactKinds.WikiPage, artifactId, result);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetAttachmentContent", "application/octet-stream")]
    [ClientLocationId("5100D976-363D-42E7-A19D-4171ECB44782")]
    [TraceFilter(15251000, 15251099)]
    [FeatureEnabled("Wiki.WikiPageComments")]
    [PublicProjectRequestRestrictions]
    [ClientInternalUseOnly(true)]
    public HttpResponseMessage GetCommentAttachment(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromUri] int pageId,
      [FromUri] Guid attachmentId)
    {
      string artifactId = WikiPageIdHelper.GetArtifactId(this.ProjectId, (WikiV2Helper.GetWikiByIdentifier(this.TfsRequestContext, this.ProjectId, wikiIdentifier) ?? throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound)).Id, pageId);
      (Microsoft.TeamFoundation.Comments.Server.CommentAttachment securedObject, Stream content) = this.GetAttachment(WikiArtifactKinds.WikiPage, artifactId, attachmentId);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerStreamContent(content, (object) securedObject);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      return response;
    }

    protected override void AddUrlToAttachment<CommentAttachment>(
      Guid projectId,
      string artifactId,
      CommentAttachment commentAttachment)
    {
      Guid projectId1;
      Guid wikiId;
      int pageId;
      WikiPageIdHelper.GetIDsFromArtifactiId(artifactId, out projectId1, out wikiId, out pageId);
      commentAttachment.Url = WikiUrlHelper.GetWikiPageCommentAttachmentsResourceUrl(this.TfsRequestContext, projectId1, wikiId, pageId, new Guid?(commentAttachment.Id));
    }
  }
}
