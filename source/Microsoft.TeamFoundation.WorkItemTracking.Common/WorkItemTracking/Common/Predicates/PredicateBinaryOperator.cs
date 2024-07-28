// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateBinaryOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  public abstract class PredicateBinaryOperator : PredicateOperator
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
    public PredicateOperator[] Operands { get; set; }

    protected abstract string WiqlOperator { get; }

    protected abstract string InverseWiqlOperator { get; }

    public override void Validate(IPredicateValidationHelper validator)
    {
      if (this.Operands == null || this.Operands.Length < 2)
        throw new InvalidOperationException("");
      foreach (PredicateOperator operand in this.Operands)
        operand.Validate(validator);
    }

    protected internal override string ToWIQLPredicate(
      IPredicateValidationHelper validator,
      bool inverse)
    {
      return ((IEnumerable<PredicateOperator>) this.Operands).Any<PredicateOperator>() ? "(" + string.Join(" " + (inverse ? this.InverseWiqlOperator : this.WiqlOperator) + " ", ((IEnumerable<PredicateOperator>) this.Operands).Select<PredicateOperator, string>((Func<PredicateOperator, string>) (op => op.ToWIQLPredicate(validator, inverse))).ToArray<string>()) + ")" : string.Empty;
    }

    public override IEnumerable<int> GetReferencedFields() => this.Operands == null ? Enumerable.Empty<int>() : ((IEnumerable<PredicateOperator>) this.Operands).SelectMany<PredicateOperator, int>((Func<PredicateOperator, IEnumerable<int>>) (op => op.GetReferencedFields()));

    public override void FixFieldReferences(IPredicateValidationHelper validationHelper)
    {
      if (this.Operands == null)
        return;
      foreach (PredicateOperator operand in this.Operands)
        operand.FixFieldReferences(validationHelper);
    }

    public override bool Walk(
      PredicateOperator parent,
      PredicateVisitor before,
      PredicateVisitor after)
    {
      return base.Walk(parent, before, (PredicateVisitor) ((c, p) =>
      {
        if (this.Operands != null)
        {
          foreach (PredicateOperator operand in this.Operands)
          {
            if (!operand.Walk((PredicateOperator) this, before, after))
              return false;
          }
        }
        return after == null || after((PredicateOperator) this, parent);
      }));
    }

    public override PredicateOperator Clone()
    {
      PredicateBinaryOperator predicateBinaryOperator = base.Clone() as PredicateBinaryOperator;
      if (this.Operands != null)
        predicateBinaryOperator.Operands = ((IEnumerable<PredicateOperator>) this.Operands).Select<PredicateOperator, PredicateOperator>((Func<PredicateOperator, PredicateOperator>) (op => op.Clone())).ToArray<PredicateOperator>();
      return (PredicateOperator) predicateBinaryOperator;
    }
  }
}
