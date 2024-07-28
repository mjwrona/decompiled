// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeoJsonObjectFormatterImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;

namespace Microsoft.Spatial
{
  internal class GeoJsonObjectFormatterImplementation : GeoJsonObjectFormatter
  {
    private readonly SpatialImplementation creator;
    private SpatialBuilder builder;
    private SpatialPipeline parsePipeline;

    public GeoJsonObjectFormatterImplementation(SpatialImplementation creator) => this.creator = creator;

    public override T Read<T>(IDictionary<string, object> source)
    {
      this.EnsureParsePipeline();
      if (typeof (Geometry).IsAssignableFrom(typeof (T)))
      {
        new GeoJsonObjectReader((SpatialPipeline) this.builder).ReadGeometry(source);
        return this.builder.ConstructedGeometry as T;
      }
      new GeoJsonObjectReader((SpatialPipeline) this.builder).ReadGeography(source);
      return this.builder.ConstructedGeography as T;
    }

    public override IDictionary<string, object> Write(ISpatial value)
    {
      GeoJsonObjectWriter current = new GeoJsonObjectWriter();
      value.SendTo((SpatialPipeline) new ForwardingSegment((SpatialPipeline) (DrawBoth) current));
      return current.JsonObject;
    }

    public override SpatialPipeline CreateWriter(IGeoJsonWriter writer) => (SpatialPipeline) new ForwardingSegment((SpatialPipeline) (DrawBoth) new WrappedGeoJsonWriter(writer));

    private void EnsureParsePipeline()
    {
      if (this.parsePipeline == null)
      {
        this.builder = this.creator.CreateBuilder();
        this.parsePipeline = this.creator.CreateValidator().ChainTo((SpatialPipeline) this.builder);
      }
      else
      {
        this.parsePipeline.GeographyPipeline.Reset();
        this.parsePipeline.GeometryPipeline.Reset();
      }
    }
  }
}
