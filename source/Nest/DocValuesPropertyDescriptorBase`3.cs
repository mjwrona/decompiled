// Decompiled with JetBrains decompiler
// Type: Nest.DocValuesPropertyDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class DocValuesPropertyDescriptorBase<TDescriptor, TInterface, T> : 
    CorePropertyDescriptorBase<TDescriptor, TInterface, T>,
    IDocValuesProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where TDescriptor : DocValuesPropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, IDocValuesProperty
    where T : class
  {
    protected DocValuesPropertyDescriptorBase(FieldType type)
      : base(type)
    {
    }

    bool? IDocValuesProperty.DocValues { get; set; }

    public TDescriptor DocValues(bool? docValues = true) => this.Assign<bool?>(docValues, (Action<TInterface, bool?>) ((a, v) => a.DocValues = v));
  }
}
