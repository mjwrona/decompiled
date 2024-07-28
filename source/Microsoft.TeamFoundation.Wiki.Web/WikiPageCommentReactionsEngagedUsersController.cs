// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPageCommentReactionsEngagedUsersController
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
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pageCommentReactionsEngagedUsers")]
  [FeatureEnabled("Wiki.WikiPageCommentReactions")]
  public class WikiPageCommentReactionsEngagedUsersController : 
    CommentReactionsEngagedUsersBaseController
  {
    [HttpGet]
    [ClientLocationId("598A5268-41A7-4162-B7DC-344131E4D1FA")]
    [TraceFilter(15251200, 15251299)]
    [ClientInternalUseOnly(true)]
    public IEnumerable<IdentityRef> GetEngagedUsers(
      [ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      int pageId,
      int commentId,
      CommentReactionType type,
      [FromUri(Name = "$top")] int top = 100,
      [FromUri(Name = "$skip")] int skip = 0)
    {
      WikiV2 wikiV2 = WikiPageCommentValidationHelper.ValidateWikiAndPageId(this.TfsRequestContext, this.ProjectId, wikiIdentifier, pageId);
      return this.GetEngagedUsers(WikiArtifactKinds.WikiPage, WikiPageIdHelper.GetArtifactId(this.ProjectId, wikiV2.Id, pageId), commentId, type, new int?(top), new int?(skip));
    }
  }
}
