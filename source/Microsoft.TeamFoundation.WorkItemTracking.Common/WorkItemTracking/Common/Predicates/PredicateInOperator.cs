// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateInOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  [XmlType("in")]
  public class PredicateInOperator : PredicateFieldComparisonOperator
  {
    protected override string WiqlOperator => "IN";

    protected override string InverseWiqlOperator => "NOT IN";

    public override void Validate(IPredicateValidationHelper validator)
    {
      base.Validate(validator);
      if (!(this.Value is string[] strArray))
        throw new InvalidPredicateException("In operator can only be used with string array.");
      if (strArray.Length < 1)
        throw new InvalidPredicateException("In operator cannot operate with an empty set of values.");
    }

    protected override void ValidateFieldType(IPredicateValidationHelper validator)
    {
    }

    public override bool Evaluate(IPredicateEvaluationHelper helper)
    {
      object fieldValue = helper.GetFieldValue(this.FieldId);
      if (fieldValue == null)
        return false;
      string str = Convert.ToString(fieldValue);
      return !string.IsNullOrEmpty(str) && ((IEnumerable<string>) (string[]) this.Value).Contains<string>(str, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
