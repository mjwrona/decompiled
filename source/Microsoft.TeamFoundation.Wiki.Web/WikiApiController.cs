// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiApiController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  public abstract class WikiApiController : TfsProjectApiController
  {
    private const string WebAccessWikiRichCodeWikiEditing = "WebAccess.NewWiki.RichCodeWikiEditing";
    protected static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();
    protected WikiJobHandler m_WikiJobHandler;

    static WikiApiController()
    {
      WikiApiController.s_httpExceptions.Add(typeof (AccessCheckException), HttpStatusCode.Forbidden);
      WikiApiController.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (InvalidArgumentValueException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (WikiOperationNotSupportedException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (GitObjectRejectedException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (WikiPageOperationFailedException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (ScopeNotSupportedException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (WikiInvalidNameException), HttpStatusCode.BadRequest);
      WikiApiController.s_httpExceptions.Add(typeof (WikiNotFoundException), HttpStatusCode.NotFound);
      WikiApiController.s_httpExceptions.Add(typeof (WikiPageNotFoundException), HttpStatusCode.NotFound);
      WikiApiController.s_httpExceptions.Add(typeof (WikiAncestorPageNotFoundException), HttpStatusCode.NotFound);
      WikiApiController.s_httpExceptions.Add(typeof (WikiPageRenameSourceNotFoundException), HttpStatusCode.NotFound);
      WikiApiController.s_httpExceptions.Add(typeof (GitRepositoryNotFoundException), HttpStatusCode.NotFound);
      WikiApiController.s_httpExceptions.Add(typeof (WikiAlreadyExistsException), HttpStatusCode.Conflict);
      WikiApiController.s_httpExceptions.Add(typeof (WikiRenameConflictException), HttpStatusCode.Conflict);
      WikiApiController.s_httpExceptions.Add(typeof (WikiPageHasConflictsException), HttpStatusCode.PreconditionFailed);
    }

    protected override void InitializeInternal(HttpControllerContext controllerContext)
    {
      base.InitializeInternal(controllerContext);
      this.m_WikiJobHandler = new WikiJobHandler();
    }

    protected WikiJobHandler GetWikiJobHandler() => this.m_WikiJobHandler;

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) WikiApiController.s_httpExceptions;

    protected ITfsGitRepository GetWikiRepository(WikiV2 wiki)
    {
      if (wiki == null)
        throw new WikiNotFoundException(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiNotFound);
      return this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(this.TfsRequestContext, wiki.RepositoryId) ?? throw new GitRepositoryNotFoundException(wiki.RepositoryId);
    }

    protected Sha1Id ParseSha1Id(string sha1Id) => new Sha1Id(sha1Id);

    protected TfsGitRefUpdateResultSet PerformActionOnWikiPage(
      ITfsGitRepository repository,
      string mappedPath,
      WikiPageChange wikiPageChange,
      GitVersionDescriptor versionDescriptor)
    {
      IGitChangesProvider<WikiPageChange> gitChangesProvider = (IGitChangesProvider<WikiPageChange>) new GitChangesProvider();
      TfsGitRef versionDescriptor1 = this.GetRefFromVersionDescriptor(repository, versionDescriptor);
      if (versionDescriptor1 == null)
        throw new InvalidArgumentValueException("Version", string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiVersionInvalidOrDoesNotExist, (object) versionDescriptor.Version));
      try
      {
        TfsGitRefUpdateResultSet refUpdateResultSet = repository.ModifyPaths(versionDescriptor1.Name, versionDescriptor1.ObjectId, wikiPageChange.Comment, (IEnumerable<GitChange>) gitChangesProvider.GetChanges(this.TfsRequestContext, repository, versionDescriptor, mappedPath, wikiPageChange), (GitUserDate) null, (GitUserDate) null);
        TfsGitRefUpdateResult gitRefUpdateResult = refUpdateResultSet.Results.Single<TfsGitRefUpdateResult>();
        if (gitRefUpdateResult.Succeeded)
          return refUpdateResultSet;
        switch (gitRefUpdateResult.Status)
        {
          case GitRefUpdateStatus.StaleOldObjectId:
            throw new WikiPageOperationFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageOperationFailedWithMessage, (object) Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageStalePageObjectId));
          case GitRefUpdateStatus.WritePermissionRequired:
            throw new WikiPageOperationFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageOperationFailedWithMessage, (object) Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageWritePermissionRequired));
          case GitRefUpdateStatus.RejectedByPolicy:
            throw new WikiPageOperationFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageOperationFailedWithMessage, (object) gitRefUpdateResult.CustomMessage));
          default:
            throw new WikiPageOperationFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageOperationFailedWithStatus, (object) gitRefUpdateResult.Status));
        }
      }
      catch (Exception ex) when (
      {
        // ISSUE: unable to correctly present filter
        int num;
        switch (ex)
        {
          case WikiPageNotFoundException _:
          case WikiAncestorPageNotFoundException _:
          case WikiPageRenameSourceNotFoundException _:
          case GitObjectRejectedException _:
            num = 1;
            break;
          default:
            num = ex is WikiPageOperationFailedException ? 1 : 0;
            break;
        }
        if ((uint) num > 0U)
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
        this.TfsRequestContext.TraceException(15252800, "Wiki", "Service", ex);
        throw;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(15252800, "Wiki", "Service", ex);
        throw new WikiPageOperationFailedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageOperationFailedWithMessage, (object) ex.Message), ex);
      }
    }

    protected void ThrowPageOperationUnsupportedIfCodeWiki(WikiV2 wiki)
    {
      if (wiki.Type == WikiType.CodeWiki)
        throw new WikiOperationNotSupportedException(string.Format(Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageOperationNotSupported, (object) WikiType.CodeWiki));
    }

    protected TfsGitRef GetRefFromVersionDescriptor(
      ITfsGitRepository repository,
      GitVersionDescriptor versionDesc)
    {
      if (versionDesc == null || repository == null)
        return (TfsGitRef) null;
      switch (versionDesc.VersionType)
      {
        case GitVersionType.Branch:
          return repository.Refs?.MatchingName("refs/heads/" + versionDesc.Version);
        case GitVersionType.Tag:
          return repository.Refs?.MatchingName("refs/tags/" + versionDesc.Version);
        default:
          return (TfsGitRef) null;
      }
    }

    protected GitVersionDescriptor ValidateAndGetWikiVersion(
      WikiV2 wiki,
      GitVersionDescriptor versionDescriptor)
    {
      return ValidationUtils.ValidateAndGetWikiVersionForEditScenarios(this.TfsRequestContext, wiki, versionDescriptor, Microsoft.TeamFoundation.Wiki.Web.Resources.ErrorMessageInvalidWikiVersion, Microsoft.TeamFoundation.Wiki.Web.Resources.WikiPageOperationNotSupported, Microsoft.TeamFoundation.Wiki.Web.Resources.WikiVersionInvalidOrDoesNotExist);
    }

    protected bool TryProcessPushAndGetPageId(
      WikiV2 wiki,
      ITfsGitRepository repository,
      GitVersionDescriptor version,
      int interestedPushId,
      WikiPageChange pageChange,
      int tracePoint,
      out int? pageId)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Wiki.EnablePageIdSyncProcessing"))
      {
        pageId = new int?();
        this.TfsRequestContext.TraceConditionally(1, TraceLevel.Info, "Wiki", "Service", (Func<string>) (() => "FeatureFlag: Wiki.EnablePageIdSyncProcessing is not enabled"));
        return false;
      }
      if (pageChange.ChangeType != WikiChangeType.Rename && pageChange.ChangeType != WikiChangeType.Add)
      {
        pageId = new int?();
        return false;
      }
      try
      {
        using (TimedCiEvent timedCiEvent = new TimedCiEvent(this.TfsRequestContext, "Wiki", "Service"))
        {
          List<WikiPageWithId> addedPagesInInterestPush;
          List<WikiPageWithId> renamedPagesInInterestPush;
          if (WikiPageIdHelper.ProcessPushForWikiVersionIfLatest(this.TfsRequestContext, wiki, repository, version, timedCiEvent, new WikiPushWaterMarkProvider(), new WikiPageIdProvider(), interestedPushId, this.TfsRequestContext.GetMaxPushToBeProcessedInSync(), out addedPagesInInterestPush, out renamedPagesInInterestPush, tracePoint, "Service"))
          {
            if (pageChange.ChangeType == WikiChangeType.Rename)
            {
              if (this.TryGetPageId(pageChange.NewPath, tracePoint, renamedPagesInInterestPush, timedCiEvent, out pageId))
                return true;
            }
            else if (this.TryGetPageId(pageChange.Path, tracePoint, addedPagesInInterestPush, timedCiEvent, out pageId))
              return true;
          }
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(ex, tracePoint);
      }
      pageId = new int?();
      return false;
    }

    private bool TryGetPageId(
      WikiPagePath interestedWikiPagePath,
      int tracePoint,
      List<WikiPageWithId> pages,
      TimedCiEvent ciEvent,
      out int? pageId)
    {
      pageId = new int?();
      WikiPageWithId wikiPageWithId = pages.Find((Predicate<WikiPageWithId>) (x => interestedWikiPagePath.IsEquivalentTo(x.GitFriendlyPagePath)));
      if (wikiPageWithId == null)
      {
        this.TfsRequestContext.TraceErrorAlways("Process Push does not have interested page change", "Error_PageIdSyncError", ciEvent, tracePoint, "Service");
        return false;
      }
      pageId = new int?(wikiPageWithId.PageId);
      return true;
    }
  }
}
