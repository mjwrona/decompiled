// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeoJsonObjectFormatter
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;

namespace Microsoft.Spatial
{
  public abstract class GeoJsonObjectFormatter
  {
    public static GeoJsonObjectFormatter Create() => SpatialImplementation.CurrentImplementation.CreateGeoJsonObjectFormatter();

    public abstract T Read<T>(IDictionary<string, object> source) where T : class, ISpatial;

    public abstract IDictionary<string, object> Write(ISpatial value);

    public abstract SpatialPipeline CreateWriter(IGeoJsonWriter writer);
  }
}
