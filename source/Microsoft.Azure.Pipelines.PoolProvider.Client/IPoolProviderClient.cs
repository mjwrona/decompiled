// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Client.IPoolProviderClient
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D55F5C7-EE6B-4E5B-8407-D17F3B35057D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Client.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.PoolProvider.Client
{
  public interface IPoolProviderClient : IDisposable
  {
    Task AddAgentCloudRequestMessageAsync(
      AgentRequestMessageVerbosity verbosity,
      string message,
      CancellationToken cancellationToken = default (CancellationToken));

    Task UpdateAgentCloudRequestAsync(
      AgentRequestProvisioningResult provisioningResult,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
