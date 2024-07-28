// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TfsGitPullRequestRefUpdateValidator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  internal sealed class TfsGitPullRequestRefUpdateValidator : 
    DefaultGitRefUpdateValidator,
    IDisposable
  {
    private readonly TfsGitPullRequest m_pullRequest;
    private const string c_layer = "TfsGitPullRequestRefUpdateValidator";

    internal TfsGitPullRequestRefUpdateValidator(TfsGitPullRequest pullRequest)
    {
      this.m_pullRequest = pullRequest;
      this.Transaction = (PolicyEvaluationTransaction<ITeamFoundationGitPullRequestPolicy>) null;
    }

    internal PolicyEvaluationTransaction<ITeamFoundationGitPullRequestPolicy> Transaction { get; private set; }

    protected override void AddAuditDetails(Dictionary<string, object> auditDetails)
    {
      if (this.m_pullRequest == null)
        return;
      auditDetails["PullRequestId"] = (object) this.m_pullRequest.PullRequestId;
      auditDetails["BypassReason"] = (object) (this.m_pullRequest.CompletionOptions?.BypassReason ?? "");
      auditDetails["LastMergeSourceCommit"] = (object) this.m_pullRequest.LastMergeSourceCommit;
      auditDetails["LastMergeTargetCommit"] = (object) this.m_pullRequest.LastMergeTargetCommit;
    }

    protected override bool ResultIsApproved(PolicyEvaluationResult result, out string message)
    {
      message = (string) null;
      bool flag = result.IsPassed;
      if (flag && result.RequiresBypass)
      {
        TfsGitPullRequest pullRequest = this.m_pullRequest;
        if ((pullRequest != null ? (pullRequest.CompletionOptions.BypassPolicy ? 1 : 0) : 0) == 0)
        {
          flag = false;
          message = Resources.Format("GitRefPolicyFailure", (object) this.m_pullRequest.TargetBranchName, (object) result.RejectionReason);
          goto label_5;
        }
      }
      if (!flag)
        message = result.RejectionReason;
label_5:
      return flag;
    }

    protected override PolicyEvaluationResult ValidatePRPoliciesInternal(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyService policyService,
      ITfsGitRepository repository,
      TfsGitRefUpdateRequest refUpdate)
    {
      requestContext.Trace(1013347, TraceLevel.Info, GitServerUtils.TraceArea, nameof (TfsGitPullRequestRefUpdateValidator), "Pull request found: {0}", (object) this.m_pullRequest.PullRequestId);
      ArtifactId artifactId = this.m_pullRequest.BuildLegacyArtifactId(repository.Key.GetProjectUri());
      PolicyEvaluationResult result;
      this.Transaction = policyService.CheckPolicies<ITeamFoundationGitPullRequestPolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitPullRequestTarget(repository.Key.GetProjectUri(), this.m_pullRequest, DefaultBranchUtils.IsPullRequestTargetDefaultBranch(requestContext, this.m_pullRequest)), artifactId, out result, (Func<ITeamFoundationGitPullRequestPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult>) ((policy, existingStatus, existingContext) => policy.CheckRefUpdate(requestContext, repository, this.m_pullRequest, refUpdate.Name, refUpdate.OldObjectId, refUpdate.NewObjectId, existingStatus, existingContext)));
      return result;
    }

    public void Dispose()
    {
      PolicyEvaluationTransaction<ITeamFoundationGitPullRequestPolicy> transaction = this.Transaction;
      if (transaction == null)
        return;
      this.Transaction = (PolicyEvaluationTransaction<ITeamFoundationGitPullRequestPolicy>) null;
      transaction.Dispose();
    }
  }
}
