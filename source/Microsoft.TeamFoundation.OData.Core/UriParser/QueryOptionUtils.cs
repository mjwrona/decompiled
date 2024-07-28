// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.QueryOptionUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  internal static class QueryOptionUtils
  {
    internal static Dictionary<string, string> GetParameterAliases(
      this List<CustomQueryOptionToken> queryOptions)
    {
      Dictionary<string, string> parameterAliases = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.Ordinal);
      List<CustomQueryOptionToken> queryOptionTokenList = new List<CustomQueryOptionToken>();
      foreach (CustomQueryOptionToken queryOption in queryOptions)
      {
        if (!string.IsNullOrEmpty(queryOption.Name) && queryOption.Name[0] == '@')
        {
          parameterAliases[queryOption.Name] = queryOption.Value;
          queryOptionTokenList.Add(queryOption);
        }
      }
      foreach (CustomQueryOptionToken queryOptionToken in queryOptionTokenList)
        queryOptions.Remove(queryOptionToken);
      return parameterAliases;
    }

    internal static string GetQueryOptionValue(
      this List<CustomQueryOptionToken> queryOptions,
      string queryOptionName)
    {
      CustomQueryOptionToken queryOptionToken = (CustomQueryOptionToken) null;
      foreach (CustomQueryOptionToken queryOption in queryOptions)
      {
        if (queryOption.Name == queryOptionName)
          queryOptionToken = queryOptionToken == null ? queryOption : throw new ODataException(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce((object) queryOptionName));
      }
      return queryOptionToken?.Value;
    }

    internal static string GetQueryOptionValueAndRemove(
      this List<CustomQueryOptionToken> queryOptions,
      string queryOptionName)
    {
      CustomQueryOptionToken queryOptionToken = (CustomQueryOptionToken) null;
      for (int index = 0; index < queryOptions.Count; ++index)
      {
        if (queryOptions[index].Name == queryOptionName)
        {
          queryOptionToken = queryOptionToken == null ? queryOptions[index] : throw new ODataException(Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce((object) queryOptionName));
          queryOptions.RemoveAt(index);
          --index;
        }
      }
      return queryOptionToken?.Value;
    }

    internal static List<CustomQueryOptionToken> ParseQueryOptions(Uri uri)
    {
      List<CustomQueryOptionToken> queryOptions = new List<CustomQueryOptionToken>();
      string str1 = uri.Query.Replace('+', ' ');
      int num1;
      if (str1 != null)
      {
        if (str1.Length > 0 && str1[0] == '?')
          str1 = str1.Substring(1);
        num1 = str1.Length;
      }
      else
        num1 = 0;
      for (int index = 0; index < num1; ++index)
      {
        int startIndex = index;
        int num2 = -1;
        for (; index < num1; ++index)
        {
          switch (str1[index])
          {
            case '&':
              goto label_12;
            case '=':
              if (num2 < 0)
              {
                num2 = index;
                break;
              }
              break;
          }
        }
label_12:
        string stringToUnescape1 = (string) null;
        string stringToUnescape2;
        if (num2 >= 0)
        {
          stringToUnescape1 = str1.Substring(startIndex, num2 - startIndex);
          stringToUnescape2 = str1.Substring(num2 + 1, index - num2 - 1);
        }
        else
          stringToUnescape2 = str1.Substring(startIndex, index - startIndex);
        string name = stringToUnescape1 == null ? (string) null : Uri.UnescapeDataString(stringToUnescape1).Trim();
        string str2 = stringToUnescape2 == null ? (string) null : Uri.UnescapeDataString(stringToUnescape2).Trim();
        queryOptions.Add(new CustomQueryOptionToken(name, str2));
        if (index == num1 - 1 && str1[index] == '&')
          queryOptions.Add(new CustomQueryOptionToken((string) null, string.Empty));
      }
      return queryOptions;
    }
  }
}
