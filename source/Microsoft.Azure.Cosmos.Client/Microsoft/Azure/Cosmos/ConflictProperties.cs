// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ConflictProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public class ConflictProperties
  {
    [JsonProperty(PropertyName = "id")]
    public string Id { get; internal set; }

    [JsonConverter(typeof (StringEnumConverter))]
    [JsonProperty(PropertyName = "operationType")]
    public OperationKind OperationKind { get; internal set; }

    [JsonProperty(PropertyName = "_self", NullValueHandling = NullValueHandling.Ignore)]
    public string SelfLink { get; private set; }

    [JsonConverter(typeof (ConflictResourceTypeJsonConverter))]
    [JsonProperty(PropertyName = "resourceType")]
    internal Type ResourceType { get; set; }

    [JsonProperty(PropertyName = "resourceId")]
    internal string SourceResourceId { get; set; }

    [JsonProperty(PropertyName = "content")]
    internal string Content { get; set; }

    [JsonProperty(PropertyName = "conflict_lsn")]
    internal long ConflictLSN { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
