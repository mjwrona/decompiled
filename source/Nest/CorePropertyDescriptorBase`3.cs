// Decompiled with JetBrains decompiler
// Type: Nest.CorePropertyDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class CorePropertyDescriptorBase<TDescriptor, TInterface, T> : 
    PropertyDescriptorBase<TDescriptor, TInterface, T>,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where TDescriptor : CorePropertyDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, ICoreProperty
    where T : class
  {
    protected CorePropertyDescriptorBase(FieldType type)
      : base(type)
    {
    }

    Nest.Fields ICoreProperty.CopyTo { get; set; }

    IProperties ICoreProperty.Fields { get; set; }

    string ICoreProperty.Similarity { get; set; }

    bool? ICoreProperty.Store { get; set; }

    public TDescriptor Store(bool? store = true) => this.Assign<bool?>(store, (Action<TInterface, bool?>) ((a, v) => a.Store = v));

    public TDescriptor Fields(
      Func<PropertiesDescriptor<T>, IPromise<IProperties>> selector)
    {
      return this.Assign<Func<PropertiesDescriptor<T>, IPromise<IProperties>>>(selector, (Action<TInterface, Func<PropertiesDescriptor<T>, IPromise<IProperties>>>) ((a, v) => a.Fields = v != null ? v(new PropertiesDescriptor<T>())?.Value : (IProperties) null));
    }

    public TDescriptor Similarity(string similarity) => this.Assign<string>(similarity, (Action<TInterface, string>) ((a, v) => a.Similarity = v));

    public TDescriptor CopyTo(Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<TInterface, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.CopyTo = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));
  }
}
