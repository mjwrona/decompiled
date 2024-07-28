// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Index
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  [JsonConverter(typeof (CosmosIndexJsonConverter))]
  internal abstract class Index
  {
    protected Index(IndexKind kind) => this.Kind = kind;

    [JsonProperty(PropertyName = "kind")]
    [JsonConverter(typeof (StringEnumConverter))]
    public IndexKind Kind { get; set; }

    [JsonExtensionData]
    internal IDictionary<string, JToken> AdditionalProperties { get; private set; }

    public static RangeIndex Range(DataType dataType) => new RangeIndex(dataType);

    public static RangeIndex Range(DataType dataType, short precision) => new RangeIndex(dataType, precision);

    public static HashIndex Hash(DataType dataType) => new HashIndex(dataType);

    public static HashIndex Hash(DataType dataType, short precision) => new HashIndex(dataType, precision);

    public static SpatialIndex Spatial(DataType dataType) => new SpatialIndex(dataType);
  }
}
