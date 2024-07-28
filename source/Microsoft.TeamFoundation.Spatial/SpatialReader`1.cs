// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialReader`1
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;

namespace Microsoft.Spatial
{
  internal abstract class SpatialReader<TSource>
  {
    protected SpatialReader(SpatialPipeline destination)
    {
      Util.CheckArgumentNull((object) destination, nameof (destination));
      this.Destination = destination;
    }

    protected SpatialPipeline Destination { get; set; }

    public void ReadGeography(TSource input)
    {
      Util.CheckArgumentNull((object) input, nameof (input));
      try
      {
        this.ReadGeographyImplementation(input);
      }
      catch (Exception ex)
      {
        if (Util.IsCatchableExceptionType(ex))
          throw new ParseErrorException(ex.Message, ex);
        throw;
      }
    }

    public void ReadGeometry(TSource input)
    {
      Util.CheckArgumentNull((object) input, nameof (input));
      try
      {
        this.ReadGeometryImplementation(input);
      }
      catch (Exception ex)
      {
        if (Util.IsCatchableExceptionType(ex))
          throw new ParseErrorException(ex.Message, ex);
        throw;
      }
    }

    public virtual void Reset()
    {
      ((GeographyPipeline) this.Destination).Reset();
      ((GeometryPipeline) this.Destination).Reset();
    }

    protected abstract void ReadGeometryImplementation(TSource input);

    protected abstract void ReadGeographyImplementation(TSource input);
  }
}
