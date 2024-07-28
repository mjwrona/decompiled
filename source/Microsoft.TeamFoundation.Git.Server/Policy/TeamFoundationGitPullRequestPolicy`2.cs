// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitPullRequestPolicy`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  public abstract class TeamFoundationGitPullRequestPolicy<TSettings, TContext> : 
    TeamFoundationPolicy<TSettings, TContext, GitBranchNameTarget>,
    ITeamFoundationGitPullRequestPolicy,
    ITeamFoundationPolicy,
    ITfsGitRefUpdatePolicy,
    IPolicySettingsAuditDetailsProvider
    where TSettings : TeamFoundationGitPullRequestPolicySettings
    where TContext : TeamFoundationPolicyEvaluationRecordContext
  {
    private const string c_layer = "TeamFoundationGitPullRequestPolicy";
    private const string c_doNotApplyApprovers = "DO_NOT_APPLY_APPROVERS";

    protected virtual bool SupportsFileScopes => false;

    protected virtual bool AppliesToDraftPullRequests => false;

    protected virtual bool CanBeOverridenByTags => false;

    protected override sealed void Initialize(IVssRequestContext requestContext) => base.Initialize(requestContext);

    protected override sealed bool IsApplicableTo(
      IVssRequestContext requestContext,
      GitBranchNameTarget target)
    {
      using (requestContext.TimeRegion(nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), nameof (IsApplicableTo)))
      {
        if ((target is GitPullRequestTarget pullRequestTarget1 ? (pullRequestTarget1.PullRequest.IsDraft ? 1 : 0) : 0) != 0 && !this.AppliesToDraftPullRequests)
          return false;
        if (this.CanBeOverridenByTags && requestContext.IsFeatureEnabled("Git.PullRequests.TagCanOverrideApproverPolicy"))
        {
          requestContext.To(TeamFoundationHostType.Deployment);
          string userAllowedToIgnoreApprovers = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Service/Git/Settings/UserIgnoresApprovers", string.Empty);
          TfsGitPullRequest pullRequest = target is GitPullRequestTarget pullRequestTarget2 ? pullRequestTarget2.PullRequest : (TfsGitPullRequest) null;
          if (pullRequest != null && !string.IsNullOrEmpty(userAllowedToIgnoreApprovers) && Guid.Parse(userAllowedToIgnoreApprovers).Equals(pullRequest.Creator))
          {
            using (ITfsGitRepository repositoryOrDefault = GitRequestContextCacheUtil.GetRepositoryOrDefault(requestContext, pullRequest.RepositoryId, true))
            {
              if (requestContext.GetService<ITeamFoundationGitPullRequestService>().GetPullRequestLabels(requestContext, repositoryOrDefault, pullRequest).ToApiTagDefinitions().Any<WebApiTagDefinition>((Func<WebApiTagDefinition, bool>) (label => label.Name.Equals("DO_NOT_APPLY_APPROVERS", StringComparison.InvariantCulture))))
              {
                TracepointUtils.TraceAlways(requestContext, 1013814, this.Area, nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), (object) (() => new
                {
                  RepositoryId = pullRequest.RepositoryId,
                  PullRequestId = pullRequest.PullRequestId,
                  userAllowedToIgnoreApprovers = userAllowedToIgnoreApprovers
                }), caller: nameof (IsApplicableTo));
                return false;
              }
            }
          }
        }
        bool flag = this.Settings.Scope == null || target.IsInScope(this.Settings.Scope);
        if (this.SupportsFileScopes & flag)
        {
          IReadOnlyList<string> filenamePatterns = this.Settings.FilenamePatterns;
          if ((filenamePatterns != null ? (filenamePatterns.Count > 0 ? 1 : 0) : 0) != 0 && target is GitPullRequestTarget && ((GitPullRequestTarget) target).PullRequest != null)
            return this.IsApplicableDueToAFilePathMatch(requestContext, ((GitPullRequestTarget) target).PullRequest);
        }
        return flag;
      }
    }

    protected bool IsApplicableDueToAFilePathMatch(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      Stopwatch timer = Stopwatch.StartNew();
      IReadOnlyList<TfsGitCommitChange> changes = (IReadOnlyList<TfsGitCommitChange>) null;
      HashSet<string> paths = (HashSet<string>) null;
      try
      {
        if (this.Settings.IgnoreIfSourceIsInScope && this.Settings.Scope.IsInScope(pullRequest.RepositoryId, pullRequest.SourceBranchName, DefaultBranchUtils.IsPullRequestSourceDefaultBranch(requestContext, pullRequest)) || this.Settings.FilenamePatterns == null || this.Settings.FilenamePatterns.Count < 1)
          return false;
        changes = this.GetChangesetForPolicyApplicability(requestContext, pullRequest);
        using (requestContext.TimeRegion(nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), "FindPathsWhichChanged"))
          paths = this.FindPathsWhichChanged(requestContext, (IEnumerable<TfsGitCommitChange>) changes);
        using (requestContext.TimeRegion(nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), "CheckPathsMatchPatterns"))
          return PathUtils.FastCheckPathsMatchPatterns((IEnumerable<string>) paths, (IEnumerable<string>) this.Settings.FilenamePatterns);
      }
      finally
      {
        this.ReportPerformance(requestContext, pullRequest, timer, false, changes?.Count, paths?.Count);
      }
    }

    protected bool IsApplicableDueToAFilePathMatchWithStatistics(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      out string exampleMatchingFile,
      out int countOfMatchingFiles)
    {
      exampleMatchingFile = (string) null;
      countOfMatchingFiles = 0;
      Stopwatch timer = Stopwatch.StartNew();
      IEnumerable<TfsGitCommitChange> tfsGitCommitChanges = (IEnumerable<TfsGitCommitChange>) null;
      HashSet<string> paths = (HashSet<string>) null;
      try
      {
        if (this.Settings.IgnoreIfSourceIsInScope && this.Settings.Scope.IsInScope(pullRequest.RepositoryId, pullRequest.SourceBranchName, DefaultBranchUtils.IsPullRequestSourceDefaultBranch(requestContext, pullRequest)) || this.Settings.FilenamePatterns == null || this.Settings.FilenamePatterns.Count < 1)
          return false;
        tfsGitCommitChanges = (IEnumerable<TfsGitCommitChange>) this.GetChangesetForPolicyApplicability(requestContext, pullRequest);
        using (requestContext.TimeRegion(nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), "FindPathsWhichChanged"))
          paths = this.FindPathsWhichChanged(requestContext, tfsGitCommitChanges);
        using (requestContext.TimeRegion(nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), "CheckPathsMatchPatterns"))
          return PathUtils.CheckPathsMatchPatterns((IEnumerable<string>) paths, (IEnumerable<string>) this.Settings.FilenamePatterns, out exampleMatchingFile, out countOfMatchingFiles);
      }
      finally
      {
        this.ReportPerformance(requestContext, pullRequest, timer, true, tfsGitCommitChanges != null ? new int?(tfsGitCommitChanges.Count<TfsGitCommitChange>()) : new int?(), paths?.Count);
      }
    }

    internal virtual IReadOnlyList<TfsGitCommitChange> GetChangesetForPolicyApplicability(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      return pullRequest.GetChangesetForPolicyApplicability(requestContext);
    }

    private HashSet<string> FindPathsWhichChanged(
      IVssRequestContext requestContext,
      IEnumerable<TfsGitCommitChange> changes)
    {
      changes = !this.Settings.AddedFilesOnly ? changes.Where<TfsGitCommitChange>((Func<TfsGitCommitChange, bool>) (c => c.ChangeType == TfsGitChangeType.Add || c.ChangeType == TfsGitChangeType.Edit || c.ChangeType == TfsGitChangeType.Delete)) : changes.Where<TfsGitCommitChange>((Func<TfsGitCommitChange, bool>) (c => c.ChangeType == TfsGitChangeType.Add));
      IEqualityComparer<string> ordinal = (IEqualityComparer<string>) StringComparer.Ordinal;
      return new HashSet<string>(changes.Select<TfsGitCommitChange, string>((Func<TfsGitCommitChange, string>) (c => c.ParentPath + (c.ParentPath.EndsWith("/", StringComparison.Ordinal) ? "" : "/") + c.ChildItem)), ordinal);
    }

    private void ReportPerformance(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Stopwatch timer,
      bool withStats,
      int? fileCount,
      int? changedPathsCount)
    {
      if (!requestContext.IsFeatureEnabled("Git.PullRequests.PolicyReportStatistics"))
        return;
      timer.Stop();
      if (timer.ElapsedMilliseconds < 10L)
        return;
      string format = JsonConvert.SerializeObject((object) new Dictionary<string, object>()
      {
        ["withAdditionalStats"] = (object) withStats,
        ["repositoryId"] = (object) pullRequest.RepositoryId,
        ["pullRequestId"] = (object) pullRequest.PullRequestId,
        ["policyType"] = (object) this.GetType().Name,
        ["policyId"] = (object) this.Configuration.ConfigurationId,
        ["filesCount"] = (object) fileCount.GetValueOrDefault(),
        [nameof (changedPathsCount)] = (object) changedPathsCount.GetValueOrDefault(),
        ["filePatternsCount"] = (object) this.Settings?.FilenamePatterns.Count,
        ["elapsedMs"] = (object) timer.ElapsedMilliseconds,
        ["addedFilesOnly"] = (object) this.Settings.AddedFilesOnly
      });
      requestContext.TraceAlways(1013921, TraceLevel.Info, this.Area, nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), format, (object[]) null);
    }

    public override void AppendClientTraceData(TSettings settings, ref ClientTraceData eventData)
    {
      base.AppendClientTraceData(settings, ref eventData);
      if ((object) settings != null && settings.Scope != null && settings.Scope.ScopeItems != null)
      {
        eventData.Add("ScopeCount", (object) settings.Scope.ScopeItems.Count);
        if (settings.Scope.ScopeItems.Count == 1 && settings.Scope.ScopeItems[0].RepositoryId.HasValue)
          eventData.Add("ScopeRepositoryId", (object) settings.Scope.ScopeItems[0].RepositoryId.ToString());
      }
      if (settings.FilenamePatterns == null)
        return;
      eventData.Add("FilenamePatternsCount", (object) settings.FilenamePatterns.Count<string>());
    }

    public override bool CheckSettingsValidity(
      IVssRequestContext requestContext,
      Guid projectId,
      TSettings settings,
      out string errorMessage)
    {
      ArgumentUtility.CheckForNull<TSettings>(settings, nameof (settings));
      if (!base.CheckSettingsValidity(requestContext, projectId, settings, out errorMessage) || !settings.Scope.CheckValidity(requestContext, projectId, out errorMessage))
        return false;
      if (!this.SupportsFileScopes)
      {
        IReadOnlyList<string> filenamePatterns = settings.FilenamePatterns;
        if ((filenamePatterns != null ? (filenamePatterns.Count<string>() > 0 ? 1 : 0) : 0) != 0 || settings.AddedFilesOnly || settings.IgnoreIfSourceIsInScope)
        {
          errorMessage = Microsoft.TeamFoundation.Git.Server.Resources.Format("PolicyScopeFileScopesNotSupported");
          return false;
        }
      }
      errorMessage = (string) null;
      return true;
    }

    public override string[] GetScopes(TSettings settings) => settings.Scope.FlattenScope();

    public override void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      TSettings settings)
    {
      if ((object) settings == null || settings.Scope == null)
        SecurityHelper.Instance.CheckEditPoliciesPermission(requestContext, new RepoScope(projectId, Guid.Empty));
      else
        settings.Scope.CheckEditPoliciesPermission(requestContext, projectId);
    }

    public override sealed void CheckSupportedMatchKind(
      IVssRequestContext requestContext,
      TSettings settings)
    {
      if (settings?.Scope == null)
        return;
      settings.Scope.CheckSupportedMatchKind(requestContext);
    }

    protected virtual Dictionary<string, JToken> SummarizePolicyConfiguration(
      IVssRequestContext requestContext,
      TSettings settings)
    {
      Dictionary<string, JToken> dictionary = new Dictionary<string, JToken>();
      if (this.SupportsFileScopes)
      {
        if (settings.AddedFilesOnly)
          dictionary["AddedFilesOnly"] = (JToken) new JValue(true);
        if (settings.IgnoreIfSourceIsInScope)
          dictionary["IgnoreIfSourceIsInScope"] = (JToken) new JValue(true);
        IReadOnlyList<string> filenamePatterns = settings.FilenamePatterns;
        if ((filenamePatterns != null ? (filenamePatterns.Count > 0 ? 1 : 0) : 0) != 0)
          dictionary["FilenamePatterns"] = (JToken) new JArray((object) settings.FilenamePatterns);
      }
      return dictionary;
    }

    Dictionary<string, JToken> IPolicySettingsAuditDetailsProvider.SummarizePolicyScope(
      IVssRequestContext requestContext,
      object settings)
    {
      TSettings settings1 = (TSettings) settings;
      Dictionary<string, JToken> dictionary1 = new Dictionary<string, JToken>();
      GitPolicyRefScope scope = settings1.Scope;
      dictionary1["Scope"] = (JToken) new JArray((object[]) scope.FlattenScope());
      int valueOrDefault = scope?.ScopeItems?.Count.GetValueOrDefault();
      dictionary1["ScopeCount"] = (JToken) new JValue((long) valueOrDefault);
      if (valueOrDefault == 1)
      {
        GitPolicyRepositoryScopeItem scopeItem = scope.ScopeItems[0];
        Guid? repositoryId1 = scopeItem.RepositoryId;
        if (repositoryId1.HasValue)
        {
          Dictionary<string, JToken> dictionary2 = dictionary1;
          repositoryId1 = scopeItem.RepositoryId;
          JValue jvalue = new JValue(repositoryId1.Value.ToString("N"));
          dictionary2["RepoId"] = (JToken) jvalue;
          ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
          IVssRequestContext requestContext1 = requestContext;
          repositoryId1 = scopeItem.RepositoryId;
          Guid repositoryId2 = repositoryId1.Value;
          ITfsGitRepository tfsGitRepository;
          ref ITfsGitRepository local = ref tfsGitRepository;
          if (service.TryFindRepositoryById(requestContext1, repositoryId2, false, out local))
          {
            dictionary1["RepoName"] = (JToken) new JValue(tfsGitRepository.Name);
            tfsGitRepository.Dispose();
          }
        }
        if (scopeItem is GitPolicyRefScopeItem policyRefScopeItem)
        {
          string refName = policyRefScopeItem.RefName;
          if (policyRefScopeItem.MatchKind == RefNameMatchType.Prefix)
            refName += refName.EndsWith("/") ? "*" : "/*";
          dictionary1["RefName"] = (JToken) new JValue(refName);
        }
      }
      return dictionary1;
    }

    public virtual string AuditDisplayName => this.DisplayName;

    Dictionary<string, JToken> IPolicySettingsAuditDetailsProvider.SummarizePolicySettings(
      IVssRequestContext requestContext,
      object settings)
    {
      return this.SummarizePolicyConfiguration(requestContext, (TSettings) settings);
    }

    protected abstract PolicyCheckResult<TContext> CheckRefUpdate(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string name,
      Sha1Id oldObjectId,
      Sha1Id newObjectId,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext);

    protected virtual PolicyCheckResult<TContext> CheckRefUpdate(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string name,
      Sha1Id oldObjectId,
      Sha1Id newObjectId)
    {
      return newObjectId == Sha1Id.Empty ? PolicyCheckResult.Rejected<TContext>(Microsoft.TeamFoundation.Git.Server.Resources.Format("CannotDeleteProtectedBranch"), default (TContext)) : PolicyCheckResult.Rejected<TContext>(Microsoft.TeamFoundation.Git.Server.Resources.Format("PolicyPullRequestRequired"), default (TContext));
    }

    protected abstract PolicyCheckResult<TContext> CheckEnterCompletionQueue(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPullRequestCompletionOptions completionOptions,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext);

    protected virtual AutoCompleteStatus GetAutoCompleteStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPullRequestCompletionOptions completionOptions,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      if (this.Configuration.IsBlocking)
      {
        PolicyEvaluationStatus? nullable = existingStatus;
        PolicyEvaluationStatus evaluationStatus1 = PolicyEvaluationStatus.Approved;
        if (!(nullable.GetValueOrDefault() == evaluationStatus1 & nullable.HasValue))
        {
          nullable = existingStatus;
          PolicyEvaluationStatus evaluationStatus2 = PolicyEvaluationStatus.NotApplicable;
          if (!(nullable.GetValueOrDefault() == evaluationStatus2 & nullable.HasValue))
            return AutoCompleteStatus.Blocking;
        }
      }
      return AutoCompleteStatus.NotBlocking;
    }

    public virtual PolicyNotificationResult<TContext> OnCreated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      return PolicyNotificationResult.Queued<TContext>(default (TContext));
    }

    public virtual PolicyNotificationResult<TContext> OnReactivated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return PolicyNotificationResult.Queued<TContext>(default (TContext));
    }

    public virtual PolicyNotificationResult<TContext> OnAbandoned(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual TContext OnCommitted(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus statusFromCheckRefUpdate,
      TContext contextFromCheckRefUpdate)
    {
      return contextFromCheckRefUpdate;
    }

    public virtual PolicyNotificationResult<TContext> OnReviewerVoteChanged(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      TeamFoundationIdentity reviewer,
      ReviewerVote vote,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnSourceBranchUpdated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Sha1Id oldCommit,
      Sha1Id newCommit,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnTargetBranchWillChange(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      string newTargetRef,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnTargetBranchChanged(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      string newTargetRef,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnPublished(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnNewMergeCommitAvailable(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Sha1Id mergeCommit,
      Sha1Id conflictResolutionHash,
      bool inCompletionQueue,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnReviewerListUpdated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      IEnumerable<Guid> addedReviewers,
      IEnumerable<Guid> removedReviewers,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnNewPullRequestStatusAdded(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestStatus pullRequestStatus,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    public virtual PolicyNotificationResult<TContext> OnPullRequestStatusesDeleted(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    protected virtual PolicyNotificationResult<TContext> Requeue(
      IVssRequestContext requestContext,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext,
      TfsGitPullRequest pullRequest,
      bool triggeredByAutoComplete = false)
    {
      return (PolicyNotificationResult<TContext>) null;
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnCreated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest)
    {
      return (PolicyNotificationResult) this.OnCreated(requestContext, pullRequest);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnReactivated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnReactivated(requestContext, pullRequest, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnAbandoned(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnAbandoned(requestContext, pullRequest, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnReviewerVoteChanged(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      TeamFoundationIdentity reviewer,
      ReviewerVote vote,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnReviewerVoteChanged(requestContext, pullRequest, reviewer, vote, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnReviewerListUpdated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      IEnumerable<Guid> addedReviewers,
      IEnumerable<Guid> removedReviewers,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnReviewerListUpdated(requestContext, pullRequest, addedReviewers, removedReviewers, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnSourceBranchUpdated(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Sha1Id oldCommit,
      Sha1Id newCommit,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnSourceBranchUpdated(requestContext, pullRequest, oldCommit, newCommit, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnTargetBranchWillChange(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      string newTargetRef,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnTargetBranchWillChange(requestContext, pullRequest, newTargetRef, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnTargetBranchChanged(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      string newTargetRef,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnTargetBranchChanged(requestContext, pullRequest, newTargetRef, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnPublished(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnPublished(requestContext, pullRequest, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnNewMergeCommitAvailable(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      Sha1Id mergeCommit,
      Sha1Id conflictResolutionHash,
      bool inCompletionQueue,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnNewMergeCommitAvailable(requestContext, pullRequest, mergeCommit, conflictResolutionHash, inCompletionQueue, existingStatus, (TContext) existingContext);
    }

    PolicyCheckResult ITeamFoundationGitPullRequestPolicy.CheckRefUpdate(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      string name,
      Sha1Id oldObjectId,
      Sha1Id newObjectId,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyCheckResult) this.CheckRefUpdate(requestContext, repository, pullRequest, name, oldObjectId, newObjectId, existingStatus, (TContext) existingContext);
    }

    PolicyCheckResult ITfsGitRefUpdatePolicy.CheckRefUpdate(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      string name,
      Sha1Id oldObjectId,
      Sha1Id newObjectId)
    {
      return (PolicyCheckResult) this.CheckRefUpdate(requestContext, repository, name, oldObjectId, newObjectId);
    }

    PolicyCheckResult ITeamFoundationGitPullRequestPolicy.CheckEnterCompletionQueue(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPullRequestCompletionOptions completionOptions,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyCheckResult) this.CheckEnterCompletionQueue(requestContext, repository, completionOptions, pullRequest, existingStatus, (TContext) existingContext);
    }

    AutoCompleteStatus ITeamFoundationGitPullRequestPolicy.GetAutoCompleteStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPullRequestCompletionOptions completionOptions,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return this.GetAutoCompleteStatus(requestContext, repository, completionOptions, pullRequest, existingStatus, (TContext) existingContext);
    }

    TeamFoundationPolicyEvaluationRecordContext ITeamFoundationGitPullRequestPolicy.OnCommitted(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus statusFromCheckRefUpdate,
      TeamFoundationPolicyEvaluationRecordContext contextFromCheckRefUpdate)
    {
      try
      {
        return (TeamFoundationPolicyEvaluationRecordContext) this.OnCommitted(requestContext, pullRequest, statusFromCheckRefUpdate, (TContext) contextFromCheckRefUpdate);
      }
      catch (Exception ex)
      {
        PolicyImplementationException implementationException = new PolicyImplementationException(this.DisplayName, new int?(this.Configuration.ConfigurationId), ex);
        requestContext.TraceException(1390062, GitServerUtils.TraceArea, nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), (Exception) implementationException);
        return contextFromCheckRefUpdate;
      }
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnNewPullRequestStatusAdded(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      GitPullRequestStatus pullRequestStatus,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnNewPullRequestStatusAdded(requestContext, repository, pullRequest, pullRequestStatus, existingStatus, (TContext) existingContext);
    }

    PolicyNotificationResult ITeamFoundationGitPullRequestPolicy.OnPullRequestStatusesDeleted(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      return (PolicyNotificationResult) this.OnPullRequestStatusesDeleted(requestContext, repository, pullRequest, existingStatus, (TContext) existingContext);
    }

    public override sealed PolicyNotificationResult Requeue(
      IVssRequestContext requestContext,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext,
      ITeamFoundationPolicyTarget target)
    {
      return (PolicyNotificationResult) this.Requeue(requestContext, existingStatus, (TContext) existingContext, ((GitPullRequestTarget) target).PullRequest);
    }

    public PolicyNotificationResult AutoRequeue(
      IVssRequestContext requestContext,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext,
      GitPullRequestTarget target)
    {
      return (PolicyNotificationResult) this.Requeue(requestContext, existingStatus, (TContext) existingContext, target.PullRequest, true);
    }

    protected void AddDiscussionThread(
      IVssRequestContext requestContext,
      string teamProjectUri,
      TfsGitPullRequest pullRequest,
      PropertiesCollection extendedProperties)
    {
      DiscussionThread discussionThread = (DiscussionThread) new ArtifactDiscussionThread();
      discussionThread.ArtifactUri = pullRequest.BuildArtifactUriForDiscussions(teamProjectUri);
      requestContext.Trace(1013344, TraceLevel.Verbose, GitServerUtils.TraceArea, nameof (TeamFoundationGitPullRequestPolicy<TSettings, TContext>), "Creating a discussion message for policy: {0} on pull request: {1} team project: {2}", (object) this.GetType().Name, (object) pullRequest.PullRequestId, (object) teamProjectUri);
      DiscussionComment discussionComment = new DiscussionComment();
      discussionComment.CommentType = Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.System;
      discussionComment.DiscussionId = discussionThread.DiscussionId;
      discussionComment.Content = Microsoft.TeamFoundation.Git.Server.Resources.Format("PolicyDefaultDiscussionComment");
      discussionThread.Properties = new PropertiesCollection((IDictionary<string, object>) extendedProperties)
      {
        ["CodeReviewThreadType"] = (object) "PolicyStatusUpdate",
        ["CodeReviewPolicyType"] = (object) this.Id.ToString()
      };
      requestContext.GetService<ITeamFoundationDiscussionService>().PublishDiscussions(requestContext, new DiscussionThread[1]
      {
        discussionThread
      }, new DiscussionComment[1]{ discussionComment }, (CommentId[]) null, out List<short> _, out DateTime _);
    }

    protected internal virtual Sha1Id ConflictResolutionHashForCurrentMerge(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int pullRequestId)
    {
      return GitConflictService.GetConflictResolutionHash(requestContext, repository, GitConflictSourceType.PullRequest, pullRequestId);
    }
  }
}
