// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.Geometry
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Spatial.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents.Spatial
{
  [JsonObject(MemberSerialization.OptIn)]
  [JsonConverter(typeof (GeometryJsonConverter))]
  public abstract class Geometry
  {
    protected Geometry(GeometryType type, GeometryParams geometryParams)
    {
      if (geometryParams == null)
        throw new ArgumentNullException(nameof (geometryParams));
      this.Type = type;
      this.CrsForSerialization = geometryParams.Crs == null || geometryParams.Crs.Equals((object) Crs.Default) ? (Crs) null : geometryParams.Crs;
      this.BoundingBox = geometryParams.BoundingBox;
      this.AdditionalProperties = geometryParams.AdditionalProperties ?? (IDictionary<string, object>) new Dictionary<string, object>();
    }

    public Crs Crs => this.CrsForSerialization ?? Crs.Default;

    [JsonProperty("type", Required = Required.Always, Order = 0)]
    [JsonConverter(typeof (StringEnumConverter))]
    public GeometryType Type { get; private set; }

    [JsonProperty("bbox", DefaultValueHandling = DefaultValueHandling.Ignore, Order = 3)]
    public BoundingBox BoundingBox { get; private set; }

    [JsonExtensionData]
    public IDictionary<string, object> AdditionalProperties { get; private set; }

    [JsonProperty("crs", DefaultValueHandling = DefaultValueHandling.Ignore, Order = 2)]
    private Crs CrsForSerialization { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as Geometry);

    public override int GetHashCode() => this.AdditionalProperties.Aggregate<KeyValuePair<string, object>, int>((int) ((GeometryType) (this.Crs.GetHashCode() * 397) ^ this.Type) * 397 ^ (this.BoundingBox != null ? this.BoundingBox.GetHashCode() : 0), (Func<int, KeyValuePair<string, object>, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    private bool Equals(Geometry other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.Crs.Equals((object) other.Crs) && this.Type == other.Type && object.Equals((object) this.BoundingBox, (object) other.BoundingBox) && this.AdditionalProperties.SequenceEqual<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) other.AdditionalProperties);
    }
  }
}
