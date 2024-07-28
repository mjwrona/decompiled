// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.RetryPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class RetryPolicy : IRetryPolicyFactory
  {
    private readonly GlobalPartitionEndpointManager partitionKeyRangeLocationCache;
    private readonly GlobalEndpointManager globalEndpointManager;
    private readonly bool enableEndpointDiscovery;
    private readonly RetryOptions retryOptions;

    public RetryPolicy(
      GlobalEndpointManager globalEndpointManager,
      ConnectionPolicy connectionPolicy,
      GlobalPartitionEndpointManager partitionKeyRangeLocationCache)
    {
      this.enableEndpointDiscovery = connectionPolicy.EnableEndpointDiscovery;
      this.globalEndpointManager = globalEndpointManager;
      this.retryOptions = connectionPolicy.RetryOptions;
      this.partitionKeyRangeLocationCache = partitionKeyRangeLocationCache;
    }

    public IDocumentClientRetryPolicy GetRequestPolicy() => (IDocumentClientRetryPolicy) new ClientRetryPolicy(this.globalEndpointManager, this.partitionKeyRangeLocationCache, this.enableEndpointDiscovery, this.retryOptions);
  }
}
