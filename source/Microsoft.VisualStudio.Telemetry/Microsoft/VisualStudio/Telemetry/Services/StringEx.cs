// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.Services.StringEx
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry.Services
{
  internal static class StringEx
  {
    private const int MaxPath = 255;
    private static char[] buffer = new char[(int) byte.MaxValue];

    public static string NormalizePath(this string path)
    {
      if (string.IsNullOrEmpty(path))
        return path;
      int index1 = 0;
      while (index1 < path.Length && StringEx.IsSkippable(path[index1]))
        ++index1;
      if (index1 == path.Length)
        return string.Empty;
      int index2 = path.Length - 1;
      while (StringEx.IsSkippable(path[index2]))
        --index2;
      int num = index2 - index1 + 1;
      if (num > (int) byte.MaxValue)
        throw new ArgumentException("Too long path", nameof (path));
      char[] chArray = Interlocked.Exchange<char[]>(ref StringEx.buffer, (char[]) null) ?? new char[(int) byte.MaxValue];
      bool flag = num != path.Length;
      int length = 0;
      char ch1 = ' ';
      for (int index3 = index1; index3 <= index2; ++index3)
      {
        char ch2 = path[index3];
        if (ch2 == '/')
        {
          ch2 = '\\';
          flag = true;
        }
        if (ch2 == '\\' && ch1 == '\\')
        {
          flag = true;
        }
        else
        {
          ch1 = ch2;
          chArray[length++] = ch2;
        }
      }
      if (flag)
        path = new string(chArray, 0, length);
      Interlocked.Exchange<char[]>(ref StringEx.buffer, chArray);
      return path;
    }

    public static string GetRootSubCollectionOfPath(this string path)
    {
      path = path.NormalizePath();
      int length = path.IndexOf('\\');
      return length > 0 ? path.Substring(0, length) : path;
    }

    private static bool IsSkippable(char c) => char.IsWhiteSpace(c) || c == '/' || c == '\\';
  }
}
