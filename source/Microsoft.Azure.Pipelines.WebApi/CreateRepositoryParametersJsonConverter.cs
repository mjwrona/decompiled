// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApi.CreateRepositoryParametersJsonConverter
// Assembly: Microsoft.Azure.Pipelines.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9955A178-37CB-46CB-B455-32EA2A66C5BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.WebApi
{
  public class CreateRepositoryParametersJsonConverter : 
    RepositoryTypeJsonConverter<CreateRepositoryParameters>
  {
    protected override CreateRepositoryParameters Create(Type objectType)
    {
      if (objectType == typeof (CreateAzureReposGitRepositoryParameters))
        return (CreateRepositoryParameters) new CreateAzureReposGitRepositoryParameters();
      return objectType == typeof (CreateGitHubRepositoryParameters) ? (CreateRepositoryParameters) new CreateGitHubRepositoryParameters() : (CreateRepositoryParameters) null;
    }

    protected override CreateRepositoryParameters Create(RepositoryType type)
    {
      if (type == RepositoryType.GitHub)
        return (CreateRepositoryParameters) new CreateGitHubRepositoryParameters();
      return type == RepositoryType.AzureReposGit ? (CreateRepositoryParameters) new CreateAzureReposGitRepositoryParameters() : (CreateRepositoryParameters) new UnknownCreateRepositoryParameters(type);
    }
  }
}
