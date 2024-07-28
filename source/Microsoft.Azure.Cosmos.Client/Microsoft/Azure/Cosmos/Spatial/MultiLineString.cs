// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.MultiLineString
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
  internal sealed class MultiLineString : Geometry, IEquatable<MultiLineString>
  {
    public MultiLineString(IList<LineStringCoordinates> lineStrings)
      : this(lineStrings, new GeometryParams())
    {
    }

    public MultiLineString(IList<LineStringCoordinates> lineStrings, GeometryParams geometryParams)
      : base(GeometryType.MultiLineString, geometryParams)
    {
      this.LineStrings = lineStrings != null ? new ReadOnlyCollection<LineStringCoordinates>(lineStrings) : throw new ArgumentNullException(nameof (lineStrings));
    }

    internal MultiLineString()
      : base(GeometryType.MultiLineString, new GeometryParams())
    {
    }

    [DataMember(Name = "coordinates")]
    [JsonProperty("coordinates", Required = Required.Always, Order = 1)]
    public ReadOnlyCollection<LineStringCoordinates> LineStrings { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as MultiLineString);

    public override int GetHashCode() => this.LineStrings.Aggregate<LineStringCoordinates, int>(base.GetHashCode(), (Func<int, LineStringCoordinates, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(MultiLineString other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return base.Equals((object) other) && this.LineStrings.SequenceEqual<LineStringCoordinates>((IEnumerable<LineStringCoordinates>) other.LineStrings);
    }
  }
}
