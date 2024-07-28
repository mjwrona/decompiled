// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class RuleActionModel : IEquatable<RuleActionModel>
  {
    [DataMember]
    public string ActionType { get; set; }

    [DataMember]
    public string TargetField { get; set; }

    [DataMember]
    public string Value { get; set; }

    [IgnoreDataMember]
    public Guid ForVsId { get; set; }

    [IgnoreDataMember]
    public Guid NotVsId { get; set; }

    public bool Equals(RuleActionModel other) => StringComparer.Ordinal.Equals(this.ActionType, other.ActionType) && StringComparer.Ordinal.Equals(this.TargetField, other.TargetField) && StringComparer.Ordinal.Equals(this.Value, other.Value) && this.ForVsId == other.ForVsId && this.NotVsId == other.NotVsId;

    public override int GetHashCode()
    {
      int hashCode1 = this.ActionType == null ? 0 : this.ActionType.GetHashCode();
      int hashCode2 = this.TargetField == null ? 0 : this.TargetField.GetHashCode();
      int hashCode3 = this.Value == null ? 0 : this.Value.GetHashCode();
      Guid guid = this.ForVsId;
      int hashCode4 = guid.GetHashCode();
      guid = this.NotVsId;
      int hashCode5 = guid.GetHashCode();
      int num = hashCode2;
      return hashCode1 ^ num ^ hashCode3 ^ hashCode4 ^ hashCode5;
    }

    public static bool TryCreate(
      string targetField,
      WorkItemRule rule,
      out IEnumerable<RuleActionModel> actions)
    {
      string actionType;
      IEnumerable<string> values;
      if (RuleActionModel.TryGetActionType(rule, out actionType, out values))
      {
        actions = (IEnumerable<RuleActionModel>) RuleActionModel.Create(targetField, actionType, values).ToList<RuleActionModel>();
        foreach (RuleActionModel ruleActionModel in actions)
        {
          ruleActionModel.ForVsId = rule.ForVsId;
          ruleActionModel.NotVsId = rule.NotVsId;
        }
        return true;
      }
      actions = (IEnumerable<RuleActionModel>) null;
      return false;
    }

    public static IEnumerable<RuleActionModel> Create(
      string targetField,
      string actionType,
      IEnumerable<string> values)
    {
      foreach (string str in values)
        yield return new RuleActionModel()
        {
          ActionType = actionType,
          Value = str,
          TargetField = targetField
        };
    }

    private static bool TryGetActionType(
      WorkItemRule rule,
      out string actionType,
      out IEnumerable<string> values)
    {
      values = Enumerable.Repeat<string>((string) null, 1);
      actionType = (string) null;
      switch (rule)
      {
        case RequiredRule _:
          actionType = "$makeRequired";
          return true;
        case ReadOnlyRule _:
          actionType = "$makeReadOnly";
          return true;
        case HideFieldRule _:
          actionType = "$hideTargetField";
          return true;
        case DefaultRule _:
          if (rule is IdentityDefaultRule)
            return false;
          DefaultRule defaultRule = rule as DefaultRule;
          if (defaultRule.ValueFrom == RuleValueFrom.Value)
          {
            actionType = "$setDefaultValue";
            values = Enumerable.Repeat<string>(defaultRule.Value, 1);
            return true;
          }
          if (defaultRule.ValueFrom == RuleValueFrom.Clock)
          {
            actionType = "$setDefaultFromClock";
            return true;
          }
          if (defaultRule.ValueFrom == RuleValueFrom.CurrentUser)
          {
            actionType = "$setDefaultFromCurrentUser";
            return true;
          }
          if (defaultRule.ValueFrom == RuleValueFrom.OtherFieldCurrentValue)
          {
            actionType = "$setDefaultFromField";
            values = Enumerable.Repeat<string>(defaultRule.Value, 1);
            return true;
          }
          break;
        case CopyRule _:
          CopyRule copyRule = rule as CopyRule;
          if (copyRule.ValueFrom == RuleValueFrom.Value)
          {
            actionType = "$copyValue";
            values = Enumerable.Repeat<string>(copyRule.Value, 1);
            return true;
          }
          if (copyRule.ValueFrom == RuleValueFrom.Clock)
          {
            actionType = "$copyFromClock";
            return true;
          }
          if (copyRule.ValueFrom == RuleValueFrom.CurrentUser)
          {
            actionType = "$copyFromCurrentUser";
            return true;
          }
          if (copyRule.ValueFrom == RuleValueFrom.OtherFieldCurrentValue)
          {
            actionType = "$copyFromField";
            values = Enumerable.Repeat<string>(copyRule.Value, 1);
            return true;
          }
          break;
        case EmptyRule _:
          actionType = "$setValueToEmpty";
          return true;
        case ServerDefaultRule _:
          ServerDefaultRule serverDefaultRule = rule as ServerDefaultRule;
          if (serverDefaultRule.From == ServerDefaultType.ServerDateTime)
          {
            actionType = "$copyFromServerClock";
            return true;
          }
          if (serverDefaultRule.From == ServerDefaultType.CallerIdentity)
          {
            actionType = "$copyFromServerCurrentUser";
            return true;
          }
          break;
        case ProhibitedValuesRule _:
          actionType = "$disallowValue";
          ProhibitedValuesRule prohibitedValuesRule = (ProhibitedValuesRule) rule;
          values = (IEnumerable<string>) prohibitedValuesRule.Values;
          return true;
        case WhenChangedRule _:
          WhenChangedRule whenChangedRule = (WhenChangedRule) rule;
          if (whenChangedRule.SubRules.Length == 1 && whenChangedRule.SubRules[0] is ProhibitedValuesRule)
          {
            actionType = "$disallowValue";
            ProhibitedValuesRule subRule = (ProhibitedValuesRule) whenChangedRule.SubRules[0];
            values = (IEnumerable<string>) subRule.Values;
            return true;
          }
          break;
      }
      return false;
    }

    public WorkItemRule ToWorkItemRule(
      IVssRequestContext requestContext,
      Guid ruleId,
      string friendlyName,
      bool isDisabled,
      bool isIdentity)
    {
      string actionType = this.ActionType;
      if (actionType != null)
      {
        WorkItemRule rule;
        switch (actionType.Length)
        {
          case 10:
            if (actionType == "$copyValue")
            {
              if (isIdentity && !string.IsNullOrEmpty(this.Value))
              {
                Guid result = Guid.Empty;
                Guid.TryParse(this.Value, out result);
                IdentityCopyRule identityCopyRule = new IdentityCopyRule();
                identityCopyRule.Value = this.Value;
                identityCopyRule.ValueFrom = RuleValueFrom.Value;
                identityCopyRule.Vsid = result;
                rule = (WorkItemRule) identityCopyRule;
                break;
              }
              CopyRule copyRule = new CopyRule();
              copyRule.Value = this.Value;
              copyRule.ValueFrom = RuleValueFrom.Value;
              rule = (WorkItemRule) copyRule;
              break;
            }
            goto label_38;
          case 13:
            switch (actionType[7])
            {
              case 'a':
                if (actionType == "$makeReadOnly")
                {
                  rule = (WorkItemRule) new ReadOnlyRule();
                  this.SetForNotVsIds(requestContext, rule);
                  break;
                }
                goto label_38;
              case 'q':
                if (actionType == "$makeRequired")
                {
                  rule = (WorkItemRule) new RequiredRule();
                  this.SetForNotVsIds(requestContext, rule);
                  break;
                }
                goto label_38;
              default:
                goto label_38;
            }
            break;
          case 14:
            switch (actionType[9])
            {
              case 'C':
                if (actionType == "$copyFromClock")
                {
                  CopyRule copyRule = new CopyRule();
                  copyRule.ValueFrom = RuleValueFrom.Clock;
                  rule = (WorkItemRule) copyRule;
                  break;
                }
                goto label_38;
              case 'F':
                if (actionType == "$copyFromField")
                {
                  CopyRule copyRule = new CopyRule();
                  copyRule.Value = this.Value;
                  copyRule.ValueFrom = RuleValueFrom.OtherFieldCurrentValue;
                  rule = (WorkItemRule) copyRule;
                  break;
                }
                goto label_38;
              case 'V':
                if (actionType == "$disallowValue")
                {
                  ProhibitedValuesRule prohibitedValuesRule = new ProhibitedValuesRule();
                  prohibitedValuesRule.Values = new HashSet<string>()
                  {
                    this.Value
                  };
                  rule = (WorkItemRule) prohibitedValuesRule;
                  this.SetForNotVsIds(requestContext, rule);
                  break;
                }
                goto label_38;
              default:
                goto label_38;
            }
            break;
          case 16:
            switch (actionType[4])
            {
              case 'D':
                if (actionType == "$setDefaultValue")
                {
                  DefaultRule defaultRule = new DefaultRule();
                  defaultRule.Value = this.Value;
                  defaultRule.ValueFrom = RuleValueFrom.Value;
                  rule = (WorkItemRule) defaultRule;
                  break;
                }
                goto label_38;
              case 'V':
                if (actionType == "$setValueToEmpty")
                {
                  rule = (WorkItemRule) new EmptyRule();
                  break;
                }
                goto label_38;
              case 'e':
                if (actionType == "$hideTargetField")
                {
                  rule = (WorkItemRule) new HideFieldRule();
                  this.SetForNotVsIds(requestContext, rule);
                  break;
                }
                goto label_38;
              default:
                goto label_38;
            }
            break;
          case 20:
            switch (actionType[9])
            {
              case 'C':
                if (actionType == "$copyFromCurrentUser")
                {
                  CopyRule copyRule = new CopyRule();
                  copyRule.ValueFrom = RuleValueFrom.CurrentUser;
                  rule = (WorkItemRule) copyRule;
                  break;
                }
                goto label_38;
              case 'S':
                if (actionType == "$copyFromServerClock")
                {
                  rule = (WorkItemRule) new ServerDefaultRule()
                  {
                    From = ServerDefaultType.ServerDateTime
                  };
                  break;
                }
                goto label_38;
              case 'l':
                switch (actionType)
                {
                  case "$setDefaultFromClock":
                    DefaultRule defaultRule1 = new DefaultRule();
                    defaultRule1.ValueFrom = RuleValueFrom.Clock;
                    rule = (WorkItemRule) defaultRule1;
                    break;
                  case "$setDefaultFromField":
                    DefaultRule defaultRule2 = new DefaultRule();
                    defaultRule2.Value = this.Value;
                    defaultRule2.ValueFrom = RuleValueFrom.OtherFieldCurrentValue;
                    rule = (WorkItemRule) defaultRule2;
                    break;
                  default:
                    goto label_38;
                }
                break;
              default:
                goto label_38;
            }
            break;
          case 26:
            switch (actionType[1])
            {
              case 'c':
                if (actionType == "$copyFromServerCurrentUser")
                {
                  rule = (WorkItemRule) new ServerDefaultRule()
                  {
                    From = ServerDefaultType.CallerIdentity
                  };
                  break;
                }
                goto label_38;
              case 's':
                if (actionType == "$setDefaultFromCurrentUser")
                {
                  DefaultRule defaultRule3 = new DefaultRule();
                  defaultRule3.ValueFrom = RuleValueFrom.CurrentUser;
                  rule = (WorkItemRule) defaultRule3;
                  break;
                }
                goto label_38;
              default:
                goto label_38;
            }
            break;
          default:
            goto label_38;
        }
        rule.Id = ruleId;
        rule.FriendlyName = friendlyName;
        rule.IsDisabled = isDisabled;
        return rule;
      }
label_38:
      throw new ArgumentException(ResourceStrings.WorkItemRuleActionInvalid((object) this.ActionType));
    }

    private void SetForNotVsIds(IVssRequestContext requestContext, WorkItemRule rule)
    {
      if (this.ForVsId != Guid.Empty)
        rule.ForVsId = this.ForVsId;
      if (!(this.NotVsId != Guid.Empty))
        return;
      rule.NotVsId = this.NotVsId;
    }
  }
}
