// Decompiled with JetBrains decompiler
// Type: Nest.DistanceFeatureQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DistanceFeatureQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<DistanceFeatureQueryDescriptor<T>, IDistanceFeatureQuery, T>,
    IDistanceFeatureQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    Union<GeoCoordinate, DateMath> IDistanceFeatureQuery.Origin { get; set; }

    Union<Distance, Time> IDistanceFeatureQuery.Pivot { get; set; }

    protected override bool Conditionless => DistanceFeatureQuery.IsConditionless((IDistanceFeatureQuery) this);

    public DistanceFeatureQueryDescriptor<T> Origin(DateMath origin) => this.Assign<DateMath>(origin, (Action<IDistanceFeatureQuery, DateMath>) ((a, v) => a.Origin = (Union<GeoCoordinate, DateMath>) v));

    public DistanceFeatureQueryDescriptor<T> Origin(GeoCoordinate origin) => this.Assign<GeoCoordinate>(origin, (Action<IDistanceFeatureQuery, GeoCoordinate>) ((a, v) => a.Origin = (Union<GeoCoordinate, DateMath>) v));

    public DistanceFeatureQueryDescriptor<T> Pivot(Time pivot) => this.Assign<Time>(pivot, (Action<IDistanceFeatureQuery, Time>) ((a, v) => a.Pivot = (Union<Distance, Time>) v));

    public DistanceFeatureQueryDescriptor<T> Pivot(Distance pivot) => this.Assign<Distance>(pivot, (Action<IDistanceFeatureQuery, Distance>) ((a, v) => a.Pivot = (Union<Distance, Time>) v));
  }
}
