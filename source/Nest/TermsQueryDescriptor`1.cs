// Decompiled with JetBrains decompiler
// Type: Nest.TermsQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TermsQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<TermsQueryDescriptor<T>, ITermsQuery, T>,
    ITermsQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => TermsQuery.IsConditionless((ITermsQuery) this);

    IEnumerable<object> ITermsQuery.Terms { get; set; }

    IFieldLookup ITermsQuery.TermsLookup { get; set; }

    public TermsQueryDescriptor<T> TermsLookup<TOther>(
      Func<FieldLookupDescriptor<TOther>, IFieldLookup> selector)
      where TOther : class
    {
      return this.Assign<IFieldLookup>(selector(new FieldLookupDescriptor<TOther>()), (Action<ITermsQuery, IFieldLookup>) ((a, v) => a.TermsLookup = v));
    }

    public TermsQueryDescriptor<T> Terms<TValue>(IEnumerable<TValue> terms) => this.Assign<IEnumerable<object>>(terms != null ? terms.Cast<object>() : (IEnumerable<object>) null, (Action<ITermsQuery, IEnumerable<object>>) ((a, v) => a.Terms = v));

    public TermsQueryDescriptor<T> Terms<TValue>(params TValue[] terms) => this.Assign<TValue[]>(terms, (Action<ITermsQuery, TValue[]>) ((a, v) =>
    {
      if (v != null && v.Length == 1 && typeof (IEnumerable).IsAssignableFrom(typeof (TValue)) && typeof (TValue) != typeof (string))
        a.Terms = ((IEnumerable<TValue>) v).First<TValue>() is IEnumerable source2 ? source2.Cast<object>() : (IEnumerable<object>) null;
      else
        a.Terms = v != null ? v.Cast<object>() : (IEnumerable<object>) null;
    }));
  }
}
