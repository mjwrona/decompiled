// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.RoosterJsResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class RoosterJsResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (RoosterJsResources), typeof (RoosterJsResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => RoosterJsResources.s_resMgr;

    private static string Get(string resourceName) => RoosterJsResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? RoosterJsResources.Get(resourceName) : RoosterJsResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) RoosterJsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? RoosterJsResources.GetInt(resourceName) : (int) RoosterJsResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) RoosterJsResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? RoosterJsResources.GetBool(resourceName) : (bool) RoosterJsResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => RoosterJsResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = RoosterJsResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string Color_Black() => RoosterJsResources.Get(nameof (Color_Black));

    public static string Color_Black(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Black), culture);

    public static string Color_Blue() => RoosterJsResources.Get(nameof (Color_Blue));

    public static string Color_Blue(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Blue), culture);

    public static string Color_Cyan() => RoosterJsResources.Get(nameof (Color_Cyan));

    public static string Color_Cyan(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Cyan), culture);

    public static string Color_DarkBlue() => RoosterJsResources.Get(nameof (Color_DarkBlue));

    public static string Color_DarkBlue(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkBlue), culture);

    public static string Color_DarkerBlue() => RoosterJsResources.Get(nameof (Color_DarkerBlue));

    public static string Color_DarkerBlue(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkerBlue), culture);

    public static string Color_DarkerGray() => RoosterJsResources.Get(nameof (Color_DarkerGray));

    public static string Color_DarkerGray(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkerGray), culture);

    public static string Color_DarkerGreen() => RoosterJsResources.Get(nameof (Color_DarkerGreen));

    public static string Color_DarkerGreen(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkerGreen), culture);

    public static string Color_DarkerOrange() => RoosterJsResources.Get(nameof (Color_DarkerOrange));

    public static string Color_DarkerOrange(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkerOrange), culture);

    public static string Color_DarkerPurple() => RoosterJsResources.Get(nameof (Color_DarkerPurple));

    public static string Color_DarkerPurple(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkerPurple), culture);

    public static string Color_DarkerRed() => RoosterJsResources.Get(nameof (Color_DarkerRed));

    public static string Color_DarkerRed(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkerRed), culture);

    public static string Color_DarkerYellow() => RoosterJsResources.Get(nameof (Color_DarkerYellow));

    public static string Color_DarkerYellow(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkerYellow), culture);

    public static string Color_DarkGray() => RoosterJsResources.Get(nameof (Color_DarkGray));

    public static string Color_DarkGray(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkGray), culture);

    public static string Color_DarkGreen() => RoosterJsResources.Get(nameof (Color_DarkGreen));

    public static string Color_DarkGreen(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkGreen), culture);

    public static string Color_DarkOrange() => RoosterJsResources.Get(nameof (Color_DarkOrange));

    public static string Color_DarkOrange(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkOrange), culture);

    public static string Color_DarkPurple() => RoosterJsResources.Get(nameof (Color_DarkPurple));

    public static string Color_DarkPurple(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkPurple), culture);

    public static string Color_DarkRed() => RoosterJsResources.Get(nameof (Color_DarkRed));

    public static string Color_DarkRed(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkRed), culture);

    public static string Color_DarkYellow() => RoosterJsResources.Get(nameof (Color_DarkYellow));

    public static string Color_DarkYellow(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_DarkYellow), culture);

    public static string Color_Gray() => RoosterJsResources.Get(nameof (Color_Gray));

    public static string Color_Gray(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Gray), culture);

    public static string Color_Green() => RoosterJsResources.Get(nameof (Color_Green));

    public static string Color_Green(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Green), culture);

    public static string Color_LightBlue() => RoosterJsResources.Get(nameof (Color_LightBlue));

    public static string Color_LightBlue(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightBlue), culture);

    public static string Color_LightCyan() => RoosterJsResources.Get(nameof (Color_LightCyan));

    public static string Color_LightCyan(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightCyan), culture);

    public static string Color_LightGray() => RoosterJsResources.Get(nameof (Color_LightGray));

    public static string Color_LightGray(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightGray), culture);

    public static string Color_LightGreen() => RoosterJsResources.Get(nameof (Color_LightGreen));

    public static string Color_LightGreen(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightGreen), culture);

    public static string Color_LightMagenta() => RoosterJsResources.Get(nameof (Color_LightMagenta));

    public static string Color_LightMagenta(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightMagenta), culture);

    public static string Color_LightOrange() => RoosterJsResources.Get(nameof (Color_LightOrange));

    public static string Color_LightOrange(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightOrange), culture);

    public static string Color_LightPurple() => RoosterJsResources.Get(nameof (Color_LightPurple));

    public static string Color_LightPurple(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightPurple), culture);

    public static string Color_LightRed() => RoosterJsResources.Get(nameof (Color_LightRed));

    public static string Color_LightRed(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightRed), culture);

    public static string Color_LightYellow() => RoosterJsResources.Get(nameof (Color_LightYellow));

    public static string Color_LightYellow(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_LightYellow), culture);

    public static string Color_Magenta() => RoosterJsResources.Get(nameof (Color_Magenta));

    public static string Color_Magenta(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Magenta), culture);

    public static string Color_Orange() => RoosterJsResources.Get(nameof (Color_Orange));

    public static string Color_Orange(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Orange), culture);

    public static string Color_Purple() => RoosterJsResources.Get(nameof (Color_Purple));

    public static string Color_Purple(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Purple), culture);

    public static string Color_Red() => RoosterJsResources.Get(nameof (Color_Red));

    public static string Color_Red(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Red), culture);

    public static string Color_White() => RoosterJsResources.Get(nameof (Color_White));

    public static string Color_White(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_White), culture);

    public static string Color_Yellow() => RoosterJsResources.Get(nameof (Color_Yellow));

    public static string Color_Yellow(CultureInfo culture) => RoosterJsResources.Get(nameof (Color_Yellow), culture);

    public static string Toolbar_Bold() => RoosterJsResources.Get(nameof (Toolbar_Bold));

    public static string Toolbar_Bold(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Bold), culture);

    public static string Toolbar_BulletedList() => RoosterJsResources.Get(nameof (Toolbar_BulletedList));

    public static string Toolbar_BulletedList(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_BulletedList), culture);

    public static string Toolbar_ClearFormat() => RoosterJsResources.Get(nameof (Toolbar_ClearFormat));

    public static string Toolbar_ClearFormat(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_ClearFormat), culture);

    public static string Toolbar_Emoji() => RoosterJsResources.Get(nameof (Toolbar_Emoji));

    public static string Toolbar_Emoji(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Emoji), culture);

    public static string Toolbar_FontColor() => RoosterJsResources.Get(nameof (Toolbar_FontColor));

    public static string Toolbar_FontColor(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_FontColor), culture);

    public static string Toolbar_Header() => RoosterJsResources.Get(nameof (Toolbar_Header));

    public static string Toolbar_Header(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Header), culture);

    public static string Toolbar_HeaderN(object arg0) => RoosterJsResources.Format(nameof (Toolbar_HeaderN), arg0);

    public static string Toolbar_HeaderN(object arg0, CultureInfo culture) => RoosterJsResources.Format(nameof (Toolbar_HeaderN), culture, arg0);

    public static string Toolbar_Highlight() => RoosterJsResources.Get(nameof (Toolbar_Highlight));

    public static string Toolbar_Highlight(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Highlight), culture);

    public static string Toolbar_Indent() => RoosterJsResources.Get(nameof (Toolbar_Indent));

    public static string Toolbar_Indent(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Indent), culture);

    public static string Toolbar_InsertImage() => RoosterJsResources.Get(nameof (Toolbar_InsertImage));

    public static string Toolbar_InsertImage(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_InsertImage), culture);

    public static string Toolbar_Italic() => RoosterJsResources.Get(nameof (Toolbar_Italic));

    public static string Toolbar_Italic(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Italic), culture);

    public static string Toolbar_Link() => RoosterJsResources.Get(nameof (Toolbar_Link));

    public static string Toolbar_Link(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Link), culture);

    public static string Toolbar_NumberedList() => RoosterJsResources.Get(nameof (Toolbar_NumberedList));

    public static string Toolbar_NumberedList(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_NumberedList), culture);

    public static string Toolbar_Outdent() => RoosterJsResources.Get(nameof (Toolbar_Outdent));

    public static string Toolbar_Outdent(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Outdent), culture);

    public static string Toolbar_Strikethrough() => RoosterJsResources.Get(nameof (Toolbar_Strikethrough));

    public static string Toolbar_Strikethrough(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Strikethrough), culture);

    public static string Toolbar_Underline() => RoosterJsResources.Get(nameof (Toolbar_Underline));

    public static string Toolbar_Underline(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Underline), culture);

    public static string Toolbar_Unlink() => RoosterJsResources.Get(nameof (Toolbar_Unlink));

    public static string Toolbar_Unlink(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Unlink), culture);

    public static string Toolbar_Code() => RoosterJsResources.Get(nameof (Toolbar_Code));

    public static string Toolbar_Code(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_Code), culture);

    public static string SearchForEmoji() => RoosterJsResources.Get(nameof (SearchForEmoji));

    public static string SearchForEmoji(CultureInfo culture) => RoosterJsResources.Get(nameof (SearchForEmoji), culture);

    public static string FontColorPicker_AriaLabel() => RoosterJsResources.Get(nameof (FontColorPicker_AriaLabel));

    public static string FontColorPicker_AriaLabel(CultureInfo culture) => RoosterJsResources.Get(nameof (FontColorPicker_AriaLabel), culture);

    public static string HeaderMenu_AriaLabel() => RoosterJsResources.Get(nameof (HeaderMenu_AriaLabel));

    public static string HeaderMenu_AriaLabel(CultureInfo culture) => RoosterJsResources.Get(nameof (HeaderMenu_AriaLabel), culture);

    public static string HighlightColorPicker_AriaLabel() => RoosterJsResources.Get(nameof (HighlightColorPicker_AriaLabel));

    public static string HighlightColorPicker_AriaLabel(CultureInfo culture) => RoosterJsResources.Get(nameof (HighlightColorPicker_AriaLabel), culture);

    public static string Toolbar_More() => RoosterJsResources.Get(nameof (Toolbar_More));

    public static string Toolbar_More(CultureInfo culture) => RoosterJsResources.Get(nameof (Toolbar_More), culture);

    public static string InsertLink_FieldLabel() => RoosterJsResources.Get(nameof (InsertLink_FieldLabel));

    public static string InsertLink_FieldLabel(CultureInfo culture) => RoosterJsResources.Get(nameof (InsertLink_FieldLabel), culture);

    public static string ImageDefaultAltText() => RoosterJsResources.Get(nameof (ImageDefaultAltText));

    public static string ImageDefaultAltText(CultureInfo culture) => RoosterJsResources.Get(nameof (ImageDefaultAltText), culture);
  }
}
