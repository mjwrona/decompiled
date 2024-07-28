// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseHistoryChangeDetailsExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseHistoryChangeDetailsExtensions
  {
    public static string ExtractString(string changeDetails, string key)
    {
      List<string> strings = ReleaseHistoryChangeDetailsExtensions.ExtractStrings(changeDetails, key);
      return strings.Count != 0 ? strings[0] : string.Empty;
    }

    [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Ignoring for now.")]
    public static List<string> ExtractStrings(string changeDetails, string key)
    {
      List<string> strings = new List<string>();
      if (changeDetails == null)
        return strings;
      string str = "%" + key + "=\"";
      int startIndex = 0;
      while (startIndex < changeDetails.Length)
      {
        int num1 = changeDetails.IndexOf(str, startIndex, StringComparison.Ordinal);
        if (num1 != -1)
        {
          int num2 = changeDetails.IndexOf("\";%", num1 + 1, StringComparison.Ordinal);
          if (num2 == -1)
          {
            startIndex = num1 + 1;
          }
          else
          {
            strings.Add(changeDetails.Substring(num1 + str.Length, num2 - (num1 + str.Length)));
            startIndex = num2 + 1;
          }
        }
        else
          break;
      }
      return strings;
    }

    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "int", Justification = "Ignoring for now.")]
    public static int ExtractInt(string changeDetails, string key)
    {
      int result;
      return int.TryParse(ReleaseHistoryChangeDetailsExtensions.ExtractString(changeDetails, key), out result) ? result : -1;
    }

    public static string ExtractEnumName(string inputMessage, string key, Type enumerationType)
    {
      string s = ReleaseHistoryChangeDetailsExtensions.ExtractString(inputMessage, key);
      int result;
      return !int.TryParse(s, out result) ? s : Enum.Format(enumerationType, (object) result, "G");
    }
  }
}
