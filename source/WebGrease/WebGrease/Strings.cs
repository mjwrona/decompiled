// Decompiled with JetBrains decompiler
// Type: WebGrease.Strings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Linq;

namespace WebGrease
{
  internal static class Strings
  {
    internal const string Css = "css";
    internal const string DpiResourcePivotKey = "dpi";
    internal const string ThemesResourcePivotKey = "themes";
    internal const string LocalesResourcePivotKey = "locales";
    internal const string CssFilter = "*.css";
    internal const string JsFilter = "*.js";
    internal const string JS = "js";
    internal const string Px = "px";
    internal const string ScanLogExtension = ".scan.xml";
    internal const string ResxExtension = ".resx";
    internal const string Semicolon = ";";
    internal const string DefaultLocale = "generic-generic";
    internal const string DefaultResx = "generic-generic.resx";
    internal const string GlobalsToIgnoreArg = "/global:";
    internal const string DefaultGlobalsToIgnore = "jQuery";
    internal const string DefaultMinifyArgs = "";
    internal const string DefaultAnalyzeArgs = "-analyze -WARN:4";
    internal const string CssLocalizedOutput = "CssLocalizedOutput";
    internal const string JsLocalizedOutput = "JsLocalizedOutput";
    internal const string ImagesLogFile = "images_log.xml";
    internal const string CssLogFile = "css_log.xml";
    internal const string JsLogFile = "js_log.xml";
    internal static readonly char[] FileFilterSeparator = ",".ToCharArray();
    internal static readonly List<string> DefaultImageExtensions = ((IEnumerable<string>) new string[4]
    {
      "png",
      "jpg",
      "jpeg",
      "gif"
    }).ToList<string>();
    private static readonly char[] SemicolonSeparatorField = new char[1]
    {
      ';'
    };

    internal static char[] SemicolonSeparator => Strings.SemicolonSeparatorField;
  }
}
