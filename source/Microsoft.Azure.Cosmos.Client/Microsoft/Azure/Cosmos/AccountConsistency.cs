// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AccountConsistency
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public class AccountConsistency
  {
    private const ConsistencyLevel defaultDefaultConsistencyLevel = ConsistencyLevel.Session;
    internal const int DefaultMaxStalenessInterval = 5;
    internal const int DefaultMaxStalenessPrefix = 100;
    internal const int MaxStalenessIntervalInSecondsMinValue = 5;
    internal const int MaxStalenessIntervalInSecondsMaxValue = 86400;
    internal const int MaxStalenessPrefixMinValue = 10;
    internal const int MaxStalenessPrefixMaxValue = 1000000;

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "defaultConsistencyLevel")]
    public ConsistencyLevel DefaultConsistencyLevel { get; internal set; }

    [JsonProperty(PropertyName = "maxStalenessPrefix")]
    public int MaxStalenessPrefix { get; internal set; }

    [JsonProperty(PropertyName = "maxIntervalInSeconds")]
    public int MaxStalenessIntervalInSeconds { get; internal set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    internal Microsoft.Azure.Documents.ConsistencyLevel ToDirectConsistencyLevel() => (Microsoft.Azure.Documents.ConsistencyLevel) this.DefaultConsistencyLevel;
  }
}
