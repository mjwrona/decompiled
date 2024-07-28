// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.RetryPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;

namespace Microsoft.Azure.Documents.Client
{
  internal sealed class RetryPolicy : IRetryPolicyFactory
  {
    private readonly GlobalEndpointManager globalEndpointManager;
    private readonly bool enableEndpointDiscovery;
    private readonly RetryOptions retryOptions;

    public RetryPolicy(
      GlobalEndpointManager globalEndpointManager,
      ConnectionPolicy connectionPolicy)
    {
      this.enableEndpointDiscovery = connectionPolicy.EnableEndpointDiscovery;
      this.globalEndpointManager = globalEndpointManager;
      this.retryOptions = connectionPolicy.RetryOptions;
    }

    public IDocumentClientRetryPolicy GetRequestPolicy() => (IDocumentClientRetryPolicy) new ClientRetryPolicy(this.globalEndpointManager, this.enableEndpointDiscovery, this.retryOptions);
  }
}
