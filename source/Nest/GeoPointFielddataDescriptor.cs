// Decompiled with JetBrains decompiler
// Type: Nest.GeoPointFielddataDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GeoPointFielddataDescriptor : 
    FielddataDescriptorBase<GeoPointFielddataDescriptor, IGeoPointFielddata>,
    IGeoPointFielddata,
    IFielddata
  {
    GeoPointFielddataFormat? IGeoPointFielddata.Format { get; set; }

    Distance IGeoPointFielddata.Precision { get; set; }

    public GeoPointFielddataDescriptor Format(GeoPointFielddataFormat? format) => this.Assign<GeoPointFielddataFormat?>(format, (Action<IGeoPointFielddata, GeoPointFielddataFormat?>) ((a, v) => a.Format = v));

    public GeoPointFielddataDescriptor Precision(Distance distance) => this.Assign<Distance>(distance, (Action<IGeoPointFielddata, Distance>) ((a, v) => a.Precision = v));
  }
}
