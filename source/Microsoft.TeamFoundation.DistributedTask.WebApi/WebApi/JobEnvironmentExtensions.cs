// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobEnvironmentExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal static class JobEnvironmentExtensions
  {
    private static readonly Lazy<Regex> s_processParameterRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("^(Parameters|ProcParam)[\\.]([^)]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline)), true);

    public static Dictionary<string, string> GetProcessParameters(this JobEnvironment environment) => environment.Variables.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (v => JobEnvironmentExtensions.s_processParameterRegex.Value.IsMatch(v.Key))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (v => v.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
