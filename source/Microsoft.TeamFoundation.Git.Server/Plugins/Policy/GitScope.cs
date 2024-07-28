// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitScope
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  public abstract class GitScope
  {
    public const string DefaultBranchScope = "~default";

    internal GitScope(
      IReadOnlyList<GitPolicyRepositoryScopeItem> scopeItems)
    {
      this.ScopeItems = scopeItems;
    }

    internal bool ContainsAnyScope(GitScope scope)
    {
      foreach (GitPolicyRepositoryScopeItem scopeItem1 in (IEnumerable<GitPolicyRepositoryScopeItem>) this.ScopeItems)
      {
        foreach (GitPolicyRepositoryScopeItem scopeItem2 in (IEnumerable<GitPolicyRepositoryScopeItem>) scope.ScopeItems)
        {
          if (scopeItem1.Equals((object) scopeItem2))
            return true;
        }
      }
      return false;
    }

    internal IReadOnlyList<GitPolicyRepositoryScopeItem> ScopeItems { get; private set; }

    public abstract string[] FlattenScope();
  }
}
