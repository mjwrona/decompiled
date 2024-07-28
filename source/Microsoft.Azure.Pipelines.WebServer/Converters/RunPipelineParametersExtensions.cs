// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Converters.RunPipelineParametersExtensions
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.WebServer.Converters
{
  public static class RunPipelineParametersExtensions
  {
    public static Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters ToRunPipelineParameters(
      this Microsoft.Azure.Pipelines.WebApi.RunPipelineParameters parameters)
    {
      if (parameters == null)
        return (Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters) null;
      Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters pipelineParameters = new Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters();
      pipelineParameters.Configuration = (JustInTimeConfiguration) null;
      pipelineParameters.Context = new JustInTimeContext();
      Dictionary<string, Microsoft.Azure.Pipelines.WebApi.Variable> variables = parameters.Variables;
      pipelineParameters.Variables = variables != null ? variables.ToDictionary<KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.Variable>, string, string>((Func<KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.Variable>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.Variable>, string>) (kvp => kvp.Value?.Value)) : (Dictionary<string, string>) null;
      pipelineParameters.YamlOverride = parameters.YamlOverride;
      pipelineParameters.PreviewRun = parameters.PreviewRun;
      pipelineParameters.TemplateParameters = new Dictionary<string, string>((IDictionary<string, string>) parameters.TemplateParameters);
      Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters result = pipelineParameters;
      parameters.UpdateContextAndSecrets(result.Context, result.Secrets);
      parameters.UpdateResources(result);
      result.StagesToSkip.UnionWith((IEnumerable<string>) parameters.StagesToSkip);
      return result;
    }

    private static void UpdateResources(
      this Microsoft.Azure.Pipelines.WebApi.RunPipelineParameters parameters,
      Microsoft.Azure.Pipelines.Server.ObjectModel.RunPipelineParameters result)
    {
      if (parameters.Resources == null)
        return;
      result.Resources = new Microsoft.Azure.Pipelines.Server.ObjectModel.RunResourcesParameters();
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.PipelineResourceParameters> pipeline in parameters.Resources.Pipelines)
        result.Resources.Pipelines[pipeline.Key] = new Microsoft.Azure.Pipelines.Server.ObjectModel.PipelineResourceParameters(pipeline.Value.Version);
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.RepositoryResourceParameters> repository in parameters.Resources.Repositories)
        result.Resources.Repositories[repository.Key] = new Microsoft.Azure.Pipelines.Server.ObjectModel.RepositoryResourceParameters(repository.Value.RefName, repository.Value.Version);
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.BuildResourceParameters> build in parameters.Resources.Builds)
        result.Resources.Builds[build.Key] = new Microsoft.Azure.Pipelines.Server.ObjectModel.BuildResourceParameters(build.Value.Version);
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.ContainerResourceParameters> container in parameters.Resources.Containers)
        result.Resources.Containers[container.Key] = new Microsoft.Azure.Pipelines.Server.ObjectModel.ContainerResourceParameters(container.Value.Version);
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.PackageResourceParameters> package in parameters.Resources.Packages)
        result.Resources.Packages[package.Key] = new Microsoft.Azure.Pipelines.Server.ObjectModel.PackageResourceParameters(package.Value.Version);
    }

    private static void UpdateContextAndSecrets(
      this Microsoft.Azure.Pipelines.WebApi.RunPipelineParameters parameters,
      JustInTimeContext context,
      Dictionary<string, string> secrets)
    {
      if (context == null)
        return;
      Microsoft.Azure.Pipelines.WebApi.RunResourcesParameters resources = parameters.Resources;
      int num;
      if (resources == null)
      {
        num = 0;
      }
      else
      {
        bool? nullable = resources.Repositories?.ContainsKey("self");
        bool flag = true;
        num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
      }
      if (num != 0)
      {
        Microsoft.Azure.Pipelines.WebApi.RepositoryResourceParameters repository = parameters.Resources?.Repositories["self"];
        context.Ref = repository.RefName;
        context.Sha = repository.Version;
        context.Token = repository.Token;
        context.TokenType = repository.TokenType;
      }
      if (parameters.Variables == null)
        return;
      foreach (KeyValuePair<string, Microsoft.Azure.Pipelines.WebApi.Variable> variable in parameters.Variables)
      {
        if (variable.Value.IsSecret)
          secrets[variable.Key] = variable.Value.Value;
        else if (variable.Value.Value != null)
          context.ExtendedContext[variable.Key] = JToken.FromObject((object) variable.Value.Value);
      }
    }
  }
}
