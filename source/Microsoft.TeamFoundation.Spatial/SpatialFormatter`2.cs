// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.SpatialFormatter`2
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  public abstract class SpatialFormatter<TReaderStream, TWriterStream>
  {
    private readonly SpatialImplementation creator;

    protected SpatialFormatter(SpatialImplementation creator)
    {
      Util.CheckArgumentNull((object) creator, nameof (creator));
      this.creator = creator;
    }

    public TResult Read<TResult>(TReaderStream input) where TResult : class, ISpatial
    {
      KeyValuePair<SpatialPipeline, IShapeProvider> keyValuePair = this.MakeValidatingBuilder();
      IShapeProvider shapeProvider = keyValuePair.Value;
      this.Read<TResult>(input, keyValuePair.Key);
      return typeof (Geometry).IsAssignableFrom(typeof (TResult)) ? (TResult) shapeProvider.ConstructedGeometry : (TResult) shapeProvider.ConstructedGeography;
    }

    [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The type hierarchy is too deep to have a specificly typed Read for each of them.")]
    public void Read<TResult>(TReaderStream input, SpatialPipeline pipeline) where TResult : class, ISpatial
    {
      if (typeof (Geometry).IsAssignableFrom(typeof (TResult)))
        this.ReadGeometry(input, pipeline);
      else
        this.ReadGeography(input, pipeline);
    }

    public void Write(ISpatial spatial, TWriterStream writerStream)
    {
      SpatialPipeline writer = this.CreateWriter(writerStream);
      spatial.SendTo(writer);
    }

    public abstract SpatialPipeline CreateWriter(TWriterStream writerStream);

    protected abstract void ReadGeography(TReaderStream readerStream, SpatialPipeline pipeline);

    protected abstract void ReadGeometry(TReaderStream readerStream, SpatialPipeline pipeline);

    protected KeyValuePair<SpatialPipeline, IShapeProvider> MakeValidatingBuilder()
    {
      SpatialBuilder builder = this.creator.CreateBuilder();
      SpatialPipeline validator = this.creator.CreateValidator();
      validator.ChainTo((SpatialPipeline) builder);
      return new KeyValuePair<SpatialPipeline, IShapeProvider>(validator, (IShapeProvider) builder);
    }
  }
}
