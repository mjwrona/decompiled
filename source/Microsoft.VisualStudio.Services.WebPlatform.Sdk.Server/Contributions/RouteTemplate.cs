// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.RouteTemplate
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  internal static class RouteTemplate
  {
    private static char[] s_namespaceSeparator = new char[1]
    {
      '.'
    };

    public static string Parse(
      string routeTemplate,
      out Dictionary<string, RouteParameter> routeParameters)
    {
      StringBuilder stringBuilder = new StringBuilder(routeTemplate);
      bool flag = false;
      int num1 = -1;
      int num2 = 0;
      routeParameters = new Dictionary<string, RouteParameter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      int index = 0;
      int num3 = 0;
      while (index < routeTemplate.Length)
      {
        switch (routeTemplate[index])
        {
          case '[':
            if (num1 == -1)
            {
              num1 = index;
              flag = true;
              break;
            }
            break;
          case ']':
            if (num1 >= 0 && flag)
            {
              RouteParameter parameter = RouteTemplate.CreateParameter(routeTemplate.Substring(num1 + 1, index - num1 - 1));
              routeParameters[parameter.ParameterName] = parameter;
              stringBuilder.Remove(num1 - num2, index - num1 + 1);
              num2 += index - num1 + 1;
              num1 = -1;
              break;
            }
            break;
          case '{':
            if (num1 == -1)
            {
              num1 = index;
              flag = false;
              break;
            }
            break;
          case '}':
            if (num1 >= 0 && !flag)
            {
              RouteParameter parameter = RouteTemplate.CreateParameter(routeTemplate.Substring(num1 + 1, index - num1 - 1));
              routeParameters[parameter.ParameterName] = parameter;
              if (parameter.ParameterNamespace != null)
              {
                stringBuilder.Remove(num1 + 1 - num2, parameter.ParameterNamespace.Length + 1);
                num2 += parameter.ParameterNamespace.Length + 1;
              }
              num1 = -1;
              break;
            }
            break;
        }
        ++index;
        ++num3;
      }
      if (stringBuilder.Length != routeTemplate.Length)
        routeTemplate = stringBuilder.ToString();
      return routeTemplate;
    }

    private static RouteParameter CreateParameter(string parameterString)
    {
      RouteParameter parameter = new RouteParameter();
      string[] strArray = parameterString.Split(RouteTemplate.s_namespaceSeparator, 2);
      if (strArray.Length == 1)
        parameter.ParameterName = strArray[0];
      else if (strArray.Length == 2)
      {
        parameter.ParameterNamespace = strArray[0];
        parameter.ParameterName = strArray[1];
      }
      if (parameter.ParameterName.Length > 0 && (parameter.Wildcard = parameter.ParameterName[0] == '*'))
        parameter.ParameterName = parameter.ParameterName.Substring(1);
      parameter.ParameterString = parameterString;
      return parameter;
    }
  }
}
