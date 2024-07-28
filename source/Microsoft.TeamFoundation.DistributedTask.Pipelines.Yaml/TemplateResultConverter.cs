// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.TemplateResultConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using NCrontab;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal static class TemplateResultConverter
  {
    private const string c_latestTaskKey = "latesttask";
    private const string c_crossOrgErrorForRepoTriggers = "Triggers are not supported for cross-organization repository resources in Azure Repos";

    internal static PipelineTemplate ConvertToPipelineTemplate(
      TemplateContext context,
      PipelineResources resources,
      IList<TaskDefinition> tasks,
      IArtifactResolver artifactResolver,
      TemplateToken pipeline)
    {
      PipelineTemplate pipelineTemplate = new PipelineTemplate();
      try
      {
        if (pipeline == null || context.Errors.Count > 0)
          return pipelineTemplate;
        pipelineTemplate.Resources.MergeWith(resources);
        MappingToken mappingToken = TemplateUtil.AssertMapping(pipeline, nameof (pipeline));
        MappingToken parentPool = (MappingToken) null;
        ExclusiveLockType parentLockBehavior = ExclusiveLockType.RunLatest;
        TemplateToken stages = (TemplateToken) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          string str = (keyValuePair.Key as LiteralToken).Value;
          if (str != null)
          {
            switch (str.Length)
            {
              case 2:
                if (str == "pr")
                {
                  pipelineTemplate.Triggers.Add((PipelineTrigger) TemplateResultConverter.ConvertToPullRequestTrigger(context, keyValuePair.Value));
                  continue;
                }
                continue;
              case 4:
                switch (str[0])
                {
                  case 'n':
                    if (str == "name")
                    {
                      pipelineTemplate.Name = TemplateUtil.AssertLiteral(keyValuePair.Value, "pipeline name").Value;
                      continue;
                    }
                    continue;
                  case 'p':
                    if (str == "pool")
                    {
                      parentPool = TemplateUtil.AssertMapping(keyValuePair.Value, "pool");
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 6:
                if (str == "stages")
                {
                  stages = keyValuePair.Value;
                  continue;
                }
                continue;
              case 7:
                if (str == "trigger")
                {
                  pipelineTemplate.Triggers.Add((PipelineTrigger) TemplateResultConverter.ConvertToContinuousIntegrationTrigger(context, keyValuePair.Value));
                  continue;
                }
                continue;
              case 9:
                switch (str[0])
                {
                  case 'r':
                    if (str == nameof (resources))
                    {
                      if (context.AllowTemplateExpressionsInRef)
                        pipelineTemplate.Resources.MergeWith(TemplateResultConverter.ConvertToPipelineRepositoryResources(context, keyValuePair.Value, true));
                      pipelineTemplate.Resources.MergeWith(TemplateResultConverter.ConvertToNonRepositoryResources(context, artifactResolver, keyValuePair.Value));
                      continue;
                    }
                    continue;
                  case 's':
                    if (str == "schedules")
                    {
                      pipelineTemplate.Schedules.AddRange<PipelineSchedule, IList<PipelineSchedule>>(TemplateResultConverter.ConvertToSchedules(context, keyValuePair.Value));
                      continue;
                    }
                    continue;
                  case 'v':
                    if (str == "variables")
                    {
                      pipelineTemplate.Variables.AddRange<IVariable, IList<IVariable>>(TemplateResultConverter.ConvertToVariables(context, keyValuePair.Value));
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              case 10:
                if (str == "parameters")
                {
                  pipelineTemplate.Parameters.AddRange<TemplateParameter, IList<TemplateParameter>>(TemplateResultConverter.ConvertToTemplateParameters(context, keyValuePair.Value));
                  continue;
                }
                continue;
              case 12:
                if (str == "lockBehavior")
                {
                  parentLockBehavior = TemplateResultConverter.ConvertToExclusiveLockType(keyValuePair.Value);
                  continue;
                }
                continue;
              case 28:
                if (str == "appendCommitMessageToRunName")
                {
                  pipelineTemplate.AppendCommitMessageToRunName = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "append the commit message to the build number"));
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
        }
        if (stages != null)
          pipelineTemplate.Stages.AddRange<Stage, IList<Stage>>(TemplateResultConverter.ConvertToStages(context, tasks, pipelineTemplate.Resources, artifactResolver, stages, parentPool, parentLockBehavior));
      }
      catch (Exception ex)
      {
        context.Errors.Add(ex);
      }
      finally
      {
        if (context.Errors.Count > 0)
        {
          foreach (PipelineValidationError error in context.Errors)
            pipelineTemplate.Errors.Add(error);
        }
      }
      return pipelineTemplate;
    }

    internal static PipelineResources ConvertToNonRepositoryResources(
      TemplateContext context,
      IArtifactResolver artifactResolver,
      TemplateToken resources)
    {
      PipelineResources repositoryResources = new PipelineResources();
      if (resources is MappingToken mappingToken)
      {
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "resources key");
          switch (literal.Value)
          {
            case "builds":
              repositoryResources.Builds.UnionWith(TemplateResultConverter.ConvertToBuildResources(context, artifactResolver, keyValuePair.Value));
              continue;
            case "containers":
              repositoryResources.Containers.UnionWith(TemplateResultConverter.ConvertToContainerResources(context, keyValuePair.Value));
              continue;
            case "packages":
              repositoryResources.Packages.UnionWith(TemplateResultConverter.ConvertToPackageResources(context, artifactResolver, keyValuePair.Value));
              continue;
            case "pipelines":
              repositoryResources.Pipelines.UnionWith(TemplateResultConverter.ConvertToPipelineResources(context, artifactResolver, keyValuePair.Value));
              continue;
            case "repositories":
              continue;
            case "webhooks":
              repositoryResources.Webhooks.UnionWith(TemplateResultConverter.ConvertToWebhookResources(context, keyValuePair.Value, artifactResolver));
              continue;
            default:
              TemplateUtil.AssertUnexpectedValue(literal, "resources key");
              continue;
          }
        }
      }
      return repositoryResources;
    }

    internal static PipelineResources ConvertToWebhookResources(
      TemplateContext context,
      TemplateToken resources,
      out TemplateToken webhookResourceToken)
    {
      PipelineResources webhookResources = new PipelineResources();
      webhookResourceToken = (TemplateToken) null;
      if (resources is MappingToken mappingToken)
      {
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          if (string.Equals(TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "resources key").Value, "webhooks", StringComparison.Ordinal))
          {
            webhookResourceToken = keyValuePair.Value;
            webhookResources.Webhooks.UnionWith(TemplateResultConverter.ConvertToWebhookResources(context, keyValuePair.Value, (IArtifactResolver) null));
          }
        }
      }
      return webhookResources;
    }

    internal static PipelineResources ConvertToPipelineRepositoryResources(
      TemplateContext context,
      TemplateToken resources,
      bool acceptsExpression)
    {
      PipelineResources repositoryResources = new PipelineResources();
      if (resources is MappingToken mappingToken)
      {
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          if (string.Equals(TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "resources key").Value, "repositories", StringComparison.Ordinal))
          {
            if (context.AllowTemplateExpressionsInRef)
              repositoryResources.Repositories.UnionWith(TemplateResultConverter.ConvertToRepositoryResources(context, keyValuePair.Value, acceptsExpression));
            else
              repositoryResources.Repositories.UnionWith(TemplateResultConverter.ConvertToRepositoryResources(context, keyValuePair.Value));
          }
        }
      }
      else
      {
        foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(resources, nameof (resources)))
        {
          RepositoryResource repositoryResource = new RepositoryResource();
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(templateToken, "resources item"))
          {
            LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "repository key");
            if (string.Equals(literalToken1.Value, "repo", StringComparison.Ordinal))
            {
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository alias");
              repositoryResource.Alias = literalToken2.Value;
            }
            else
              repositoryResource.Properties.Set<JToken>(literalToken1.Value, TemplateUtil.ConvertToJToken(keyValuePair.Value));
          }
          TemplateResultConverter.ValidateRepositoryResourceTemplate(context, repositoryResource);
          repositoryResources.Repositories.Add(repositoryResource);
        }
      }
      return repositoryResources;
    }

    internal static PipelineStepsTemplate ConvertToStepsTemplate(
      TemplateContext context,
      TemplateToken stepsContribution)
    {
      PipelineStepsTemplate stepsTemplate = new PipelineStepsTemplate();
      try
      {
        if (stepsContribution == null || context.Errors.Count > 0)
          return stepsTemplate;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(stepsContribution, "steps contribution"))
        {
          LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "steps contribution key");
          if (literal.Value == "steps")
          {
            foreach (Step step in TemplateResultConverter.ConvertToSteps(context, (IList<TaskDefinition>) null, (PipelineResources) null, (IArtifactResolver) null, keyValuePair.Value))
              stepsTemplate.Steps.Add(step);
          }
          else
            TemplateUtil.AssertUnexpectedValue(literal, "steps contribution key");
        }
      }
      catch (Exception ex)
      {
        context.Errors.Add(ex);
      }
      finally
      {
        if (context.Errors.Count > 0)
        {
          foreach (PipelineValidationError error in context.Errors)
            stepsTemplate.Errors.Add(error);
        }
      }
      return stepsTemplate;
    }

    internal static IEnumerable<TemplateParameter> ConvertToTemplateParameters(
      TemplateContext context,
      TemplateToken parametersToken)
    {
      SequenceToken sequenceToken = TemplateUtil.AssertSequence(parametersToken, "parameters");
      HashSet<string> knownNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (TemplateToken parameterToken in sequenceToken)
      {
        TemplateParameter templateParameter = TemplateResultConverter.ConvertToTemplateParameter(context, parameterToken);
        if (knownNames.Contains(templateParameter.Name))
        {
          context.Error(parameterToken, YamlStrings.DuplicateTemplateParameter((object) templateParameter.Name));
        }
        else
        {
          knownNames.Add(templateParameter.Name);
          yield return TemplateResultConverter.ConvertToTemplateParameter(context, parameterToken);
        }
      }
    }

    internal static TemplateParameter ConvertToTemplateParameter(
      TemplateContext context,
      TemplateToken parameterToken)
    {
      TemplateToken defaultToken;
      TemplateParameter templateParameter = TemplateResultConverter.ConvertToTemplateParameter(context, parameterToken, out defaultToken);
      if (defaultToken != null)
        templateParameter.Default = TemplateResultConverter.ConvertTemplateParameterValue(context, templateParameter.Name, templateParameter.Type, defaultToken);
      return templateParameter;
    }

    internal static TemplateParameter ConvertToTemplateParameter(
      TemplateContext context,
      TemplateToken parameterToken,
      out TemplateToken defaultToken)
    {
      MappingToken mappingToken = TemplateUtil.AssertMapping(parameterToken, "parameter");
      TemplateParameter templateParameter = new TemplateParameter();
      defaultToken = (TemplateToken) null;
      TemplateToken templateToken = (TemplateToken) null;
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
      {
        switch (TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "parameterKey").Value)
        {
          case "default":
            defaultToken = keyValuePair.Value;
            continue;
          case "displayName":
            LiteralToken literalToken1 = TemplateUtil.AssertLiteral(keyValuePair.Value, "displayName");
            templateParameter.DisplayName = literalToken1.Value;
            continue;
          case "name":
            LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "name");
            templateParameter.Name = literalToken2.Value;
            continue;
          case "type":
            templateParameter.Type = TemplateResultConverter.ConvertTemplateParameterType(keyValuePair.Value);
            continue;
          case "values":
            templateToken = keyValuePair.Value;
            continue;
          default:
            continue;
        }
      }
      if (templateToken != null && (templateParameter.Type == TemplateParameterType.Number || templateParameter.Type == TemplateParameterType.String))
      {
        foreach (TemplateToken token in TemplateUtil.AssertSequence(templateToken, "values"))
          templateParameter.Values.Add(TemplateResultConverter.ConvertTemplateParameterValue(context, templateParameter.Name, templateParameter.Type, token));
      }
      return templateParameter;
    }

    internal static JToken ConvertTemplateParameterValue(
      TemplateContext context,
      string name,
      TemplateParameterType parameterType,
      TemplateToken token)
    {
      JToken result;
      if (TemplateResultConverter.TryConvertParameterValueToJToken(parameterType, token, out result))
        return result;
      context.Error(token, YamlStrings.InvalidTemplateParameter((object) name, (object) parameterType));
      return (JToken) null;
    }

    internal static bool TryConvertParameterValueToJToken(
      TemplateParameterType parameterType,
      TemplateToken token,
      out JToken result)
    {
      result = (JToken) null;
      switch (parameterType)
      {
        case TemplateParameterType.String:
          return TemplateUtil.TryConvertToStringJToken(token, out result);
        case TemplateParameterType.Number:
          return TemplateUtil.TryConvertToNumberJToken(token, out result);
        case TemplateParameterType.Boolean:
          return TemplateUtil.TryConvertToBooleanJToken(token, out result);
        case TemplateParameterType.Object:
          return TemplateUtil.TryConvertToJToken(token, out result, true);
        default:
          return TemplateUtil.TryConvertToJToken(token, out result);
      }
    }

    private static TemplateParameterType ConvertTemplateParameterType(TemplateToken typeToken)
    {
      LiteralToken literal = TemplateUtil.AssertLiteral(typeToken, "type");
      TemplateParameterType templateParameterType = TemplateParameterType.String;
      string str = literal.Value;
      if (str != null)
      {
        switch (str.Length)
        {
          case 3:
            if (str == "job")
            {
              templateParameterType = TemplateParameterType.Job;
              goto label_49;
            }
            else
              break;
          case 4:
            switch (str[0])
            {
              case 'p':
                if (str == "pool")
                {
                  templateParameterType = TemplateParameterType.Pool;
                  goto label_49;
                }
                else
                  break;
              case 's':
                if (str == "step")
                {
                  templateParameterType = TemplateParameterType.Step;
                  goto label_49;
                }
                else
                  break;
            }
            break;
          case 5:
            if (str == "stage")
            {
              templateParameterType = TemplateParameterType.Stage;
              goto label_49;
            }
            else
              break;
          case 6:
            switch (str[0])
            {
              case 'n':
                if (str == "number")
                {
                  templateParameterType = TemplateParameterType.Number;
                  goto label_49;
                }
                else
                  break;
              case 'o':
                if (str == "object")
                {
                  templateParameterType = TemplateParameterType.Object;
                  goto label_49;
                }
                else
                  break;
              case 's':
                if (str == "string")
                {
                  templateParameterType = TemplateParameterType.String;
                  goto label_49;
                }
                else
                  break;
            }
            break;
          case 7:
            switch (str[0])
            {
              case 'b':
                if (str == "boolean")
                {
                  templateParameterType = TemplateParameterType.Boolean;
                  goto label_49;
                }
                else
                  break;
              case 'j':
                if (str == "jobList")
                {
                  templateParameterType = TemplateParameterType.JobList;
                  goto label_49;
                }
                else
                  break;
            }
            break;
          case 8:
            switch (str[0])
            {
              case 'f':
                if (str == "filePath")
                {
                  templateParameterType = TemplateParameterType.FilePath;
                  goto label_49;
                }
                else
                  break;
              case 's':
                if (str == "stepList")
                {
                  templateParameterType = TemplateParameterType.StepList;
                  goto label_49;
                }
                else
                  break;
            }
            break;
          case 9:
            switch (str[0])
            {
              case 'c':
                if (str == "container")
                {
                  templateParameterType = TemplateParameterType.Container;
                  goto label_49;
                }
                else
                  break;
              case 's':
                if (str == "stageList")
                {
                  templateParameterType = TemplateParameterType.StageList;
                  goto label_49;
                }
                else
                  break;
            }
            break;
          case 10:
            switch (str[0])
            {
              case 'd':
                if (str == "deployment")
                {
                  templateParameterType = TemplateParameterType.Deployment;
                  goto label_49;
                }
                else
                  break;
              case 's':
                if (str == "secureFile")
                {
                  templateParameterType = TemplateParameterType.SecureFile;
                  goto label_49;
                }
                else
                  break;
            }
            break;
          case 11:
            if (str == "environment")
            {
              templateParameterType = TemplateParameterType.Environment;
              goto label_49;
            }
            else
              break;
          case 12:
            if (str == "legacyObject")
            {
              templateParameterType = TemplateParameterType.LegacyObject;
              goto label_49;
            }
            else
              break;
          case 13:
            if (str == "containerList")
            {
              templateParameterType = TemplateParameterType.ContainerList;
              goto label_49;
            }
            else
              break;
          case 14:
            if (str == "deploymentList")
            {
              templateParameterType = TemplateParameterType.DeploymentList;
              goto label_49;
            }
            else
              break;
          case 17:
            if (str == "serviceConnection")
            {
              templateParameterType = TemplateParameterType.ServiceConnection;
              goto label_49;
            }
            else
              break;
        }
      }
      TemplateUtil.AssertUnexpectedValue(literal, "parameter type");
label_49:
      return templateParameterType;
    }

    private static ExclusiveLockType ConvertToExclusiveLockType(TemplateToken lockBehaviorToken)
    {
      LiteralToken literal = TemplateUtil.AssertLiteral(lockBehaviorToken, "lockBehavior");
      ExclusiveLockType exclusiveLockType = ExclusiveLockType.RunLatest;
      switch (literal.Value)
      {
        case "sequential":
          exclusiveLockType = ExclusiveLockType.Sequential;
          break;
        case "runLatest":
          exclusiveLockType = ExclusiveLockType.RunLatest;
          break;
        default:
          TemplateUtil.AssertUnexpectedValue(literal, "lock behavior");
          break;
      }
      return exclusiveLockType;
    }

    private static IEnumerable<BuildResource> ConvertToBuildResources(
      TemplateContext context,
      IArtifactResolver artifactResolver,
      TemplateToken builds)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(builds, nameof (builds)))
      {
        BuildResource buildResource1 = new BuildResource();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "build resource");
        LiteralToken literal = (LiteralToken) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "build resource key");
          switch (literalToken1.Value)
          {
            case "build":
              literal = TemplateUtil.AssertLiteral(keyValuePair.Value, "build resource ref name");
              if (string.IsNullOrEmpty(literal.Value))
                TemplateUtil.AssertUnexpectedValue(literal, "build resource ref name");
              buildResource1.Alias = literal.Value;
              continue;
            case "connection":
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "build resource connection");
              BuildResource buildResource2 = buildResource1;
              ServiceEndpointReference endpointReference = new ServiceEndpointReference();
              endpointReference.Name = (ExpressionValue<string>) literalToken2.Value;
              buildResource2.Endpoint = endpointReference;
              continue;
            case "trigger":
              buildResource1.Trigger = TemplateResultConverter.ConvertToBuildResourceTrigger(context, keyValuePair.Value);
              continue;
            default:
              buildResource1.Properties.Set<JToken>(literalToken1.Value, TemplateUtil.ConvertToJToken(keyValuePair.Value));
              continue;
          }
        }
        PipelineValidationError error = (PipelineValidationError) null;
        if (artifactResolver != null && !artifactResolver.ValidateDeclaredResource((Resource) buildResource1, out error))
          context.Error((TemplateToken) literal, error);
        yield return buildResource1;
      }
    }

    private static BuildResourceTrigger ConvertToBuildResourceTrigger(
      TemplateContext context,
      TemplateToken triggerToken)
    {
      BuildResourceTrigger buildResourceTrigger = new BuildResourceTrigger();
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(triggerToken, "build resource trigger"))
      {
        if (TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "build resource trigger key").Value.Equals("enabled"))
          buildResourceTrigger.Enabled = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "pipeline resource trigger enabled"));
      }
      return buildResourceTrigger;
    }

    private static IEnumerable<ContainerResource> ConvertToContainerResources(
      TemplateContext context,
      TemplateToken containers)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(containers, nameof (containers)))
      {
        ContainerResource containerResource = new ContainerResource();
        foreach (KeyValuePair<ScalarToken, TemplateToken> containerPair in TemplateUtil.AssertMapping(templateToken, "container resource"))
        {
          LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) containerPair.Key, "container resource key");
          switch (literalToken1.Value)
          {
            case "container":
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(containerPair.Value, "container resource alias");
              containerResource.Alias = literalToken2.Value;
              continue;
            case "trigger":
              containerResource.Trigger = TemplateResultConverter.ConvertToContainerResourceTrigger(context, containerPair.Value);
              continue;
            default:
              if (!TemplateResultConverter.ParseContainerResourceProperty(context, containerPair, false, ref containerResource))
              {
                containerResource.Properties.Set<JToken>(literalToken1.Value, TemplateUtil.ConvertToJToken(containerPair.Value));
                continue;
              }
              continue;
          }
        }
        yield return containerResource;
      }
    }

    private static IEnumerable<PipelineResource> ConvertToPipelineResources(
      TemplateContext context,
      IArtifactResolver artifactResolver,
      TemplateToken pipelines)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(pipelines, nameof (pipelines)))
      {
        PipelineResource pipelineResource = new PipelineResource();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "pipeline resource");
        LiteralToken literal = (LiteralToken) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "pipeline resource key");
          switch (literalToken.Value)
          {
            case "pipeline":
              literal = TemplateUtil.AssertLiteral(keyValuePair.Value, "pipeline resource name");
              if (string.IsNullOrEmpty(literal.Value))
                TemplateUtil.AssertUnexpectedValue(literal, "pipeline resource name");
              pipelineResource.Alias = literal.Value;
              continue;
            case "trigger":
              pipelineResource.Trigger = TemplateResultConverter.ConvertToPipelineResourceTrigger(context, keyValuePair.Value);
              continue;
            default:
              pipelineResource.Properties.Set<JToken>(literalToken.Value, TemplateUtil.ConvertToJToken(keyValuePair.Value));
              continue;
          }
        }
        PipelineValidationError error = (PipelineValidationError) null;
        if (artifactResolver != null && !artifactResolver.ValidateDeclaredResource((Resource) pipelineResource, out error))
          context.Error((TemplateToken) literal, error);
        yield return pipelineResource;
      }
    }

    private static IEnumerable<PackageResource> ConvertToPackageResources(
      TemplateContext context,
      IArtifactResolver artifactResolver,
      TemplateToken packages)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(packages, nameof (packages)))
      {
        PackageResource packageResource1 = new PackageResource();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "package resource");
        LiteralToken literal1 = (LiteralToken) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "package resource key");
          string str = literalToken.Value;
          if (str != null)
          {
            switch (str.Length)
            {
              case 3:
                if (str == "tag")
                {
                  LiteralToken literal2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "tag");
                  if (string.IsNullOrEmpty(literal2.Value))
                    TemplateUtil.AssertUnexpectedValue(literal2, "tag");
                  packageResource1.Tag = literal2.Value;
                  continue;
                }
                break;
              case 4:
                switch (str[0])
                {
                  case 'n':
                    if (str == "name")
                    {
                      LiteralToken literal3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "package name");
                      if (string.IsNullOrEmpty(literal3.Value))
                        TemplateUtil.AssertUnexpectedValue(literal3, "package name");
                      packageResource1.Name = literal3.Value;
                      continue;
                    }
                    break;
                  case 't':
                    if (str == "type")
                    {
                      LiteralToken literal4 = TemplateUtil.AssertLiteral(keyValuePair.Value, "package type");
                      if (string.IsNullOrEmpty(literal4.Value))
                        TemplateUtil.AssertUnexpectedValue(literal4, "package type");
                      packageResource1.Type = literal4.Value;
                      continue;
                    }
                    break;
                }
                break;
              case 7:
                switch (str[0])
                {
                  case 'p':
                    if (str == "package")
                    {
                      literal1 = TemplateUtil.AssertLiteral(keyValuePair.Value, "package resource name");
                      if (string.IsNullOrEmpty(literal1.Value))
                        TemplateUtil.AssertUnexpectedValue(literal1, "package resource name");
                      packageResource1.Alias = literal1.Value;
                      continue;
                    }
                    break;
                  case 't':
                    if (str == "trigger")
                    {
                      packageResource1.Trigger = TemplateResultConverter.ConvertToPackageResourceTrigger(context, keyValuePair.Value);
                      continue;
                    }
                    break;
                  case 'v':
                    if (str == "version")
                    {
                      LiteralToken literal5 = TemplateUtil.AssertLiteral(keyValuePair.Value, "package version");
                      if (string.IsNullOrEmpty(literal5.Value))
                        TemplateUtil.AssertUnexpectedValue(literal5, "package version");
                      packageResource1.Version = literal5.Value;
                      continue;
                    }
                    break;
                }
                break;
              case 10:
                if (str == "connection")
                {
                  LiteralToken literal6 = TemplateUtil.AssertLiteral(keyValuePair.Value, "package resource connection");
                  if (string.IsNullOrEmpty(literal6.Value))
                    TemplateUtil.AssertUnexpectedValue(literal6, "package resource connection");
                  PackageResource packageResource2 = packageResource1;
                  ServiceEndpointReference endpointReference = new ServiceEndpointReference();
                  endpointReference.Name = (ExpressionValue<string>) literal6.Value;
                  packageResource2.Endpoint = endpointReference;
                  continue;
                }
                break;
            }
          }
          packageResource1.Properties.Set<JToken>(literalToken.Value, TemplateUtil.ConvertToJToken(keyValuePair.Value));
        }
        PipelineValidationError error = (PipelineValidationError) null;
        if (artifactResolver != null && !artifactResolver.ValidateDeclaredResource((Resource) packageResource1, out error))
          context.Error((TemplateToken) literal1, error.Message);
        yield return packageResource1;
      }
    }

    private static PackageResourceTrigger ConvertToPackageResourceTrigger(
      TemplateContext context,
      TemplateToken triggerToken)
    {
      PackageResourceTrigger packageResourceTrigger = new PackageResourceTrigger();
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(triggerToken, "package resource trigger"))
      {
        if (TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "package resource trigger key").Value.Equals("enabled"))
          packageResourceTrigger.Enabled = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "package resource trigger enabled"));
      }
      return packageResourceTrigger;
    }

    private static ContainerResourceTrigger ConvertToContainerResourceTrigger(
      TemplateContext context,
      TemplateToken triggerToken)
    {
      ContainerResourceTrigger containerResourceTrigger = new ContainerResourceTrigger();
      bool flag = true;
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(triggerToken, "container resource trigger"))
      {
        switch (TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "container resource trigger key").Value)
        {
          case "enabled":
            flag = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "container resource trigger enabled"));
            continue;
          case "tags":
            containerResourceTrigger.TagFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value));
            continue;
          default:
            continue;
        }
      }
      return !flag ? (ContainerResourceTrigger) null : containerResourceTrigger;
    }

    private static PipelineResourceTrigger ConvertToPipelineResourceTrigger(
      TemplateContext context,
      TemplateToken triggerToken)
    {
      PipelineResourceTrigger pipelineResourceTrigger = new PipelineResourceTrigger();
      bool flag = true;
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(triggerToken, "pipeline resource trigger"))
      {
        switch (TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "pipeline resource trigger key").Value)
        {
          case "branches":
            pipelineResourceTrigger.BranchFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value));
            continue;
          case "enabled":
            flag = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "pipeline resource trigger enabled"));
            continue;
          case "stages":
            pipelineResourceTrigger.StageFilters.AddRange<string, IList<string>>((IEnumerable<string>) TemplateResultConverter.ConvertToListOfString(context, keyValuePair.Value));
            continue;
          case "tags":
            pipelineResourceTrigger.TagFilters.AddRange<string, IList<string>>((IEnumerable<string>) TemplateResultConverter.ConvertToListOfString(context, keyValuePair.Value));
            continue;
          default:
            continue;
        }
      }
      return !flag ? (PipelineResourceTrigger) null : pipelineResourceTrigger;
    }

    private static IEnumerable<WebhookResource> ConvertToWebhookResources(
      TemplateContext context,
      TemplateToken webhooks,
      IArtifactResolver artifactResolver)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(webhooks, nameof (webhooks)))
      {
        WebhookResource webhookResource1 = new WebhookResource();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "webhook resource");
        LiteralToken literal = (LiteralToken) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          switch (TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "webhook resource key").Value)
          {
            case "webhook":
              literal = TemplateUtil.AssertLiteral(keyValuePair.Value, "webhook resource alias");
              if (string.IsNullOrEmpty(literal.Value))
                TemplateUtil.AssertUnexpectedValue(literal, "webhook resource ref name");
              webhookResource1.Alias = literal.Value;
              webhookResource1.Trigger = new WebhookResourceTrigger();
              continue;
            case "filters":
              SequenceToken triggerFilters = TemplateUtil.AssertSequence(keyValuePair.Value, "webhook resource filters");
              webhookResource1.Trigger.Filters.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) TemplateResultConverter.ConvertToWebhookTriggerFilters(context, (TemplateToken) triggerFilters));
              continue;
            case "type":
              TemplateUtil.AssertLiteral(keyValuePair.Value, "webhook resource type");
              webhookResource1.Properties.Set<JToken>(WebhookPropertyNames.Type, TemplateUtil.ConvertToJToken(keyValuePair.Value));
              continue;
            case "connection":
              LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "container resource endpoint");
              WebhookResource webhookResource2 = webhookResource1;
              ServiceEndpointReference endpointReference = new ServiceEndpointReference();
              endpointReference.Name = (ExpressionValue<string>) literalToken.Value;
              webhookResource2.Endpoint = endpointReference;
              continue;
            default:
              continue;
          }
        }
        PipelineValidationError error = (PipelineValidationError) null;
        if (artifactResolver != null && !artifactResolver.ValidateDeclaredResource((Resource) webhookResource1, out error))
          context.Error((TemplateToken) literal, error.Message);
        yield return webhookResource1;
      }
    }

    private static IDictionary<string, string> ConvertToWebhookTriggerFilters(
      TemplateContext context,
      TemplateToken triggerFilters)
    {
      SequenceToken sequenceToken = TemplateUtil.AssertSequence(triggerFilters, "webhooks trigger filters");
      IDictionary<string, string> webhookTriggerFilters = (IDictionary<string, string>) new Dictionary<string, string>();
      foreach (TemplateToken templateToken in sequenceToken)
      {
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "webhook resource trigger filter");
        LiteralToken literal1 = (LiteralToken) null;
        LiteralToken literal2 = (LiteralToken) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          switch (TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "webhook resource trigger filter key").Value)
          {
            case "path":
              literal1 = TemplateUtil.AssertLiteral(keyValuePair.Value, "webhook resource alias");
              continue;
            case "value":
              literal2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "webhook resource alias");
              continue;
            default:
              continue;
          }
        }
        if (string.IsNullOrEmpty(literal1.Value))
          TemplateUtil.AssertUnexpectedValue(literal1, "webhook resource trigger filter path property");
        if (string.IsNullOrEmpty(literal2.Value))
          TemplateUtil.AssertUnexpectedValue(literal2, "webhook resource trigger filter value property");
        webhookTriggerFilters[literal1.Value] = literal2.Value;
      }
      return webhookTriggerFilters;
    }

    private static List<string> ConvertToListOfString(
      TemplateContext context,
      TemplateToken stringsToken)
    {
      List<string> listOfString = new List<string>();
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(stringsToken, "stringtokens"))
      {
        string str = TemplateUtil.AssertLiteral(templateToken, "string value").Value ?? string.Empty;
        if (!string.IsNullOrEmpty(str))
          listOfString.Add(str);
      }
      return listOfString;
    }

    private static void ValidateRepositoryResourceTemplate(
      TemplateContext context,
      RepositoryResource repositoryResource)
    {
      ResourceProperties properties = repositoryResource.Properties;
      string str;
      if ((!properties.TryGetValue<string>(RepositoryPropertyNames.Type, out str) || !(str == "git") || !properties.TryGetValue<string>(RepositoryPropertyNames.Connection, out string _) ? 0 : (properties.TryGetValue<ContinuousIntegrationTrigger>(RepositoryPropertyNames.Trigger, out ContinuousIntegrationTrigger _) ? 1 : (properties.TryGetValue<PullRequestTrigger>(RepositoryPropertyNames.PR, out PullRequestTrigger _) ? 1 : 0))) == 0)
        return;
      context.Errors.Add("Triggers are not supported for cross-organization repository resources in Azure Repos");
    }

    private static ContinuousIntegrationTrigger ConvertToContinuousIntegrationTrigger(
      TemplateContext context,
      TemplateToken trigger)
    {
      ContinuousIntegrationTrigger integrationTrigger = new ContinuousIntegrationTrigger();
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(trigger, nameof (trigger)))
      {
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "trigger key");
        switch (literal.Value)
        {
          case "enabled":
            integrationTrigger.Enabled = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "trigger enabled"));
            continue;
          case "batch":
            integrationTrigger.BatchChanges = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "trigger batch"));
            continue;
          case "branches":
            integrationTrigger.BranchFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value));
            continue;
          case "paths":
            integrationTrigger.PathFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value));
            continue;
          case "tags":
            integrationTrigger.BranchFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value, "refs/tags/"));
            continue;
          default:
            TemplateUtil.AssertUnexpectedValue(literal, "trigger key");
            continue;
        }
      }
      return integrationTrigger;
    }

    private static PullRequestTrigger ConvertToPullRequestTrigger(
      TemplateContext context,
      TemplateToken pr)
    {
      PullRequestTrigger pullRequestTrigger = new PullRequestTrigger();
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(pr, nameof (pr)))
      {
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "pr key");
        switch (literal.Value)
        {
          case "autoCancel":
            pullRequestTrigger.AutoCancel = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "pr auto cancel"));
            continue;
          case "branches":
            pullRequestTrigger.BranchFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value));
            continue;
          case "enabled":
            pullRequestTrigger.Enabled = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "pr enabled"));
            continue;
          case "paths":
            pullRequestTrigger.PathFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value));
            continue;
          case "drafts":
            pullRequestTrigger.Drafts = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "pr draft"));
            continue;
          default:
            TemplateUtil.AssertUnexpectedValue(literal, "pr key");
            continue;
        }
      }
      return pullRequestTrigger;
    }

    private static IEnumerable<string> ConvertToIncludeExcludeFilters(
      TemplateContext context,
      TemplateToken filters,
      string prefix = "")
    {
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(filters, "include exclude filters"))
      {
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "include exclude filters key");
        switch (literal.Value)
        {
          case "include":
            foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(keyValuePair.Value, "include filters"))
            {
              string str = TemplateUtil.AssertLiteral(templateToken, "include filter").Value ?? string.Empty;
              if (string.IsNullOrEmpty(prefix) || str.StartsWith(prefix, StringComparison.Ordinal))
                yield return "+" + str;
              else
                yield return "+" + prefix + str;
            }
            continue;
          case "exclude":
            foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(keyValuePair.Value, "exclude filters"))
            {
              string str = TemplateUtil.AssertLiteral(templateToken, "exclude filter").Value ?? string.Empty;
              if (string.IsNullOrEmpty(prefix) || str.StartsWith(prefix, StringComparison.Ordinal))
                yield return "-" + str;
              else
                yield return "-" + prefix + str;
            }
            continue;
          default:
            TemplateUtil.AssertUnexpectedValue(literal, "include exclude filters key");
            continue;
        }
      }
    }

    private static IEnumerable<PipelineSchedule> ConvertToSchedules(
      TemplateContext context,
      TemplateToken schedules)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(schedules, nameof (schedules)))
      {
        PipelineSchedule schedule = new PipelineSchedule();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "schedule item");
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken key = keyValuePair.Key as LiteralToken;
          switch (key.Value)
          {
            case "cron":
              LiteralToken literalToken1 = TemplateUtil.AssertLiteral(keyValuePair.Value, "schedule cron details");
              schedule.ScheduleDetails = literalToken1.Value;
              continue;
            case "displayName":
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "schedule displayName");
              schedule.DisplayName = literalToken2.Value;
              continue;
            case "branches":
              schedule.BranchFilters.AddRange<string, IList<string>>(TemplateResultConverter.ConvertToIncludeExcludeFilters(context, keyValuePair.Value));
              continue;
            case "batch":
              schedule.BatchSchedules = TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "schedule batch"));
              continue;
            case "always":
              schedule.ScheduleOnlyWithChanges = !TemplateResultConverter.ConvertToBoolean(context, TemplateUtil.AssertLiteral(keyValuePair.Value, "always schedule definition"));
              continue;
            default:
              TemplateUtil.AssertUnexpectedValue(key, "schedule key");
              continue;
          }
        }
        string errorMessage;
        if (!TemplateResultConverter.ValidatePipelineSchedule(schedule, out errorMessage))
          context.Error((TemplateToken) mappingToken, errorMessage);
        else
          yield return schedule;
      }
    }

    private static IEnumerable<Stage> ConvertToStages(
      TemplateContext context,
      IList<TaskDefinition> tasks,
      PipelineResources resources,
      IArtifactResolver artifactResolver,
      TemplateToken stages,
      MappingToken parentPool,
      ExclusiveLockType parentLockBehavior)
    {
      List<Stage> stages1 = new List<Stage>();
      SequenceToken sequenceToken = TemplateUtil.AssertSequence(stages, nameof (stages));
      List<Stage> stageList = new List<Stage>();
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<Tuple<Stage, Stage>> tupleList = new List<Tuple<Stage, Stage>>();
      foreach (TemplateToken templateToken in sequenceToken)
      {
        Stage stage = new Stage();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "stage");
        bool flag = false;
        MappingToken parentPool1 = parentPool;
        ExclusiveLockType exclusiveLockType = parentLockBehavior;
        TemplateToken jobs = (TemplateToken) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "stage key");
          string str = literal.Value;
          if (str != null)
          {
            switch (str.Length)
            {
              case 4:
                switch (str[0])
                {
                  case 'j':
                    if (str == "jobs")
                    {
                      jobs = keyValuePair.Value;
                      continue;
                    }
                    break;
                  case 'p':
                    if (str == "pool")
                    {
                      parentPool1 = TemplateUtil.AssertMapping(keyValuePair.Value, "pool");
                      continue;
                    }
                    break;
                }
                break;
              case 5:
                switch (str[0])
                {
                  case 'g':
                    if (str == "group")
                    {
                      LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "stage group");
                      stage.Group = literalToken.Value;
                      continue;
                    }
                    break;
                  case 's':
                    if (str == "stage")
                    {
                      LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "stage name");
                      stage.Name = literalToken.Value;
                      continue;
                    }
                    break;
                }
                break;
              case 9:
                switch (str[0])
                {
                  case 'c':
                    if (str == "condition")
                    {
                      LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "stage condition");
                      stage.Condition = literalToken.Value;
                      continue;
                    }
                    break;
                  case 'd':
                    if (str == "dependsOn")
                    {
                      flag = true;
                      using (IEnumerator<TemplateToken> enumerator = TemplateUtil.AssertSequence(keyValuePair.Value, "stage dependsOn").GetEnumerator())
                      {
                        while (enumerator.MoveNext())
                        {
                          LiteralToken literalToken = TemplateUtil.AssertLiteral(enumerator.Current, "stage dependency");
                          stage.DependsOn.Add(literalToken.Value);
                        }
                        continue;
                      }
                    }
                    else
                      break;
                  case 'v':
                    if (str == "variables")
                    {
                      stage.Variables.AddRange<IVariable, IList<IVariable>>(TemplateResultConverter.ConvertToVariables(context, keyValuePair.Value));
                      continue;
                    }
                    break;
                }
                break;
              case 11:
                if (str == "displayName")
                {
                  LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "stage displayName");
                  stage.DisplayName = literalToken.Value;
                  continue;
                }
                break;
              case 12:
                if (str == "lockBehavior")
                {
                  exclusiveLockType = TemplateResultConverter.ConvertToExclusiveLockType(keyValuePair.Value);
                  continue;
                }
                break;
              case 15:
                if (str == "templateContext")
                {
                  TemplateUtil.AssertMapping(keyValuePair.Value, "templateContext");
                  continue;
                }
                break;
            }
          }
          TemplateUtil.AssertUnexpectedValue(literal, "stage key");
        }
        if (jobs != null)
          stage.Phases.AddRange<PhaseNode, IList<PhaseNode>>(TemplateResultConverter.ConvertToPhases(context, tasks, resources, artifactResolver, jobs, parentPool1));
        stage.LockBehavior = exclusiveLockType;
        if (string.IsNullOrEmpty(stage.DisplayName))
          stage.DisplayName = stage.Name;
        if (string.IsNullOrEmpty(stage.Group))
          stage.Group = "\\";
        else if (stage.Group[0] == '\\')
          stage.Group = "\\" + stage.Group;
        if (string.IsNullOrEmpty(stage.Name))
        {
          stageList.Add(stage);
          stage.IsKnownName = false;
        }
        else
        {
          stringSet.Add(stage.Name);
          stage.IsKnownName = true;
        }
        if (!flag && stages1.Count > 0)
          tupleList.Add(new Tuple<Stage, Stage>(stage, stages1[stages1.Count - 1]));
        stages1.Add(stage);
      }
      int num = 1;
      foreach (Stage stage in stageList)
      {
        string str;
        for (str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "stage{0}", (object) num); stringSet.Contains(str); str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "stage{0}", (object) num))
          ++num;
        stage.Name = str;
        ++num;
      }
      foreach (Tuple<Stage, Stage> tuple in tupleList)
        tuple.Item1.DependsOn.Add(tuple.Item2.Name);
      return (IEnumerable<Stage>) stages1;
    }

    private static IEnumerable<PhaseNode> ConvertToPhases(
      TemplateContext context,
      IList<TaskDefinition> tasks,
      PipelineResources resources,
      IArtifactResolver artifactResolver,
      TemplateToken jobs,
      MappingToken parentPool)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(jobs, nameof (jobs)))
      {
        MappingToken mappingToken1 = TemplateUtil.AssertMapping(templateToken, "job");
        TemplateUtil.AssertNotEmpty(mappingToken1, "job");
        LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) mappingToken1[0].Key, "job first key");
        LiteralToken literalToken2 = TemplateUtil.AssertLiteral(mappingToken1[0].Value, "job name");
        if (string.Equals(literalToken1.Value, "job", StringComparison.Ordinal))
        {
          Phase phase1 = new Phase();
          phase1.Name = literalToken2.Value;
          Phase phase2 = phase1;
          MappingToken strategy = (MappingToken) null;
          MappingToken pool = parentPool;
          MappingToken container = (MappingToken) null;
          MappingToken services = (MappingToken) null;
          MappingToken workspace = (MappingToken) null;
          LiteralToken timeoutInMinutes = (LiteralToken) null;
          LiteralToken cancelTimeoutInMinutes = (LiteralToken) null;
          LiteralToken retryCountOnTaskFailure = (LiteralToken) null;
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken1.Skip<KeyValuePair<ScalarToken, TemplateToken>>(1))
          {
            LiteralToken literal1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "job key");
            string str = literal1.Value;
            if (str != null)
            {
              switch (str.Length)
              {
                case 4:
                  switch (str[0])
                  {
                    case 'p':
                      if (str == "pool")
                      {
                        pool = TemplateUtil.AssertMapping(keyValuePair.Value, "pool");
                        continue;
                      }
                      break;
                    case 'u':
                      if (str == "uses")
                      {
                        phase2.ExplicitResources = TemplateResultConverter.ConvertToReferencedResources(context, keyValuePair.Value);
                        continue;
                      }
                      break;
                  }
                  break;
                case 5:
                  if (str == "steps")
                  {
                    phase2.Steps.AddRange<Step, IList<Step>>(TemplateResultConverter.ConvertToSteps(context, tasks, resources, artifactResolver, keyValuePair.Value));
                    continue;
                  }
                  break;
                case 8:
                  switch (str[1])
                  {
                    case 'e':
                      if (str == "services")
                      {
                        services = TemplateUtil.AssertMapping(keyValuePair.Value, "job services");
                        continue;
                      }
                      break;
                    case 't':
                      if (str == "strategy")
                      {
                        strategy = TemplateUtil.AssertMapping(keyValuePair.Value, "strategy");
                        continue;
                      }
                      break;
                  }
                  break;
                case 9:
                  switch (str[3])
                  {
                    case 'd':
                      if (str == "condition")
                      {
                        LiteralToken literalToken3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "job condition");
                        phase2.Condition = literalToken3.Value;
                        continue;
                      }
                      break;
                    case 'e':
                      if (str == "dependsOn")
                      {
                        using (IEnumerator<TemplateToken> enumerator = TemplateUtil.AssertSequence(keyValuePair.Value, "job dependsOn").GetEnumerator())
                        {
                          while (enumerator.MoveNext())
                          {
                            LiteralToken literalToken4 = TemplateUtil.AssertLiteral(enumerator.Current, "job dependency");
                            phase2.DependsOn.Add(literalToken4.Value);
                          }
                          continue;
                        }
                      }
                      else
                        break;
                    case 'i':
                      if (str == "variables")
                      {
                        phase2.Variables.AddRange<IVariable, IList<IVariable>>(TemplateResultConverter.ConvertToVariables(context, keyValuePair.Value));
                        continue;
                      }
                      break;
                    case 'k':
                      if (str == "workspace")
                      {
                        workspace = TemplateUtil.AssertMapping(keyValuePair.Value, "job workspace");
                        continue;
                      }
                      break;
                    case 't':
                      if (str == "container")
                      {
                        container = TemplateUtil.AssertMapping(keyValuePair.Value, "job container");
                        continue;
                      }
                      break;
                  }
                  break;
                case 11:
                  if (str == "displayName")
                  {
                    LiteralToken literalToken5 = TemplateUtil.AssertLiteral(keyValuePair.Value, "job displayName");
                    phase2.DisplayName = literalToken5.Value;
                    continue;
                  }
                  break;
                case 15:
                  switch (str[0])
                  {
                    case 'c':
                      if (str == "continueOnError")
                      {
                        LiteralToken literal2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "job continue on error");
                        phase2.ContinueOnError = TemplateResultConverter.ConvertToPipelineExpressionOfBoolean(context, literal2);
                        continue;
                      }
                      break;
                    case 't':
                      if (str == "templateContext")
                      {
                        TemplateUtil.AssertMapping(keyValuePair.Value, "templateContext");
                        continue;
                      }
                      break;
                  }
                  break;
                case 16:
                  if (str == "timeoutInMinutes")
                  {
                    timeoutInMinutes = TemplateUtil.AssertLiteral(keyValuePair.Value, "timeoutInMinutes");
                    continue;
                  }
                  break;
                case 22:
                  if (str == "cancelTimeoutInMinutes")
                  {
                    cancelTimeoutInMinutes = TemplateUtil.AssertLiteral(keyValuePair.Value, "cancelTimeoutInMinutes");
                    continue;
                  }
                  break;
                case 23:
                  if (str == "retryCountOnTaskFailure")
                  {
                    retryCountOnTaskFailure = TemplateUtil.AssertLiteral(keyValuePair.Value, "retryCountOnTaskFailure");
                    continue;
                  }
                  break;
              }
            }
            TemplateUtil.AssertUnexpectedValue(literal1, "job key");
          }
          phase2.Target = TemplateResultConverter.ConvertToPhaseTarget(context, resources, strategy, pool, container, services, workspace, timeoutInMinutes, cancelTimeoutInMinutes, retryCountOnTaskFailure);
          if (string.IsNullOrEmpty(phase2.DisplayName))
            phase2.DisplayName = phase2.Name;
          yield return (PhaseNode) phase2;
        }
        else
        {
          MappingToken pool = parentPool;
          MappingToken workspace = (MappingToken) null;
          LiteralToken timeoutInMinutes = (LiteralToken) null;
          LiteralToken cancelTimeoutInMinutes = (LiteralToken) null;
          LiteralToken retryCountOnTaskFailure = (LiteralToken) null;
          MappingToken container = (MappingToken) null;
          MappingToken services = (MappingToken) null;
          ProviderPhase providerPhase = new ProviderPhase();
          providerPhase.Name = literalToken2.Value;
          providerPhase.Provider = literalToken1.Value;
          ProviderPhase phase = providerPhase;
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken1.Skip<KeyValuePair<ScalarToken, TemplateToken>>(1))
          {
            LiteralToken literal3 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "job key");
            string str = literal3.Value;
            if (str != null)
            {
              switch (str.Length)
              {
                case 4:
                  switch (str[0])
                  {
                    case 'p':
                      if (str == "pool")
                      {
                        pool = TemplateUtil.AssertMapping(keyValuePair.Value, "pool");
                        continue;
                      }
                      break;
                    case 'u':
                      if (str == "uses")
                      {
                        phase.ExplicitResources = TemplateResultConverter.ConvertToReferencedResources(context, keyValuePair.Value);
                        continue;
                      }
                      break;
                  }
                  break;
                case 8:
                  switch (str[1])
                  {
                    case 'e':
                      if (str == "services")
                      {
                        services = TemplateUtil.AssertMapping(keyValuePair.Value, "job services");
                        continue;
                      }
                      break;
                    case 't':
                      if (str == "strategy")
                      {
                        MappingToken mappingToken2 = TemplateUtil.AssertMapping(keyValuePair.Value, "job strategy");
                        if (phase.Strategy == null)
                          phase.Strategy = new Dictionary<string, JToken>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                        using (IEnumerator<KeyValuePair<ScalarToken, TemplateToken>> enumerator = mappingToken2.GetEnumerator())
                        {
                          while (enumerator.MoveNext())
                          {
                            KeyValuePair<ScalarToken, TemplateToken> current = enumerator.Current;
                            LiteralToken literalToken6 = TemplateUtil.AssertLiteral((TemplateToken) current.Key, "strategy key");
                            phase.Strategy[literalToken6.Value] = TemplateUtil.ConvertToJToken(current.Value);
                          }
                          continue;
                        }
                      }
                      else
                        break;
                  }
                  break;
                case 9:
                  switch (str[3])
                  {
                    case 'd':
                      if (str == "condition")
                      {
                        LiteralToken literalToken7 = TemplateUtil.AssertLiteral(keyValuePair.Value, "job condition");
                        phase.Condition = literalToken7.Value;
                        continue;
                      }
                      break;
                    case 'e':
                      if (str == "dependsOn")
                      {
                        using (IEnumerator<TemplateToken> enumerator = TemplateUtil.AssertSequence(keyValuePair.Value, "job dependsOn").GetEnumerator())
                        {
                          while (enumerator.MoveNext())
                          {
                            LiteralToken literalToken8 = TemplateUtil.AssertLiteral(enumerator.Current, "job dependency");
                            phase.DependsOn.Add(literalToken8.Value);
                          }
                          continue;
                        }
                      }
                      else
                        break;
                    case 'i':
                      if (str == "variables")
                      {
                        phase.Variables.AddRange<IVariable, IList<IVariable>>(TemplateResultConverter.ConvertToVariables(context, keyValuePair.Value));
                        continue;
                      }
                      break;
                    case 'k':
                      if (str == "workspace")
                      {
                        workspace = TemplateUtil.AssertMapping(keyValuePair.Value, "job workspace");
                        continue;
                      }
                      break;
                    case 't':
                      if (str == "container")
                      {
                        container = TemplateUtil.AssertMapping(keyValuePair.Value, "job container");
                        continue;
                      }
                      break;
                  }
                  break;
                case 11:
                  switch (str[0])
                  {
                    case 'd':
                      if (str == "displayName")
                      {
                        LiteralToken literalToken9 = TemplateUtil.AssertLiteral(keyValuePair.Value, "job displayName");
                        phase.DisplayName = literalToken9.Value;
                        continue;
                      }
                      break;
                    case 'e':
                      if (str == "environment")
                      {
                        MappingToken environment = TemplateUtil.AssertMapping(keyValuePair.Value, "environment");
                        phase.EnvironmentTarget = TemplateResultConverter.ConvertToEnvironmentTarget(context, environment);
                        continue;
                      }
                      break;
                  }
                  break;
                case 15:
                  switch (str[0])
                  {
                    case 'c':
                      if (str == "continueOnError")
                      {
                        LiteralToken literal4 = TemplateUtil.AssertLiteral(keyValuePair.Value, "job continue on error");
                        phase.ContinueOnError = TemplateResultConverter.ConvertToPipelineExpressionOfBoolean(context, literal4);
                        continue;
                      }
                      break;
                    case 't':
                      if (str == "templateContext")
                      {
                        TemplateUtil.AssertMapping(keyValuePair.Value, "templateContext");
                        continue;
                      }
                      break;
                  }
                  break;
                case 16:
                  if (str == "timeoutInMinutes")
                  {
                    timeoutInMinutes = TemplateUtil.AssertLiteral(keyValuePair.Value, "timeoutInMinutes");
                    continue;
                  }
                  break;
                case 22:
                  if (str == "cancelTimeoutInMinutes")
                  {
                    cancelTimeoutInMinutes = TemplateUtil.AssertLiteral(keyValuePair.Value, "cancelTimeoutInMinutes");
                    continue;
                  }
                  break;
                case 23:
                  if (str == "retryCountOnTaskFailure")
                  {
                    retryCountOnTaskFailure = TemplateUtil.AssertLiteral(keyValuePair.Value, "retryCountOnTaskFailure");
                    continue;
                  }
                  break;
              }
            }
            TemplateUtil.AssertUnexpectedValue(literal3, "job key");
          }
          if (string.IsNullOrEmpty(phase.DisplayName))
            phase.DisplayName = phase.Name;
          phase.Target = TemplateResultConverter.ConvertToPhaseTarget(context, resources, (MappingToken) null, pool, container, services, workspace, timeoutInMinutes, cancelTimeoutInMinutes, retryCountOnTaskFailure);
          yield return (PhaseNode) phase;
        }
      }
    }

    private static IEnumerable<IVariable> ConvertToVariables(
      TemplateContext context,
      TemplateToken variables)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(variables, nameof (variables)))
      {
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "variables item");
        KeyValuePair<ScalarToken, TemplateToken> keyValuePair = mappingToken[0];
        LiteralToken literal1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "variable first key");
        switch (literal1.Value)
        {
          case "name":
            keyValuePair = mappingToken[0];
            LiteralToken literalToken1 = TemplateUtil.AssertLiteral(keyValuePair.Value, "variable name");
            LiteralToken literalToken2 = (LiteralToken) null;
            bool flag = false;
            if (mappingToken.Count > 1)
            {
              for (int index = 1; index < mappingToken.Count; ++index)
              {
                keyValuePair = mappingToken[index];
                LiteralToken literal2 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "variable next key");
                switch (literal2.Value)
                {
                  case "value":
                    keyValuePair = mappingToken[index];
                    literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "variable value");
                    break;
                  case "readonly":
                    TemplateContext context1 = context;
                    keyValuePair = mappingToken[index];
                    LiteralToken literal3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "variable readonly");
                    flag = TemplateResultConverter.ConvertToBoolean(context1, literal3);
                    break;
                  default:
                    TemplateUtil.AssertUnexpectedValue(literal2, "variable next key");
                    break;
                }
              }
            }
            yield return (IVariable) new Variable()
            {
              Name = literalToken1.Value,
              Value = literalToken2?.Value,
              Readonly = flag
            };
            continue;
          case "group":
            keyValuePair = mappingToken[0];
            LiteralToken literalToken3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "variable group name");
            VariableGroupReference variable = new VariableGroupReference();
            variable.Name = (ExpressionValue<string>) literalToken3.Value;
            yield return (IVariable) variable;
            continue;
          default:
            TemplateUtil.AssertUnexpectedValue(literal1, "variable first key");
            continue;
        }
      }
    }

    private static EnvironmentDeploymentTarget ConvertToEnvironmentTarget(
      TemplateContext context,
      MappingToken environment)
    {
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      int? nullable1 = new int?();
      EnvironmentResourceType? nullable2 = new EnvironmentResourceType?();
      IList<string> collection = (IList<string>) new List<string>();
      bool flag = false;
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in environment)
      {
        LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "environment key");
        LiteralToken literal = TemplateUtil.AssertLiteral(keyValuePair.Value, "environment value");
        if (string.Equals(literalToken.Value, "name", StringComparison.OrdinalIgnoreCase))
        {
          flag = true;
          empty1 = literal.Value;
        }
        if (string.Equals(literalToken.Value, "resourceName", StringComparison.OrdinalIgnoreCase))
          empty2 = literal.Value;
        if (string.Equals(literalToken.Value, "resourceId", StringComparison.OrdinalIgnoreCase))
          nullable1 = new int?(TemplateResultConverter.ConvertToInt32(context, literal));
        if (string.Equals(literalToken.Value, "resourceType", StringComparison.OrdinalIgnoreCase))
          nullable2 = new EnvironmentResourceType?(EnvironmentDeploymentUtility.ConvertToEnvironmentResourceType(literal.Value));
        if (string.Equals(literalToken.Value, "tags", StringComparison.OrdinalIgnoreCase))
          collection.AddRange<string, IList<string>>(((IEnumerable<string>) literal.Value.Split(',')).Select<string, string>((Func<string, string>) (x => x.Trim())).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
      }
      if (!flag)
        context.Error((TemplateToken) environment, YamlStrings.EnvironmentNameKeyNotFound((object) "name"));
      if (!VariableUtility.IsVariable(empty1))
      {
        string[] strArray = empty1.Split('.');
        if (strArray.Length > 1)
        {
          if (!string.IsNullOrWhiteSpace(empty2))
            context.Error((TemplateToken) environment, YamlStrings.EnvironmentResourceNameConflict((object) strArray[1], (object) empty2));
          empty1 = strArray[0];
          empty2 = strArray[1];
        }
      }
      EnvironmentDeploymentTarget environmentTarget = new EnvironmentDeploymentTarget()
      {
        EnvironmentName = empty1
      };
      EnvironmentResourceFilter environmentResourceFilter = new EnvironmentResourceFilter();
      environmentResourceFilter.Tags = collection;
      if (!string.IsNullOrWhiteSpace(empty2))
      {
        environmentTarget.Resource = new EnvironmentResourceReference()
        {
          Name = empty2
        };
        environmentResourceFilter.Name = empty2;
      }
      if (nullable1.HasValue)
        environmentResourceFilter.Id = new int?(nullable1.Value);
      if (nullable2.HasValue)
        environmentResourceFilter.Type = new EnvironmentResourceType?(nullable2.Value);
      if (collection.Count > 0 && !environmentResourceFilter.Type.HasValue)
        context.Error((TemplateToken) environment, YamlStrings.ResourceTypeIsMandatoryWithTags());
      environmentTarget.ResourceFilter = environmentResourceFilter;
      return environmentTarget;
    }

    private static PhaseTarget ConvertToPhaseTarget(
      TemplateContext context,
      PipelineResources resources,
      MappingToken strategy,
      MappingToken pool,
      MappingToken container,
      MappingToken services,
      MappingToken workspace,
      LiteralToken timeoutInMinutes,
      LiteralToken cancelTimeoutInMinutes,
      LiteralToken retryCountOnTaskFailure)
    {
      PhaseTarget phaseTarget = (PhaseTarget) null;
      if (pool == null)
      {
        phaseTarget = (PhaseTarget) new AgentQueueTarget();
      }
      else
      {
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in pool)
        {
          if (string.Equals(TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "pool key").Value, "name", StringComparison.Ordinal))
          {
            LiteralToken literal = TemplateUtil.AssertLiteral(keyValuePair.Value, "pool name");
            if (string.Equals(literal.Value, "server", StringComparison.OrdinalIgnoreCase))
            {
              phaseTarget = (PhaseTarget) new ServerTarget();
              break;
            }
            AgentQueueTarget agentQueueTarget = new AgentQueueTarget();
            AgentQueueReference agentQueueReference = new AgentQueueReference();
            agentQueueReference.Name = TemplateResultConverter.ConvertToPipelineExpressionOfString(context, literal);
            agentQueueTarget.Queue = agentQueueReference;
            phaseTarget = (PhaseTarget) agentQueueTarget;
            break;
          }
        }
        if (phaseTarget == null)
          phaseTarget = (PhaseTarget) new AgentQueueTarget();
      }
      if (phaseTarget is ServerTarget serverTarget)
      {
        serverTarget.Execution = new ParallelExecutionOptions();
        if (strategy != null)
        {
          bool flag = false;
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in strategy)
          {
            LiteralToken literal1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "strategy key");
            switch (literal1.Value)
            {
              case "matrix":
                serverTarget.Execution.Matrix = TemplateResultConverter.ConvertToMatrix(context, keyValuePair.Value);
                continue;
              case "maxParallel":
                LiteralToken literal2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "max parallel");
                serverTarget.Execution.MaxConcurrency = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, literal2);
                flag = true;
                continue;
              case "parallel":
                LiteralToken literal3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "parallel");
                serverTarget.Execution.MaxConcurrency = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, literal3);
                continue;
              default:
                TemplateUtil.AssertUnexpectedValue(literal1, "strategy key");
                continue;
            }
          }
          if (flag && serverTarget.Execution.Matrix == (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) null)
            serverTarget.Execution.MaxConcurrency = (ExpressionValue<int>) null;
        }
        if (timeoutInMinutes != null)
          serverTarget.TimeoutInMinutes = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, timeoutInMinutes);
        if (cancelTimeoutInMinutes != null)
          serverTarget.CancelTimeoutInMinutes = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, cancelTimeoutInMinutes);
      }
      else
      {
        AgentQueueTarget agentQueueTarget1 = phaseTarget as AgentQueueTarget;
        agentQueueTarget1.Execution = new ParallelExecutionOptions();
        agentQueueTarget1.Workspace = new WorkspaceOptions();
        if (strategy != null)
        {
          bool flag = false;
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in strategy)
          {
            LiteralToken literal4 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "strategy key");
            switch (literal4.Value)
            {
              case "matrix":
                agentQueueTarget1.Execution.Matrix = TemplateResultConverter.ConvertToMatrix(context, keyValuePair.Value);
                continue;
              case "maxParallel":
                LiteralToken literal5 = TemplateUtil.AssertLiteral(keyValuePair.Value, "max parallel");
                agentQueueTarget1.Execution.MaxConcurrency = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, literal5);
                flag = true;
                continue;
              case "parallel":
                LiteralToken literal6 = TemplateUtil.AssertLiteral(keyValuePair.Value, "parallel");
                agentQueueTarget1.Execution.MaxConcurrency = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, literal6);
                continue;
              default:
                TemplateUtil.AssertUnexpectedValue(literal4, "strategy key");
                continue;
            }
          }
          if (flag && agentQueueTarget1.Execution.Matrix == (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) null)
            agentQueueTarget1.Execution.MaxConcurrency = (ExpressionValue<int>) null;
        }
        if (pool != null)
        {
          IEnumerable<KeyValuePair<ScalarToken, TemplateToken>> keyValuePairs1 = pool.Where<KeyValuePair<ScalarToken, TemplateToken>>((Func<KeyValuePair<ScalarToken, TemplateToken>, bool>) (poolPair => TemplateUtil.AssertLiteral((TemplateToken) poolPair.Key, "pool key").Value == "demands"));
          IEnumerable<KeyValuePair<ScalarToken, TemplateToken>> keyValuePairs2 = pool.Where<KeyValuePair<ScalarToken, TemplateToken>>((Func<KeyValuePair<ScalarToken, TemplateToken>, bool>) (poolPair =>
          {
            string str = TemplateUtil.AssertLiteral((TemplateToken) poolPair.Key, "pool key").Value;
            return str != "name" && str != "demands";
          }));
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in keyValuePairs1)
          {
            foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(keyValuePair.Value, "demands"))
            {
              LiteralToken literalToken = TemplateUtil.AssertLiteral(templateToken, "demand");
              Demand demand;
              if (!Demand.TryParse(literalToken.Value, out demand))
                context.Error((TemplateToken) literalToken, "Invalid demand '" + literalToken.Value + "'. The demand should be in the format '<NAME>' to test existence, and '<NAME> -equals <VALUE>' to test for a specific value. For example, 'VISUALSTUDIO' or 'agent.os -equals Windows_NT'.");
              else
                agentQueueTarget1.Demands.Add(demand);
            }
          }
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in keyValuePairs2)
          {
            LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "pool key");
            if (string.Equals(literalToken1.Value, "vmImage", StringComparison.OrdinalIgnoreCase) && (agentQueueTarget1.Queue?.Name == (ExpressionValue<string>) null || agentQueueTarget1.Queue.Name.IsLiteral) && string.IsNullOrEmpty(agentQueueTarget1.Queue?.Name?.Literal))
            {
              string str = (string) null;
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "vm image");
              bool flag = AgentQueueTarget.IsProbablyExpressionOrMacro(literalToken2.Value);
              if (!context.RunJobsWithDemandsOnSingleHostedPool && agentQueueTarget1.Demands.Count > 0 && !flag)
                str = AgentQueueTarget.PoolNameForVMImage(literalToken2.Value);
              if (string.IsNullOrEmpty(str))
              {
                if (!flag)
                  str = "Azure Pipelines";
                if (agentQueueTarget1.AgentSpecification == null)
                  agentQueueTarget1.AgentSpecification = new JObject();
                agentQueueTarget1.AgentSpecification.Add("vmImage", TemplateUtil.ConvertToJToken((TemplateToken) literalToken2));
              }
              if (!string.IsNullOrEmpty(str))
              {
                AgentQueueTarget agentQueueTarget2 = agentQueueTarget1;
                AgentQueueReference agentQueueReference = new AgentQueueReference();
                agentQueueReference.Name = (ExpressionValue<string>) str;
                agentQueueTarget2.Queue = agentQueueReference;
              }
            }
            else
            {
              if (agentQueueTarget1.AgentSpecification == null)
                agentQueueTarget1.AgentSpecification = new JObject();
              agentQueueTarget1.AgentSpecification.Add(literalToken1.Value, TemplateUtil.ConvertToJToken(keyValuePair.Value));
            }
          }
        }
        if (timeoutInMinutes != null)
          agentQueueTarget1.TimeoutInMinutes = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, timeoutInMinutes);
        if (cancelTimeoutInMinutes != null)
          agentQueueTarget1.CancelTimeoutInMinutes = TemplateResultConverter.ConvertToPipelineExpressionOfInt32(context, cancelTimeoutInMinutes);
        if (container != null)
        {
          ContainerResource containerResource;
          agentQueueTarget1.Container = TemplateResultConverter.ConvertToContainerReference(context, container, out containerResource);
          if (containerResource != null)
            resources.Containers.Add(containerResource);
        }
        if (services != null)
        {
          foreach (KeyValuePair<ScalarToken, TemplateToken> service in services)
          {
            LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) service.Key, "services key");
            MappingToken containerReference = TemplateUtil.AssertMapping(service.Value, "services value");
            ContainerResource containerResource;
            agentQueueTarget1.SidecarContainers[literalToken.Value] = TemplateResultConverter.ConvertToContainerReference(context, containerReference, out containerResource);
            if (containerResource != null)
              resources.Containers.Add(containerResource);
          }
        }
        if (workspace != null)
        {
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in workspace)
          {
            LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "workspace key");
            if (literal.Value == "clean")
            {
              LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "workspace clean");
              agentQueueTarget1.Workspace.Clean = literalToken.Value;
            }
            else
              TemplateUtil.AssertUnexpectedValue(literal, "workspace key");
          }
        }
      }
      return phaseTarget;
    }

    private static TaskStepDefinitionReference ConvertBuildResourceReferenceToTaskReference(
      TemplateContext context,
      IList<TaskDefinition> tasks,
      PipelineResources resources,
      IArtifactResolver artifactResolver,
      LiteralToken downloadBuildLiteral,
      out Resource referencedResource)
    {
      TaskStepDefinitionReference taskReference = (TaskStepDefinitionReference) null;
      referencedResource = (Resource) null;
      if (artifactResolver == null)
        return (TaskStepDefinitionReference) null;
      object obj;
      Dictionary<Guid, TaskDefinition> dictionary;
      if (context.State.TryGetValue("latesttask", out obj))
      {
        dictionary = obj as Dictionary<Guid, TaskDefinition>;
      }
      else
      {
        dictionary = new Dictionary<Guid, TaskDefinition>();
        if (tasks != null && tasks.Count > 0)
        {
          foreach (TaskDefinition taskDefinition in tasks.GroupBy(x => new
          {
            Id = x.Id,
            Major = x.Version.Major
          }).Select<IGrouping<\u003C\u003Ef__AnonymousType0<Guid, int>, TaskDefinition>, TaskDefinition>(x => x.OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (y => y.Version)).First<TaskDefinition>()))
            dictionary[taskDefinition.Id] = taskDefinition;
        }
        context.State.Add("latesttask", (object) dictionary);
      }
      if (resources != null && resources.Builds != null && resources.Builds.Any<BuildResource>())
      {
        BuildResource buildResource = resources.Builds.FirstOrDefault<BuildResource>((Func<BuildResource, bool>) (x => x.Alias.Equals(downloadBuildLiteral.Value)));
        if (buildResource == null)
        {
          context.Error((TemplateToken) downloadBuildLiteral, YamlStrings.CannotFindBuildResource((object) downloadBuildLiteral.Value));
        }
        else
        {
          referencedResource = (Resource) buildResource;
          Guid artifactDownloadTaskId = artifactResolver.GetArtifactDownloadTaskId((Resource) buildResource);
          if (artifactDownloadTaskId != Guid.Empty)
          {
            TaskDefinition taskDefinition;
            if (dictionary.TryGetValue(artifactDownloadTaskId, out taskDefinition))
              taskReference = new TaskStepDefinitionReference()
              {
                Id = taskDefinition.Id,
                Name = taskDefinition.Name,
                Version = (string) taskDefinition.Version
              };
            else
              context.Error((TemplateToken) downloadBuildLiteral, YamlStrings.CannotFindTaskId((object) artifactDownloadTaskId, (object) downloadBuildLiteral.Value));
          }
          else
            context.Error((TemplateToken) downloadBuildLiteral, YamlStrings.CannotFindTaskIdFromArtifactExtension((object) downloadBuildLiteral.Value));
        }
      }
      else
        context.Error((TemplateToken) downloadBuildLiteral, YamlStrings.CannotFindBuildResource((object) downloadBuildLiteral.Value));
      return taskReference;
    }

    private static TaskStepDefinitionReference ConvertPackageResourceReferenceToTaskReference(
      TemplateContext context,
      IList<TaskDefinition> tasks,
      PipelineResources resources,
      IArtifactResolver artifactResolver,
      LiteralToken downloadPackageLiteral,
      out Resource referencedResource)
    {
      TaskStepDefinitionReference taskReference = (TaskStepDefinitionReference) null;
      referencedResource = (Resource) null;
      if (artifactResolver == null)
        return (TaskStepDefinitionReference) null;
      object obj;
      Dictionary<Guid, TaskDefinition> dictionary;
      if (context.State.TryGetValue("latesttask", out obj))
      {
        dictionary = obj as Dictionary<Guid, TaskDefinition>;
      }
      else
      {
        dictionary = new Dictionary<Guid, TaskDefinition>();
        if (tasks != null && tasks.Count > 0)
        {
          foreach (TaskDefinition taskDefinition in tasks.GroupBy(x => new
          {
            Id = x.Id,
            Major = x.Version.Major
          }).Select<IGrouping<\u003C\u003Ef__AnonymousType0<Guid, int>, TaskDefinition>, TaskDefinition>(x => x.OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (y => y.Version)).First<TaskDefinition>()))
            dictionary[taskDefinition.Id] = taskDefinition;
        }
        context.State.Add("latesttask", (object) dictionary);
      }
      if (resources != null && resources.Packages != null && resources.Packages.Any<PackageResource>())
      {
        PackageResource packageResource = resources.Packages.FirstOrDefault<PackageResource>((Func<PackageResource, bool>) (x => x.Alias.Equals(downloadPackageLiteral.Value)));
        if (packageResource == null)
        {
          context.Error((TemplateToken) downloadPackageLiteral, YamlStrings.CannotFindPackageResource((object) downloadPackageLiteral.Value));
        }
        else
        {
          referencedResource = (Resource) packageResource;
          Guid artifactDownloadTaskId = artifactResolver.GetArtifactDownloadTaskId((Resource) packageResource);
          if (artifactDownloadTaskId != Guid.Empty)
          {
            TaskDefinition taskDefinition;
            if (dictionary.TryGetValue(artifactDownloadTaskId, out taskDefinition))
              taskReference = new TaskStepDefinitionReference()
              {
                Id = taskDefinition.Id,
                Name = taskDefinition.Name,
                Version = (string) taskDefinition.Version
              };
            else
              context.Error((TemplateToken) downloadPackageLiteral, YamlStrings.CannotFindTaskIdForPackageResources((object) artifactDownloadTaskId, (object) downloadPackageLiteral.Value));
          }
          else
            context.Error((TemplateToken) downloadPackageLiteral, YamlStrings.CannotFindTaskIdFromPackageArtifactExtension((object) downloadPackageLiteral.Value));
        }
      }
      else
        context.Error((TemplateToken) downloadPackageLiteral, YamlStrings.CannotFindPackageResource((object) downloadPackageLiteral.Value));
      return taskReference;
    }

    private static bool ParseContainerResourceProperty(
      TemplateContext context,
      KeyValuePair<ScalarToken, TemplateToken> containerPair,
      bool errorOnUnexpectedKey,
      ref ContainerResource containerResource)
    {
      LiteralToken literal1 = TemplateUtil.AssertLiteral((TemplateToken) containerPair.Key, "container key");
      string str = literal1.Value;
      if (str != null)
      {
        switch (str.Length)
        {
          case 3:
            if (str == "env")
            {
              MappingToken mappingToken = TemplateUtil.AssertMapping(containerPair.Value, "container environment");
              Dictionary<string, string> dictionary = new Dictionary<string, string>();
              foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
              {
                LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "container environment key");
                LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "container environment value");
                dictionary[literalToken1.Value] = literalToken2.Value;
              }
              containerResource.Environment = (IDictionary<string, string>) dictionary;
              break;
            }
            goto label_49;
          case 5:
            switch (str[0])
            {
              case 'i':
                if (str == "image")
                {
                  LiteralToken literalToken = TemplateUtil.AssertLiteral(containerPair.Value, "container image");
                  containerResource.Image = literalToken.Value;
                  break;
                }
                goto label_49;
              case 'p':
                if (str == "ports")
                {
                  SequenceToken sequenceToken = TemplateUtil.AssertSequence(containerPair.Value, "container ports");
                  List<string> stringList = new List<string>();
                  foreach (TemplateToken templateToken in sequenceToken)
                  {
                    LiteralToken literalToken = TemplateUtil.AssertLiteral(templateToken, "container port");
                    stringList.Add(literalToken.Value);
                  }
                  containerResource.Ports = (IList<string>) stringList;
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 7:
            switch (str[0])
            {
              case 'o':
                if (str == "options")
                {
                  LiteralToken literalToken = TemplateUtil.AssertLiteral(containerPair.Value, "container options");
                  containerResource.Options = literalToken.Value;
                  break;
                }
                goto label_49;
              case 'v':
                if (str == "volumes")
                {
                  SequenceToken sequenceToken = TemplateUtil.AssertSequence(containerPair.Value, "container volumes");
                  List<string> stringList = new List<string>();
                  foreach (TemplateToken templateToken in sequenceToken)
                  {
                    LiteralToken literalToken = TemplateUtil.AssertLiteral(templateToken, "container volume");
                    stringList.Add(literalToken.Value);
                  }
                  containerResource.Volumes = (IList<string>) stringList;
                  break;
                }
                goto label_49;
              default:
                goto label_49;
            }
            break;
          case 8:
            if (str == "endpoint")
            {
              LiteralToken literalToken = TemplateUtil.AssertLiteral(containerPair.Value, "container endpoint");
              ContainerResource containerResource1 = containerResource;
              ServiceEndpointReference endpointReference = new ServiceEndpointReference();
              endpointReference.Name = (ExpressionValue<string>) literalToken.Value;
              containerResource1.Endpoint = endpointReference;
              break;
            }
            goto label_49;
          case 13:
            if (str == "mountReadOnly")
            {
              MappingToken mappingToken = TemplateUtil.AssertMapping(containerPair.Value, "container volume readonly ");
              List<string> stringList = new List<string>();
              foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
              {
                LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "container volume readonly key");
                LiteralToken literal2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "container volume readonly value");
                if (TemplateResultConverter.ConvertToBoolean(context, literal2))
                  stringList.Add(literalToken.Value);
              }
              containerResource.ReadOnlyMounts = (IList<string>) stringList;
              break;
            }
            goto label_49;
          case 15:
            if (str == "mapDockerSocket")
            {
              LiteralToken literal3 = TemplateUtil.AssertLiteral(containerPair.Value, "container mapDockerSocket");
              containerResource.MapDockerSocket = TemplateResultConverter.ConvertToBoolean(context, literal3);
              break;
            }
            goto label_49;
          default:
            goto label_49;
        }
        return true;
      }
