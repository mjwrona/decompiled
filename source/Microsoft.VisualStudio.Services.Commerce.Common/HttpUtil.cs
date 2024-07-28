// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.HttpUtil
// Assembly: Microsoft.VisualStudio.Services.Commerce.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A442E579-88AD-441C-B92A-FDB0C6C9E30B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class HttpUtil
  {
    public static string ConstructQueryParameters(
      string baseUrl,
      IDictionary<string, string> parameters)
    {
      string str1 = HttpUtil.TrimUrl(baseUrl);
      if (parameters == null || parameters.Count == 0)
        return str1;
      string str2 = string.Join("&", parameters.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (x => x.Key + "=" + x.Value)));
      return str1 + "?" + str2;
    }

    public static string TrimUrl(string url)
    {
      string str = url.Trim();
      return !str.EndsWith("/") ? str : str.Substring(0, str.Length - 1);
    }
  }
}
