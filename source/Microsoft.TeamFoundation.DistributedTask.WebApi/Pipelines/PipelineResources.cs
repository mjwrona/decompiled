// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineResources
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PipelineResources
  {
    [DataMember(Name = "Builds", EmitDefaultValue = false)]
    private HashSet<BuildResource> m_builds;
    [DataMember(Name = "Containers", EmitDefaultValue = false)]
    private HashSet<ContainerResource> m_containers;
    [DataMember(Name = "Packages", EmitDefaultValue = false)]
    private HashSet<PackageResource> m_packages;
    [DataMember(Name = "Endpoints", EmitDefaultValue = false)]
    private HashSet<ServiceEndpointReference> m_endpoints;
    [DataMember(Name = "Files", EmitDefaultValue = false)]
    private HashSet<SecureFileReference> m_files;
    [DataMember(Name = "PersistedStages", EmitDefaultValue = false)]
    private HashSet<PersistedStageReference> m_stages;
    [DataMember(Name = "Pipelines", EmitDefaultValue = false)]
    private HashSet<PipelineResource> m_pipelines;
    [DataMember(Name = "Queues", EmitDefaultValue = false)]
    private HashSet<AgentQueueReference> m_queues;
    [DataMember(Name = "Pools", EmitDefaultValue = false)]
    private HashSet<AgentPoolReference> m_pools;
    [DataMember(Name = "Repositories", EmitDefaultValue = false)]
    private HashSet<RepositoryResource> m_repositories;
    [DataMember(Name = "VariableGroups", EmitDefaultValue = false)]
    private HashSet<VariableGroupReference> m_variableGroups;
    [DataMember(Name = "Environments", EmitDefaultValue = false)]
    private HashSet<EnvironmentReference> m_environments;
    [DataMember(Name = "WebHookResource", EmitDefaultValue = false)]
    private HashSet<WebhookResource> m_webhookResources;

    public PipelineResources()
    {
    }

    private PipelineResources(PipelineResources resourcesToCopy)
    {
      HashSet<BuildResource> builds = resourcesToCopy.m_builds;
      // ISSUE: explicit non-virtual call
      if ((builds != null ? (__nonvirtual (builds.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_builds = new HashSet<BuildResource>(resourcesToCopy.m_builds.Select<BuildResource, BuildResource>((Func<BuildResource, BuildResource>) (x => x.Clone())), (IEqualityComparer<BuildResource>) new ResourceComparer());
      HashSet<ContainerResource> containers = resourcesToCopy.m_containers;
      // ISSUE: explicit non-virtual call
      if ((containers != null ? (__nonvirtual (containers.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_containers = new HashSet<ContainerResource>(resourcesToCopy.m_containers.Select<ContainerResource, ContainerResource>((Func<ContainerResource, ContainerResource>) (x => x.Clone())), (IEqualityComparer<ContainerResource>) new ResourceComparer());
      HashSet<ServiceEndpointReference> endpoints = resourcesToCopy.m_endpoints;
      // ISSUE: explicit non-virtual call
      if ((endpoints != null ? (__nonvirtual (endpoints.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_endpoints = new HashSet<ServiceEndpointReference>(resourcesToCopy.m_endpoints.Select<ServiceEndpointReference, ServiceEndpointReference>((Func<ServiceEndpointReference, ServiceEndpointReference>) (x => x.Clone())), (IEqualityComparer<ServiceEndpointReference>) new PipelineResources.EndpointComparer());
      HashSet<EnvironmentReference> environments = resourcesToCopy.m_environments;
      // ISSUE: explicit non-virtual call
      if ((environments != null ? (__nonvirtual (environments.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_environments = new HashSet<EnvironmentReference>(resourcesToCopy.m_environments.Select<EnvironmentReference, EnvironmentReference>((Func<EnvironmentReference, EnvironmentReference>) (x => x.Clone())), (IEqualityComparer<EnvironmentReference>) new PipelineResources.EnvironmentComparer());
      HashSet<SecureFileReference> files = resourcesToCopy.m_files;
      // ISSUE: explicit non-virtual call
      if ((files != null ? (__nonvirtual (files.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_files = new HashSet<SecureFileReference>(resourcesToCopy.m_files.Select<SecureFileReference, SecureFileReference>((Func<SecureFileReference, SecureFileReference>) (x => x.Clone())), (IEqualityComparer<SecureFileReference>) new PipelineResources.FileComparer());
      HashSet<PersistedStageReference> stages = resourcesToCopy.m_stages;
      // ISSUE: explicit non-virtual call
      if ((stages != null ? (__nonvirtual (stages.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_stages = new HashSet<PersistedStageReference>(resourcesToCopy.m_stages.Select<PersistedStageReference, PersistedStageReference>((Func<PersistedStageReference, PersistedStageReference>) (x => x.Clone())), (IEqualityComparer<PersistedStageReference>) new PipelineResources.PersistedStageComparer());
      HashSet<PipelineResource> pipelines = resourcesToCopy.m_pipelines;
      // ISSUE: explicit non-virtual call
      if ((pipelines != null ? (__nonvirtual (pipelines.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_pipelines = new HashSet<PipelineResource>(resourcesToCopy.m_pipelines.Select<PipelineResource, PipelineResource>((Func<PipelineResource, PipelineResource>) (x => x.Clone())), (IEqualityComparer<PipelineResource>) new ResourceComparer());
      HashSet<AgentQueueReference> queues = resourcesToCopy.m_queues;
      // ISSUE: explicit non-virtual call
      if ((queues != null ? (__nonvirtual (queues.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_queues = new HashSet<AgentQueueReference>(resourcesToCopy.m_queues.Select<AgentQueueReference, AgentQueueReference>((Func<AgentQueueReference, AgentQueueReference>) (x => x.Clone())), (IEqualityComparer<AgentQueueReference>) new PipelineResources.QueueComparer());
      HashSet<AgentPoolReference> pools = resourcesToCopy.m_pools;
      // ISSUE: explicit non-virtual call
      if ((pools != null ? (__nonvirtual (pools.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_pools = new HashSet<AgentPoolReference>(resourcesToCopy.m_pools.Select<AgentPoolReference, AgentPoolReference>((Func<AgentPoolReference, AgentPoolReference>) (x => x.Clone())), (IEqualityComparer<AgentPoolReference>) new PipelineResources.PoolComparer());
      HashSet<RepositoryResource> repositories = resourcesToCopy.m_repositories;
      // ISSUE: explicit non-virtual call
      if ((repositories != null ? (__nonvirtual (repositories.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_repositories = new HashSet<RepositoryResource>(resourcesToCopy.m_repositories.Select<RepositoryResource, RepositoryResource>((Func<RepositoryResource, RepositoryResource>) (x => x.Clone())), (IEqualityComparer<RepositoryResource>) new ResourceComparer());
      HashSet<VariableGroupReference> variableGroups = resourcesToCopy.m_variableGroups;
      // ISSUE: explicit non-virtual call
      if ((variableGroups != null ? (__nonvirtual (variableGroups.Count) > 0 ? 1 : 0) : 0) != 0)
        this.m_variableGroups = new HashSet<VariableGroupReference>(resourcesToCopy.m_variableGroups.Select<VariableGroupReference, VariableGroupReference>((Func<VariableGroupReference, VariableGroupReference>) (x => x.Clone())), (IEqualityComparer<VariableGroupReference>) new PipelineResources.VariableGroupComparer());
      HashSet<PackageResource> packages = resourcesToCopy.m_packages;
      // ISSUE: explicit non-virtual call
      if ((packages != null ? (__nonvirtual (packages.Count) > 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_packages = new HashSet<PackageResource>(resourcesToCopy.m_packages.Select<PackageResource, PackageResource>((Func<PackageResource, PackageResource>) (x => x.Clone())), (IEqualityComparer<PackageResource>) new ResourceComparer());
    }

    public int Count
    {
      get
      {
        HashSet<BuildResource> builds = this.m_builds;
        int count1 = builds != null ? __nonvirtual (builds.Count) : 0;
        HashSet<ContainerResource> containers = this.m_containers;
        int count2 = containers != null ? __nonvirtual (containers.Count) : 0;
        int num1 = count1 + count2;
        HashSet<ServiceEndpointReference> endpoints = this.m_endpoints;
        int count3 = endpoints != null ? __nonvirtual (endpoints.Count) : 0;
        int num2 = num1 + count3;
        HashSet<EnvironmentReference> environments = this.m_environments;
        int count4 = environments != null ? __nonvirtual (environments.Count) : 0;
        int num3 = num2 + count4;
        HashSet<SecureFileReference> files = this.m_files;
        int count5 = files != null ? __nonvirtual (files.Count) : 0;
        int num4 = num3 + count5;
        HashSet<PipelineResource> pipelines = this.m_pipelines;
        int count6 = pipelines != null ? __nonvirtual (pipelines.Count) : 0;
        int num5 = num4 + count6;
        HashSet<AgentQueueReference> queues = this.m_queues;
        int count7 = queues != null ? __nonvirtual (queues.Count) : 0;
        int num6 = num5 + count7;
        HashSet<AgentPoolReference> pools = this.m_pools;
        int count8 = pools != null ? __nonvirtual (pools.Count) : 0;
        int num7 = num6 + count8;
        HashSet<RepositoryResource> repositories = this.m_repositories;
        int count9 = repositories != null ? __nonvirtual (repositories.Count) : 0;
        int num8 = num7 + count9;
        HashSet<PersistedStageReference> stages = this.m_stages;
        int count10 = stages != null ? __nonvirtual (stages.Count) : 0;
        int num9 = num8 + count10;
        HashSet<VariableGroupReference> variableGroups = this.m_variableGroups;
        int count11 = variableGroups != null ? __nonvirtual (variableGroups.Count) : 0;
        int num10 = num9 + count11;
        HashSet<PackageResource> packages = this.m_packages;
        int count12 = packages != null ? __nonvirtual (packages.Count) : 0;
        return num10 + count12;
      }
    }

    public IEnumerable<ResourceReference> GetSecurableResources(bool includeRepositories)
    {
      IEnumerable<ResourceReference>[] resourceReferencesArray = new IEnumerable<ResourceReference>[8]
      {
        (IEnumerable<ResourceReference>) this.m_endpoints,
        (IEnumerable<ResourceReference>) this.m_environments,
        (IEnumerable<ResourceReference>) this.m_files,
        (IEnumerable<ResourceReference>) this.m_queues,
        (IEnumerable<ResourceReference>) this.m_pools,
        (IEnumerable<ResourceReference>) this.m_stages,
        (IEnumerable<ResourceReference>) this.m_variableGroups,
        (IEnumerable<ResourceReference>) this.GetSecurableRepositories(includeRepositories)
      };
      for (int index = 0; index < resourceReferencesArray.Length; ++index)
      {
        IEnumerable<ResourceReference> resourceReferences = resourceReferencesArray[index];
        if (resourceReferences != null)
        {
          foreach (ResourceReference securableResource in resourceReferences)
          {
            if (securableResource != null)
              yield return securableResource;
          }
        }
      }
      resourceReferencesArray = (IEnumerable<ResourceReference>[]) null;
    }

    public ISet<BuildResource> Builds
    {
      get
      {
        if (this.m_builds == null)
          this.m_builds = new HashSet<BuildResource>((IEqualityComparer<BuildResource>) new ResourceComparer());
        return (ISet<BuildResource>) this.m_builds;
      }
    }

    public ISet<ContainerResource> Containers
    {
      get
      {
        if (this.m_containers == null)
          this.m_containers = new HashSet<ContainerResource>((IEqualityComparer<ContainerResource>) new ResourceComparer());
        return (ISet<ContainerResource>) this.m_containers;
      }
    }

    public ISet<PackageResource> Packages
    {
      get
      {
        if (this.m_packages == null)
          this.m_packages = new HashSet<PackageResource>((IEqualityComparer<PackageResource>) new ResourceComparer());
        return (ISet<PackageResource>) this.m_packages;
      }
    }

    public ISet<ServiceEndpointReference> Endpoints
    {
      get
      {
        if (this.m_endpoints == null)
          this.m_endpoints = new HashSet<ServiceEndpointReference>((IEqualityComparer<ServiceEndpointReference>) new PipelineResources.EndpointComparer());
        return (ISet<ServiceEndpointReference>) this.m_endpoints;
      }
    }

    public ISet<EnvironmentReference> Environments
    {
      get
      {
        if (this.m_environments == null)
          this.m_environments = new HashSet<EnvironmentReference>((IEqualityComparer<EnvironmentReference>) new PipelineResources.EnvironmentComparer());
        return (ISet<EnvironmentReference>) this.m_environments;
      }
    }

    public ISet<SecureFileReference> Files
    {
      get
      {
        if (this.m_files == null)
          this.m_files = new HashSet<SecureFileReference>((IEqualityComparer<SecureFileReference>) new PipelineResources.FileComparer());
        return (ISet<SecureFileReference>) this.m_files;
      }
    }

    public ISet<PersistedStageReference> PersistedStages => (ISet<PersistedStageReference>) this.m_stages ?? (ISet<PersistedStageReference>) (this.m_stages = new HashSet<PersistedStageReference>((IEqualityComparer<PersistedStageReference>) new PipelineResources.PersistedStageComparer()));

    public ISet<PipelineResource> Pipelines
    {
      get
      {
        if (this.m_pipelines == null)
          this.m_pipelines = new HashSet<PipelineResource>((IEqualityComparer<PipelineResource>) new ResourceComparer());
        return (ISet<PipelineResource>) this.m_pipelines;
      }
    }

    public ISet<AgentQueueReference> Queues
    {
      get
      {
        if (this.m_queues == null)
          this.m_queues = new HashSet<AgentQueueReference>((IEqualityComparer<AgentQueueReference>) new PipelineResources.QueueComparer());
        return (ISet<AgentQueueReference>) this.m_queues;
      }
    }

    public ISet<AgentPoolReference> Pools
    {
      get
      {
        if (this.m_pools == null)
          this.m_pools = new HashSet<AgentPoolReference>((IEqualityComparer<AgentPoolReference>) new PipelineResources.PoolComparer());
        return (ISet<AgentPoolReference>) this.m_pools;
      }
    }

    public ISet<RepositoryResource> Repositories
    {
      get
      {
        if (this.m_repositories == null)
          this.m_repositories = new HashSet<RepositoryResource>((IEqualityComparer<RepositoryResource>) new ResourceComparer());
        return (ISet<RepositoryResource>) this.m_repositories;
      }
    }

    public ISet<VariableGroupReference> VariableGroups
    {
      get
      {
        if (this.m_variableGroups == null)
          this.m_variableGroups = new HashSet<VariableGroupReference>((IEqualityComparer<VariableGroupReference>) new PipelineResources.VariableGroupComparer());
        return (ISet<VariableGroupReference>) this.m_variableGroups;
      }
    }

    public ISet<WebhookResource> Webhooks
    {
      get
      {
        if (this.m_webhookResources == null)
          this.m_webhookResources = new HashSet<WebhookResource>((IEqualityComparer<WebhookResource>) new ResourceComparer());
        return (ISet<WebhookResource>) this.m_webhookResources;
      }
    }

    public PipelineResources Clone() => new PipelineResources(this);

    public void MergeWith(PipelineResources resources)
    {
      if (resources == null)
        return;
      this.Builds.UnionWith((IEnumerable<BuildResource>) resources.Builds);
      this.Containers.UnionWith((IEnumerable<ContainerResource>) resources.Containers);
      this.Endpoints.UnionWith((IEnumerable<ServiceEndpointReference>) resources.Endpoints);
      this.Environments.UnionWith((IEnumerable<EnvironmentReference>) resources.Environments);
      this.Files.UnionWith((IEnumerable<SecureFileReference>) resources.Files);
      this.Packages.UnionWith((IEnumerable<PackageResource>) resources.Packages);
      this.PersistedStages.UnionWith((IEnumerable<PersistedStageReference>) resources.PersistedStages);
      this.Pipelines.UnionWith((IEnumerable<PipelineResource>) resources.Pipelines);
      this.Queues.UnionWith((IEnumerable<AgentQueueReference>) resources.Queues);
      this.Pools.UnionWith((IEnumerable<AgentPoolReference>) resources.Pools);
      this.Repositories.UnionWith((IEnumerable<RepositoryResource>) resources.Repositories);
      this.VariableGroups.UnionWith((IEnumerable<VariableGroupReference>) resources.VariableGroups);
      this.Webhooks.UnionWith((IEnumerable<WebhookResource>) resources.Webhooks);
    }

    internal void AddEndpointReference(string endpointId)
    {
      Guid result;
      if (Guid.TryParse(endpointId, out result))
      {
        this.Endpoints.Add(new ServiceEndpointReference()
        {
          Id = result
        });
      }
      else
      {
        ISet<ServiceEndpointReference> endpoints = this.Endpoints;
        ServiceEndpointReference endpointReference = new ServiceEndpointReference();
        endpointReference.Name = (ExpressionValue<string>) endpointId;
        endpoints.Add(endpointReference);
      }
    }

    internal void AddEndpointReference(ServiceEndpointReference reference) => this.Endpoints.Add(reference);

    internal void AddSecureFileReference(string fileId)
    {
      Guid result;
      if (Guid.TryParse(fileId, out result))
      {
        this.Files.Add(new SecureFileReference()
        {
          Id = result
        });
      }
      else
      {
        ISet<SecureFileReference> files = this.Files;
        SecureFileReference secureFileReference = new SecureFileReference();
        secureFileReference.Name = (ExpressionValue<string>) fileId;
        files.Add(secureFileReference);
      }
    }

    internal void AddSecureFileReference(SecureFileReference reference) => this.Files.Add(reference);

    internal void AddAgentQueueReference(AgentQueueReference reference) => this.Queues.Add(reference);

    internal void AddAgentPoolReference(AgentPoolReference reference) => this.Pools.Add(reference);

    internal void AddVariableGroupReference(VariableGroupReference reference) => this.VariableGroups.Add(reference);

    internal void AddEnvironmentReference(EnvironmentReference reference) => this.Environments.Add(reference);

    internal void Clear()
    {
      this.m_builds?.Clear();
      this.m_containers?.Clear();
      this.m_endpoints?.Clear();
      this.m_files?.Clear();
      this.m_pipelines?.Clear();
      this.m_queues?.Clear();
      this.m_pools?.Clear();
      this.m_repositories?.Clear();
      this.m_variableGroups?.Clear();
      this.m_environments?.Clear();
      this.m_packages?.Clear();
      this.m_webhookResources?.Clear();
    }

    private List<RepositoryReference> GetSecurableRepositories(bool includeRepositories)
    {
      if (!includeRepositories)
        return new List<RepositoryReference>();
      HashSet<RepositoryResource> repositories = this.m_repositories;
      // ISSUE: explicit non-virtual call
      return (repositories != null ? (__nonvirtual (repositories.Count) == 1 ? 1 : 0) : 0) != 0 && this.m_repositories.First<RepositoryResource>().Alias.Equals(PipelineConstants.DesignerRepo) ? new List<RepositoryReference>() : this.m_repositories.Where<RepositoryResource>((Func<RepositoryResource, bool>) (x => x.Type.Equals(RepositoryTypes.Git, StringComparison.OrdinalIgnoreCase) && !x.Alias.Equals(PipelineConstants.SelfAlias) && x.Endpoint == null)).Select<RepositoryResource, RepositoryReference>((Func<RepositoryResource, RepositoryReference>) (x =>
      {
        return new RepositoryReference()
        {
          Id = x.Properties.Get<Guid>(RepositoryPropertyNames.Project).ToString() + "." + x.Id,
          Name = (ExpressionValue<string>) x.Name
        };
      })).ToList<RepositoryReference>();
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      HashSet<BuildResource> builds = this.m_builds;
      // ISSUE: explicit non-virtual call
      if ((builds != null ? (__nonvirtual (builds.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_builds = (HashSet<BuildResource>) null;
      HashSet<ContainerResource> containers = this.m_containers;
      // ISSUE: explicit non-virtual call
      if ((containers != null ? (__nonvirtual (containers.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_containers = (HashSet<ContainerResource>) null;
      HashSet<ServiceEndpointReference> endpoints = this.m_endpoints;
      // ISSUE: explicit non-virtual call
      if ((endpoints != null ? (__nonvirtual (endpoints.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_endpoints = (HashSet<ServiceEndpointReference>) null;
      HashSet<SecureFileReference> files = this.m_files;
      // ISSUE: explicit non-virtual call
      if ((files != null ? (__nonvirtual (files.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_files = (HashSet<SecureFileReference>) null;
      HashSet<PipelineResource> pipelines = this.m_pipelines;
      // ISSUE: explicit non-virtual call
      if ((pipelines != null ? (__nonvirtual (pipelines.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_pipelines = (HashSet<PipelineResource>) null;
      HashSet<AgentQueueReference> queues = this.m_queues;
      // ISSUE: explicit non-virtual call
      if ((queues != null ? (__nonvirtual (queues.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_queues = (HashSet<AgentQueueReference>) null;
      HashSet<AgentPoolReference> pools = this.m_pools;
      // ISSUE: explicit non-virtual call
      if ((pools != null ? (__nonvirtual (pools.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_pools = (HashSet<AgentPoolReference>) null;
      HashSet<RepositoryResource> repositories = this.m_repositories;
      // ISSUE: explicit non-virtual call
      if ((repositories != null ? (__nonvirtual (repositories.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_repositories = (HashSet<RepositoryResource>) null;
      HashSet<VariableGroupReference> variableGroups = this.m_variableGroups;
      // ISSUE: explicit non-virtual call
      if ((variableGroups != null ? (__nonvirtual (variableGroups.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_variableGroups = (HashSet<VariableGroupReference>) null;
      HashSet<EnvironmentReference> environments = this.m_environments;
      // ISSUE: explicit non-virtual call
      if ((environments != null ? (__nonvirtual (environments.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_environments = (HashSet<EnvironmentReference>) null;
      HashSet<PackageResource> packages = this.m_packages;
      // ISSUE: explicit non-virtual call
      if ((packages != null ? (__nonvirtual (packages.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_packages = (HashSet<PackageResource>) null;
      HashSet<WebhookResource> webhookResources = this.m_webhookResources;
      // ISSUE: explicit non-virtual call
      if ((webhookResources != null ? (__nonvirtual (webhookResources.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_webhookResources = (HashSet<WebhookResource>) null;
    }

    internal abstract class ResourceReferenceComparer<TId, TResource> : IEqualityComparer<TResource> where TResource : ResourceReference
    {
      private readonly IEqualityComparer<TId> m_idComparer;

      protected ResourceReferenceComparer(IEqualityComparer<TId> idComparer) => this.m_idComparer = idComparer;

      public abstract TId GetId(TResource resource);

      public bool Equals(TResource left, TResource right)
      {
        if ((object) left == null && (object) right == null)
          return true;
        if ((object) left != null && (object) right == null || (object) left == null && (object) right != null)
          return false;
        TId id1 = this.GetId(left);
        TId id2 = this.GetId(right);
        return this.m_idComparer.Equals(id1, default (TId)) && this.m_idComparer.Equals(id2, default (TId)) ? StringComparer.OrdinalIgnoreCase.Equals((object) left.Name, (object) right.Name) : this.m_idComparer.Equals(id1, id2);
      }

      public int GetHashCode(TResource obj)
      {
        TId id = this.GetId(obj);
        return !this.m_idComparer.Equals(id, default (TId)) ? id.GetHashCode() : StringComparer.OrdinalIgnoreCase.GetHashCode((object) obj.Name);
      }
    }

    internal class EndpointComparer : 
      PipelineResources.ResourceReferenceComparer<Guid, ServiceEndpointReference>
    {
      public EndpointComparer()
        : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default)
      {
      }

      public override Guid GetId(ServiceEndpointReference resource) => resource.Id;
    }

    internal class FileComparer : 
      PipelineResources.ResourceReferenceComparer<Guid, SecureFileReference>
    {
      public FileComparer()
        : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default)
      {
      }

      public override Guid GetId(SecureFileReference resource) => resource.Id;
    }

    private class QueueComparer : 
      PipelineResources.ResourceReferenceComparer<int, AgentQueueReference>
    {
      public QueueComparer()
        : base((IEqualityComparer<int>) EqualityComparer<int>.Default)
      {
      }

      public override int GetId(AgentQueueReference resource) => resource.Id;
    }

    private class PoolComparer : PipelineResources.ResourceReferenceComparer<int, AgentPoolReference>
    {
      public PoolComparer()
        : base((IEqualityComparer<int>) EqualityComparer<int>.Default)
      {
      }

      public override int GetId(AgentPoolReference resource) => resource.Id;
    }

    internal class VariableGroupComparer : 
      PipelineResources.ResourceReferenceComparer<int, VariableGroupReference>
    {
      public VariableGroupComparer()
        : base((IEqualityComparer<int>) EqualityComparer<int>.Default)
      {
      }

      public override int GetId(VariableGroupReference resource) => resource.Id;
    }

    private class EnvironmentComparer : 
      PipelineResources.ResourceReferenceComparer<int, EnvironmentReference>
    {
      public EnvironmentComparer()
        : base((IEqualityComparer<int>) EqualityComparer<int>.Default)
      {
      }

      public override int GetId(EnvironmentReference resource) => resource.Id;
    }

    private class PersistedStageComparer : 
      PipelineResources.ResourceReferenceComparer<long, PersistedStageReference>
    {
      public PersistedStageComparer()
        : base((IEqualityComparer<long>) EqualityComparer<long>.Default)
      {
      }

      public override long GetId(PersistedStageReference resource) => resource.Id;
    }
  }
}
