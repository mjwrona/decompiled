// Decompiled with JetBrains decompiler
// Type: Nest.FielddataDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class FielddataDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IFielddata
    where TDescriptor : FielddataDescriptorBase<TDescriptor, TInterface>, TInterface
    where TInterface : class, IFielddata
  {
    IFielddataFilter IFielddata.Filter { get; set; }

    FielddataLoading? IFielddata.Loading { get; set; }

    public TDescriptor Filter(
      Func<FielddataFilterDescriptor, IFielddataFilter> filterSelector)
    {
      return this.Assign<IFielddataFilter>(filterSelector(new FielddataFilterDescriptor()), (Action<TInterface, IFielddataFilter>) ((a, v) => a.Filter = v));
    }

    public TDescriptor Loading(FielddataLoading? loading) => this.Assign<FielddataLoading?>(loading, (Action<TInterface, FielddataLoading?>) ((a, v) => a.Loading = v));
  }
}
