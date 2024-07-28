// Decompiled with JetBrains decompiler
// Type: WebGrease.Extensions.StringPathExtensions
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace WebGrease.Extensions
{
  public static class StringPathExtensions
  {
    public static string EnsureEndSeparator(this string directory) => directory.EndsWith(new string(Path.DirectorySeparatorChar, 1), StringComparison.OrdinalIgnoreCase) ? directory : directory + (object) Path.DirectorySeparatorChar;

    internal static string GetFullPathWithLowercase(this string originalPath)
    {
      if (string.IsNullOrWhiteSpace(originalPath))
        return originalPath;
      try
      {
        return string.IsNullOrWhiteSpace(originalPath) ? originalPath : Path.GetFullPath(originalPath).ToLower(CultureInfo.CurrentUICulture);
      }
      catch (Exception ex)
      {
        Trace.TraceWarning("Exception occurred while trying to get the full path for path: {0}\r\n{1} ".InvariantFormat((object) originalPath, (object) ex.ToString()));
        return originalPath.ToLower(CultureInfo.CurrentUICulture);
      }
    }

    internal static string MakeAbsoluteTo(this string pathToConvert, string pathToConvertFrom)
    {
      try
      {
        return string.IsNullOrWhiteSpace(pathToConvert) || string.IsNullOrWhiteSpace(pathToConvertFrom) ? pathToConvert : Path.Combine(Path.GetDirectoryName(pathToConvertFrom), pathToConvert).GetFullPathWithLowercase();
      }
      catch (Exception ex)
      {
        Trace.TraceWarning("Exception occurred while trying make {0} absolute to {1}\r\n{2} ".InvariantFormat((object) pathToConvert, (object) pathToConvertFrom, (object) ex.ToString()));
        return pathToConvert;
      }
    }

    internal static string MakeRelativeTo(
      this string pathToConvert,
      string pathToConvertFrom,
      params char[] separators)
    {
      if (string.IsNullOrWhiteSpace(pathToConvert))
        throw new ArgumentNullException(nameof (pathToConvert));
      if (pathToConvertFrom.IsNullOrWhitespace())
        return (string) null;
      char ch1 = Path.DirectorySeparatorChar;
      char ch2 = Path.AltDirectorySeparatorChar;
      if (separators != null && separators.Length == 2)
      {
        ch1 = separators[0];
        ch2 = separators[1];
      }
      string[] strArray1 = pathToConvert.Split(ch1);
      string[] strArray2 = pathToConvertFrom.Split(ch1);
      if (strArray2.Length == 0 || strArray1.Length == 0 || strArray2[0] != strArray1[0])
        return pathToConvert;
      int index1 = 1;
      while (index1 < strArray2.Length && index1 < strArray1.Length && !(strArray2[index1] != strArray1[index1]))
        ++index1;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index2 = index1; index2 < strArray2.Length - 1; ++index2)
      {
        stringBuilder.Append("..");
        stringBuilder.Append(ch2);
      }
      for (int index3 = index1; index3 < strArray1.Length; ++index3)
      {
        stringBuilder.Append(strArray1[index3]);
        if (index3 < strArray1.Length - 1)
          stringBuilder.Append(ch2);
      }
      return stringBuilder.ToString().ToLower(CultureInfo.CurrentUICulture);
    }

    internal static string MakeRelativeToDirectory(this string absolutePath, string relativeTo)
    {
      if (string.IsNullOrWhiteSpace(relativeTo))
        return absolutePath;
      if (absolutePath.Equals(relativeTo, StringComparison.OrdinalIgnoreCase))
        return string.Empty;
      relativeTo = relativeTo.EnsureEndSeparator();
      return new Uri(relativeTo).MakeRelativeUri(new Uri(absolutePath)).ToString().Replace("/", "\\");
    }

    internal static string NormalizeUrl(this string url)
    {
      url = url.Trim('\'', '"');
      if (url.StartsWith("hash(", StringComparison.OrdinalIgnoreCase) && url.EndsWith(")", StringComparison.OrdinalIgnoreCase))
        url = url.Substring(5, url.Length - 6);
      if (url.StartsWith("hash://", StringComparison.OrdinalIgnoreCase))
        url = url.Substring(7);
      return url.Replace('/', '\\').TrimStart('\\').ToLowerInvariant();
    }
  }
}
