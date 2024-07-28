// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitSecuredObjectFactory
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitSecuredObjectFactory
  {
    public static ISecuredObject CreateRepositoryReadOnly(RepoKey repoKey)
    {
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      return GitSecuredObjectFactory.CreateReadOnlySecuredObject((RepoScope) repoKey);
    }

    public static ISecuredObject CreateRepositoryReadOnly(Guid projectId, Guid repoId) => GitSecuredObjectFactory.CreateRepositoryReadOnly(new RepoKey(projectId, repoId));

    public static ISecuredObject CreateProjectReadOnly(Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return GitSecuredObjectFactory.CreateReadOnlySecuredObject(new RepoScope(projectId, Guid.Empty));
    }

    private static ISecuredObject CreateReadOnlySecuredObject(RepoScope scope)
    {
      string securable = GitUtils.CalculateSecurable(scope.ProjectId, scope.RepoId, (string) null);
      return (ISecuredObject) new GitSecuredObjectFactory.GitSecuredObject(GitConstants.GitSecurityNamespaceId, 2, securable);
    }

    private class GitSecuredObject : ISecuredObject
    {
      private readonly Guid m_namespaceId;
      private readonly string m_token;
      private readonly int m_requiredPermissions;

      public GitSecuredObject(Guid namespaceId, int requiredPermissions, string token)
      {
        this.m_namespaceId = namespaceId;
        this.m_requiredPermissions = requiredPermissions;
        this.m_token = token;
      }

      string ISecuredObject.GetToken() => this.m_token;

      Guid ISecuredObject.NamespaceId => this.m_namespaceId;

      int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;
    }
  }
}
