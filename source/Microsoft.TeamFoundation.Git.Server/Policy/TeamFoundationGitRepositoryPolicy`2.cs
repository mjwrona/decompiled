// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitRepositoryPolicy`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  public abstract class TeamFoundationGitRepositoryPolicy<TSettings, TContext> : 
    TeamFoundationPolicy<TSettings, TContext, GitRepositoryTarget>,
    ITeamFoundationGitRepositoryPolicy,
    ITeamFoundationPolicy
    where TSettings : ITeamFoundationGitRepositoryPolicySettings
    where TContext : TeamFoundationPolicyEvaluationRecordContext
  {
    public override bool CheckSettingsValidity(
      IVssRequestContext requestContext,
      Guid projectId,
      TSettings settings,
      out string errorMessage)
    {
      ArgumentUtility.CheckGenericForNull((object) settings, nameof (settings));
      if (!base.CheckSettingsValidity(requestContext, projectId, settings, out errorMessage))
        return false;
      errorMessage = (string) null;
      return true;
    }

    protected override bool IsApplicableTo(
      IVssRequestContext requestContext,
      GitRepositoryTarget target)
    {
      return this.Settings.Scope == null || target.IsInScope(this.Settings.Scope);
    }

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

    public override string[] GetScopes(TSettings settings) => settings.Scope.FlattenScope();
  }
}
