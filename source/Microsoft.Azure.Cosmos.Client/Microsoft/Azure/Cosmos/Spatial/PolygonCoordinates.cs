// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.PolygonCoordinates
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Spatial.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Spatial
{
  [DataContract]
  [JsonConverter(typeof (PolygonCoordinatesJsonConverter))]
  public sealed class PolygonCoordinates : IEquatable<PolygonCoordinates>
  {
    public PolygonCoordinates(IList<LinearRing> rings) => this.Rings = rings != null ? new ReadOnlyCollection<LinearRing>(rings) : throw new ArgumentException(nameof (rings));

    [DataMember(Name = "rings")]
    public ReadOnlyCollection<LinearRing> Rings { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as PolygonCoordinates);

    public override int GetHashCode() => this.Rings.Aggregate<LinearRing, int>(0, (Func<int, LinearRing, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(PolygonCoordinates other)
    {
      if (other == null)
        return false;
      return this == other || this.Rings.SequenceEqual<LinearRing>((IEnumerable<LinearRing>) other.Rings);
    }
  }
}
