// Decompiled with JetBrains decompiler
// Type: Nest.TermsQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class TermsQuery : FieldNameQueryBase, ITermsQuery, IFieldNameQuery, IQuery
  {
    public IEnumerable<object> Terms { get; set; }

    public IFieldLookup TermsLookup { get; set; }

    protected override bool Conditionless => TermsQuery.IsConditionless((ITermsQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Terms = (ITermsQuery) this;

    internal static bool IsConditionless(ITermsQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      if (q.Terms != null && q.Terms.HasAny<object>() && !q.Terms.All<object>((Func<object, bool>) (t => t == null || (t is string str ? new bool?(str.IsNullOrEmpty()) : new bool?()).GetValueOrDefault(false))))
        return false;
      return q.TermsLookup == null || q.TermsLookup.Id == (Id) null || q.TermsLookup.Path.IsConditionless() || q.TermsLookup.Index == (IndexName) null;
    }
  }
}
