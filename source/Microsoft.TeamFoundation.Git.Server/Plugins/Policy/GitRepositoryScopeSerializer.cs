// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitRepositoryScopeSerializer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  internal sealed class GitRepositoryScopeSerializer : GitPolicyScopeSerializer
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (GitPolicyRepositoryScope);

    protected override bool CanTakeRefNameScopes => false;

    protected override object CreateParsedObject(
      IReadOnlyList<GitPolicyRepositoryScopeItem> scopeItems)
    {
      return (object) new GitPolicyRepositoryScope(scopeItems);
    }

    protected override IReadOnlyList<GitPolicyRepositoryScopeItem> GetScopeItems(object value) => ((GitScope) value).ScopeItems;
  }
}
