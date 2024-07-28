// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskInputVisibilityRule
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class TaskInputVisibilityRule
  {
    private const string _operatorAnd = "&&";
    private const string _operatorOr = "||";

    public static VisibilityRule GetVisibilityRule(string visibleRule)
    {
      VisibilityRule visibilityRule = (VisibilityRule) null;
      if (!string.IsNullOrEmpty(visibleRule))
      {
        if (visibleRule.IndexOf("&&") != -1)
        {
          IEnumerable<PredicateRule> source = ((IEnumerable<string>) visibleRule.Split(new string[1]
          {
            "&&"
          }, StringSplitOptions.None)).Select<string, PredicateRule>((Func<string, PredicateRule>) (r => TaskInputVisibilityRule.GetPredicateRule(r)));
          visibilityRule = new VisibilityRule()
          {
            opr = "&&",
            predicateRules = source.ToArray<PredicateRule>()
          };
        }
        else if (visibleRule.IndexOf("||") != -1)
        {
          IEnumerable<PredicateRule> source = ((IEnumerable<string>) visibleRule.Split(new string[1]
          {
            "||"
          }, StringSplitOptions.None)).Select<string, PredicateRule>((Func<string, PredicateRule>) (r => TaskInputVisibilityRule.GetPredicateRule(r)));
          visibilityRule = new VisibilityRule()
          {
            opr = "||",
            predicateRules = source.ToArray<PredicateRule>()
          };
        }
        else
        {
          PredicateRule predicateRule = TaskInputVisibilityRule.GetPredicateRule(visibleRule);
          visibilityRule = new VisibilityRule()
          {
            opr = (string) null,
            predicateRules = new PredicateRule[1]
            {
              predicateRule
            }
          };
        }
      }
      return visibilityRule;
    }

    public static bool GetVisibility(
      VisibilityRule visibilityRule,
      IDictionary<string, string> taskInputs)
    {
      bool expr1 = string.IsNullOrEmpty(visibilityRule.opr) || visibilityRule.opr == "&&";
      if (visibilityRule.predicateRules == null)
        return expr1;
      for (int index = 0; index < visibilityRule.predicateRules.Length; ++index)
      {
        PredicateRule predicateRule = visibilityRule.predicateRules[index];
        if (taskInputs.ContainsKey(predicateRule.InputName))
        {
          string taskInput = taskInputs[predicateRule.InputName];
          bool predicateResult = TaskInputVisibilityRule.GetPredicateResult(predicateRule, taskInput);
          expr1 = TaskInputVisibilityRule.Evaluate(expr1, predicateResult, visibilityRule.opr);
        }
        else
          expr1 = false;
      }
      return expr1;
    }

    private static PredicateRule GetPredicateRule(string visibleRule)
    {
      PredicateRule predicateRule = (PredicateRule) null;
      string pattern = "([!=<>]+)";
      try
      {
        string[] strArray = Regex.Split(visibleRule, pattern, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(3000.0));
        if (strArray != null)
        {
          if (strArray.Length == 3)
            predicateRule = new PredicateRule()
            {
              InputName = strArray[0].Trim(),
              Condition = strArray[1].Trim(),
              ExpectedValue = strArray[2].Trim()
            };
        }
      }
      catch (RegexMatchTimeoutException ex)
      {
        throw new TaskDefinitionInvalidException(TaskResources.VisibilityRuleEvaluationFailure((object) visibleRule), (Exception) ex);
      }
      return predicateRule;
    }

    private static bool GetPredicateResult(PredicateRule rule, string valueToCheck)
    {
      if (rule == null)
        return false;
      string expectedValue = rule.ExpectedValue;
      switch (rule.Condition)
      {
        case "=":
        case "==":
          return string.Equals(valueToCheck, expectedValue, StringComparison.OrdinalIgnoreCase);
        case "!=":
          return !string.Equals(valueToCheck, expectedValue, StringComparison.OrdinalIgnoreCase);
        default:
          return TaskInputVisibilityRule.GetIntPredicateResult(valueToCheck, expectedValue, rule.Condition);
      }
    }

    private static bool GetIntPredicateResult(
      string valueToCheck,
      string expectedValue,
      string condition)
    {
      bool intPredicateResult = false;
      int result1 = -1;
      int result2 = -1;
      if (int.TryParse(valueToCheck, out result1) && int.TryParse(expectedValue, out result2))
      {
        switch (condition)
        {
          case "<":
            intPredicateResult = result1 < result2;
            break;
          case ">":
            intPredicateResult = result1 > result2;
            break;
          case "<=":
            intPredicateResult = result1 <= result2;
            break;
          case ">=":
            intPredicateResult = result1 >= result2;
            break;
        }
      }
      return intPredicateResult;
    }

    private static bool Evaluate(bool expr1, bool expr2, string opr)
    {
      bool flag = false;
      if (string.Equals(opr, "&&", StringComparison.OrdinalIgnoreCase))
        flag = expr1 & expr2;
      else if (string.Equals(opr, "||", StringComparison.OrdinalIgnoreCase))
        flag = expr1 | expr2;
      else if (string.IsNullOrEmpty(opr))
        flag = expr2;
      return flag;
    }
  }
}
