// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskDefinitionExtensions
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
  public static class TaskDefinitionExtensions
  {
    private static readonly Regex m_InputRegex = new Regex("\\$\\((?<inputName>.+?)\\)", RegexOptions.Compiled);

    public static string ComputeDisplayName(
      this TaskDefinition taskDefinition,
      IDictionary<string, string> inputs)
    {
      if (!string.IsNullOrEmpty(taskDefinition.InstanceNameFormat))
      {
        string str;
        return TaskDefinitionExtensions.m_InputRegex.Replace(taskDefinition.InstanceNameFormat, (MatchEvaluator) (match => inputs.TryGetValue(match.Groups["inputName"].Value, out str) ? str : match.Value));
      }
      return !string.IsNullOrEmpty(taskDefinition.FriendlyName) ? taskDefinition.FriendlyName : taskDefinition.Name;
    }

    internal static bool IsVisible(this TaskDefinition taskDefinition, string[] visiblities)
    {
      if (visiblities != null && visiblities.Length != 0)
      {
        IList<string> visibility = taskDefinition.Visibility;
        string[] array = visibility != null ? visibility.Where<string>((Func<string, bool>) (x => !string.Equals(x, "Preview", StringComparison.OrdinalIgnoreCase))).ToArray<string>() : (string[]) null;
        if (array != null && array.Length != 0)
          return ((IEnumerable<string>) array).Intersect<string>((IEnumerable<string>) visiblities, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Any<string>();
      }
      return true;
    }
  }
}
