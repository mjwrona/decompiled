// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class RuleConditionModel : IEquatable<RuleConditionModel>, IComparable<RuleConditionModel>
  {
    [DataMember]
    public string ConditionType { get; set; }

    [DataMember]
    public string Field { get; set; }

    [DataMember]
    public string Value { get; set; }

    public bool Equals(RuleConditionModel other) => StringComparer.Ordinal.Equals(this.ConditionType, other.ConditionType) && StringComparer.Ordinal.Equals(this.Field, other.Field) && StringComparer.Ordinal.Equals(this.Value, other.Value);

    public override int GetHashCode()
    {
      int hashCode1 = this.ConditionType == null ? 0 : this.ConditionType.GetHashCode();
      int hashCode2 = this.Field == null ? 0 : this.Field.GetHashCode();
      int hashCode3 = this.Value == null ? 0 : this.Value.GetHashCode();
      int num = hashCode2;
      return hashCode1 ^ num ^ hashCode3;
    }

    public int CompareTo(RuleConditionModel other)
    {
      int num1 = RuleConditionType.SortOrder[this.ConditionType];
      int num2 = RuleConditionType.SortOrder[other.ConditionType];
      if (num1 != num2)
        return num1 - num2;
      if (TFStringComparer.WorkItemFieldReferenceName.Equals(this.Field, "System.State"))
        return -1;
      return TFStringComparer.WorkItemFieldReferenceName.Equals(other.Field, "System.State") ? 1 : TFStringComparer.WorkItemFieldReferenceName.Compare(this.Field, other.Field);
    }

    public static bool TryCreate(
      ConditionalBlockRule conditionBlockRule,
      out RuleConditionModel condition)
    {
      string conditionType;
      string str;
      if (RuleConditionModel.TryGetConditionType(conditionBlockRule, out conditionType, out str))
      {
        condition = new RuleConditionModel()
        {
          ConditionType = conditionType,
          Field = conditionBlockRule.Field,
          Value = str
        };
        return true;
      }
      condition = (RuleConditionModel) null;
      return false;
    }

    public ConditionalBlockRule ToRuleCondition()
    {
      ConditionalBlockRule ruleCondition;
      switch (this.ConditionType)
      {
        case "$when":
          WhenRule whenRule1 = new WhenRule();
          whenRule1.Value = this.Value;
          ruleCondition = (ConditionalBlockRule) whenRule1;
          break;
        case "$whenNot":
          WhenRule whenRule2 = new WhenRule();
          whenRule2.Inverse = true;
          whenRule2.Value = this.Value;
          ruleCondition = (ConditionalBlockRule) whenRule2;
          break;
        case "$whenChanged":
          ruleCondition = (ConditionalBlockRule) new WhenChangedRule();
          break;
        case "$whenNotChanged":
          WhenChangedRule whenChangedRule = new WhenChangedRule();
          whenChangedRule.Inverse = true;
          ruleCondition = (ConditionalBlockRule) whenChangedRule;
          break;
        case "$whenWas":
          WhenWasRule whenWasRule = new WhenWasRule();
          whenWasRule.Value = this.Value;
          ruleCondition = (ConditionalBlockRule) whenWasRule;
          break;
        default:
          throw new ArgumentException(ResourceStrings.WorkItemRuleConditionInvalid((object) this.ConditionType));
      }
      ruleCondition.Field = this.Field;
      return ruleCondition;
    }

    private static bool TryGetConditionType(
      ConditionalBlockRule conditionRule,
      out string conditionType,
      out string value)
    {
      conditionType = (string) null;
      value = (string) null;
      switch (conditionRule)
      {
        case WhenRule _:
          conditionType = !conditionRule.Inverse ? "$when" : "$whenNot";
          value = conditionRule.Value;
          return true;
        case WhenChangedRule _:
          if (((IEnumerable<WorkItemRule>) conditionRule.SubRules).Any<WorkItemRule>((Func<WorkItemRule, bool>) (r => r is ProhibitedValuesRule)))
            return false;
          conditionType = !conditionRule.Inverse ? "$whenChanged" : "$whenNotChanged";
          return true;
        case WhenWasRule _:
          if (!conditionRule.Inverse)
            conditionType = "$whenWas";
          value = conditionRule.Value;
          return true;
        default:
          return false;
      }
    }
  }
}
