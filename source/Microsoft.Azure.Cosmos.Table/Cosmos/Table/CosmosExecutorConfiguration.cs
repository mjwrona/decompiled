// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.CosmosExecutorConfiguration
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Documents.Client;

namespace Microsoft.Azure.Cosmos.Table
{
  public class CosmosExecutorConfiguration
  {
    public bool UseConnectionModeDirect { get; set; } = true;

    public string UserAgentSuffix { get; set; }

    public string CurrentRegion { get; set; }

    public int MaxConnectionLimit { get; set; } = 50;

    public int? MaxRetryAttemptsOnThrottledRequests { get; set; }

    public int? MaxRetryWaitTimeOnThrottledRequests { get; set; }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel { get; set; }

    internal ConnectionPolicy GetConnectionPolicy()
    {
      ConnectionPolicy connectionPolicy = new ConnectionPolicy()
      {
        EnableEndpointDiscovery = true,
        UseMultipleWriteLocations = true,
        UserAgentSuffix = string.Format(" {0}/{1} {2}", (object) "cosmos-table-sdk", (object) "1.0.7", (object) this.UserAgentSuffix)
      };
      if (this.UseConnectionModeDirect)
      {
        connectionPolicy.ConnectionMode = ConnectionMode.Direct;
        connectionPolicy.ConnectionProtocol = Protocol.Tcp;
      }
      else
      {
        connectionPolicy.ConnectionMode = ConnectionMode.Gateway;
        connectionPolicy.ConnectionProtocol = Protocol.Https;
        this.MaxConnectionLimit = this.MaxConnectionLimit;
      }
      if (!string.IsNullOrEmpty(this.CurrentRegion))
        connectionPolicy.SetCurrentLocation(this.CurrentRegion);
      int? throttledRequests = this.MaxRetryAttemptsOnThrottledRequests;
      if (throttledRequests.HasValue)
      {
        RetryOptions retryOptions = connectionPolicy.RetryOptions;
        throttledRequests = this.MaxRetryAttemptsOnThrottledRequests;
        int num = throttledRequests.Value;
        retryOptions.MaxRetryAttemptsOnThrottledRequests = num;
      }
      throttledRequests = this.MaxRetryWaitTimeOnThrottledRequests;
      if (throttledRequests.HasValue)
      {
        RetryOptions retryOptions = connectionPolicy.RetryOptions;
        throttledRequests = this.MaxRetryWaitTimeOnThrottledRequests;
        int num = throttledRequests.Value;
        retryOptions.MaxRetryWaitTimeInSeconds = num;
      }
      return connectionPolicy;
    }
  }
}
