// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseDefinitionConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseDefinitionConverter
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition FromWebApi(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition)
    {
      return ReleaseDefinitionConverter.FromWebApi(context, webApiDefinition, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, ArtifactSource>) (artifact => artifact.FromWebApi(context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion"))));
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition ToWebApi(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition)
    {
      return ReleaseDefinitionConverter.ToWebApi(context, projectId, serverDefinition, (Func<ArtifactSource, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) (artifact => artifact.ToWebApi(context.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion"))));
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "By Design")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition FromWebApi(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApiDefinition,
      Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, ArtifactSource> artifactConverter)
    {
      if (webApiDefinition == null)
        throw new ArgumentNullException(nameof (webApiDefinition));
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition()
      {
        Id = webApiDefinition.Id,
        Revision = webApiDefinition.Revision,
        Name = webApiDefinition.Name,
        Description = webApiDefinition.Description,
        ReleaseNameFormat = string.IsNullOrWhiteSpace(webApiDefinition.ReleaseNameFormat) ? string.Empty : webApiDefinition.ReleaseNameFormat,
        Source = webApiDefinition.Source == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinitionSource.Undefined || !Enum.IsDefined(typeof (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinitionSource), (object) webApiDefinition.Source) ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionSource.RestApi : (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseDefinitionSource) webApiDefinition.Source,
        Path = webApiDefinition.Path,
        IsDeleted = webApiDefinition.IsDeleted,
        Comment = webApiDefinition.Comment ?? string.Empty,
        IsDisabled = webApiDefinition.IsDisabled
      };
      if (!webApiDefinition.VariableGroups.IsNullOrEmpty<int>())
        releaseDefinition.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) webApiDefinition.VariableGroups);
      if (!webApiDefinition.Tags.IsNullOrEmpty<string>())
        releaseDefinition.Tags.AddRange<string, IList<string>>((IEnumerable<string>) webApiDefinition.Tags);
      foreach (DefinitionEnvironment definitionEnvironment in webApiDefinition.Environments.Select<ReleaseDefinitionEnvironment, DefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, DefinitionEnvironment>) (environment => ReleaseDefinitionEnvironmentConverter.ConvertToServerEnvironment(context, environment))))
        releaseDefinition.Environments.Add(definitionEnvironment);
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) webApiDefinition.Variables)
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue configurationVariableValue = variable.Value.ToServerConfigurationVariableValue();
        releaseDefinition.Variables[variable.Key.Trim()] = configurationVariableValue;
      }
      foreach (ArtifactSource artifactSource in webApiDefinition.Artifacts.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact, ArtifactSource>(artifactConverter))
        releaseDefinition.LinkedArtifacts.Add(artifactSource);
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger> artifactSourceTriggers = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger> containerImageTriggerTriggers = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger> packageTriggers = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger> pullRequestTriggers = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger>();
      List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger> sourceRepoTriggers = new List<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger>();
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase trigger in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase>) webApiDefinition.Triggers)
      {
        switch (trigger.TriggerType)
        {
          case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerType.ArtifactSource:
            artifactSourceTriggers.Add(trigger.ToArtifactSourceTrigger().FromWebApi());
            continue;
          case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerType.Schedule:
            releaseDefinition.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase) trigger.ToScheduledReleaseTrigger().FromWebApi());
            continue;
          case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerType.SourceRepo:
            sourceRepoTriggers.Add(trigger.ToSourceRepoTrigger().FromWebApi());
            continue;
          case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerType.ContainerImage:
            containerImageTriggerTriggers.Add(trigger.ToContainerImageTrigger().FromWebApi());
            continue;
          case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerType.Package:
            packageTriggers.Add(trigger.ToPackageTrigger().FromWebApi());
            continue;
          case Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerType.PullRequest:
            pullRequestTriggers.Add(trigger.ToPullRequestTrigger().FromWebApi());
            continue;
          default:
            throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidTriggerType, (object) trigger.TriggerType.ToString()));
        }
      }
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger artifactSourceTrigger in artifactSourceTriggers.GetConsolidatedArtifactSourceTriggers())
        releaseDefinition.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase) artifactSourceTrigger);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger containerImageTrigger in containerImageTriggerTriggers.GetConsolidatedContainerImageTriggers())
        releaseDefinition.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase) containerImageTrigger);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger consolidatedPackageTrigger in packageTriggers.GetConsolidatedPackageTriggers())
        releaseDefinition.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase) consolidatedPackageTrigger);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger pullRequestTrigger in pullRequestTriggers.GetConsolidatedPullRequestTriggers())
        releaseDefinition.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase) pullRequestTrigger);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger sourceRepoTrigger in sourceRepoTriggers.GetConsolidatedSourceRepoTriggers())
        releaseDefinition.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseTriggerBase) sourceRepoTrigger);
      if (webApiDefinition.Properties != null && webApiDefinition.Properties.Any<KeyValuePair<string, object>>())
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) webApiDefinition.Properties)
          releaseDefinition.Properties.Add(new PropertyValue(property.Key, property.Value));
      }
      if (webApiDefinition.PipelineProcess != null)
      {
        releaseDefinition.PipelineProcessType = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PipelineProcessTypes) webApiDefinition.PipelineProcess.Type;
        releaseDefinition.PipelineProcess = ReleasePipelineConverter.FromWebApiPipeline(webApiDefinition.PipelineProcess);
      }
      else
      {
        releaseDefinition.PipelineProcess = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PipelineProcess) new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DesignerPipelineProcess();
        releaseDefinition.PipelineProcessType = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PipelineProcessTypes.Designer;
      }
      return releaseDefinition;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class has many dependencies on other classes.")]
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition ToWebApi(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition,
      Func<ArtifactSource, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifactConverter)
    {
      if (serverDefinition == null)
        throw new ArgumentNullException(nameof (serverDefinition));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(context, "Service", "ReleaseDefintionConverter.ToWebApi", 1971060))
      {
        if (serverDefinition.Environments.All<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env.Conditions == null)))
          serverDefinition.PopulateEnvironmentConditions();
        List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact> artifactList = new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>();
        if (serverDefinition.LinkedArtifacts != null)
          artifactList = serverDefinition.LinkedArtifacts.Select<ArtifactSource, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>(artifactConverter).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>();
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition();
        releaseDefinition.Source = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinitionSource) serverDefinition.Source;
        releaseDefinition.Id = serverDefinition.Id;
        releaseDefinition.Revision = serverDefinition.Revision;
        releaseDefinition.Name = serverDefinition.Name;
        releaseDefinition.Description = serverDefinition.Description;
        releaseDefinition.CreatedBy = new IdentityRef()
        {
          Id = serverDefinition.CreatedBy.ToString()
        };
        DateTime? nullable = serverDefinition.CreatedOn;
        releaseDefinition.CreatedOn = nullable.GetValueOrDefault();
        releaseDefinition.ModifiedBy = new IdentityRef()
        {
          Id = serverDefinition.ModifiedBy.ToString()
        };
        nullable = serverDefinition.ModifiedOn;
        releaseDefinition.ModifiedOn = nullable.GetValueOrDefault();
        releaseDefinition.IsDeleted = serverDefinition.IsDeleted;
        releaseDefinition.IsDisabled = serverDefinition.IsDisabled;
        releaseDefinition.Environments = (IList<ReleaseDefinitionEnvironment>) serverDefinition.Environments.Select<DefinitionEnvironment, ReleaseDefinitionEnvironment>((Func<DefinitionEnvironment, ReleaseDefinitionEnvironment>) (env => ReleaseDefinitionEnvironmentConverter.ConvertToWebApiEnvironment(context, projectId, env))).ToList<ReleaseDefinitionEnvironment>();
        releaseDefinition.Artifacts = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact>) artifactList;
        releaseDefinition.ReleaseNameFormat = serverDefinition.ReleaseNameFormat;
        releaseDefinition.LastRelease = ReleaseDefinitionConverter.GetLastRelease(serverDefinition);
        releaseDefinition.RetentionPolicy = RetentionPolicyConverter.ToWebApiRetentionPolicy(serverDefinition);
        releaseDefinition.Path = serverDefinition.Path;
        releaseDefinition.Comment = serverDefinition.Comment;
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition webApi = releaseDefinition;
        webApi.VariableGroups.AddRange<int, IList<int>>((IEnumerable<int>) serverDefinition.VariableGroups);
        webApi.Tags.AddRange<string, IList<string>>((IEnumerable<string>) serverDefinition.Tags);
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) serverDefinition.Variables)
        {
          Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue configurationVariableValue = variable.Value.ToWebApiConfigurationVariableValue();
          webApi.Variables[variable.Key] = configurationVariableValue;
        }
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ArtifactSourceTrigger artifactSourceTrigger in serverDefinition.ArtifactSourceTriggers)
          webApi.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase) artifactSourceTrigger.ToWebApi());
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ScheduledReleaseTrigger scheduledTrigger in serverDefinition.ScheduledTriggers)
          webApi.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase) scheduledTrigger.ToWebApi());
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.SourceRepoTrigger sourceRepoTrigger in serverDefinition.SourceRepoTriggers)
          webApi.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase) sourceRepoTrigger.ToWebApi());
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ContainerImageTrigger containerImageTrigger in serverDefinition.ContainerImageTriggers)
          webApi.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase) containerImageTrigger.ToWebApi());
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PackageTrigger packageTrigger in serverDefinition.PackageTriggers)
          webApi.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase) packageTrigger.ToWebApi());
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestTrigger pullRequestTrigger in serverDefinition.PullRequestTriggers)
          webApi.Triggers.Add((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseTriggerBase) pullRequestTrigger.ToWebApi());
        if (serverDefinition.Properties != null && serverDefinition.Properties.Any<PropertyValue>())
        {
          foreach (PropertyValue property in (IEnumerable<PropertyValue>) serverDefinition.Properties)
            webApi.Properties.TryAdd<string, object>(property.PropertyName, property.Value);
        }
        webApi.PipelineProcess = ReleasePipelineConverter.ToWebApiPipeline(serverDefinition.PipelineProcess);
        releaseManagementTimer.RecordLap("Service", "ReleaseDefintionConverter.ToWebApi.CreateUrlBegin", 1971060);
        string definitionRestUrl = WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(context, projectId, webApi.Id);
        string definitionWebAccessUri = WebAccessUrlBuilder.GetReleaseDefinitionWebAccessUri(context, projectId.ToString(), webApi.Id);
        webApi.Url = definitionRestUrl;
        webApi.Links.AddLink("self", definitionRestUrl);
        webApi.Links.AddLink("web", definitionWebAccessUri);
        return webApi;
      }
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReference GetLastRelease(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition serverDefinition)
    {
      if (serverDefinition.LastRelease == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReference) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseReference()
      {
        Id = serverDefinition.LastRelease.Id,
        ReleaseDefinitionReference = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
        {
          Id = serverDefinition.LastRelease.ReleaseDefinitionId
        },
        CreatedBy = new IdentityRef()
        {
          Id = serverDefinition.LastRelease.CreatedBy.ToString()
        },
        CreatedOn = serverDefinition.LastRelease.CreatedOn,
        Description = serverDefinition.LastRelease.Description,
        Name = serverDefinition.LastRelease.Name
      };
    }
  }
}
