// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlErrorExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SqlErrorExtensions
  {
    public static string ExtractString(this SqlError error, string key)
    {
      List<string> strings = error.ExtractStrings(key);
      return strings.Count == 0 ? string.Empty : strings[0];
    }

    public static List<string> ExtractStrings(this SqlError error, string key)
    {
      string str = "%" + key + "=\"";
      string message = error.Message;
      List<string> strings = new List<string>();
      int startIndex = 0;
      while (startIndex < message.Length)
      {
        int num1 = message.IndexOf(str, startIndex, StringComparison.Ordinal);
        if (num1 != -1)
        {
          int num2 = message.IndexOf("\";%", num1 + 1, StringComparison.Ordinal);
          if (num2 == -1)
          {
            startIndex = num1 + 1;
          }
          else
          {
            strings.Add(message.Substring(num1 + str.Length, num2 - (num1 + str.Length)));
            startIndex = num2 + 1;
          }
        }
        else
          break;
      }
      return strings;
    }

    public static int ExtractInt(this SqlError error, string key)
    {
      int result;
      return int.TryParse(error.ExtractString(key), out result) ? result : -1;
    }

    public static long ExtractLong(this SqlError error, string key)
    {
      long result;
      return long.TryParse(error.ExtractString(key), out result) ? result : -1L;
    }

    public static List<int> ExtractInts(this SqlError error, string key)
    {
      List<string> strings = error.ExtractStrings(key);
      List<int> ints = new List<int>(strings.Count);
      foreach (string s in strings)
      {
        int result;
        if (int.TryParse(s, out result))
          ints.Add(result);
      }
      return ints;
    }

    public static string ExtractEnumName(this SqlError error, string key, Type enumerationType)
    {
      string s = error.ExtractString(key);
      int result;
      return int.TryParse(s, out result) ? Enum.Format(enumerationType, (object) result, "G") : s;
    }

    public static bool IsInformational(this SqlError error) => error.Class <= (byte) 10;

    public static bool IsStatistical(this SqlError error) => error.Number == 3612 || error.Number == 3613 || error.Number == 3615;
  }
}
