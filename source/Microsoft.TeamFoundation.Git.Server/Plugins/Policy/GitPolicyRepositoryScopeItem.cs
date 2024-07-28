// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Plugins.Policy.GitPolicyRepositoryScopeItem
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Plugins.Policy
{
  [DataContract]
  internal class GitPolicyRepositoryScopeItem
  {
    [JsonConstructor]
    internal GitPolicyRepositoryScopeItem(Guid? repositoryId = null) => this.RepositoryId = repositoryId;

    [JsonProperty(Required = Required.AllowNull)]
    internal Guid? RepositoryId { get; private set; }

    internal bool IsRepositoryValidWithinProject(
      IVssRequestContext requestContext,
      ITeamFoundationGitRepositoryService repositoryService,
      Guid projectId,
      out string errorMessage)
    {
      errorMessage = (string) null;
      if (!this.RepositoryId.HasValue)
        return true;
      try
      {
        using (ITfsGitRepository repositoryById = repositoryService.FindRepositoryById(requestContext, this.RepositoryId.Value))
        {
          if (repositoryById.Key.ProjectId != projectId)
          {
            errorMessage = Resources.Format("PolicyScopeRepositoryMustBeInProject");
            return false;
          }
        }
      }
      catch (GitRepositoryNotFoundException ex)
      {
        errorMessage = Resources.Format("PolicyScopeExistingRepositoriesOnly");
        return false;
      }
      catch (GitNeedsPermissionException ex)
      {
        errorMessage = Resources.Format("PolicyScopeAccessibleRepositoriesOnly");
        return false;
      }
      return true;
    }

    internal bool IsInScope(Guid targetRepository) => !this.RepositoryId.HasValue || this.RepositoryId.Value == targetRepository;

    internal virtual bool IsInScope(
      Guid targetRepository,
      string targetRefName,
      bool targetIsDefaultBranch)
    {
      return this.IsInScope(targetRepository);
    }

    internal virtual void CheckEditPoliciesPermission(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      SecurityHelper.Instance.CheckEditPoliciesPermission(requestContext, new RepoScope(projectId, this.RepositoryId ?? Guid.Empty));
    }

    internal void CheckManageAdvSecPermission(IVssRequestContext requestContext, Guid projectId) => SecurityHelper.Instance.CheckManageAdvSecPermission(requestContext, new RepoScope(projectId, this.RepositoryId ?? Guid.Empty));

    public virtual string[] FlattenScope() => new string[1]
    {
      GitPolicyScopeResolver.PolicySettingsToScope(this.RepositoryId)
    };

    public override bool Equals(object obj)
    {
      if (!(obj is GitPolicyRepositoryScopeItem repositoryScopeItem))
        return false;
      Guid? repositoryId1 = this.RepositoryId;
      Guid? repositoryId2 = repositoryScopeItem.RepositoryId;
      if (repositoryId1.HasValue != repositoryId2.HasValue)
        return false;
      return !repositoryId1.HasValue || repositoryId1.GetValueOrDefault() == repositoryId2.GetValueOrDefault();
    }

    public override int GetHashCode() => (this.RepositoryId.HasValue ? this.RepositoryId.Value : Guid.Empty).GetHashCode();
  }
}
