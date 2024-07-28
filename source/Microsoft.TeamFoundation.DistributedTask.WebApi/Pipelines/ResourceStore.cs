// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ResourceStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ResourceStore : IResourceStore, IStepProvider
  {
    public ResourceStore()
      : this((IList<ServiceEndpoint>) null, (IList<SecureFile>) null, (IList<TaskAgentQueue>) null, (IList<VariableGroup>) null, (IList<BuildResource>) null, (IList<ContainerResource>) null, (IList<RepositoryResource>) null, (IList<PipelineResource>) null, (IList<TaskAgentPool>) null, (IList<PackageResource>) null, (IList<PersistedStage>) null)
    {
    }

    public ResourceStore(
      IList<ServiceEndpoint> endpoints = null,
      IList<SecureFile> files = null,
      IList<TaskAgentQueue> queues = null,
      IList<VariableGroup> variableGroups = null,
      IList<BuildResource> builds = null,
      IList<ContainerResource> containers = null,
      IList<RepositoryResource> repositories = null,
      IList<PipelineResource> pipelines = null,
      IList<TaskAgentPool> pools = null,
      IList<PackageResource> packages = null,
      IList<PersistedStage> stages = null)
      : this((IServiceEndpointStore) new ServiceEndpointStore(endpoints), (ISecureFileStore) new SecureFileStore(files), (IAgentQueueStore) new AgentQueueStore(queues), (IVariableGroupStore) new VariableGroupStore(variableGroups, (IVariableGroupResolver) null, false, Array.Empty<IVariableValueProvider>()), (IBuildStore) new BuildResourceStore((IEnumerable<BuildResource>) builds), (IContainerStore) new ContainerResourceStore((IEnumerable<ContainerResource>) containers), (IRepositoryStore) new RepositoryResourceStore((IEnumerable<RepositoryResource>) repositories), (IPipelineStore) new PipelineResourceStore((IEnumerable<PipelineResource>) pipelines), (IAgentPoolStore) new AgentPoolStore(pools), (IEnvironmentStore) new EnvironmentStore((IList<EnvironmentInstance>) null), (IPackageResourceStore) new PackageResourceStore((IEnumerable<PackageResource>) packages), (IPersistedStageStore) new PersistedStageStore(stages))
    {
    }

    public ResourceStore(
      IServiceEndpointStore endpointStore = null,
      ISecureFileStore fileStore = null,
      IAgentQueueStore queueStore = null,
      IVariableGroupStore variableGroupStore = null,
      IBuildStore buildStore = null,
      IContainerStore containerStore = null,
      IRepositoryStore repositoryStore = null,
      IPipelineStore pipelineStore = null,
      IAgentPoolStore poolStore = null,
      IEnvironmentStore environmentStore = null,
      IPackageResourceStore packageStore = null,
      IPersistedStageStore stageStore = null)
    {
      this.Builds = buildStore ?? (IBuildStore) new BuildResourceStore((BuildResource[]) null);
      this.Containers = containerStore ?? (IContainerStore) new ContainerResourceStore((ContainerResource[]) null);
      this.Endpoints = endpointStore ?? (IServiceEndpointStore) new ServiceEndpointStore((IList<ServiceEndpoint>) null);
      this.Files = fileStore ?? (ISecureFileStore) new SecureFileStore((IList<SecureFile>) null);
      this.Pipelines = pipelineStore ?? (IPipelineStore) new PipelineResourceStore((IEnumerable<PipelineResource>) null);
      this.Queues = queueStore ?? (IAgentQueueStore) new AgentQueueStore((IList<TaskAgentQueue>) null);
      this.Pools = poolStore ?? (IAgentPoolStore) new AgentPoolStore((IList<TaskAgentPool>) null);
      this.Repositories = repositoryStore ?? (IRepositoryStore) new RepositoryResourceStore((IEnumerable<RepositoryResource>) null);
      this.VariableGroups = variableGroupStore ?? (IVariableGroupStore) new VariableGroupStore((IList<VariableGroup>) null, (IVariableGroupResolver) null, false, Array.Empty<IVariableValueProvider>());
      this.Environments = environmentStore ?? (IEnvironmentStore) new EnvironmentStore((IList<EnvironmentInstance>) null);
      this.Packages = packageStore ?? (IPackageResourceStore) new PackageResourceStore((PackageResource[]) null);
      this.PersistedStages = stageStore ?? (IPersistedStageStore) new PersistedStageStore((IList<PersistedStage>) null);
    }

    public IBuildStore Builds { get; }

    public IPackageResourceStore Packages { get; }

    public IContainerStore Containers { get; }

    public IServiceEndpointStore Endpoints { get; }

    public IEnvironmentStore Environments { get; }

    public ISecureFileStore Files { get; }

    public IPersistedStageStore PersistedStages { get; }

    public IPipelineStore Pipelines { get; }

    public IAgentQueueStore Queues { get; }

    public IAgentPoolStore Pools { get; }

    public IRepositoryStore Repositories { get; }

    public IVariableGroupStore VariableGroups { get; }

    public PipelineResources GetAuthorizedResources()
    {
      PipelineResources authorizedResources = new PipelineResources();
      authorizedResources.Builds.AddRange<BuildResource, ISet<BuildResource>>(this.Builds.GetAll());
      authorizedResources.Packages.AddRange<PackageResource, ISet<PackageResource>>(this.Packages.GetAll());
      authorizedResources.Containers.AddRange<ContainerResource, ISet<ContainerResource>>(this.Containers.GetAll());
      authorizedResources.Endpoints.AddRange<ServiceEndpointReference, ISet<ServiceEndpointReference>>((IEnumerable<ServiceEndpointReference>) this.Endpoints.GetAuthorizedReferences());
      authorizedResources.Files.AddRange<SecureFileReference, ISet<SecureFileReference>>((IEnumerable<SecureFileReference>) this.Files.GetAuthorizedReferences());
      authorizedResources.Pipelines.AddRange<PipelineResource, ISet<PipelineResource>>(this.Pipelines.GetAll());
      authorizedResources.Queues.AddRange<AgentQueueReference, ISet<AgentQueueReference>>((IEnumerable<AgentQueueReference>) this.Queues.GetAuthorizedReferences());
      authorizedResources.Pools.AddRange<AgentPoolReference, ISet<AgentPoolReference>>((IEnumerable<AgentPoolReference>) this.Pools.GetAuthorizedReferences());
      authorizedResources.Repositories.AddRange<RepositoryResource, ISet<RepositoryResource>>(this.Repositories.GetAll());
      authorizedResources.VariableGroups.AddRange<VariableGroupReference, ISet<VariableGroupReference>>((IEnumerable<VariableGroupReference>) this.VariableGroups.GetAuthorizedReferences());
      authorizedResources.Environments.AddRange<EnvironmentReference, ISet<EnvironmentReference>>((IEnumerable<EnvironmentReference>) this.Environments.GetReferences());
      authorizedResources.PersistedStages.AddRange<PersistedStageReference, ISet<PersistedStageReference>>((IEnumerable<PersistedStageReference>) this.PersistedStages.GetAll());
      return authorizedResources;
    }

    public IList<TaskStep> GetPreSteps(IPipelineContext context, IReadOnlyList<JobStep> steps)
    {
      List<TaskStep> collection = new List<TaskStep>();
      if (context.EnvironmentVersion > 1 && context is PipelineExecutionContext)
      {
        collection.AddRangeIfRangeNotNull<TaskStep, List<TaskStep>>((IEnumerable<TaskStep>) this.VariableGroups.GetPreSteps(context, steps));
        collection.AddRangeIfRangeNotNull<TaskStep, List<TaskStep>>((IEnumerable<TaskStep>) this.Builds.GetPreSteps(context, steps));
        collection.AddRangeIfRangeNotNull<TaskStep, List<TaskStep>>((IEnumerable<TaskStep>) this.Packages.GetPreSteps(context, steps));
        collection.AddRangeIfRangeNotNull<TaskStep, List<TaskStep>>((IEnumerable<TaskStep>) this.Repositories.GetPreSteps(context, steps));
        collection.AddRangeIfRangeNotNull<TaskStep, List<TaskStep>>((IEnumerable<TaskStep>) this.Pipelines.GetPreSteps(context, steps));
      }
      return (IList<TaskStep>) collection;
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPostTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<TaskStep>> GetPreTargetTaskSteps(
      IPipelineContext context,
      IReadOnlyList<JobStep> steps)
    {
      return new Dictionary<Guid, List<TaskStep>>();
    }

    public Dictionary<Guid, List<string>> GetInputsToProvide(IPipelineContext context) => new Dictionary<Guid, List<string>>();

    public IList<TaskStep> GetPostSteps(IPipelineContext context, IReadOnlyList<JobStep> steps) => (IList<TaskStep>) new List<TaskStep>();

    public ServiceEndpoint GetEndpoint(Guid endpointId) => this.Endpoints.Get(new ServiceEndpointReference()
    {
      Id = endpointId
    });

    public ServiceEndpoint GetEndpoint(string endpointId)
    {
      ServiceEndpoint endpoint1 = (ServiceEndpoint) null;
      Guid result;
      if (Guid.TryParse(endpointId, out result))
        endpoint1 = this.GetEndpoint(result);
      if (endpoint1 == null)
      {
        IServiceEndpointStore endpoints = this.Endpoints;
        ServiceEndpointReference endpoint2 = new ServiceEndpointReference();
        endpoint2.Name = (ExpressionValue<string>) endpointId;
        endpoint1 = endpoints.Get(endpoint2);
      }
      return endpoint1;
    }

    public SecureFile GetFile(Guid fileId) => this.Files.Get(new SecureFileReference()
    {
      Id = fileId
    });

    public SecureFile GetFile(string fileId)
    {
      SecureFile file = (SecureFile) null;
      Guid result;
      if (Guid.TryParse(fileId, out result))
        file = this.GetFile(result);
      if (file == null)
      {
        ISecureFileStore files = this.Files;
        SecureFileReference reference = new SecureFileReference();
        reference.Name = (ExpressionValue<string>) fileId;
        file = files.Get(reference);
      }
      return file;
    }

    public TaskAgentQueue GetQueue(int queueId) => this.Queues.Get(new AgentQueueReference()
    {
      Id = queueId
    });

    public TaskAgentQueue GetQueue(string queueId)
    {
      TaskAgentQueue queue = (TaskAgentQueue) null;
      int result;
      if (int.TryParse(queueId, out result))
        queue = this.GetQueue(result);
      if (queue == null)
      {
        IAgentQueueStore queues = this.Queues;
        AgentQueueReference reference = new AgentQueueReference();
        reference.Name = (ExpressionValue<string>) queueId;
        queue = queues.Get(reference);
      }
      return queue;
    }

    public TaskAgentPool GetPool(int poolId) => this.Pools.Get(new AgentPoolReference()
    {
      Id = poolId
    });

    public TaskAgentPool GetPool(string poolName)
    {
      IAgentPoolStore pools = this.Pools;
      AgentPoolReference reference = new AgentPoolReference();
      reference.Name = (ExpressionValue<string>) poolName;
      return pools.Get(reference);
    }

    public VariableGroup GetVariableGroup(int groupId) => this.VariableGroups.Get(new VariableGroupReference()
    {
      Id = groupId
    });

    public VariableGroup GetVariableGroup(string groupId)
    {
      VariableGroup variableGroup = (VariableGroup) null;
      int result;
      if (int.TryParse(groupId, out result))
        variableGroup = this.GetVariableGroup(result);
      if (variableGroup == null)
      {
        IVariableGroupStore variableGroups = this.VariableGroups;
        VariableGroupReference queue = new VariableGroupReference();
        queue.Name = (ExpressionValue<string>) groupId;
        variableGroup = variableGroups.Get(queue);
      }
      return variableGroup;
    }

    public bool ResolveStep(
      IPipelineContext context,
      JobStep step,
      out IList<TaskStep> resolvedSteps)
    {
      resolvedSteps = (IList<TaskStep>) new List<TaskStep>();
      return context.EnvironmentVersion > 1 && context is PipelineExecutionContext && this.Pipelines.ResolveStep(context, step, out resolvedSteps);
    }
  }
}
