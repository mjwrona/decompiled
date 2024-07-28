// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitPullRequestDynamicPolicy`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  public abstract class TeamFoundationGitPullRequestDynamicPolicy<TSettings, TContext> : 
    TeamFoundationGitPullRequestPolicy<TSettings, TContext>,
    IDynamicEvaluationPolicy,
    ITeamFoundationPolicy
    where TSettings : TeamFoundationGitPullRequestPolicySettings
    where TContext : TeamFoundationPolicyEvaluationRecordContext
  {
    protected override AutoCompleteStatus GetAutoCompleteStatus(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitPullRequestCompletionOptions completionOptions,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext)
    {
      if ((!requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.SelectiveAutoComplete") || completionOptions?.AutoCompleteIgnoreConfigIds == null || requestContext.IsFeatureEnabled("SourceControl.GitPullRequests.AutoCompleteLegacyNonBlockingBehavior")) && !this.Configuration.IsBlocking)
        return AutoCompleteStatus.NotBlocking;
      PolicyNotificationResult<TContext> notificationResult = this.OnDynamicEvaluation(requestContext, pullRequest, existingStatus, existingContext);
      if (notificationResult != null)
      {
        existingStatus = new PolicyEvaluationStatus?(notificationResult.Status);
        existingContext = notificationResult.Context;
      }
      PolicyEvaluationStatus? nullable1 = existingStatus;
      PolicyEvaluationStatus evaluationStatus1 = PolicyEvaluationStatus.Approved;
      if (!(nullable1.GetValueOrDefault() == evaluationStatus1 & nullable1.HasValue))
      {
        PolicyEvaluationStatus? nullable2 = existingStatus;
        PolicyEvaluationStatus evaluationStatus2 = PolicyEvaluationStatus.NotApplicable;
        if (!(nullable2.GetValueOrDefault() == evaluationStatus2 & nullable2.HasValue))
          return AutoCompleteStatus.Blocking;
      }
      return AutoCompleteStatus.NotBlocking;
    }

    PolicyNotificationResult IDynamicEvaluationPolicy.OnDynamicEvaluation(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      PolicyEvaluationStatus? existingStatus,
      TeamFoundationPolicyEvaluationRecordContext existingContext)
    {
      if (!(target is GitPullRequestTarget pullRequestTarget))
        return (PolicyNotificationResult) null;
      TfsGitPullRequest pullRequest = pullRequestTarget.PullRequest;
      return pullRequest.Status == PullRequestStatus.Completed ? (PolicyNotificationResult) null : (PolicyNotificationResult) this.OnDynamicEvaluation(requestContext, pullRequest, existingStatus, (TContext) existingContext);
    }

    public abstract PolicyNotificationResult<TContext> OnDynamicEvaluation(
      IVssRequestContext requestContext,
      TfsGitPullRequest pullRequest,
      PolicyEvaluationStatus? existingStatus,
      TContext existingContext);
  }
}
