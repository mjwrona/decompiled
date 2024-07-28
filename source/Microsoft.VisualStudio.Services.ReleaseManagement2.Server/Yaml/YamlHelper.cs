// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml.YamlHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Yaml
{
  internal class YamlHelper
  {
    private const string ProjectId = "ProjectId";
    private const string RepositoryId = "RepositoryId";

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Logging")]
    public static YamlLoadResult Load(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId,
      YamlFileSource fileSource,
      string yamlFileName,
      YamlPipelineProcessResource authorizedResources,
      bool authorizeNewResources)
    {
      YamlLoadResult yamlLoadResult = new YamlLoadResult();
      RepositoryResource repositoryResource = YamlHelper.ToRepositoryResource(fileSource);
      try
      {
        IYamlPipelineLoaderService service = requestContext.GetService<IYamlPipelineLoaderService>();
        PipelineBuilder builder1 = requestContext.GetService<IPipelineBuilderService>().GetBuilder(requestContext, projectId, "Release", definitionId, authorizedResources != null ? authorizedResources.ToPipelineResources() : (PipelineResources) null, authorizeNewResources);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        RepositoryResource repository = repositoryResource;
        string filePath = yamlFileName;
        PipelineBuilder builder2 = builder1;
        int? defaultQueueId = new int?();
        YamlPipelineLoadResult pipelineLoadResult = service.Load(requestContext1, projectId1, repository, filePath, builder2, defaultQueueId);
        yamlLoadResult.Errors.AddRange<string, IList<string>>(pipelineLoadResult.Template.Errors.Select<PipelineValidationError, string>((Func<PipelineValidationError, string>) (x => x.Message)));
        yamlLoadResult.PipelineTemplate = pipelineLoadResult.Template;
        yamlLoadResult.PipelineResources = pipelineLoadResult.Environment.Resources;
      }
      catch (Exception ex)
      {
        Exception exception = ex;
        yamlLoadResult.Errors.Add(exception.Message);
        for (; exception.InnerException != null; exception = exception.InnerException)
          yamlLoadResult.Errors.Add(exception.InnerException.Message);
      }
      return yamlLoadResult;
    }

    public static void UpdateDefinitionFromYaml(
      IVssRequestContext requestContext,
      PipelineTemplate yamlPipeline,
      ReleaseDefinition existingDefinition,
      ReleaseDefinition targetDefinition)
    {
      if (yamlPipeline == null)
        throw new ArgumentNullException(nameof (yamlPipeline));
      if (targetDefinition == null)
        throw new ArgumentNullException(nameof (targetDefinition));
      Dictionary<string, DefinitionEnvironment> stagesLookup = new Dictionary<string, DefinitionEnvironment>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (existingDefinition != null)
        existingDefinition.Environments.ForEach<DefinitionEnvironment>((Action<DefinitionEnvironment>) (x => stagesLookup[x.Name] = x));
      targetDefinition.Environments.Clear();
      foreach (Stage stage in (IEnumerable<Stage>) yamlPipeline.Stages)
      {
        DefinitionEnvironment definitionEnvironment1 = (DefinitionEnvironment) null;
        if (stagesLookup.TryGetValue(stage.Name, out definitionEnvironment1))
        {
          targetDefinition.Environments.Add(definitionEnvironment1);
        }
        else
        {
          DefinitionEnvironment definitionEnvironment2 = new DefinitionEnvironment()
          {
            Name = stage.Name,
            OwnerId = requestContext.GetUserId(),
            RetentionPolicy = new EnvironmentRetentionPolicy()
          };
          targetDefinition.Environments.Add(definitionEnvironment2);
        }
      }
    }

    private static RepositoryResource ToRepositoryResource(YamlFileSource fileSource)
    {
      if (fileSource.Type != YamlFileSourceTypes.TFSGit)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidFileSourceProviderinYamlPipelineProcess));
      return new RepositoryResource()
      {
        Id = YamlHelper.GetValue(fileSource.SourceData, "RepositoryId"),
        Type = RepositoryTypes.Git
      };
    }

    private static string GetValue(IDictionary<string, InputValue> sourceData, string key)
    {
      InputValue inputValue;
      if (!sourceData.TryGetValue(key, out inputValue))
        throw new KeyNotFoundException(key);
      return inputValue.Value;
    }
  }
}
