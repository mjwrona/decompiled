// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Converters.CreateRepositoryParametersExtensions
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Azure.Pipelines.WebServer.Converters
{
  public static class CreateRepositoryParametersExtensions
  {
    public static Microsoft.Azure.Pipelines.Server.ObjectModel.CreateRepositoryParameters ToCreateRepositoryParameters(
      this Microsoft.Azure.Pipelines.WebApi.CreateRepositoryParameters parameters)
    {
      switch (parameters)
      {
        case null:
          return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreateRepositoryParameters) null;
        case Microsoft.Azure.Pipelines.WebApi.CreateAzureReposGitRepositoryParameters _:
          return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreateRepositoryParameters) ((Microsoft.Azure.Pipelines.WebApi.CreateAzureReposGitRepositoryParameters) parameters).ToCreateAzureReposGitRepositoryParameters();
        case Microsoft.Azure.Pipelines.WebApi.CreateGitHubRepositoryParameters _:
          return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreateRepositoryParameters) ((Microsoft.Azure.Pipelines.WebApi.CreateGitHubRepositoryParameters) parameters).ToCreateGitHubRepositoryParameters();
        default:
          throw new UnsupportedRepositoryTypeException(WebServerResources.CreatePipeline_UnsupportedRepositoryType((object) parameters.Type));
      }
    }

    public static Microsoft.Azure.Pipelines.Server.ObjectModel.CreateAzureReposGitRepositoryParameters ToCreateAzureReposGitRepositoryParameters(
      this Microsoft.Azure.Pipelines.WebApi.CreateAzureReposGitRepositoryParameters parameters)
    {
      if (parameters == null)
        return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreateAzureReposGitRepositoryParameters) null;
      return new Microsoft.Azure.Pipelines.Server.ObjectModel.CreateAzureReposGitRepositoryParameters()
      {
        RepositoryId = parameters.Id,
        RepositoryName = parameters.Name
      };
    }

    public static Microsoft.Azure.Pipelines.Server.ObjectModel.CreateGitHubRepositoryParameters ToCreateGitHubRepositoryParameters(
      this Microsoft.Azure.Pipelines.WebApi.CreateGitHubRepositoryParameters parameters)
    {
      if (parameters == null)
        return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreateGitHubRepositoryParameters) null;
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.WebApi.CreateServiceConnectionParameters>(parameters.Connection, "Connection", "pipelines");
      ArgumentUtility.CheckForEmptyGuid(parameters.Connection.Id, "Id", "pipelines");
      ArgumentUtility.CheckStringForNullOrEmpty(parameters.FullName, "FullName", "pipelines");
      return new Microsoft.Azure.Pipelines.Server.ObjectModel.CreateGitHubRepositoryParameters()
      {
        Connection = new Microsoft.Azure.Pipelines.Server.ObjectModel.CreateServiceConnectionParameters()
        {
          ServiceConnectionId = parameters.Connection.Id
        },
        FullName = parameters.FullName
      };
    }
  }
}
