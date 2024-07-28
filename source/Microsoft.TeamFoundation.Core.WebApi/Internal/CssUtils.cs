// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.Internal.CssUtils
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;

namespace Microsoft.TeamFoundation.Core.WebApi.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CssUtils
  {
    public const int MaxCssNodeNameLength = 255;
    public const int MaxProjectLength = 64;
    internal const int MaxSyncRefName = 255;
    internal const int MaxSyncUniqueId = 255;
    private static readonly char[] illegalCssChars = new char[13]
    {
      '\\',
      '/',
      '$',
      '?',
      '*',
      ':',
      '"',
      '&',
      '<',
      '>',
      '#',
      '%',
      '|'
    };
    private static readonly string[] illegalCssNames = new string[25]
    {
      "prn",
      "com1",
      "com2",
      "com3",
      "com4",
      "com5",
      "com6",
      "com7",
      "com8",
      "com9",
      "com10",
      "lpt1",
      "lpt2",
      "lpt3",
      "lpt4",
      "lpt5",
      "lpt6",
      "lpt7",
      "lpt8",
      "lpt9",
      "nul",
      "con",
      "aux",
      ".",
      ".."
    };
    private static readonly string[] illegalProjectNames = new string[11]
    {
      "server",
      "bin",
      "App_code",
      "App_Browsers",
      "App_Data",
      "App_GlobalResources",
      "App_LocalResources",
      "App_Themes",
      "App_WebReferences",
      "web.config",
      "SignalR"
    };
    private static readonly char[] illegalProjectChars = new char[23]
    {
      '@',
      '~',
      ';',
      '{',
      '}',
      '\'',
      '+',
      '=',
      ',',
      '<',
      '>',
      '|',
      '/',
      '\\',
      '?',
      ':',
      '&',
      '$',
      '*',
      '"',
      '#',
      '[',
      ']'
    };

    public static bool IsReservedCssName(string name)
    {
      foreach (string illegalCssName in CssUtils.illegalCssNames)
      {
        if (TFStringComparer.CssNodeName.Equals(name, illegalCssName))
          return true;
      }
      return false;
    }

    public static bool IsReservedProjectName(string name, bool checkReservedCssNames)
    {
      foreach (string illegalProjectName in CssUtils.illegalProjectNames)
      {
        if (TFStringComparer.TeamProjectName.Equals(name, illegalProjectName))
          return true;
      }
      return checkReservedCssNames && CssUtils.IsReservedCssName(name);
    }

    public static bool HasInvalidCssCharacters(string name)
    {
      if (-1 != name.IndexOfAny(CssUtils.illegalCssChars))
        return true;
      for (int index = 0; index < name.Length; ++index)
      {
        if (char.IsControl(name, index))
          return true;
      }
      return -1 != name.IndexOfAny(Path.GetInvalidPathChars());
    }

    public static bool IsValidCssNodeName(string name) => name.Length <= (int) byte.MaxValue && !CssUtils.IsReservedCssName(name) && !CssUtils.HasInvalidCssCharacters(name);

    public static bool IsValidProjectName(string projectName)
    {
      if (projectName == null)
        return false;
      string str = projectName.Trim();
      int length = str.Length;
      if (length == 0 || length > 64)
        return false;
      switch (str[0])
      {
        case '.':
        case '_':
          return false;
        default:
          if (str[length - 1] == '.' || str.IndexOfAny(CssUtils.illegalProjectChars) != -1 || !CssUtils.IsValidCssNodeName(str) || CssUtils.IsReservedProjectName(str, false) || !CssUtils.TryCreateUri("http://www.cpandl.com/" + str, UriKind.Absolute, out Uri _))
            return false;
          bool isChineseOs = OSDetails.IsChineseOS;
          return !ArgumentUtility.IsInvalidString(str, false, isChineseOs) && (isChineseOs || !ArgumentUtility.HasSurrogates(str));
      }
    }

    private static bool TryCreateUri(string uriString, UriKind uriKind, out Uri result)
    {
      try
      {
        return Uri.TryCreate(uriString, uriKind, out result);
      }
      catch (IndexOutOfRangeException ex)
      {
        result = (Uri) null;
        return false;
      }
    }
  }
}
