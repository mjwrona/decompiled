// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.Utils
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Antlr4.Runtime.Misc
{
  internal class Utils
  {
    public static string Join<T>(string separator, IEnumerable<T> items)
    {
      ArrayList<string> arrayList = new ArrayList<string>();
      foreach (T obj in items)
      {
        if ((object) obj == null)
          arrayList.Add("");
        else
          arrayList.Add(obj.ToString());
      }
      return string.Join(separator, arrayList.ToArray());
    }

    public static int NumNonnull(object[] data)
    {
      int num = 0;
      if (data == null)
        return num;
      foreach (object obj in data)
      {
        if (obj != null)
          ++num;
      }
      return num;
    }

    public static void RemoveAllElements<T>(ICollection<T> data, T value)
    {
      if (data == null)
        return;
      while (data.Contains(value))
        data.Remove(value);
    }

    public static string EscapeWhitespace(string s, bool escapeSpaces)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char ch in s.ToCharArray())
      {
        if (ch == ' ' & escapeSpaces)
        {
          stringBuilder.Append('·');
        }
        else
        {
          switch (ch)
          {
            case '\t':
              stringBuilder.Append("\\t");
              continue;
            case '\n':
              stringBuilder.Append("\\n");
              continue;
            case '\r':
              stringBuilder.Append("\\r");
              continue;
            default:
              stringBuilder.Append(ch);
              continue;
          }
        }
      }
      return stringBuilder.ToString();
    }

    public static void RemoveAll<T>(IList<T> list, Predicate<T> predicate)
    {
      int index1 = 0;
      for (int index2 = 0; index2 < list.Count; ++index2)
      {
        T obj = list[index2];
        if (!predicate(obj))
        {
          if (index1 != index2)
            list[index1] = obj;
          ++index1;
        }
      }
      while (index1 < list.Count)
        list.RemoveAt(list.Count - 1);
    }

    public static IDictionary<string, int> ToMap(string[] keys)
    {
      IDictionary<string, int> map = (IDictionary<string, int>) new Dictionary<string, int>();
      for (int index = 0; index < keys.Length; ++index)
        map[keys[index]] = index;
      return map;
    }
  }
}
