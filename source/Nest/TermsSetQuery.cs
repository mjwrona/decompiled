// Decompiled with JetBrains decompiler
// Type: Nest.TermsSetQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class TermsSetQuery : FieldNameQueryBase, ITermsSetQuery, IFieldNameQuery, IQuery
  {
    public Field MinimumShouldMatchField { get; set; }

    public IScript MinimumShouldMatchScript { get; set; }

    public IEnumerable<object> Terms { get; set; }

    protected override bool Conditionless => TermsSetQuery.IsConditionless((ITermsSetQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.TermsSet = (ITermsSetQuery) this;

    internal static bool IsConditionless(ITermsSetQuery q)
    {
      if (q.Field.IsConditionless() || q.Terms == null || !q.Terms.HasAny<object>() || q.Terms.All<object>((Func<object, bool>) (t => t == null || (t is string str ? new bool?(str.IsNullOrEmpty()) : new bool?()).GetValueOrDefault(false))))
        return true;
      return q.MinimumShouldMatchField.IsConditionless() && q.MinimumShouldMatchScript == null;
    }
  }
}
