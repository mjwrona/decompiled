// Decompiled with JetBrains decompiler
// Type: Nest.QueryDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class QueryDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IQuery
    where TDescriptor : QueryDescriptorBase<TDescriptor, TInterface>, TInterface
    where TInterface : class, IQuery
  {
    protected abstract bool Conditionless { get; }

    double? IQuery.Boost { get; set; }

    bool IQuery.Conditionless => this.Conditionless;

    bool IQuery.IsStrict { get; set; }

    bool IQuery.IsVerbatim { get; set; }

    bool IQuery.IsWritable => this.Self.IsVerbatim || !this.Self.Conditionless;

    string IQuery.Name { get; set; }

    public TDescriptor Name(string name) => this.Assign<string>(name, (Action<TInterface, string>) ((a, v) => a.Name = v));

    public TDescriptor Boost(double? boost) => this.Assign<double?>(boost, (Action<TInterface, double?>) ((a, v) => a.Boost = v));

    public TDescriptor Verbatim(bool verbatim = true) => this.Assign<bool>(verbatim, (Action<TInterface, bool>) ((a, v) => a.IsVerbatim = v));

    public TDescriptor Strict(bool strict = true) => this.Assign<bool>(strict, (Action<TInterface, bool>) ((a, v) => a.IsStrict = v));
  }
}
