// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.WellKnownTextSqlFormatterImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.IO;

namespace Microsoft.Spatial
{
  internal class WellKnownTextSqlFormatterImplementation : WellKnownTextSqlFormatter
  {
    private readonly bool allowOnlyTwoDimensions;

    internal WellKnownTextSqlFormatterImplementation(SpatialImplementation creator)
      : base(creator)
    {
    }

    internal WellKnownTextSqlFormatterImplementation(
      SpatialImplementation creator,
      bool allowOnlyTwoDimensions)
      : base(creator)
    {
      this.allowOnlyTwoDimensions = allowOnlyTwoDimensions;
    }

    public override SpatialPipeline CreateWriter(TextWriter target) => (SpatialPipeline) new ForwardingSegment((SpatialPipeline) (DrawBoth) new WellKnownTextSqlWriter(target, this.allowOnlyTwoDimensions));

    protected override void ReadGeography(TextReader readerStream, SpatialPipeline pipeline) => new WellKnownTextSqlReader(pipeline, this.allowOnlyTwoDimensions).ReadGeography(readerStream);

    protected override void ReadGeometry(TextReader readerStream, SpatialPipeline pipeline) => new WellKnownTextSqlReader(pipeline, this.allowOnlyTwoDimensions).ReadGeometry(readerStream);
  }
}
