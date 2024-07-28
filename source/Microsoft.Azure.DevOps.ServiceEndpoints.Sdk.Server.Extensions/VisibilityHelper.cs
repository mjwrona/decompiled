// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.VisibilityHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F602B8A6-9B4B-4971-8764-E3FEAFAB8CD5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions
{
  public class VisibilityHelper
  {
    public static bool IsInputVisible(InputDescriptor descriptor, IDictionary<string, string> data)
    {
      bool flag = true;
      if (descriptor.Properties != null && descriptor.Properties.Count > 0)
      {
        string visibleRule;
        descriptor.Properties.TryGetValue<string>("visibleRule", out visibleRule);
        if (!string.IsNullOrWhiteSpace(visibleRule))
          flag = VisibilityHelper.getVisibility(VisibilityHelper.getVisibilityRule(visibleRule), data);
      }
      return flag;
    }

    private static VisibilityRule getVisibilityRule(string visibleRule)
    {
      VisibilityRule visibilityRule = (VisibilityRule) null;
      if (visibleRule != null)
      {
        if (visibleRule.IndexOf("&&") == -1 && visibleRule.IndexOf("||") == -1)
          return new VisibilityRule(new List<PredicateRule>()
          {
            VisibilityHelper.getPredicateRule(visibleRule)
          }, (string) null);
        string ruleOperator = (string) null;
        if (visibleRule.IndexOf("&&") != -1)
          ruleOperator = "&&";
        else if (visibleRule.IndexOf("||") != -1)
          ruleOperator = "||";
        if (ruleOperator != null)
        {
          string[] collection = visibleRule.Split(ruleOperator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          List<PredicateRule> predicateRules = new List<PredicateRule>();
          Action<string> action = (Action<string>) (rule =>
          {
            PredicateRule predicateRule = VisibilityHelper.getPredicateRule(rule);
            if (predicateRule == null)
              return;
            predicateRules.Add(predicateRule);
          });
          ((IEnumerable<string>) collection).ForEach<string>(action);
          return new VisibilityRule(predicateRules, ruleOperator);
        }
      }
      return visibilityRule;
    }

    private static bool getVisibility(
      VisibilityRule visibilityRule,
      IDictionary<string, string> data)
    {
      if (visibilityRule == null)
        return true;
      bool result = string.Equals(visibilityRule.ruleOperator, "&&");
      visibilityRule.predicateRules.ForEach((Action<PredicateRule>) (predicateRule =>
      {
        if (predicateRule == null)
          return;
        string valueToCheck;
        data.TryGetValue(predicateRule.inputName, out valueToCheck);
        result = VisibilityHelper.evaluate(result, VisibilityHelper.getPredicateResult(predicateRule, valueToCheck), visibilityRule.ruleOperator);
      }));
      return result;
    }

    private static bool evaluate(bool expr1, bool expr2, string ruleOperator)
    {
      if (string.Equals(ruleOperator, "&&"))
        return expr1 & expr2;
      if (string.Equals(ruleOperator, "||"))
        return expr1 | expr2;
      return !string.IsNullOrWhiteSpace(ruleOperator) || expr2;
    }

    private static bool getPredicateResult(PredicateRule rule, string valueToCheck)
    {
      bool predicateResult = false;
      string str1 = !string.IsNullOrWhiteSpace(valueToCheck) ? valueToCheck.ToLower() : valueToCheck;
      if (rule != null)
      {
        string str2 = !string.IsNullOrWhiteSpace(rule.expectedValue) ? rule.expectedValue.ToLower() : rule.expectedValue;
        string condition = rule.condition;
        if (condition != null)
        {
          switch (condition.Length)
          {
            case 1:
              switch (condition[0])
              {
                case '<':
                  predicateResult = string.Compare(str1, str2) < 0;
                  goto label_29;
                case '=':
                  break;
                case '>':
                  predicateResult = string.Compare(str1, str2) > 0;
                  goto label_29;
                default:
                  goto label_29;
              }
              break;
            case 2:
              switch (condition[0])
              {
                case '!':
                  if (condition == "!=")
                  {
                    predicateResult = !string.Equals(str1, str2);
                    goto label_29;
                  }
                  else
                    goto label_29;
                case '<':
                  if (condition == "<=")
                  {
                    predicateResult = string.Compare(str1, str2) <= 0;
                    goto label_29;
                  }
                  else
                    goto label_29;
                case '=':
                  if (condition == "==")
                    break;
                  goto label_29;
                case '>':
                  if (condition == ">=")
                  {
                    predicateResult = string.Compare(str1, str2) >= 0;
                    goto label_29;
                  }
                  else
                    goto label_29;
                default:
                  goto label_29;
              }
              break;
            case 8:
              switch (condition[0])
              {
                case 'C':
                  if (condition == "Contains")
                  {
                    predicateResult = !string.IsNullOrWhiteSpace(valueToCheck) && valueToCheck.IndexOf(str2) >= 0;
                    goto label_29;
                  }
                  else
                    goto label_29;
                case 'E':
                  if (condition == "EndsWith")
                  {
                    predicateResult = !string.IsNullOrWhiteSpace(valueToCheck) && valueToCheck.EndsWith(str2, true, CultureInfo.InvariantCulture);
                    goto label_29;
                  }
                  else
                    goto label_29;
                default:
                  goto label_29;
              }
            case 10:
              if (condition == "StartsWith")
              {
                predicateResult = !string.IsNullOrWhiteSpace(valueToCheck) && valueToCheck.StartsWith(str2, true, CultureInfo.InvariantCulture);
                goto label_29;
              }
              else
                goto label_29;
            case 11:
              switch (condition[3])
              {
                case 'C':
                  if (condition == "NotContains")
                  {
                    predicateResult = string.IsNullOrWhiteSpace(valueToCheck) || valueToCheck.IndexOf(str2) < 0;
                    goto label_29;
                  }
                  else
                    goto label_29;
                case 'E':
                  if (condition == "NotEndsWith")
                  {
                    predicateResult = string.IsNullOrWhiteSpace(valueToCheck) || !valueToCheck.EndsWith(str2, true, CultureInfo.InvariantCulture);
                    goto label_29;
                  }
                  else
                    goto label_29;
                default:
                  goto label_29;
              }
            case 13:
              if (condition == "NotStartsWith")
              {
                predicateResult = string.IsNullOrWhiteSpace(valueToCheck) || !valueToCheck.StartsWith(str2, true, CultureInfo.InvariantCulture);
                goto label_29;
              }
              else
                goto label_29;
            default:
              goto label_29;
          }
          predicateResult = string.Equals(str1, str2);
        }
      }
label_29:
      return predicateResult;
    }

    private static PredicateRule getPredicateRule(string visibleRule)
    {
      Regex regex = new Regex("([a-zA-Z0-9 ]+)([!=<>]+)([a-zA-Z0-9. ]+)|([a-zA-Z0-9 ]+(?=NotContains|NotEndsWith|NotStartsWith))(NotContains|NotEndsWith|NotStartsWith)([a-zA-Z0-9. ]+)|([a-zA-Z0-9 ]+(?=Contains|EndsWith|StartsWith))(Contains|EndsWith|StartsWith)([a-zA-Z0-9. ]+)");
      PredicateRule predicateRule = (PredicateRule) null;
      string input = visibleRule;
      Match match = regex.Match(input);
      if (match.Success)
      {
        string str = match.Value;
        foreach (string condition in new List<string>()
        {
          "==",
          "!=",
          "<=",
          ">=",
          "<",
          ">",
          "=",
          "NotContains",
          "NotStartsWith",
          "NotEndsWith",
          "Contains",
          "StartsWith",
          "EndsWith"
        })
        {
          if (str.Contains(condition))
          {
            string[] strArray = str.Split(condition.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length == 2)
            {
              predicateRule = new PredicateRule(strArray[0].Trim(), condition, strArray[1].Trim());
              break;
            }
            break;
          }
        }
      }
      return predicateRule;
    }
  }
}
