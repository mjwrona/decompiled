// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Spatial.LineString
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Azure.Documents.Spatial
{
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
