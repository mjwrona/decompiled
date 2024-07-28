// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.Contracts.RepositoryJsonConverter
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.WebApi.Contracts
{
  internal class RepositoryJsonConverter : RepositoryTypeJsonConverter<Repository>
  {
    protected override Repository Create(Type objectType)
    {
      if (objectType == typeof (AzureReposGitRepository))
        return (Repository) new AzureReposGitRepository();
      return objectType == typeof (GitHubRepository) ? (Repository) new GitHubRepository() : (Repository) null;
    }

    protected override Repository Create(RepositoryType type)
    {
      if (type == RepositoryType.GitHub)
        return (Repository) new GitHubRepository();
      return type == RepositoryType.AzureReposGit ? (Repository) new AzureReposGitRepository() : (Repository) null;
    }
  }
}
