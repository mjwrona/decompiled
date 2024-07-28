// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IResourceStoreExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IResourceStoreExtensions
  {
    public static JobResources GetJobResources(
      this IResourceStore store,
      PipelineResources resources)
    {
      JobResources jobResources = new JobResources();
      jobResources.Containers.AddRange(resources.Containers.Select<ContainerResource, ContainerResource>((Func<ContainerResource, ContainerResource>) (x => x.Clone())));
      foreach (ServiceEndpointReference endpoint in (IEnumerable<ServiceEndpointReference>) resources.Endpoints)
      {
        ServiceEndpoint serviceEndpoint = store.Endpoints.Get(endpoint);
        if (serviceEndpoint != null)
          jobResources.Endpoints.Add(serviceEndpoint);
      }
      foreach (SecureFileReference file in (IEnumerable<SecureFileReference>) resources.Files)
      {
        SecureFile secureFile = store.Files.Get(file);
        if (secureFile != null)
          jobResources.SecureFiles.Add(secureFile);
      }
      foreach (RepositoryResource repository in (IEnumerable<RepositoryResource>) resources.Repositories)
        jobResources.Repositories.Add(store.Repositories.Get(repository.Alias));
      return jobResources;
    }

    public static ServiceEndpoint GetEndpoint(
      this IResourceStore store,
      ServiceEndpointReference reference)
    {
      return store.Endpoints.Get(reference);
    }

    public static SecureFile GetFile(this IResourceStore store, SecureFileReference reference) => store.Files.Get(reference);

    public static TaskAgentQueue GetQueue(this IResourceStore store, AgentQueueReference reference) => store.Queues.Get(reference);

    public static TaskAgentPool GetPool(this IResourceStore store, AgentPoolReference reference) => store.Pools.Get(reference);

    public static VariableGroup GetVariableGroup(
      this IResourceStore store,
      VariableGroupReference reference)
    {
      return store.VariableGroups.Get(reference);
    }

    public static ResourceReference GetSnappedReference(
      this IResourceStore store,
      ResourceReference r)
    {
      switch (r)
      {
        case VariableGroupReference queue:
          VariableGroup variableGroup = store.VariableGroups.Get(queue);
          if (variableGroup != null)
          {
            VariableGroupReference snappedReference = new VariableGroupReference();
            snappedReference.Id = variableGroup.Id;
            snappedReference.Name = (ExpressionValue<string>) variableGroup.Name;
            return (ResourceReference) snappedReference;
          }
          break;
        case AgentQueueReference reference1:
          TaskAgentQueue taskAgentQueue = store.Queues.Get(reference1);
          if (taskAgentQueue != null)
          {
            AgentQueueReference snappedReference = new AgentQueueReference();
            snappedReference.Id = taskAgentQueue.Id;
            snappedReference.Name = (ExpressionValue<string>) taskAgentQueue.Name;
            return (ResourceReference) snappedReference;
          }
          break;
        case AgentPoolReference reference2:
          TaskAgentPool taskAgentPool = store.Pools.Get(reference2);
          if (taskAgentPool != null)
          {
            AgentPoolReference snappedReference = new AgentPoolReference();
            snappedReference.Id = taskAgentPool.Id;
            snappedReference.Name = (ExpressionValue<string>) taskAgentPool.Name;
            return (ResourceReference) snappedReference;
          }
          break;
        case ServiceEndpointReference endpoint:
          ServiceEndpoint serviceEndpoint = store.Endpoints.Get(endpoint);
          if (serviceEndpoint != null)
          {
            ServiceEndpointReference snappedReference = new ServiceEndpointReference();
            snappedReference.Id = serviceEndpoint.Id;
            snappedReference.Name = (ExpressionValue<string>) serviceEndpoint.Name;
            return (ResourceReference) snappedReference;
          }
          break;
        case SecureFileReference reference3:
          SecureFile secureFile = store.Files.Get(reference3);
          if (secureFile != null)
          {
            SecureFileReference snappedReference = new SecureFileReference();
            snappedReference.Id = secureFile.Id;
            snappedReference.Name = (ExpressionValue<string>) secureFile.Name;
            return (ResourceReference) snappedReference;
          }
          break;
        case EnvironmentReference reference4:
          EnvironmentInstance environmentInstance = store.Environments.Get(reference4);
          if (environmentInstance != null)
          {
            EnvironmentReference snappedReference = new EnvironmentReference();
            snappedReference.Id = environmentInstance.Id;
            snappedReference.Name = (ExpressionValue<string>) environmentInstance.Name;
            return (ResourceReference) snappedReference;
          }
          break;
        case PersistedStageReference reference5:
          PersistedStage persistedStage = store.PersistedStages.Get(reference5);
          if (persistedStage != null)
          {
            PersistedStageReference snappedReference = new PersistedStageReference();
            snappedReference.Id = persistedStage.Id;
            snappedReference.Name = (ExpressionValue<string>) persistedStage.Name;
            snappedReference.DefinitionId = persistedStage.DefinitionId;
            snappedReference.GroupPath = persistedStage.Group.Path;
            snappedReference.BuildId = persistedStage.Group.BuildId;
            return (ResourceReference) snappedReference;
          }
          break;
      }
      return r;
    }
  }
}
