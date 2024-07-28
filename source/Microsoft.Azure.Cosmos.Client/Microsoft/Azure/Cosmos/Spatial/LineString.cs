// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.LineString
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  public sealed class LineString : Geometry, IEquatable<LineString>
  {
    public LineString(IList<Position> coordinates)
      : this(coordinates, new GeometryParams())
    {
    }

    public LineString(IList<Position> coordinates, GeometryParams geometryParams)
      : base(GeometryType.LineString, geometryParams)
    {
      this.Positions = coordinates != null ? new ReadOnlyCollection<Position>(coordinates) : throw new ArgumentNullException(nameof (coordinates));
    }

    internal LineString()
      : base(GeometryType.LineString, new GeometryParams())
    {
    }

    [DataMember(Name = "coordinates")]
    [JsonProperty("coordinates", Required = Required.Always, Order = 1)]
    public ReadOnlyCollection<Position> Positions { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as LineString);

    public override int GetHashCode() => this.Positions.Aggregate<Position, int>(base.GetHashCode(), (Func<int, Position, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(LineString other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return base.Equals((object) other) && this.Positions.SequenceEqual<Position>((IEnumerable<Position>) other.Positions);
    }
  }
}
