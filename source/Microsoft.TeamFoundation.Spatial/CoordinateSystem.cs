// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.CoordinateSystem
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.Spatial
{
  public class CoordinateSystem
  {
    public static readonly CoordinateSystem DefaultGeometry = new CoordinateSystem(0, "Unitless Plane", CoordinateSystem.Topology.Geometry);
    public static readonly CoordinateSystem DefaultGeography = new CoordinateSystem(4326, "WGS84", CoordinateSystem.Topology.Geography);
    private static readonly Dictionary<CompositeKey<int, CoordinateSystem.Topology>, CoordinateSystem> References;
    private static readonly object referencesLock = new object();
    private CoordinateSystem.Topology topology;

    [SuppressMessage("Microsoft.Performance", "CA1810", Justification = "Static Constructor required")]
    static CoordinateSystem()
    {
      CoordinateSystem.References = new Dictionary<CompositeKey<int, CoordinateSystem.Topology>, CoordinateSystem>((IEqualityComparer<CompositeKey<int, CoordinateSystem.Topology>>) EqualityComparer<CompositeKey<int, CoordinateSystem.Topology>>.Default);
      CoordinateSystem.AddRef(CoordinateSystem.DefaultGeometry);
      CoordinateSystem.AddRef(CoordinateSystem.DefaultGeography);
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
    internal CoordinateSystem(int epsgId, string name, CoordinateSystem.Topology topology)
    {
      this.topology = topology;
      this.EpsgId = new int?(epsgId);
      this.Name = name;
    }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
    public int? EpsgId { get; internal set; }

    public string Id => this.EpsgId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);

    public string Name { get; internal set; }

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
    public static CoordinateSystem Geography(int? epsgId) => !epsgId.HasValue ? CoordinateSystem.DefaultGeography : CoordinateSystem.GetOrCreate(epsgId.Value, CoordinateSystem.Topology.Geography);

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "epsg", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
    public static CoordinateSystem Geometry(int? epsgId) => !epsgId.HasValue ? CoordinateSystem.DefaultGeometry : CoordinateSystem.GetOrCreate(epsgId.Value, CoordinateSystem.Topology.Geometry);

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}CoordinateSystem(EpsgId={1})", new object[2]
    {
      (object) this.topology,
      (object) this.EpsgId
    });

    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Wkt", Justification = "This is not hungarian notation, but the widley accepted abreviation")]
    public string ToWktId() => "SRID=" + (object) this.EpsgId + ";";

    public override bool Equals(object obj) => this.Equals(obj as CoordinateSystem);

    public bool Equals(CoordinateSystem other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return object.Equals((object) other.topology, (object) this.topology) && other.EpsgId.Equals((object) this.EpsgId);
    }

    public override int GetHashCode() => this.topology.GetHashCode() * 397 ^ (this.EpsgId.HasValue ? this.EpsgId.Value : 0);

    internal bool TopologyIs(CoordinateSystem.Topology expected) => this.topology == expected;

    private static CoordinateSystem GetOrCreate(int epsgId, CoordinateSystem.Topology topology)
    {
      CoordinateSystem coords;
      lock (CoordinateSystem.referencesLock)
      {
        if (CoordinateSystem.References.TryGetValue(CoordinateSystem.KeyFor(epsgId, topology), out coords))
          return coords;
        coords = new CoordinateSystem(epsgId, "ID " + epsgId.ToString((IFormatProvider) CultureInfo.InvariantCulture), topology);
        CoordinateSystem.AddRef(coords);
      }
      return coords;
    }

    private static void AddRef(CoordinateSystem coords) => CoordinateSystem.References.Add(CoordinateSystem.KeyFor(coords.EpsgId.Value, coords.topology), coords);

    private static CompositeKey<int, CoordinateSystem.Topology> KeyFor(
      int epsgId,
      CoordinateSystem.Topology topology)
    {
      return new CompositeKey<int, CoordinateSystem.Topology>(epsgId, topology);
    }

    internal enum Topology
    {
      Geography,
      Geometry,
    }
  }
}
