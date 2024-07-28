// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IResourceStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IResourceStore : IStepProvider
  {
    IBuildStore Builds { get; }

    IPackageResourceStore Packages { get; }

    IContainerStore Containers { get; }

    IServiceEndpointStore Endpoints { get; }

    ISecureFileStore Files { get; }

    IEnvironmentStore Environments { get; }

    IPersistedStageStore PersistedStages { get; }

    IPipelineStore Pipelines { get; }

    IAgentQueueStore Queues { get; }

    IAgentPoolStore Pools { get; }

    IRepositoryStore Repositories { get; }

    IVariableGroupStore VariableGroups { get; }

    PipelineResources GetAuthorizedResources();

    ServiceEndpoint GetEndpoint(Guid endpointId);

    ServiceEndpoint GetEndpoint(string endpointId);

    SecureFile GetFile(Guid fileId);

    SecureFile GetFile(string fileId);

    TaskAgentQueue GetQueue(int queueId);

    TaskAgentQueue GetQueue(string queueId);

    TaskAgentPool GetPool(int poolId);

    TaskAgentPool GetPool(string poolName);

    VariableGroup GetVariableGroup(int groupId);

    VariableGroup GetVariableGroup(string groupId);
  }
}
