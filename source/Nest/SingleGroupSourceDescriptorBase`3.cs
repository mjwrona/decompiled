// Decompiled with JetBrains decompiler
// Type: Nest.SingleGroupSourceDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public abstract class SingleGroupSourceDescriptorBase<TDescriptor, TInterface, T> : 
    DescriptorBase<TDescriptor, TInterface>,
    ISingleGroupSource
    where TDescriptor : SingleGroupSourceDescriptorBase<TDescriptor, TInterface, T>, TInterface
    where TInterface : class, ISingleGroupSource
  {
    Nest.Field ISingleGroupSource.Field { get; set; }

    IScript ISingleGroupSource.Script { get; set; }

    public TDescriptor Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<TInterface, Nest.Field>) ((a, v) => a.Field = v));

    public TDescriptor Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<TInterface, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public TDescriptor Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<TInterface, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public TDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<TInterface, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
  }
}
