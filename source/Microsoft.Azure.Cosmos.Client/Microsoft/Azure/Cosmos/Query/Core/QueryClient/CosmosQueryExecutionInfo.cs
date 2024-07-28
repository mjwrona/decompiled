// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryClient.CosmosQueryExecutionInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryClient
{
  internal sealed class CosmosQueryExecutionInfo
  {
    [JsonConstructor]
    public CosmosQueryExecutionInfo(bool reverseRidEnabled, bool reverseIndexScan)
    {
      this.ReverseRidEnabled = reverseRidEnabled;
      this.ReverseIndexScan = reverseIndexScan;
    }

    [JsonProperty("reverseRidEnabled")]
    public bool ReverseRidEnabled { get; }

    [JsonProperty("reverseIndexScan")]
    public bool ReverseIndexScan { get; }
  }
}
