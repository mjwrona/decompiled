// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitPolicyRefScopeItem
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  internal class GitPolicyRefScopeItem : GitPolicyRepositoryScopeItem
  {
    [JsonProperty(Required = Required.AllowNull)]
    internal string RefName { get; set; }

    [JsonProperty(Required = Required.Always)]
    [JsonConverter(typeof (StringEnumConverter))]
    internal RefNameMatchType MatchKind { get; set; }

    internal override bool IsInScope(Guid repository, string refName, bool isDefaultBranch)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(refName, nameof (refName));
      return this.IsInScope(repository) && this.RefNameMatches(refName, isDefaultBranch);
    }

    internal override void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      Guid repoId = this.RepositoryId ?? Guid.Empty;
      if (repoId == Guid.Empty)
      {
        base.CheckEditPoliciesPermission(requestContext, projectId);
      }
      else
      {
        string refName = this.RefName;
        if (refName == "refs/heads" && (this.MatchKind == RefNameMatchType.Prefix || this.MatchKind == RefNameMatchType.PrefixDeprecated))
          refName = (string) null;
        SecurityHelper.Instance.CheckEditPoliciesPermission(requestContext, new RepoScope(projectId, repoId), refName);
      }
    }

    internal bool IsRefNameNull() => this.RefName == null;

    internal bool IsDefaultBranchScope() => this.MatchKind == RefNameMatchType.DefaultBranch;

    private bool RefNameMatches(string refName, bool isDefaultBranch)
    {
      bool flag;
      switch (this.MatchKind)
      {
        case RefNameMatchType.Prefix:
        case RefNameMatchType.PrefixDeprecated:
          flag = refName.StartsWith(this.RefName, StringComparison.Ordinal);
          break;
        case RefNameMatchType.DefaultBranch:
          flag = isDefaultBranch;
          break;
        default:
          flag = this.RefName.Equals(refName, StringComparison.Ordinal);
          break;
      }
      return flag;
    }

    public override string[] FlattenScope() => new string[1]
    {
      GitPolicyScopeResolver.PolicySettingsToScope(this.RepositoryId, this.RefName, new RefNameMatchType?(this.MatchKind))
    };

    public override bool Equals(object obj)
    {
      if (!(obj is GitPolicyRefScopeItem policyRefScopeItem))
        return false;
      Guid? repositoryId1 = this.RepositoryId;
      Guid? repositoryId2 = policyRefScopeItem.RepositoryId;
      return (repositoryId1.HasValue == repositoryId2.HasValue ? (repositoryId1.HasValue ? (repositoryId1.GetValueOrDefault() == repositoryId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0 && this.RefName == policyRefScopeItem.RefName && this.MatchKind == policyRefScopeItem.MatchKind;
    }

    public override int GetHashCode()
    {
      string str = string.IsNullOrEmpty(this.RefName) ? string.Empty : this.RefName;
      return HashCodeUtil.GetHashCode<int, int, int>(base.GetHashCode(), str.GetHashCode(), this.MatchKind.GetHashCode());
    }

    public GitPolicyRefScopeItem()
      : base()
    {
    }
  }
}
