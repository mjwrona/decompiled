// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitApiController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.CodeReview.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [ApiTelemetry(true, false)]
  [AccessReadConsistencyFilter("Git.Server.RestHeaderOptInReadReplicaEnabled")]
  public abstract class GitApiController : TfsProjectApiController
  {
    protected static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static GitApiController()
    {
      GitApiController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (InvalidGitRepositoryNameException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (InvalidArgumentValueException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (InvalidEnumArgumentException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestInvalidParametersException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (TeamFoundationServiceException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (ProjectException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitNoPreviousChangeException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestNotEditableException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestCannotBeActivated), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitRepositoryRequiredForBranchFilterException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitArgumentException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (NotSupportedException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (InvalidGitRefNameException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitRefNameConflictException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitRefUpdateRejectedByPluginException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitAsyncRefOperationInvalidSourceException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestCommentThreadCreateIdException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestInvalidReviewer), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestReviewerPermission), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestMaxReviewerCountException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (RemoteRepositoryValidationFailed), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestAttachmentsNotSupported), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestMaxAttachmentCountException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestMaxDiscussionThreadCountException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestLabelNameInvalid), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestCommitsInvalidContinuationTokenException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestCommitsStaleContinuationTokenException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (PropertiesCollectionPatchException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (UnsafeGitmodulesException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (VssPropertyValidationException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (PropertyTypeNotSupportedException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitRecursionLimitReachedException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (CodeReviewMaxCommentCountException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestDraftCannotVoteException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (InvalidSvgException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitObjectRejectedException), HttpStatusCode.BadRequest);
      GitApiController.s_httpExceptions.Add(typeof (GitNeedsPermissionException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (UnauthorizedAccessException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestUpdateRejectedByPolicyException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitRefLockedException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitForcePushDeniedException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitRefUpdateRejectedByPolicyException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitRefLockDeniedException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitRefUnlockDeniedException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestStatusCannotBeCreatedException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestStatusNotEditableException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitFilePathsMaxCountExceededException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (PolicyNeedsPermissionException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestStatusRejectedByPolicyException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestCommentCannotBeUpdatedException), HttpStatusCode.Forbidden);
      GitApiController.s_httpExceptions.Add(typeof (GitRepositoryNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestAttachmentNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestIterationNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestStatusNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestChangesNotAvailableException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestCommentThreadNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestCommentNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestLabelNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitRefNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitRefFavoriteNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitIdentityNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitItemNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitCommitDoesNotExistException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitAsyncOperationNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitAsyncRefOperationNotFoundException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (ProjectDoesNotExistWithNameException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitUnresolvableToCommitException), HttpStatusCode.NotFound);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestExistsException), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitPullRequestStaleException), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitReferenceStaleException), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitDuplicateRefFavoriteException), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitAsyncOperationAlreadyExistsException), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitImportForbiddenOnNonEmptyRepository), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitAsyncRefOperationInvalidOntoOrGeneratedRefs), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitRefNameAlreadyExistsException), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitFailedToCreateTagException), HttpStatusCode.Conflict);
      GitApiController.s_httpExceptions.Add(typeof (GitServiceUnavailableException), HttpStatusCode.ServiceUnavailable);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) GitApiController.s_httpExceptions;

    public override string TraceArea => "GitService";

    public override string ActivityLogArea => GitServerConstants.ServerName;

    public override Exception TranslateException(Exception ex) => ex is CircuitBreakerException ? (Exception) new GitServiceUnavailableException(ex) : base.TranslateException(ex);

    protected void ThrowIfFeatureNotEnabled(string featureFlag)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled(featureFlag))
        throw new HttpException(404, Microsoft.TeamFoundation.SourceControl.WebServer.Resources.Get("APINotFound"));
    }

    protected string GetProjectFilter(string projectId) => this.GetProjectFilter(projectId, out ProjectInfo _);

    protected string GetProjectFilter(string projectId, out ProjectInfo projectInfo)
    {
      string projectFilter = (string) null;
      projectInfo = (ProjectInfo) null;
      if (string.IsNullOrEmpty(projectId))
      {
        if (this.ProjectInfo != null)
        {
          projectFilter = this.ProjectInfo.Uri;
          projectInfo = this.ProjectInfo;
        }
      }
      else
      {
        projectFilter = GitServerUtils.GetProjectUriFilter(this.TfsRequestContext, projectId, out projectInfo);
        if (projectFilter == null)
          throw new GitRepositoryNotFoundException(projectId);
      }
      return projectFilter;
    }

    protected Guid ResolveProjectId(string projectIdOrName)
    {
      if (projectIdOrName != null)
        return GitServerUtils.GetProjectInfo(this.TfsRequestContext, projectIdOrName, true).Id;
      return this.ProjectInfo != null ? this.ProjectInfo.Id : throw new ProjectDoesNotExistWithNameException(projectIdOrName);
    }

    protected ITfsGitRepository GetTfsGitRepository(
      string repositoryId,
      string projectId = null,
      bool allowReadByAdmins = false)
    {
      string projectFilter = this.GetProjectFilter(projectId);
      return GitServerUtils.FindRepositoryByFilters(this.TfsRequestContext, repositoryId, projectFilter, allowReadByAdmins);
    }

    protected RepoKey ResolveRepoKey(string repositoryNameOrId)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryNameOrId, this.ProjectInfo?.Id.ToString()))
        return tfsGitRepository.Key;
    }

    protected static CacheControlHeaderValue GetCacheControl() => new CacheControlHeaderValue()
    {
      Public = false,
      MaxAge = new TimeSpan?(TimeSpan.FromHours(72.0))
    };
  }
}
