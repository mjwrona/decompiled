// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.RepositoryConverters
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public static class RepositoryConverters
  {
    public static Microsoft.Azure.Pipelines.WebApi.Repository ToWebApiRepository(
      this Microsoft.Azure.Pipelines.Server.ObjectModel.Repository repository,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      switch (repository)
      {
        case null:
          return (Microsoft.Azure.Pipelines.WebApi.Repository) null;
        case Microsoft.Azure.Pipelines.Server.ObjectModel.AzureReposGitRepository reposGitRepository:
          return (Microsoft.Azure.Pipelines.WebApi.Repository) new Microsoft.Azure.Pipelines.WebApi.AzureReposGitRepository(securedObject)
          {
            Id = reposGitRepository.RepositoryId
          };
        case Microsoft.Azure.Pipelines.Server.ObjectModel.GitHubRepository gitHubRepository:
          return (Microsoft.Azure.Pipelines.WebApi.Repository) new Microsoft.Azure.Pipelines.WebApi.GitHubRepository(securedObject)
          {
            Connection = new ServiceConnection(securedObject)
            {
              Id = gitHubRepository.ConnectionId
            },
            FullName = gitHubRepository.FullName
          };
        case Microsoft.Azure.Pipelines.Server.ObjectModel.GitHubEnterpriseRepository enterpriseRepository:
          return (Microsoft.Azure.Pipelines.WebApi.Repository) new Microsoft.Azure.Pipelines.WebApi.GitHubEnterpriseRepository(securedObject)
          {
            Connection = new ServiceConnection(securedObject)
            {
              Id = enterpriseRepository.ConnectionId
            },
            FullName = enterpriseRepository.FullName
          };
        default:
          return (Microsoft.Azure.Pipelines.WebApi.Repository) null;
      }
    }
  }
}
