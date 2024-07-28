// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactTypeBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  [InheritedExport]
  public abstract class ArtifactTypeBase : IArtifactType
  {
    public const string TokenKeyBuildNumber = "build.buildNumber";
    public const string TokenKeyBuildDefinitionName = "build.definitionName";
    public const string TokenKeyBuildDefinitionId = "Build.DefinitionId";
    public const string TokenKeySourceBranch = "Build.SourceBranch";
    public const string TokenKeySourceBranchFullName = "Build.SourceBranchFullName";
    public const string TokenKeyArtifactType = "Artifact.ArtifactType";
    public static readonly string BranchPrefix = "refs/heads/";

    public abstract string Name { get; }

    public abstract string EndpointTypeId { get; }

    public virtual string ArtifactType => (string) null;

    public abstract Guid ArtifactDownloadTaskId { get; }

    public abstract IDictionary<string, Guid> ArtifactDownloadTaskIds { get; }

    public abstract IDictionary<string, string> TaskInputMapping { get; }

    public abstract IDictionary<Guid, IDictionary<string, string>> TaskInputMappings { get; }

    public abstract IDictionary<string, string> TaskInputDefaultValues { get; }

    public abstract string Type { get; }

    public abstract bool IsCommitsTraceabilitySupported { get; }

    public abstract bool IsWorkitemsTraceabilitySupported { get; }

    public abstract string DisplayName { get; }

    public abstract string UniqueSourceIdentifier { get; }

    public virtual IDictionary<string, string> YamlInputMapping { get; }

    public static string GetSourceInput(IDictionary<string, string> dictionary, string inputId)
    {
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      string sourceInput;
      if (!dictionary.TryGetValue(inputId, out sourceInput) || string.IsNullOrEmpty(sourceInput))
        throw new ArgumentException(inputId);
      return sourceInput;
    }

    public static InputValue GetSourceInput(
      IDictionary<string, InputValue> dictionary,
      string inputId,
      bool throwOnException,
      string exceptionMessage)
    {
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      if (inputId == null)
        throw new ArgumentNullException(nameof (inputId));
      InputValue sourceInput;
      if (dictionary.TryGetValue(inputId, out sourceInput) && sourceInput != null)
        return sourceInput;
      if (throwOnException)
        throw new InvalidRequestException(exceptionMessage);
      return (InputValue) null;
    }

    public IDictionary<string, string> GetTaskInputMapping(Guid? taskId = null)
    {
      IDictionary<string, string> dictionary;
      return !taskId.HasValue || taskId.Equals((object) Guid.Empty) || this.TaskInputMappings == null || !this.TaskInputMappings.TryGetValue(taskId.Value, out dictionary) ? this.TaskInputMapping : dictionary;
    }

    protected TaskStep GetTaskStep(
      ITaskStore taskStore,
      TaskStep step,
      IDictionary<string, string> inputs,
      string artifactResourceType = "",
      string version = "")
    {
      TaskStep taskStep = (TaskStep) null;
      Guid taskId = Guid.Empty;
      if (this.ArtifactDownloadTaskIds != null && this.ArtifactDownloadTaskIds.Any<KeyValuePair<string, Guid>>() && !string.IsNullOrEmpty(artifactResourceType))
        this.ArtifactDownloadTaskIds.TryGetValue(artifactResourceType, out taskId);
      else
        taskId = this.ArtifactDownloadTaskId;
      if (taskId != Guid.Empty)
      {
        TaskDefinition taskDefinition = taskStore?.ResolveTask(taskId, version);
        if (taskDefinition != null)
        {
          taskStep = (TaskStep) step.Clone();
          taskStep.Reference = new TaskStepDefinitionReference()
          {
            Id = taskDefinition.Id,
            Name = taskDefinition.Name,
            Version = (string) taskDefinition.Version
          };
          ArtifactTaskInputMapper.PopulateMappedTaskInputs((IArtifactType) this, inputs, new Guid?(taskDefinition.Id));
          taskStep.Inputs.Clear();
          taskStep.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) inputs);
        }
      }
      return taskStep;
    }

    public static string GetBranchName(string sourceBranch)
    {
      if (string.IsNullOrEmpty(sourceBranch))
        return sourceBranch;
      int num = sourceBranch.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
      return num != -1 ? sourceBranch.Substring(num + 1) : sourceBranch;
    }

    public abstract IList<InputDescriptor> InputDescriptors { get; }

    public virtual ArtifactTriggerConfiguration ArtifactTriggerConfiguration => (ArtifactTriggerConfiguration) null;

    public virtual void FillQueuePipelineDataParameters(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      WebHookEventPayloadInputMapper inputMapper)
    {
    }

    public abstract InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues);

    public abstract AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact);

    public abstract IList<InputValue> GetAvailableVersions(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo);

    public abstract InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo);

    public virtual bool? UsesExternalAndPublicSourceRepo(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      Guid projectId,
      object artifactContext)
    {
      return new bool?();
    }

    public abstract IList<Change> GetChanges(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext);

    public abstract IList<Change> GetChangesBetweenArtifactSource(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineArtifactSource currentPipelineArtifactSource,
      PipelineArtifactSource lastPipelineArtifactSource,
      int top);

    public abstract IList<WorkItemRef> GetWorkItems(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext,
      GetConfig getConfig);

    public virtual void LinkDeploymentToWorkItems(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo artifactSourceProjectInfo,
      ProjectInfo releaseProjectInfo,
      int top,
      object artifactContext,
      LinkConfig linkConfig,
      DeploymentData deploymentData)
    {
    }

    public virtual IEnumerable<string> GetLinkedWorkItemIds(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId)
    {
      return (IEnumerable<string>) new List<string>();
    }

    public virtual string GetDefaultSourceAlias(ArtifactSource artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      return artifact != null ? artifact.SourceId : throw new ArgumentException("source");
    }

    public virtual IDictionary<string, string> GetFormatMaskTokensFromReleaseArtifactInstance(
      IDictionary<string, InputValue> sourceInputs)
    {
      return (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "Artifact.ArtifactType",
          this.Name
        },
        {
          "Build.DefinitionId",
          string.Empty
        },
        {
          "build.definitionName",
          string.Empty
        },
        {
          "build.buildNumber",
          string.Empty
        },
        {
          "Build.SourceBranch",
          string.Empty
        }
      };
    }

    public IDictionary<string, ConfigurationVariableValue> GetConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource PipelineArtifactSource)
    {
      IDictionary<string, ConfigurationVariableValue> configurationVariables = this.GetArtifactConfigurationVariables(context, PipelineArtifactSource);
      VariablesUtility.FillVariables(this.GetCommonArtifactConfigurationVariables(), configurationVariables);
      return configurationVariables;
    }

    public virtual IList<string> GetValidYamlInputs()
    {
      IList<string> validYamlInputs = (IList<string>) new List<string>();
      foreach (InputDescriptor inputDescriptor in (IEnumerable<InputDescriptor>) (this.InputDescriptors ?? (IList<InputDescriptor>) new List<InputDescriptor>()))
        validYamlInputs.Add(inputDescriptor.Id);
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) (this.YamlInputMapping ?? (IDictionary<string, string>) new Dictionary<string, string>()))
        validYamlInputs.Add(keyValuePair.Key);
      return validYamlInputs;
    }

    public static string RefToBranchName(string branch) => string.IsNullOrEmpty(branch) || !branch.StartsWith(ArtifactTypeBase.BranchPrefix, StringComparison.Ordinal) ? branch : branch.Substring(ArtifactTypeBase.BranchPrefix.Length);

    public abstract bool ResolveStep(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      JobStep step,
      out IList<TaskStep> resolvedSteps);

    public virtual bool SupportsLatestVersionDataSourceBinding() => false;

    protected static Guid GetCollectionId(IVssRequestContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return context.ServiceHost.InstanceId;
    }

    protected static Uri GetUriFromRequestContext(IVssRequestContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      string empty = string.Empty;
      ILocationService service = context.GetService<ILocationService>();
      string str = !context.ExecutionEnvironment.IsHostedDeployment ? service.GetPublicAccessMapping(context).AccessPoint : service.GetLocationServiceUrl(context, ServiceInstanceTypes.TFS, AccessMappingConstants.ClientAccessMappingMoniker);
      string uriString = str == null ? string.Empty : str;
      if (!uriString.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        uriString += "/";
      return new Uri(uriString);
    }

    protected abstract IDictionary<string, ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource);

    public abstract Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId);

    public abstract Uri GetArtifactSourceDefinitionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId);

    private IDictionary<string, ConfigurationVariableValue> GetCommonArtifactConfigurationVariables() => (IDictionary<string, ConfigurationVariableValue>) new Dictionary<string, ConfigurationVariableValue>()
    {
      {
        "type",
        new ConfigurationVariableValue() { Value = this.Name }
      }
    };

    public void UpdateWebApiArtifact(ArtifactSource serverArtifact)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      serverArtifact.ArtifactTypeId = this.Name;
    }
  }
}
