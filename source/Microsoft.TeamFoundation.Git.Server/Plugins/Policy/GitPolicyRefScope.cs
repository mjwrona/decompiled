// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitPolicyRefScope
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  [JsonConverter(typeof (GitRefScopeSerializer))]
  [DataContract]
  public sealed class GitPolicyRefScope : GitScope
  {
    internal bool CheckValidity(
      IVssRequestContext requestContext,
      Guid projectId,
      out string errorMessage)
    {
      if (this.ScopeItems != null)
      {
        foreach (GitPolicyRepositoryScopeItem scopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
        {
          ITeamFoundationGitRepositoryService service = requestContext.GetService<ITeamFoundationGitRepositoryService>();
          IVssRequestContext requestContext1 = requestContext;
          ITeamFoundationGitRepositoryService repositoryService = service;
          Guid projectId1 = projectId;
          ref string local = ref errorMessage;
          if (!scopeItem.IsRepositoryValidWithinProject(requestContext1, repositoryService, projectId1, out local))
            return false;
        }
      }
      errorMessage = (string) null;
      return true;
    }

    public bool IsInScope(Guid targetRepository, string targetRefName, bool targetIsDefaultBranch)
    {
      ArgumentUtility.CheckForNull<string>(targetRefName, "refName");
      return this.ScopeItems.Any<GitPolicyRepositoryScopeItem>((Func<GitPolicyRepositoryScopeItem, bool>) (s => s.IsInScope(targetRepository, targetRefName, targetIsDefaultBranch)));
    }

    internal GitPolicyRefScope(
      IReadOnlyList<GitPolicyRepositoryScopeItem> scopeItems)
      : base(scopeItems)
    {
    }

    internal void CheckEditPoliciesPermission(IVssRequestContext requestContext, Guid projectId)
    {
      foreach (GitPolicyRepositoryScopeItem scopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
        scopeItem.CheckEditPoliciesPermission(requestContext, projectId);
    }

    internal void CheckSupportedMatchKind(IVssRequestContext requestContext)
    {
      foreach (GitPolicyRepositoryScopeItem scopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
      {
        if (scopeItem is GitPolicyRefScopeItem && ((GitPolicyRefScopeItem) scopeItem).MatchKind == RefNameMatchType.PrefixDeprecated)
          throw new CannotCreateNewDeprecatedScopeException();
        if (scopeItem is GitPolicyRefScopeItem && ((GitPolicyRefScopeItem) scopeItem).MatchKind == RefNameMatchType.DefaultBranch && !requestContext.IsFeatureEnabled("SourceControl.Policy.DefaultBranchScope"))
          throw new CannotCreateUnknownScopeException();
      }
    }

    public override string[] FlattenScope()
    {
      List<string> stringList = new List<string>();
      if (this.ScopeItems != null)
      {
        foreach (GitPolicyRepositoryScopeItem scopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
          stringList.AddRange((IEnumerable<string>) scopeItem.FlattenScope());
      }
      return stringList.ToArray();
    }
  }
}
