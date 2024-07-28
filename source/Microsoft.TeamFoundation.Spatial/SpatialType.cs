// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialType
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  [SuppressMessage("Microsoft.Design", "CA1028", Justification = "byte required for packing")]
  public enum SpatialType : byte
  {
    Unknown = 0,
    Point = 1,
    LineString = 2,
    Polygon = 3,
    MultiPoint = 4,
    MultiLineString = 5,
    MultiPolygon = 6,
    Collection = 7,
    FullGlobe = 11, // 0x0B
  }
}
