// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.ArtifactTriggerHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  public static class ArtifactTriggerHelper
  {
    public static IList<PipelineDefinitionTrigger> GetMatchingTriggers(
      IList<PipelineDefinitionTrigger> triggers,
      IArtifactType artifactType)
    {
      IList<PipelineDefinitionTrigger> matchingTriggers = (IList<PipelineDefinitionTrigger>) new List<PipelineDefinitionTrigger>();
      if (triggers != null)
      {
        foreach (PipelineDefinitionTrigger trigger in (IEnumerable<PipelineDefinitionTrigger>) triggers)
        {
          if (trigger.ArtifactDefinition.ArtifactType.Equals(artifactType.Name, StringComparison.OrdinalIgnoreCase))
            matchingTriggers.Add(trigger);
        }
      }
      return matchingTriggers;
    }

    public static bool IsTriggerFilterMatches(
      PipelineDefinitionTrigger definitionTrigger,
      WebHookEventPayloadInputMapper inputMapper)
    {
      switch (definitionTrigger.TriggerContent)
      {
        case PipelineResourceTrigger pipelineResourceTrigger:
          PipelineArtifactFilter pipelineArtifactFilter1 = new PipelineArtifactFilter(pipelineResourceTrigger.BranchFilters, pipelineResourceTrigger.StageFilters, pipelineResourceTrigger.TagFilters);
          string str1;
          inputMapper.GetValue("SourceBranch", out str1);
          string fromEventPaylaod = inputMapper.GetValueFromEventPaylaod("EventType");
          IList<string> tagsFromInputMapper = ArtifactTriggerHelper.ExtractTagsFromInputMapper(inputMapper);
          IList<Microsoft.Azure.Pipelines.WebApi.Stage> stagesFromInputMapper = ArtifactTriggerHelper.ExtractStagesFromInputMapper(inputMapper);
          string branch = str1;
          string eventType = fromEventPaylaod;
          IList<Microsoft.Azure.Pipelines.WebApi.Stage> stages = stagesFromInputMapper;
          IList<string> tags = tagsFromInputMapper;
          return pipelineArtifactFilter1.IsFilterMatches(branch, eventType, stages, tags);
        case ContainerResourceTrigger containerResourceTrigger:
          PipelineArtifactFilter pipelineArtifactFilter2 = new PipelineArtifactFilter(tags: containerResourceTrigger.TagFilters);
          string str2;
          inputMapper.GetValue("tag", out str2);
          string imageTag = str2;
          return pipelineArtifactFilter2.IsImageTagIncluded(imageTag);
        case PackageResourceTrigger _:
          string packageName;
          inputMapper.GetValue("name", out packageName);
          return ArtifactTriggerHelper.IsTriggerEnabledForPackageResources(definitionTrigger, packageName);
        case BuildResourceTrigger _:
          return true;
        case WebhookResourceTrigger webhookResourceTrigger:
          return new CustomArtifactFilter(webhookResourceTrigger.Filters).IsFilterMatches(inputMapper);
        default:
          return false;
      }
    }

    public static string ResolveArtifactSource(
      IVssRequestContext requestContext,
      IArtifactType artifactType,
      string projectName,
      string source,
      IDictionary<string, string> parameters = null)
    {
      if (artifactType is PipelineArtifact)
      {
        string str;
        if (parameters == null || !parameters.TryGetValue(PipelinePropertyNames.DefinitionId, out str))
          str = requestContext.GetService<IPipelineTfsBuildService>().GetDefinitionId(requestContext, projectName, source).ToString();
        return str;
      }
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (artifactType.YamlInputMapping ?? (IDictionary<string, string>) new Dictionary<string, string>()))
      {
        if (parameters != null && parameters.ContainsKey(keyValuePair.Key) && string.Equals(keyValuePair.Value, "definition", StringComparison.OrdinalIgnoreCase))
          return parameters[keyValuePair.Key];
      }
      return source;
    }

    private static bool IsTriggerEnabledForPackageResources(
      PipelineDefinitionTrigger definitionTrigger,
      string packageName)
    {
      ArtifactDefinitionReference artifactDefinition = definitionTrigger.ArtifactDefinition;
      return artifactDefinition != null && !string.IsNullOrEmpty(packageName) && string.Equals(packageName, artifactDefinition.Properties["name"]);
    }

    private static IList<string> ExtractTagsFromInputMapper(
      WebHookEventPayloadInputMapper inputMapper)
    {
      IList<string> source = (IList<string>) null;
      string fromEventPaylaod = inputMapper.GetValueFromEventPaylaod("Tags");
      if (!fromEventPaylaod.IsNullOrEmpty<char>())
        source = (IList<string>) JsonUtilities.Deserialize<List<string>>(fromEventPaylaod);
      return source != null ? (IList<string>) source.Select<string, string>((Func<string, string>) (t => t.Trim())).ToList<string>() : (IList<string>) null;
    }

    private static IList<Microsoft.Azure.Pipelines.WebApi.Stage> ExtractStagesFromInputMapper(
      WebHookEventPayloadInputMapper inputMapper)
    {
      IList<Microsoft.Azure.Pipelines.WebApi.Stage> stagesFromInputMapper = (IList<Microsoft.Azure.Pipelines.WebApi.Stage>) null;
      string fromEventPaylaod = inputMapper.GetValueFromEventPaylaod("Stages");
      if (!fromEventPaylaod.IsNullOrEmpty<char>())
        stagesFromInputMapper = (IList<Microsoft.Azure.Pipelines.WebApi.Stage>) JsonUtilities.Deserialize<List<Microsoft.Azure.Pipelines.WebApi.Stage>>(fromEventPaylaod);
      return stagesFromInputMapper;
    }
  }
}
