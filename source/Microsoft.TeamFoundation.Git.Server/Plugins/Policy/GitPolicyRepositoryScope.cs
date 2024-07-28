// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitPolicyRepositoryScope
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  [JsonConverter(typeof (GitRepositoryScopeSerializer))]
  [DataContract]
  public sealed class GitPolicyRepositoryScope : GitScope
  {
    internal GitPolicyRepositoryScope(
      IReadOnlyList<GitPolicyRepositoryScopeItem> scopeItems)
      : base(scopeItems)
    {
    }

    internal void CheckEditPoliciesPermission(IVssRequestContext requestContext, Guid projectId)
    {
      foreach (GitPolicyRepositoryScopeItem scopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
        scopeItem.CheckEditPoliciesPermission(requestContext, projectId);
    }

    internal void CheckManageAdvSecPermission(IVssRequestContext requestContext, Guid projectId)
    {
      foreach (GitPolicyRepositoryScopeItem scopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
        scopeItem.CheckManageAdvSecPermission(requestContext, projectId);
    }

    internal GitPolicyRepositoryScope(Guid? repoId = null)
      : this((IReadOnlyList<GitPolicyRepositoryScopeItem>) new GitPolicyRepositoryScopeItem[1]
      {
        new GitPolicyRepositoryScopeItem(repoId)
      })
    {
    }

    public bool IsInScope(Guid repository) => this.ScopeItems.Any<GitPolicyRepositoryScopeItem>((Func<GitPolicyRepositoryScopeItem, bool>) (s => s.IsInScope(repository)));

    public override string[] FlattenScope()
    {
      List<string> stringList = new List<string>();
      if (this.ScopeItems != null)
      {
        foreach (GitPolicyRepositoryScopeItem scopeItem in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
          stringList.AddRange((IEnumerable<string>) scopeItem.FlattenScope());
      }
      else
        stringList.Add(GitPolicyScopeResolver.PolicySettingsToScope());
      return stringList.ToArray();
    }
  }
}
