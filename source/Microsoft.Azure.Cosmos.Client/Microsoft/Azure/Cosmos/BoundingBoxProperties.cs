// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BoundingBoxProperties
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public sealed class BoundingBoxProperties
  {
    [JsonProperty(PropertyName = "xmin")]
    public double Xmin { get; set; }

    [JsonProperty(PropertyName = "ymin")]
    public double Ymin { get; set; }

    [JsonProperty(PropertyName = "xmax")]
    public double Xmax { get; set; }

    [JsonProperty(PropertyName = "ymax")]
    public double Ymax { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }
  }
}
