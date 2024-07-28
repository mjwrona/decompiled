// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateContainsOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  [XmlType("contains")]
  public class PredicateContainsOperator : PredicateFieldComparisonOperator
  {
    protected override string WiqlOperator => "CONTAINS";

    protected override string InverseWiqlOperator => "NOT CONTAINS";

    public override void Validate(IPredicateValidationHelper validator)
    {
      base.Validate(validator);
      switch (this.GetFieldType(validator))
      {
        case InternalFieldType.String:
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.TreePath:
        case InternalFieldType.History:
          if (!string.IsNullOrEmpty((string) this.Value))
            break;
          throw new InvalidPredicateException("Contains operator cannot operate with a null or empty operand value.");
        default:
          throw new InvalidPredicateException("Contains operator can only be used with string fields.");
      }
    }

    public override bool Evaluate(IPredicateEvaluationHelper helper)
    {
      string fieldValue = helper.GetFieldValue(this.FieldId) as string;
      return !string.IsNullOrEmpty(fieldValue) && fieldValue.IndexOf((string) this.Value, StringComparison.OrdinalIgnoreCase) >= 0;
    }
  }
}
