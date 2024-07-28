// Decompiled with JetBrains decompiler
// Type: Nest.DescriptorPromiseBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.ComponentModel;

namespace Nest
{
  public abstract class DescriptorPromiseBase<TDescriptor, TValue> : IDescriptor, IPromise<TValue>
    where TDescriptor : DescriptorPromiseBase<TDescriptor, TValue>
    where TValue : class
  {
    internal readonly TValue PromisedValue;

    protected DescriptorPromiseBase(TValue instance)
    {
      this.PromisedValue = instance;
      this.Self = (TDescriptor) this;
    }

    TValue IPromise<TValue>.Value => this.PromisedValue;

    protected TDescriptor Self { get; }

    protected TDescriptor Assign(Action<TValue> assigner)
    {
      assigner(this.PromisedValue);
      return this.Self;
    }

    protected TDescriptor Assign<TNewValue>(TNewValue value, Action<TValue, TNewValue> assigner)
    {
      assigner(this.PromisedValue, value);
      return this.Self;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj) => base.Equals(obj);

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode() => base.GetHashCode();

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override string ToString() => base.ToString();
  }
}
