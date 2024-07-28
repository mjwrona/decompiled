// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ThemesUtility
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ThemesUtility
  {
    private const string BowtiePreviewEnableCookie = "TFS-BOWTIE-PREVIEW";
    public static readonly string DefaultThemeName = "Default";
    public static readonly string HighContrastThemeName = "HighContrast";

    public static ThemeModel[] GetThemes(HttpContextBase context) => ThemesUtility.GetThemes(StaticResources.Versioned.Themes.GetPhysicalLocation(string.Empty), context);

    public static ThemeModel[] GetThemes(string appThemesDir, HttpContextBase context)
    {
      if (!Directory.Exists(appThemesDir))
        return Array.Empty<ThemeModel>();
      Dictionary<string, ThemeModel> supportedThemes = ThemesUtility.GetSupportedThemes(context);
      IEnumerable<string> source = ((IEnumerable<string>) Directory.GetDirectories(appThemesDir)).Select<string, string>((Func<string, string>) (d => Path.GetFileName(d)));
      List<ThemeModel> themeModelList = new List<ThemeModel>();
      themeModelList.AddRange(source.Where<string>((Func<string, bool>) (tn => supportedThemes.Keys.Contains<string>(tn, (IEqualityComparer<string>) StringComparer.InvariantCulture))).OrderBy<string, string>((Func<string, string>) (tn => tn)).Select<string, ThemeModel>((Func<string, ThemeModel>) (tn => supportedThemes[tn])));
      if (source.Any<string>((Func<string, bool>) (d => string.Equals(d, ThemesUtility.DefaultThemeName))))
        themeModelList.Insert(0, ThemesUtility.DefaultTheme);
      return themeModelList.ToArray();
    }

    public static bool IsThemeNameValid(string themeName, HttpContextBase requestContext) => ThemesUtility.GetThemeFromName(themeName, requestContext) != null;

    public static ThemeModel GetThemeFromName(string themeName, HttpContextBase requestContext) => ThemesUtility.DefaultTheme;

    public static ThemeModel DefaultTheme => new ThemeModel()
    {
      DisplayName = WACommonResources.DefaultThemeName,
      ThemeName = ThemesUtility.DefaultThemeName
    };

    private static Dictionary<string, ThemeModel> GetSupportedThemes(HttpContextBase context) => new Dictionary<string, ThemeModel>();
  }
}
