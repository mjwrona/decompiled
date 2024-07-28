// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Spatial.Position
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
  [JsonConverter(typeof (PositionJsonConverter))]
  public sealed class Position : IEquatable<Position>
  {
    public Position(double longitude, double latitude)
      : this(longitude, latitude, new double?())
    {
    }

    public Position(double longitude, double latitude, double? altitude)
    {
      if (altitude.HasValue)
        this.Coordinates = new ReadOnlyCollection<double>((IList<double>) new double[3]
        {
          longitude,
          latitude,
          altitude.Value
        });
      else
        this.Coordinates = new ReadOnlyCollection<double>((IList<double>) new double[2]
        {
          longitude,
          latitude
        });
    }

    public Position(IList<double> coordinates) => this.Coordinates = coordinates.Count >= 2 ? new ReadOnlyCollection<double>(coordinates) : throw new ArgumentException(nameof (coordinates));

    [DataMember(Name = "Coordinates")]
    public ReadOnlyCollection<double> Coordinates { get; private set; }

    public double Longitude => this.Coordinates[0];

    public double Latitude => this.Coordinates[1];

    public double? Altitude => this.Coordinates.Count <= 2 ? new double?() : new double?(this.Coordinates[2]);

    public override bool Equals(object obj) => this.Equals(obj as Position);

    public override int GetHashCode() => this.Coordinates.Aggregate<double, int>(0, (Func<int, double, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public bool Equals(Position other)
    {
      if (other == null)
        return false;
      return this == other || this.Coordinates.SequenceEqual<double>((IEnumerable<double>) other.Coordinates);
    }
  }
}
