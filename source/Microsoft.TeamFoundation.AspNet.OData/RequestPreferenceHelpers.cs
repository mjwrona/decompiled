// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.RequestPreferenceHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  internal static class RequestPreferenceHelpers
  {
    public const string PreferHeaderName = "Prefer";
    public const string ReturnContentHeaderValue = "return=representation";
    public const string ReturnNoContentHeaderValue = "return=minimal";
    public const string ODataMaxPageSize = "odata.maxpagesize";
    public const string MaxPageSize = "maxpagesize";

    internal static bool RequestPrefersReturnContent(IWebApiHeaders headers)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      return headers.TryGetValues("Prefer", out values) && values.FirstOrDefault<string>((Func<string, bool>) (s => s.IndexOf("return=representation", StringComparison.OrdinalIgnoreCase) >= 0)) != null;
    }

    internal static bool RequestPrefersReturnNoContent(IWebApiHeaders headers)
    {
      IEnumerable<string> values = (IEnumerable<string>) null;
      return headers.TryGetValues("Prefer", out values) && values.FirstOrDefault<string>((Func<string, bool>) (s => s.IndexOf("return=minimal", StringComparison.OrdinalIgnoreCase) >= 0)) != null;
    }

    internal static bool RequestPrefersMaxPageSize(IWebApiHeaders headers, out int pageSize)
    {
      pageSize = -1;
      IEnumerable<string> values = (IEnumerable<string>) null;
      if (headers.TryGetValues("Prefer", out values))
      {
        pageSize = RequestPreferenceHelpers.GetMaxPageSize(values, "maxpagesize");
        if (pageSize >= 0)
          return true;
        pageSize = RequestPreferenceHelpers.GetMaxPageSize(values, "odata.maxpagesize");
        if (pageSize >= 0)
          return true;
      }
      return false;
    }

    private static int GetMaxPageSize(IEnumerable<string> preferences, string preferenceHeaderName)
    {
      string str1 = preferences.FirstOrDefault<string>((Func<string, bool>) (s => s.IndexOf(preferenceHeaderName, StringComparison.OrdinalIgnoreCase) >= 0));
      if (string.IsNullOrEmpty(str1))
        return -1;
      int num = str1.IndexOf(preferenceHeaderName, StringComparison.OrdinalIgnoreCase) + preferenceHeaderName.Length;
      string empty = string.Empty;
      string str2 = str1;
      int index1 = num;
      int index2 = index1 + 1;
      if (str2[index1] == '=')
      {
        while (index2 < str1.Length && char.IsDigit(str1[index2]))
          empty += str1[index2++].ToString();
      }
      int result = -1;
      return int.TryParse(empty, out result) ? result : -1;
    }

    internal static string GetRequestPreferHeader(IWebApiHeaders headers)
    {
      IEnumerable<string> values;
      return headers.TryGetValues("Prefer", out values) ? values.FirstOrDefault<string>() : (string) null;
    }
  }
}
