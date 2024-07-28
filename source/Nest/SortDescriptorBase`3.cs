// Decompiled with JetBrains decompiler
// Type: Nest.SortDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class SortDescriptorBase<TDescriptor, TInterface, T> : 
    DescriptorBase<TDescriptor, TInterface>,
    ISort
    where TDescriptor : SortDescriptorBase<TDescriptor, TInterface, T>, TInterface, ISort
    where TInterface : class, ISort
    where T : class
  {
    protected abstract Field SortKey { get; }

    string ISort.Format { get; set; }

    object ISort.Missing { get; set; }

    SortMode? ISort.Mode { get; set; }

    Nest.NumericType? ISort.NumericType { get; set; }

    INestedSort ISort.Nested { get; set; }

    SortOrder? ISort.Order { get; set; }

    Field ISort.SortKey => this.SortKey;

    public virtual TDescriptor Ascending() => this.Assign<SortOrder>(SortOrder.Ascending, (Action<TInterface, SortOrder>) ((a, v) => a.Order = new SortOrder?(v)));

    public virtual TDescriptor Descending() => this.Assign<SortOrder>(SortOrder.Descending, (Action<TInterface, SortOrder>) ((a, v) => a.Order = new SortOrder?(v)));

    public virtual TDescriptor Format(string format) => this.Assign<string>(format, (Action<TInterface, string>) ((a, v) => a.Format = v));

    public virtual TDescriptor Order(SortOrder? order) => this.Assign<SortOrder?>(order, (Action<TInterface, SortOrder?>) ((a, v) => a.Order = v));

    public virtual TDescriptor NumericType(Nest.NumericType? numericType) => this.Assign<Nest.NumericType?>(numericType, (Action<TInterface, Nest.NumericType?>) ((a, v) => a.NumericType = v));

    public virtual TDescriptor Mode(SortMode? mode) => this.Assign<SortMode?>(mode, (Action<TInterface, SortMode?>) ((a, v) => a.Mode = v));

    public virtual TDescriptor MissingLast() => this.Assign<string>("_last", (Action<TInterface, string>) ((a, v) => a.Missing = (object) v));

    public virtual TDescriptor MissingFirst() => this.Assign<string>("_first", (Action<TInterface, string>) ((a, v) => a.Missing = (object) v));

    public virtual TDescriptor Missing(object value) => this.Assign<object>(value, (Action<TInterface, object>) ((a, v) => a.Missing = v));

    public virtual TDescriptor Nested(
      Func<NestedSortDescriptor<T>, INestedSort> selector)
    {
      return this.Assign<Func<NestedSortDescriptor<T>, INestedSort>>(selector, (Action<TInterface, Func<NestedSortDescriptor<T>, INestedSort>>) ((a, v) => a.Nested = v != null ? v(new NestedSortDescriptor<T>()) : (INestedSort) null));
    }
  }
}
