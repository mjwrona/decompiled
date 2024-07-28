// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.LinearRing
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
  [JsonConverter(typeof (LinearRingJsonConverter))]
  public sealed class LinearRing : IEquatable<LinearRing>
  {
    public LinearRing(IList<Position> coordinates) => this.Positions = coordinates != null ? new ReadOnlyCollection<Position>(coordinates) : throw new ArgumentNullException(nameof (coordinates));

    [DataMember(Name = "coordinates")]
    public ReadOnlyCollection<Position> Positions { get; private set; }

    public override bool Equals(object obj) => this.Equals(obj as LinearRing);

    public override int GetHashCode() => this.Positions.Aggregate<Position, int>(0, (Func<int, Position, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(LinearRing other)
    {
      if (other == null)
        return false;
      return this == other || this.Positions.SequenceEqual<Position>((IEnumerable<Position>) other.Positions);
    }
  }
}
