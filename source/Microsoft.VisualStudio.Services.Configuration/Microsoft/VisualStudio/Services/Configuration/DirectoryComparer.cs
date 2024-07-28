// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.DirectoryComparer
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class DirectoryComparer
  {
    public List<Tuple<string, string>> Compare(
      string dir1,
      string dir2,
      string searchPattern,
      SearchOption searchOption)
    {
      List<Tuple<string, string>> tupleList = new List<Tuple<string, string>>();
      if (!dir1.EndsWith("\\", StringComparison.Ordinal))
        dir1 += "\\";
      if (!dir2.EndsWith("\\", StringComparison.Ordinal))
        dir2 += "\\";
      string[] array;
      if (Directory.Exists(dir1))
      {
        array = Directory.GetFiles(dir1, searchPattern, searchOption);
        DirectoryComparer.RemovePrefix(array, dir1);
      }
      else
        array = Array.Empty<string>();
      HashSet<string> stringSet;
      if (Directory.Exists(dir2))
      {
        string[] files = Directory.GetFiles(dir2, searchPattern, searchOption);
        DirectoryComparer.RemovePrefix(files, dir2);
        stringSet = new HashSet<string>((IEnumerable<string>) files, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      }
      else
        stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < array.Length; ++index)
      {
        string str = array[index];
        if (!stringSet.Remove(str))
          tupleList.Add(new Tuple<string, string>(dir1 + str, dir2 + str));
      }
      foreach (string str in stringSet)
        tupleList.Add(new Tuple<string, string>(dir2 + str, dir1 + str));
      return tupleList;
    }

    private static void RemovePrefix(string[] array, string prefix)
    {
      int length = prefix.Length;
      for (int index = 0; index < array.Length; ++index)
        array[index] = array[index].Substring(length);
    }
  }
}
