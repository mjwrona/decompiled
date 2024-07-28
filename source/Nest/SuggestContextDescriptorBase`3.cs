// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContextDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nest
{
  public abstract class SuggestContextDescriptorBase<TDescriptor, TInterface, T> : 
    DescriptorBase<TDescriptor, TInterface>,
    ISuggestContext
    where TDescriptor : SuggestContextDescriptorBase<TDescriptor, TInterface, T>, TInterface, ISuggestContext
    where TInterface : class, ISuggestContext
  {
    [IgnoreDataMember]
    protected abstract string Type { get; }

    string ISuggestContext.Name { get; set; }

    Field ISuggestContext.Path { get; set; }

    string ISuggestContext.Type => this.Type;

    public TDescriptor Name(string name) => this.Assign<string>(name, (Action<TInterface, string>) ((a, v) => a.Name = v));

    public TDescriptor Path(Field field) => this.Assign<Field>(field, (Action<TInterface, Field>) ((a, v) => a.Path = v));

    public TDescriptor Path<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<TInterface, Expression<Func<T, TValue>>>) ((a, v) => a.Path = (Field) (Expression) v));
  }
}
