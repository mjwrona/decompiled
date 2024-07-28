// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateInGroupOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  [XmlType("in-group")]
  public class PredicateInGroupOperator : PredicateFieldComparisonOperator
  {
    protected override string WiqlOperator => "IN GROUP";

    protected override string InverseWiqlOperator => "NOT IN GROUP";

    public override void Validate(IPredicateValidationHelper validator)
    {
      base.Validate(validator);
      if (this.Value == null || !(this.Value is string) || string.IsNullOrEmpty((string) this.Value))
        throw new InvalidPredicateException("In Group operator cannot operate with a null or empty operand value.");
      if (validator.GetFieldType(this.Field) != InternalFieldType.String)
        throw new InvalidPredicateException("In Group operator can only work with string fields.");
      if (validator.GetFieldId(this.Field) != 25)
        throw new InvalidPredicateException("Not supported");
    }

    protected override void ValidateFieldType(IPredicateValidationHelper validator)
    {
    }

    public override bool Evaluate(IPredicateEvaluationHelper helper)
    {
      string fieldValue = helper.GetFieldValue(this.FieldId) as string;
      return !string.IsNullOrEmpty(fieldValue) && helper.IsWorkItemTypeInCategory(fieldValue, (string) this.Value);
    }
  }
}
