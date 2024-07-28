// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.PredicateFieldComparisonOperator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  public abstract class PredicateFieldComparisonOperator : PredicateOperator
  {
    private InternalFieldType? m_fieldType;

    [XmlAttribute("field")]
    public string Field { get; set; }

    [XmlIgnore]
    internal int FieldId { get; set; }

    [XmlElement("string", typeof (string))]
    [XmlElement("int", typeof (int))]
    [XmlElement("double", typeof (double))]
    [XmlElement("guid", typeof (Guid))]
    [XmlElement("date-time", typeof (DateTime))]
    [XmlElement("bool", typeof (bool))]
    [XmlElement("strings", typeof (string[]))]
    public object Value { get; set; }

    protected abstract string WiqlOperator { get; }

    protected abstract string InverseWiqlOperator { get; }

    protected InternalFieldType GetFieldType(IPredicateValidationHelper helper)
    {
      if (!this.m_fieldType.HasValue)
        this.m_fieldType = new InternalFieldType?(helper.GetFieldType(this.Field));
      return this.m_fieldType.Value;
    }

    public override void Validate(IPredicateValidationHelper validator)
    {
      if (string.IsNullOrEmpty(this.Field))
        throw new InvalidPredicateException("PredicateErrorMissingFieldName");
      if (validator.GetFieldId(this.Field) == 0)
        throw new InvalidPredicateException("Field does not exist.");
      this.ValidateFieldType(validator);
    }

    protected virtual void ValidateFieldType(IPredicateValidationHelper validator)
    {
      Type fieldSystemType = FieldHelpers.GetFieldSystemType(this.GetFieldType(validator));
      Type c = this.Value != null ? this.Value.GetType() : (!(fieldSystemType == typeof (string)) ? (Type) null : typeof (string));
      if (c == (Type) null || !fieldSystemType.IsAssignableFrom(c))
        throw new InvalidPredicateException("PredicateErrorInvalidValueType");
    }

    protected int Compare(IPredicateEvaluationHelper helper)
    {
      switch (this.GetFieldType((IPredicateValidationHelper) helper))
      {
        case InternalFieldType.String:
        case InternalFieldType.PlainText:
        case InternalFieldType.Html:
        case InternalFieldType.TreePath:
        case InternalFieldType.History:
          return StringComparer.OrdinalIgnoreCase.Compare(helper.GetFieldValue(this.FieldId), this.Value);
        case InternalFieldType.Integer:
          return ((int) helper.GetFieldValue(this.FieldId)).CompareTo((int) this.Value);
        case InternalFieldType.DateTime:
          return ((DateTime) helper.GetFieldValue(this.FieldId)).CompareTo((DateTime) this.Value);
        case InternalFieldType.Double:
          return ((double) helper.GetFieldValue(this.FieldId)).CompareTo((double) this.Value);
        case InternalFieldType.Guid:
          return ((Guid) helper.GetFieldValue(this.FieldId)).CompareTo((Guid) this.Value);
        case InternalFieldType.Boolean:
          return ((bool) helper.GetFieldValue(this.FieldId)).CompareTo((bool) this.Value);
        default:
          return 0;
      }
    }

    protected virtual string GetWiqlFieldReferenceName(IPredicateValidationHelper validator) => this.Field;

    protected virtual object GetWiqlFieldValue(IPredicateValidationHelper validator) => this.Value;

    private string GetFalsyExpression(bool inverse) => inverse ? "[System.Id] = [System.Id]" : "[System.Id] <> [System.Id]";

    protected internal override string ToWIQLPredicate(
      IPredicateValidationHelper validator,
      bool inverse)
    {
      string fieldReferenceName = this.GetWiqlFieldReferenceName(validator);
      object wiqlFieldValue = this.GetWiqlFieldValue(validator);
      bool flag1 = TFStringComparer.WorkItemFieldReferenceName.Equals(fieldReferenceName, "System.AreaPath");
      bool flag2 = TFStringComparer.WorkItemFieldReferenceName.Equals(fieldReferenceName, "System.IterationPath");
      if (flag1 | flag2)
      {
        string path = wiqlFieldValue as string;
        if (string.IsNullOrEmpty(path) || validator.GetTreeId(path, flag1 ? TreeStructureType.Area : TreeStructureType.Iteration) < 0)
          return this.GetFalsyExpression(inverse);
      }
      string str1 = inverse ? this.InverseWiqlOperator : this.WiqlOperator;
      StringBuilder stringBuilder = new StringBuilder(64);
      stringBuilder.Append('[');
      stringBuilder.Append(fieldReferenceName);
      stringBuilder.Append("] ");
      stringBuilder.Append(str1).Append(' ');
      switch (wiqlFieldValue)
      {
        case null:
          stringBuilder.Append("''");
          break;
        case int num1:
          stringBuilder.Append(num1.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
          break;
        case double num2:
          stringBuilder.Append(num2.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
          break;
        case bool flag3:
          stringBuilder.Append(flag3.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
          break;
        case Guid guid:
          stringBuilder.Append(PredicateFieldComparisonOperator.QuoteStringValue(guid.ToString()));
          break;
        case DateTime dateTime:
          stringBuilder.Append(PredicateFieldComparisonOperator.QuoteStringValue(dateTime.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture)));
          break;
        case string[] _:
          stringBuilder.Append('(');
          stringBuilder.Append(string.Join(",", ((IEnumerable<string>) (string[]) wiqlFieldValue).Select<string, string>((Func<string, string>) (s => PredicateFieldComparisonOperator.QuoteStringValue(s))).ToArray<string>()));
          stringBuilder.Append(')');
          break;
        default:
          string str2 = Convert.ToString(wiqlFieldValue);
          if (this.GetFieldType(validator) == InternalFieldType.TreePath)
            str2 = str2.Trim('\\', '/');
          stringBuilder.Append(PredicateFieldComparisonOperator.QuoteStringValue(str2));
          break;
      }
      return stringBuilder.ToString();
    }

    public override IEnumerable<int> GetReferencedFields() => (IEnumerable<int>) new int[1]
    {
      this.FieldId
    };

    public override void FixFieldReferences(IPredicateValidationHelper validationHelper) => this.FieldId = validationHelper.GetFieldId(this.Field);

    private static string QuoteStringValue(string value) => string.IsNullOrEmpty(value) ? "''" : "'" + value.Replace("'", "''") + "'";
  }
}
