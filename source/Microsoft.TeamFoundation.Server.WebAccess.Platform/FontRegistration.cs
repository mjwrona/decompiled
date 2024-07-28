// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.FontRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class FontRegistration
  {
    public const string FontsFolder = "/_fonts";
    public const string FeatureFlag = "VisualStudio.Services.WebAccess.CustomFonts";
    public const string VssCssName = "@font-face-vss";
    public const string FabricCssName = "@font-face-fabric";
    private static IDictionary<string, string> s_fontFamilyCultureMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private const string c_fontBaseUrl = "../../../../3rdParty/_fonts/";

    static FontRegistration()
    {
      FontRegistration.s_fontFamilyCultureMap["ar"] = "Arabic";
      FontRegistration.s_fontFamilyCultureMap["bg"] = "Cyrillic";
      FontRegistration.s_fontFamilyCultureMap["cs"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["el"] = "Greek";
      FontRegistration.s_fontFamilyCultureMap["he"] = "Hebrew";
      FontRegistration.s_fontFamilyCultureMap["hr"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["hu"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["kk"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["lt"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["lv"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["pl"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["ru"] = "Cyrillic";
      FontRegistration.s_fontFamilyCultureMap["sk"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["sr-latn"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["th"] = "Thai";
      FontRegistration.s_fontFamilyCultureMap["tr"] = "EastEuropean";
      FontRegistration.s_fontFamilyCultureMap["uk"] = "Cyrillic";
      FontRegistration.s_fontFamilyCultureMap["vi"] = "Vietnamese";
    }

    private static string GetFontFamily(string cultureName)
    {
      if (!string.IsNullOrEmpty(cultureName))
      {
        KeyValuePair<string, string> keyValuePair = FontRegistration.s_fontFamilyCultureMap.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (f => cultureName.StartsWith(f.Key, StringComparison.OrdinalIgnoreCase)));
        if (!string.IsNullOrEmpty(keyValuePair.Value))
          return keyValuePair.Value;
      }
      return "WestEuropean";
    }

    private static void RegisterFontFace(
      StringBuilder entries,
      string fontName,
      FontSet fontSet,
      string fontBaseUrl,
      string fontFamily = "",
      string fontFolder = "TextFonts",
      string fontFilename = "segoeui",
      bool trueType = false)
    {
      List<FontFormat> fontFormatList = new List<FontFormat>()
      {
        FontFormat.Woff2,
        FontFormat.Woff
      };
      if (trueType)
        fontFormatList.Add(FontFormat.TrueType);
      string str = VirtualPathUtility.AppendTrailingSlash(fontBaseUrl) + fontFolder;
      List<string> values = new List<string>();
      foreach (FontFormat fontFormat in fontFormatList)
        values.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "url('{0}/{1}{2}{3}{4}') format('{5}')", (object) str, (object) (fontFamily + (!string.IsNullOrEmpty(fontFamily) ? "/" : string.Empty)), (object) fontFilename, fontSet.Name == "normal" ? (object) string.Empty : (object) ("-" + fontSet.Name), (object) fontFormat.Extension, (object) fontFormat.Name));
      entries.AppendLine("@font-face{");
      entries.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "  font-family:'{0}';", (object) fontName));
      entries.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "  src: {0};", (object) string.Join(",", (IEnumerable<string>) values)));
      entries.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "  font-weight:{0};", (object) fontSet.Weight));
      entries.AppendLine("  font-style:normal;");
      entries.AppendLine("}");
    }

    private static void RegisterFontSet(
      StringBuilder entries,
      string fontName,
      string fontBaseUrl,
      string fontFamily)
    {
      string fontFilename = "segoeui";
      FontRegistration.RegisterFontFace(entries, fontName, FontSet.Light, fontBaseUrl, fontFamily, fontFilename: fontFilename);
      FontRegistration.RegisterFontFace(entries, fontName, FontSet.SemiLight, fontBaseUrl, fontFamily, fontFilename: fontFilename);
      FontRegistration.RegisterFontFace(entries, fontName, FontSet.Regular, fontBaseUrl, fontFamily, fontFilename: fontFilename);
      FontRegistration.RegisterFontFace(entries, fontName, FontSet.SemiBold, fontBaseUrl, fontFamily, fontFilename: fontFilename);
    }

    internal static void RegisterFontFaces(
      StringBuilder entries,
      CultureInfo culture,
      string cssName,
      string fontBaseUrl)
    {
      if (string.Equals(cssName, "@font-face-vss", StringComparison.OrdinalIgnoreCase))
      {
        string fontFamily = FontRegistration.GetFontFamily(culture.Name);
        int num = fontFamily == "Thai" ? 1 : 0;
        string fontFilename = num != 0 ? "leelawadeeui" : "segoeui";
        FontSet fontSet = num != 0 ? new FontSet("semilight", "100") : FontSet.Light;
        if (num == 0)
        {
          FontRegistration.RegisterFontSet(entries, "Segoe UI VSS (Regular)", fontBaseUrl, fontFamily);
        }
        else
        {
          FontRegistration.RegisterFontFace(entries, "Segoe UI VSS (Regular)", new FontSet("semilight", "100"), fontBaseUrl, "Thai", fontFilename: fontFilename);
          FontRegistration.RegisterFontFace(entries, "Segoe UI VSS (Regular)", new FontSet("semilight", "300"), fontBaseUrl, "Thai", fontFilename: fontFilename);
          FontRegistration.RegisterFontFace(entries, "Segoe UI VSS (Regular)", new FontSet("regular", "400"), fontBaseUrl, "Thai", fontFilename: fontFilename);
          FontRegistration.RegisterFontFace(entries, "Segoe UI VSS (Regular)", new FontSet("bold", "600"), fontBaseUrl, "Thai", fontFilename: fontFilename);
        }
        FontRegistration.RegisterFontFace(entries, "Segoe UI VSS (Light)", fontSet, fontBaseUrl, fontFamily, fontFilename: fontFilename);
        FontRegistration.RegisterFontFace(entries, "Segoe UI VSS (Semibold)", FontSet.SemiBold, fontBaseUrl, fontFamily, fontFilename: fontFilename);
      }
      else
      {
        if (!string.Equals(cssName, "@font-face-fabric", StringComparison.OrdinalIgnoreCase))
          return;
        FontRegistration.RegisterFontSet(entries, "Segoe UI Web (Arabic)", fontBaseUrl, "Arabic");
        FontRegistration.RegisterFontSet(entries, "Segoe UI Web (Cyrillic)", fontBaseUrl, "Cyrillic");
        FontRegistration.RegisterFontSet(entries, "Segoe UI Web (East European)", fontBaseUrl, "EastEuropean");
        FontRegistration.RegisterFontSet(entries, "Segoe UI Web (Greek)", fontBaseUrl, "Greek");
        FontRegistration.RegisterFontSet(entries, "Segoe UI Web (Vietnamese)", fontBaseUrl, "Vietnamese");
        FontRegistration.RegisterFontSet(entries, "Segoe UI Web (West European)", fontBaseUrl, "WestEuropean");
        string fontFilename = "leelawadeeui";
        FontRegistration.RegisterFontFace(entries, "Leelawadee UI Web", new FontSet("semilight", "100"), fontBaseUrl, "Thai", fontFilename: fontFilename);
        FontRegistration.RegisterFontFace(entries, "Leelawadee UI Web", new FontSet("semilight", "300"), fontBaseUrl, "Thai", fontFilename: fontFilename);
        FontRegistration.RegisterFontFace(entries, "Leelawadee UI Web", new FontSet("regular", "400"), fontBaseUrl, "Thai", fontFilename: fontFilename);
        FontRegistration.RegisterFontFace(entries, "Leelawadee UI Web", new FontSet("bold", "600"), fontBaseUrl, "Thai", fontFilename: fontFilename);
      }
    }

    internal static string GetRegisteredFontFaces(
      IVssRequestContext requestContext,
      string cssName,
      CultureInfo culture = null,
      string fontBaseUrl = null)
    {
      StringBuilder entries = new StringBuilder();
      if (FontRegistration.IsFontRegistration(cssName))
      {
        if (culture == null)
          culture = CultureInfo.CurrentUICulture;
        if (fontBaseUrl == null)
          fontBaseUrl = "../../../../3rdParty/_fonts/";
        FontRegistration.RegisterFontFaces(entries, culture, cssName, fontBaseUrl);
      }
      return entries.ToString();
    }

    public static bool IsFontRegistration(string cssName) => !string.IsNullOrEmpty(cssName) && cssName.StartsWith("@font-face-", StringComparison.OrdinalIgnoreCase);

    public static string GetCssUrl(WebContext webContext, string cssName) => webContext.Url.RouteUrl((object) new
    {
      controller = "Font",
      action = "Register",
      routeArea = "Public",
      serviceHost = webContext.TfsRequestContext.ServiceHost,
      project = string.Empty,
      team = string.Empty
    }) + "?locale=" + webContext.Globalization.Culture + "&css=" + cssName;

    internal static bool IsCultureSpecific(string cssName) => string.Equals(cssName, "@font-face-vss", StringComparison.OrdinalIgnoreCase);

    public static void RegisterFonts(RequestContext requestContext)
    {
      if (!requestContext.TfsRequestContext().IsFeatureEnabled("VisualStudio.Services.WebAccess.CustomFonts"))
        return;
      GeneralHtmlExtensions.UseCommonScriptModules(requestContext.HttpContext, "VSS/Fonts/TextFonts", "VSS/Fonts/IconFonts");
    }

    public static void RegisterFonts(this HtmlHelper htmlHelper) => FontRegistration.RegisterFonts(htmlHelper.ViewContext.RequestContext);
  }
}
