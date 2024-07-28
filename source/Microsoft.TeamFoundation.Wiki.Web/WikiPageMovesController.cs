// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPageMovesController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pageMoves")]
  public class WikiPageMovesController : WikiApiController
  {
    [HttpPost]
    [ClientResourceOperation(ClientResourceOperationName.Create)]
    [ClientResponseCode(HttpStatusCode.Created, "Page move created. Moved page's version is populated in the ETag response header.", false)]
    [ClientResponseType(typeof (WikiPageMoveResponse), null, null)]
    [ClientLocationId("E37BBE71-CBAE-49E5-9A4E-949143B9D910")]
    [TraceFilter(15250400, 15250499)]
    [ClientExample("POST_pageMoves_reorder.json", "Reorder page", null, null)]
    [ClientExample("POST_pageMoves_reparent.json", "Reparent page", null, null)]
    public HttpResponseMessage CreatePageMove(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromBody] WikiPageMoveParameters pageMoveParameters,
      [FromUri] string comment = null,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null)
    {
      WikiPageChange wikiPageChange = pageMoveParameters != null ? this.GetValidPageChange(pageMoveParameters, comment) : throw new ArgumentNullException(nameof (pageMoveParameters), Microsoft.TeamFoundation.Wiki.Web.Resources.InvalidParametersOrNull);
      WikiV2 wikiByIdentifier = WikiV2Helper.GetWikiByIdentifier(this.TfsRequestContext, this.ProjectId, wikiIdentifier);
      versionDescriptor = wikiByIdentifier != null ? this.ValidateAndGetWikiVersion(wikiByIdentifier, versionDescriptor) : throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound);
      ITfsGitRepository wikiRepository = this.GetWikiRepository(wikiByIdentifier);
      if (wikiRepository.IsInMaintenance)
        throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
      this.Request.RegisterForDispose((IDisposable) wikiRepository);
      TfsGitRefUpdateResultSet refUpdateResultSet = this.PerformActionOnWikiPage(wikiRepository, wikiByIdentifier.MappedPath, wikiPageChange, versionDescriptor);
      int? pageId;
      this.TryProcessPushAndGetPageId(wikiByIdentifier, wikiRepository, versionDescriptor, refUpdateResultSet.PushId.Value, wikiPageChange, 15250401, out pageId);
      return this.CreateWikiPageMoveResponse(wikiRepository, wikiByIdentifier, versionDescriptor, pageMoveParameters, wikiPageChange, pageId);
    }

    private WikiPageChange GetValidPageChange(
      WikiPageMoveParameters pageMoveParameters,
      string comment)
    {
      string pageFilePath1 = PathHelper.GetPageFilePath(pageMoveParameters.Path, "/");
      string pageFilePath2 = PathHelper.GetPageFilePath(pageMoveParameters.NewPath, "/");
      int? newOrder = pageMoveParameters.NewOrder;
      if (string.IsNullOrEmpty(pageFilePath1) || pageFilePath1.Equals("/"))
        throw new InvalidArgumentValueException("pageFilePath", Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageWriteOperationInvalidPagePath);
      WikiChangeType changeType;
      if (!string.IsNullOrEmpty(pageFilePath2) && !pageFilePath2.Equals(pageFilePath1))
      {
        changeType = WikiChangeType.Rename;
        comment = string.IsNullOrEmpty(comment) ? string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.PageRenamedDefaultComment, (object) pageFilePath1, (object) pageFilePath2) : comment;
        foreach (string reservedCharacter in PathConstants.PageNameReservedCharacters)
        {
          if (pageFilePath2.Contains(reservedCharacter))
            throw new InvalidArgumentValueException("newPageFilePath", string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageNameHasReservedCharacters, (object) string.Join(", ", PathConstants.PageNameReservedCharacters)));
        }
      }
      else
      {
        if (!newOrder.HasValue)
          throw new InvalidArgumentValueException(nameof (pageMoveParameters), Microsoft.TeamFoundation.Wiki.Web.Resources.InvalidParametersOrNull);
        changeType = WikiChangeType.Reorder;
        comment = string.IsNullOrEmpty(comment) ? string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.PageReorderedDefaultComment, (object) pageFilePath1, (object) newOrder.ToString()) : comment;
      }
      return WikiPagePath.WikiPageMoveParametersToWikiPageChange(pageMoveParameters, changeType, comment);
    }

    private HttpResponseMessage CreateWikiPageMoveResponse(
      ITfsGitRepository repository,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion,
      WikiPageMoveParameters pageMoveParams,
      WikiPageChange pageChange,
      int? pageId)
    {
      WikiPagePath wikiPagePath;
      HttpStatusCode statusCode;
      switch (pageChange.ChangeType)
      {
        case WikiChangeType.Rename:
          wikiPagePath = pageChange.NewPath;
          statusCode = HttpStatusCode.Created;
          break;
        default:
          wikiPagePath = pageChange.Path;
          statusCode = HttpStatusCode.Created;
          break;
      }
      WikiPage page = new WikiPagesProvider().GetPage(this.TfsRequestContext, repository, wiki.Id, wiki.MappedPath, wikiPagePath, wikiVersion, (IWikiPagesOrderReader) new WikiPagesOrderReader(), includePageId: !pageId.HasValue);
      if (pageId.HasValue)
        page.Id = pageId;
      string str = "PageMoveController: MovePage";
      this.TfsRequestContext.TraceAlways(15250001, TraceLevel.Info, "Wiki", str, string.Format("Wikiid:{0}, version:{1} Permanent id present: {2}", (object) wiki.Id, (object) wikiVersion.Version, (object) page.Id.HasValue));
      TelemetryUtil.EmitHasPageIdCiDataIfNeeded(this.TfsRequestContext, wiki, wikiVersion, page, str, "Wiki", "Service");
      WikiPageMove wikiPageMove1 = new WikiPageMove();
      wikiPageMove1.Path = pageMoveParams.Path;
      wikiPageMove1.NewPath = pageMoveParams.NewPath;
      wikiPageMove1.NewOrder = pageMoveParams.NewOrder;
      wikiPageMove1.Page = page;
      WikiPageMove wikiPageMove2 = wikiPageMove1;
      string objectId = page.InternalItem?.ObjectId;
      HttpResponseMessage response = this.Request.CreateResponse<WikiPageMove>(statusCode, wikiPageMove2);
      response.Headers.ETag = new EntityTagHeaderValue("\"" + objectId + "\"");
      return response;
    }
  }
}
