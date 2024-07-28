// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineArtifact
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class PipelineArtifact : ArtifactTypeBase
  {
    public static readonly Guid FileContainerArtifactDownloadTaskId = new Guid("a433f589-fce1-4460-9ee6-44a624aeb1fb");
    public static readonly Guid FileShareArtifactDownloadTaskId = new Guid("E3CF3806-AD30-4EC4-8F1E-8ECD98771AA0");
    public static readonly Guid PipelineArtifactDownloadTaskId = new Guid("61F2A582-95AE-4948-B34D-A1B3C4F6A737");
    private readonly IDictionary<string, Guid> m_artifactDownloadTaskId = (IDictionary<string, Guid>) new Dictionary<string, Guid>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Container",
        PipelineArtifact.FileContainerArtifactDownloadTaskId
      },
      {
        "FilePath",
        PipelineArtifact.FileShareArtifactDownloadTaskId
      },
      {
        nameof (PipelineArtifact),
        PipelineArtifact.PipelineArtifactDownloadTaskId
      }
    };
    private readonly IDictionary<Guid, IDictionary<string, string>> m_taskInputMapping = (IDictionary<Guid, IDictionary<string, string>>) new Dictionary<Guid, IDictionary<string, string>>()
    {
      {
        PipelineArtifact.FileContainerArtifactDownloadTaskId,
        PipelineArtifact.m_commonTaskInputMapping
      },
      {
        PipelineArtifact.FileShareArtifactDownloadTaskId,
        PipelineArtifact.m_commonTaskInputMapping
      },
      {
        PipelineArtifact.PipelineArtifactDownloadTaskId,
        PipelineArtifact.m_commonTaskInputMapping
      }
    };
    private static readonly IDictionary<string, string> m_commonTaskInputMapping = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "artifactName",
        PipelineArtifact.GetInputMappingTemplateFormat("artifact")
      },
      {
        "itemPattern",
        PipelineArtifact.GetInputMappingTemplateFormat("patterns")
      },
      {
        "downloadPath",
        "$(Pipeline.Workspace)/{{path}}"
      }
    };
    private readonly IDictionary<string, string> m_taskInputDefaultVaules = (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "itemPattern",
        "**"
      },
      {
        "downloadPath",
        "$(Pipeline.Workspace)"
      }
    };
    private readonly string m_stableDownloadPipelineArtifactVersion = "1";
    private readonly string m_latestDownloadPipelineArtifactVersion = "2";
    private readonly string m_stableDownloadBuildArtifactVersion = "0";
    private readonly string m_uniqueSourceIdentifier = "{{connection}}:{{project}}:{{definition}}";

    public override ArtifactTriggerConfiguration ArtifactTriggerConfiguration { get; }

    public override IDictionary<string, Guid> ArtifactDownloadTaskIds => this.m_artifactDownloadTaskId;

    public override Guid ArtifactDownloadTaskId { get; }

    public override IDictionary<Guid, IDictionary<string, string>> TaskInputMappings => this.m_taskInputMapping;

    public override IDictionary<string, string> TaskInputMapping { get; }

    public override IDictionary<string, string> TaskInputDefaultValues => this.m_taskInputDefaultVaules;

    public override string Type => "Pipeline";

    public override string Name => "Pipeline";

    public override string EndpointTypeId { get; }

    public override string UniqueSourceIdentifier => this.m_uniqueSourceIdentifier;

    public override IList<InputDescriptor> InputDescriptors { get; }

    public override string DisplayName => "Pipeline";

    public override bool IsCommitsTraceabilitySupported => false;

    public override bool IsWorkitemsTraceabilitySupported => false;

    public override IDictionary<string, string> YamlInputMapping { get; }

    public PipelineArtifact()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      {
        {
          "TfsGitRepositoryId",
          "{{Run.Resources.Repositories['self'].Repository.Id}}"
        },
        {
          "GitHubRepositoryId",
          "{{Run.Resources.Repositories['self'].Repository.FullName}}"
        },
        {
          "GitHubEnterpriseRepositoryId",
          "{{Run.Resources.Repositories['self'].Repository.FullName}}"
        },
        {
          "RepositoryType",
          "{{Run.Resources.Repositories['self'].Repository.Type}}"
        },
        {
          "SourceBranch",
          "{{Run.Resources.Repositories['self'].RefName}}"
        },
        {
          "SourceName",
          "{{Pipeline.Name}}"
        },
        {
          "SourceVersion",
          "{{Run.Resources.Repositories['self'].Version}}"
        },
        {
          "StageName",
          "{{Stage.Name}}"
        },
        {
          "TriggeredByPipeline.ProjectId",
          "{{ProjectId}}"
        },
        {
          "pipelineId",
          "{{Run.Id}}"
        },
        {
          "Version",
          "{{Run.Name}}"
        },
        {
          "PipelineDefinitionId",
          "{{Run.Pipeline.Id}}"
        }
      };
      this.ArtifactTriggerConfiguration = new ArtifactTriggerConfiguration()
      {
        IsTriggerSupported = true,
        IsTriggerSupportedOnlyInHosted = false,
        IsWebhookSupportedAtServerLevel = false,
        WebhookPayloadMapping = dictionary
      };
    }

    public override void FillQueuePipelineDataParameters(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      WebHookEventPayloadInputMapper inputMapper)
    {
      string str1;
      if (this.IsWebHookFromSameRepository(requestContext.GetService<IPipelineTfsBuildService>().GetPipelineDefinition(requestContext, build.Project, build.Definition.Id).Repository, inputMapper) && inputMapper.GetValue("SourceBranch", out str1) && !string.IsNullOrEmpty(str1))
      {
        build.SourceBranch = str1;
        string str2;
        if (inputMapper.GetValue("SourceVersion", out str2) && !string.IsNullOrEmpty(str2))
          build.SourceVersion = str2;
      }
      string str3;
      if (inputMapper.GetValue("SourceName", out str3))
        build.TriggerInfo[PipelinePropertyNames.Source] = str3;
      string input;
      if (inputMapper.GetValue("TriggeredByPipeline.ProjectId", out input) && Guid.TryParse(input, out Guid _))
        build.TriggerInfo[PipelinePropertyNames.ProjectId] = input;
      string s;
      if (inputMapper.GetValue("pipelineId", out s) && int.TryParse(s, out int _))
        build.TriggerInfo[PipelinePropertyNames.PipelineId] = s;
      string str4;
      if (!inputMapper.GetValue("Version", out str4))
        return;
      build.TriggerInfo[PipelinePropertyNames.Version] = str4;
    }

    public static IList<BuildArtifact> GetBuildArtifacts(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      bool resolveToXamlClientIfRequired = false)
    {
      BuildHttpClient client = requestContext.GetClient<BuildHttpClient>();
      try
      {
        return (IList<BuildArtifact>) client.GetArtifactsAsync(projectId, buildId).Result;
      }
      catch (AggregateException ex)
      {
        if (ex.InnerException is BuildNotFoundException & resolveToXamlClientIfRequired)
          return (IList<BuildArtifact>) requestContext.GetClient<XamlBuildHttpClient>().GetArtifactsAsync(projectId, buildId).Result;
        throw;
      }
    }

    public static Guid? GetProjectId(IVssRequestContext requestContext, PipelineResource resource)
    {
      ProjectInfo projectInfo = (ProjectInfo) null;
      string projectName;
      if (resource.Properties.TryGetValue<string>(PipelinePropertyNames.Project, out projectName))
        projectInfo = requestContext.GetService<IProjectService>().GetProject(requestContext, projectName);
      return projectInfo?.Id;
    }

    public override bool ResolveStep(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IPipelineContext>(pipelineContext, nameof (pipelineContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<JobStep>(step, nameof (step));
      bool flag = false;
      resolvedSteps = (IList<TaskStep>) new List<TaskStep>();
      TaskStep taskStep1 = step as TaskStep;
      IList<BuildArtifact> buildArtifactList = (IList<BuildArtifact>) null;
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (taskStep1.IsDownloadPipelineArtifactStepDisabled())
        flag = true;
      else if (taskStep1.IsDownloadCurrentPipelineArtifactStep())
      {
        IList<BuildArtifact> buildArtifacts = this.GetBuildArtifacts(requestContext, pipelineContext, projectId);
        string artifactName;
        if (taskStep1.Inputs.TryGetValue("artifact", out artifactName))
        {
          BuildArtifact pipelineArtifact = buildArtifacts.FirstOrDefault<BuildArtifact>((Func<BuildArtifact, bool>) (x => x.Name.Equals(artifactName, StringComparison.Ordinal)));
          TaskStep taskStep2 = this.ConvertArtifactToTaskStep(requestContext, pipelineContext.TaskStore, pipelineArtifact, taskStep1, artifactName, (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(taskStep1.Inputs));
          if (taskStep2 != null)
          {
            resolvedSteps.Add(taskStep2);
            flag = true;
          }
        }
        else
        {
          resolvedSteps.AddRange<TaskStep, IList<TaskStep>>((IEnumerable<TaskStep>) this.ConvertArtifactsToArtifactSteps(requestContext, pipelineContext.TaskStore, buildArtifacts, taskStep1, (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(taskStep1.Inputs)));
          flag = true;
        }
      }
      else if (taskStep1.IsDownloadExternalPipelineArtifactStep())
      {
        string aliasFromTaskStep = taskStep1.GetAliasFromTaskStep();
        PipelineResource resource = pipelineContext.ResourceStore?.Pipelines.Get(aliasFromTaskStep);
        if (resource == null)
          throw new ArtifactNotFoundException("Download Pipeline Artifacts shortcut: cannot resolve source name");
        if (!resource.Properties.TryGetValue<IList<BuildArtifact>>(PipelinePropertyNames.Artifacts, out buildArtifactList))
          buildArtifactList = (IList<BuildArtifact>) new List<BuildArtifact>();
        IDictionary<string, string> resourceInputs = resource.GetResourceInputs();
        IDictionary<string, string> pipelineInputs = this.GetPipelineInputs(resource);
        dictionary.Merge(resourceInputs);
        dictionary.Merge(pipelineInputs);
        string artifactName;
        if (taskStep1.Inputs.TryGetValue("artifact", out artifactName))
        {
          BuildArtifact pipelineArtifact = buildArtifactList.FirstOrDefault<BuildArtifact>((Func<BuildArtifact, bool>) (x => x.Name.Equals(artifactName, StringComparison.Ordinal)));
          dictionary.Merge(taskStep1.Inputs);
          TaskStep taskStep3 = this.ConvertArtifactToTaskStep(requestContext, pipelineContext.TaskStore, pipelineArtifact, taskStep1, artifactName, (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(dictionary), resource.Alias);
          if (taskStep3 != null)
          {
            resolvedSteps.Add(taskStep3);
            flag = true;
          }
        }
        else
        {
          dictionary.Merge(taskStep1.Inputs);
          resolvedSteps.AddRange<TaskStep, IList<TaskStep>>((IEnumerable<TaskStep>) this.ConvertArtifactsToArtifactSteps(requestContext, pipelineContext.TaskStore, buildArtifactList, taskStep1, (IReadOnlyDictionary<string, string>) new ReadOnlyDictionary<string, string>(dictionary), resource.Alias));
          flag = true;
        }
      }
      PipelineArtifact.PublishCustomerIntelligenceData(requestContext, pipelineContext, projectId, taskStep1);
      return flag;
    }

    public override InputValues GetInputValues(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      string inputId,
      IDictionary<string, string> currentInputValues)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo));
      ArgumentUtility.CheckStringForNullOrEmpty(inputId, nameof (inputId));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(currentInputValues, nameof (currentInputValues));
      IWebHookExtension webHookExtension = context.GetService<IWebHookService>().GetWebHookExtension(context);
      if (webHookExtension != null)
        return webHookExtension.GetWebHookPublisherInputValues(context, projectInfo, inputId, currentInputValues);
      return new InputValues()
      {
        Error = new InputValuesError()
        {
          Message = TaskResources.CannotFindWebHookExtension()
        }
      };
    }

    public override AgentArtifactDefinition ToAgentArtifact(
      IVssRequestContext requestContext,
      Guid projectId,
      AgentArtifactDefinition serverArtifact)
    {
      throw new NotImplementedException();
    }

    public override IList<InputValue> GetAvailableVersions(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      throw new NotImplementedException();
    }

    public override InputValue GetLatestVersion(
      IVssRequestContext requestContext,
      IDictionary<string, string> sourceInputs,
      ProjectInfo projectInfo)
    {
      throw new NotImplementedException();
    }

    public override IList<Change> GetChanges(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext)
    {
      throw new NotImplementedException();
    }

    public override IList<Change> GetChangesBetweenArtifactSource(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineArtifactSource currentPipelineArtifactSource,
      PipelineArtifactSource lastPipelineArtifactSource,
      int top)
    {
      throw new NotImplementedException();
    }

    public override IList<WorkItemRef> GetWorkItems(
      IVssRequestContext requestContext,
      Dictionary<string, InputValue> sourceData,
      InputValue startVersion,
      InputValue endVersion,
      ProjectInfo projectInfo,
      int top,
      object artifactContext,
      GetConfig getConfig)
    {
      throw new NotImplementedException();
    }

    protected override IDictionary<string, ConfigurationVariableValue> GetArtifactConfigurationVariables(
      IVssRequestContext context,
      ArtifactSource artifactSource)
    {
      throw new NotImplementedException();
    }

    public override Uri GetArtifactSourceVersionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      throw new NotImplementedException();
    }

    public override Uri GetArtifactSourceDefinitionUrl(
      IVssRequestContext requestContext,
      ArtifactSource serverArtifact,
      Guid projectId)
    {
      throw new NotImplementedException();
    }

    private IList<TaskStep> ConvertArtifactsToArtifactSteps(
      IVssRequestContext requestContext,
      ITaskStore taskStore,
      IList<BuildArtifact> publishedArtifacts,
      TaskStep downloadStep,
      IReadOnlyDictionary<string, string> taskInputs,
      string resourceAlias = null)
    {
      IList<TaskStep> artifactSteps = (IList<TaskStep>) new List<TaskStep>();
      int num = 0;
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dictionary1.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) taskInputs);
      if (!string.IsNullOrEmpty(resourceAlias))
        dictionary1["path"] = resourceAlias;
      if (publishedArtifacts != null && publishedArtifacts.Any<BuildArtifact>() && this.IsAnyPipelineArtifactTypeExists(publishedArtifacts) && !requestContext.IsFeatureEnabled("DistributedTask.UseNewDownloadPipelineArtifactsTaskForDeploymentJobs"))
      {
        Dictionary<string, string> dictionary2 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        dictionary2.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) dictionary1);
        TaskStep taskStep = this.GetTaskStep(requestContext, taskStore, downloadStep, (IDictionary<string, string>) dictionary2, nameof (PipelineArtifact));
        artifactSteps.Add(taskStep);
      }
      foreach (BuildArtifact buildArtifact in (IEnumerable<BuildArtifact>) (publishedArtifacts ?? (IList<BuildArtifact>) new List<BuildArtifact>()))
      {
        if (buildArtifact.Resource != null && (string.Equals(buildArtifact.Resource.Type, "Container", StringComparison.OrdinalIgnoreCase) || string.Equals(buildArtifact.Resource.Type, "FilePath", StringComparison.OrdinalIgnoreCase) || requestContext.IsFeatureEnabled("DistributedTask.UseNewDownloadPipelineArtifactsTaskForDeploymentJobs")))
        {
          Dictionary<string, string> dictionary3 = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary3.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) dictionary1);
          dictionary3["artifact"] = buildArtifact.Name;
          if (string.Equals(buildArtifact.Resource.Type, "FilePath", StringComparison.OrdinalIgnoreCase))
            dictionary3["fileSharePath"] = buildArtifact.Resource.Data;
          TaskStep taskStep = this.GetTaskStep(requestContext, taskStore, downloadStep, buildArtifact.Resource.Type, buildArtifact.Name, (IDictionary<string, string>) dictionary3);
          if (taskStep != null)
          {
            string name = PipelineArtifactConstants.DownloadTask.Name + "_" + buildArtifact.Name;
            if (!string.IsNullOrEmpty(resourceAlias))
              name = PipelineArtifactConstants.DownloadTask.Name + "_" + resourceAlias + "_" + buildArtifact.Name;
            taskStep.Name = string.Format("{0}{1}", (object) NameValidation.Sanitize(name), (object) num);
            ++num;
            if (taskStep.Reference.Name == "DownloadPipelineArtifact")
              taskStep.Inputs["path"] = taskStep.Inputs["downloadPath"] + taskStep.Inputs["artifactName"] + "/";
            artifactSteps.Add(taskStep);
          }
        }
      }
      return artifactSteps;
    }

    private TaskStep ConvertArtifactToTaskStep(
      IVssRequestContext requestContext,
      ITaskStore taskStore,
      BuildArtifact pipelineArtifact,
      TaskStep downloadStep,
      string artifactName,
      IReadOnlyDictionary<string, string> taskInputs,
      string resourceAlias = null)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dictionary.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) taskInputs);
      string str1 = nameof (PipelineArtifact);
      if (pipelineArtifact != null && pipelineArtifact.Resource != null && (string.Equals(pipelineArtifact.Resource.Type, "Container", StringComparison.OrdinalIgnoreCase) || string.Equals(pipelineArtifact.Resource.Type, "FilePath", StringComparison.OrdinalIgnoreCase)))
      {
        str1 = pipelineArtifact.Resource.Type;
        if (string.Equals(str1, "Container", StringComparison.OrdinalIgnoreCase))
          dictionary["downloadType"] = "single";
        else if (string.Equals(str1, "FilePath", StringComparison.OrdinalIgnoreCase))
          dictionary["fileSharePath"] = pipelineArtifact.Resource.Data;
      }
      string str2 = artifactName;
      if (!string.IsNullOrEmpty(resourceAlias))
        str2 = resourceAlias + "/" + artifactName;
      dictionary["path"] = !string.Equals(str1, nameof (PipelineArtifact), StringComparison.OrdinalIgnoreCase) ? (string.IsNullOrEmpty(resourceAlias) ? string.Empty : resourceAlias) : str2;
      TaskStep taskStep = this.GetTaskStep(requestContext, taskStore, downloadStep, dictionary, str1);
      if (taskStep.Reference.Name == "DownloadPipelineArtifact" && requestContext.IsFeatureEnabled("DistributedTask.UseNewDownloadPipelineArtifactsTaskForDeploymentJobs"))
        taskStep.Inputs["path"] = taskStep.Inputs["downloadPath"];
      return taskStep;
    }

    private IList<BuildArtifact> GetBuildArtifacts(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId)
    {
      VariableValue variableValue;
      int result;
      if (pipelineContext.Variables.TryGetValue("build.buildId", out variableValue) && int.TryParse(variableValue.Value, out result))
        return PipelineArtifact.GetBuildArtifacts(requestContext, projectId, result);
      throw new PipelineException(TaskResources.CannotFindCurrentPipelineId());
    }

    private string GetDownloadPipelineArtifactVersion(
      IVssRequestContext requestContext,
      string resourceType)
    {
      string pipelineArtifactVersion = string.Empty;
      if (string.Equals(resourceType, nameof (PipelineArtifact), StringComparison.OrdinalIgnoreCase))
      {
        pipelineArtifactVersion = this.m_stableDownloadPipelineArtifactVersion;
        if (requestContext.IsFeatureEnabled("DistributedTask.UseNewDownloadPipelineArtifactsTaskForDeploymentJobs"))
          pipelineArtifactVersion = this.m_latestDownloadPipelineArtifactVersion;
      }
      else if (string.Equals(resourceType, "Container", StringComparison.OrdinalIgnoreCase) && !requestContext.IsFeatureEnabled("DistributedTask.UseNewDownloadBuildArtifactsTaskForDeploymentJobs"))
      {
        pipelineArtifactVersion = this.m_stableDownloadBuildArtifactVersion;
        requestContext.TraceAlways(10016148, nameof (PipelineArtifact), "Injecting older V0 version of DownloadBuildArtifacts");
      }
      return pipelineArtifactVersion;
    }

    private IDictionary<string, string> GetPipelineInputs(PipelineResource resource)
    {
      Dictionary<string, string> pipelineInputs = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string str;
      if (resource.Properties.TryGetValue<string>(PipelinePropertyNames.PipelineId, out str))
      {
        pipelineInputs["buildType"] = "specific";
        Guid guid;
        if (resource.Properties.TryGetValue<Guid>(PipelinePropertyNames.ProjectId, out guid))
          pipelineInputs["project"] = guid.ToString();
        int num;
        if (resource.Properties.TryGetValue<int>(PipelinePropertyNames.DefinitionId, out num))
          pipelineInputs["definition"] = num.ToString();
        pipelineInputs["buildId"] = str;
        pipelineInputs["buildVersionToDownload"] = "specific";
      }
      return (IDictionary<string, string>) pipelineInputs;
    }

    private TaskStep GetTaskStep(
      IVssRequestContext requestContext,
      ITaskStore taskStore,
      TaskStep downloadStep,
      string artifactType,
      string artifactName,
      IDictionary<string, string> inputs)
    {
      TaskStep taskStep = this.GetTaskStep(requestContext, taskStore, downloadStep, inputs, artifactType);
      if (taskStep != null && (string.IsNullOrEmpty(taskStep.DisplayName) || string.Equals(taskStep.DisplayName, PipelineArtifactConstants.DownloadTask.Name)))
        taskStep.DisplayName = TaskResources.DownloadArtifact((object) artifactName);
      return taskStep;
    }

    protected TaskStep GetTaskStep(
      IVssRequestContext requestContext,
      ITaskStore taskStore,
      TaskStep step,
      IDictionary<string, string> inputs,
      string artifactResourceType = "")
    {
      string pipelineArtifactVersion = this.GetDownloadPipelineArtifactVersion(requestContext, artifactResourceType);
      this.PrepareForDownloadPipelineArtifactTask(artifactResourceType, inputs);
      return this.GetTaskStep(taskStore, step, inputs, artifactResourceType, pipelineArtifactVersion);
    }

    private void PrepareForDownloadPipelineArtifactTask(
      string artifactResourceType,
      IDictionary<string, string> inputs)
    {
      string str;
      if (!string.Equals(artifactResourceType, nameof (PipelineArtifact), StringComparison.OrdinalIgnoreCase) || !inputs.TryGetValue("path", out str) || !string.IsNullOrEmpty(str))
        return;
      inputs.Remove("path");
    }

    private static string GetInputMappingTemplateFormat(string input) => "{{" + input + "}}";

    private bool IsAnyPipelineArtifactTypeExists(IList<BuildArtifact> artifacts)
    {
      foreach (BuildArtifact artifact in (IEnumerable<BuildArtifact>) artifacts)
      {
        if (artifact.Resource != null && string.Equals(artifact.Resource.Type, nameof (PipelineArtifact), StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private bool IsWebHookFromSameRepository(
      RepositoryResource resource,
      WebHookEventPayloadInputMapper inputMapper)
    {
      string str;
      if (resource == null || inputMapper == null || !inputMapper.GetValue("RepositoryType", out str))
        return false;
      switch (str)
      {
        case "azureReposGit":
          string a1;
          if (string.Equals(resource.Type, "TfsGit", StringComparison.OrdinalIgnoreCase) && inputMapper.GetValue("TfsGitRepositoryId", out a1) && string.Equals(a1, resource.Id, StringComparison.OrdinalIgnoreCase))
            return true;
          break;
        case "gitHub":
          string a2;
          if (inputMapper.GetValue("GitHubRepositoryId", out a2) && string.Equals(a2, resource.Id, StringComparison.OrdinalIgnoreCase))
            return true;
          break;
        case "gitHubEnterprise":
          string a3;
          if (inputMapper.GetValue("GitHubEnterpriseRepositoryId", out a3) && string.Equals(a3, resource.Id, StringComparison.OrdinalIgnoreCase))
            return true;
          break;
        default:
          return false;
      }
      return false;
    }

    private static void PublishCustomerIntelligenceData(
      IVssRequestContext requestContext,
      IPipelineContext pipelineContext,
      Guid projectId,
      TaskStep taskStep)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      if (!service.IsTracingEnabled(requestContext) || taskStep == null)
        return;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(PipelinePropertyNames.ProjectId, (object) projectId);
      VariableValue variableValue;
      if (pipelineContext.Variables.TryGetValue(WellKnownDistributedTaskVariables.PlanId, out variableValue) && !string.IsNullOrEmpty(variableValue?.Value))
        properties.Add("planId", variableValue.Value);
      if (pipelineContext is JobExecutionContext executionContext)
      {
        Guid guid = executionContext.Job.Definition.Id;
        if (guid == Guid.Empty)
          guid = PipelineUtilities.GetJobInstanceId(executionContext.Stage, executionContext.Phase, executionContext.Job);
        properties.Add("jobId", (object) guid);
        properties.Add("jobName", executionContext.Job.Name);
        string str = string.Empty;
        if (executionContext.Phase.Definition.Type == PhaseType.Phase)
          str = "pipeline";
        else if (executionContext.Phase.Definition.Type == PhaseType.Provider)
          str = "deployment";
        properties.Add("jobType", str);
      }
      properties.Add("alias", taskStep.Inputs["alias"]);
      string str1;
      if (!taskStep.Inputs.TryGetValue("mode", out str1))
        str1 = "manual";
      properties.Add("mode", str1);
      string str2;
      if (taskStep.Inputs.TryGetValue("artifact", out str2))
        properties.Add("artifact", str2);
      string str3;
      if (taskStep.Inputs.TryGetValue("path", out str3))
        properties.Add("path", str3);
      string str4;
      if (taskStep.Inputs.TryGetValue("patterns", out str4))
        properties.Add("patterns", str4);
      service.Publish(requestContext, "PipelineArtifacts", "DownloadPipelineArtifact", properties);
    }
  }
}
