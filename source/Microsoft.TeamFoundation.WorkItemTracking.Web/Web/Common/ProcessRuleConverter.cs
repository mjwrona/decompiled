// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ProcessRuleConverter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public static class ProcessRuleConverter
  {
    public const string When = "$when";
    public const string WhenWas = "$whenWas";
    public const string WhenNot = "$whenNot";
    public const string WhenChanged = "$whenChanged";
    public const string WhenNotChanged = "$whenNotChanged";
    public const string WhenStateChangedTo = "$whenStateChangedTo";
    public const string WhenStateChangedFromAndTo = "$whenStateChangedFromAndTo";
    public const string WhenWorkItemIsCreated = "$whenWorkItemIsCreated";
    public const string WhenValueIsDefined = "$whenValueIsDefined";
    public const string WhenValueIsNotDefined = "$whenValueIsNotDefined";
    public const string WhenCurrentUserIsMemberOfGroup = "$whenCurrentUserIsMemberOfGroup";
    public const string WhenCurrentUserIsNotMemberOfGroup = "$whenCurrentUserIsNotMemberOfGroup";
    public const string MakeRequired = "$makeRequired";
    public const string MakeReadOnly = "$makeReadOnly";
    public const string SetDefaultValue = "$setDefaultValue";
    public const string SetDefaultFromClock = "$setDefaultFromClock";
    public const string SetDefaultFromCurrentUser = "$setDefaultFromCurrentUser";
    public const string SetDefaultFromField = "$setDefaultFromField";
    public const string CopyValue = "$copyValue";
    public const string CopyFromClock = "$copyFromClock";
    public const string CopyFromCurrentUser = "$copyFromCurrentUser";
    public const string CopyFromField = "$copyFromField";
    public const string SetValueToEmpty = "$setValueToEmpty";
    public const string CopyFromServerClock = "$copyFromServerClock";
    public const string CopyFromServerCurrentUser = "$copyFromServerCurrentUser";
    public const string HideTargetField = "$hideTargetField";
    public const string DisallowValue = "$disallowValue";

    private static void CheckGuidValidity(string vsId)
    {
      if (!Guid.TryParse(vsId, out Guid _))
        throw new FieldRuleModelValidationException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.UnrecognizedPropertyValue((object) nameof (vsId), (object) vsId));
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel CreateFieldRuleModel(
      CreateProcessRuleRequest processRule)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRuleModel = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel();
      fieldRuleModel.FriendlyName = processRule.Name;
      List<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel> ruleConditionModelList = new List<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>();
      string str1 = "";
      string str2 = "";
      bool flag1 = false;
      bool flag2 = false;
      if (processRule.Conditions != null)
      {
        foreach (RuleCondition condition in processRule.Conditions)
        {
          if (condition.ConditionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsMemberOfGroup)
          {
            flag1 = true;
            str1 = condition.Value;
            ProcessRuleConverter.CheckGuidValidity(str1);
          }
          else if (condition.ConditionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsNotMemberOfGroup)
          {
            flag2 = true;
            str2 = condition.Value;
            ProcessRuleConverter.CheckGuidValidity(str2);
          }
          else
            ruleConditionModelList.Add(condition.ToRuleConditionModel());
        }
      }
      fieldRuleModel.Conditions = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>) ruleConditionModelList;
      List<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel> ruleActionModelList = new List<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel>();
      if (processRule.Actions != null)
      {
        foreach (RuleAction action in processRule.Actions)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel ruleActionModel = action.ToRuleActionModel();
          if (action.ActionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.MakeRequired || action.ActionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.MakeReadOnly || action.ActionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.HideTargetField || action.ActionType == Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.DisallowValue)
          {
            if (flag1)
              ruleActionModel.ForVsId = new Guid(str1);
            else if (flag2)
              ruleActionModel.NotVsId = new Guid(str2);
          }
          ruleActionModelList.Add(ruleActionModel);
        }
      }
      fieldRuleModel.Actions = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel>) ruleActionModelList;
      fieldRuleModel.IsDisabled = processRule.IsDisabled;
      return fieldRuleModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel ToFieldRuleModel(
      this CreateProcessRuleRequest processRule)
    {
      return ProcessRuleConverter.CreateFieldRuleModel(processRule);
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel ToFieldRuleModel(
      this UpdateProcessRuleRequest processRule)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRuleModel = ProcessRuleConverter.CreateFieldRuleModel((CreateProcessRuleRequest) processRule);
      fieldRuleModel.Id = new Guid?(processRule.Id);
      return fieldRuleModel;
    }

    public static ProcessRule ToProcessRule(
      this Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel fieldRule,
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName)
    {
      ProcessRule processRule = new ProcessRule();
      processRule.Name = fieldRule.FriendlyName;
      processRule.Id = fieldRule.Id.Value;
      List<RuleAction> ruleActionList = new List<RuleAction>();
      Guid guid1 = Guid.Empty;
      Guid guid2 = Guid.Empty;
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel action in fieldRule.Actions)
      {
        if (action.ForVsId != Guid.Empty)
          guid1 = action.ForVsId;
        else if (action.NotVsId != Guid.Empty)
          guid2 = action.NotVsId;
        ruleActionList.Add(action.ToRuleAction());
      }
      List<RuleCondition> ruleConditionList = new List<RuleCondition>();
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel condition in fieldRule.Conditions)
        ruleConditionList.Add(ProcessRuleConverter.ToRuleCondition(condition));
      if (guid1 != Guid.Empty || guid2 != Guid.Empty)
        ruleConditionList.Add(new RuleCondition()
        {
          ConditionType = guid1 != Guid.Empty ? Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsMemberOfGroup : Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsNotMemberOfGroup,
          Value = guid1 != Guid.Empty ? guid1.ToString() : guid2.ToString()
        });
      processRule.Conditions = (IEnumerable<RuleCondition>) ruleConditionList;
      processRule.Actions = (IEnumerable<RuleAction>) ruleActionList;
      processRule.IsDisabled = fieldRule.IsDisabled;
      processRule.CustomizationType = fieldRule.IsSystem ? CustomizationType.System : CustomizationType.Custom;
      processRule.Url = ProcessRuleModelFactory.GetLocationUrlForWorkItemTypeRule(requestContext, processId, witRefName, processRule.Id);
      return processRule;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ToRuleConditionModel(
      this RuleCondition ruleCondition)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel()
      {
        ConditionType = ProcessRuleConverter.ConvertRuleConditionToStringValue(ruleCondition.ConditionType),
        Field = ruleCondition.Field,
        Value = ruleCondition.Value
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel ToRuleActionModel(
      this RuleAction ruleAction)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel()
      {
        ActionType = ProcessRuleConverter.ConvertRuleActionToStringValue(ruleAction.ActionType),
        TargetField = ruleAction.TargetField,
        Value = ruleAction.Value
      };
    }

    public static RuleCondition ToRuleCondition(this Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ruleConditionModel) => new RuleCondition()
    {
      ConditionType = ProcessRuleConverter.ConvertRuleConditionToEnumValue(ruleConditionModel.ConditionType),
      Field = ruleConditionModel.Field,
      Value = ruleConditionModel.Value
    };

    public static RuleAction ToRuleAction(this Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel ruleActionModel) => new RuleAction()
    {
      ActionType = ProcessRuleConverter.ConvertRuleActionToEnumValue(ruleActionModel.ActionType),
      TargetField = ruleActionModel.TargetField,
      Value = ruleActionModel.Value
    };

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType ConvertRuleConditionToEnumValue(
      string ruleConditionType)
    {
      Dictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType> dictionary = new Dictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType>()
      {
        {
          "$when",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.When
        },
        {
          "$whenChanged",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenChanged
        },
        {
          "$whenNot",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenNot
        },
        {
          "$whenNotChanged",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenNotChanged
        },
        {
          "$whenWas",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenWas
        },
        {
          "$whenStateChangedTo",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenStateChangedTo
        },
        {
          "$whenStateChangedFromAndTo",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenStateChangedFromAndTo
        },
        {
          "$whenValueIsDefined",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenValueIsDefined
        },
        {
          "$whenValueIsNotDefined",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenValueIsNotDefined
        },
        {
          "$whenCurrentUserIsMemberOfGroup",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsMemberOfGroup
        },
        {
          "$whenCurrentUserIsNotMemberOfGroup",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsMemberOfGroup
        }
      };
      return !dictionary.ContainsKey(ruleConditionType) ? (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType) 0 : dictionary[ruleConditionType];
    }

    public static string ConvertRuleConditionToStringValue(Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType ruleConditionType)
    {
      Dictionary<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType, string> dictionary = new Dictionary<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType, string>()
      {
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.When,
          "$when"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenNot,
          "$whenNot"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenChanged,
          "$whenChanged"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenNotChanged,
          "$whenNotChanged"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenWas,
          "$whenWas"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenStateChangedTo,
          "$whenStateChangedTo"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenStateChangedFromAndTo,
          "$whenStateChangedFromAndTo"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenWorkItemIsCreated,
          "$whenWorkItemIsCreated"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenValueIsDefined,
          "$whenValueIsDefined"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenValueIsNotDefined,
          "$whenValueIsNotDefined"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsMemberOfGroup,
          "$whenCurrentUserIsMemberOfGroup"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleConditionType.WhenCurrentUserIsNotMemberOfGroup,
          "$whenCurrentUserIsNotMemberOfGroup"
        }
      };
      return !dictionary.ContainsKey(ruleConditionType) ? "" : dictionary[ruleConditionType];
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType ConvertRuleActionToEnumValue(
      string ruleActionType)
    {
      IDictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType> dictionary = (IDictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType>) new Dictionary<string, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType>()
      {
        {
          "$makeRequired",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.MakeRequired
        },
        {
          "$makeReadOnly",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.MakeReadOnly
        },
        {
          "$setDefaultValue",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultValue
        },
        {
          "$setDefaultFromClock",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultFromClock
        },
        {
          "$setDefaultFromField",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultFromField
        },
        {
          "$setDefaultFromCurrentUser",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultFromCurrentUser
        },
        {
          "$copyValue",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyValue
        },
        {
          "$copyFromClock",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromClock
        },
        {
          "$copyFromCurrentUser",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromCurrentUser
        },
        {
          "$copyFromField",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromField
        },
        {
          "$setValueToEmpty",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetValueToEmpty
        },
        {
          "$copyFromServerClock",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromServerClock
        },
        {
          "$copyFromServerCurrentUser",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromServerCurrentUser
        },
        {
          "$hideTargetField",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.HideTargetField
        },
        {
          "$disallowValue",
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.DisallowValue
        }
      };
      return !dictionary.ContainsKey(ruleActionType) ? (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType) 0 : dictionary[ruleActionType];
    }

    public static string ConvertRuleActionToStringValue(Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType ruleActionType)
    {
      IDictionary<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType, string> dictionary = (IDictionary<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType, string>) new Dictionary<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType, string>()
      {
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.MakeRequired,
          "$makeRequired"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.MakeReadOnly,
          "$makeReadOnly"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultValue,
          "$setDefaultValue"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultFromClock,
          "$setDefaultFromClock"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetDefaultFromField,
          "$setDefaultFromField"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyValue,
          "$copyValue"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromClock,
          "$copyFromClock"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromCurrentUser,
          "$copyFromCurrentUser"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromField,
          "$copyFromField"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.SetValueToEmpty,
          "$setValueToEmpty"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromServerClock,
          "$copyFromServerClock"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.CopyFromServerCurrentUser,
          "$copyFromServerCurrentUser"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.HideTargetField,
          "$hideTargetField"
        },
        {
          Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.RuleActionType.DisallowValue,
          "$disallowValue"
        }
      };
      return !dictionary.ContainsKey(ruleActionType) ? "" : dictionary[ruleActionType];
    }

    internal static Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel DecodeRuleConditionType(
      Microsoft.TeamFoundation.WorkItemTracking.Web.Models.FieldRuleModel rule)
    {
      int index1 = -1;
      int index2 = -1;
      int index3 = -1;
      int index4 = -1;
      int num = -1;
      if (rule.Conditions != null)
      {
        List<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel> list = rule.Conditions.ToList<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>();
        int count = list.Count;
        for (int index5 = 0; index5 < count; ++index5)
        {
          switch (list.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>(index5).ConditionType)
          {
            case "$whenStateChangedTo":
              index1 = index5;
              break;
            case "$whenStateChangedFromAndTo":
              index2 = index5;
              break;
            case "$whenWorkItemIsCreated":
              index3 = index5;
              break;
            case "$whenValueIsDefined":
              index4 = index5;
              break;
            case "$whenValueIsNotDefined":
              num = index5;
              break;
          }
        }
        if (index4 != -1)
          list.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>(index4).ConditionType = "$whenNot";
        if (num != -1)
          list.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>(index4).ConditionType = "$when";
        if (index2 != -1)
        {
          string[] strArray = list.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>(index2).Value.Split('.');
          Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ruleConditionModel1 = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel()
          {
            ConditionType = "$whenWas",
            Field = "System.State",
            Value = strArray[0]
          };
          Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ruleConditionModel2 = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel()
          {
            ConditionType = "$when",
            Field = "System.State",
            Value = strArray[1]
          };
          list.Add(ruleConditionModel2);
          list.Add(ruleConditionModel1);
        }
        else if (index1 != -1)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ruleConditionModel3 = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel()
          {
            ConditionType = "$whenChanged",
            Field = "System.State",
            Value = (string) null
          };
          Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ruleConditionModel4 = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel()
          {
            ConditionType = "$when",
            Field = "System.State",
            Value = rule.Conditions.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>(index1).Value
          };
          list.Add(ruleConditionModel4);
          list.Add(ruleConditionModel3);
        }
        else if (index3 != -1)
        {
          string[] strArray = rule.Conditions.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>(index3).Value.Split('.');
          Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ruleConditionModel5 = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel()
          {
            ConditionType = "$whenWas",
            Field = "System.State",
            Value = strArray[0]
          };
          Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel ruleConditionModel6 = new Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel()
          {
            ConditionType = "$when",
            Field = "System.State",
            Value = strArray[1]
          };
          list.Add(ruleConditionModel6);
          list.Add(ruleConditionModel5);
        }
        if (index2 != -1)
          list.RemoveAt(index2);
        if (index1 != -1)
          list.RemoveAt(index1);
        if (index3 != -1)
          list.RemoveAt(index3);
        rule.Conditions = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleConditionModel>) list;
      }
      for (int index6 = 0; index6 < rule.Actions.Count<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel>(); ++index6)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel ruleActionModel = rule.Actions.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.Web.Models.RuleActionModel>(index6);
        if (ruleActionModel.ActionType == "$setValueToEmpty")
        {
          ruleActionModel.ActionType = "$copyValue";
          ruleActionModel.Value = "";
        }
      }
      return rule;
    }
  }
}
