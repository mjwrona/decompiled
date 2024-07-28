// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeographyCollectionImplementation
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Spatial
{
  internal class GeographyCollectionImplementation : GeographyCollection
  {
    private Geography[] geographyArray;

    internal GeographyCollectionImplementation(
      CoordinateSystem coordinateSystem,
      SpatialImplementation creator,
      params Geography[] geography)
      : base(coordinateSystem, creator)
    {
      this.geographyArray = geography ?? new Geography[0];
    }

    internal GeographyCollectionImplementation(
      SpatialImplementation creator,
      params Geography[] geography)
      : this(CoordinateSystem.DefaultGeography, creator, geography)
    {
    }

    public override bool IsEmpty => this.geographyArray.Length == 0;

    public override ReadOnlyCollection<Geography> Geographies => new ReadOnlyCollection<Geography>((IList<Geography>) this.geographyArray);

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "base does the validation")]
    public override void SendTo(GeographyPipeline pipeline)
    {
      base.SendTo(pipeline);
      pipeline.BeginGeography(SpatialType.Collection);
      for (int index = 0; index < this.geographyArray.Length; ++index)
        this.geographyArray[index].SendTo(pipeline);
      pipeline.EndGeography();
    }
  }
}
