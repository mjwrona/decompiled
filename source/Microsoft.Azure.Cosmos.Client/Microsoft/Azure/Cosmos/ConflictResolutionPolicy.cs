// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ConflictResolutionPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public class ConflictResolutionPolicy
  {
    public ConflictResolutionPolicy() => this.Mode = ConflictResolutionMode.LastWriterWins;

    [JsonProperty(PropertyName = "mode", NullValueHandling = NullValueHandling.Ignore)]
    [JsonConverter(typeof (StringEnumConverter))]
    public ConflictResolutionMode Mode { get; set; }

    [JsonProperty(PropertyName = "conflictResolutionPath", NullValueHandling = NullValueHandling.Ignore)]
    public string ResolutionPath { get; set; }

    [JsonProperty(PropertyName = "conflictResolutionProcedure", NullValueHandling = NullValueHandling.Ignore)]
    public string ResolutionProcedure { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
