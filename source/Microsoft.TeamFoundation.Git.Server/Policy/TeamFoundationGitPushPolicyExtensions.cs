// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitPushPolicyExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  internal static class TeamFoundationGitPushPolicyExtensions
  {
    public static IReadOnlyList<ITeamFoundationGitPushPolicy> GetApplicablePushPolicies(
      this ITeamFoundationPolicyService service,
      IVssRequestContext requestContext,
      string teamProjectUri,
      Guid repositoryId,
      out List<PolicyFailures> failedToInitialize,
      bool isBlockingOnly = false)
    {
      IReadOnlyList<ITeamFoundationGitPushPolicy> source1 = !requestContext.IsFeatureEnabled("Policy.HiddenPolicies") ? (IReadOnlyList<ITeamFoundationGitPushPolicy>) service.GetApplicablePolicies<ITeamFoundationGitPushPolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitRepositoryTarget(teamProjectUri, repositoryId), out failedToInitialize, isBlockingOnly, true).ToList<ITeamFoundationGitPushPolicy>() : (IReadOnlyList<ITeamFoundationGitPushPolicy>) service.GetApplicablePolicies<ITeamFoundationGitPushPolicy>(requestContext, (ITeamFoundationPolicyTarget) new GitRepositoryTarget(teamProjectUri, repositoryId), out failedToInitialize, isBlockingOnly, true).Where<ITeamFoundationGitPushPolicy>((Func<ITeamFoundationGitPushPolicy, bool>) (x => !x.IsHidden || !x.Configuration.ExistsInDatabase)).ToList<ITeamFoundationGitPushPolicy>();
      List<ITeamFoundationGitPushPolicy> applicablePushPolicies = new List<ITeamFoundationGitPushPolicy>();
      foreach (ITeamFoundationGitPushPolicy foundationGitPushPolicy in source1.Where<ITeamFoundationGitPushPolicy>((Func<ITeamFoundationGitPushPolicy, bool>) (x => !x.AggregateScopeSettings)))
      {
        foundationGitPushPolicy.RepositoryId = repositoryId;
        applicablePushPolicies.Add(foundationGitPushPolicy);
      }
      foreach (IEnumerable<ITeamFoundationGitPushPolicy> source2 in source1.Where<ITeamFoundationGitPushPolicy>((Func<ITeamFoundationGitPushPolicy, bool>) (x => x.AggregateScopeSettings)).GroupBy<ITeamFoundationGitPushPolicy, Guid>((Func<ITeamFoundationGitPushPolicy, Guid>) (x => x.Configuration.TypeId)))
      {
        List<ITeamFoundationGitPushPolicy> list = source2.OrderBy<ITeamFoundationGitPushPolicy, int>((Func<ITeamFoundationGitPushPolicy, int>) (x => x.ScopeOrder)).ToList<ITeamFoundationGitPushPolicy>();
        ITeamFoundationGitPushPolicy foundationGitPushPolicy = list.First<ITeamFoundationGitPushPolicy>();
        foundationGitPushPolicy.RepositoryId = repositoryId;
        foundationGitPushPolicy.AllScopeSettings = (IList<ITeamFoundationGitRepositoryPolicySettings>) list.Select<ITeamFoundationGitPushPolicy, ITeamFoundationGitRepositoryPolicySettings>((Func<ITeamFoundationGitPushPolicy, ITeamFoundationGitRepositoryPolicySettings>) (x => (ITeamFoundationGitRepositoryPolicySettings) x.DeserializeSettings(x.Configuration.Settings))).ToList<ITeamFoundationGitRepositoryPolicySettings>();
        applicablePushPolicies.Add(foundationGitPushPolicy);
      }
      return (IReadOnlyList<ITeamFoundationGitPushPolicy>) applicablePushPolicies;
    }
  }
}
