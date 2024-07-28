// Decompiled with JetBrains decompiler
// Type: Nest.DescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Nest
{
  public abstract class DescriptorBase<TDescriptor, TInterface> : IDescriptor
    where TDescriptor : DescriptorBase<TDescriptor, TInterface>, TInterface
    where TInterface : class
  {
    private readonly TDescriptor _self;

    protected DescriptorBase() => this._self = (TDescriptor) this;

    [IgnoreDataMember]
    protected TInterface Self => (TInterface) this._self;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected TDescriptor Assign<TValue>(TValue value, Action<TInterface, TValue> assigner) => Fluent.Assign<TDescriptor, TInterface, TValue>(this._self, value, assigner);

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
