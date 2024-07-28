// Decompiled with JetBrains decompiler
// Type: Nest.RangePropertyDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class RangePropertyDescriptorBase<TDescriptor, TInterface, T> : 
    DocValuesPropertyDescriptorBase<TDescriptor, TInterface, T>,
    IRangeProperty,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where TDescriptor : RangePropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, IRangeProperty
    where T : class
  {
    protected RangePropertyDescriptorBase(RangeType type)
      : base(type.ToFieldType())
    {
    }

    double? IRangeProperty.Boost { get; set; }

    bool? IRangeProperty.Coerce { get; set; }

    bool? IRangeProperty.Index { get; set; }

    public TDescriptor Coerce(bool? coerce = true) => this.Assign<bool?>(coerce, (Action<TInterface, bool?>) ((a, v) => a.Coerce = v));

    [Obsolete("The server always treated this as a noop and has been removed in 7.10")]
    public TDescriptor Boost(double? boost) => this.Assign<double?>(boost, (Action<TInterface, double?>) ((a, v) => a.Boost = v));

    public TDescriptor Index(bool? index = true) => this.Assign<bool?>(index, (Action<TInterface, bool?>) ((a, v) => a.Index = v));
  }
}
