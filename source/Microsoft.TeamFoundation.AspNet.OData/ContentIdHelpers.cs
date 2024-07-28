// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ContentIdHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData
{
  internal static class ContentIdHelpers
  {
    public static string ResolveContentId(
      string url,
      IDictionary<string, string> contentIdToLocationMapping)
    {
      int startIndex = 0;
      int num;
      int length;
      string str;
      while (true)
      {
        num = url.IndexOf('$', startIndex);
        if (num != -1)
        {
          length = 0;
          while (num + length < url.Length - 1 && ContentIdHelpers.IsContentIdCharacter(url[num + length + 1]))
            ++length;
          if (length > 0)
          {
            string key = url.Substring(num + 1, length);
            if (contentIdToLocationMapping.TryGetValue(key, out str))
              break;
          }
          startIndex = num + 1;
        }
        else
          goto label_9;
      }
      return str + url.Substring(num + 1 + length);
label_9:
      return url;
    }

    public static void AddLocationHeaderToMapping(
      Uri location,
      IDictionary<string, string> contentIdToLocationMapping,
      string contentId)
    {
      if (!(location != (Uri) null))
        return;
      contentIdToLocationMapping.Add(contentId, location.AbsoluteUri);
    }

    private static bool IsContentIdCharacter(char c)
    {
      if (c <= '.')
      {
        if (c != '-' && c != '.')
          goto label_4;
      }
      else if (c != '_' && c != '~')
        goto label_4;
      return true;
label_4:
      return char.IsLetterOrDigit(c);
    }
  }
}
