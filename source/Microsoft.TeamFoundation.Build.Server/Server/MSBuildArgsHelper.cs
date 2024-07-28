// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.MSBuildArgsHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Build.Server
{
  public static class MSBuildArgsHelper
  {
    private static readonly Lazy<Regex> Dev12MSBuildArgsRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("<mtbc:BuildParameter x:Key=\"AdvancedBuildSettings\">(.*?)</mtbc:BuildParameter>", RegexOptions.Compiled)));
    private static readonly Lazy<Regex> Dev11MSBuildArgsRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("<x:String x:Key=\"MSBuildArguments\".*?>(.*?)</x:String>", RegexOptions.Compiled)));

    public static string GetMSBuildArguments(string processParameters)
    {
      if (string.IsNullOrEmpty(processParameters))
        return string.Empty;
      string empty = string.Empty;
      bool flag = false;
      MatchCollection matchCollection1 = MSBuildArgsHelper.Dev12MSBuildArgsRegex.Value.Matches(processParameters);
      if (matchCollection1.Count == 1 && matchCollection1[0].Groups.Count == 2)
      {
        string json = matchCollection1[0].Groups[1].Value;
        if (!string.IsNullOrEmpty(json))
        {
          try
          {
            empty = new BuildParameter(json).GetValue<string>("MSBuildArguments");
            flag = true;
          }
          catch (BuildParameterException ex)
          {
          }
        }
      }
      if (!flag)
      {
        MatchCollection matchCollection2 = MSBuildArgsHelper.Dev11MSBuildArgsRegex.Value.Matches(processParameters);
        if (matchCollection2.Count == 1 && matchCollection2[0].Groups.Count == 2)
          empty = matchCollection2[0].Groups[1].Value;
      }
      return empty;
    }

    public static string CreateMSBuildArgsProcessParameters(string msbuildArgs)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("<Dictionary x:TypeArguments=\"x:String, x:Object\" xmlns=\"clr-namespace:System.Collections.Generic;assembly=mscorlib\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
      stringBuilder.Append("<x:String x:Key=\"MSBuildArguments\">");
      stringBuilder.Append(msbuildArgs);
      stringBuilder.Append("</x:String>");
      stringBuilder.Append("</Dictionary>");
      return stringBuilder.ToString();
    }
  }
}
