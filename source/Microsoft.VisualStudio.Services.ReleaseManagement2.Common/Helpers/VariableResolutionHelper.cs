// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.VariableResolutionHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class VariableResolutionHelper
  {
    private const string VariableFormatPattern = "\\$\\(([^)]+)\\)";

    public static string ResolveVariableValue(
      string input,
      IDictionary<string, string> allVariables,
      IDictionary<string, string> visitedVariables)
    {
      if (allVariables == null)
        throw new ArgumentNullException(nameof (allVariables));
      if (visitedVariables == null)
        visitedVariables = (IDictionary<string, string>) new Dictionary<string, string>();
      if (string.IsNullOrWhiteSpace(input))
        return input;
      if (!Regex.IsMatch(input, "\\$\\(([^)]+)\\)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500.0)))
        return input.Trim();
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      MatchCollection matchCollection = Regex.Matches(input, "\\$\\(([^)]+)\\)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500.0));
      StringBuilder stringBuilder = new StringBuilder(input);
      for (int i = 0; i < matchCollection.Count; ++i)
      {
        GroupCollection groups = matchCollection[i].Groups;
        string key = groups[1].Value;
        string empty = string.Empty;
        string newValue;
        if (visitedVariables.ContainsKey(key))
          newValue = !string.IsNullOrWhiteSpace(visitedVariables[key]) ? visitedVariables[key] : groups[0].Value;
        else if (dictionary.ContainsKey(groups[0].Value))
        {
          newValue = dictionary[key];
        }
        else
        {
          if (!visitedVariables.ContainsKey(key))
            visitedVariables[key] = (string) null;
          newValue = !allVariables.ContainsKey(key) ? groups[0].Value : VariableResolutionHelper.ResolveVariableValue(allVariables[key], allVariables, visitedVariables);
          visitedVariables[key] = newValue;
        }
        stringBuilder.Replace(groups[0].Value, newValue);
        if (!dictionary.ContainsKey(groups[0].Value))
          dictionary.Add(groups[0].Value, newValue);
      }
      return stringBuilder.ToString().Trim();
    }
  }
}
