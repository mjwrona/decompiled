// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitPullRequestsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class GitPullRequestsController : GitPullRequestsBaseController
  {
    private const int c_maxPullRequestDescriptionLength = 4000;
    private const int c_maxPullRequestCompletionOptionsLength = 4000;

    [HttpPost]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [ClientResponseType(typeof (GitPullRequest), null, null)]
    [ClientExample("POST__git_repositories__repositoryId__pullRequests.json", null, null, null)]
    public HttpResponseMessage CreatePullRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      GitPullRequest gitPullRequestToCreate,
      [ClientIgnore] string projectId = null,
      [ClientInclude(RestClientLanguages.Java | RestClientLanguages.TypeScript)] bool linkBranchWorkItems = false,
      [ClientInclude(RestClientLanguages.Java | RestClientLanguages.TypeScript)] bool linkCommitWorkItems = false,
      bool supportsIterations = true)
    {
      if (gitPullRequestToCreate == null || string.IsNullOrEmpty(gitPullRequestToCreate.SourceRefName) || string.IsNullOrEmpty(gitPullRequestToCreate.TargetRefName))
        throw new InvalidArgumentValueException(Resources.Get("MissingPullRequestRef"));
      if (string.IsNullOrWhiteSpace(gitPullRequestToCreate.Title))
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestTitle"));
      if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState") && gitPullRequestToCreate?.Commits != null && ((IEnumerable<GitCommitRef>) gitPullRequestToCreate.Commits).Any<GitCommitRef>((Func<GitCommitRef, bool>) (c => c?.Statuses != null && c.Statuses.Any<GitStatus>((Func<GitStatus, bool>) (s => s != null && s.State == GitStatusState.PartiallySucceeded)))))
        throw new InvalidArgumentValueException(Resources.Get("InvalidGitStatusStateValue"), "State");
      using (this.TfsRequestContext.TimeRegion(this.TraceArea, WebApiTraceLayers.Controller, regionName: nameof (CreatePullRequest), tracepoint: 1013209))
      {
        using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        {
          if (gitPullRequestToCreate.Repository != null && gitPullRequestToCreate.Repository.Id != Guid.Empty && gitPullRequestToCreate.Repository.Id != tfsGitRepository.Key.RepoId)
            throw new InvalidArgumentValueException(Resources.Get("MismatchRepositoryId"));
          if (!string.IsNullOrWhiteSpace(gitPullRequestToCreate.Description) && gitPullRequestToCreate.Description.Length > 4000)
            throw new InvalidArgumentValueException(Resources.Format("PullRequestDescriptionTooLong", (object) 4000));
          GitRepository repository1 = gitPullRequestToCreate.ForkSource?.Repository;
          if ((repository1 == null || repository1.Id == tfsGitRepository.Key.RepoId) && string.Compare(gitPullRequestToCreate.SourceRefName, gitPullRequestToCreate.TargetRefName, StringComparison.Ordinal) == 0)
            throw new InvalidArgumentValueException(Resources.Get("PullRequestTargetEqualSource"));
          bool includeLabels = this.TfsRequestContext.IsFeatureEnabled("WebAccess.VersionControl.PullRequests.Labels");
          List<TfsGitPullRequest.ReviewerBase> reviewers = this.ProcessReviewersList(gitPullRequestToCreate.Reviewers, tfsGitRepository);
          GitRepositoryRef forkRepositoryRef = TfsGitPullRequest.GetForkRepositoryRef(this.TfsRequestContext, tfsGitRepository, repository1?.Id);
          ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
          TfsGitPullRequest pullRequest;
          try
          {
            ITeamFoundationGitPullRequestService pullRequestService = service;
            IVssRequestContext tfsRequestContext = this.TfsRequestContext;
            ITfsGitRepository repository2 = tfsGitRepository;
            string title = gitPullRequestToCreate.Title;
            string description = gitPullRequestToCreate.Description;
            string sourceRefName = gitPullRequestToCreate.SourceRefName;
            string targetRefName = gitPullRequestToCreate.TargetRefName;
            List<TfsGitPullRequest.ReviewerBase> reviewers1 = reviewers;
            GitPullRequestMergeOptions mergeOptions1 = gitPullRequestToCreate.MergeOptions;
            int num1 = supportsIterations ? 1 : 0;
            GitPullRequestMergeOptions mergeOptions2 = mergeOptions1;
            IEnumerable<int> workItemIds = this.GetWorkItemIds(gitPullRequestToCreate.WorkItemRefs);
            int num2 = linkBranchWorkItems ? 1 : 0;
            int num3 = linkCommitWorkItems ? 1 : 0;
            GitRepositoryRef sourceForkRepositoryRef = forkRepositoryRef;
            WebApiTagDefinition[] labels = includeLabels ? gitPullRequestToCreate.Labels : (WebApiTagDefinition[]) null;
            int num4 = gitPullRequestToCreate.IsDraft.GetValueOrDefault() ? 1 : 0;
            pullRequest = pullRequestService.CreatePullRequest(tfsRequestContext, repository2, title, description, sourceRefName, targetRefName, (IEnumerable<TfsGitPullRequest.ReviewerBase>) reviewers1, true, num1 != 0, mergeOptions2, workItemIds, num2 != 0, num3 != 0, sourceForkRepositoryRef, labels, num4 != 0);
          }
          catch (SqlException ex)
          {
            if (ex.Message.Contains("Cannot insert duplicate key row in object 'dbo.tbl_GitPullRequestReviewer' with unique index 'PK_tbl_GitPullRequestReviewer'."))
              this.TfsRequestContext.TraceDataConditionally(1013962, TraceLevel.Warning, GitServerUtils.TraceArea, "TfsGitPullRequest", ex.Message, (Func<object>) (() => (object) new
              {
                Original = gitPullRequestToCreate.Reviewers,
                Processed = reviewers
              }), nameof (CreatePullRequest));
            throw;
          }
          return this.Request.CreateResponse<GitPullRequest>(HttpStatusCode.Created, pullRequest.ToWebApiItem(this.TfsRequestContext, tfsGitRepository, forkRepositoryRef, includeLinks: true, includeWorkItemRefs: true, includeLabels: includeLabels));
        }
      }
    }

    [HttpGet]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [PublicProjectRequestRestrictions]
    public virtual GitPullRequest GetPullRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId,
      [ClientIgnore] string projectId = null,
      int? maxCommentLength = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null,
      bool includeCommits = false,
      bool includeWorkItemRefs = false)
    {
      GitPullRequest pullRequestInternal = this.GetGitPullRequestInternal(repositoryId, pullRequestId, projectId, includeCommits, includeWorkItemRefs);
      GitStatusStateMapper.MapGitEntity<GitPullRequest>(pullRequestInternal, this.TfsRequestContext);
      return pullRequestInternal;
    }

    [HttpGet]
    [ClientLocationId("01A46DEA-7D46-4D40-BC84-319E7C260D99")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests__pullRequestId_.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public virtual GitPullRequest GetPullRequestById(int pullRequestId)
    {
      GitPullRequest requestByIdInternal = this.GetPullRequestByIdInternal(pullRequestId);
      GitStatusStateMapper.MapGitEntity<GitPullRequest>(requestByIdInternal, this.TfsRequestContext);
      return requestByIdInternal;
    }

    [HttpGet]
    [ClientResponseType(typeof (IList<GitPullRequest>), null, null)]
    [ClientLocationId("A5D28130-9CD2-40FA-9F08-902E7DAA9EFB")]
    [ClientExample("GET__git_pullRequests.json", "Pull requests by project", null, null)]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetPullRequestsByProject(
      [ModelBinder] GitPullRequestSearchCriteria searchCriteria,
      int? maxCommentLength = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null)
    {
      return this.GetPullRequests((string) null, searchCriteria, maxCommentLength: maxCommentLength, skip: skip, top: top);
    }

    [HttpGet]
    [ClientResponseType(typeof (IList<GitPullRequest>), null, null)]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests.json", "Pull requests by repository", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests_status-completed.json", "Just completed pull requests", null, null)]
    [ClientExample("GET__git_repositories__repositoryId__pullRequests_targetRefName-refs_heads_master.json", "Targeting a specific branch", null, null)]
    [PublicProjectRequestRestrictions]
    public virtual HttpResponseMessage GetPullRequests(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ModelBinder] GitPullRequestSearchCriteria searchCriteria,
      [ClientIgnore] string projectId = null,
      int? maxCommentLength = null,
      [FromUri(Name = "$skip")] int? skip = null,
      [FromUri(Name = "$top")] int? top = null)
    {
      (List<GitPullRequest> gitPullRequests, ISecuredObject securedObject) requestsInternal = this.GetPullRequestsInternal(repositoryId, searchCriteria, projectId, skip, top);
      GitStatusStateMapper.MapGitEntity<List<GitPullRequest>>(requestsInternal.gitPullRequests, this.TfsRequestContext);
      return this.GenerateResponse<GitPullRequest>((IEnumerable<GitPullRequest>) requestsInternal.gitPullRequests, requestsInternal.securedObject);
    }

    [HttpPatch]
    [ClientLocationId("9946FD70-0D40-406E-B686-B4744CBBCC37")]
    [ClientResponseType(typeof (GitPullRequest), null, null)]
    [ClientExample("PATCH__git_repositories__repositoryId__pullRequests__pullRequestId_.json", "Update title", null, null)]
    [ClientExample("PATCH__git_repositories__repositoryId__pullRequests__pullRequestId_2.json", "Update description", null, null)]
    [ClientExample("PATCH__git_repositories__repositoryId__pullRequests__autoCompletePullRequestId_.json", "Enable auto-completion and set other completion options", null, null)]
    public virtual HttpResponseMessage UpdatePullRequest(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      GitPullRequest gitPullRequestToUpdate,
      [ClientIgnore] string projectId = null)
    {
      GitPullRequest entity = this.UpdatePullRequestInternal(repositoryId, pullRequestId, gitPullRequestToUpdate, projectId, false);
      GitStatusStateMapper.MapGitEntity<GitPullRequest>(entity, this.TfsRequestContext);
      return this.Request.CreateResponse<GitPullRequest>(HttpStatusCode.OK, entity);
    }

    protected GitPullRequest UpdatePullRequestInternal(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      [ClientParameterType(typeof (int), false)] string pullRequestId,
      GitPullRequest gitPullRequestToUpdate,
      [ClientIgnore] string projectId = null,
      bool useStrictBypass = true)
    {
      if (gitPullRequestToUpdate == null)
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestToUpdateInformation"));
      if (gitPullRequestToUpdate.Title != null && gitPullRequestToUpdate.Title.Trim().Length == 0)
        throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestTitle"));
      using (this.TfsRequestContext.TimeRegion(this.TraceArea, WebApiTraceLayers.Controller, regionName: nameof (UpdatePullRequestInternal), tracepoint: 1013211))
      {
        using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
        {
          if (gitPullRequestToUpdate.Repository != null && gitPullRequestToUpdate.Repository.Id != Guid.Empty && gitPullRequestToUpdate.Repository.Id != tfsGitRepository.Key.RepoId)
            throw new InvalidArgumentValueException(Resources.Get("MismatchRepositoryId"));
          if (tfsGitRepository.IsInMaintenance)
            throw new GitRepoInMaintenanceException(MaintenanceMessageUtils.GetSanitizedMaintenanceMessage(this.TfsRequestContext));
          int result1;
          if (!int.TryParse(pullRequestId, out result1))
            throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestId"));
          if (this.TfsRequestContext.IsFeatureEnabled("SourceControl.EnableReturningPartiallySucceededGitStatusState") && gitPullRequestToUpdate?.Commits != null && ((IEnumerable<GitCommitRef>) gitPullRequestToUpdate.Commits).Any<GitCommitRef>((Func<GitCommitRef, bool>) (c => c?.Statuses != null && c.Statuses.Any<GitStatus>((Func<GitStatus, bool>) (s => s != null && s.State == GitStatusState.PartiallySucceeded)))))
            throw new InvalidArgumentValueException(Resources.Get("InvalidGitStatusStateValue"), "State");
          bool flag = this.TfsRequestContext.IsFeatureEnabled("SourceControl.GitPullRequests.Retarget");
          if (gitPullRequestToUpdate.CreationDate != new DateTime() || gitPullRequestToUpdate.ClosedDate != new DateTime() || gitPullRequestToUpdate.CreatedBy != null || gitPullRequestToUpdate.LastMergeCommit != null || gitPullRequestToUpdate.LastMergeSourceCommit != null && gitPullRequestToUpdate.Status != PullRequestStatus.Completed || gitPullRequestToUpdate.LastMergeTargetCommit != null || gitPullRequestToUpdate.MergeId != new Guid() || gitPullRequestToUpdate.PullRequestId != 0 || gitPullRequestToUpdate.Repository != null || gitPullRequestToUpdate.SourceRefName != null || !flag && gitPullRequestToUpdate.TargetRefName != null || gitPullRequestToUpdate.Reviewers != null)
            throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestUpdate"));
          if (flag && gitPullRequestToUpdate.TargetRefName != null)
            return this.RetargetPullRequest(tfsGitRepository, result1, gitPullRequestToUpdate);
          if (gitPullRequestToUpdate.CompletionOptions != null)
          {
            gitPullRequestToUpdate.CompletionOptions.Normalize();
            string str = gitPullRequestToUpdate.CompletionOptions.Serialize<GitPullRequestCompletionOptions>();
            int length = str != null ? str.Length : 0;
            if (length > 4000)
              throw new InvalidArgumentValueException(Resources.Format("PullRequestCompletionOptionsTooLong", (object) length, (object) 4000));
            if (!this.TfsRequestContext.IsFeatureEnabled("SourceControl.GitPullRequests.MergeStrategy.Rebase"))
            {
              GitPullRequestMergeStrategy? mergeStrategy = gitPullRequestToUpdate.CompletionOptions.MergeStrategy;
              if (mergeStrategy.HasValue)
              {
                switch (mergeStrategy.GetValueOrDefault())
                {
                  case GitPullRequestMergeStrategy.Rebase:
                  case GitPullRequestMergeStrategy.RebaseMerge:
                    throw new NotSupportedException(Resources.Get("MergeNotSupported"));
                }
              }
            }
          }
          TfsGitPullRequest pullRequest;
          if (gitPullRequestToUpdate.MergeStatus != PullRequestAsyncStatus.NotSet)
          {
            if (gitPullRequestToUpdate.MergeStatus != PullRequestAsyncStatus.Queued)
              throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestMergeStatusUpdate"));
            pullRequest = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().TryMerge(this.TfsRequestContext, tfsGitRepository, result1);
          }
          else if (gitPullRequestToUpdate.Status == PullRequestStatus.Completed)
          {
            GitCommitRef mergeSourceCommit = gitPullRequestToUpdate.LastMergeSourceCommit;
            int num1;
            if (mergeSourceCommit == null)
            {
              num1 = 1;
            }
            else
            {
              int? length = mergeSourceCommit.CommitId?.Length;
              int num2 = 40;
              num1 = !(length.GetValueOrDefault() == num2 & length.HasValue) ? 1 : 0;
            }
            if (num1 != 0)
              throw new InvalidArgumentValueException(Resources.Get("LastMergeSourceCommitRequired"));
            pullRequest = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().TryCommit(this.TfsRequestContext, tfsGitRepository, result1, GitCommitUtility.ParseSha1Id(gitPullRequestToUpdate.LastMergeSourceCommit.CommitId), gitPullRequestToUpdate.CompletionOptions, useStrictBypass);
          }
          else
          {
            string description = gitPullRequestToUpdate.Description;
            if ((description != null ? (description.Length > 4000 ? 1 : 0) : 0) != 0)
              throw new InvalidArgumentValueException(Resources.Format("PullRequestDescriptionTooLong", (object) 4000));
            Guid? autoCompleteAuthority = new Guid?();
            Guid result2 = new Guid();
            string id = gitPullRequestToUpdate.AutoCompleteSetBy?.Id;
            if (id != null)
            {
              if (!Guid.TryParse(id, out result2))
                throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestAutoCompleteSetById"));
              if (result2 != this.TfsRequestContext.GetUserIdentity().Id && result2 != Guid.Empty)
                throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestAutoCompleteSetById"));
              autoCompleteAuthority = new Guid?(result2);
              if (gitPullRequestToUpdate?.CompletionOptions?.BypassPolicy.GetValueOrDefault())
                throw new InvalidArgumentValueException(Resources.Get("BypassOptionNotAllowedWithAutoComplete"));
            }
            if (autoCompleteAuthority.HasValue)
            {
              Guid? nullable1 = autoCompleteAuthority;
              Guid empty = Guid.Empty;
              if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
              {
                Guid? nullable2 = autoCompleteAuthority;
                Guid userId = this.TfsRequestContext.GetUserId();
                if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != userId ? 1 : 0) : 0) : 1) != 0)
                  throw new InvalidArgumentValueException(Resources.Get("InvalidPullRequestUpdate"));
              }
            }
            pullRequest = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().UpdatePullRequest(this.TfsRequestContext, tfsGitRepository, result1, gitPullRequestToUpdate.Status, gitPullRequestToUpdate.Title, gitPullRequestToUpdate.Description, gitPullRequestToUpdate.CompletionOptions, gitPullRequestToUpdate.MergeOptions, autoCompleteAuthority, gitPullRequestToUpdate.IsDraft);
          }
          return pullRequest.ToWebApiItem(this.TfsRequestContext, tfsGitRepository, pullRequest.GetForkRepositoryRef(this.TfsRequestContext, tfsGitRepository), includeLinks: true);
        }
      }
    }

    protected GitPullRequest RetargetPullRequest(
      ITfsGitRepository repository,
      int pullRequestId,
      GitPullRequest gitPullRequestToUpdate)
    {
      TfsGitPullRequest pullRequest = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().RetargetPullRequest(this.TfsRequestContext, repository, pullRequestId, gitPullRequestToUpdate.TargetRefName);
      return pullRequest.ToWebApiItem(this.TfsRequestContext, repository, pullRequest.GetForkRepositoryRef(this.TfsRequestContext, repository), includeLinks: true);
    }

    private IEnumerable<int> GetWorkItemIds(ResourceRef[] workItemRefs)
    {
      int result;
      return workItemRefs != null ? ((IEnumerable<ResourceRef>) workItemRefs).Where<ResourceRef>((Func<ResourceRef, bool>) (workItem => workItem != null)).Select<ResourceRef, int>((Func<ResourceRef, int>) (workItem => !int.TryParse(workItem.Id, out result) ? 0 : result)).Distinct<int>() : Enumerable.Empty<int>();
    }

    protected void ValidateTimeRange(GitPullRequestSearchCriteria searchCriteria)
    {
      if (searchCriteria.QueryTimeRangeType.HasValue && !EnumUtility.IsDefined<PullRequestTimeRangeType>(searchCriteria.QueryTimeRangeType.Value))
        throw new InvalidArgumentValueException("QueryTimeRangeType");
      PullRequestTimeRangeType? queryTimeRangeType = searchCriteria.QueryTimeRangeType;
      PullRequestTimeRangeType requestTimeRangeType = PullRequestTimeRangeType.Closed;
      if (queryTimeRangeType.GetValueOrDefault() == requestTimeRangeType & queryTimeRangeType.HasValue && this.IsActiveStatus(searchCriteria.Status))
        throw new InvalidArgumentValueException("QueryTimeRangeType", "Active Pull Request do not have closed time.");
      if (!searchCriteria.MinTime.HasValue || !searchCriteria.MaxTime.HasValue)
        return;
      DateTime? minTime = searchCriteria.MinTime;
      DateTime? maxTime = searchCriteria.MaxTime;
      if ((minTime.HasValue & maxTime.HasValue ? (minTime.GetValueOrDefault() > maxTime.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        throw new InvalidArgumentValueException("MinTime", "Value of minTime should be earlier than maxTime.");
    }

    protected GitPullRequest GetGitPullRequestInternal(
      string repositoryId,
      int pullRequestId,
      string projectId,
      bool includeCommits,
      bool includeWorkItemRefs)
    {
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
      {
        TfsGitPullRequest pullRequestDetails = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequestDetails == null)
          throw new GitPullRequestNotFoundException();
        return pullRequestDetails.ToWebApiItem(this.TfsRequestContext, tfsGitRepository, pullRequestDetails.GetForkRepositoryRef(this.TfsRequestContext, tfsGitRepository), includeLinks: true, includeCommits: includeCommits, includeWorkItemRefs: includeWorkItemRefs);
      }
    }

    protected GitPullRequest GetPullRequestByIdInternal(int pullRequestId)
    {
      TfsGitPullRequest pullRequestDetails = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestDetails(this.TfsRequestContext, pullRequestId);
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(pullRequestDetails.RepositoryId.ToString()))
        return pullRequestDetails.ToWebApiItem(this.TfsRequestContext, tfsGitRepository);
    }

    protected (List<GitPullRequest> gitPullRequests, ISecuredObject securedObject) GetPullRequestsInternal(
      string repositoryId,
      GitPullRequestSearchCriteria searchCriteria,
      string projectId,
      int? skip,
      int? top)
    {
      if (searchCriteria.Status.HasValue && !EnumUtility.IsDefined<PullRequestStatus>(searchCriteria.Status.Value))
        throw new InvalidArgumentValueException("Status");
      this.ValidateTimeRange(searchCriteria);
      using (this.TfsRequestContext.TimeRegion(this.TraceArea, WebApiTraceLayers.Controller, regionName: nameof (GetPullRequestsInternal), tracepoint: 1013210))
      {
        Guid? nullable1 = new Guid?();
        ISecuredObject securedObject = (ISecuredObject) null;
        string teamProjectUri;
        if (!string.IsNullOrWhiteSpace(repositoryId))
        {
          using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId, projectId))
          {
            teamProjectUri = tfsGitRepository.Key.GetProjectUri();
            nullable1 = new Guid?(tfsGitRepository.Key.RepoId);
            securedObject = GitSecuredObjectFactory.CreateRepositoryReadOnly(tfsGitRepository.Key);
          }
        }
        else
        {
          ProjectInfo projectInfo = (ProjectInfo) null;
          teamProjectUri = this.GetProjectFilter(projectId, out projectInfo);
          if (projectInfo != null)
            securedObject = SharedSecuredObjectFactory.CreateTeamProjectReadOnly(projectInfo.Id);
        }
        top = new int?(Math.Min(top ?? 101, 1000));
        skip = new int?(Math.Max(skip.GetValueOrDefault(), 0));
        PullRequestStatus? statusFilter;
        ref PullRequestStatus? local = ref statusFilter;
        PullRequestStatus? nullable2 = searchCriteria.Status;
        int num1 = (int) nullable2 ?? 1;
        local = new PullRequestStatus?((PullRequestStatus) num1);
        nullable2 = statusFilter;
        PullRequestStatus pullRequestStatus = PullRequestStatus.All;
        if (nullable2.GetValueOrDefault() == pullRequestStatus & nullable2.HasValue)
          statusFilter = new PullRequestStatus?();
        IEnumerable<string> sourceBranchFilters = (IEnumerable<string>) null;
        IEnumerable<string> targetBranchFilters = (IEnumerable<string>) null;
        if (searchCriteria.SourceRefName != null)
          sourceBranchFilters = (IEnumerable<string>) new string[1]
          {
            searchCriteria.SourceRefName
          };
        if (searchCriteria.TargetRefName != null)
          targetBranchFilters = (IEnumerable<string>) new string[1]
          {
            searchCriteria.TargetRefName
          };
        IEnumerable<TfsGitPullRequest> pullRequests = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>().QueryPullRequests(this.TfsRequestContext, teamProjectUri, nullable1 ?? searchCriteria.RepositoryId, searchCriteria.SourceRepositoryId, sourceBranchFilters, targetBranchFilters, new bool?(false), statusFilter, searchCriteria.CreatorId, searchCriteria.ReviewerId, timeFilter: new TimeFilter(searchCriteria), top: top.Value, skip: skip.Value);
        bool flag = this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(this.TfsRequestContext, "WebAccess.VersionControl.PullRequests.Labels");
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        int num2 = searchCriteria.IncludeLinks ? 1 : 0;
        int num3 = flag ? 1 : 0;
        return (pullRequests.ToWebApiItems(tfsRequestContext, includeLinks: num2 != 0, includeLabels: num3 != 0).ToList<GitPullRequest>(), securedObject);
      }
    }

    private bool IsActiveStatus(PullRequestStatus? status)
    {
      if (status.HasValue)
      {
        PullRequestStatus? nullable = status;
        PullRequestStatus pullRequestStatus1 = PullRequestStatus.Active;
        if (!(nullable.GetValueOrDefault() == pullRequestStatus1 & nullable.HasValue))
        {
          nullable = status;
          PullRequestStatus pullRequestStatus2 = PullRequestStatus.NotSet;
          return nullable.GetValueOrDefault() == pullRequestStatus2 & nullable.HasValue;
        }
      }
      return true;
    }
  }
}
