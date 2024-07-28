// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Geometry
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Spatial.Converters;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
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

    [DataMember(Name = "type")]
    [JsonProperty("type", Required = Required.Always, Order = 0)]
    [JsonConverter(typeof (StringEnumConverter))]
    public GeometryType Type { get; private set; }

    [DataMember(Name = "bbox")]
    [JsonProperty("bbox", DefaultValueHandling = DefaultValueHandling.Ignore, Order = 3)]
    public BoundingBox BoundingBox { get; private set; }

    [JsonExtensionData]
    [DataMember(Name = "properties")]
    public IDictionary<string, object> AdditionalProperties { get; private set; }

    [DataMember(Name = "crs")]
    [JsonProperty("crs", DefaultValueHandling = DefaultValueHandling.Ignore, Order = 2)]
    private Crs CrsForSerialization { get; set; }

    public override bool Equals(object obj) => this.Equals(obj as Geometry);

    public override int GetHashCode() => this.AdditionalProperties.Aggregate<KeyValuePair<string, object>, int>((int) ((GeometryType) (this.Crs.GetHashCode() * 397) ^ this.Type) * 397 ^ (this.BoundingBox != null ? this.BoundingBox.GetHashCode() : 0), (Func<int, KeyValuePair<string, object>, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public double Distance(Geometry to) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public bool Within(Geometry outer) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public bool IsValid() => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public GeometryValidationResult IsValidDetailed() => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

    public bool Intersects(Geometry geometry2) => throw new NotImplementedException(RMResources.SpatialExtensionMethodsNotImplemented);

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
