// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IncludedPath
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Azure.Cosmos
{
  public sealed class IncludedPath
  {
    [JsonProperty(PropertyName = "path")]
    public string Path { get; set; }

    [JsonProperty(PropertyName = "indexes")]
    internal Collection<Index> Indexes { get; set; } = new Collection<Index>();

    [JsonProperty(PropertyName = "isFullIndex", NullValueHandling = NullValueHandling.Ignore)]
    internal bool? IsFullIndex { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
