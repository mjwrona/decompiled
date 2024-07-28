// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.DefaultGitRefUpdateValidator
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Utils;
using Microsoft.TeamFoundation.Policy.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  internal class DefaultGitRefUpdateValidator : ITeamFoundationGitRefUpdateValidator
  {
    private const string c_layer = "DefaultGitRefUpdateValidator";

    public bool ValidatePolicies(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      TfsGitRefUpdateRequest refUpdate,
      out string message)
    {
      message = (string) null;
      requestContext.Trace(1013345, TraceLevel.Info, GitServerUtils.TraceArea, nameof (DefaultGitRefUpdateValidator), "Will policy check happen? OldObjectId: {0}, NewObjectId: {1}, RefName: {2}, IsRequestContextSystem: {3}", (object) refUpdate.OldObjectId, (object) refUpdate.NewObjectId, (object) refUpdate.Name, (object) requestContext.IsSystemContext);
      bool flag1 = true;
      bool flag2 = false;
      ITeamFoundationPolicyService service = requestContext.GetService<ITeamFoundationPolicyService>();
      try
      {
        if (!refUpdate.OldObjectId.IsEmpty)
        {
          PolicyEvaluationResult result = this.ValidatePRPoliciesInternal(requestContext, service, repository, refUpdate);
          flag1 &= this.ResultIsApproved(result, out message);
          flag2 |= result.RequiresBypass;
        }
        if (flag1)
        {
          PolicyEvaluationResult result = this.ValidatePushPoliciesInternal(requestContext, service, repository, refUpdate);
          string message1;
          flag1 &= this.ResultIsApproved(result, out message1);
          flag2 |= result.RequiresBypass;
          if (message1 != null)
            message = message1;
        }
      }
      catch (PolicyImplementationException ex)
      {
        requestContext.TraceException(1013341, GitServerUtils.TraceArea, nameof (DefaultGitRefUpdateValidator), (Exception) ex);
        message = ex.Message;
        flag1 = false;
      }
      if (flag1 & flag2)
      {
        try
        {
          Dictionary<string, object> dictionary = new Dictionary<string, object>();
          this.AddAuditDetails(dictionary);
          GitAuditLogHelper.RefUpdatePoliciesBypassed(requestContext, repository, refUpdate, dictionary);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1013881, GitServerUtils.TraceArea, nameof (DefaultGitRefUpdateValidator), ex);
        }
      }
      return flag1;
    }

    protected virtual void AddAuditDetails(Dictionary<string, object> details)
    {
    }

    protected virtual PolicyEvaluationResult ValidatePushPoliciesInternal(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyService policyService,
      ITfsGitRepository repository,
      TfsGitRefUpdateRequest refUpdate)
    {
      requestContext.Trace(1013346, TraceLevel.Info, GitServerUtils.TraceArea, nameof (DefaultGitRefUpdateValidator), "Going through CheckPolicies using the push policy validator.");
      return policyService.CheckPolicies<ITfsGitRefPushPolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitBranchNameTarget(repository.Key.GetProjectUri(), repository.Key.RepoId, refUpdate.Name, DefaultBranchUtils.IsDefaultBranch(requestContext, repository.Key.RepoId, refUpdate.Name)), (Func<ITfsGitRefPushPolicy, PolicyCheckResult>) (policy => policy.CheckRefUpdate(requestContext, repository, refUpdate.Name, refUpdate.OldObjectId, refUpdate.NewObjectId)));
    }

    protected virtual bool ResultIsApproved(PolicyEvaluationResult result, out string message)
    {
      message = (string) null;
      bool isPassed = result.IsPassed;
      if (isPassed && result.RequiresBypass)
        message = Resources.Format("PoliciesBypassed");
      else if (!isPassed)
        message = result.RejectionReason;
      return isPassed;
    }

    protected virtual PolicyEvaluationResult ValidatePRPoliciesInternal(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyService policyService,
      ITfsGitRepository repository,
      TfsGitRefUpdateRequest refUpdate)
    {
      requestContext.Trace(1013346, TraceLevel.Info, GitServerUtils.TraceArea, nameof (DefaultGitRefUpdateValidator), "Going through CheckPolicies using the default validator.");
      return policyService.CheckPolicies<ITfsGitRefUpdatePolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitBranchNameTarget(repository.Key.GetProjectUri(), repository.Key.RepoId, refUpdate.Name, DefaultBranchUtils.IsDefaultBranch(requestContext, repository.Key.RepoId, refUpdate.Name)), (Func<ITfsGitRefUpdatePolicy, PolicyCheckResult>) (policy => policy.CheckRefUpdate(requestContext, repository, refUpdate.Name, refUpdate.OldObjectId, refUpdate.NewObjectId)));
    }
  }
}
