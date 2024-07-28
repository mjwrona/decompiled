// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateUnaryOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  public abstract class PredicateUnaryOperator : PredicateOperator
  {
    [XmlElement(typeof (PredicateAndOperator))]
    [XmlElement(typeof (PredicateOrOperator))]
    [XmlElement(typeof (PredicateNotOperator))]
    [XmlElement(typeof (PredicateEqualsOperator))]
    [XmlElement(typeof (PredicateNotEqualsOperator))]
    [XmlElement(typeof (PredicateGreaterThanOperator))]
    [XmlElement(typeof (PredicateGreaterOrEqualsOperator))]
    [XmlElement(typeof (PredicateLessThanOperator))]
    [XmlElement(typeof (PredicateLessOrEqualsOperator))]
    [XmlElement(typeof (PredicateContainsOperator))]
    [XmlElement(typeof (PredicateNotContainsOperator))]
    [XmlElement(typeof (PredicateUnderOperator))]
    [XmlElement(typeof (PredicateNotUnderOperator))]
    [XmlElement(typeof (PredicateInOperator))]
    [XmlElement(typeof (PredicateInGroupOperator))]
    [XmlElement(typeof (PredicateInListOperator))]
    public PredicateOperator Operand { get; set; }

    public override bool Evaluate(IPredicateEvaluationHelper helper) => this.Operand.Evaluate(helper);

    public override void Validate(IPredicateValidationHelper validator)
    {
      if (this.Operand == null)
        throw new InvalidOperationException();
      this.Operand.Validate(validator);
    }

    protected internal override string ToWIQLPredicate(
      IPredicateValidationHelper validator,
      bool inverse)
    {
      return this.Operand != null ? this.Operand.ToWIQLPredicate(validator, inverse) : string.Empty;
    }

    public override IEnumerable<int> GetReferencedFields() => this.Operand == null ? Enumerable.Empty<int>() : this.Operand.GetReferencedFields();

    public override void FixFieldReferences(IPredicateValidationHelper validationHelper)
    {
      if (this.Operand == null)
        return;
      this.Operand.FixFieldReferences(validationHelper);
    }

    public override bool Walk(
      PredicateOperator parent,
      PredicateVisitor before,
      PredicateVisitor after)
    {
      return base.Walk(parent, before, (PredicateVisitor) ((c, p) =>
      {
        if (this.Operand != null && !this.Operand.Walk((PredicateOperator) this, before, after))
          return false;
        return after == null || after((PredicateOperator) this, parent);
      }));
    }
  }
}
