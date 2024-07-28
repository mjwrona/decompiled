// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CollectionThroughputInfo
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;

namespace Microsoft.Azure.Documents
{
  internal sealed class CollectionThroughputInfo : JsonSerializable
  {
    [JsonProperty(PropertyName = "minimumRUForCollection", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal int? MinimumRUForCollection
    {
      get => this.GetValue<int?>("minimumRUForCollection");
      set => this.SetValue("minimumRUForCollection", (object) value);
    }

    [JsonProperty(PropertyName = "numPhysicalPartitions", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal int? NumberOfPhysicalPartitions
    {
      get => this.GetValue<int?>("numPhysicalPartitions");
      set => this.SetValue("numPhysicalPartitions", (object) value);
    }

    [JsonProperty(PropertyName = "userSpecifiedThroughput", DefaultValueHandling = DefaultValueHandling.Ignore)]
    internal int? UserSpecifiedThroughput
    {
      get => this.GetValue<int?>("userSpecifiedThroughput");
      set => this.SetValue("userSpecifiedThroughput", (object) value);
    }
  }
}
