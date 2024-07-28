// Decompiled with JetBrains decompiler
// Type: Nest.TermsSetQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class TermsSetQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<TermsSetQueryDescriptor<T>, ITermsSetQuery, T>,
    ITermsSetQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => TermsSetQuery.IsConditionless((ITermsSetQuery) this);

    Nest.Field ITermsSetQuery.MinimumShouldMatchField { get; set; }

    IScript ITermsSetQuery.MinimumShouldMatchScript { get; set; }

    IEnumerable<object> ITermsSetQuery.Terms { get; set; }

    public TermsSetQueryDescriptor<T> Terms<TValue>(IEnumerable<TValue> terms) => this.Assign<IEnumerable<object>>(terms != null ? terms.Cast<object>() : (IEnumerable<object>) null, (Action<ITermsSetQuery, IEnumerable<object>>) ((a, v) => a.Terms = v));

    public TermsSetQueryDescriptor<T> Terms<TValue>(params TValue[] terms) => this.Assign<TValue[]>(terms, (Action<ITermsSetQuery, TValue[]>) ((a, v) =>
    {
      if (v != null && v.Length == 1 && typeof (IEnumerable).IsAssignableFrom(typeof (TValue)) && typeof (TValue) != typeof (string))
        a.Terms = ((IEnumerable<TValue>) v).First<TValue>() is IEnumerable source2 ? source2.Cast<object>() : (IEnumerable<object>) null;
      else
        a.Terms = v != null ? v.Cast<object>() : (IEnumerable<object>) null;
    }));

    public TermsSetQueryDescriptor<T> MinimumShouldMatchField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ITermsSetQuery, Nest.Field>) ((a, v) => a.MinimumShouldMatchField = v));

    public TermsSetQueryDescriptor<T> MinimumShouldMatchField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ITermsSetQuery, Expression<Func<T, TValue>>>) ((a, v) => a.MinimumShouldMatchField = (Nest.Field) (Expression) v));
    }

    public TermsSetQueryDescriptor<T> MinimumShouldMatchScript(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<ITermsSetQuery, Func<ScriptDescriptor, IScript>>) ((a, v) => a.MinimumShouldMatchScript = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }
  }
}
