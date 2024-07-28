// Decompiled with JetBrains decompiler
// Type: Nest.GeoDistanceQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GeoDistanceQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<GeoDistanceQueryDescriptor<T>, IGeoDistanceQuery, T>,
    IGeoDistanceQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => GeoDistanceQuery.IsConditionless((IGeoDistanceQuery) this);

    Nest.Distance IGeoDistanceQuery.Distance { get; set; }

    GeoDistanceType? IGeoDistanceQuery.DistanceType { get; set; }

    GeoLocation IGeoDistanceQuery.Location { get; set; }

    GeoValidationMethod? IGeoDistanceQuery.ValidationMethod { get; set; }

    bool? IGeoDistanceQuery.IgnoreUnmapped { get; set; }

    public GeoDistanceQueryDescriptor<T> Location(GeoLocation location) => this.Assign<GeoLocation>(location, (Action<IGeoDistanceQuery, GeoLocation>) ((a, v) => a.Location = v));

    public GeoDistanceQueryDescriptor<T> Location(double lat, double lon) => this.Assign<GeoLocation>(new GeoLocation(lat, lon), (Action<IGeoDistanceQuery, GeoLocation>) ((a, v) => a.Location = v));

    public GeoDistanceQueryDescriptor<T> Distance(Nest.Distance distance) => this.Assign<Nest.Distance>(distance, (Action<IGeoDistanceQuery, Nest.Distance>) ((a, v) => a.Distance = v));

    public GeoDistanceQueryDescriptor<T> Distance(double distance, DistanceUnit unit) => this.Assign<Nest.Distance>(new Nest.Distance(distance, unit), (Action<IGeoDistanceQuery, Nest.Distance>) ((a, v) => a.Distance = v));

    public GeoDistanceQueryDescriptor<T> DistanceType(GeoDistanceType? type) => this.Assign<GeoDistanceType?>(type, (Action<IGeoDistanceQuery, GeoDistanceType?>) ((a, v) => a.DistanceType = v));

    public GeoDistanceQueryDescriptor<T> ValidationMethod(GeoValidationMethod? validation) => this.Assign<GeoValidationMethod?>(validation, (Action<IGeoDistanceQuery, GeoValidationMethod?>) ((a, v) => a.ValidationMethod = v));

    public GeoDistanceQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IGeoDistanceQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}
