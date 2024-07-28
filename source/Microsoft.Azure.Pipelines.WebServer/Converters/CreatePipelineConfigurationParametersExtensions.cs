// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Converters.CreatePipelineConfigurationParametersExtensions
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.WebServer.Converters
{
  public static class CreatePipelineConfigurationParametersExtensions
  {
    public static Microsoft.Azure.Pipelines.Server.ObjectModel.CreatePipelineConfigurationParameters ToCreatePipelineConfigurationParameters(
      this Microsoft.Azure.Pipelines.WebApi.CreatePipelineConfigurationParameters parameters)
    {
      if (parameters == null)
        return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreatePipelineConfigurationParameters) null;
      return parameters is Microsoft.Azure.Pipelines.WebApi.CreateYamlPipelineConfigurationParameters ? (Microsoft.Azure.Pipelines.Server.ObjectModel.CreatePipelineConfigurationParameters) ((Microsoft.Azure.Pipelines.WebApi.CreateYamlPipelineConfigurationParameters) parameters).ToCreateYamlConfigurationParameters() : throw new UnsupportedConfigurationTypeException(WebServerResources.CreatePipeline_UnsupportedConfigurationType((object) parameters.Type));
    }

    public static Microsoft.Azure.Pipelines.Server.ObjectModel.CreateYamlPipelineConfigurationParameters ToCreateYamlConfigurationParameters(
      this Microsoft.Azure.Pipelines.WebApi.CreateYamlPipelineConfigurationParameters parameters)
    {
      if (parameters == null)
        return (Microsoft.Azure.Pipelines.Server.ObjectModel.CreateYamlPipelineConfigurationParameters) null;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(parameters.Path, "Path");
      ArgumentUtility.CheckForNull<Microsoft.Azure.Pipelines.WebApi.CreateRepositoryParameters>(parameters.Repository, "Repository");
      Microsoft.Azure.Pipelines.Server.ObjectModel.CreateYamlPipelineConfigurationParameters configurationParameters = new Microsoft.Azure.Pipelines.Server.ObjectModel.CreateYamlPipelineConfigurationParameters()
      {
        Path = parameters.Path,
        Repository = parameters.Repository.ToCreateRepositoryParameters()
      };
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.Variable> variable in parameters.Variables)
        configurationParameters.Variables.Add(variable.Key, new Microsoft.Azure.Pipelines.Server.ObjectModel.Variable()
        {
          IsSecret = variable.Value.IsSecret,
          Value = variable.Value.Value
        });
      return configurationParameters;
    }
  }
}
