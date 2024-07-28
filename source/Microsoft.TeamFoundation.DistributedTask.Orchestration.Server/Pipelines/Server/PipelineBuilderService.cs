// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.PipelineBuilderService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Expressions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  public class PipelineBuilderService : IPipelineBuilderService, IVssFrameworkService
  {
    internal TaskOverrideResolver2 TaskOverrideResolver2 { get; } = new TaskOverrideResolver2();

    public TaskBuildConfigExceptionResolver TaskBuildConfigExceptionResolver { get; } = new TaskBuildConfigExceptionResolver();

    public PipelineBuilder GetBuilder(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int definitionId,
      PipelineResources authorizedResources,
      bool authorizeNewResources = false,
      bool evaluateCounters = false,
      IDictionary<string, int> counters = null)
    {
      return this.GetBuilder(requestContext, projectId, planType, 0, definitionId, authorizedResources, authorizeNewResources, evaluateCounters, counters);
    }

    public PipelineBuilder GetBuilder(IVssRequestContext requestContext, TaskOrchestrationPlan plan)
    {
      PipelineEnvironment environment = plan.GetEnvironment<PipelineEnvironment>();
      PlanSecretStore planSecretStore = new PlanSecretStore(requestContext, plan.PlanId);
      IVssRequestContext requestContext1 = requestContext;
      Guid scopeIdentifier = plan.ScopeIdentifier;
      string planType = plan.PlanType;
      int version = plan.Version;
      TaskOrchestrationOwner definition = plan.Definition;
      int id = definition != null ? definition.Id : 0;
      PipelineResources resources = environment.Resources;
      IDictionary<string, int> counters = environment.Counters;
      PipelineBuilder builder = this.GetBuilder(requestContext1, scopeIdentifier, planType, version, id, resources, false, true, counters);
      builder.EnvironmentVersion = environment.Version;
      if (environment.Version < 2)
      {
        if (environment.Resources.VariableGroups.Count > 0)
        {
          foreach (VariableGroupReference variableGroup in (IEnumerable<VariableGroupReference>) environment.Resources.VariableGroups)
            builder.UserVariables.Add((IVariable) variableGroup);
        }
        foreach (Variable variable in environment.Variables.Select<KeyValuePair<string, VariableValue>, Variable>((Func<KeyValuePair<string, VariableValue>, Variable>) (x => new Variable()
        {
          Name = x.Key,
          Secret = x.Value.IsSecret,
          Value = x.Value.Value
        })))
        {
          if (variable.Secret)
            variable.Value = planSecretStore.GetVariable(variable.Name);
          builder.UserVariables.Add((IVariable) variable);
        }
      }
      else
      {
        foreach (IVariable userVariable in (IEnumerable<IVariable>) environment.UserVariables)
        {
          if (userVariable is Variable variable)
          {
            if (variable.Secret)
            {
              variable = variable.Clone();
              variable.Value = planSecretStore.GetVariable(variable.Name);
            }
            builder.UserVariables.Add((IVariable) variable);
          }
          else
            builder.UserVariables.Add(userVariable);
        }
        foreach (KeyValuePair<string, VariableValue> keyValuePair in environment.SystemVariables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x => !builder.SystemVariables.ContainsKey(x.Key))))
          builder.SystemVariables[keyValuePair.Key] = keyValuePair.Value;
      }
      if (plan.Definition != null)
        builder.SystemVariables[WellKnownDistributedTaskVariables.DefinitionName] = (VariableValue) plan.Definition.Name;
      builder.SystemVariables[WellKnownDistributedTaskVariables.PlanId] = (VariableValue) plan.PlanId.ToString("D");
      builder.SystemVariables[WellKnownDistributedTaskVariables.TimelineId] = (VariableValue) plan.Timeline.Id.ToString("D");
      return builder;
    }

    public PipelineBuilder GetBuilder(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int planVersion,
      int definitionId,
      Guid planId,
      PipelineEnvironment environment)
    {
      PipelineBuilder builder = this.GetBuilder(requestContext, projectId, planType, planVersion, definitionId, environment.Resources, false, true, environment.Counters);
      builder.EnvironmentVersion = environment.Version;
      if (environment.Version < 2)
      {
        foreach (VariableGroupReference variableGroup in (IEnumerable<VariableGroupReference>) environment.Resources.VariableGroups)
          builder.UserVariables.Add((IVariable) variableGroup);
        foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) environment.Variables)
          builder.UserVariables.Add((IVariable) new Variable()
          {
            Name = variable.Key,
            Secret = variable.Value.IsSecret,
            Value = variable.Value.Value
          });
      }
      else
      {
        builder.UserVariables.AddRange<IVariable, IList<IVariable>>((IEnumerable<IVariable>) environment.UserVariables);
        foreach (KeyValuePair<string, VariableValue> systemVariable in (IEnumerable<KeyValuePair<string, VariableValue>>) environment.SystemVariables)
          builder.SystemVariables[systemVariable.Key] = systemVariable.Value;
      }
      builder.SystemVariables[WellKnownDistributedTaskVariables.PlanId] = (VariableValue) planId.ToString("D");
      return builder;
    }

    public ICounterStore GetCounterStore(
      IVssRequestContext requestContext,
      bool evaluateCounters = false,
      IDictionary<string, int> counters = null)
    {
      return (ICounterStore) new CounterStore(counters, evaluateCounters ? (ICounterResolver) new CounterResolver(requestContext) : (ICounterResolver) null);
    }

    public IPackageStore GetPackageStore(IVssRequestContext requestContext) => (IPackageStore) new PackageStore(resolver: (IPackageResolver) new PackageResolver(requestContext));

    public PipelineResourceStore GetPipelineStore(
      IVssRequestContext requestContext,
      PipelineResources authorizedResources,
      Guid projectId,
      bool useSystemStepsDecorator)
    {
      ArtifactResolver artifactResolver = new ArtifactResolver(requestContext, projectId);
      return new PipelineResourceStore((IEnumerable<PipelineResource>) authorizedResources?.Pipelines, (IArtifactResolver) artifactResolver, true, useSystemStepsDecorator);
    }

    public IResourceStore GetResourceStore(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResources authorizedResources,
      bool authorizeNewResources = false)
    {
      return this.GetResourceStore(requestContext, projectId, authorizedResources, authorizeNewResources, false, false);
    }

    private IResourceStore GetResourceStore(
      IVssRequestContext requestContext,
      Guid projectId,
      PipelineResources authorizedResources,
      bool authorizeNewResources,
      bool useSystemStepsDecorator,
      bool includeCheckoutOptions)
    {
      using (new MethodScope(requestContext, "TaskHub", nameof (GetResourceStore)))
      {
        authorizeNewResources = requestContext.IsFeatureEnabled("DistributedTask.EnableJustInTimeAuthorization") | authorizeNewResources;
        IVssRequestContext requestContext1 = requestContext;
        IVssRequestContext requestContext2 = requestContext.Elevate();
        bool flag = false;
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          RegistryQuery query = (RegistryQuery) (RegistryKeys.RegistrySettingsPath + "AutoAuthorizeResources/" + projectId.ToString("D"));
          flag = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in query);
        }
        if (flag)
        {
          authorizeNewResources = true;
          requestContext1 = requestContext2;
        }
        ServiceEndpointStore endpointStore = new ServiceEndpointStore((IList<ServiceEndpoint>) null, (IServiceEndpointResolver) new ServiceEndpointResolver(requestContext1, projectId, authorizeNewResources));
        if (authorizedResources != null)
        {
          ISet<ServiceEndpointReference> endpoints = authorizedResources.Endpoints;
          if (endpoints != null)
            endpoints.ForEach<ServiceEndpointReference>(new Action<ServiceEndpointReference>(endpointStore.Authorize));
        }
        SecureFileStore fileStore = new SecureFileStore((IList<SecureFile>) null, (ISecureFileResolver) new SecureFileResolver(requestContext1, projectId, authorizeNewResources), true);
        if (authorizedResources != null)
        {
          ISet<SecureFileReference> files = authorizedResources.Files;
          if (files != null)
            files.ForEach<SecureFileReference>(new Action<SecureFileReference>(fileStore.Authorize));
        }
        AgentQueueStore queueStore = new AgentQueueStore(AgentQueueResolver.Resolve(requestContext2, projectId, (ICollection<AgentQueueReference>) authorizedResources?.Queues), authorizeNewResources ? (IAgentQueueResolver) new AgentQueueResolver(requestContext1, projectId) : (IAgentQueueResolver) null);
        AgentPoolStore poolStore = new AgentPoolStore(AgentPoolResolver.Resolve(requestContext2, (ICollection<AgentPoolReference>) authorizedResources?.Pools), (IAgentPoolResolver) new AgentPoolResolver(requestContext2));
        VstsVariableGroupValueProvider groupValueProvider = new VstsVariableGroupValueProvider();
        AzureKeyVaultValueProvider vaultValueProvider = new AzureKeyVaultValueProvider(requestContext2, projectId);
        VariableGroupStore variableGroupStore = new VariableGroupStore((IList<VariableGroup>) null, (IVariableGroupResolver) new VariableGroupResolver(requestContext1, projectId, authorizeNewResources), true, new IVariableValueProvider[2]
        {
          (IVariableValueProvider) groupValueProvider,
          (IVariableValueProvider) vaultValueProvider
        });
        if (authorizedResources != null)
        {
          ISet<VariableGroupReference> variableGroups = authorizedResources.VariableGroups;
          if (variableGroups != null)
            variableGroups.ForEach<VariableGroupReference>(new Action<VariableGroupReference>(variableGroupStore.Authorize));
        }
        IArtifactResolver resolver1 = (IArtifactResolver) new ArtifactResolver(requestContext, projectId, (IServiceEndpointStore) endpointStore);
        BuildResourceStore buildResourceStore = new BuildResourceStore((IEnumerable<BuildResource>) authorizedResources?.Builds, resolver1);
        PackageResourceStore packageStore = new PackageResourceStore((IEnumerable<PackageResource>) authorizedResources?.Packages, resolver1);
        ContainerResourceStore containerResourceStore = new ContainerResourceStore((IEnumerable<ContainerResource>) authorizedResources?.Containers);
        PipelineResourceStore pipelineStore = this.GetPipelineStore(requestContext, authorizedResources, projectId, useSystemStepsDecorator);
        RepositoryResourceStore repositoryResourceStore = new RepositoryResourceStore((IEnumerable<RepositoryResource>) authorizedResources?.Repositories, useSystemStepsDecorator, includeCheckoutOptions);
        IList<EnvironmentInstance> environments1 = EnvironmentResolver.Resolve(requestContext2, projectId, (ICollection<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference>) authorizedResources?.Environments);
        EnvironmentResolver resolver2;
        if (!authorizeNewResources)
        {
          if (authorizedResources != null)
          {
            ISet<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference> environments2 = authorizedResources.Environments;
            int? nullable = environments2 != null ? new int?(environments2.Count<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference>()) : new int?();
            int num = 0;
            if (nullable.GetValueOrDefault() == num & nullable.HasValue)
              goto label_18;
          }
          resolver2 = (EnvironmentResolver) null;
          goto label_19;
        }