label_49:
      if (errorOnUnexpectedKey)
        TemplateUtil.AssertUnexpectedValue(literal1, "container key");
      return false;
    }

    private static ExpressionValue<string> ConvertToContainerReference(
      TemplateContext context,
      MappingToken containerReference,
      out ContainerResource containerResource)
    {
      if (containerReference.Count == 1)
      {
        LiteralToken literalToken = TemplateUtil.AssertLiteral((TemplateToken) containerReference[0].Key, "container key");
        if (literalToken != null && string.Equals(literalToken.Value, "alias", StringComparison.Ordinal))
        {
          LiteralToken literal = TemplateUtil.AssertLiteral(containerReference[0].Value, "container alias");
          containerResource = (ContainerResource) null;
          return TemplateResultConverter.ConvertToPipelineExpressionOfString(context, literal);
        }
      }
      ref ContainerResource local = ref containerResource;
      ContainerResource containerResource1 = new ContainerResource();
      containerResource1.Alias = Guid.NewGuid().ToString("N");
      local = containerResource1;
      foreach (KeyValuePair<ScalarToken, TemplateToken> containerPair in containerReference)
      {
        TemplateUtil.AssertLiteral((TemplateToken) containerPair.Key, "container key");
        TemplateResultConverter.ParseContainerResourceProperty(context, containerPair, true, ref containerResource);
      }
      return (ExpressionValue<string>) containerResource?.Alias;
    }

    private static ExpressionValue<IDictionary<string, IDictionary<string, string>>> ConvertToMatrix(
      TemplateContext context,
      TemplateToken matrix)
    {
      if (matrix is LiteralToken literalToken1)
      {
        Exception ex;
        if (TemplateResultConverter.TryParsePipelineExpression(literalToken1.Value, out ex))
          return ExpressionValue.FromExpression<IDictionary<string, IDictionary<string, string>>>(literalToken1.Value);
        if (ex != null)
        {
          context.Error((TemplateToken) literalToken1, ex);
          return (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) null;
        }
      }
      Dictionary<string, IDictionary<string, string>> matrix1 = new Dictionary<string, IDictionary<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair1 in TemplateUtil.AssertMapping(matrix, nameof (matrix)))
      {
        LiteralToken literalToken2 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair1.Key, "matrix key");
        MappingToken mappingToken = TemplateUtil.AssertMapping(keyValuePair1.Value, "matrix variables");
        Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        matrix1[literalToken2.Value] = (IDictionary<string, string>) dictionary;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair2 in mappingToken)
        {
          LiteralToken literalToken3 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair2.Key, "matrix variables key");
          LiteralToken literalToken4 = TemplateUtil.AssertLiteral(keyValuePair2.Value, "matrix variables value");
          dictionary[literalToken3.Value] = literalToken4.Value;
        }
      }
      return (ExpressionValue<IDictionary<string, IDictionary<string, string>>>) (IDictionary<string, IDictionary<string, string>>) matrix1;
    }

    private static StepTarget ConvertToStepTarget(TemplateContext context, TemplateToken target)
    {
      StepTarget stepTarget = new StepTarget();
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(target, nameof (target)))
      {
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "target key");
        switch (literal.Value)
        {
          case "container":
            LiteralToken literalToken1 = TemplateUtil.AssertLiteral(keyValuePair.Value, "container");
            stepTarget.Target = literalToken1.Value;
            continue;
          case "commands":
            LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "commands");
            stepTarget.Commands = literalToken2.Value;
            continue;
          case "settableVariables":
            SequenceToken stringsToken = TemplateUtil.AssertSequence(keyValuePair.Value, "settableVariables");
            List<string> listOfString = TemplateResultConverter.ConvertToListOfString(context, (TemplateToken) stringsToken);
            stepTarget.SettableVariables = new TaskVariableRestrictions();
            stepTarget.SettableVariables.Allowed.AddRange<string, IList<string>>((IEnumerable<string>) listOfString);
            continue;
          default:
            TemplateUtil.AssertUnexpectedValue(literal, "target key");
            continue;
        }
      }
      return stepTarget;
    }

    private static IEnumerable<Step> ConvertToSteps(
      TemplateContext context,
      IList<TaskDefinition> tasks,
      PipelineResources resources,
      IArtifactResolver artifactResolver,
      TemplateToken steps)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(steps, nameof (steps)))
      {
        TaskStep taskStep = new TaskStep();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "step");
        bool flag = false;
        Resource referencedResource = (Resource) null;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken literal1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "step key");
          string str = literal1.Value;
          if (str != null)
          {
            switch (str.Length)
            {
              case 3:
                if (str == "env")
                {
                  using (IEnumerator<KeyValuePair<ScalarToken, TemplateToken>> enumerator = TemplateUtil.AssertMapping(keyValuePair.Value, "steps environment").GetEnumerator())
                  {
                    while (enumerator.MoveNext())
                    {
                      KeyValuePair<ScalarToken, TemplateToken> current = enumerator.Current;
                      LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) current.Key, "steps environment key");
                      LiteralToken literalToken2 = TemplateUtil.AssertLiteral(current.Value, "steps environment value");
                      taskStep.Environment[literalToken1.Value] = literalToken2.Value;
                    }
                    continue;
                  }
                }
                else
                  break;
              case 4:
                switch (str[0])
                {
                  case 'n':
                    if (str == "name")
                    {
                      LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "step name");
                      taskStep.Name = literalToken.Value;
                      continue;
                    }
                    break;
                  case 't':
                    if (str == "task")
                    {
                      LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "task reference");
                      string name;
                      string version;
                      if (!TemplateResultConverter.TryParseTaskReference(literalToken.Value, out name, out version))
                      {
                        context.Error((TemplateToken) literalToken, YamlStrings.InvalidTaskReference((object) literalToken.Value));
                        continue;
                      }
                      taskStep.Reference = new TaskStepDefinitionReference()
                      {
                        Name = name,
                        Version = version
                      };
                      continue;
                    }
                    break;
                }
                break;
              case 6:
                switch (str[0])
                {
                  case 'i':
                    if (str == "inputs")
                    {
                      using (IEnumerator<KeyValuePair<ScalarToken, TemplateToken>> enumerator = TemplateUtil.AssertMapping(keyValuePair.Value, "steps input").GetEnumerator())
                      {
                        while (enumerator.MoveNext())
                        {
                          KeyValuePair<ScalarToken, TemplateToken> current = enumerator.Current;
                          LiteralToken literalToken3 = TemplateUtil.AssertLiteral((TemplateToken) current.Key, "steps input key");
                          LiteralToken literalToken4 = TemplateUtil.AssertLiteral(current.Value, "steps input value");
                          taskStep.Inputs[literalToken3.Value] = literalToken4.Value;
                        }
                        continue;
                      }
                    }
                    else
                      break;
                  case 't':
                    if (str == "target")
                    {
                      MappingToken target = TemplateUtil.AssertMapping(keyValuePair.Value, "step target");
                      taskStep.Target = TemplateResultConverter.ConvertToStepTarget(context, (TemplateToken) target);
                      continue;
                    }
                    break;
                }
                break;
              case 7:
                if (str == "enabled")
                {
                  LiteralToken literal2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "step enabled");
                  taskStep.Enabled = TemplateResultConverter.ConvertToBoolean(context, literal2);
                  continue;
                }
                break;
              case 9:
                if (str == "condition")
                {
                  LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "step condition");
                  taskStep.Condition = literalToken.Value;
                  continue;
                }
                break;
              case 10:
                if (str == "getPackage")
                {
                  LiteralToken downloadPackageLiteral = TemplateUtil.AssertLiteral(keyValuePair.Value, "get package step");
                  taskStep.Reference = TemplateResultConverter.ConvertPackageResourceReferenceToTaskReference(context, tasks, resources, artifactResolver, downloadPackageLiteral, out referencedResource);
                  flag = true;
                  continue;
                }
                break;
              case 11:
                if (str == "displayName")
                {
                  LiteralToken literalToken = TemplateUtil.AssertLiteral(keyValuePair.Value, "step display name");
                  taskStep.DisplayName = literalToken.Value;
                  continue;
                }
                break;
              case 13:
                if (str == "downloadBuild")
                {
                  LiteralToken downloadBuildLiteral = TemplateUtil.AssertLiteral(keyValuePair.Value, "download build step");
                  taskStep.Reference = TemplateResultConverter.ConvertBuildResourceReferenceToTaskReference(context, tasks, resources, artifactResolver, downloadBuildLiteral, out referencedResource);
                  flag = true;
                  continue;
                }
                break;
              case 15:
                if (str == "continueOnError")
                {
                  LiteralToken literal3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "step continue on error");
                  taskStep.ContinueOnError = TemplateResultConverter.ConvertToBoolean(context, literal3);
                  continue;
                }
                break;
              case 16:
                if (str == "timeoutInMinutes")
                {
                  LiteralToken literal4 = TemplateUtil.AssertLiteral(keyValuePair.Value, "step timeout in minutes");
                  taskStep.TimeoutInMinutes = TemplateResultConverter.ConvertToInt32(context, literal4);
                  continue;
                }
                break;
              case 23:
                if (str == "retryCountOnTaskFailure")
                {
                  LiteralToken literal5 = TemplateUtil.AssertLiteral(keyValuePair.Value, "step retry in times");
                  taskStep.RetryCountOnTaskFailure = TemplateResultConverter.ConvertToInt32(context, literal5);
                  continue;
                }
                break;
            }
          }
          TemplateUtil.AssertUnexpectedValue(literal1, "step key");
        }
        if (string.IsNullOrEmpty(taskStep.DisplayName))
          taskStep.DisplayName = taskStep.Name;
        if (flag && referencedResource != null && artifactResolver != null)
          artifactResolver.PopulateMappedTaskInputs(referencedResource, taskStep);
        yield return (Step) taskStep;
      }
    }

    private static bool ConvertToBoolean(TemplateContext context, LiteralToken literal)
    {
      bool result;
      if (TemplateResultConverter.TryParseBoolean(literal.Value, out result))
        return result;
      context.Error((TemplateToken) literal, YamlStrings.ExpectedBoolean((object) literal.Value));
      return false;
    }

    private static int ConvertToInt32(TemplateContext context, LiteralToken literal)
    {
      int result;
      if (TemplateResultConverter.TryParseInt32(literal.Value, out result))
        return result;
      context.Error((TemplateToken) literal, YamlStrings.ExpectedInteger((object) literal.Value));
      return 0;
    }

    private static ExpressionValue<bool> ConvertToPipelineExpressionOfBoolean(
      TemplateContext context,
      LiteralToken literal)
    {
      ExpressionValue<bool> result;
      Exception ex;
      if (TemplateResultConverter.TryParsePipelineExpressionOfBoolean(literal.Value, out result, out ex))
        return result;
      if (ex != null)
        context.Error((TemplateToken) literal, ex);
      else
        context.Error((TemplateToken) literal, YamlStrings.ExpectedBoolean((object) literal.Value));
      return (ExpressionValue<bool>) null;
    }

    private static ExpressionValue<int> ConvertToPipelineExpressionOfInt32(
      TemplateContext context,
      LiteralToken literal)
    {
      ExpressionValue<int> result;
      Exception ex;
      if (TemplateResultConverter.TryParsePipelineExpressionOfInt32(literal.Value, out result, out ex))
        return result;
      if (ex != null)
        context.Error((TemplateToken) literal, ex);
      else
        context.Error((TemplateToken) literal, YamlStrings.ExpectedInteger((object) literal.Value));
      return (ExpressionValue<int>) null;
    }

    private static ExpressionValue<string> ConvertToPipelineExpressionOfString(
      TemplateContext context,
      LiteralToken literal)
    {
      ExpressionValue<string> result;
      Exception ex;
      if (TemplateResultConverter.TryParsePipelineExpressionOfString(literal.Value, out result, out ex))
        return result;
      context.Error((TemplateToken) literal, ex);
      return (ExpressionValue<string>) null;
    }

    private static ResourceReferences ConvertToReferencedResources(
      TemplateContext context,
      TemplateToken token)
    {
      MappingToken mappingToken = TemplateUtil.AssertMapping(token, "job resources");
      ResourceReferences referencedResources = new ResourceReferences();
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
      {
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "job resource key");
        switch (literal.Value)
        {
          case "repositories":
            using (IEnumerator<TemplateToken> enumerator = TemplateUtil.AssertSequence(keyValuePair.Value, "job repository resources").GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                LiteralToken literalToken = TemplateUtil.AssertLiteral(enumerator.Current, "job repository resource");
                referencedResources.Repositories.Add(literalToken.Value);
              }
              continue;
            }
          case "pools":
            using (IEnumerator<TemplateToken> enumerator = TemplateUtil.AssertSequence(keyValuePair.Value, "job pool resources").GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                LiteralToken literalToken = TemplateUtil.AssertLiteral(enumerator.Current, "job pool resource");
                referencedResources.Queues.Add(literalToken.Value);
              }
              continue;
            }
          default:
            TemplateUtil.AssertUnexpectedValue(literal, "job resource key");
            continue;
        }
      }
      return referencedResources;
    }

    private static bool TryParseBoolean(string value, out bool result)
    {
      if (!string.IsNullOrEmpty(value))
      {
        if (string.Equals(value, "TRUE", StringComparison.OrdinalIgnoreCase))
        {
          result = true;
          return true;
        }
        if (string.Equals(value, "FALSE", StringComparison.OrdinalIgnoreCase))
        {
          result = false;
          return true;
        }
      }
      result = false;
      return false;
    }

    private static bool TryParseInt32(string value, out int result)
    {
      if (!string.IsNullOrEmpty(value) && int.TryParse(value, NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        return true;
      result = 0;
      return false;
    }

    private static bool TryParsePipelineExpressionOfBoolean(
      string value,
      out ExpressionValue<bool> result,
      out Exception ex)
    {
      bool result1;
      if (TemplateResultConverter.TryParseBoolean(value, out result1))
      {
        result = (ExpressionValue<bool>) result1;
        ex = (Exception) null;
        return true;
      }
      if (TemplateResultConverter.TryParsePipelineExpression(value, out ex))
      {
        result = ExpressionValue.FromExpression<bool>(value);
        return true;
      }
      result = (ExpressionValue<bool>) null;
      return false;
    }

    private static bool TryParsePipelineExpressionOfInt32(
      string value,
      out ExpressionValue<int> result,
      out Exception ex)
    {
      int result1;
      if (TemplateResultConverter.TryParseInt32(value, out result1))
      {
        result = (ExpressionValue<int>) result1;
        ex = (Exception) null;
        return true;
      }
      if (TemplateResultConverter.TryParsePipelineExpression(value, out ex))
      {
        result = ExpressionValue.FromExpression<int>(value);
        return true;
      }
      result = (ExpressionValue<int>) null;
      return false;
    }

    private static bool TryParsePipelineExpressionOfString(
      string value,
      out ExpressionValue<string> result,
      out Exception ex)
    {
      if (TemplateResultConverter.TryParsePipelineExpression(value, out ex))
      {
        result = ExpressionValue.FromExpression<string>(value);
        return true;
      }
      if (ex == null)
      {
        result = new ExpressionValue<string>(value);
        return true;
      }
      result = (ExpressionValue<string>) null;
      return false;
    }

    private static bool TryParsePipelineExpression(string value, out Exception ex)
    {
      if (ExpressionValue.IsExpression(value))
      {
        try
        {
          new ExpressionParser().ValidateSyntax(ExpressionValue.TrimExpression(value), (Microsoft.TeamFoundation.DistributedTask.Expressions.ITraceWriter) null);
          ex = (Exception) null;
          return true;
        }
        catch (Exception ex1)
        {
          ex = ex1;
          return false;
        }
      }
      else
      {
        ex = (Exception) null;
        return false;
      }
    }

    private static bool TryParseTaskReference(string value, out string name, out string version)
    {
      bool taskReference;
      if (!string.IsNullOrEmpty(value))
      {
        string[] strArray = value.Split('@');
        if (strArray.Length == 2 && !string.IsNullOrEmpty(strArray[0]) && !string.IsNullOrEmpty(strArray[1]) && TemplateResultConverter.ValidateTaskVersion(strArray[1]))
        {
          taskReference = true;
          name = strArray[0];
          version = strArray[1];
        }
        else
        {
          taskReference = false;
          name = (string) null;
          version = (string) null;
        }
      }
      else
      {
        taskReference = false;
        name = (string) null;
        version = (string) null;
      }
      return taskReference;
    }

    private static bool ValidateTaskVersion(string taskVersion)
    {
      if (int.TryParse(taskVersion, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out int _))
        return true;
      string[] strArray = taskVersion.Split('.');
      if (strArray.Length != 3)
        return false;
      foreach (string s in strArray)
      {
        if (!int.TryParse(s, NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out int _))
          return false;
      }
      return true;
    }

    private static bool ValidatePipelineSchedule(PipelineSchedule schedule, out string errorMessage)
    {
      if (!string.Equals(schedule.ScheduleType, PipelineConstants.ScheduleType.Cron, StringComparison.OrdinalIgnoreCase))
        throw new NotSupportedException("Unexpected schedule type '" + schedule.ScheduleType + "'");
      if (schedule.ScheduleDetails != null)
      {
        if (CrontabSchedule.TryParse(schedule.ScheduleDetails, new CrontabSchedule.ParseOptions()
        {
          IncludingSeconds = false
        }) != null)
        {
          if (schedule.BranchFilters == null || schedule.BranchFilters.Count < 1)
          {
            errorMessage = YamlStrings.InvalidPipelineScheduleBranchFilters();
            return false;
          }
          errorMessage = string.Empty;
          return true;
        }
      }
      errorMessage = YamlStrings.CronSyntaxError((object) schedule.ScheduleDetails);
      return false;
    }

    private static IEnumerable<RepositoryResource> ConvertToRepositoryResources(
      TemplateContext context,
      TemplateToken repositories,
      bool acceptsExpression)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(repositories, nameof (repositories)))
      {
        RepositoryResource repositoryResource1 = new RepositoryResource();
        MappingToken mappingToken = TemplateUtil.AssertMapping(templateToken, "repository resource");
        bool flag = true;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in mappingToken)
        {
          LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "repository resource key");
          switch (literalToken1.Value)
          {
            case "repository":
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository resource alias");
              repositoryResource1.Alias = literalToken2.Value;
              continue;
            case "endpoint":
              LiteralToken literalToken3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository resource endpoint");
              RepositoryResource repositoryResource2 = repositoryResource1;
              ServiceEndpointReference endpointReference = new ServiceEndpointReference();
              endpointReference.Name = (ExpressionValue<string>) literalToken3.Value;
              repositoryResource2.Endpoint = endpointReference;
              continue;
            case "trigger":
              ContinuousIntegrationTrigger integrationTrigger = TemplateResultConverter.ConvertToContinuousIntegrationTrigger(context, keyValuePair.Value);
              repositoryResource1.Trigger = integrationTrigger;
              continue;
            case "pr":
              PullRequestTrigger pullRequestTrigger = TemplateResultConverter.ConvertToPullRequestTrigger(context, keyValuePair.Value);
              repositoryResource1.PR = pullRequestTrigger;
              continue;
            case "name":
              LiteralToken literalToken4 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository resource name");
              repositoryResource1.Name = literalToken4.Value;
              continue;
            case "ref":
              if (!acceptsExpression && keyValuePair.Value is ExpressionToken)
              {
                flag = false;
                continue;
              }
              LiteralToken literalToken5 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository ref name");
              repositoryResource1.Ref = literalToken5.Value;
              continue;
            default:
              repositoryResource1.Properties.Set<JToken>(literalToken1.Value, TemplateUtil.ConvertToJToken(keyValuePair.Value));
              continue;
          }
        }
        TemplateResultConverter.ValidateRepositoryResourceTemplate(context, repositoryResource1);
        if (acceptsExpression | flag)
          yield return repositoryResource1;
      }
    }

    private static IEnumerable<RepositoryResource> ConvertToRepositoryResources(
      TemplateContext context,
      TemplateToken repositories)
    {
      foreach (TemplateToken templateToken in TemplateUtil.AssertSequence(repositories, nameof (repositories)))
      {
        RepositoryResource repositoryResource1 = new RepositoryResource();
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in TemplateUtil.AssertMapping(templateToken, "repository resource"))
        {
          LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "repository resource key");
          switch (literalToken1.Value)
          {
            case "repository":
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository resource alias");
              repositoryResource1.Alias = literalToken2.Value;
              continue;
            case "endpoint":
              LiteralToken literalToken3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository resource endpoint");
              RepositoryResource repositoryResource2 = repositoryResource1;
              ServiceEndpointReference endpointReference = new ServiceEndpointReference();
              endpointReference.Name = (ExpressionValue<string>) literalToken3.Value;
              repositoryResource2.Endpoint = endpointReference;
              continue;
            case "trigger":
              ContinuousIntegrationTrigger integrationTrigger = TemplateResultConverter.ConvertToContinuousIntegrationTrigger(context, keyValuePair.Value);
              repositoryResource1.Trigger = integrationTrigger;
              continue;
            case "pr":
              PullRequestTrigger pullRequestTrigger = TemplateResultConverter.ConvertToPullRequestTrigger(context, keyValuePair.Value);
              repositoryResource1.PR = pullRequestTrigger;
              continue;
            case "name":
              LiteralToken literalToken4 = TemplateUtil.AssertLiteral(keyValuePair.Value, "repository resource name");
              repositoryResource1.Name = literalToken4.Value;
              continue;
            default:
              repositoryResource1.Properties.Set<JToken>(literalToken1.Value, TemplateUtil.ConvertToJToken(keyValuePair.Value));
              continue;
          }
        }
        TemplateResultConverter.ValidateRepositoryResourceTemplate(context, repositoryResource1);
        yield return repositoryResource1;
      }
    }
  }
}
