// Decompiled with JetBrains decompiler
// Type: Nest.BoundingBoxDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class BoundingBoxDescriptor : 
    DescriptorBase<BoundingBoxDescriptor, IBoundingBox>,
    IBoundingBox
  {
    GeoLocation IBoundingBox.BottomRight { get; set; }

    GeoLocation IBoundingBox.TopLeft { get; set; }

    string IBoundingBox.WellKnownText { get; set; }

    public BoundingBoxDescriptor TopLeft(GeoLocation topLeft) => this.Assign<GeoLocation>(topLeft, (Action<IBoundingBox, GeoLocation>) ((a, v) => a.TopLeft = v));

    public BoundingBoxDescriptor TopLeft(double lat, double lon) => this.Assign<GeoLocation>(new GeoLocation(lat, lon), (Action<IBoundingBox, GeoLocation>) ((a, v) => a.TopLeft = v));

    public BoundingBoxDescriptor BottomRight(GeoLocation bottomRight) => this.Assign<GeoLocation>(bottomRight, (Action<IBoundingBox, GeoLocation>) ((a, v) => a.BottomRight = v));

    public BoundingBoxDescriptor BottomRight(double lat, double lon) => this.Assign<GeoLocation>(new GeoLocation(lat, lon), (Action<IBoundingBox, GeoLocation>) ((a, v) => a.BottomRight = v));

    public BoundingBoxDescriptor WellKnownText(string wkt) => this.Assign<string>(wkt, (Action<IBoundingBox, string>) ((a, v) => a.WellKnownText = v));
  }
}
