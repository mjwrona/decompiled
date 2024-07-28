// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.Geography
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Spatial
{
  public abstract class Geography : ISpatial
  {
    private SpatialImplementation creator;
    private CoordinateSystem coordinateSystem;

    protected Geography(CoordinateSystem coordinateSystem, SpatialImplementation creator)
    {
      Util.CheckArgumentNull((object) coordinateSystem, nameof (coordinateSystem));
      Util.CheckArgumentNull((object) creator, nameof (creator));
      this.coordinateSystem = coordinateSystem;
      this.creator = creator;
    }

    public CoordinateSystem CoordinateSystem
    {
      get => this.coordinateSystem;
      internal set => this.coordinateSystem = value;
    }

    public abstract bool IsEmpty { get; }

    internal SpatialImplementation Creator
    {
      get => this.creator;
      set => this.creator = value;
    }

    public virtual void SendTo(GeographyPipeline chain)
    {
      Util.CheckArgumentNull((object) chain, nameof (chain));
      chain.SetCoordinateSystem(this.coordinateSystem);
    }

    internal static int ComputeHashCodeFor<T>(CoordinateSystem coords, IEnumerable<T> fields) => fields.Aggregate<T, int>(coords.GetHashCode(), (Func<int, T, int>) ((current, field) => current * 397 ^ field.GetHashCode()));

    internal bool? BaseEquals(Geography other)
    {
      if (other == null)
        return new bool?(false);
      if (this == other)
        return new bool?(true);
      if (!this.coordinateSystem.Equals(other.coordinateSystem))
        return new bool?(false);
      return this.IsEmpty || other.IsEmpty ? new bool?(this.IsEmpty && other.IsEmpty) : new bool?();
    }
  }
}
