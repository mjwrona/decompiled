// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateUnderOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  [XmlType("under")]
  public class PredicateUnderOperator : PredicateFieldComparisonOperator
  {
    protected override string WiqlOperator => "UNDER";

    protected override string InverseWiqlOperator => "NOT UNDER";

    public override void Validate(IPredicateValidationHelper validator)
    {
      base.Validate(validator);
      if (this.GetFieldType(validator) == InternalFieldType.TreePath)
      {
        if (this.Value == null || !(this.Value is string) || string.IsNullOrEmpty((string) this.Value))
          throw new InvalidPredicateException("Under operator cannot operate with a null or empty operand value.");
      }
      else
      {
        switch (validator.GetFieldId(this.Field))
        {
          case -104:
          case -2:
            if (this.Value is int)
              break;
            throw new InvalidPredicateException("Invalid Tree Id.");
          default:
            throw new InvalidPredicateException("Under operator can only be used with tree path or tree id fields.");
        }
      }
    }

    public override bool Evaluate(IPredicateEvaluationHelper helper)
    {
      switch (this.FieldId)
      {
        case -105:
        case -7:
          string fieldValue1 = helper.GetFieldValue(this.FieldId) as string;
          string str = (string) this.Value;
          if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(fieldValue1))
            return false;
          return (fieldValue1.TrimEnd('\\') + "\\").StartsWith(str.TrimEnd('\\') + "\\", StringComparison.OrdinalIgnoreCase);
        case -104:
        case -2:
          object fieldValue2 = helper.GetFieldValue(this.FieldId);
          return fieldValue2 != null && helper.IsTreeNodeUnder((int) this.Value, Convert.ToInt32(fieldValue2));
        default:
          return false;
      }
    }

    protected override string GetWiqlFieldReferenceName(IPredicateValidationHelper validator)
    {
      if (validator.GetFieldType(this.Field) == InternalFieldType.TreePath)
        return base.GetWiqlFieldReferenceName(validator);
      return validator.GetFieldId(this.Field) != -2 ? "System.IterationPath" : "System.AreaPath";
    }

    protected override object GetWiqlFieldValue(IPredicateValidationHelper validator) => validator.GetFieldType(this.Field) == InternalFieldType.TreePath ? base.GetWiqlFieldValue(validator) : (object) validator.GetTreePath((int) this.Value);
  }
}
