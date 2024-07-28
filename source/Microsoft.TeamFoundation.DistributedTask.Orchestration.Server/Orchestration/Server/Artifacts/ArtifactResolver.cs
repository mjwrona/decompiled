// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PackageProviders;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.BuildProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.ContainerProviders;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public sealed class ArtifactResolver : IArtifactResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;
    private IServiceEndpointStore m_serviceEndpointStore;
    private ITaskStore m_taskStore;

    public ArtifactResolver(
      IVssRequestContext requestContext,
      Guid projectId,
      IServiceEndpointStore endpointStore,
      ITaskStore taskStore = null)
    {
      this.m_projectId = projectId;
      this.m_requestContext = requestContext;
      this.m_serviceEndpointStore = endpointStore;
      if (taskStore == null)
        taskStore = requestContext.GetService<IPipelineBuilderService>().GetTaskStore(requestContext);
      this.m_taskStore = taskStore;
    }

    public ArtifactResolver(
      IVssRequestContext requestContext,
      Guid projectId,
      IServiceEndpointStore endpointStore,
      IList<TaskDefinition> tasks)
      : this(requestContext, projectId, endpointStore, (ITaskStore) new TaskStore((IEnumerable<TaskDefinition>) tasks, featureCallback: new Func<string, bool>(((VssRequestContextExtensions) requestContext).IsFeatureEnabled)))
    {
    }

    public ArtifactResolver(IVssRequestContext requestContext, Guid projectId)
    {
      this.m_projectId = projectId;
      this.m_requestContext = requestContext;
    }

    public Guid GetArtifactDownloadTaskId(Resource resource)
    {
      string typeId = string.Empty;
      if (resource is BuildResource buildResource)
        typeId = buildResource.Type;
      else if (resource is PackageResource packageResource)
        typeId = packageResource.Type;
      if (!string.IsNullOrEmpty(typeId))
      {
        IArtifactType artifactType = this.m_requestContext.GetService<IArtifactService>().GetArtifactType(this.m_requestContext, typeId);
        if (artifactType != null)
          return artifactType.ArtifactDownloadTaskId;
      }
      return Guid.Empty;
    }

    public void PopulateMappedTaskInputs(Resource resource, TaskStep taskStep)
    {
      IDictionary<string, string> resourceInputs = resource.GetResourceInputs();
      taskStep.Inputs.AddRange<KeyValuePair<string, string>, IDictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) resourceInputs);
      ServiceEndpoint serviceEndpoint = this.m_serviceEndpointStore?.Get(resource.Endpoint);
      if (serviceEndpoint != null && serviceEndpoint.Id != Guid.Empty)
        taskStep.Inputs.Add("connection", serviceEndpoint.Id.ToString());
      taskStep.Inputs["path"] = "$(Pipeline.Workspace)/" + resource.Alias;
      ArtifactTaskInputMapper.PopulateMappedTaskInputs(ArtifactResolver.GetArtifactTypeDefinition(this.m_requestContext, resource), taskStep.Inputs, new Guid?(taskStep.Reference.Id));
    }

    public bool ResolveStep(
      IPipelineContext pipelineContext,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      IArtifactService service = this.m_requestContext.GetService<IArtifactService>();
      string artifactType = this.GetArtifactType(pipelineContext, step);
      IVssRequestContext requestContext = this.m_requestContext;
      string typeId = artifactType;
      return service.GetArtifactType(requestContext, typeId).ResolveStep(this.m_requestContext, pipelineContext, this.m_projectId, step, out resolvedSteps);
    }

    public bool ResolveStep(
      IResourceStore resourceStore,
      TaskStep taskStep,
      out string errorMessage)
    {
      bool flag1 = true;
      errorMessage = string.Empty;
      Resource resource = (Resource) null;
      string alias = string.Empty;
      switch (taskStep.Reference.Name)
      {
        case "downloadBuild":
          if (resourceStore?.Builds != null && taskStep.Inputs.TryGetValue("alias", out alias))
          {
            resource = (Resource) resourceStore.Builds.GetAll().FirstOrDefault<BuildResource>((Func<BuildResource, bool>) (x => string.Equals(x.Alias, alias, StringComparison.OrdinalIgnoreCase)));
            break;
          }
          break;
        case "getPackage":
          if (resourceStore?.Packages != null && taskStep.Inputs.TryGetValue("alias", out alias))
          {
            resource = (Resource) resourceStore.Packages.GetAll().FirstOrDefault<PackageResource>((Func<PackageResource, bool>) (x => string.Equals(x.Alias, alias, StringComparison.OrdinalIgnoreCase)));
            break;
          }
          break;
        default:
          return flag1;
      }
      bool flag2;
      if (resource != null)
      {
        if (this.UpdateTaskReference(resource, taskStep))
        {
          this.PopulateMappedTaskInputs(resource, taskStep);
          flag2 = true;
        }
        else
        {
          errorMessage = TaskResources.CannotFindTaskReference((object) alias);
          flag2 = false;
        }
      }
      else
      {
        if (taskStep.Reference.Name.Equals("downloadBuild", StringComparison.OrdinalIgnoreCase))
          errorMessage = TaskResources.CannotFindBuildResource((object) alias);
        else if (taskStep.Reference.Name.Equals("getPackage", StringComparison.OrdinalIgnoreCase))
          errorMessage = TaskResources.CannotFindPackageResource((object) alias);
        flag2 = false;
      }
      return flag2;
    }

    public bool ValidateDeclaredResource(Resource resource, out PipelineValidationError error)
    {
      error = (PipelineValidationError) null;
      ServiceEndpoint endpoint = (ServiceEndpoint) null;
      if (resource.Endpoint != null)
        endpoint = this.m_serviceEndpointStore?.Get(resource.Endpoint);
      switch (resource)
      {
        case BuildResource build:
          BuildProvider buildProvider = new BuildProvider();
          try
          {
            buildProvider.Validate(this.m_requestContext, this.m_projectId, build, endpoint);
            buildProvider.ResolveVersion(this.m_requestContext, this.m_projectId, build, endpoint);
            break;
          }
          catch (Exception ex) when (ex is ExternalBuildProviderException || ex is ResourceValidationException)
          {
            this.m_requestContext.TraceError(10016107, "TaskHub", ex.Message);
            error = new PipelineValidationError(ex.GetType().Name, ex.Message);
            return false;
          }
        case PipelineResource pipeline:
          PipelineProvider pipelineProvider = new PipelineProvider();
          try
          {
            pipelineProvider.Validate(this.m_requestContext, this.m_projectId, pipeline, endpoint);
            break;
          }
          catch (Exception ex) when (ex is ExternalBuildProviderException || ex is ResourceValidationException)
          {
            this.m_requestContext.TraceError(10016213, "TaskHub", ex.Message);
            error = new PipelineValidationError(ex.GetType().Name, ex.Message);
            return false;
          }
        case PackageResource package:
          PackageProvider packageProvider = new PackageProvider();
          try
          {
            packageProvider.Validate(this.m_requestContext, package, endpoint);
            break;
          }
          catch (Exception ex) when (ex is ResourceValidationException)
          {
            this.m_requestContext.TraceError(10016216, "TaskHub", ex.Message);
            error = new PipelineValidationError(ex.GetType().Name, ex.Message);
            return false;
          }
        case WebhookResource webhook:
          WebHookProvider webHookProvider = new WebHookProvider();
          try
          {
            webHookProvider.Validate(this.m_requestContext, this.m_projectId, webhook, endpoint);
            break;
          }
          catch (ResourceValidationException ex)
          {
            this.m_requestContext.TraceError(10016213, "TaskHub", ex.Message);
            error = new PipelineValidationError(ex.GetType().Name, ex.Message);
            return false;
          }
      }
      return true;
    }

    public static void SetPipelineTemplateParameter(
      IVssRequestContext requestContext,
      IDictionary<string, string> triggerInfo,
      Dictionary<string, object> templateParameters)
    {
      string str1;
      PipelineTriggerType result;
      string key;
      string str2;
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnableWebHookResourceTriggers") || !triggerInfo.TryGetValue("pipelineTriggerType", out str1) || !System.Enum.TryParse<PipelineTriggerType>(str1, out result) || result != PipelineTriggerType.WebhookTriggeredEvent || !triggerInfo.TryGetValue("alias", out key) || !triggerInfo.TryGetValue("eventPayload", out str2))
        return;
      if (string.IsNullOrEmpty(str2))
        return;
      try
      {
        templateParameters[key] = (object) JObject.Parse(WebUtility.HtmlDecode(str2));
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(10016131, "WebHook", "Failed to parse the webhook payload event. Alias = {0}, EventPayload = {1}, Exception = {2}", (object) key, (object) str2, (object) ex.Message);
      }
      finally
      {
        triggerInfo.Remove("eventPayload");
      }
    }

    public static void SetBuildResourceVersion(
      IVssRequestContext requestContext,
      BuildResource build,
      IList<Stage> stages,
      string version)
    {
      build.Version = version;
      IEnumerable<Phase> source = stages != null ? stages.SelectMany<Stage, PhaseNode>((Func<Stage, IEnumerable<PhaseNode>>) (x =>
      {
        IList<PhaseNode> phases = x.Phases;
        return phases == null ? (IEnumerable<PhaseNode>) null : phases.Where<PhaseNode>((Func<PhaseNode, bool>) (y => y is Phase));
      })).Cast<Phase>() : (IEnumerable<Phase>) null;
      foreach (TaskStep taskStep1 in (source != null ? source.SelectMany<Phase, Step>((Func<Phase, IEnumerable<Step>>) (x => x.Steps.Where<Step>((Func<Step, bool>) (y => y is TaskStep)))).Cast<TaskStep>() : (IEnumerable<TaskStep>) null) ?? (IEnumerable<TaskStep>) new List<TaskStep>())
      {
        string a;
        if (taskStep1 is TaskStep taskStep2 && taskStep2.Inputs.TryGetValue("alias", out a) && string.Equals(a, build.Alias))
          ArtifactResolver.MapVersionToTaskInputs(requestContext, (Resource) build, build.Version, taskStep2.Inputs);
      }
    }

    public static void SetContainerResourceVersion(
      ContainerResource containerResource,
      string version)
    {
      new ContainerProvider().SetContainerResourceVersion(containerResource, version);
    }

    public static void SetPipelineVersion(
      IVssRequestContext requestContext,
      PipelineResources resources,
      PipelineProcess process,
      IDictionary<string, string> triggerInfo)
    {
      string version;
      string alias;
      if (!triggerInfo.TryGetValue("alias", out alias) || !triggerInfo.TryGetValue(PipelinePropertyNames.Version, out version))
        return;
      foreach (PipelineResource pipeline in (IEnumerable<PipelineResource>) resources.Pipelines)
      {
        if (string.Equals(pipeline.Alias, alias, StringComparison.OrdinalIgnoreCase))
        {
          pipeline.Version = version;
          return;
        }
      }
      ISet<BuildResource> builds = resources.Builds;
      BuildResource build = builds != null ? builds.Where<BuildResource>((Func<BuildResource, bool>) (x => string.Equals(x.Alias, alias, StringComparison.OrdinalIgnoreCase))).SingleOrDefault<BuildResource>() : (BuildResource) null;
      if (build != null && process != null)
        ArtifactResolver.SetBuildResourceVersion(requestContext, build, process.Stages, version);
      if (!requestContext.IsFeatureEnabled("DistributedTask.EnablePackageResourceTriggers"))
        return;
      ISet<PackageResource> packages = resources.Packages;
      PackageResource package = packages != null ? packages.Where<PackageResource>((Func<PackageResource, bool>) (x => string.Equals(x.Alias, alias, StringComparison.OrdinalIgnoreCase))).SingleOrDefault<PackageResource>() : (PackageResource) null;
      if (package == null || process == null)
        return;
      ArtifactResolver.SetPackageResourceVersion(requestContext, package, process.Stages, version);
    }

    public static void SetPackageResourceVersion(
      IVssRequestContext requestContext,
      PackageResource package,
      IList<Stage> stages,
      string version)
    {
      package.Version = version;
      IEnumerable<Phase> source = stages != null ? stages.SelectMany<Stage, PhaseNode>((Func<Stage, IEnumerable<PhaseNode>>) (x =>
      {
        IList<PhaseNode> phases = x.Phases;
        return phases == null ? (IEnumerable<PhaseNode>) null : phases.Where<PhaseNode>((Func<PhaseNode, bool>) (y => y is Phase));
      })).Cast<Phase>() : (IEnumerable<Phase>) null;
      foreach (TaskStep taskStep1 in (source != null ? source.SelectMany<Phase, Step>((Func<Phase, IEnumerable<Step>>) (x => x.Steps.Where<Step>((Func<Step, bool>) (y => y is TaskStep)))).Cast<TaskStep>() : (IEnumerable<TaskStep>) null) ?? (IEnumerable<TaskStep>) new List<TaskStep>())
      {
        string a;
        if (taskStep1 is TaskStep taskStep2 && taskStep2.Inputs.TryGetValue("alias", out a) && string.Equals(a, package.Alias))
          ArtifactResolver.MapVersionToTaskInputs(requestContext, (Resource) package, package.Version, taskStep2.Inputs);
      }
    }

    private string GetArtifactType(IPipelineContext pipelineContext, JobStep step)
    {
      string artifactType = string.Empty;
      if (step.IsDownloadTask())
        artifactType = "Pipeline";
      else if (step.IsDownloadBuildTask() && step is TaskStep step1)
      {
        string aliasFromTaskStep = step1.GetAliasFromTaskStep();
        BuildResource buildResource = pipelineContext.ResourceStore?.Builds?.Get(aliasFromTaskStep);
        if (buildResource != null)
          artifactType = ArtifactResolver.GetArtifactType((Resource) buildResource);
      }
      return artifactType;
    }

    private bool UpdateTaskReference(Resource resource, TaskStep taskStep)
    {
      Guid artifactDownloadTaskId = this.GetArtifactDownloadTaskId(resource);
      if (artifactDownloadTaskId != Guid.Empty)
      {
        TaskDefinition taskDefinition = this.m_taskStore.ResolveTask(artifactDownloadTaskId, string.Empty);
        if (taskDefinition != null)
        {
          taskStep.Reference.Name = taskDefinition.Name;
          taskStep.Reference.Id = taskDefinition.Id;
          taskStep.Reference.Version = (string) taskDefinition.Version;
          if (string.IsNullOrEmpty(taskStep.DisplayName) || string.Equals(taskStep.DisplayName, "downloadBuild", StringComparison.OrdinalIgnoreCase))
            taskStep.DisplayName = taskDefinition.Name;
          if (taskStep.DisplayName.StartsWith("getPackage", StringComparison.OrdinalIgnoreCase))
            taskStep.DisplayName = taskDefinition.FriendlyName + " - " + resource.Alias;
          return true;
        }
      }
      return false;
    }

    private static string GetArtifactType(Resource resource)
    {
      string artifactType = string.Empty;
      switch (resource)
      {
        case null:
        case PipelineResource _:
          artifactType = "Pipeline";
          break;
        case BuildResource buildResource:
          artifactType = buildResource.Type;
          break;
        case PackageResource packageResource:
          artifactType = packageResource.Type;
          break;
      }
      return artifactType;
    }

    private static IArtifactType GetArtifactTypeDefinition(
      IVssRequestContext requestContext,
      Resource resource)
    {
      string artifactType = ArtifactResolver.GetArtifactType(resource);
      return !artifactType.IsNullOrEmpty<char>() ? requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, artifactType) : (IArtifactType) null;
    }

    private static void MapVersionToTaskInputs(
      IVssRequestContext requestContext,
      Resource resource,
      string version,
      IDictionary<string, string> actualTaskInputs)
    {
      IDictionary<string, string> taskInputs = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          nameof (version),
          version
        }
      };
      ArtifactTaskInputMapper.PopulateMappedTaskInputs(ArtifactResolver.GetArtifactTypeDefinition(requestContext, resource), taskInputs);
      foreach (string key in (IEnumerable<string>) taskInputs.Keys)
      {
        if (!string.IsNullOrEmpty(taskInputs[key]))
          actualTaskInputs[key] = taskInputs[key];
      }
    }
  }
}
