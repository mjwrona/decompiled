// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyMultiLineStringImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeographyMultiLineStringImplementation : GeographyMultiLineString
  {
    private GeographyLineString[] lineStrings;

    internal GeographyMultiLineStringImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params GeographyLineString[] lineStrings)
      : base(coordinateSystem, creator)
    {
      this.lineStrings = lineStrings ?? new GeographyLineString[0];
    }

    internal GeographyMultiLineStringImplementation(
      SpatialImplementation creator,
      params GeographyLineString[] lineStrings)
      : this(CoordinateSystem.DefaultGeography, creator, lineStrings)
    {
    }

    public override bool IsEmpty => this.lineStrings.Length == 0;

    public override ReadOnlyCollection<Geography> Geographies => new ReadOnlyCollection<Geography>((IList<Geography>) this.lineStrings);

    public override ReadOnlyCollection<GeographyLineString> LineStrings => new ReadOnlyCollection<GeographyLineString>((IList<GeographyLineString>) this.lineStrings);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeographyPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeography(SpatialType.MultiLineString);
      for (int index = 0; index < this.lineStrings.Length; ++index)
        this.lineStrings[index].SendTo(pipeline);
      pipeline.EndGeography();
    }
  }
}
