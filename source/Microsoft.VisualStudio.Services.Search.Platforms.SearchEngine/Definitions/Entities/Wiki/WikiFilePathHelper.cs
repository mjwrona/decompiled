// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiFilePathHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public static class WikiFilePathHelper
  {
    private const string WikiRootPath = "/";
    private const string WikiPathSeparator = "/";

    public static string GetPageReadablePath(string pageFilePath, string wikiRootPath)
    {
      if (string.IsNullOrEmpty(pageFilePath))
        return (string) null;
      if (wikiRootPath.Equals(pageFilePath, StringComparison.Ordinal))
        return "/";
      if (wikiRootPath != "/" && pageFilePath != "/")
        pageFilePath = pageFilePath.Substring(wikiRootPath.Length);
      pageFilePath = pageFilePath.Replace(WikiFilePathHelper.SpecialChars.Hyphen, WikiFilePathHelper.SpecialChars.Space);
      for (int index = 0; index < WikiFilePathHelper.PathConstants.GitIllegalSpecialCharEcapes.Count; ++index)
        pageFilePath = pageFilePath.Replace(WikiFilePathHelper.PathConstants.GitIllegalSpecialCharEcapes[index], WikiFilePathHelper.PathConstants.GitIllegalSpecialChars[index]);
      return !pageFilePath.StartsWith(wikiRootPath, StringComparison.Ordinal) || wikiRootPath == "/" ? WikiFilePathHelper.NormalizePath(pageFilePath) : pageFilePath;
    }

    public static string NormalizePath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      path = path.Trim();
      path = path.Replace("\\", "/");
      if (!path.StartsWith("/", StringComparison.Ordinal))
        path = "/" + path;
      return path;
    }

    public static string GetFileNameWithoutExtension(string path)
    {
      path = WikiFilePathHelper.ComputeFileName(path);
      if (path == null)
        return (string) null;
      int length;
      return (length = path.LastIndexOf('.')) == -1 ? path : path.Substring(0, length);
    }

    internal static string ComputeFileName(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath))
        return filePath;
      int num1 = filePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
      if (num1 >= 0)
        return filePath.Substring(num1 + 1);
      int num2 = filePath.LastIndexOf("\\", StringComparison.OrdinalIgnoreCase);
      return num2 < 0 ? filePath : filePath.Substring(num2 + 1);
    }

    public static class SpecialChars
    {
      public static readonly string Ampersand = "&";
      public static readonly string Caret = "^";
      public static readonly string Colon = ":";
      public static readonly string Greaterthan = ">";
      public static readonly string Hash = "#";
      public static readonly string Hyphen = "-";
      public static readonly string Lessthan = "<";
      public static readonly string Percent = "%";
      public static readonly string Pipe = "|";
      public static readonly string Question = "?";
      public static readonly string Quote = "\"";
      public static readonly string Slash = "/";
      public static readonly string Space = " ";
      public static readonly string Star = "*";
      public static readonly string Tilde = "~";
    }

    public static class SpecialCharEncodings
    {
      public static readonly string Ampersand = "%26";
      public static readonly string Caret = "%5E";
      public static readonly string Colon = "%3A";
      public static readonly string Greaterthan = "%3E";
      public static readonly string Hash = "%23";
      public static readonly string Hyphen = "%2D";
      public static readonly string Lessthan = "%3C";
      public static readonly string Percent = "%25";
      public static readonly string Pipe = "%7C";
      public static readonly string Question = "%3F";
      public static readonly string Quote = "%22";
      public static readonly string Slash = "%2F";
      public static readonly string SingleQuote = "%27";
      public static readonly string Space = "%20";
      public static readonly string Star = "%2A";
      public static readonly string Tilde = "%7E";
    }

    public static class PathConstants
    {
      public static readonly IList<string> GitIllegalSpecialChars = (IList<string>) new string[8]
      {
        WikiFilePathHelper.SpecialChars.Colon,
        WikiFilePathHelper.SpecialChars.Question,
        WikiFilePathHelper.SpecialChars.Star,
        WikiFilePathHelper.SpecialChars.Lessthan,
        WikiFilePathHelper.SpecialChars.Greaterthan,
        WikiFilePathHelper.SpecialChars.Hyphen,
        WikiFilePathHelper.SpecialChars.Pipe,
        WikiFilePathHelper.SpecialChars.Quote
      };
      public static readonly IList<string> GitIllegalSpecialCharEcapes = (IList<string>) new string[8]
      {
        WikiFilePathHelper.SpecialCharEncodings.Colon,
        WikiFilePathHelper.SpecialCharEncodings.Question,
        WikiFilePathHelper.SpecialCharEncodings.Star,
        WikiFilePathHelper.SpecialCharEncodings.Lessthan,
        WikiFilePathHelper.SpecialCharEncodings.Greaterthan,
        WikiFilePathHelper.SpecialCharEncodings.Hyphen,
        WikiFilePathHelper.SpecialCharEncodings.Pipe,
        WikiFilePathHelper.SpecialCharEncodings.Quote
      };
    }
  }
}