label_18:
        resolver2 = new EnvironmentResolver(requestContext, projectId);
label_19:
        EnvironmentStore environmentStore = new EnvironmentStore(environments1, (IEnvironmentResolver) resolver2);
        PersistedStageResolver persistedStageResolver = new PersistedStageResolver(requestContext, projectId);
        IList<PersistedStage> stages = persistedStageResolver.Resolve((ICollection<PersistedStageReference>) authorizedResources?.PersistedStages);
        PersistedStageResolver resolver3;
        if (authorizedResources != null)
        {
          int? count = authorizedResources.PersistedStages?.Count;
          int num = 0;
          if (count.GetValueOrDefault() == num & count.HasValue)
          {
            resolver3 = persistedStageResolver;
            goto label_23;
          }
        }
        resolver3 = (PersistedStageResolver) null;
label_23:
        PersistedStageStore stageStore = new PersistedStageStore(stages, (IPersistedStageResolver) resolver3);
        return (IResourceStore) new ResourceStore((IServiceEndpointStore) endpointStore, (ISecureFileStore) fileStore, (IAgentQueueStore) queueStore, (IVariableGroupStore) variableGroupStore, (IBuildStore) buildResourceStore, (IContainerStore) containerResourceStore, (IRepositoryStore) repositoryResourceStore, (IPipelineStore) pipelineStore, (IAgentPoolStore) poolStore, (IEnvironmentStore) environmentStore, (IPackageResourceStore) packageStore, (IPersistedStageStore) stageStore);
      }
    }

    public ITaskStore GetTaskStore(IVssRequestContext requestContext)
    {
      using (new MethodScope(requestContext, "TaskHub", nameof (GetTaskStore)))
      {
        IList<TaskDefinition> taskDefinitions = requestContext.GetService<IDistributedTaskPoolService>().GetTaskDefinitions(requestContext);
        TaskResolver taskResolver = new TaskResolver(requestContext);
        IDictionary<Tuple<Guid, string>, IList<string>> dictionary1 = this.TaskBuildConfigExceptionResolver.TaskBuildConfigExceptions(requestContext);
        IDictionary<Guid, IDictionary<string, string>> dictionary2 = TaskOverrideResolver2.TaskOverrides(requestContext);
        TaskResolver resolver = taskResolver;
        IDictionary<Guid, IDictionary<string, string>> overrideLookup = dictionary2;
        IDictionary<Tuple<Guid, string>, IList<string>> buildConfigExceptionLookup = dictionary1;
        Func<string, bool> featureCallback = new Func<string, bool>(((VssRequestContextExtensions) requestContext).IsFeatureEnabled);
        return (ITaskStore) new TaskStore((IEnumerable<TaskDefinition>) taskDefinitions, (ITaskResolver) resolver, overrideLookup, buildConfigExceptionLookup, featureCallback);
      }
    }

    public ITaskTemplateStore GetTaskTemplateStore(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return (ITaskTemplateStore) new TaskTemplateStore((IList<ITaskTemplateResolver>) new TaskGroupTemplateResolver[1]
      {
        new TaskGroupTemplateResolver(requestContext, projectId)
      });
    }

    public PipelineBuilder GetBuilder(
      IVssRequestContext requestContext,
      Guid projectId,
      string planType,
      int planVersion,
      int definitionId,
      PipelineResources authorizedResources,
      bool authorizeNewResources,
      bool evaluateCounters,
      IDictionary<string, int> counters)
    {
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      ICollectionPreferencesService service2 = requestContext.GetService<ICollectionPreferencesService>();
      EvaluationOptions expressionOptions = new EvaluationOptions()
      {
        MaxMemory = service1.GetValue<int>(requestContext, in RegistryKeys.PipelineParserMaxResultSize, false, 1048576),
        TimeZone = service2.GetCollectionTimeZone(requestContext) ?? TimeZoneInfo.Local
      };
      bool useSystemStepsDecorator = requestContext.IsFeatureEnabled("DistributedTask.YamlSystemStepsDecorator");
      bool includeCheckoutOptions = requestContext.IsFeatureEnabled("DistributedTask.IncludeCheckoutOptions");
      ICounterStore counterStore = this.GetCounterStore(requestContext, evaluateCounters, counters);
      PipelineIdGenerator idGenerator = new PipelineIdGenerator(planVersion > 0 && planVersion < 4);
      IPackageStore packageStore = this.GetPackageStore(requestContext);
      IResourceStore resourceStore = this.GetResourceStore(requestContext, projectId, authorizedResources, authorizeNewResources, useSystemStepsDecorator, includeCheckoutOptions);
      ITaskStore taskStore = this.GetTaskStore(requestContext);
      ITaskTemplateStore taskTemplateStore = this.GetTaskTemplateStore(requestContext, projectId);
      List<IStepProvider> stepProviderList = new List<IStepProvider>();
      if (useSystemStepsDecorator)
        stepProviderList.Add((IStepProvider) new SystemPipelineDecorator(requestContext));
      if (PublishPipelineMetadataStepProvider.IsPublishPipelineMetadataProjectSettingEnabled(requestContext, projectId))
        stepProviderList.Add((IStepProvider) new PublishPipelineMetadataStepProvider(requestContext));
      this.AddPipelineDecoratorsForPlanType(requestContext, planType, (IList<IStepProvider>) stepProviderList);
      List<IPhaseProvider> phaseProviders = new List<IPhaseProvider>();
      IDisposableReadOnlyList<IPhaseProviderExtension> extensions = requestContext.GetExtensions<IPhaseProviderExtension>(ExtensionLifetime.Service);
      if (extensions != null && extensions.Any<IPhaseProviderExtension>())
        phaseProviders.AddRange((IEnumerable<IPhaseProvider>) extensions);
      CultureInfo culture = requestContext.ServiceHost.GetCulture(requestContext);
      string absoluteUri1 = PipelineBuilderService.GetVssUrl(requestContext, Guid.Empty).AbsoluteUri;
      string absoluteUri2 = PipelineBuilderService.GetVssUrl(requestContext, ServiceInstanceTypes.TFS).AbsoluteUri;
      string str = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, expressionOptions.TimeZone).ToString(ExpressionConstants.DateTimeFormat, (IFormatProvider) CultureInfo.InvariantCulture);
      PipelineBuilder builder = new PipelineBuilder(expressionOptions, counterStore, packageStore, resourceStore, (IList<IStepProvider>) stepProviderList, taskStore, taskTemplateStore, (IPipelineIdGenerator) idGenerator, (IList<IPhaseProvider>) phaseProviders, PipelineFeatureFlagDictionaryFactory.Create(requestContext));
      builder.EnvironmentVersion = this.GetEnvironmentVersion(requestContext);
      builder.SystemVariables[WellKnownDistributedTaskVariables.System] = (VariableValue) planType.ToLowerInvariant();
      builder.SystemVariables[WellKnownDistributedTaskVariables.HostType] = (VariableValue) planType.ToLowerInvariant();
      builder.SystemVariables[WellKnownDistributedTaskVariables.ServerType] = (VariableValue) (requestContext.ExecutionEnvironment.IsHostedDeployment ? "Hosted" : "OnPremises");
      builder.SystemVariables[WellKnownDistributedTaskVariables.Culture] = (VariableValue) culture.Name;
      builder.SystemVariables[WellKnownDistributedTaskVariables.CollectionId] = (VariableValue) requestContext.ServiceHost.InstanceId.ToString("D");
      builder.SystemVariables[WellKnownDistributedTaskVariables.CollectionUrl] = (VariableValue) absoluteUri1;
      builder.SystemVariables[WellKnownDistributedTaskVariables.TFCollectionUrl] = (VariableValue) absoluteUri2;
      builder.SystemVariables[WellKnownDistributedTaskVariables.TaskDefinitionsUrl] = (VariableValue) absoluteUri2;
      builder.SystemVariables[WellKnownDistributedTaskVariables.PipelineStartTime] = (VariableValue) str;
      if (projectId != Guid.Empty)
      {
        IProjectService service3 = requestContext.GetService<IProjectService>();
        builder.SystemVariables[WellKnownDistributedTaskVariables.TeamProject] = (VariableValue) service3.GetProjectName(requestContext, projectId);
        builder.SystemVariables[WellKnownDistributedTaskVariables.TeamProjectId] = (VariableValue) projectId.ToString("D");
      }
      if (definitionId != 0)
        builder.SystemVariables[WellKnownDistributedTaskVariables.DefinitionId] = (VariableValue) definitionId.ToString("D");
      return builder;
    }

    private void AddPipelineDecoratorsForPlanType(
      IVssRequestContext requestContext,
      string planType,
      IList<IStepProvider> pipelineDecorators)
    {
      IDisposableReadOnlyList<IPipelineDecoratorLoaderExtension> extensions = requestContext.GetExtensions<IPipelineDecoratorLoaderExtension>(ExtensionLifetime.Service);
      IPipelineDecorator pipelineDecorator1;
      if (extensions == null)
      {
        pipelineDecorator1 = (IPipelineDecorator) null;
      }
      else
      {
        IEnumerable<IPipelineDecoratorLoaderExtension> source1 = extensions.Where<IPipelineDecoratorLoaderExtension>((Func<IPipelineDecoratorLoaderExtension, bool>) (ext => string.Equals(ext?.PlanType, planType, StringComparison.OrdinalIgnoreCase)));
        if (source1 == null)
        {
          pipelineDecorator1 = (IPipelineDecorator) null;
        }
        else
        {
          IPipelineDecorator pipelineDecorator2;
          IEnumerable<IPipelineDecorator> source2 = source1.Select<IPipelineDecoratorLoaderExtension, IPipelineDecorator>((Func<IPipelineDecoratorLoaderExtension, IPipelineDecorator>) (ext => ext == null || !ext.TryGetPipelineDecorator(requestContext, out pipelineDecorator2) ? (IPipelineDecorator) null : pipelineDecorator2));
          if (source2 == null)
          {
            pipelineDecorator1 = (IPipelineDecorator) null;
          }
          else
          {
            IEnumerable<IPipelineDecorator> source3 = source2.Where<IPipelineDecorator>((Func<IPipelineDecorator, bool>) (d => d != null));
            pipelineDecorator1 = source3 != null ? source3.FirstOrDefault<IPipelineDecorator>() : (IPipelineDecorator) null;
          }
        }
      }
      IPipelineDecorator pipelineDecorator3 = pipelineDecorator1;
      if (pipelineDecorator3 == null || pipelineDecorators == null)
        return;
      pipelineDecorators.Add((IStepProvider) pipelineDecorator3);
    }

    private int GetEnvironmentVersion(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("DistributedTask.NewPipelineEnvironment") ? 2 : 1;

    private static Uri GetVssUrl(IVssRequestContext requestContext, Guid serviceAreaIdentifier) => new Uri(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, serviceAreaIdentifier, AccessMappingConstants.ClientAccessMappingMoniker), UriKind.Absolute);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
