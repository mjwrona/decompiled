// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.RoosterJsEmojiResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public static class RoosterJsEmojiResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (RoosterJsEmojiResources), typeof (RoosterJsEmojiResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => RoosterJsEmojiResources.s_resMgr;

    private static string Get(string resourceName) => RoosterJsEmojiResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? RoosterJsEmojiResources.Get(resourceName) : RoosterJsEmojiResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) RoosterJsEmojiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? RoosterJsEmojiResources.GetInt(resourceName) : (int) RoosterJsEmojiResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) RoosterJsEmojiResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? RoosterJsEmojiResources.GetBool(resourceName) : (bool) RoosterJsEmojiResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => RoosterJsEmojiResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = RoosterJsEmojiResources.Get(resourceName, culture);
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

    public static string EmojiSearchTags_02601() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02601));

    public static string EmojiSearchTags_02601(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02601), culture);

    public static string EmojiSearchTags_02614() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02614));

    public static string EmojiSearchTags_02614(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02614), culture);

    public static string EmojiSearchTags_02615() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02615));

    public static string EmojiSearchTags_02615(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02615), culture);

    public static string EmojiSearchTags_0261d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0261d));

    public static string EmojiSearchTags_0261d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0261d), culture);

    public static string EmojiSearchTags_02648() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02648));

    public static string EmojiSearchTags_02648(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02648), culture);

    public static string EmojiSearchTags_02649() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02649));

    public static string EmojiSearchTags_02649(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02649), culture);

    public static string EmojiSearchTags_0264a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264a));

    public static string EmojiSearchTags_0264a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264a), culture);

    public static string EmojiSearchTags_0264b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264b));

    public static string EmojiSearchTags_0264b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264b), culture);

    public static string EmojiSearchTags_0264c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264c));

    public static string EmojiSearchTags_0264c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264c), culture);

    public static string EmojiSearchTags_0264d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264d));

    public static string EmojiSearchTags_0264d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264d), culture);

    public static string EmojiSearchTags_0264e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264e));

    public static string EmojiSearchTags_0264e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264e), culture);

    public static string EmojiSearchTags_0264f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264f));

    public static string EmojiSearchTags_0264f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0264f), culture);

    public static string EmojiSearchTags_02650() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02650));

    public static string EmojiSearchTags_02650(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02650), culture);

    public static string EmojiSearchTags_02651() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02651));

    public static string EmojiSearchTags_02651(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02651), culture);

    public static string EmojiSearchTags_02652() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02652));

    public static string EmojiSearchTags_02652(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02652), culture);

    public static string EmojiSearchTags_02653() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02653));

    public static string EmojiSearchTags_02653(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02653), culture);

    public static string EmojiSearchTags_02668() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02668));

    public static string EmojiSearchTags_02668(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02668), culture);

    public static string EmojiSearchTags_0267f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0267f));

    public static string EmojiSearchTags_0267f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0267f), culture);

    public static string EmojiSearchTags_02693() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02693));

    public static string EmojiSearchTags_02693(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02693), culture);

    public static string EmojiSearchTags_026a0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026a0));

    public static string EmojiSearchTags_026a0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026a0), culture);

    public static string EmojiSearchTags_026a1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026a1));

    public static string EmojiSearchTags_026a1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026a1), culture);

    public static string EmojiSearchTags_026bd() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026bd));

    public static string EmojiSearchTags_026bd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026bd), culture);

    public static string EmojiSearchTags_026be() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026be));

    public static string EmojiSearchTags_026be(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026be), culture);

    public static string EmojiSearchTags_026c4() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026c4));

    public static string EmojiSearchTags_026c4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026c4), culture);

    public static string EmojiSearchTags_026ea() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026ea));

    public static string EmojiSearchTags_026ea(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026ea), culture);

    public static string EmojiSearchTags_026f2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026f2));

    public static string EmojiSearchTags_026f2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026f2), culture);

    public static string EmojiSearchTags_026f3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026f3));

    public static string EmojiSearchTags_026f3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026f3), culture);

    public static string EmojiSearchTags_026f5() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026f5));

    public static string EmojiSearchTags_026f5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026f5), culture);

    public static string EmojiSearchTags_026fa() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026fa));

    public static string EmojiSearchTags_026fa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026fa), culture);

    public static string EmojiSearchTags_026fd() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026fd));

    public static string EmojiSearchTags_026fd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_026fd), culture);

    public static string EmojiSearchTags_02702() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02702));

    public static string EmojiSearchTags_02702(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02702), culture);

    public static string EmojiSearchTags_02708() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02708));

    public static string EmojiSearchTags_02708(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02708), culture);

    public static string EmojiSearchTags_02709() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02709));

    public static string EmojiSearchTags_02709(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02709), culture);

    public static string EmojiSearchTags_0270a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270a));

    public static string EmojiSearchTags_0270a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270a), culture);

    public static string EmojiSearchTags_0270b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270b));

    public static string EmojiSearchTags_0270b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270b), culture);

    public static string EmojiSearchTags_0270c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270c));

    public static string EmojiSearchTags_0270c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270c), culture);

    public static string EmojiSearchTags_0270f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270f));

    public static string EmojiSearchTags_0270f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_0270f), culture);

    public static string EmojiSearchTags_02728() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02728));

    public static string EmojiSearchTags_02728(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02728), culture);

    public static string EmojiSearchTags_02733() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02733));

    public static string EmojiSearchTags_02733(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02733), culture);

    public static string EmojiSearchTags_02734() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02734));

    public static string EmojiSearchTags_02734(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02734), culture);

    public static string EmojiSearchTags_02744() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02744));

    public static string EmojiSearchTags_02744(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02744), culture);

    public static string EmojiSearchTags_02747() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02747));

    public static string EmojiSearchTags_02747(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02747), culture);

    public static string EmojiSearchTags_02764() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02764));

    public static string EmojiSearchTags_02764(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02764), culture);

    public static string EmojiSearchTags_02b50() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02b50));

    public static string EmojiSearchTags_02b50(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_02b50), culture);

    public static string EmojiSearchTags_1f0cf() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f0cf));

    public static string EmojiSearchTags_1f0cf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f0cf), culture);

    public static string EmojiSearchTags_1f17f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f17f));

    public static string EmojiSearchTags_1f17f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f17f), culture);

    public static string EmojiSearchTags_1f300() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f300));

    public static string EmojiSearchTags_1f300(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f300), culture);

    public static string EmojiSearchTags_1f301() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f301));

    public static string EmojiSearchTags_1f301(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f301), culture);

    public static string EmojiSearchTags_1f302() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f302));

    public static string EmojiSearchTags_1f302(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f302), culture);

    public static string EmojiSearchTags_1f303() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f303));

    public static string EmojiSearchTags_1f303(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f303), culture);

    public static string EmojiSearchTags_1f304() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f304));

    public static string EmojiSearchTags_1f304(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f304), culture);

    public static string EmojiSearchTags_1f305() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f305));

    public static string EmojiSearchTags_1f305(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f305), culture);

    public static string EmojiSearchTags_1f306() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f306));

    public static string EmojiSearchTags_1f306(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f306), culture);

    public static string EmojiSearchTags_1f307() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f307));

    public static string EmojiSearchTags_1f307(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f307), culture);

    public static string EmojiSearchTags_1f308() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f308));

    public static string EmojiSearchTags_1f308(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f308), culture);

    public static string EmojiSearchTags_1f309() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f309));

    public static string EmojiSearchTags_1f309(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f309), culture);

    public static string EmojiSearchTags_1f30a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f30a));

    public static string EmojiSearchTags_1f30a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f30a), culture);

    public static string EmojiSearchTags_1f30c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f30c));

    public static string EmojiSearchTags_1f30c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f30c), culture);

    public static string EmojiSearchTags_1f311() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f311));

    public static string EmojiSearchTags_1f311(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f311), culture);

    public static string EmojiSearchTags_1f313() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f313));

    public static string EmojiSearchTags_1f313(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f313), culture);

    public static string EmojiSearchTags_1f314() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f314));

    public static string EmojiSearchTags_1f314(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f314), culture);

    public static string EmojiSearchTags_1f315() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f315));

    public static string EmojiSearchTags_1f315(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f315), culture);

    public static string EmojiSearchTags_1f319() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f319));

    public static string EmojiSearchTags_1f319(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f319), culture);

    public static string EmojiSearchTags_1f31b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f31b));

    public static string EmojiSearchTags_1f31b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f31b), culture);

    public static string EmojiSearchTags_1f31f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f31f));

    public static string EmojiSearchTags_1f31f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f31f), culture);

    public static string EmojiSearchTags_1f320() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f320));

    public static string EmojiSearchTags_1f320(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f320), culture);

    public static string EmojiSearchTags_1f330() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f330));

    public static string EmojiSearchTags_1f330(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f330), culture);

    public static string EmojiSearchTags_1f331() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f331));

    public static string EmojiSearchTags_1f331(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f331), culture);

    public static string EmojiSearchTags_1f334() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f334));

    public static string EmojiSearchTags_1f334(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f334), culture);

    public static string EmojiSearchTags_1f335() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f335));

    public static string EmojiSearchTags_1f335(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f335), culture);

    public static string EmojiSearchTags_1f337() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f337));

    public static string EmojiSearchTags_1f337(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f337), culture);

    public static string EmojiSearchTags_1f338() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f338));

    public static string EmojiSearchTags_1f338(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f338), culture);

    public static string EmojiSearchTags_1f339() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f339));

    public static string EmojiSearchTags_1f339(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f339), culture);

    public static string EmojiSearchTags_1f33a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33a));

    public static string EmojiSearchTags_1f33a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33a), culture);

    public static string EmojiSearchTags_1f33b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33b));

    public static string EmojiSearchTags_1f33b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33b), culture);

    public static string EmojiSearchTags_1f33c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33c));

    public static string EmojiSearchTags_1f33c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33c), culture);

    public static string EmojiSearchTags_1f33d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33d));

    public static string EmojiSearchTags_1f33d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33d), culture);

    public static string EmojiSearchTags_1f33e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33e));

    public static string EmojiSearchTags_1f33e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33e), culture);

    public static string EmojiSearchTags_1f33f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33f));

    public static string EmojiSearchTags_1f33f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f33f), culture);

    public static string EmojiSearchTags_1f340() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f340));

    public static string EmojiSearchTags_1f340(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f340), culture);

    public static string EmojiSearchTags_1f341() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f341));

    public static string EmojiSearchTags_1f341(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f341), culture);

    public static string EmojiSearchTags_1f342() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f342));

    public static string EmojiSearchTags_1f342(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f342), culture);

    public static string EmojiSearchTags_1f343() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f343));

    public static string EmojiSearchTags_1f343(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f343), culture);

    public static string EmojiSearchTags_1f344() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f344));

    public static string EmojiSearchTags_1f344(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f344), culture);

    public static string EmojiSearchTags_1f345() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f345));

    public static string EmojiSearchTags_1f345(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f345), culture);

    public static string EmojiSearchTags_1f346() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f346));

    public static string EmojiSearchTags_1f346(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f346), culture);

    public static string EmojiSearchTags_1f347() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f347));

    public static string EmojiSearchTags_1f347(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f347), culture);

    public static string EmojiSearchTags_1f348() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f348));

    public static string EmojiSearchTags_1f348(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f348), culture);

    public static string EmojiSearchTags_1f349() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f349));

    public static string EmojiSearchTags_1f349(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f349), culture);

    public static string EmojiSearchTags_1f34a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34a));

    public static string EmojiSearchTags_1f34a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34a), culture);

    public static string EmojiSearchTags_1f34c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34c));

    public static string EmojiSearchTags_1f34c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34c), culture);

    public static string EmojiSearchTags_1f34d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34d));

    public static string EmojiSearchTags_1f34d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34d), culture);

    public static string EmojiSearchTags_1f34e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34e));

    public static string EmojiSearchTags_1f34e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34e), culture);

    public static string EmojiSearchTags_1f34f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34f));

    public static string EmojiSearchTags_1f34f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f34f), culture);

    public static string EmojiSearchTags_1f351() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f351));

    public static string EmojiSearchTags_1f351(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f351), culture);

    public static string EmojiSearchTags_1f352() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f352));

    public static string EmojiSearchTags_1f352(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f352), culture);

    public static string EmojiSearchTags_1f353() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f353));

    public static string EmojiSearchTags_1f353(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f353), culture);

    public static string EmojiSearchTags_1f354() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f354));

    public static string EmojiSearchTags_1f354(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f354), culture);

    public static string EmojiSearchTags_1f355() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f355));

    public static string EmojiSearchTags_1f355(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f355), culture);

    public static string EmojiSearchTags_1f356() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f356));

    public static string EmojiSearchTags_1f356(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f356), culture);

    public static string EmojiSearchTags_1f357() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f357));

    public static string EmojiSearchTags_1f357(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f357), culture);

    public static string EmojiSearchTags_1f358() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f358));

    public static string EmojiSearchTags_1f358(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f358), culture);

    public static string EmojiSearchTags_1f359() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f359));

    public static string EmojiSearchTags_1f359(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f359), culture);

    public static string EmojiSearchTags_1f35a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35a));

    public static string EmojiSearchTags_1f35a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35a), culture);

    public static string EmojiSearchTags_1f35b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35b));

    public static string EmojiSearchTags_1f35b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35b), culture);

    public static string EmojiSearchTags_1f35c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35c));

    public static string EmojiSearchTags_1f35c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35c), culture);

    public static string EmojiSearchTags_1f35d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35d));

    public static string EmojiSearchTags_1f35d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35d), culture);

    public static string EmojiSearchTags_1f35e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35e));

    public static string EmojiSearchTags_1f35e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35e), culture);

    public static string EmojiSearchTags_1f35f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35f));

    public static string EmojiSearchTags_1f35f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f35f), culture);

    public static string EmojiSearchTags_1f360() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f360));

    public static string EmojiSearchTags_1f360(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f360), culture);

    public static string EmojiSearchTags_1f361() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f361));

    public static string EmojiSearchTags_1f361(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f361), culture);

    public static string EmojiSearchTags_1f362() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f362));

    public static string EmojiSearchTags_1f362(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f362), culture);

    public static string EmojiSearchTags_1f363() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f363));

    public static string EmojiSearchTags_1f363(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f363), culture);

    public static string EmojiSearchTags_1f364() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f364));

    public static string EmojiSearchTags_1f364(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f364), culture);

    public static string EmojiSearchTags_1f365() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f365));

    public static string EmojiSearchTags_1f365(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f365), culture);

    public static string EmojiSearchTags_1f366() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f366));

    public static string EmojiSearchTags_1f366(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f366), culture);

    public static string EmojiSearchTags_1f368() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f368));

    public static string EmojiSearchTags_1f368(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f368), culture);

    public static string EmojiSearchTags_1f369() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f369));

    public static string EmojiSearchTags_1f369(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f369), culture);

    public static string EmojiSearchTags_1f36a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36a));

    public static string EmojiSearchTags_1f36a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36a), culture);

    public static string EmojiSearchTags_1f36b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36b));

    public static string EmojiSearchTags_1f36b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36b), culture);

    public static string EmojiSearchTags_1f36c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36c));

    public static string EmojiSearchTags_1f36c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36c), culture);

    public static string EmojiSearchTags_1f36d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36d));

    public static string EmojiSearchTags_1f36d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36d), culture);

    public static string EmojiSearchTags_1f36e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36e));

    public static string EmojiSearchTags_1f36e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36e), culture);

    public static string EmojiSearchTags_1f36f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36f));

    public static string EmojiSearchTags_1f36f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f36f), culture);

    public static string EmojiSearchTags_1f370() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f370));

    public static string EmojiSearchTags_1f370(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f370), culture);

    public static string EmojiSearchTags_1f371() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f371));

    public static string EmojiSearchTags_1f371(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f371), culture);

    public static string EmojiSearchTags_1f372() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f372));

    public static string EmojiSearchTags_1f372(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f372), culture);

    public static string EmojiSearchTags_1f373() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f373));

    public static string EmojiSearchTags_1f373(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f373), culture);

    public static string EmojiSearchTags_1f374() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f374));

    public static string EmojiSearchTags_1f374(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f374), culture);

    public static string EmojiSearchTags_1f375() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f375));

    public static string EmojiSearchTags_1f375(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f375), culture);

    public static string EmojiSearchTags_1f376() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f376));

    public static string EmojiSearchTags_1f376(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f376), culture);

    public static string EmojiSearchTags_1f377() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f377));

    public static string EmojiSearchTags_1f377(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f377), culture);

    public static string EmojiSearchTags_1f378() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f378));

    public static string EmojiSearchTags_1f378(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f378), culture);

    public static string EmojiSearchTags_1f379() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f379));

    public static string EmojiSearchTags_1f379(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f379), culture);

    public static string EmojiSearchTags_1f37a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f37a));

    public static string EmojiSearchTags_1f37a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f37a), culture);

    public static string EmojiSearchTags_1f37b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f37b));

    public static string EmojiSearchTags_1f37b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f37b), culture);

    public static string EmojiSearchTags_1f380() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f380));

    public static string EmojiSearchTags_1f380(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f380), culture);

    public static string EmojiSearchTags_1f381() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f381));

    public static string EmojiSearchTags_1f381(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f381), culture);

    public static string EmojiSearchTags_1f382() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f382));

    public static string EmojiSearchTags_1f382(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f382), culture);

    public static string EmojiSearchTags_1f383() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f383));

    public static string EmojiSearchTags_1f383(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f383), culture);

    public static string EmojiSearchTags_1f384() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f384));

    public static string EmojiSearchTags_1f384(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f384), culture);

    public static string EmojiSearchTags_1f385() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f385));

    public static string EmojiSearchTags_1f385(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f385), culture);

    public static string EmojiSearchTags_1f386() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f386));

    public static string EmojiSearchTags_1f386(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f386), culture);

    public static string EmojiSearchTags_1f387() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f387));

    public static string EmojiSearchTags_1f387(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f387), culture);

    public static string EmojiSearchTags_1f388() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f388));

    public static string EmojiSearchTags_1f388(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f388), culture);

    public static string EmojiSearchTags_1f389() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f389));

    public static string EmojiSearchTags_1f389(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f389), culture);

    public static string EmojiSearchTags_1f38a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f38a));

    public static string EmojiSearchTags_1f38a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f38a), culture);

    public static string EmojiSearchTags_1f390() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f390));

    public static string EmojiSearchTags_1f390(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f390), culture);

    public static string EmojiSearchTags_1f392() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f392));

    public static string EmojiSearchTags_1f392(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f392), culture);

    public static string EmojiSearchTags_1f393() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f393));

    public static string EmojiSearchTags_1f393(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f393), culture);

    public static string EmojiSearchTags_1f3a0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a0));

    public static string EmojiSearchTags_1f3a0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a0), culture);

    public static string EmojiSearchTags_1f3a1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a1));

    public static string EmojiSearchTags_1f3a1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a1), culture);

    public static string EmojiSearchTags_1f3a2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a2));

    public static string EmojiSearchTags_1f3a2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a2), culture);

    public static string EmojiSearchTags_1f3a3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a3));

    public static string EmojiSearchTags_1f3a3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a3), culture);

    public static string EmojiSearchTags_1f3a4() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a4));

    public static string EmojiSearchTags_1f3a4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a4), culture);

    public static string EmojiSearchTags_1f3a5() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a5));

    public static string EmojiSearchTags_1f3a5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a5), culture);

    public static string EmojiSearchTags_1f3a6() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a6));

    public static string EmojiSearchTags_1f3a6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a6), culture);

    public static string EmojiSearchTags_1f3a7() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a7));

    public static string EmojiSearchTags_1f3a7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a7), culture);

    public static string EmojiSearchTags_1f3a8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a8));

    public static string EmojiSearchTags_1f3a8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a8), culture);

    public static string EmojiSearchTags_1f3a9() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a9));

    public static string EmojiSearchTags_1f3a9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3a9), culture);

    public static string EmojiSearchTags_1f3aa() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3aa));

    public static string EmojiSearchTags_1f3aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3aa), culture);

    public static string EmojiSearchTags_1f3ab() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ab));

    public static string EmojiSearchTags_1f3ab(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ab), culture);

    public static string EmojiSearchTags_1f3ac() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ac));

    public static string EmojiSearchTags_1f3ac(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ac), culture);

    public static string EmojiSearchTags_1f3ad() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ad));

    public static string EmojiSearchTags_1f3ad(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ad), culture);

    public static string EmojiSearchTags_1f3ae() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ae));

    public static string EmojiSearchTags_1f3ae(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ae), culture);

    public static string EmojiSearchTags_1f3af() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3af));

    public static string EmojiSearchTags_1f3af(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3af), culture);

    public static string EmojiSearchTags_1f3b0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b0));

    public static string EmojiSearchTags_1f3b0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b0), culture);

    public static string EmojiSearchTags_1f3b1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b1));

    public static string EmojiSearchTags_1f3b1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b1), culture);

    public static string EmojiSearchTags_1f3b2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b2));

    public static string EmojiSearchTags_1f3b2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b2), culture);

    public static string EmojiSearchTags_1f3b3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b3));

    public static string EmojiSearchTags_1f3b3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b3), culture);

    public static string EmojiSearchTags_1f3b5() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b5));

    public static string EmojiSearchTags_1f3b5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b5), culture);

    public static string EmojiSearchTags_1f3b6() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b6));

    public static string EmojiSearchTags_1f3b6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b6), culture);

    public static string EmojiSearchTags_1f3b7() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b7));

    public static string EmojiSearchTags_1f3b7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b7), culture);

    public static string EmojiSearchTags_1f3b8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b8));

    public static string EmojiSearchTags_1f3b8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b8), culture);

    public static string EmojiSearchTags_1f3b9() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b9));

    public static string EmojiSearchTags_1f3b9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3b9), culture);

    public static string EmojiSearchTags_1f3ba() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ba));

    public static string EmojiSearchTags_1f3ba(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ba), culture);

    public static string EmojiSearchTags_1f3bb() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bb));

    public static string EmojiSearchTags_1f3bb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bb), culture);

    public static string EmojiSearchTags_1f3bc() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bc));

    public static string EmojiSearchTags_1f3bc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bc), culture);

    public static string EmojiSearchTags_1f3bd() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bd));

    public static string EmojiSearchTags_1f3bd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bd), culture);

    public static string EmojiSearchTags_1f3be() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3be));

    public static string EmojiSearchTags_1f3be(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3be), culture);

    public static string EmojiSearchTags_1f3bf() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bf));

    public static string EmojiSearchTags_1f3bf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3bf), culture);

    public static string EmojiSearchTags_1f3c0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c0));

    public static string EmojiSearchTags_1f3c0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c0), culture);

    public static string EmojiSearchTags_1f3c1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c1));

    public static string EmojiSearchTags_1f3c1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c1), culture);

    public static string EmojiSearchTags_1f3c2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c2));

    public static string EmojiSearchTags_1f3c2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c2), culture);

    public static string EmojiSearchTags_1f3c3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c3));

    public static string EmojiSearchTags_1f3c3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c3), culture);

    public static string EmojiSearchTags_1f3c4() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c4));

    public static string EmojiSearchTags_1f3c4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c4), culture);

    public static string EmojiSearchTags_1f3c6() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c6));

    public static string EmojiSearchTags_1f3c6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c6), culture);

    public static string EmojiSearchTags_1f3c8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c8));

    public static string EmojiSearchTags_1f3c8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3c8), culture);

    public static string EmojiSearchTags_1f3ca() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ca));

    public static string EmojiSearchTags_1f3ca(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ca), culture);

    public static string EmojiSearchTags_1f3e0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e0));

    public static string EmojiSearchTags_1f3e0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e0), culture);

    public static string EmojiSearchTags_1f3e1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e1));

    public static string EmojiSearchTags_1f3e1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e1), culture);

    public static string EmojiSearchTags_1f3e2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e2));

    public static string EmojiSearchTags_1f3e2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e2), culture);

    public static string EmojiSearchTags_1f3e6() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e6));

    public static string EmojiSearchTags_1f3e6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e6), culture);

    public static string EmojiSearchTags_1f3e7() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e7));

    public static string EmojiSearchTags_1f3e7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e7), culture);

    public static string EmojiSearchTags_1f3e8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e8));

    public static string EmojiSearchTags_1f3e8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3e8), culture);

    public static string EmojiSearchTags_1f3ea() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ea));

    public static string EmojiSearchTags_1f3ea(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ea), culture);

    public static string EmojiSearchTags_1f3eb() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3eb));

    public static string EmojiSearchTags_1f3eb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3eb), culture);

    public static string EmojiSearchTags_1f3ec() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ec));

    public static string EmojiSearchTags_1f3ec(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ec), culture);

    public static string EmojiSearchTags_1f3ed() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ed));

    public static string EmojiSearchTags_1f3ed(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3ed), culture);

    public static string EmojiSearchTags_1f3f0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3f0));

    public static string EmojiSearchTags_1f3f0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f3f0), culture);

    public static string EmojiSearchTags_1f40c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f40c));

    public static string EmojiSearchTags_1f40c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f40c), culture);

    public static string EmojiSearchTags_1f40d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f40d));

    public static string EmojiSearchTags_1f40d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f40d), culture);

    public static string EmojiSearchTags_1f40e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f40e));

    public static string EmojiSearchTags_1f40e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f40e), culture);

    public static string EmojiSearchTags_1f411() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f411));

    public static string EmojiSearchTags_1f411(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f411), culture);

    public static string EmojiSearchTags_1f412() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f412));

    public static string EmojiSearchTags_1f412(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f412), culture);

    public static string EmojiSearchTags_1f414() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f414));

    public static string EmojiSearchTags_1f414(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f414), culture);

    public static string EmojiSearchTags_1f417() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f417));

    public static string EmojiSearchTags_1f417(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f417), culture);

    public static string EmojiSearchTags_1f418() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f418));

    public static string EmojiSearchTags_1f418(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f418), culture);

    public static string EmojiSearchTags_1f419() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f419));

    public static string EmojiSearchTags_1f419(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f419), culture);

    public static string EmojiSearchTags_1f41a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41a));

    public static string EmojiSearchTags_1f41a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41a), culture);

    public static string EmojiSearchTags_1f41b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41b));

    public static string EmojiSearchTags_1f41b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41b), culture);

    public static string EmojiSearchTags_1f41c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41c));

    public static string EmojiSearchTags_1f41c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41c), culture);

    public static string EmojiSearchTags_1f41d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41d));

    public static string EmojiSearchTags_1f41d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41d), culture);

    public static string EmojiSearchTags_1f41e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41e));

    public static string EmojiSearchTags_1f41e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41e), culture);

    public static string EmojiSearchTags_1f41f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41f));

    public static string EmojiSearchTags_1f41f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f41f), culture);

    public static string EmojiSearchTags_1f420() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f420));

    public static string EmojiSearchTags_1f420(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f420), culture);

    public static string EmojiSearchTags_1f421() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f421));

    public static string EmojiSearchTags_1f421(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f421), culture);

    public static string EmojiSearchTags_1f422() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f422));

    public static string EmojiSearchTags_1f422(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f422), culture);

    public static string EmojiSearchTags_1f423() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f423));

    public static string EmojiSearchTags_1f423(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f423), culture);

    public static string EmojiSearchTags_1f425() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f425));

    public static string EmojiSearchTags_1f425(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f425), culture);

    public static string EmojiSearchTags_1f426() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f426));

    public static string EmojiSearchTags_1f426(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f426), culture);

    public static string EmojiSearchTags_1f427() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f427));

    public static string EmojiSearchTags_1f427(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f427), culture);

    public static string EmojiSearchTags_1f429() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f429));

    public static string EmojiSearchTags_1f429(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f429), culture);

    public static string EmojiSearchTags_1f42b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42b));

    public static string EmojiSearchTags_1f42b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42b), culture);

    public static string EmojiSearchTags_1f42c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42c));

    public static string EmojiSearchTags_1f42c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42c), culture);

    public static string EmojiSearchTags_1f42d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42d));

    public static string EmojiSearchTags_1f42d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42d), culture);

    public static string EmojiSearchTags_1f42e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42e));

    public static string EmojiSearchTags_1f42e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42e), culture);

    public static string EmojiSearchTags_1f42f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42f));

    public static string EmojiSearchTags_1f42f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f42f), culture);

    public static string EmojiSearchTags_1f430() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f430));

    public static string EmojiSearchTags_1f430(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f430), culture);

    public static string EmojiSearchTags_1f431() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f431));

    public static string EmojiSearchTags_1f431(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f431), culture);

    public static string EmojiSearchTags_1f432() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f432));

    public static string EmojiSearchTags_1f432(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f432), culture);

    public static string EmojiSearchTags_1f433() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f433));

    public static string EmojiSearchTags_1f433(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f433), culture);

    public static string EmojiSearchTags_1f434() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f434));

    public static string EmojiSearchTags_1f434(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f434), culture);

    public static string EmojiSearchTags_1f435() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f435));

    public static string EmojiSearchTags_1f435(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f435), culture);

    public static string EmojiSearchTags_1f436() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f436));

    public static string EmojiSearchTags_1f436(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f436), culture);

    public static string EmojiSearchTags_1f437() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f437));

    public static string EmojiSearchTags_1f437(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f437), culture);

    public static string EmojiSearchTags_1f438() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f438));

    public static string EmojiSearchTags_1f438(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f438), culture);

    public static string EmojiSearchTags_1f439() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f439));

    public static string EmojiSearchTags_1f439(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f439), culture);

    public static string EmojiSearchTags_1f43a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43a));

    public static string EmojiSearchTags_1f43a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43a), culture);

    public static string EmojiSearchTags_1f43b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43b));

    public static string EmojiSearchTags_1f43b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43b), culture);

    public static string EmojiSearchTags_1f43c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43c));

    public static string EmojiSearchTags_1f43c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43c), culture);

    public static string EmojiSearchTags_1f43d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43d));

    public static string EmojiSearchTags_1f43d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43d), culture);

    public static string EmojiSearchTags_1f43e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43e));

    public static string EmojiSearchTags_1f43e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f43e), culture);

    public static string EmojiSearchTags_1f440() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f440));

    public static string EmojiSearchTags_1f440(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f440), culture);

    public static string EmojiSearchTags_1f442() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f442));

    public static string EmojiSearchTags_1f442(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f442), culture);

    public static string EmojiSearchTags_1f443() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f443));

    public static string EmojiSearchTags_1f443(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f443), culture);

    public static string EmojiSearchTags_1f444() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f444));

    public static string EmojiSearchTags_1f444(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f444), culture);

    public static string EmojiSearchTags_1f445() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f445));

    public static string EmojiSearchTags_1f445(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f445), culture);

    public static string EmojiSearchTags_1f446() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f446));

    public static string EmojiSearchTags_1f446(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f446), culture);

    public static string EmojiSearchTags_1f447() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f447));

    public static string EmojiSearchTags_1f447(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f447), culture);

    public static string EmojiSearchTags_1f448() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f448));

    public static string EmojiSearchTags_1f448(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f448), culture);

    public static string EmojiSearchTags_1f449() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f449));

    public static string EmojiSearchTags_1f449(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f449), culture);

    public static string EmojiSearchTags_1f44a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44a));

    public static string EmojiSearchTags_1f44a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44a), culture);

    public static string EmojiSearchTags_1f44b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44b));

    public static string EmojiSearchTags_1f44b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44b), culture);

    public static string EmojiSearchTags_1f44c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44c));

    public static string EmojiSearchTags_1f44c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44c), culture);

    public static string EmojiSearchTags_1f44d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44d));

    public static string EmojiSearchTags_1f44d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44d), culture);

    public static string EmojiSearchTags_1f44e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44e));

    public static string EmojiSearchTags_1f44e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44e), culture);

    public static string EmojiSearchTags_1f44f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44f));

    public static string EmojiSearchTags_1f44f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f44f), culture);

    public static string EmojiSearchTags_1f451() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f451));

    public static string EmojiSearchTags_1f451(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f451), culture);

    public static string EmojiSearchTags_1f452() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f452));

    public static string EmojiSearchTags_1f452(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f452), culture);

    public static string EmojiSearchTags_1f453() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f453));

    public static string EmojiSearchTags_1f453(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f453), culture);

    public static string EmojiSearchTags_1f454() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f454));

    public static string EmojiSearchTags_1f454(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f454), culture);

    public static string EmojiSearchTags_1f455() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f455));

    public static string EmojiSearchTags_1f455(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f455), culture);

    public static string EmojiSearchTags_1f456() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f456));

    public static string EmojiSearchTags_1f456(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f456), culture);

    public static string EmojiSearchTags_1f457() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f457));

    public static string EmojiSearchTags_1f457(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f457), culture);

    public static string EmojiSearchTags_1f458() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f458));

    public static string EmojiSearchTags_1f458(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f458), culture);

    public static string EmojiSearchTags_1f459() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f459));

    public static string EmojiSearchTags_1f459(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f459), culture);

    public static string EmojiSearchTags_1f45a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45a));

    public static string EmojiSearchTags_1f45a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45a), culture);

    public static string EmojiSearchTags_1f45b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45b));

    public static string EmojiSearchTags_1f45b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45b), culture);

    public static string EmojiSearchTags_1f45c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45c));

    public static string EmojiSearchTags_1f45c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45c), culture);

    public static string EmojiSearchTags_1f45d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45d));

    public static string EmojiSearchTags_1f45d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45d), culture);

    public static string EmojiSearchTags_1f45e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45e));

    public static string EmojiSearchTags_1f45e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45e), culture);

    public static string EmojiSearchTags_1f45f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45f));

    public static string EmojiSearchTags_1f45f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f45f), culture);

    public static string EmojiSearchTags_1f461() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f461));

    public static string EmojiSearchTags_1f461(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f461), culture);

    public static string EmojiSearchTags_1f462() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f462));

    public static string EmojiSearchTags_1f462(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f462), culture);

    public static string EmojiSearchTags_1f463() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f463));

    public static string EmojiSearchTags_1f463(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f463), culture);

    public static string EmojiSearchTags_1f466() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f466));

    public static string EmojiSearchTags_1f466(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f466), culture);

    public static string EmojiSearchTags_1f467() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f467));

    public static string EmojiSearchTags_1f467(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f467), culture);

    public static string EmojiSearchTags_1f468() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f468));

    public static string EmojiSearchTags_1f468(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f468), culture);

    public static string EmojiSearchTags_1f469() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f469));

    public static string EmojiSearchTags_1f469(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f469), culture);

    public static string EmojiSearchTags_1f46a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46a));

    public static string EmojiSearchTags_1f46a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46a), culture);

    public static string EmojiSearchTags_1f46b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46b));

    public static string EmojiSearchTags_1f46b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46b), culture);

    public static string EmojiSearchTags_1f46e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46e));

    public static string EmojiSearchTags_1f46e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46e), culture);

    public static string EmojiSearchTags_1f46f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46f));

    public static string EmojiSearchTags_1f46f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f46f), culture);

    public static string EmojiSearchTags_1f470() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f470));

    public static string EmojiSearchTags_1f470(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f470), culture);

    public static string EmojiSearchTags_1f471() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f471));

    public static string EmojiSearchTags_1f471(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f471), culture);

    public static string EmojiSearchTags_1f474() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f474));

    public static string EmojiSearchTags_1f474(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f474), culture);

    public static string EmojiSearchTags_1f475() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f475));

    public static string EmojiSearchTags_1f475(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f475), culture);

    public static string EmojiSearchTags_1f476() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f476));

    public static string EmojiSearchTags_1f476(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f476), culture);

    public static string EmojiSearchTags_1f477() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f477));

    public static string EmojiSearchTags_1f477(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f477), culture);

    public static string EmojiSearchTags_1f478() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f478));

    public static string EmojiSearchTags_1f478(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f478), culture);

    public static string EmojiSearchTags_1f47b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47b));

    public static string EmojiSearchTags_1f47b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47b), culture);

    public static string EmojiSearchTags_1f47c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47c));

    public static string EmojiSearchTags_1f47c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47c), culture);

    public static string EmojiSearchTags_1f47d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47d));

    public static string EmojiSearchTags_1f47d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47d), culture);

    public static string EmojiSearchTags_1f47e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47e));

    public static string EmojiSearchTags_1f47e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47e), culture);

    public static string EmojiSearchTags_1f47f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47f));

    public static string EmojiSearchTags_1f47f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f47f), culture);

    public static string EmojiSearchTags_1f480() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f480));

    public static string EmojiSearchTags_1f480(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f480), culture);

    public static string EmojiSearchTags_1f481() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f481));

    public static string EmojiSearchTags_1f481(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f481), culture);

    public static string EmojiSearchTags_1f482() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f482));

    public static string EmojiSearchTags_1f482(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f482), culture);

    public static string EmojiSearchTags_1f483() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f483));

    public static string EmojiSearchTags_1f483(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f483), culture);

    public static string EmojiSearchTags_1f484() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f484));

    public static string EmojiSearchTags_1f484(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f484), culture);

    public static string EmojiSearchTags_1f485() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f485));

    public static string EmojiSearchTags_1f485(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f485), culture);

    public static string EmojiSearchTags_1f486() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f486));

    public static string EmojiSearchTags_1f486(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f486), culture);

    public static string EmojiSearchTags_1f487() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f487));

    public static string EmojiSearchTags_1f487(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f487), culture);

    public static string EmojiSearchTags_1f488() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f488));

    public static string EmojiSearchTags_1f488(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f488), culture);

    public static string EmojiSearchTags_1f489() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f489));

    public static string EmojiSearchTags_1f489(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f489), culture);

    public static string EmojiSearchTags_1f48a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48a));

    public static string EmojiSearchTags_1f48a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48a), culture);

    public static string EmojiSearchTags_1f48b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48b));

    public static string EmojiSearchTags_1f48b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48b), culture);

    public static string EmojiSearchTags_1f48c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48c));

    public static string EmojiSearchTags_1f48c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48c), culture);

    public static string EmojiSearchTags_1f48d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48d));

    public static string EmojiSearchTags_1f48d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48d), culture);

    public static string EmojiSearchTags_1f48e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48e));

    public static string EmojiSearchTags_1f48e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48e), culture);

    public static string EmojiSearchTags_1f48f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48f));

    public static string EmojiSearchTags_1f48f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f48f), culture);

    public static string EmojiSearchTags_1f490() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f490));

    public static string EmojiSearchTags_1f490(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f490), culture);

    public static string EmojiSearchTags_1f491() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f491));

    public static string EmojiSearchTags_1f491(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f491), culture);

    public static string EmojiSearchTags_1f492() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f492));

    public static string EmojiSearchTags_1f492(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f492), culture);

    public static string EmojiSearchTags_1f493() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f493));

    public static string EmojiSearchTags_1f493(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f493), culture);

    public static string EmojiSearchTags_1f494() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f494));

    public static string EmojiSearchTags_1f494(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f494), culture);

    public static string EmojiSearchTags_1f495() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f495));

    public static string EmojiSearchTags_1f495(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f495), culture);

    public static string EmojiSearchTags_1f496() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f496));

    public static string EmojiSearchTags_1f496(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f496), culture);

    public static string EmojiSearchTags_1f497() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f497));

    public static string EmojiSearchTags_1f497(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f497), culture);

    public static string EmojiSearchTags_1f498() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f498));

    public static string EmojiSearchTags_1f498(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f498), culture);

    public static string EmojiSearchTags_1f499() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f499));

    public static string EmojiSearchTags_1f499(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f499), culture);

    public static string EmojiSearchTags_1f49a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49a));

    public static string EmojiSearchTags_1f49a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49a), culture);

    public static string EmojiSearchTags_1f49b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49b));

    public static string EmojiSearchTags_1f49b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49b), culture);

    public static string EmojiSearchTags_1f49c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49c));

    public static string EmojiSearchTags_1f49c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49c), culture);

    public static string EmojiSearchTags_1f49d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49d));

    public static string EmojiSearchTags_1f49d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49d), culture);

    public static string EmojiSearchTags_1f49e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49e));

    public static string EmojiSearchTags_1f49e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49e), culture);

    public static string EmojiSearchTags_1f49f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49f));

    public static string EmojiSearchTags_1f49f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f49f), culture);

    public static string EmojiSearchTags_1f4a1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a1));

    public static string EmojiSearchTags_1f4a1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a1), culture);

    public static string EmojiSearchTags_1f4a3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a3));

    public static string EmojiSearchTags_1f4a3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a3), culture);

    public static string EmojiSearchTags_1f4a4() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a4));

    public static string EmojiSearchTags_1f4a4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a4), culture);

    public static string EmojiSearchTags_1f4a5() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a5));

    public static string EmojiSearchTags_1f4a5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a5), culture);

    public static string EmojiSearchTags_1f4a8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a8));

    public static string EmojiSearchTags_1f4a8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a8), culture);

    public static string EmojiSearchTags_1f4a9() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a9));

    public static string EmojiSearchTags_1f4a9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4a9), culture);

    public static string EmojiSearchTags_1f4aa() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4aa));

    public static string EmojiSearchTags_1f4aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4aa), culture);

    public static string EmojiSearchTags_1f4b0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b0));

    public static string EmojiSearchTags_1f4b0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b0), culture);

    public static string EmojiSearchTags_1f4b3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b3));

    public static string EmojiSearchTags_1f4b3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b3), culture);

    public static string EmojiSearchTags_1f4b4() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b4));

    public static string EmojiSearchTags_1f4b4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b4), culture);

    public static string EmojiSearchTags_1f4b5() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b5));

    public static string EmojiSearchTags_1f4b5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4b5), culture);

    public static string EmojiSearchTags_1f4ba() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ba));

    public static string EmojiSearchTags_1f4ba(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ba), culture);

    public static string EmojiSearchTags_1f4bb() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4bb));

    public static string EmojiSearchTags_1f4bb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4bb), culture);

    public static string EmojiSearchTags_1f4bc() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4bc));

    public static string EmojiSearchTags_1f4bc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4bc), culture);

    public static string EmojiSearchTags_1f4be() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4be));

    public static string EmojiSearchTags_1f4be(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4be), culture);

    public static string EmojiSearchTags_1f4bf() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4bf));

    public static string EmojiSearchTags_1f4bf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4bf), culture);

    public static string EmojiSearchTags_1f4c0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c0));

    public static string EmojiSearchTags_1f4c0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c0), culture);

    public static string EmojiSearchTags_1f4c1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c1));

    public static string EmojiSearchTags_1f4c1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c1), culture);

    public static string EmojiSearchTags_1f4c5() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c5));

    public static string EmojiSearchTags_1f4c5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c5), culture);

    public static string EmojiSearchTags_1f4c8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c8));

    public static string EmojiSearchTags_1f4c8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c8), culture);

    public static string EmojiSearchTags_1f4c9() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c9));

    public static string EmojiSearchTags_1f4c9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4c9), culture);

    public static string EmojiSearchTags_1f4ca() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ca));

    public static string EmojiSearchTags_1f4ca(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ca), culture);

    public static string EmojiSearchTags_1f4cb() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cb));

    public static string EmojiSearchTags_1f4cb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cb), culture);

    public static string EmojiSearchTags_1f4cc() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cc));

    public static string EmojiSearchTags_1f4cc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cc), culture);

    public static string EmojiSearchTags_1f4cd() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cd));

    public static string EmojiSearchTags_1f4cd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cd), culture);

    public static string EmojiSearchTags_1f4ce() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ce));

    public static string EmojiSearchTags_1f4ce(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ce), culture);

    public static string EmojiSearchTags_1f4cf() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cf));

    public static string EmojiSearchTags_1f4cf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4cf), culture);

    public static string EmojiSearchTags_1f4d2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4d2));

    public static string EmojiSearchTags_1f4d2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4d2), culture);

    public static string EmojiSearchTags_1f4d3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4d3));

    public static string EmojiSearchTags_1f4d3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4d3), culture);

    public static string EmojiSearchTags_1f4da() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4da));

    public static string EmojiSearchTags_1f4da(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4da), culture);

    public static string EmojiSearchTags_1f4db() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4db));

    public static string EmojiSearchTags_1f4db(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4db), culture);

    public static string EmojiSearchTags_1f4dd() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4dd));

    public static string EmojiSearchTags_1f4dd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4dd), culture);

    public static string EmojiSearchTags_1f4de() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4de));

    public static string EmojiSearchTags_1f4de(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4de), culture);

    public static string EmojiSearchTags_1f4df() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4df));

    public static string EmojiSearchTags_1f4df(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4df), culture);

    public static string EmojiSearchTags_1f4e0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e0));

    public static string EmojiSearchTags_1f4e0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e0), culture);

    public static string EmojiSearchTags_1f4e1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e1));

    public static string EmojiSearchTags_1f4e1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e1), culture);

    public static string EmojiSearchTags_1f4e3() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e3));

    public static string EmojiSearchTags_1f4e3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e3), culture);

    public static string EmojiSearchTags_1f4e6() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e6));

    public static string EmojiSearchTags_1f4e6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e6), culture);

    public static string EmojiSearchTags_1f4e7() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e7));

    public static string EmojiSearchTags_1f4e7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e7), culture);

    public static string EmojiSearchTags_1f4e8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e8));

    public static string EmojiSearchTags_1f4e8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e8), culture);

    public static string EmojiSearchTags_1f4e9() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e9));

    public static string EmojiSearchTags_1f4e9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4e9), culture);

    public static string EmojiSearchTags_1f4ea() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ea));

    public static string EmojiSearchTags_1f4ea(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ea), culture);

    public static string EmojiSearchTags_1f4eb() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4eb));

    public static string EmojiSearchTags_1f4eb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4eb), culture);

    public static string EmojiSearchTags_1f4ee() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ee));

    public static string EmojiSearchTags_1f4ee(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4ee), culture);

    public static string EmojiSearchTags_1f4f0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f0));

    public static string EmojiSearchTags_1f4f0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f0), culture);

    public static string EmojiSearchTags_1f4f1() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f1));

    public static string EmojiSearchTags_1f4f1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f1), culture);

    public static string EmojiSearchTags_1f4f6() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f6));

    public static string EmojiSearchTags_1f4f6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f6), culture);

    public static string EmojiSearchTags_1f4f7() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f7));

    public static string EmojiSearchTags_1f4f7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f7), culture);

    public static string EmojiSearchTags_1f4f9() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f9));

    public static string EmojiSearchTags_1f4f9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4f9), culture);

    public static string EmojiSearchTags_1f4fa() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4fa));

    public static string EmojiSearchTags_1f4fa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4fa), culture);

    public static string EmojiSearchTags_1f4fb() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4fb));

    public static string EmojiSearchTags_1f4fb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4fb), culture);

    public static string EmojiSearchTags_1f4fc() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4fc));

    public static string EmojiSearchTags_1f4fc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f4fc), culture);

    public static string EmojiSearchTags_1f503() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f503));

    public static string EmojiSearchTags_1f503(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f503), culture);

    public static string EmojiSearchTags_1f50b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50b));

    public static string EmojiSearchTags_1f50b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50b), culture);

    public static string EmojiSearchTags_1f50c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50c));

    public static string EmojiSearchTags_1f50c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50c), culture);

    public static string EmojiSearchTags_1f50d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50d));

    public static string EmojiSearchTags_1f50d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50d), culture);

    public static string EmojiSearchTags_1f50f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50f));

    public static string EmojiSearchTags_1f50f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f50f), culture);

    public static string EmojiSearchTags_1f510() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f510));

    public static string EmojiSearchTags_1f510(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f510), culture);

    public static string EmojiSearchTags_1f511() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f511));

    public static string EmojiSearchTags_1f511(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f511), culture);

    public static string EmojiSearchTags_1f512() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f512));

    public static string EmojiSearchTags_1f512(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f512), culture);

    public static string EmojiSearchTags_1f513() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f513));

    public static string EmojiSearchTags_1f513(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f513), culture);

    public static string EmojiSearchTags_1f514() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f514));

    public static string EmojiSearchTags_1f514(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f514), culture);

    public static string EmojiSearchTags_1f516() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f516));

    public static string EmojiSearchTags_1f516(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f516), culture);

    public static string EmojiSearchTags_1f525() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f525));

    public static string EmojiSearchTags_1f525(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f525), culture);

    public static string EmojiSearchTags_1f526() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f526));

    public static string EmojiSearchTags_1f526(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f526), culture);

    public static string EmojiSearchTags_1f527() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f527));

    public static string EmojiSearchTags_1f527(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f527), culture);

    public static string EmojiSearchTags_1f528() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f528));

    public static string EmojiSearchTags_1f528(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f528), culture);

    public static string EmojiSearchTags_1f529() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f529));

    public static string EmojiSearchTags_1f529(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f529), culture);

    public static string EmojiSearchTags_1f52a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f52a));

    public static string EmojiSearchTags_1f52a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f52a), culture);

    public static string EmojiSearchTags_1f52e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f52e));

    public static string EmojiSearchTags_1f52e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f52e), culture);

    public static string EmojiSearchTags_1f531() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f531));

    public static string EmojiSearchTags_1f531(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f531), culture);

    public static string EmojiSearchTags_1f550() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f550));

    public static string EmojiSearchTags_1f550(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f550), culture);

    public static string EmojiSearchTags_1f551() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f551));

    public static string EmojiSearchTags_1f551(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f551), culture);

    public static string EmojiSearchTags_1f552() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f552));

    public static string EmojiSearchTags_1f552(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f552), culture);

    public static string EmojiSearchTags_1f553() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f553));

    public static string EmojiSearchTags_1f553(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f553), culture);

    public static string EmojiSearchTags_1f554() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f554));

    public static string EmojiSearchTags_1f554(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f554), culture);

    public static string EmojiSearchTags_1f555() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f555));

    public static string EmojiSearchTags_1f555(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f555), culture);

    public static string EmojiSearchTags_1f556() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f556));

    public static string EmojiSearchTags_1f556(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f556), culture);

    public static string EmojiSearchTags_1f557() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f557));

    public static string EmojiSearchTags_1f557(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f557), culture);

    public static string EmojiSearchTags_1f558() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f558));

    public static string EmojiSearchTags_1f558(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f558), culture);

    public static string EmojiSearchTags_1f559() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f559));

    public static string EmojiSearchTags_1f559(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f559), culture);

    public static string EmojiSearchTags_1f55a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f55a));

    public static string EmojiSearchTags_1f55a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f55a), culture);

    public static string EmojiSearchTags_1f55b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f55b));

    public static string EmojiSearchTags_1f55b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f55b), culture);

    public static string EmojiSearchTags_1f5fd() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f5fd));

    public static string EmojiSearchTags_1f5fd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f5fd), culture);

    public static string EmojiSearchTags_1f601() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f601));

    public static string EmojiSearchTags_1f601(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f601), culture);

    public static string EmojiSearchTags_1f602() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f602));

    public static string EmojiSearchTags_1f602(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f602), culture);

    public static string EmojiSearchTags_1f603() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f603));

    public static string EmojiSearchTags_1f603(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f603), culture);

    public static string EmojiSearchTags_1f604() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f604));

    public static string EmojiSearchTags_1f604(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f604), culture);

    public static string EmojiSearchTags_1f605() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f605));

    public static string EmojiSearchTags_1f605(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f605), culture);

    public static string EmojiSearchTags_1f606() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f606));

    public static string EmojiSearchTags_1f606(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f606), culture);

    public static string EmojiSearchTags_1f607() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f607));

    public static string EmojiSearchTags_1f607(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f607), culture);

    public static string EmojiSearchTags_1f609() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f609));

    public static string EmojiSearchTags_1f609(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f609), culture);

    public static string EmojiSearchTags_1f60a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60a));

    public static string EmojiSearchTags_1f60a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60a), culture);

    public static string EmojiSearchTags_1f60b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60b));

    public static string EmojiSearchTags_1f60b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60b), culture);

    public static string EmojiSearchTags_1f60c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60c));

    public static string EmojiSearchTags_1f60c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60c), culture);

    public static string EmojiSearchTags_1f60d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60d));

    public static string EmojiSearchTags_1f60d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60d), culture);

    public static string EmojiSearchTags_1f60e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60e));

    public static string EmojiSearchTags_1f60e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60e), culture);

    public static string EmojiSearchTags_1f60f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60f));

    public static string EmojiSearchTags_1f60f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f60f), culture);

    public static string EmojiSearchTags_1f610() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f610));

    public static string EmojiSearchTags_1f610(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f610), culture);

    public static string EmojiSearchTags_1f612() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f612));

    public static string EmojiSearchTags_1f612(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f612), culture);

    public static string EmojiSearchTags_1f613() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f613));

    public static string EmojiSearchTags_1f613(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f613), culture);

    public static string EmojiSearchTags_1f614() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f614));

    public static string EmojiSearchTags_1f614(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f614), culture);

    public static string EmojiSearchTags_1f616() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f616));

    public static string EmojiSearchTags_1f616(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f616), culture);

    public static string EmojiSearchTags_1f618() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f618));

    public static string EmojiSearchTags_1f618(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f618), culture);

    public static string EmojiSearchTags_1f61a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61a));

    public static string EmojiSearchTags_1f61a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61a), culture);

    public static string EmojiSearchTags_1f61c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61c));

    public static string EmojiSearchTags_1f61c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61c), culture);

    public static string EmojiSearchTags_1f61d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61d));

    public static string EmojiSearchTags_1f61d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61d), culture);

    public static string EmojiSearchTags_1f61e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61e));

    public static string EmojiSearchTags_1f61e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f61e), culture);

    public static string EmojiSearchTags_1f620() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f620));

    public static string EmojiSearchTags_1f620(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f620), culture);

    public static string EmojiSearchTags_1f621() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f621));

    public static string EmojiSearchTags_1f621(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f621), culture);

    public static string EmojiSearchTags_1f622() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f622));

    public static string EmojiSearchTags_1f622(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f622), culture);

    public static string EmojiSearchTags_1f623() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f623));

    public static string EmojiSearchTags_1f623(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f623), culture);

    public static string EmojiSearchTags_1f624() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f624));

    public static string EmojiSearchTags_1f624(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f624), culture);

    public static string EmojiSearchTags_1f625() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f625));

    public static string EmojiSearchTags_1f625(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f625), culture);

    public static string EmojiSearchTags_1f628() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f628));

    public static string EmojiSearchTags_1f628(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f628), culture);

    public static string EmojiSearchTags_1f629() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f629));

    public static string EmojiSearchTags_1f629(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f629), culture);

    public static string EmojiSearchTags_1f62a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f62a));

    public static string EmojiSearchTags_1f62a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f62a), culture);

    public static string EmojiSearchTags_1f62b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f62b));

    public static string EmojiSearchTags_1f62b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f62b), culture);

    public static string EmojiSearchTags_1f62d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f62d));

    public static string EmojiSearchTags_1f62d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f62d), culture);

    public static string EmojiSearchTags_1f630() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f630));

    public static string EmojiSearchTags_1f630(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f630), culture);

    public static string EmojiSearchTags_1f631() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f631));

    public static string EmojiSearchTags_1f631(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f631), culture);

    public static string EmojiSearchTags_1f632() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f632));

    public static string EmojiSearchTags_1f632(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f632), culture);

    public static string EmojiSearchTags_1f633() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f633));

    public static string EmojiSearchTags_1f633(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f633), culture);

    public static string EmojiSearchTags_1f635() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f635));

    public static string EmojiSearchTags_1f635(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f635), culture);

    public static string EmojiSearchTags_1f636() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f636));

    public static string EmojiSearchTags_1f636(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f636), culture);

    public static string EmojiSearchTags_1f637() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f637));

    public static string EmojiSearchTags_1f637(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f637), culture);

    public static string EmojiSearchTags_1f638() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f638));

    public static string EmojiSearchTags_1f638(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f638), culture);

    public static string EmojiSearchTags_1f639() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f639));

    public static string EmojiSearchTags_1f639(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f639), culture);

    public static string EmojiSearchTags_1f63a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63a));

    public static string EmojiSearchTags_1f63a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63a), culture);

    public static string EmojiSearchTags_1f63b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63b));

    public static string EmojiSearchTags_1f63b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63b), culture);

    public static string EmojiSearchTags_1f63c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63c));

    public static string EmojiSearchTags_1f63c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63c), culture);

    public static string EmojiSearchTags_1f63d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63d));

    public static string EmojiSearchTags_1f63d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63d), culture);

    public static string EmojiSearchTags_1f63e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63e));

    public static string EmojiSearchTags_1f63e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63e), culture);

    public static string EmojiSearchTags_1f63f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63f));

    public static string EmojiSearchTags_1f63f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f63f), culture);

    public static string EmojiSearchTags_1f642() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f642));

    public static string EmojiSearchTags_1f642(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f642), culture);

    public static string EmojiSearchTags_1f645() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f645));

    public static string EmojiSearchTags_1f645(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f645), culture);

    public static string EmojiSearchTags_1f646() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f646));

    public static string EmojiSearchTags_1f646(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f646), culture);

    public static string EmojiSearchTags_1f648() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f648));

    public static string EmojiSearchTags_1f648(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f648), culture);

    public static string EmojiSearchTags_1f649() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f649));

    public static string EmojiSearchTags_1f649(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f649), culture);

    public static string EmojiSearchTags_1f64a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64a));

    public static string EmojiSearchTags_1f64a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64a), culture);

    public static string EmojiSearchTags_1f64b() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64b));

    public static string EmojiSearchTags_1f64b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64b), culture);

    public static string EmojiSearchTags_1f64c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64c));

    public static string EmojiSearchTags_1f64c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64c), culture);

    public static string EmojiSearchTags_1f64d() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64d));

    public static string EmojiSearchTags_1f64d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64d), culture);

    public static string EmojiSearchTags_1f64e() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64e));

    public static string EmojiSearchTags_1f64e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64e), culture);

    public static string EmojiSearchTags_1f64f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64f));

    public static string EmojiSearchTags_1f64f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f64f), culture);

    public static string EmojiSearchTags_1f680() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f680));

    public static string EmojiSearchTags_1f680(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f680), culture);

    public static string EmojiSearchTags_1f683() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f683));

    public static string EmojiSearchTags_1f683(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f683), culture);

    public static string EmojiSearchTags_1f684() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f684));

    public static string EmojiSearchTags_1f684(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f684), culture);

    public static string EmojiSearchTags_1f687() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f687));

    public static string EmojiSearchTags_1f687(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f687), culture);

    public static string EmojiSearchTags_1f689() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f689));

    public static string EmojiSearchTags_1f689(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f689), culture);

    public static string EmojiSearchTags_1f68c() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f68c));

    public static string EmojiSearchTags_1f68c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f68c), culture);

    public static string EmojiSearchTags_1f68f() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f68f));

    public static string EmojiSearchTags_1f68f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f68f), culture);

    public static string EmojiSearchTags_1f692() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f692));

    public static string EmojiSearchTags_1f692(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f692), culture);

    public static string EmojiSearchTags_1f693() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f693));

    public static string EmojiSearchTags_1f693(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f693), culture);

    public static string EmojiSearchTags_1f695() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f695));

    public static string EmojiSearchTags_1f695(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f695), culture);

    public static string EmojiSearchTags_1f697() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f697));

    public static string EmojiSearchTags_1f697(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f697), culture);

    public static string EmojiSearchTags_1f69a() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f69a));

    public static string EmojiSearchTags_1f69a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f69a), culture);

    public static string EmojiSearchTags_1f6a2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a2));

    public static string EmojiSearchTags_1f6a2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a2), culture);

    public static string EmojiSearchTags_1f6a5() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a5));

    public static string EmojiSearchTags_1f6a5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a5), culture);

    public static string EmojiSearchTags_1f6a7() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a7));

    public static string EmojiSearchTags_1f6a7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a7), culture);

    public static string EmojiSearchTags_1f6a8() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a8));

    public static string EmojiSearchTags_1f6a8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6a8), culture);

    public static string EmojiSearchTags_1f6aa() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6aa));

    public static string EmojiSearchTags_1f6aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6aa), culture);

    public static string EmojiSearchTags_1f6ab() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ab));

    public static string EmojiSearchTags_1f6ab(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ab), culture);

    public static string EmojiSearchTags_1f6ac() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ac));

    public static string EmojiSearchTags_1f6ac(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ac), culture);

    public static string EmojiSearchTags_1f6ad() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ad));

    public static string EmojiSearchTags_1f6ad(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ad), culture);

    public static string EmojiSearchTags_1f6b2() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6b2));

    public static string EmojiSearchTags_1f6b2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6b2), culture);

    public static string EmojiSearchTags_1f6b6() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6b6));

    public static string EmojiSearchTags_1f6b6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6b6), culture);

    public static string EmojiSearchTags_1f6b9() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6b9));

    public static string EmojiSearchTags_1f6b9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6b9), culture);

    public static string EmojiSearchTags_1f6ba() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ba));

    public static string EmojiSearchTags_1f6ba(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6ba), culture);

    public static string EmojiSearchTags_1f6bb() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6bb));

    public static string EmojiSearchTags_1f6bb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6bb), culture);

    public static string EmojiSearchTags_1f6bd() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6bd));

    public static string EmojiSearchTags_1f6bd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6bd), culture);

    public static string EmojiSearchTags_1f6c0() => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6c0));

    public static string EmojiSearchTags_1f6c0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiSearchTags_1f6c0), culture);

    public static string Emoji_02049() => RoosterJsEmojiResources.Get(nameof (Emoji_02049));

    public static string Emoji_02049(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02049), culture);

    public static string Emoji_02139() => RoosterJsEmojiResources.Get(nameof (Emoji_02139));

    public static string Emoji_02139(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02139), culture);

    public static string Emoji_021a9() => RoosterJsEmojiResources.Get(nameof (Emoji_021a9));

    public static string Emoji_021a9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_021a9), culture);

    public static string Emoji_021aa() => RoosterJsEmojiResources.Get(nameof (Emoji_021aa));

    public static string Emoji_021aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_021aa), culture);

    public static string Emoji_0231a() => RoosterJsEmojiResources.Get(nameof (Emoji_0231a));

    public static string Emoji_0231a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0231a), culture);

    public static string Emoji_0231b() => RoosterJsEmojiResources.Get(nameof (Emoji_0231b));

    public static string Emoji_0231b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0231b), culture);

    public static string Emoji_023e9() => RoosterJsEmojiResources.Get(nameof (Emoji_023e9));

    public static string Emoji_023e9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_023e9), culture);

    public static string Emoji_023ea() => RoosterJsEmojiResources.Get(nameof (Emoji_023ea));

    public static string Emoji_023ea(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_023ea), culture);

    public static string Emoji_023eb() => RoosterJsEmojiResources.Get(nameof (Emoji_023eb));

    public static string Emoji_023eb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_023eb), culture);

    public static string Emoji_023ec() => RoosterJsEmojiResources.Get(nameof (Emoji_023ec));

    public static string Emoji_023ec(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_023ec), culture);

    public static string Emoji_023f0() => RoosterJsEmojiResources.Get(nameof (Emoji_023f0));

    public static string Emoji_023f0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_023f0), culture);

    public static string Emoji_023f3() => RoosterJsEmojiResources.Get(nameof (Emoji_023f3));

    public static string Emoji_023f3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_023f3), culture);

    public static string Emoji_024c2() => RoosterJsEmojiResources.Get(nameof (Emoji_024c2));

    public static string Emoji_024c2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_024c2), culture);

    public static string Emoji_025b6() => RoosterJsEmojiResources.Get(nameof (Emoji_025b6));

    public static string Emoji_025b6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_025b6), culture);

    public static string Emoji_025c0() => RoosterJsEmojiResources.Get(nameof (Emoji_025c0));

    public static string Emoji_025c0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_025c0), culture);

    public static string Emoji_025fb() => RoosterJsEmojiResources.Get(nameof (Emoji_025fb));

    public static string Emoji_025fb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_025fb), culture);

    public static string Emoji_025fc() => RoosterJsEmojiResources.Get(nameof (Emoji_025fc));

    public static string Emoji_025fc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_025fc), culture);

    public static string Emoji_02600() => RoosterJsEmojiResources.Get(nameof (Emoji_02600));

    public static string Emoji_02600(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02600), culture);

    public static string Emoji_02601() => RoosterJsEmojiResources.Get(nameof (Emoji_02601));

    public static string Emoji_02601(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02601), culture);

    public static string Emoji_0260e() => RoosterJsEmojiResources.Get(nameof (Emoji_0260e));

    public static string Emoji_0260e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0260e), culture);

    public static string Emoji_02611() => RoosterJsEmojiResources.Get(nameof (Emoji_02611));

    public static string Emoji_02611(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02611), culture);

    public static string Emoji_02614() => RoosterJsEmojiResources.Get(nameof (Emoji_02614));

    public static string Emoji_02614(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02614), culture);

    public static string Emoji_02615() => RoosterJsEmojiResources.Get(nameof (Emoji_02615));

    public static string Emoji_02615(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02615), culture);

    public static string Emoji_0261d() => RoosterJsEmojiResources.Get(nameof (Emoji_0261d));

    public static string Emoji_0261d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0261d), culture);

    public static string Emoji_02648() => RoosterJsEmojiResources.Get(nameof (Emoji_02648));

    public static string Emoji_02648(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02648), culture);

    public static string Emoji_02649() => RoosterJsEmojiResources.Get(nameof (Emoji_02649));

    public static string Emoji_02649(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02649), culture);

    public static string Emoji_0264a() => RoosterJsEmojiResources.Get(nameof (Emoji_0264a));

    public static string Emoji_0264a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0264a), culture);

    public static string Emoji_0264b() => RoosterJsEmojiResources.Get(nameof (Emoji_0264b));

    public static string Emoji_0264b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0264b), culture);

    public static string Emoji_0264c() => RoosterJsEmojiResources.Get(nameof (Emoji_0264c));

    public static string Emoji_0264c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0264c), culture);

    public static string Emoji_0264d() => RoosterJsEmojiResources.Get(nameof (Emoji_0264d));

    public static string Emoji_0264d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0264d), culture);

    public static string Emoji_0264e() => RoosterJsEmojiResources.Get(nameof (Emoji_0264e));

    public static string Emoji_0264e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0264e), culture);

    public static string Emoji_0264f() => RoosterJsEmojiResources.Get(nameof (Emoji_0264f));

    public static string Emoji_0264f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0264f), culture);

    public static string Emoji_02650() => RoosterJsEmojiResources.Get(nameof (Emoji_02650));

    public static string Emoji_02650(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02650), culture);

    public static string Emoji_02651() => RoosterJsEmojiResources.Get(nameof (Emoji_02651));

    public static string Emoji_02651(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02651), culture);

    public static string Emoji_02652() => RoosterJsEmojiResources.Get(nameof (Emoji_02652));

    public static string Emoji_02652(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02652), culture);

    public static string Emoji_02653() => RoosterJsEmojiResources.Get(nameof (Emoji_02653));

    public static string Emoji_02653(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02653), culture);

    public static string Emoji_02668() => RoosterJsEmojiResources.Get(nameof (Emoji_02668));

    public static string Emoji_02668(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02668), culture);

    public static string Emoji_0267b() => RoosterJsEmojiResources.Get(nameof (Emoji_0267b));

    public static string Emoji_0267b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0267b), culture);

    public static string Emoji_0267f() => RoosterJsEmojiResources.Get(nameof (Emoji_0267f));

    public static string Emoji_0267f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0267f), culture);

    public static string Emoji_02693() => RoosterJsEmojiResources.Get(nameof (Emoji_02693));

    public static string Emoji_02693(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02693), culture);

    public static string Emoji_026a0() => RoosterJsEmojiResources.Get(nameof (Emoji_026a0));

    public static string Emoji_026a0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026a0), culture);

    public static string Emoji_026a1() => RoosterJsEmojiResources.Get(nameof (Emoji_026a1));

    public static string Emoji_026a1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026a1), culture);

    public static string Emoji_026aa() => RoosterJsEmojiResources.Get(nameof (Emoji_026aa));

    public static string Emoji_026aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026aa), culture);

    public static string Emoji_026ab() => RoosterJsEmojiResources.Get(nameof (Emoji_026ab));

    public static string Emoji_026ab(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026ab), culture);

    public static string Emoji_026bd() => RoosterJsEmojiResources.Get(nameof (Emoji_026bd));

    public static string Emoji_026bd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026bd), culture);

    public static string Emoji_026be() => RoosterJsEmojiResources.Get(nameof (Emoji_026be));

    public static string Emoji_026be(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026be), culture);

    public static string Emoji_026c4() => RoosterJsEmojiResources.Get(nameof (Emoji_026c4));

    public static string Emoji_026c4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026c4), culture);

    public static string Emoji_026c5() => RoosterJsEmojiResources.Get(nameof (Emoji_026c5));

    public static string Emoji_026c5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026c5), culture);

    public static string Emoji_026ce() => RoosterJsEmojiResources.Get(nameof (Emoji_026ce));

    public static string Emoji_026ce(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026ce), culture);

    public static string Emoji_026d4() => RoosterJsEmojiResources.Get(nameof (Emoji_026d4));

    public static string Emoji_026d4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026d4), culture);

    public static string Emoji_026ea() => RoosterJsEmojiResources.Get(nameof (Emoji_026ea));

    public static string Emoji_026ea(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026ea), culture);

    public static string Emoji_026f2() => RoosterJsEmojiResources.Get(nameof (Emoji_026f2));

    public static string Emoji_026f2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026f2), culture);

    public static string Emoji_026f3() => RoosterJsEmojiResources.Get(nameof (Emoji_026f3));

    public static string Emoji_026f3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026f3), culture);

    public static string Emoji_026f5() => RoosterJsEmojiResources.Get(nameof (Emoji_026f5));

    public static string Emoji_026f5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026f5), culture);

    public static string Emoji_026fa() => RoosterJsEmojiResources.Get(nameof (Emoji_026fa));

    public static string Emoji_026fa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026fa), culture);

    public static string Emoji_026fd() => RoosterJsEmojiResources.Get(nameof (Emoji_026fd));

    public static string Emoji_026fd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_026fd), culture);

    public static string Emoji_02702() => RoosterJsEmojiResources.Get(nameof (Emoji_02702));

    public static string Emoji_02702(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02702), culture);

    public static string Emoji_02705() => RoosterJsEmojiResources.Get(nameof (Emoji_02705));

    public static string Emoji_02705(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02705), culture);

    public static string Emoji_02708() => RoosterJsEmojiResources.Get(nameof (Emoji_02708));

    public static string Emoji_02708(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02708), culture);

    public static string Emoji_02709() => RoosterJsEmojiResources.Get(nameof (Emoji_02709));

    public static string Emoji_02709(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02709), culture);

    public static string Emoji_0270a() => RoosterJsEmojiResources.Get(nameof (Emoji_0270a));

    public static string Emoji_0270a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0270a), culture);

    public static string Emoji_0270b() => RoosterJsEmojiResources.Get(nameof (Emoji_0270b));

    public static string Emoji_0270b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0270b), culture);

    public static string Emoji_0270c() => RoosterJsEmojiResources.Get(nameof (Emoji_0270c));

    public static string Emoji_0270c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0270c), culture);

    public static string Emoji_0270f() => RoosterJsEmojiResources.Get(nameof (Emoji_0270f));

    public static string Emoji_0270f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0270f), culture);

    public static string Emoji_02712() => RoosterJsEmojiResources.Get(nameof (Emoji_02712));

    public static string Emoji_02712(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02712), culture);

    public static string Emoji_02714() => RoosterJsEmojiResources.Get(nameof (Emoji_02714));

    public static string Emoji_02714(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02714), culture);

    public static string Emoji_02716() => RoosterJsEmojiResources.Get(nameof (Emoji_02716));

    public static string Emoji_02716(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02716), culture);

    public static string Emoji_02728() => RoosterJsEmojiResources.Get(nameof (Emoji_02728));

    public static string Emoji_02728(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02728), culture);

    public static string Emoji_02733() => RoosterJsEmojiResources.Get(nameof (Emoji_02733));

    public static string Emoji_02733(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02733), culture);

    public static string Emoji_02734() => RoosterJsEmojiResources.Get(nameof (Emoji_02734));

    public static string Emoji_02734(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02734), culture);

    public static string Emoji_02744() => RoosterJsEmojiResources.Get(nameof (Emoji_02744));

    public static string Emoji_02744(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02744), culture);

    public static string Emoji_02747() => RoosterJsEmojiResources.Get(nameof (Emoji_02747));

    public static string Emoji_02747(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02747), culture);

    public static string Emoji_0274c() => RoosterJsEmojiResources.Get(nameof (Emoji_0274c));

    public static string Emoji_0274c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0274c), culture);

    public static string Emoji_0274e() => RoosterJsEmojiResources.Get(nameof (Emoji_0274e));

    public static string Emoji_0274e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0274e), culture);

    public static string Emoji_02753() => RoosterJsEmojiResources.Get(nameof (Emoji_02753));

    public static string Emoji_02753(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02753), culture);

    public static string Emoji_02754() => RoosterJsEmojiResources.Get(nameof (Emoji_02754));

    public static string Emoji_02754(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02754), culture);

    public static string Emoji_02755() => RoosterJsEmojiResources.Get(nameof (Emoji_02755));

    public static string Emoji_02755(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02755), culture);

    public static string Emoji_02757() => RoosterJsEmojiResources.Get(nameof (Emoji_02757));

    public static string Emoji_02757(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02757), culture);

    public static string Emoji_02764() => RoosterJsEmojiResources.Get(nameof (Emoji_02764));

    public static string Emoji_02764(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02764), culture);

    public static string Emoji_02795() => RoosterJsEmojiResources.Get(nameof (Emoji_02795));

    public static string Emoji_02795(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02795), culture);

    public static string Emoji_02796() => RoosterJsEmojiResources.Get(nameof (Emoji_02796));

    public static string Emoji_02796(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02796), culture);

    public static string Emoji_02797() => RoosterJsEmojiResources.Get(nameof (Emoji_02797));

    public static string Emoji_02797(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02797), culture);

    public static string Emoji_027a1() => RoosterJsEmojiResources.Get(nameof (Emoji_027a1));

    public static string Emoji_027a1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_027a1), culture);

    public static string Emoji_027b0() => RoosterJsEmojiResources.Get(nameof (Emoji_027b0));

    public static string Emoji_027b0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_027b0), culture);

    public static string Emoji_027bf() => RoosterJsEmojiResources.Get(nameof (Emoji_027bf));

    public static string Emoji_027bf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_027bf), culture);

    public static string Emoji_02934() => RoosterJsEmojiResources.Get(nameof (Emoji_02934));

    public static string Emoji_02934(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02934), culture);

    public static string Emoji_02935() => RoosterJsEmojiResources.Get(nameof (Emoji_02935));

    public static string Emoji_02935(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02935), culture);

    public static string Emoji_02b05() => RoosterJsEmojiResources.Get(nameof (Emoji_02b05));

    public static string Emoji_02b05(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02b05), culture);

    public static string Emoji_02b06() => RoosterJsEmojiResources.Get(nameof (Emoji_02b06));

    public static string Emoji_02b06(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02b06), culture);

    public static string Emoji_02b07() => RoosterJsEmojiResources.Get(nameof (Emoji_02b07));

    public static string Emoji_02b07(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02b07), culture);

    public static string Emoji_02b1b() => RoosterJsEmojiResources.Get(nameof (Emoji_02b1b));

    public static string Emoji_02b1b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02b1b), culture);

    public static string Emoji_02b1c() => RoosterJsEmojiResources.Get(nameof (Emoji_02b1c));

    public static string Emoji_02b1c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02b1c), culture);

    public static string Emoji_02b50() => RoosterJsEmojiResources.Get(nameof (Emoji_02b50));

    public static string Emoji_02b50(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02b50), culture);

    public static string Emoji_02b55() => RoosterJsEmojiResources.Get(nameof (Emoji_02b55));

    public static string Emoji_02b55(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_02b55), culture);

    public static string Emoji_03030() => RoosterJsEmojiResources.Get(nameof (Emoji_03030));

    public static string Emoji_03030(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_03030), culture);

    public static string Emoji_0303d() => RoosterJsEmojiResources.Get(nameof (Emoji_0303d));

    public static string Emoji_0303d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_0303d), culture);

    public static string Emoji_03297() => RoosterJsEmojiResources.Get(nameof (Emoji_03297));

    public static string Emoji_03297(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_03297), culture);

    public static string Emoji_03299() => RoosterJsEmojiResources.Get(nameof (Emoji_03299));

    public static string Emoji_03299(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_03299), culture);

    public static string Emoji_1f004() => RoosterJsEmojiResources.Get(nameof (Emoji_1f004));

    public static string Emoji_1f004(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f004), culture);

    public static string Emoji_1f0cf() => RoosterJsEmojiResources.Get(nameof (Emoji_1f0cf));

    public static string Emoji_1f0cf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f0cf), culture);

    public static string Emoji_1f170() => RoosterJsEmojiResources.Get(nameof (Emoji_1f170));

    public static string Emoji_1f170(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f170), culture);

    public static string Emoji_1f171() => RoosterJsEmojiResources.Get(nameof (Emoji_1f171));

    public static string Emoji_1f171(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f171), culture);

    public static string Emoji_1f17e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f17e));

    public static string Emoji_1f17e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f17e), culture);

    public static string Emoji_1f17f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f17f));

    public static string Emoji_1f17f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f17f), culture);

    public static string Emoji_1f18e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f18e));

    public static string Emoji_1f18e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f18e), culture);

    public static string Emoji_1f191() => RoosterJsEmojiResources.Get(nameof (Emoji_1f191));

    public static string Emoji_1f191(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f191), culture);

    public static string Emoji_1f192() => RoosterJsEmojiResources.Get(nameof (Emoji_1f192));

    public static string Emoji_1f192(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f192), culture);

    public static string Emoji_1f193() => RoosterJsEmojiResources.Get(nameof (Emoji_1f193));

    public static string Emoji_1f193(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f193), culture);

    public static string Emoji_1f194() => RoosterJsEmojiResources.Get(nameof (Emoji_1f194));

    public static string Emoji_1f194(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f194), culture);

    public static string Emoji_1f195() => RoosterJsEmojiResources.Get(nameof (Emoji_1f195));

    public static string Emoji_1f195(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f195), culture);

    public static string Emoji_1f196() => RoosterJsEmojiResources.Get(nameof (Emoji_1f196));

    public static string Emoji_1f196(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f196), culture);

    public static string Emoji_1f197() => RoosterJsEmojiResources.Get(nameof (Emoji_1f197));

    public static string Emoji_1f197(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f197), culture);

    public static string Emoji_1f198() => RoosterJsEmojiResources.Get(nameof (Emoji_1f198));

    public static string Emoji_1f198(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f198), culture);

    public static string Emoji_1f199() => RoosterJsEmojiResources.Get(nameof (Emoji_1f199));

    public static string Emoji_1f199(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f199), culture);

    public static string Emoji_1f19a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f19a));

    public static string Emoji_1f19a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f19a), culture);

    public static string Emoji_1f201() => RoosterJsEmojiResources.Get(nameof (Emoji_1f201));

    public static string Emoji_1f201(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f201), culture);

    public static string Emoji_1f202() => RoosterJsEmojiResources.Get(nameof (Emoji_1f202));

    public static string Emoji_1f202(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f202), culture);

    public static string Emoji_1f21a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f21a));

    public static string Emoji_1f21a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f21a), culture);

    public static string Emoji_1f22f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f22f));

    public static string Emoji_1f22f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f22f), culture);

    public static string Emoji_1f232() => RoosterJsEmojiResources.Get(nameof (Emoji_1f232));

    public static string Emoji_1f232(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f232), culture);

    public static string Emoji_1f233() => RoosterJsEmojiResources.Get(nameof (Emoji_1f233));

    public static string Emoji_1f233(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f233), culture);

    public static string Emoji_1f234() => RoosterJsEmojiResources.Get(nameof (Emoji_1f234));

    public static string Emoji_1f234(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f234), culture);

    public static string Emoji_1f235() => RoosterJsEmojiResources.Get(nameof (Emoji_1f235));

    public static string Emoji_1f235(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f235), culture);

    public static string Emoji_1f236() => RoosterJsEmojiResources.Get(nameof (Emoji_1f236));

    public static string Emoji_1f236(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f236), culture);

    public static string Emoji_1f237() => RoosterJsEmojiResources.Get(nameof (Emoji_1f237));

    public static string Emoji_1f237(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f237), culture);

    public static string Emoji_1f238() => RoosterJsEmojiResources.Get(nameof (Emoji_1f238));

    public static string Emoji_1f238(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f238), culture);

    public static string Emoji_1f239() => RoosterJsEmojiResources.Get(nameof (Emoji_1f239));

    public static string Emoji_1f239(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f239), culture);

    public static string Emoji_1f23a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f23a));

    public static string Emoji_1f23a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f23a), culture);

    public static string Emoji_1f250() => RoosterJsEmojiResources.Get(nameof (Emoji_1f250));

    public static string Emoji_1f250(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f250), culture);

    public static string Emoji_1f251() => RoosterJsEmojiResources.Get(nameof (Emoji_1f251));

    public static string Emoji_1f251(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f251), culture);

    public static string Emoji_1f300() => RoosterJsEmojiResources.Get(nameof (Emoji_1f300));

    public static string Emoji_1f300(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f300), culture);

    public static string Emoji_1f301() => RoosterJsEmojiResources.Get(nameof (Emoji_1f301));

    public static string Emoji_1f301(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f301), culture);

    public static string Emoji_1f302() => RoosterJsEmojiResources.Get(nameof (Emoji_1f302));

    public static string Emoji_1f302(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f302), culture);

    public static string Emoji_1f303() => RoosterJsEmojiResources.Get(nameof (Emoji_1f303));

    public static string Emoji_1f303(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f303), culture);

    public static string Emoji_1f304() => RoosterJsEmojiResources.Get(nameof (Emoji_1f304));

    public static string Emoji_1f304(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f304), culture);

    public static string Emoji_1f305() => RoosterJsEmojiResources.Get(nameof (Emoji_1f305));

    public static string Emoji_1f305(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f305), culture);

    public static string Emoji_1f306() => RoosterJsEmojiResources.Get(nameof (Emoji_1f306));

    public static string Emoji_1f306(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f306), culture);

    public static string Emoji_1f307() => RoosterJsEmojiResources.Get(nameof (Emoji_1f307));

    public static string Emoji_1f307(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f307), culture);

    public static string Emoji_1f308() => RoosterJsEmojiResources.Get(nameof (Emoji_1f308));

    public static string Emoji_1f308(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f308), culture);

    public static string Emoji_1f309() => RoosterJsEmojiResources.Get(nameof (Emoji_1f309));

    public static string Emoji_1f309(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f309), culture);

    public static string Emoji_1f30a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f30a));

    public static string Emoji_1f30a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f30a), culture);

    public static string Emoji_1f30b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f30b));

    public static string Emoji_1f30b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f30b), culture);

    public static string Emoji_1f30c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f30c));

    public static string Emoji_1f30c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f30c), culture);

    public static string Emoji_1f30f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f30f));

    public static string Emoji_1f30f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f30f), culture);

    public static string Emoji_1f311() => RoosterJsEmojiResources.Get(nameof (Emoji_1f311));

    public static string Emoji_1f311(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f311), culture);

    public static string Emoji_1f313() => RoosterJsEmojiResources.Get(nameof (Emoji_1f313));

    public static string Emoji_1f313(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f313), culture);

    public static string Emoji_1f314() => RoosterJsEmojiResources.Get(nameof (Emoji_1f314));

    public static string Emoji_1f314(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f314), culture);

    public static string Emoji_1f315() => RoosterJsEmojiResources.Get(nameof (Emoji_1f315));

    public static string Emoji_1f315(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f315), culture);

    public static string Emoji_1f319() => RoosterJsEmojiResources.Get(nameof (Emoji_1f319));

    public static string Emoji_1f319(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f319), culture);

    public static string Emoji_1f31b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f31b));

    public static string Emoji_1f31b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f31b), culture);

    public static string Emoji_1f31f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f31f));

    public static string Emoji_1f31f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f31f), culture);

    public static string Emoji_1f320() => RoosterJsEmojiResources.Get(nameof (Emoji_1f320));

    public static string Emoji_1f320(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f320), culture);

    public static string Emoji_1f330() => RoosterJsEmojiResources.Get(nameof (Emoji_1f330));

    public static string Emoji_1f330(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f330), culture);

    public static string Emoji_1f331() => RoosterJsEmojiResources.Get(nameof (Emoji_1f331));

    public static string Emoji_1f331(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f331), culture);

    public static string Emoji_1f334() => RoosterJsEmojiResources.Get(nameof (Emoji_1f334));

    public static string Emoji_1f334(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f334), culture);

    public static string Emoji_1f335() => RoosterJsEmojiResources.Get(nameof (Emoji_1f335));

    public static string Emoji_1f335(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f335), culture);

    public static string Emoji_1f337() => RoosterJsEmojiResources.Get(nameof (Emoji_1f337));

    public static string Emoji_1f337(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f337), culture);

    public static string Emoji_1f338() => RoosterJsEmojiResources.Get(nameof (Emoji_1f338));

    public static string Emoji_1f338(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f338), culture);

    public static string Emoji_1f339() => RoosterJsEmojiResources.Get(nameof (Emoji_1f339));

    public static string Emoji_1f339(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f339), culture);

    public static string Emoji_1f33a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f33a));

    public static string Emoji_1f33a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f33a), culture);

    public static string Emoji_1f33b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f33b));

    public static string Emoji_1f33b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f33b), culture);

    public static string Emoji_1f33c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f33c));

    public static string Emoji_1f33c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f33c), culture);

    public static string Emoji_1f33d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f33d));

    public static string Emoji_1f33d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f33d), culture);

    public static string Emoji_1f33e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f33e));

    public static string Emoji_1f33e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f33e), culture);

    public static string Emoji_1f33f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f33f));

    public static string Emoji_1f33f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f33f), culture);

    public static string Emoji_1f340() => RoosterJsEmojiResources.Get(nameof (Emoji_1f340));

    public static string Emoji_1f340(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f340), culture);

    public static string Emoji_1f341() => RoosterJsEmojiResources.Get(nameof (Emoji_1f341));

    public static string Emoji_1f341(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f341), culture);

    public static string Emoji_1f342() => RoosterJsEmojiResources.Get(nameof (Emoji_1f342));

    public static string Emoji_1f342(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f342), culture);

    public static string Emoji_1f343() => RoosterJsEmojiResources.Get(nameof (Emoji_1f343));

    public static string Emoji_1f343(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f343), culture);

    public static string Emoji_1f344() => RoosterJsEmojiResources.Get(nameof (Emoji_1f344));

    public static string Emoji_1f344(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f344), culture);

    public static string Emoji_1f345() => RoosterJsEmojiResources.Get(nameof (Emoji_1f345));

    public static string Emoji_1f345(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f345), culture);

    public static string Emoji_1f346() => RoosterJsEmojiResources.Get(nameof (Emoji_1f346));

    public static string Emoji_1f346(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f346), culture);

    public static string Emoji_1f347() => RoosterJsEmojiResources.Get(nameof (Emoji_1f347));

    public static string Emoji_1f347(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f347), culture);

    public static string Emoji_1f348() => RoosterJsEmojiResources.Get(nameof (Emoji_1f348));

    public static string Emoji_1f348(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f348), culture);

    public static string Emoji_1f349() => RoosterJsEmojiResources.Get(nameof (Emoji_1f349));

    public static string Emoji_1f349(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f349), culture);

    public static string Emoji_1f34a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f34a));

    public static string Emoji_1f34a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f34a), culture);

    public static string Emoji_1f34c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f34c));

    public static string Emoji_1f34c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f34c), culture);

    public static string Emoji_1f34d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f34d));

    public static string Emoji_1f34d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f34d), culture);

    public static string Emoji_1f34e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f34e));

    public static string Emoji_1f34e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f34e), culture);

    public static string Emoji_1f34f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f34f));

    public static string Emoji_1f34f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f34f), culture);

    public static string Emoji_1f351() => RoosterJsEmojiResources.Get(nameof (Emoji_1f351));

    public static string Emoji_1f351(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f351), culture);

    public static string Emoji_1f352() => RoosterJsEmojiResources.Get(nameof (Emoji_1f352));

    public static string Emoji_1f352(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f352), culture);

    public static string Emoji_1f353() => RoosterJsEmojiResources.Get(nameof (Emoji_1f353));

    public static string Emoji_1f353(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f353), culture);

    public static string Emoji_1f354() => RoosterJsEmojiResources.Get(nameof (Emoji_1f354));

    public static string Emoji_1f354(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f354), culture);

    public static string Emoji_1f355() => RoosterJsEmojiResources.Get(nameof (Emoji_1f355));

    public static string Emoji_1f355(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f355), culture);

    public static string Emoji_1f356() => RoosterJsEmojiResources.Get(nameof (Emoji_1f356));

    public static string Emoji_1f356(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f356), culture);

    public static string Emoji_1f357() => RoosterJsEmojiResources.Get(nameof (Emoji_1f357));

    public static string Emoji_1f357(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f357), culture);

    public static string Emoji_1f358() => RoosterJsEmojiResources.Get(nameof (Emoji_1f358));

    public static string Emoji_1f358(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f358), culture);

    public static string Emoji_1f359() => RoosterJsEmojiResources.Get(nameof (Emoji_1f359));

    public static string Emoji_1f359(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f359), culture);

    public static string Emoji_1f35a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f35a));

    public static string Emoji_1f35a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f35a), culture);

    public static string Emoji_1f35b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f35b));

    public static string Emoji_1f35b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f35b), culture);

    public static string Emoji_1f35c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f35c));

    public static string Emoji_1f35c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f35c), culture);

    public static string Emoji_1f35d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f35d));

    public static string Emoji_1f35d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f35d), culture);

    public static string Emoji_1f35e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f35e));

    public static string Emoji_1f35e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f35e), culture);

    public static string Emoji_1f35f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f35f));

    public static string Emoji_1f35f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f35f), culture);

    public static string Emoji_1f360() => RoosterJsEmojiResources.Get(nameof (Emoji_1f360));

    public static string Emoji_1f360(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f360), culture);

    public static string Emoji_1f361() => RoosterJsEmojiResources.Get(nameof (Emoji_1f361));

    public static string Emoji_1f361(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f361), culture);

    public static string Emoji_1f362() => RoosterJsEmojiResources.Get(nameof (Emoji_1f362));

    public static string Emoji_1f362(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f362), culture);

    public static string Emoji_1f363() => RoosterJsEmojiResources.Get(nameof (Emoji_1f363));

    public static string Emoji_1f363(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f363), culture);

    public static string Emoji_1f364() => RoosterJsEmojiResources.Get(nameof (Emoji_1f364));

    public static string Emoji_1f364(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f364), culture);

    public static string Emoji_1f365() => RoosterJsEmojiResources.Get(nameof (Emoji_1f365));

    public static string Emoji_1f365(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f365), culture);

    public static string Emoji_1f366() => RoosterJsEmojiResources.Get(nameof (Emoji_1f366));

    public static string Emoji_1f366(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f366), culture);

    public static string Emoji_1f367() => RoosterJsEmojiResources.Get(nameof (Emoji_1f367));

    public static string Emoji_1f367(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f367), culture);

    public static string Emoji_1f368() => RoosterJsEmojiResources.Get(nameof (Emoji_1f368));

    public static string Emoji_1f368(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f368), culture);

    public static string Emoji_1f369() => RoosterJsEmojiResources.Get(nameof (Emoji_1f369));

    public static string Emoji_1f369(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f369), culture);

    public static string Emoji_1f36a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f36a));

    public static string Emoji_1f36a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f36a), culture);

    public static string Emoji_1f36b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f36b));

    public static string Emoji_1f36b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f36b), culture);

    public static string Emoji_1f36c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f36c));

    public static string Emoji_1f36c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f36c), culture);

    public static string Emoji_1f36d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f36d));

    public static string Emoji_1f36d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f36d), culture);

    public static string Emoji_1f36e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f36e));

    public static string Emoji_1f36e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f36e), culture);

    public static string Emoji_1f36f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f36f));

    public static string Emoji_1f36f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f36f), culture);

    public static string Emoji_1f370() => RoosterJsEmojiResources.Get(nameof (Emoji_1f370));

    public static string Emoji_1f370(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f370), culture);

    public static string Emoji_1f371() => RoosterJsEmojiResources.Get(nameof (Emoji_1f371));

    public static string Emoji_1f371(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f371), culture);

    public static string Emoji_1f372() => RoosterJsEmojiResources.Get(nameof (Emoji_1f372));

    public static string Emoji_1f372(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f372), culture);

    public static string Emoji_1f373() => RoosterJsEmojiResources.Get(nameof (Emoji_1f373));

    public static string Emoji_1f373(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f373), culture);

    public static string Emoji_1f374() => RoosterJsEmojiResources.Get(nameof (Emoji_1f374));

    public static string Emoji_1f374(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f374), culture);

    public static string Emoji_1f375() => RoosterJsEmojiResources.Get(nameof (Emoji_1f375));

    public static string Emoji_1f375(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f375), culture);

    public static string Emoji_1f376() => RoosterJsEmojiResources.Get(nameof (Emoji_1f376));

    public static string Emoji_1f376(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f376), culture);

    public static string Emoji_1f377() => RoosterJsEmojiResources.Get(nameof (Emoji_1f377));

    public static string Emoji_1f377(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f377), culture);

    public static string Emoji_1f378() => RoosterJsEmojiResources.Get(nameof (Emoji_1f378));

    public static string Emoji_1f378(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f378), culture);

    public static string Emoji_1f379() => RoosterJsEmojiResources.Get(nameof (Emoji_1f379));

    public static string Emoji_1f379(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f379), culture);

    public static string Emoji_1f37a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f37a));

    public static string Emoji_1f37a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f37a), culture);

    public static string Emoji_1f37b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f37b));

    public static string Emoji_1f37b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f37b), culture);

    public static string Emoji_1f380() => RoosterJsEmojiResources.Get(nameof (Emoji_1f380));

    public static string Emoji_1f380(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f380), culture);

    public static string Emoji_1f381() => RoosterJsEmojiResources.Get(nameof (Emoji_1f381));

    public static string Emoji_1f381(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f381), culture);

    public static string Emoji_1f382() => RoosterJsEmojiResources.Get(nameof (Emoji_1f382));

    public static string Emoji_1f382(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f382), culture);

    public static string Emoji_1f383() => RoosterJsEmojiResources.Get(nameof (Emoji_1f383));

    public static string Emoji_1f383(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f383), culture);

    public static string Emoji_1f384() => RoosterJsEmojiResources.Get(nameof (Emoji_1f384));

    public static string Emoji_1f384(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f384), culture);

    public static string Emoji_1f385() => RoosterJsEmojiResources.Get(nameof (Emoji_1f385));

    public static string Emoji_1f385(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f385), culture);

    public static string Emoji_1f386() => RoosterJsEmojiResources.Get(nameof (Emoji_1f386));

    public static string Emoji_1f386(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f386), culture);

    public static string Emoji_1f387() => RoosterJsEmojiResources.Get(nameof (Emoji_1f387));

    public static string Emoji_1f387(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f387), culture);

    public static string Emoji_1f388() => RoosterJsEmojiResources.Get(nameof (Emoji_1f388));

    public static string Emoji_1f388(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f388), culture);

    public static string Emoji_1f389() => RoosterJsEmojiResources.Get(nameof (Emoji_1f389));

    public static string Emoji_1f389(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f389), culture);

    public static string Emoji_1f38a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f38a));

    public static string Emoji_1f38a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f38a), culture);

    public static string Emoji_1f38b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f38b));

    public static string Emoji_1f38b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f38b), culture);

    public static string Emoji_1f38c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f38c));

    public static string Emoji_1f38c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f38c), culture);

    public static string Emoji_1f38d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f38d));

    public static string Emoji_1f38d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f38d), culture);

    public static string Emoji_1f38e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f38e));

    public static string Emoji_1f38e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f38e), culture);

    public static string Emoji_1f38f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f38f));

    public static string Emoji_1f38f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f38f), culture);

    public static string Emoji_1f390() => RoosterJsEmojiResources.Get(nameof (Emoji_1f390));

    public static string Emoji_1f390(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f390), culture);

    public static string Emoji_1f391() => RoosterJsEmojiResources.Get(nameof (Emoji_1f391));

    public static string Emoji_1f391(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f391), culture);

    public static string Emoji_1f392() => RoosterJsEmojiResources.Get(nameof (Emoji_1f392));

    public static string Emoji_1f392(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f392), culture);

    public static string Emoji_1f393() => RoosterJsEmojiResources.Get(nameof (Emoji_1f393));

    public static string Emoji_1f393(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f393), culture);

    public static string Emoji_1f3a0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a0));

    public static string Emoji_1f3a0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a0), culture);

    public static string Emoji_1f3a1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a1));

    public static string Emoji_1f3a1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a1), culture);

    public static string Emoji_1f3a2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a2));

    public static string Emoji_1f3a2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a2), culture);

    public static string Emoji_1f3a3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a3));

    public static string Emoji_1f3a3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a3), culture);

    public static string Emoji_1f3a4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a4));

    public static string Emoji_1f3a4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a4), culture);

    public static string Emoji_1f3a5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a5));

    public static string Emoji_1f3a5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a5), culture);

    public static string Emoji_1f3a6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a6));

    public static string Emoji_1f3a6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a6), culture);

    public static string Emoji_1f3a7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a7));

    public static string Emoji_1f3a7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a7), culture);

    public static string Emoji_1f3a8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a8));

    public static string Emoji_1f3a8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a8), culture);

    public static string Emoji_1f3a9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a9));

    public static string Emoji_1f3a9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3a9), culture);

    public static string Emoji_1f3aa() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3aa));

    public static string Emoji_1f3aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3aa), culture);

    public static string Emoji_1f3ab() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ab));

    public static string Emoji_1f3ab(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ab), culture);

    public static string Emoji_1f3ac() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ac));

    public static string Emoji_1f3ac(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ac), culture);

    public static string Emoji_1f3ad() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ad));

    public static string Emoji_1f3ad(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ad), culture);

    public static string Emoji_1f3ae() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ae));

    public static string Emoji_1f3ae(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ae), culture);

    public static string Emoji_1f3af() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3af));

    public static string Emoji_1f3af(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3af), culture);

    public static string Emoji_1f3b0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b0));

    public static string Emoji_1f3b0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b0), culture);

    public static string Emoji_1f3b1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b1));

    public static string Emoji_1f3b1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b1), culture);

    public static string Emoji_1f3b2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b2));

    public static string Emoji_1f3b2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b2), culture);

    public static string Emoji_1f3b3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b3));

    public static string Emoji_1f3b3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b3), culture);

    public static string Emoji_1f3b4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b4));

    public static string Emoji_1f3b4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b4), culture);

    public static string Emoji_1f3b5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b5));

    public static string Emoji_1f3b5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b5), culture);

    public static string Emoji_1f3b6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b6));

    public static string Emoji_1f3b6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b6), culture);

    public static string Emoji_1f3b7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b7));

    public static string Emoji_1f3b7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b7), culture);

    public static string Emoji_1f3b8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b8));

    public static string Emoji_1f3b8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b8), culture);

    public static string Emoji_1f3b9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b9));

    public static string Emoji_1f3b9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3b9), culture);

    public static string Emoji_1f3ba() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ba));

    public static string Emoji_1f3ba(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ba), culture);

    public static string Emoji_1f3bb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bb));

    public static string Emoji_1f3bb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bb), culture);

    public static string Emoji_1f3bc() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bc));

    public static string Emoji_1f3bc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bc), culture);

    public static string Emoji_1f3bd() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bd));

    public static string Emoji_1f3bd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bd), culture);

    public static string Emoji_1f3be() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3be));

    public static string Emoji_1f3be(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3be), culture);

    public static string Emoji_1f3bf() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bf));

    public static string Emoji_1f3bf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3bf), culture);

    public static string Emoji_1f3c0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c0));

    public static string Emoji_1f3c0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c0), culture);

    public static string Emoji_1f3c1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c1));

    public static string Emoji_1f3c1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c1), culture);

    public static string Emoji_1f3c2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c2));

    public static string Emoji_1f3c2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c2), culture);

    public static string Emoji_1f3c3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c3));

    public static string Emoji_1f3c3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c3), culture);

    public static string Emoji_1f3c4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c4));

    public static string Emoji_1f3c4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c4), culture);

    public static string Emoji_1f3c6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c6));

    public static string Emoji_1f3c6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c6), culture);

    public static string Emoji_1f3c8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c8));

    public static string Emoji_1f3c8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3c8), culture);

    public static string Emoji_1f3ca() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ca));

    public static string Emoji_1f3ca(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ca), culture);

    public static string Emoji_1f3e0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e0));

    public static string Emoji_1f3e0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e0), culture);

    public static string Emoji_1f3e1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e1));

    public static string Emoji_1f3e1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e1), culture);

    public static string Emoji_1f3e2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e2));

    public static string Emoji_1f3e2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e2), culture);

    public static string Emoji_1f3e3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e3));

    public static string Emoji_1f3e3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e3), culture);

    public static string Emoji_1f3e5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e5));

    public static string Emoji_1f3e5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e5), culture);

    public static string Emoji_1f3e6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e6));

    public static string Emoji_1f3e6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e6), culture);

    public static string Emoji_1f3e7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e7));

    public static string Emoji_1f3e7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e7), culture);

    public static string Emoji_1f3e8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e8));

    public static string Emoji_1f3e8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e8), culture);

    public static string Emoji_1f3e9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e9));

    public static string Emoji_1f3e9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3e9), culture);

    public static string Emoji_1f3ea() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ea));

    public static string Emoji_1f3ea(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ea), culture);

    public static string Emoji_1f3eb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3eb));

    public static string Emoji_1f3eb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3eb), culture);

    public static string Emoji_1f3ec() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ec));

    public static string Emoji_1f3ec(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ec), culture);

    public static string Emoji_1f3ed() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ed));

    public static string Emoji_1f3ed(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ed), culture);

    public static string Emoji_1f3ee() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ee));

    public static string Emoji_1f3ee(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ee), culture);

    public static string Emoji_1f3ef() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ef));

    public static string Emoji_1f3ef(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3ef), culture);

    public static string Emoji_1f3f0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f3f0));

    public static string Emoji_1f3f0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f3f0), culture);

    public static string Emoji_1f40c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f40c));

    public static string Emoji_1f40c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f40c), culture);

    public static string Emoji_1f40d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f40d));

    public static string Emoji_1f40d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f40d), culture);

    public static string Emoji_1f40e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f40e));

    public static string Emoji_1f40e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f40e), culture);

    public static string Emoji_1f411() => RoosterJsEmojiResources.Get(nameof (Emoji_1f411));

    public static string Emoji_1f411(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f411), culture);

    public static string Emoji_1f412() => RoosterJsEmojiResources.Get(nameof (Emoji_1f412));

    public static string Emoji_1f412(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f412), culture);

    public static string Emoji_1f414() => RoosterJsEmojiResources.Get(nameof (Emoji_1f414));

    public static string Emoji_1f414(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f414), culture);

    public static string Emoji_1f417() => RoosterJsEmojiResources.Get(nameof (Emoji_1f417));

    public static string Emoji_1f417(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f417), culture);

    public static string Emoji_1f418() => RoosterJsEmojiResources.Get(nameof (Emoji_1f418));

    public static string Emoji_1f418(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f418), culture);

    public static string Emoji_1f419() => RoosterJsEmojiResources.Get(nameof (Emoji_1f419));

    public static string Emoji_1f419(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f419), culture);

    public static string Emoji_1f41a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f41a));

    public static string Emoji_1f41a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f41a), culture);

    public static string Emoji_1f41b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f41b));

    public static string Emoji_1f41b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f41b), culture);

    public static string Emoji_1f41c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f41c));

    public static string Emoji_1f41c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f41c), culture);

    public static string Emoji_1f41d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f41d));

    public static string Emoji_1f41d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f41d), culture);

    public static string Emoji_1f41e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f41e));

    public static string Emoji_1f41e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f41e), culture);

    public static string Emoji_1f41f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f41f));

    public static string Emoji_1f41f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f41f), culture);

    public static string Emoji_1f420() => RoosterJsEmojiResources.Get(nameof (Emoji_1f420));

    public static string Emoji_1f420(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f420), culture);

    public static string Emoji_1f421() => RoosterJsEmojiResources.Get(nameof (Emoji_1f421));

    public static string Emoji_1f421(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f421), culture);

    public static string Emoji_1f422() => RoosterJsEmojiResources.Get(nameof (Emoji_1f422));

    public static string Emoji_1f422(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f422), culture);

    public static string Emoji_1f423() => RoosterJsEmojiResources.Get(nameof (Emoji_1f423));

    public static string Emoji_1f423(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f423), culture);

    public static string Emoji_1f424() => RoosterJsEmojiResources.Get(nameof (Emoji_1f424));

    public static string Emoji_1f424(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f424), culture);

    public static string Emoji_1f425() => RoosterJsEmojiResources.Get(nameof (Emoji_1f425));

    public static string Emoji_1f425(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f425), culture);

    public static string Emoji_1f426() => RoosterJsEmojiResources.Get(nameof (Emoji_1f426));

    public static string Emoji_1f426(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f426), culture);

    public static string Emoji_1f427() => RoosterJsEmojiResources.Get(nameof (Emoji_1f427));

    public static string Emoji_1f427(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f427), culture);

    public static string Emoji_1f428() => RoosterJsEmojiResources.Get(nameof (Emoji_1f428));

    public static string Emoji_1f428(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f428), culture);

    public static string Emoji_1f429() => RoosterJsEmojiResources.Get(nameof (Emoji_1f429));

    public static string Emoji_1f429(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f429), culture);

    public static string Emoji_1f42b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f42b));

    public static string Emoji_1f42b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f42b), culture);

    public static string Emoji_1f42c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f42c));

    public static string Emoji_1f42c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f42c), culture);

    public static string Emoji_1f42d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f42d));

    public static string Emoji_1f42d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f42d), culture);

    public static string Emoji_1f42e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f42e));

    public static string Emoji_1f42e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f42e), culture);

    public static string Emoji_1f42f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f42f));

    public static string Emoji_1f42f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f42f), culture);

    public static string Emoji_1f430() => RoosterJsEmojiResources.Get(nameof (Emoji_1f430));

    public static string Emoji_1f430(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f430), culture);

    public static string Emoji_1f431() => RoosterJsEmojiResources.Get(nameof (Emoji_1f431));

    public static string Emoji_1f431(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f431), culture);

    public static string Emoji_1f432() => RoosterJsEmojiResources.Get(nameof (Emoji_1f432));

    public static string Emoji_1f432(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f432), culture);

    public static string Emoji_1f433() => RoosterJsEmojiResources.Get(nameof (Emoji_1f433));

    public static string Emoji_1f433(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f433), culture);

    public static string Emoji_1f434() => RoosterJsEmojiResources.Get(nameof (Emoji_1f434));

    public static string Emoji_1f434(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f434), culture);

    public static string Emoji_1f435() => RoosterJsEmojiResources.Get(nameof (Emoji_1f435));

    public static string Emoji_1f435(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f435), culture);

    public static string Emoji_1f436() => RoosterJsEmojiResources.Get(nameof (Emoji_1f436));

    public static string Emoji_1f436(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f436), culture);

    public static string Emoji_1f437() => RoosterJsEmojiResources.Get(nameof (Emoji_1f437));

    public static string Emoji_1f437(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f437), culture);

    public static string Emoji_1f438() => RoosterJsEmojiResources.Get(nameof (Emoji_1f438));

    public static string Emoji_1f438(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f438), culture);

    public static string Emoji_1f439() => RoosterJsEmojiResources.Get(nameof (Emoji_1f439));

    public static string Emoji_1f439(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f439), culture);

    public static string Emoji_1f43a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f43a));

    public static string Emoji_1f43a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f43a), culture);

    public static string Emoji_1f43b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f43b));

    public static string Emoji_1f43b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f43b), culture);

    public static string Emoji_1f43c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f43c));

    public static string Emoji_1f43c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f43c), culture);

    public static string Emoji_1f43d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f43d));

    public static string Emoji_1f43d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f43d), culture);

    public static string Emoji_1f43e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f43e));

    public static string Emoji_1f43e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f43e), culture);

    public static string Emoji_1f440() => RoosterJsEmojiResources.Get(nameof (Emoji_1f440));

    public static string Emoji_1f440(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f440), culture);

    public static string Emoji_1f442() => RoosterJsEmojiResources.Get(nameof (Emoji_1f442));

    public static string Emoji_1f442(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f442), culture);

    public static string Emoji_1f443() => RoosterJsEmojiResources.Get(nameof (Emoji_1f443));

    public static string Emoji_1f443(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f443), culture);

    public static string Emoji_1f444() => RoosterJsEmojiResources.Get(nameof (Emoji_1f444));

    public static string Emoji_1f444(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f444), culture);

    public static string Emoji_1f445() => RoosterJsEmojiResources.Get(nameof (Emoji_1f445));

    public static string Emoji_1f445(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f445), culture);

    public static string Emoji_1f446() => RoosterJsEmojiResources.Get(nameof (Emoji_1f446));

    public static string Emoji_1f446(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f446), culture);

    public static string Emoji_1f447() => RoosterJsEmojiResources.Get(nameof (Emoji_1f447));

    public static string Emoji_1f447(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f447), culture);

    public static string Emoji_1f448() => RoosterJsEmojiResources.Get(nameof (Emoji_1f448));

    public static string Emoji_1f448(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f448), culture);

    public static string Emoji_1f449() => RoosterJsEmojiResources.Get(nameof (Emoji_1f449));

    public static string Emoji_1f449(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f449), culture);

    public static string Emoji_1f44a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f44a));

    public static string Emoji_1f44a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f44a), culture);

    public static string Emoji_1f44b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f44b));

    public static string Emoji_1f44b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f44b), culture);

    public static string Emoji_1f44c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f44c));

    public static string Emoji_1f44c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f44c), culture);

    public static string Emoji_1f44d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f44d));

    public static string Emoji_1f44d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f44d), culture);

    public static string Emoji_1f44e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f44e));

    public static string Emoji_1f44e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f44e), culture);

    public static string Emoji_1f44f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f44f));

    public static string Emoji_1f44f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f44f), culture);

    public static string Emoji_1f450() => RoosterJsEmojiResources.Get(nameof (Emoji_1f450));

    public static string Emoji_1f450(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f450), culture);

    public static string Emoji_1f451() => RoosterJsEmojiResources.Get(nameof (Emoji_1f451));

    public static string Emoji_1f451(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f451), culture);

    public static string Emoji_1f452() => RoosterJsEmojiResources.Get(nameof (Emoji_1f452));

    public static string Emoji_1f452(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f452), culture);

    public static string Emoji_1f453() => RoosterJsEmojiResources.Get(nameof (Emoji_1f453));

    public static string Emoji_1f453(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f453), culture);

    public static string Emoji_1f454() => RoosterJsEmojiResources.Get(nameof (Emoji_1f454));

    public static string Emoji_1f454(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f454), culture);

    public static string Emoji_1f455() => RoosterJsEmojiResources.Get(nameof (Emoji_1f455));

    public static string Emoji_1f455(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f455), culture);

    public static string Emoji_1f456() => RoosterJsEmojiResources.Get(nameof (Emoji_1f456));

    public static string Emoji_1f456(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f456), culture);

    public static string Emoji_1f457() => RoosterJsEmojiResources.Get(nameof (Emoji_1f457));

    public static string Emoji_1f457(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f457), culture);

    public static string Emoji_1f458() => RoosterJsEmojiResources.Get(nameof (Emoji_1f458));

    public static string Emoji_1f458(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f458), culture);

    public static string Emoji_1f459() => RoosterJsEmojiResources.Get(nameof (Emoji_1f459));

    public static string Emoji_1f459(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f459), culture);

    public static string Emoji_1f45a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f45a));

    public static string Emoji_1f45a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f45a), culture);

    public static string Emoji_1f45b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f45b));

    public static string Emoji_1f45b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f45b), culture);

    public static string Emoji_1f45c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f45c));

    public static string Emoji_1f45c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f45c), culture);

    public static string Emoji_1f45d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f45d));

    public static string Emoji_1f45d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f45d), culture);

    public static string Emoji_1f45e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f45e));

    public static string Emoji_1f45e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f45e), culture);

    public static string Emoji_1f45f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f45f));

    public static string Emoji_1f45f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f45f), culture);

    public static string Emoji_1f460() => RoosterJsEmojiResources.Get(nameof (Emoji_1f460));

    public static string Emoji_1f460(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f460), culture);

    public static string Emoji_1f461() => RoosterJsEmojiResources.Get(nameof (Emoji_1f461));

    public static string Emoji_1f461(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f461), culture);

    public static string Emoji_1f462() => RoosterJsEmojiResources.Get(nameof (Emoji_1f462));

    public static string Emoji_1f462(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f462), culture);

    public static string Emoji_1f463() => RoosterJsEmojiResources.Get(nameof (Emoji_1f463));

    public static string Emoji_1f463(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f463), culture);

    public static string Emoji_1f464() => RoosterJsEmojiResources.Get(nameof (Emoji_1f464));

    public static string Emoji_1f464(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f464), culture);

    public static string Emoji_1f466() => RoosterJsEmojiResources.Get(nameof (Emoji_1f466));

    public static string Emoji_1f466(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f466), culture);

    public static string Emoji_1f467() => RoosterJsEmojiResources.Get(nameof (Emoji_1f467));

    public static string Emoji_1f467(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f467), culture);

    public static string Emoji_1f468() => RoosterJsEmojiResources.Get(nameof (Emoji_1f468));

    public static string Emoji_1f468(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f468), culture);

    public static string Emoji_1f469() => RoosterJsEmojiResources.Get(nameof (Emoji_1f469));

    public static string Emoji_1f469(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f469), culture);

    public static string Emoji_1f46a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f46a));

    public static string Emoji_1f46a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f46a), culture);

    public static string Emoji_1f46b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f46b));

    public static string Emoji_1f46b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f46b), culture);

    public static string Emoji_1f46e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f46e));

    public static string Emoji_1f46e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f46e), culture);

    public static string Emoji_1f46f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f46f));

    public static string Emoji_1f46f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f46f), culture);

    public static string Emoji_1f470() => RoosterJsEmojiResources.Get(nameof (Emoji_1f470));

    public static string Emoji_1f470(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f470), culture);

    public static string Emoji_1f471() => RoosterJsEmojiResources.Get(nameof (Emoji_1f471));

    public static string Emoji_1f471(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f471), culture);

    public static string Emoji_1f472() => RoosterJsEmojiResources.Get(nameof (Emoji_1f472));

    public static string Emoji_1f472(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f472), culture);

    public static string Emoji_1f473() => RoosterJsEmojiResources.Get(nameof (Emoji_1f473));

    public static string Emoji_1f473(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f473), culture);

    public static string Emoji_1f474() => RoosterJsEmojiResources.Get(nameof (Emoji_1f474));

    public static string Emoji_1f474(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f474), culture);

    public static string Emoji_1f475() => RoosterJsEmojiResources.Get(nameof (Emoji_1f475));

    public static string Emoji_1f475(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f475), culture);

    public static string Emoji_1f476() => RoosterJsEmojiResources.Get(nameof (Emoji_1f476));

    public static string Emoji_1f476(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f476), culture);

    public static string Emoji_1f477() => RoosterJsEmojiResources.Get(nameof (Emoji_1f477));

    public static string Emoji_1f477(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f477), culture);

    public static string Emoji_1f478() => RoosterJsEmojiResources.Get(nameof (Emoji_1f478));

    public static string Emoji_1f478(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f478), culture);

    public static string Emoji_1f479() => RoosterJsEmojiResources.Get(nameof (Emoji_1f479));

    public static string Emoji_1f479(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f479), culture);

    public static string Emoji_1f47a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f47a));

    public static string Emoji_1f47a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f47a), culture);

    public static string Emoji_1f47b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f47b));

    public static string Emoji_1f47b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f47b), culture);

    public static string Emoji_1f47c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f47c));

    public static string Emoji_1f47c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f47c), culture);

    public static string Emoji_1f47d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f47d));

    public static string Emoji_1f47d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f47d), culture);

    public static string Emoji_1f47e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f47e));

    public static string Emoji_1f47e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f47e), culture);

    public static string Emoji_1f47f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f47f));

    public static string Emoji_1f47f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f47f), culture);

    public static string Emoji_1f480() => RoosterJsEmojiResources.Get(nameof (Emoji_1f480));

    public static string Emoji_1f480(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f480), culture);

    public static string Emoji_1f481() => RoosterJsEmojiResources.Get(nameof (Emoji_1f481));

    public static string Emoji_1f481(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f481), culture);

    public static string Emoji_1f482() => RoosterJsEmojiResources.Get(nameof (Emoji_1f482));

    public static string Emoji_1f482(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f482), culture);

    public static string Emoji_1f483() => RoosterJsEmojiResources.Get(nameof (Emoji_1f483));

    public static string Emoji_1f483(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f483), culture);

    public static string Emoji_1f484() => RoosterJsEmojiResources.Get(nameof (Emoji_1f484));

    public static string Emoji_1f484(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f484), culture);

    public static string Emoji_1f485() => RoosterJsEmojiResources.Get(nameof (Emoji_1f485));

    public static string Emoji_1f485(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f485), culture);

    public static string Emoji_1f486() => RoosterJsEmojiResources.Get(nameof (Emoji_1f486));

    public static string Emoji_1f486(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f486), culture);

    public static string Emoji_1f487() => RoosterJsEmojiResources.Get(nameof (Emoji_1f487));

    public static string Emoji_1f487(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f487), culture);

    public static string Emoji_1f488() => RoosterJsEmojiResources.Get(nameof (Emoji_1f488));

    public static string Emoji_1f488(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f488), culture);

    public static string Emoji_1f489() => RoosterJsEmojiResources.Get(nameof (Emoji_1f489));

    public static string Emoji_1f489(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f489), culture);

    public static string Emoji_1f48a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f48a));

    public static string Emoji_1f48a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f48a), culture);

    public static string Emoji_1f48b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f48b));

    public static string Emoji_1f48b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f48b), culture);

    public static string Emoji_1f48c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f48c));

    public static string Emoji_1f48c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f48c), culture);

    public static string Emoji_1f48d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f48d));

    public static string Emoji_1f48d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f48d), culture);

    public static string Emoji_1f48e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f48e));

    public static string Emoji_1f48e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f48e), culture);

    public static string Emoji_1f48f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f48f));

    public static string Emoji_1f48f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f48f), culture);

    public static string Emoji_1f490() => RoosterJsEmojiResources.Get(nameof (Emoji_1f490));

    public static string Emoji_1f490(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f490), culture);

    public static string Emoji_1f491() => RoosterJsEmojiResources.Get(nameof (Emoji_1f491));

    public static string Emoji_1f491(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f491), culture);

    public static string Emoji_1f492() => RoosterJsEmojiResources.Get(nameof (Emoji_1f492));

    public static string Emoji_1f492(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f492), culture);

    public static string Emoji_1f493() => RoosterJsEmojiResources.Get(nameof (Emoji_1f493));

    public static string Emoji_1f493(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f493), culture);

    public static string Emoji_1f494() => RoosterJsEmojiResources.Get(nameof (Emoji_1f494));

    public static string Emoji_1f494(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f494), culture);

    public static string Emoji_1f495() => RoosterJsEmojiResources.Get(nameof (Emoji_1f495));

    public static string Emoji_1f495(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f495), culture);

    public static string Emoji_1f496() => RoosterJsEmojiResources.Get(nameof (Emoji_1f496));

    public static string Emoji_1f496(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f496), culture);

    public static string Emoji_1f497() => RoosterJsEmojiResources.Get(nameof (Emoji_1f497));

    public static string Emoji_1f497(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f497), culture);

    public static string Emoji_1f498() => RoosterJsEmojiResources.Get(nameof (Emoji_1f498));

    public static string Emoji_1f498(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f498), culture);

    public static string Emoji_1f499() => RoosterJsEmojiResources.Get(nameof (Emoji_1f499));

    public static string Emoji_1f499(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f499), culture);

    public static string Emoji_1f49a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f49a));

    public static string Emoji_1f49a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f49a), culture);

    public static string Emoji_1f49b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f49b));

    public static string Emoji_1f49b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f49b), culture);

    public static string Emoji_1f49c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f49c));

    public static string Emoji_1f49c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f49c), culture);

    public static string Emoji_1f49d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f49d));

    public static string Emoji_1f49d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f49d), culture);

    public static string Emoji_1f49e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f49e));

    public static string Emoji_1f49e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f49e), culture);

    public static string Emoji_1f49f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f49f));

    public static string Emoji_1f49f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f49f), culture);

    public static string Emoji_1f4a0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a0));

    public static string Emoji_1f4a0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a0), culture);

    public static string Emoji_1f4a1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a1));

    public static string Emoji_1f4a1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a1), culture);

    public static string Emoji_1f4a2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a2));

    public static string Emoji_1f4a2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a2), culture);

    public static string Emoji_1f4a3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a3));

    public static string Emoji_1f4a3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a3), culture);

    public static string Emoji_1f4a4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a4));

    public static string Emoji_1f4a4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a4), culture);

    public static string Emoji_1f4a5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a5));

    public static string Emoji_1f4a5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a5), culture);

    public static string Emoji_1f4a6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a6));

    public static string Emoji_1f4a6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a6), culture);

    public static string Emoji_1f4a7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a7));

    public static string Emoji_1f4a7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a7), culture);

    public static string Emoji_1f4a8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a8));

    public static string Emoji_1f4a8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a8), culture);

    public static string Emoji_1f4a9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a9));

    public static string Emoji_1f4a9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4a9), culture);

    public static string Emoji_1f4aa() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4aa));

    public static string Emoji_1f4aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4aa), culture);

    public static string Emoji_1f4ab() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ab));

    public static string Emoji_1f4ab(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ab), culture);

    public static string Emoji_1f4ac() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ac));

    public static string Emoji_1f4ac(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ac), culture);

    public static string Emoji_1f4ae() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ae));

    public static string Emoji_1f4ae(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ae), culture);

    public static string Emoji_1f4af() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4af));

    public static string Emoji_1f4af(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4af), culture);

    public static string Emoji_1f4b0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b0));

    public static string Emoji_1f4b0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b0), culture);

    public static string Emoji_1f4b1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b1));

    public static string Emoji_1f4b1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b1), culture);

    public static string Emoji_1f4b2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b2));

    public static string Emoji_1f4b2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b2), culture);

    public static string Emoji_1f4b3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b3));

    public static string Emoji_1f4b3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b3), culture);

    public static string Emoji_1f4b4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b4));

    public static string Emoji_1f4b4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b4), culture);

    public static string Emoji_1f4b5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b5));

    public static string Emoji_1f4b5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b5), culture);

    public static string Emoji_1f4b8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b8));

    public static string Emoji_1f4b8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b8), culture);

    public static string Emoji_1f4b9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b9));

    public static string Emoji_1f4b9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4b9), culture);

    public static string Emoji_1f4ba() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ba));

    public static string Emoji_1f4ba(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ba), culture);

    public static string Emoji_1f4bb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bb));

    public static string Emoji_1f4bb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bb), culture);

    public static string Emoji_1f4bc() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bc));

    public static string Emoji_1f4bc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bc), culture);

    public static string Emoji_1f4bd() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bd));

    public static string Emoji_1f4bd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bd), culture);

    public static string Emoji_1f4be() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4be));

    public static string Emoji_1f4be(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4be), culture);

    public static string Emoji_1f4bf() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bf));

    public static string Emoji_1f4bf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4bf), culture);

    public static string Emoji_1f4c0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c0));

    public static string Emoji_1f4c0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c0), culture);

    public static string Emoji_1f4c1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c1));

    public static string Emoji_1f4c1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c1), culture);

    public static string Emoji_1f4c2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c2));

    public static string Emoji_1f4c2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c2), culture);

    public static string Emoji_1f4c3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c3));

    public static string Emoji_1f4c3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c3), culture);

    public static string Emoji_1f4c4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c4));

    public static string Emoji_1f4c4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c4), culture);

    public static string Emoji_1f4c5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c5));

    public static string Emoji_1f4c5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c5), culture);

    public static string Emoji_1f4c6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c6));

    public static string Emoji_1f4c6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c6), culture);

    public static string Emoji_1f4c7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c7));

    public static string Emoji_1f4c7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c7), culture);

    public static string Emoji_1f4c8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c8));

    public static string Emoji_1f4c8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c8), culture);

    public static string Emoji_1f4c9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c9));

    public static string Emoji_1f4c9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4c9), culture);

    public static string Emoji_1f4ca() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ca));

    public static string Emoji_1f4ca(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ca), culture);

    public static string Emoji_1f4cb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cb));

    public static string Emoji_1f4cb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cb), culture);

    public static string Emoji_1f4cc() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cc));

    public static string Emoji_1f4cc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cc), culture);

    public static string Emoji_1f4cd() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cd));

    public static string Emoji_1f4cd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cd), culture);

    public static string Emoji_1f4ce() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ce));

    public static string Emoji_1f4ce(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ce), culture);

    public static string Emoji_1f4cf() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cf));

    public static string Emoji_1f4cf(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4cf), culture);

    public static string Emoji_1f4d0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d0));

    public static string Emoji_1f4d0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d0), culture);

    public static string Emoji_1f4d1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d1));

    public static string Emoji_1f4d1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d1), culture);

    public static string Emoji_1f4d2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d2));

    public static string Emoji_1f4d2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d2), culture);

    public static string Emoji_1f4d3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d3));

    public static string Emoji_1f4d3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d3), culture);

    public static string Emoji_1f4d4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d4));

    public static string Emoji_1f4d4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d4), culture);

    public static string Emoji_1f4d5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d5));

    public static string Emoji_1f4d5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d5), culture);

    public static string Emoji_1f4d6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d6));

    public static string Emoji_1f4d6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d6), culture);

    public static string Emoji_1f4d7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d7));

    public static string Emoji_1f4d7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d7), culture);

    public static string Emoji_1f4d8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d8));

    public static string Emoji_1f4d8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d8), culture);

    public static string Emoji_1f4d9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d9));

    public static string Emoji_1f4d9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4d9), culture);

    public static string Emoji_1f4da() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4da));

    public static string Emoji_1f4da(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4da), culture);

    public static string Emoji_1f4db() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4db));

    public static string Emoji_1f4db(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4db), culture);

    public static string Emoji_1f4dc() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4dc));

    public static string Emoji_1f4dc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4dc), culture);

    public static string Emoji_1f4dd() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4dd));

    public static string Emoji_1f4dd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4dd), culture);

    public static string Emoji_1f4de() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4de));

    public static string Emoji_1f4de(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4de), culture);

    public static string Emoji_1f4df() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4df));

    public static string Emoji_1f4df(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4df), culture);

    public static string Emoji_1f4e0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e0));

    public static string Emoji_1f4e0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e0), culture);

    public static string Emoji_1f4e1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e1));

    public static string Emoji_1f4e1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e1), culture);

    public static string Emoji_1f4e2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e2));

    public static string Emoji_1f4e2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e2), culture);

    public static string Emoji_1f4e3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e3));

    public static string Emoji_1f4e3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e3), culture);

    public static string Emoji_1f4e4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e4));

    public static string Emoji_1f4e4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e4), culture);

    public static string Emoji_1f4e5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e5));

    public static string Emoji_1f4e5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e5), culture);

    public static string Emoji_1f4e6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e6));

    public static string Emoji_1f4e6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e6), culture);

    public static string Emoji_1f4e7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e7));

    public static string Emoji_1f4e7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e7), culture);

    public static string Emoji_1f4e8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e8));

    public static string Emoji_1f4e8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e8), culture);

    public static string Emoji_1f4e9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e9));

    public static string Emoji_1f4e9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4e9), culture);

    public static string Emoji_1f4ea() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ea));

    public static string Emoji_1f4ea(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ea), culture);

    public static string Emoji_1f4eb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4eb));

    public static string Emoji_1f4eb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4eb), culture);

    public static string Emoji_1f4ee() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ee));

    public static string Emoji_1f4ee(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4ee), culture);

    public static string Emoji_1f4f0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f0));

    public static string Emoji_1f4f0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f0), culture);

    public static string Emoji_1f4f1() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f1));

    public static string Emoji_1f4f1(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f1), culture);

    public static string Emoji_1f4f2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f2));

    public static string Emoji_1f4f2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f2), culture);

    public static string Emoji_1f4f3() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f3));

    public static string Emoji_1f4f3(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f3), culture);

    public static string Emoji_1f4f4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f4));

    public static string Emoji_1f4f4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f4), culture);

    public static string Emoji_1f4f6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f6));

    public static string Emoji_1f4f6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f6), culture);

    public static string Emoji_1f4f7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f7));

    public static string Emoji_1f4f7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f7), culture);

    public static string Emoji_1f4f9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f9));

    public static string Emoji_1f4f9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4f9), culture);

    public static string Emoji_1f4fa() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4fa));

    public static string Emoji_1f4fa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4fa), culture);

    public static string Emoji_1f4fb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4fb));

    public static string Emoji_1f4fb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4fb), culture);

    public static string Emoji_1f4fc() => RoosterJsEmojiResources.Get(nameof (Emoji_1f4fc));

    public static string Emoji_1f4fc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f4fc), culture);

    public static string Emoji_1f503() => RoosterJsEmojiResources.Get(nameof (Emoji_1f503));

    public static string Emoji_1f503(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f503), culture);

    public static string Emoji_1f50a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f50a));

    public static string Emoji_1f50a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f50a), culture);

    public static string Emoji_1f50b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f50b));

    public static string Emoji_1f50b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f50b), culture);

    public static string Emoji_1f50c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f50c));

    public static string Emoji_1f50c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f50c), culture);

    public static string Emoji_1f50d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f50d));

    public static string Emoji_1f50d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f50d), culture);

    public static string Emoji_1f50e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f50e));

    public static string Emoji_1f50e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f50e), culture);

    public static string Emoji_1f50f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f50f));

    public static string Emoji_1f50f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f50f), culture);

    public static string Emoji_1f510() => RoosterJsEmojiResources.Get(nameof (Emoji_1f510));

    public static string Emoji_1f510(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f510), culture);

    public static string Emoji_1f511() => RoosterJsEmojiResources.Get(nameof (Emoji_1f511));

    public static string Emoji_1f511(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f511), culture);

    public static string Emoji_1f512() => RoosterJsEmojiResources.Get(nameof (Emoji_1f512));

    public static string Emoji_1f512(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f512), culture);

    public static string Emoji_1f513() => RoosterJsEmojiResources.Get(nameof (Emoji_1f513));

    public static string Emoji_1f513(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f513), culture);

    public static string Emoji_1f514() => RoosterJsEmojiResources.Get(nameof (Emoji_1f514));

    public static string Emoji_1f514(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f514), culture);

    public static string Emoji_1f516() => RoosterJsEmojiResources.Get(nameof (Emoji_1f516));

    public static string Emoji_1f516(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f516), culture);

    public static string Emoji_1f517() => RoosterJsEmojiResources.Get(nameof (Emoji_1f517));

    public static string Emoji_1f517(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f517), culture);

    public static string Emoji_1f518() => RoosterJsEmojiResources.Get(nameof (Emoji_1f518));

    public static string Emoji_1f518(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f518), culture);

    public static string Emoji_1f519() => RoosterJsEmojiResources.Get(nameof (Emoji_1f519));

    public static string Emoji_1f519(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f519), culture);

    public static string Emoji_1f51a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f51a));

    public static string Emoji_1f51a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f51a), culture);

    public static string Emoji_1f51b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f51b));

    public static string Emoji_1f51b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f51b), culture);

    public static string Emoji_1f51c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f51c));

    public static string Emoji_1f51c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f51c), culture);

    public static string Emoji_1f51d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f51d));

    public static string Emoji_1f51d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f51d), culture);

    public static string Emoji_1f51e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f51e));

    public static string Emoji_1f51e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f51e), culture);

    public static string Emoji_1f51f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f51f));

    public static string Emoji_1f51f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f51f), culture);

    public static string Emoji_1f520() => RoosterJsEmojiResources.Get(nameof (Emoji_1f520));

    public static string Emoji_1f520(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f520), culture);

    public static string Emoji_1f521() => RoosterJsEmojiResources.Get(nameof (Emoji_1f521));

    public static string Emoji_1f521(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f521), culture);

    public static string Emoji_1f522() => RoosterJsEmojiResources.Get(nameof (Emoji_1f522));

    public static string Emoji_1f522(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f522), culture);

    public static string Emoji_1f523() => RoosterJsEmojiResources.Get(nameof (Emoji_1f523));

    public static string Emoji_1f523(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f523), culture);

    public static string Emoji_1f524() => RoosterJsEmojiResources.Get(nameof (Emoji_1f524));

    public static string Emoji_1f524(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f524), culture);

    public static string Emoji_1f525() => RoosterJsEmojiResources.Get(nameof (Emoji_1f525));

    public static string Emoji_1f525(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f525), culture);

    public static string Emoji_1f526() => RoosterJsEmojiResources.Get(nameof (Emoji_1f526));

    public static string Emoji_1f526(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f526), culture);

    public static string Emoji_1f527() => RoosterJsEmojiResources.Get(nameof (Emoji_1f527));

    public static string Emoji_1f527(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f527), culture);

    public static string Emoji_1f528() => RoosterJsEmojiResources.Get(nameof (Emoji_1f528));

    public static string Emoji_1f528(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f528), culture);

    public static string Emoji_1f529() => RoosterJsEmojiResources.Get(nameof (Emoji_1f529));

    public static string Emoji_1f529(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f529), culture);

    public static string Emoji_1f52a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f52a));

    public static string Emoji_1f52a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f52a), culture);

    public static string Emoji_1f52b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f52b));

    public static string Emoji_1f52b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f52b), culture);

    public static string Emoji_1f52e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f52e));

    public static string Emoji_1f52e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f52e), culture);

    public static string Emoji_1f52f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f52f));

    public static string Emoji_1f52f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f52f), culture);

    public static string Emoji_1f530() => RoosterJsEmojiResources.Get(nameof (Emoji_1f530));

    public static string Emoji_1f530(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f530), culture);

    public static string Emoji_1f531() => RoosterJsEmojiResources.Get(nameof (Emoji_1f531));

    public static string Emoji_1f531(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f531), culture);

    public static string Emoji_1f532() => RoosterJsEmojiResources.Get(nameof (Emoji_1f532));

    public static string Emoji_1f532(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f532), culture);

    public static string Emoji_1f533() => RoosterJsEmojiResources.Get(nameof (Emoji_1f533));

    public static string Emoji_1f533(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f533), culture);

    public static string Emoji_1f534() => RoosterJsEmojiResources.Get(nameof (Emoji_1f534));

    public static string Emoji_1f534(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f534), culture);

    public static string Emoji_1f535() => RoosterJsEmojiResources.Get(nameof (Emoji_1f535));

    public static string Emoji_1f535(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f535), culture);

    public static string Emoji_1f536() => RoosterJsEmojiResources.Get(nameof (Emoji_1f536));

    public static string Emoji_1f536(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f536), culture);

    public static string Emoji_1f537() => RoosterJsEmojiResources.Get(nameof (Emoji_1f537));

    public static string Emoji_1f537(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f537), culture);

    public static string Emoji_1f538() => RoosterJsEmojiResources.Get(nameof (Emoji_1f538));

    public static string Emoji_1f538(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f538), culture);

    public static string Emoji_1f539() => RoosterJsEmojiResources.Get(nameof (Emoji_1f539));

    public static string Emoji_1f539(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f539), culture);

    public static string Emoji_1f53a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f53a));

    public static string Emoji_1f53a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f53a), culture);

    public static string Emoji_1f53b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f53b));

    public static string Emoji_1f53b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f53b), culture);

    public static string Emoji_1f53c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f53c));

    public static string Emoji_1f53c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f53c), culture);

    public static string Emoji_1f53d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f53d));

    public static string Emoji_1f53d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f53d), culture);

    public static string Emoji_1f550() => RoosterJsEmojiResources.Get(nameof (Emoji_1f550));

    public static string Emoji_1f550(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f550), culture);

    public static string Emoji_1f551() => RoosterJsEmojiResources.Get(nameof (Emoji_1f551));

    public static string Emoji_1f551(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f551), culture);

    public static string Emoji_1f552() => RoosterJsEmojiResources.Get(nameof (Emoji_1f552));

    public static string Emoji_1f552(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f552), culture);

    public static string Emoji_1f553() => RoosterJsEmojiResources.Get(nameof (Emoji_1f553));

    public static string Emoji_1f553(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f553), culture);

    public static string Emoji_1f554() => RoosterJsEmojiResources.Get(nameof (Emoji_1f554));

    public static string Emoji_1f554(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f554), culture);

    public static string Emoji_1f555() => RoosterJsEmojiResources.Get(nameof (Emoji_1f555));

    public static string Emoji_1f555(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f555), culture);

    public static string Emoji_1f556() => RoosterJsEmojiResources.Get(nameof (Emoji_1f556));

    public static string Emoji_1f556(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f556), culture);

    public static string Emoji_1f557() => RoosterJsEmojiResources.Get(nameof (Emoji_1f557));

    public static string Emoji_1f557(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f557), culture);

    public static string Emoji_1f558() => RoosterJsEmojiResources.Get(nameof (Emoji_1f558));

    public static string Emoji_1f558(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f558), culture);

    public static string Emoji_1f559() => RoosterJsEmojiResources.Get(nameof (Emoji_1f559));

    public static string Emoji_1f559(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f559), culture);

    public static string Emoji_1f55a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f55a));

    public static string Emoji_1f55a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f55a), culture);

    public static string Emoji_1f55b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f55b));

    public static string Emoji_1f55b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f55b), culture);

    public static string Emoji_1f590() => RoosterJsEmojiResources.Get(nameof (Emoji_1f590));

    public static string Emoji_1f590(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f590), culture);

    public static string Emoji_1f595() => RoosterJsEmojiResources.Get(nameof (Emoji_1f595));

    public static string Emoji_1f595(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f595), culture);

    public static string Emoji_1f596() => RoosterJsEmojiResources.Get(nameof (Emoji_1f596));

    public static string Emoji_1f596(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f596), culture);

    public static string Emoji_1f5fb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fb));

    public static string Emoji_1f5fb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fb), culture);

    public static string Emoji_1f5fc() => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fc));

    public static string Emoji_1f5fc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fc), culture);

    public static string Emoji_1f5fd() => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fd));

    public static string Emoji_1f5fd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fd), culture);

    public static string Emoji_1f5fe() => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fe));

    public static string Emoji_1f5fe(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f5fe), culture);

    public static string Emoji_1f5ff() => RoosterJsEmojiResources.Get(nameof (Emoji_1f5ff));

    public static string Emoji_1f5ff(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f5ff), culture);

    public static string Emoji_1f601() => RoosterJsEmojiResources.Get(nameof (Emoji_1f601));

    public static string Emoji_1f601(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f601), culture);

    public static string Emoji_1f602() => RoosterJsEmojiResources.Get(nameof (Emoji_1f602));

    public static string Emoji_1f602(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f602), culture);

    public static string Emoji_1f603() => RoosterJsEmojiResources.Get(nameof (Emoji_1f603));

    public static string Emoji_1f603(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f603), culture);

    public static string Emoji_1f604() => RoosterJsEmojiResources.Get(nameof (Emoji_1f604));

    public static string Emoji_1f604(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f604), culture);

    public static string Emoji_1f605() => RoosterJsEmojiResources.Get(nameof (Emoji_1f605));

    public static string Emoji_1f605(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f605), culture);

    public static string Emoji_1f606() => RoosterJsEmojiResources.Get(nameof (Emoji_1f606));

    public static string Emoji_1f606(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f606), culture);

    public static string Emoji_1f607() => RoosterJsEmojiResources.Get(nameof (Emoji_1f607));

    public static string Emoji_1f607(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f607), culture);

    public static string Emoji_1f608() => RoosterJsEmojiResources.Get(nameof (Emoji_1f608));

    public static string Emoji_1f608(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f608), culture);

    public static string Emoji_1f609() => RoosterJsEmojiResources.Get(nameof (Emoji_1f609));

    public static string Emoji_1f609(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f609), culture);

    public static string Emoji_1f60a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f60a));

    public static string Emoji_1f60a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f60a), culture);

    public static string Emoji_1f60b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f60b));

    public static string Emoji_1f60b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f60b), culture);

    public static string Emoji_1f60c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f60c));

    public static string Emoji_1f60c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f60c), culture);

    public static string Emoji_1f60d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f60d));

    public static string Emoji_1f60d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f60d), culture);

    public static string Emoji_1f60e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f60e));

    public static string Emoji_1f60e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f60e), culture);

    public static string Emoji_1f60f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f60f));

    public static string Emoji_1f60f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f60f), culture);

    public static string Emoji_1f610() => RoosterJsEmojiResources.Get(nameof (Emoji_1f610));

    public static string Emoji_1f610(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f610), culture);

    public static string Emoji_1f612() => RoosterJsEmojiResources.Get(nameof (Emoji_1f612));

    public static string Emoji_1f612(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f612), culture);

    public static string Emoji_1f613() => RoosterJsEmojiResources.Get(nameof (Emoji_1f613));

    public static string Emoji_1f613(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f613), culture);

    public static string Emoji_1f614() => RoosterJsEmojiResources.Get(nameof (Emoji_1f614));

    public static string Emoji_1f614(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f614), culture);

    public static string Emoji_1f616() => RoosterJsEmojiResources.Get(nameof (Emoji_1f616));

    public static string Emoji_1f616(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f616), culture);

    public static string Emoji_1f618() => RoosterJsEmojiResources.Get(nameof (Emoji_1f618));

    public static string Emoji_1f618(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f618), culture);

    public static string Emoji_1f61a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f61a));

    public static string Emoji_1f61a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f61a), culture);

    public static string Emoji_1f61c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f61c));

    public static string Emoji_1f61c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f61c), culture);

    public static string Emoji_1f61d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f61d));

    public static string Emoji_1f61d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f61d), culture);

    public static string Emoji_1f61e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f61e));

    public static string Emoji_1f61e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f61e), culture);

    public static string Emoji_1f620() => RoosterJsEmojiResources.Get(nameof (Emoji_1f620));

    public static string Emoji_1f620(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f620), culture);

    public static string Emoji_1f621() => RoosterJsEmojiResources.Get(nameof (Emoji_1f621));

    public static string Emoji_1f621(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f621), culture);

    public static string Emoji_1f622() => RoosterJsEmojiResources.Get(nameof (Emoji_1f622));

    public static string Emoji_1f622(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f622), culture);

    public static string Emoji_1f623() => RoosterJsEmojiResources.Get(nameof (Emoji_1f623));

    public static string Emoji_1f623(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f623), culture);

    public static string Emoji_1f624() => RoosterJsEmojiResources.Get(nameof (Emoji_1f624));

    public static string Emoji_1f624(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f624), culture);

    public static string Emoji_1f625() => RoosterJsEmojiResources.Get(nameof (Emoji_1f625));

    public static string Emoji_1f625(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f625), culture);

    public static string Emoji_1f628() => RoosterJsEmojiResources.Get(nameof (Emoji_1f628));

    public static string Emoji_1f628(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f628), culture);

    public static string Emoji_1f629() => RoosterJsEmojiResources.Get(nameof (Emoji_1f629));

    public static string Emoji_1f629(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f629), culture);

    public static string Emoji_1f62a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f62a));

    public static string Emoji_1f62a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f62a), culture);

    public static string Emoji_1f62b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f62b));

    public static string Emoji_1f62b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f62b), culture);

    public static string Emoji_1f62d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f62d));

    public static string Emoji_1f62d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f62d), culture);

    public static string Emoji_1f630() => RoosterJsEmojiResources.Get(nameof (Emoji_1f630));

    public static string Emoji_1f630(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f630), culture);

    public static string Emoji_1f631() => RoosterJsEmojiResources.Get(nameof (Emoji_1f631));

    public static string Emoji_1f631(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f631), culture);

    public static string Emoji_1f632() => RoosterJsEmojiResources.Get(nameof (Emoji_1f632));

    public static string Emoji_1f632(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f632), culture);

    public static string Emoji_1f633() => RoosterJsEmojiResources.Get(nameof (Emoji_1f633));

    public static string Emoji_1f633(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f633), culture);

    public static string Emoji_1f635() => RoosterJsEmojiResources.Get(nameof (Emoji_1f635));

    public static string Emoji_1f635(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f635), culture);

    public static string Emoji_1f636() => RoosterJsEmojiResources.Get(nameof (Emoji_1f636));

    public static string Emoji_1f636(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f636), culture);

    public static string Emoji_1f637() => RoosterJsEmojiResources.Get(nameof (Emoji_1f637));

    public static string Emoji_1f637(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f637), culture);

    public static string Emoji_1f638() => RoosterJsEmojiResources.Get(nameof (Emoji_1f638));

    public static string Emoji_1f638(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f638), culture);

    public static string Emoji_1f639() => RoosterJsEmojiResources.Get(nameof (Emoji_1f639));

    public static string Emoji_1f639(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f639), culture);

    public static string Emoji_1f63a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f63a));

    public static string Emoji_1f63a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f63a), culture);

    public static string Emoji_1f63b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f63b));

    public static string Emoji_1f63b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f63b), culture);

    public static string Emoji_1f63c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f63c));

    public static string Emoji_1f63c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f63c), culture);

    public static string Emoji_1f63d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f63d));

    public static string Emoji_1f63d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f63d), culture);

    public static string Emoji_1f63e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f63e));

    public static string Emoji_1f63e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f63e), culture);

    public static string Emoji_1f63f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f63f));

    public static string Emoji_1f63f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f63f), culture);

    public static string Emoji_1f640() => RoosterJsEmojiResources.Get(nameof (Emoji_1f640));

    public static string Emoji_1f640(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f640), culture);

    public static string Emoji_1f641() => RoosterJsEmojiResources.Get(nameof (Emoji_1f641));

    public static string Emoji_1f641(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f641), culture);

    public static string Emoji_1f642() => RoosterJsEmojiResources.Get(nameof (Emoji_1f642));

    public static string Emoji_1f642(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f642), culture);

    public static string Emoji_1f645() => RoosterJsEmojiResources.Get(nameof (Emoji_1f645));

    public static string Emoji_1f645(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f645), culture);

    public static string Emoji_1f646() => RoosterJsEmojiResources.Get(nameof (Emoji_1f646));

    public static string Emoji_1f646(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f646), culture);

    public static string Emoji_1f647() => RoosterJsEmojiResources.Get(nameof (Emoji_1f647));

    public static string Emoji_1f647(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f647), culture);

    public static string Emoji_1f648() => RoosterJsEmojiResources.Get(nameof (Emoji_1f648));

    public static string Emoji_1f648(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f648), culture);

    public static string Emoji_1f649() => RoosterJsEmojiResources.Get(nameof (Emoji_1f649));

    public static string Emoji_1f649(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f649), culture);

    public static string Emoji_1f64a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f64a));

    public static string Emoji_1f64a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f64a), culture);

    public static string Emoji_1f64b() => RoosterJsEmojiResources.Get(nameof (Emoji_1f64b));

    public static string Emoji_1f64b(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f64b), culture);

    public static string Emoji_1f64c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f64c));

    public static string Emoji_1f64c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f64c), culture);

    public static string Emoji_1f64d() => RoosterJsEmojiResources.Get(nameof (Emoji_1f64d));

    public static string Emoji_1f64d(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f64d), culture);

    public static string Emoji_1f64e() => RoosterJsEmojiResources.Get(nameof (Emoji_1f64e));

    public static string Emoji_1f64e(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f64e), culture);

    public static string Emoji_1f64f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f64f));

    public static string Emoji_1f64f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f64f), culture);

    public static string Emoji_1f680() => RoosterJsEmojiResources.Get(nameof (Emoji_1f680));

    public static string Emoji_1f680(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f680), culture);

    public static string Emoji_1f683() => RoosterJsEmojiResources.Get(nameof (Emoji_1f683));

    public static string Emoji_1f683(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f683), culture);

    public static string Emoji_1f684() => RoosterJsEmojiResources.Get(nameof (Emoji_1f684));

    public static string Emoji_1f684(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f684), culture);

    public static string Emoji_1f685() => RoosterJsEmojiResources.Get(nameof (Emoji_1f685));

    public static string Emoji_1f685(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f685), culture);

    public static string Emoji_1f687() => RoosterJsEmojiResources.Get(nameof (Emoji_1f687));

    public static string Emoji_1f687(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f687), culture);

    public static string Emoji_1f689() => RoosterJsEmojiResources.Get(nameof (Emoji_1f689));

    public static string Emoji_1f689(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f689), culture);

    public static string Emoji_1f68c() => RoosterJsEmojiResources.Get(nameof (Emoji_1f68c));

    public static string Emoji_1f68c(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f68c), culture);

    public static string Emoji_1f68f() => RoosterJsEmojiResources.Get(nameof (Emoji_1f68f));

    public static string Emoji_1f68f(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f68f), culture);

    public static string Emoji_1f691() => RoosterJsEmojiResources.Get(nameof (Emoji_1f691));

    public static string Emoji_1f691(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f691), culture);

    public static string Emoji_1f692() => RoosterJsEmojiResources.Get(nameof (Emoji_1f692));

    public static string Emoji_1f692(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f692), culture);

    public static string Emoji_1f693() => RoosterJsEmojiResources.Get(nameof (Emoji_1f693));

    public static string Emoji_1f693(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f693), culture);

    public static string Emoji_1f695() => RoosterJsEmojiResources.Get(nameof (Emoji_1f695));

    public static string Emoji_1f695(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f695), culture);

    public static string Emoji_1f697() => RoosterJsEmojiResources.Get(nameof (Emoji_1f697));

    public static string Emoji_1f697(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f697), culture);

    public static string Emoji_1f699() => RoosterJsEmojiResources.Get(nameof (Emoji_1f699));

    public static string Emoji_1f699(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f699), culture);

    public static string Emoji_1f69a() => RoosterJsEmojiResources.Get(nameof (Emoji_1f69a));

    public static string Emoji_1f69a(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f69a), culture);

    public static string Emoji_1f6a2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a2));

    public static string Emoji_1f6a2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a2), culture);

    public static string Emoji_1f6a4() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a4));

    public static string Emoji_1f6a4(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a4), culture);

    public static string Emoji_1f6a5() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a5));

    public static string Emoji_1f6a5(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a5), culture);

    public static string Emoji_1f6a7() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a7));

    public static string Emoji_1f6a7(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a7), culture);

    public static string Emoji_1f6a8() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a8));

    public static string Emoji_1f6a8(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a8), culture);

    public static string Emoji_1f6a9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a9));

    public static string Emoji_1f6a9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6a9), culture);

    public static string Emoji_1f6aa() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6aa));

    public static string Emoji_1f6aa(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6aa), culture);

    public static string Emoji_1f6ab() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ab));

    public static string Emoji_1f6ab(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ab), culture);

    public static string Emoji_1f6ac() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ac));

    public static string Emoji_1f6ac(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ac), culture);

    public static string Emoji_1f6ad() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ad));

    public static string Emoji_1f6ad(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ad), culture);

    public static string Emoji_1f6b2() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6b2));

    public static string Emoji_1f6b2(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6b2), culture);

    public static string Emoji_1f6b6() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6b6));

    public static string Emoji_1f6b6(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6b6), culture);

    public static string Emoji_1f6b9() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6b9));

    public static string Emoji_1f6b9(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6b9), culture);

    public static string Emoji_1f6ba() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ba));

    public static string Emoji_1f6ba(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6ba), culture);

    public static string Emoji_1f6bb() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6bb));

    public static string Emoji_1f6bb(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6bb), culture);

    public static string Emoji_1f6bc() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6bc));

    public static string Emoji_1f6bc(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6bc), culture);

    public static string Emoji_1f6bd() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6bd));

    public static string Emoji_1f6bd(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6bd), culture);

    public static string Emoji_1f6be() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6be));

    public static string Emoji_1f6be(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6be), culture);

    public static string Emoji_1f6c0() => RoosterJsEmojiResources.Get(nameof (Emoji_1f6c0));

    public static string Emoji_1f6c0(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_1f6c0), culture);

    public static string Emoji_More() => RoosterJsEmojiResources.Get(nameof (Emoji_More));

    public static string Emoji_More(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (Emoji_More), culture);

    public static string EmojiFamily_Activities() => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Activities));

    public static string EmojiFamily_Activities(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Activities), culture);

    public static string EmojiFamily_Food() => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Food));

    public static string EmojiFamily_Food(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Food), culture);

    public static string EmojiFamily_Nature() => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Nature));

    public static string EmojiFamily_Nature(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Nature), culture);

    public static string EmojiFamily_Objects() => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Objects));

    public static string EmojiFamily_Objects(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Objects), culture);

    public static string EmojiFamily_People() => RoosterJsEmojiResources.Get(nameof (EmojiFamily_People));

    public static string EmojiFamily_People(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiFamily_People), culture);

    public static string EmojiFamily_Symbols() => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Symbols));

    public static string EmojiFamily_Symbols(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Symbols), culture);

    public static string EmojiFamily_Travel() => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Travel));

    public static string EmojiFamily_Travel(CultureInfo culture) => RoosterJsEmojiResources.Get(nameof (EmojiFamily_Travel), culture);
  }
}
