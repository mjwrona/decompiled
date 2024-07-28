// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VariableUtility
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public static class VariableUtility
  {
    private static readonly Lazy<Regex> s_variableReferenceRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("\\$\\(([^)]+)\\)", RegexOptions.Compiled | RegexOptions.Singleline)), true);
    private static readonly Lazy<Regex> s_conditionVariableReferenceRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("variables\\['([^']+)\\']", RegexOptions.Compiled | RegexOptions.Singleline)), true);
    private const string c_conditionReplacementFormat = "'{0}'";
    private const string c_variableFormat = "$({0})";
    private const string c_conditionVariableFormat = "variables['{0}']";

    public static EnableAccessTokenType GetEnableAccessTokenType(
      IDictionary<string, VariableValue> variables)
    {
      VariableValue variableValue;
      EnableAccessTokenType result;
      if (variables != null && variables.TryGetValue(WellKnownDistributedTaskVariables.EnableAccessToken, out variableValue) && variableValue != null)
        Enum.TryParse<EnableAccessTokenType>(variableValue.Value, true, out result);
      else
        result = EnableAccessTokenType.None;
      return result;
    }

    public static bool IsVariable(string value) => VariableUtility.s_variableReferenceRegex.Value.IsMatch(value);

    public static JToken ExpandVariables(
      JToken token,
      IDictionary<string, string> replacementDictionary,
      bool useMachineVariables = true)
    {
      Dictionary<JTokenType, Func<JToken, JToken>> mapFuncs = new Dictionary<JTokenType, Func<JToken, JToken>>()
      {
        {
          JTokenType.String,
          (Func<JToken, JToken>) (t => (JToken) VariableUtility.ExpandVariables(t.ToString(), replacementDictionary, useMachineVariables))
        }
      };
      return token.Map(mapFuncs);
    }

    public static JToken ExpandVariables(
      JToken token,
      VariablesDictionary additionalVariableReplacements,
      bool useMachineVariables)
    {
      return VariableUtility.ExpandVariables(token, (IDictionary<string, string>) additionalVariableReplacements, useMachineVariables);
    }

    public static JToken ExpandVariables(
      JToken token,
      IList<IDictionary<string, string>> replacementsList)
    {
      Dictionary<JTokenType, Func<JToken, JToken>> mapFuncs = new Dictionary<JTokenType, Func<JToken, JToken>>()
      {
        {
          JTokenType.String,
          (Func<JToken, JToken>) (t => replacementsList.Aggregate<IDictionary<string, string>, JToken>(t, (Func<JToken, IDictionary<string, string>, JToken>) ((current, replacementVariables) => (JToken) VariableUtility.ExpandVariables(current.ToString(), replacementVariables))))
        }
      };
      return token.Map(mapFuncs);
    }

    public static string ExpandVariables(
      string input,
      IDictionary<string, string> additionalVariableReplacements)
    {
      return VariableUtility.ExpandVariables(input, additionalVariableReplacements, true);
    }

    public static string ExpandVariables(
      string input,
      IDictionary<string, string> additionalVariableReplacements,
      bool useMachineVariables)
    {
      if (!VariableUtility.s_variableReferenceRegex.Value.IsMatch(input))
        return input;
      StringBuilder stringBuilder = new StringBuilder(input);
      List<string> referencedVariables = VariableUtility.GetReferencedVariables(input);
      for (int index = 0; index < referencedVariables.Count; ++index)
      {
        string str = referencedVariables[index].Substring(2, referencedVariables[index].Length - 3);
        string environmentVariable;
        if (!additionalVariableReplacements.TryGetValue(str, out environmentVariable) & useMachineVariables)
          environmentVariable = Environment.GetEnvironmentVariable(str);
        if (environmentVariable != null)
          stringBuilder.Replace(referencedVariables[index], environmentVariable);
      }
      return stringBuilder.ToString();
    }

    public static string ExpandVariables(
      string input,
      VariablesDictionary additionalVariableReplacements,
      bool useMachineVariables,
      bool maskSecrets = false)
    {
      return VariableUtility.ExpandVariables(input, (IDictionary<string, VariableValue>) additionalVariableReplacements, useMachineVariables, maskSecrets);
    }

    public static string ExpandVariables(
      string input,
      IDictionary<string, VariableValue> additionalVariableReplacements,
      bool useMachineVariables,
      bool maskSecrets = false)
    {
      if (string.IsNullOrEmpty(input))
        return input;
      StringBuilder stringBuilder = new StringBuilder(input);
      List<string> referencedVariables = VariableUtility.GetReferencedVariables(input);
      for (int index = 0; index < referencedVariables.Count; ++index)
      {
        string str = referencedVariables[index].Substring(2, referencedVariables[index].Length - 3);
        VariableValue variableValue;
        if (!additionalVariableReplacements.TryGetValue(str, out variableValue) & useMachineVariables)
          variableValue = new VariableValue()
          {
            Value = Environment.GetEnvironmentVariable(str)
          };
        if (variableValue != null)
        {
          string newValue = variableValue.Value;
          if (variableValue.IsSecret & maskSecrets)
            newValue = "***";
          stringBuilder.Replace(referencedVariables[index], newValue);
        }
      }
      return stringBuilder.ToString();
    }

    public static string ExpandConditionVariables(
      string condition,
      IDictionary<string, string> additionalVariableReplacements,
      bool useMachineVariables)
    {
      if (!VariableUtility.s_conditionVariableReferenceRegex.Value.IsMatch(condition))
        return condition;
      StringBuilder stringBuilder = new StringBuilder(condition);
      MatchCollection matchCollection = VariableUtility.s_conditionVariableReferenceRegex.Value.Matches(condition);
      for (int i = 0; i < matchCollection.Count; ++i)
      {
        if (matchCollection[i].Length != 0 && matchCollection[i].Groups.Count >= 2)
        {
          string oldValue = matchCollection[i].Groups[0].Value;
          string str = matchCollection[i].Groups[1].Value;
          string environmentVariable;
          if (!additionalVariableReplacements.TryGetValue(str, out environmentVariable) & useMachineVariables)
            environmentVariable = Environment.GetEnvironmentVariable(str);
          if (environmentVariable != null)
          {
            string newValue = VariableUtility.PrepareReplacementStringForConditions(environmentVariable);
            stringBuilder.Replace(oldValue, newValue);
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static string PrepareReplacementStringForConditions(string replacementValue)
    {
      if (replacementValue == null || !VariableUtility.IsVariable(replacementValue))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) replacementValue);
      List<string> referencedVariables = VariableUtility.GetReferencedVariables(replacementValue);
      return referencedVariables.Count != 1 || replacementValue.Trim() != referencedVariables[0] ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}'", (object) replacementValue) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "variables['{0}']", (object) referencedVariables[0].Substring(2, referencedVariables[0].Length - 3));
    }

    public static List<string> GetReferencedVariables(string input)
    {
      int num = -1;
      bool flag = false;
      StringBuilder stringBuilder = new StringBuilder();
      HashSet<string> source = new HashSet<string>();
      for (int index = 0; index < input.Length; ++index)
      {
        if (!flag && input[index] == '$' && index + 1 < input.Length && input[index + 1] == '(')
          flag = true;
        if (flag)
          stringBuilder.Append(input[index]);
        if (flag && input[index] == '(')
          ++num;
        if (flag && input[index] == ')')
        {
          if (num == 0)
          {
            source.Add(stringBuilder.ToString());
            stringBuilder.Clear();
            flag = false;
            num = -1;
          }
          else
            --num;
        }
      }
      if (!flag)
        ;
      return source.ToList<string>();
    }
  }
}
