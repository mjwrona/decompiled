// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildPatternFormatter
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class BuildPatternFormatter
  {
    public static string FormatBuildNumber(
      BuildDefinition definition,
      BuildData build,
      IDictionary<string, VariableValue> variables,
      string format,
      DateTime now)
    {
      Dictionary<string, string> definitionVariables = BuildPatternFormatter.GetDefinitionVariables(definition, build, variables, now);
      string str = BuildCommonUtil.ExpandEnvironmentVariables(format, (IDictionary<string, string>) definitionVariables, (Func<string, string, string>) ((x, y) => BuildPatternFormatter.ProcessBuildNumberVariableExpansion(build, x, y, now, false)));
      string buildNumber = BuildCommonUtil.ExpandEnvironmentVariables(format, (IDictionary<string, string>) definitionVariables, (Func<string, string, string>) ((x, y) => BuildPatternFormatter.ProcessBuildNumberVariableExpansion(build, x, y, now, true)));
      DefinitionQuality? definitionQuality1 = definition.DefinitionQuality;
      DefinitionQuality definitionQuality2 = DefinitionQuality.Draft;
      if (definitionQuality1.GetValueOrDefault() == definitionQuality2 & definitionQuality1.HasValue)
      {
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) str, (object) BuildServerResources.DraftBuildNumberSuffix());
        buildNumber = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) buildNumber, (object) BuildServerResources.DraftBuildNumberSuffix());
      }
      if (!ArgumentValidation.IsValidBuildNumber(buildNumber))
        throw new BuildNumberFormatException(BuildServerResources.BuildNumberFormatInvalidOutput((object) format, (object) buildNumber));
      return str;
    }

    public static string FormatSourceLabel(
      BuildDefinition definition,
      BuildData build,
      IDictionary<string, VariableValue> variables,
      string format,
      DateTime now)
    {
      Dictionary<string, string> definitionVariables = BuildPatternFormatter.GetDefinitionVariables(definition, build, variables, now);
      return BuildCommonUtil.ExpandEnvironmentVariables(format, (IDictionary<string, string>) definitionVariables, (Func<string, string, string>) ((x, y) => BuildPatternFormatter.ProcessSourceLabelVariableExpansion(build, x, y, now)));
    }

    private static Dictionary<string, string> GetDefinitionVariables(
      BuildDefinition definition,
      BuildData build,
      IDictionary<string, VariableValue> variables,
      DateTime now)
    {
      Dictionary<string, string> definitionVariables = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (variables != null && variables.Count > 0)
      {
        foreach (KeyValuePair<string, VariableValue> keyValuePair in variables.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (x =>
        {
          VariableValue variableValue = x.Value;
          return variableValue != null && !variableValue.IsSecret;
        })))
          definitionVariables.Add(keyValuePair.Key, keyValuePair.Value.Value ?? string.Empty);
      }
      definitionVariables["build.definitionName"] = definition.Name;
      definitionVariables["system.definitionId"] = definition.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      definitionVariables["system.teamProject"] = definition.ProjectName;
      definitionVariables["DayOfMonth"] = now.Day.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      definitionVariables["DayOfYear"] = now.DayOfYear.ToString((IFormatProvider) CultureInfo.InvariantCulture).PadLeft(3, '0');
      Dictionary<string, string> dictionary1 = definitionVariables;
      int num = now.Month;
      string str1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary1["Month"] = str1;
      definitionVariables["Year:yy"] = now.ToString("yy", (IFormatProvider) CultureInfo.InvariantCulture);
      definitionVariables["Year:yyyy"] = now.ToString("yyyy", (IFormatProvider) CultureInfo.InvariantCulture);
      Dictionary<string, string> dictionary2 = definitionVariables;
      num = now.Hour;
      string str2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary2["Hours"] = str2;
      Dictionary<string, string> dictionary3 = definitionVariables;
      num = now.Minute;
      string str3 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary3["Minutes"] = str3;
      Dictionary<string, string> dictionary4 = definitionVariables;
      num = now.Second;
      string str4 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary4["Seconds"] = str4;
      definitionVariables["BuildDefinitionName"] = definitionVariables["build.definitionName"];
      definitionVariables["BuildDefinitionId"] = definitionVariables["system.definitionId"];
      definitionVariables["TeamProject"] = definitionVariables["system.teamProject"];
      if (build != null)
      {
        definitionVariables["build.sourceBranch"] = string.IsNullOrEmpty(build.SourceBranch) ? string.Empty : build.SourceBranch.Replace("/", "_");
        definitionVariables["build.sourceVersion"] = build.SourceVersion;
        definitionVariables["build.sourceBranchName"] = build.GetShortBranchName();
        definitionVariables["SourceBranch"] = definitionVariables["build.sourceBranch"];
        definitionVariables["SourceVersion"] = definitionVariables["build.sourceVersion"];
        definitionVariables["SourceBranchName"] = definitionVariables["build.sourceBranchName"];
      }
      return definitionVariables;
    }

    private static bool GetVariableFormat(
      string toSearch,
      string variablePrefix,
      out string fullToken,
      out string variableFormat)
    {
      int startIndex = toSearch.IndexOf(variablePrefix, StringComparison.OrdinalIgnoreCase);
      if (startIndex >= 0)
      {
        int num = toSearch.IndexOf(')', startIndex);
        if (num >= 0)
        {
          fullToken = toSearch.Substring(startIndex, num - startIndex + 1);
          variableFormat = toSearch.Substring(startIndex + variablePrefix.Length, num - (startIndex + variablePrefix.Length));
          return true;
        }
      }
      fullToken = (string) null;
      variableFormat = (string) null;
      return false;
    }

    private static string ProcessBuildNumberVariableExpansion(
      BuildData build,
      string variableName,
      string currentValue,
      DateTime now,
      bool forValidation)
    {
      if (variableName.Equals(BuildPatternFormatter.Variables.BuildId, StringComparison.OrdinalIgnoreCase) || variableName.Equals("$(BuildId)", StringComparison.OrdinalIgnoreCase))
        return forValidation ? "1" : "$$BUILDID$$";
      string variableFormat;
      if (!BuildPatternFormatter.GetVariableFormat(variableName, "$(rev:", out string _, out variableFormat))
        return BuildPatternFormatter.ProcessVariableExpansion(build, variableName, currentValue, now);
      int num1 = variableFormat.IndexOf("r", StringComparison.OrdinalIgnoreCase);
      if (num1 < 0)
        return variableName;
      string str = variableFormat.Substring(0, num1);
      int num2 = Math.Min(variableFormat.Substring(num1).Length, 9);
      return forValidation ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{{0:D{1}}}", (object) str, (object) num2), (object) 1) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}$$REV{1}$$", (object) str, (object) num2);
    }

    private static string ProcessSourceLabelVariableExpansion(
      BuildData build,
      string variableName,
      string currentValue,
      DateTime now)
    {
      if (variableName.Equals(BuildPatternFormatter.Variables.BuildNumber, StringComparison.OrdinalIgnoreCase))
        return build.BuildNumber;
      if (variableName.Equals(BuildPatternFormatter.Variables.BuildId, StringComparison.OrdinalIgnoreCase) || variableName.Equals("$(BuildId)", StringComparison.OrdinalIgnoreCase))
        return build.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      string variableFormat;
      if (!BuildPatternFormatter.GetVariableFormat(variableName, "$(rev:", out string _, out variableFormat))
        return BuildPatternFormatter.ProcessVariableExpansion(build, variableName, currentValue, now);
      int num = variableFormat.IndexOf("r", StringComparison.OrdinalIgnoreCase);
      return num >= 0 ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{{0:D{1}}}", (object) variableFormat.Substring(0, num), (object) Math.Min(variableFormat.Substring(num).Length, 9)), (object) build.BuildNumberRevision.GetValueOrDefault()) : variableName;
    }

    private static string ProcessVariableExpansion(
      BuildData build,
      string variableName,
      string currentValue,
      DateTime now)
    {
      string variableFormat;
      return BuildPatternFormatter.GetVariableFormat(variableName, "$(date:", out string _, out variableFormat) ? now.ToString(variableFormat, (IFormatProvider) DateTimeFormatInfo.InvariantInfo) : currentValue;
    }

    private static class Variables
    {
      public static readonly string BuildNumber = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0})", (object) "build.buildNumber");
      public static readonly string BuildId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0})", (object) "build.buildId");
    }
  }
}
