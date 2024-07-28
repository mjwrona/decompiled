// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  public abstract class PredicateOperator : ICloneable
  {
    public virtual bool Evaluate(IPredicateEvaluationHelper helper) => false;

    public abstract void Validate(IPredicateValidationHelper validator);

    public virtual string ToWIQLPredicate(IPredicateValidationHelper validator) => this.ToWIQLPredicate(validator, false);

    protected internal abstract string ToWIQLPredicate(
      IPredicateValidationHelper validator,
      bool inverse);

    public virtual IEnumerable<int> GetReferencedFields() => Enumerable.Empty<int>();

    public virtual void FixFieldReferences(IPredicateValidationHelper validationHelper)
    {
    }

    public virtual bool Walk(
      PredicateOperator parent,
      PredicateVisitor before,
      PredicateVisitor after)
    {
      if (before == null || !before(this, parent))
        return false;
      return after == null || after(this, parent);
    }

    public virtual PredicateOperator Clone() => this.MemberwiseClone() as PredicateOperator;

    object ICloneable.Clone() => (object) this.Clone();
  }
}
