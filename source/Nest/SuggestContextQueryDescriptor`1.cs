// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContextQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SuggestContextQueryDescriptor<T> : 
    DescriptorBase<SuggestContextQueryDescriptor<T>, ISuggestContextQuery>,
    ISuggestContextQuery
  {
    double? ISuggestContextQuery.Boost { get; set; }

    Nest.Context ISuggestContextQuery.Context { get; set; }

    Union<Distance[], int[]> ISuggestContextQuery.Neighbours { get; set; }

    Union<Distance, int> ISuggestContextQuery.Precision { get; set; }

    bool? ISuggestContextQuery.Prefix { get; set; }

    public SuggestContextQueryDescriptor<T> Prefix(bool? prefix = true) => this.Assign<bool?>(prefix, (Action<ISuggestContextQuery, bool?>) ((a, v) => a.Prefix = v));

    public SuggestContextQueryDescriptor<T> Boost(double? boost) => this.Assign<double?>(boost, (Action<ISuggestContextQuery, double?>) ((a, v) => a.Boost = v));

    public SuggestContextQueryDescriptor<T> Context(string context) => this.Assign<string>(context, (Action<ISuggestContextQuery, string>) ((a, v) => a.Context = (Nest.Context) v));

    public SuggestContextQueryDescriptor<T> Context(GeoLocation context) => this.Assign<GeoLocation>(context, (Action<ISuggestContextQuery, GeoLocation>) ((a, v) => a.Context = (Nest.Context) v));

    public SuggestContextQueryDescriptor<T> Precision(Distance precision) => this.Assign<Distance>(precision, (Action<ISuggestContextQuery, Distance>) ((a, v) => a.Precision = (Union<Distance, int>) v));

    public SuggestContextQueryDescriptor<T> Precision(int? precision) => this.Assign<int?>(precision, (Action<ISuggestContextQuery, int?>) ((a, v) =>
    {
      ISuggestContextQuery suggestContextQuery = a;
      int? nullable = v;
      Union<Distance, int> valueOrDefault = nullable.HasValue ? (Union<Distance, int>) nullable.GetValueOrDefault() : (Union<Distance, int>) null;
      suggestContextQuery.Precision = valueOrDefault;
    }));

    public SuggestContextQueryDescriptor<T> Neighbours(params int[] neighbours) => this.Assign<int[]>(neighbours, (Action<ISuggestContextQuery, int[]>) ((a, v) => a.Neighbours = (Union<Distance[], int[]>) v));

    public SuggestContextQueryDescriptor<T> Neighbours(params Distance[] neighbours) => this.Assign<Distance[]>(neighbours, (Action<ISuggestContextQuery, Distance[]>) ((a, v) => a.Neighbours = (Union<Distance[], int[]>) v));
  }
}
