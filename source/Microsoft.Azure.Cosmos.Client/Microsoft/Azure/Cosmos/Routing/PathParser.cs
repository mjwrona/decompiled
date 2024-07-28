// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PathParser
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class PathParser
  {
    private static char segmentSeparator = '/';
    private static string errorMessageFormat = "Invalid path, failed at {0}";

    public static IReadOnlyList<string> GetPathParts(string path)
    {
      List<string> pathParts = new List<string>();
      int currentIndex = 0;
      while (currentIndex < path.Length)
      {
        if ((int) path[currentIndex] != (int) PathParser.segmentSeparator)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, PathParser.errorMessageFormat, (object) currentIndex));
        if (++currentIndex != path.Length)
        {
          if (path[currentIndex] == '"' || path[currentIndex] == '\'')
            pathParts.Add(PathParser.GetEscapedToken(path, ref currentIndex));
          else
            pathParts.Add(PathParser.GetToken(path, ref currentIndex));
        }
        else
          break;
      }
      return (IReadOnlyList<string>) pathParts;
    }

    private static string GetEscapedToken(string path, ref int currentIndex)
    {
      char ch = path[currentIndex];
      int startIndex = ++currentIndex;
      int num;
      while (true)
      {
        num = path.IndexOf(ch, startIndex);
        if (num != -1)
        {
          if (path[num - 1] == '\\')
            startIndex = num + 1;
          else
            goto label_5;
        }
        else
          break;
      }
      throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, PathParser.errorMessageFormat, (object) currentIndex));
label_5:
      string escapedToken = path.Substring(currentIndex, num - currentIndex);
      currentIndex = num + 1;
      return escapedToken;
    }

    private static string GetToken(string path, ref int currentIndex)
    {
      int num = path.IndexOf(PathParser.segmentSeparator, currentIndex);
      string str;
      if (num == -1)
      {
        str = path.Substring(currentIndex);
        currentIndex = path.Length;
      }
      else
      {
        str = path.Substring(currentIndex, num - currentIndex);
        currentIndex = num;
      }
      return str.Trim();
    }
  }
}
