// Decompiled with JetBrains decompiler
// Type: Nest.SuggestDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public abstract class SuggestDescriptorBase<TDescriptor, TInterface, T> : 
    DescriptorBase<TDescriptor, TInterface>,
    ISuggester
    where TDescriptor : SuggestDescriptorBase<TDescriptor, TInterface, T>, TInterface, ISuggester
    where TInterface : class, ISuggester
  {
    string ISuggester.Analyzer { get; set; }

    Nest.Field ISuggester.Field { get; set; }

    int? ISuggester.Size { get; set; }

    public TDescriptor Size(int? size) => this.Assign<int?>(size, (Action<TInterface, int?>) ((a, v) => a.Size = v));

    public TDescriptor Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<TInterface, string>) ((a, v) => a.Analyzer = v));

    public TDescriptor Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<TInterface, Nest.Field>) ((a, v) => a.Field = v));

    public TDescriptor Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<TInterface, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
  }
}
