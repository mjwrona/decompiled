// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Milestone
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class Milestone
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (Milestone), typeof (Milestone).GetTypeInfo().Assembly);

    public static ResourceManager Manager => Milestone.s_resMgr;

    private static string Get(string resourceName) => Milestone.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? Milestone.Get(resourceName) : Milestone.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) Milestone.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? Milestone.GetInt(resourceName) : (int) Milestone.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) Milestone.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? Milestone.GetBool(resourceName) : (bool) Milestone.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => Milestone.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = Milestone.Get(resourceName, culture);
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

    public static string Tfs2010Beta1Desc() => Milestone.Get(nameof (Tfs2010Beta1Desc));

    public static string Tfs2010Beta1Desc(CultureInfo culture) => Milestone.Get(nameof (Tfs2010Beta1Desc), culture);

    public static string Tfs2010Beta2RCDesc() => Milestone.Get(nameof (Tfs2010Beta2RCDesc));

    public static string Tfs2010Beta2RCDesc(CultureInfo culture) => Milestone.Get(nameof (Tfs2010Beta2RCDesc), culture);

    public static string Tfs2010Beta2RC2Desc() => Milestone.Get(nameof (Tfs2010Beta2RC2Desc));

    public static string Tfs2010Beta2RC2Desc(CultureInfo culture) => Milestone.Get(nameof (Tfs2010Beta2RC2Desc), culture);

    public static string Tfs2010Beta2Desc() => Milestone.Get(nameof (Tfs2010Beta2Desc));

    public static string Tfs2010Beta2Desc(CultureInfo culture) => Milestone.Get(nameof (Tfs2010Beta2Desc), culture);

    public static string Tfs2010RTMDesc() => Milestone.Get(nameof (Tfs2010RTMDesc));

    public static string Tfs2010RTMDesc(CultureInfo culture) => Milestone.Get(nameof (Tfs2010RTMDesc), culture);

    public static string Tfs2010SP1Desc() => Milestone.Get(nameof (Tfs2010SP1Desc));

    public static string Tfs2010SP1Desc(CultureInfo culture) => Milestone.Get(nameof (Tfs2010SP1Desc), culture);

    public static string Dev11CTP1Desc() => Milestone.Get(nameof (Dev11CTP1Desc));

    public static string Dev11CTP1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11CTP1Desc), culture);

    public static string Dev11CTP2Desc() => Milestone.Get(nameof (Dev11CTP2Desc));

    public static string Dev11CTP2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11CTP2Desc), culture);

    public static string Dev11CTP3Desc() => Milestone.Get(nameof (Dev11CTP3Desc));

    public static string Dev11CTP3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11CTP3Desc), culture);

    public static string Dev11Beta1Desc() => Milestone.Get(nameof (Dev11Beta1Desc));

    public static string Dev11Beta1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11Beta1Desc), culture);

    public static string Dev11Beta2Desc() => Milestone.Get(nameof (Dev11Beta2Desc));

    public static string Dev11Beta2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11Beta2Desc), culture);

    public static string Dev11Beta3Desc() => Milestone.Get(nameof (Dev11Beta3Desc));

    public static string Dev11Beta3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11Beta3Desc), culture);

    public static string Dev11Beta4Desc() => Milestone.Get(nameof (Dev11Beta4Desc));

    public static string Dev11Beta4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11Beta4Desc), culture);

    public static string Dev11Beta5Desc() => Milestone.Get(nameof (Dev11Beta5Desc));

    public static string Dev11Beta5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11Beta5Desc), culture);

    public static string Dev11RC1Desc() => Milestone.Get(nameof (Dev11RC1Desc));

    public static string Dev11RC1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11RC1Desc), culture);

    public static string Dev11RC2Desc() => Milestone.Get(nameof (Dev11RC2Desc));

    public static string Dev11RC2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11RC2Desc), culture);

    public static string Dev11RTMDesc() => Milestone.Get(nameof (Dev11RTMDesc));

    public static string Dev11RTMDesc(CultureInfo culture) => Milestone.Get(nameof (Dev11RTMDesc), culture);

    public static string Dev11M38QU1Desc() => Milestone.Get(nameof (Dev11M38QU1Desc));

    public static string Dev11M38QU1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M38QU1Desc), culture);

    public static string Dev11M38QU1P1Desc() => Milestone.Get(nameof (Dev11M38QU1P1Desc));

    public static string Dev11M38QU1P1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M38QU1P1Desc), culture);

    public static string Dev11M38QU1P2Desc() => Milestone.Get(nameof (Dev11M38QU1P2Desc));

    public static string Dev11M38QU1P2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M38QU1P2Desc), culture);

    public static string Dev11M42Desc() => Milestone.Get(nameof (Dev11M42Desc));

    public static string Dev11M42Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M42Desc), culture);

    public static string Dev11M43Desc() => Milestone.Get(nameof (Dev11M43Desc));

    public static string Dev11M43Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M43Desc), culture);

    public static string Dev11M44Desc() => Milestone.Get(nameof (Dev11M44Desc));

    public static string Dev11M44Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M44Desc), culture);

    public static string Dev11M44QU2Desc() => Milestone.Get(nameof (Dev11M44QU2Desc));

    public static string Dev11M44QU2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M44QU2Desc), culture);

    public static string Dev11M45Desc() => Milestone.Get(nameof (Dev11M45Desc));

    public static string Dev11M45Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M45Desc), culture);

    public static string Dev11M46Desc() => Milestone.Get(nameof (Dev11M46Desc));

    public static string Dev11M46Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M46Desc), culture);

    public static string Dev11M47Desc() => Milestone.Get(nameof (Dev11M47Desc));

    public static string Dev11M47Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M47Desc), culture);

    public static string Dev11M48Desc() => Milestone.Get(nameof (Dev11M48Desc));

    public static string Dev11M48Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M48Desc), culture);

    public static string Dev11M49Desc() => Milestone.Get(nameof (Dev11M49Desc));

    public static string Dev11M49Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M49Desc), culture);

    public static string Dev11M51Desc() => Milestone.Get(nameof (Dev11M51Desc));

    public static string Dev11M51Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M51Desc), culture);

    public static string Dev11M52Desc() => Milestone.Get(nameof (Dev11M52Desc));

    public static string Dev11M52Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M52Desc), culture);

    public static string Dev11M53Desc() => Milestone.Get(nameof (Dev11M53Desc));

    public static string Dev11M53Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M53Desc), culture);

    public static string Dev11M54Desc() => Milestone.Get(nameof (Dev11M54Desc));

    public static string Dev11M54Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M54Desc), culture);

    public static string Dev11M55Desc() => Milestone.Get(nameof (Dev11M55Desc));

    public static string Dev11M55Desc(CultureInfo culture) => Milestone.Get(nameof (Dev11M55Desc), culture);

    public static string Dev12M45Desc() => Milestone.Get(nameof (Dev12M45Desc));

    public static string Dev12M45Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M45Desc), culture);

    public static string Dev12M46Desc() => Milestone.Get(nameof (Dev12M46Desc));

    public static string Dev12M46Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M46Desc), culture);

    public static string Dev12M47Desc() => Milestone.Get(nameof (Dev12M47Desc));

    public static string Dev12M47Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M47Desc), culture);

    public static string Dev12M48Desc() => Milestone.Get(nameof (Dev12M48Desc));

    public static string Dev12M48Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M48Desc), culture);

    public static string Dev12M48Part2Desc() => Milestone.Get(nameof (Dev12M48Part2Desc));

    public static string Dev12M48Part2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M48Part2Desc), culture);

    public static string Dev12M49Desc() => Milestone.Get(nameof (Dev12M49Desc));

    public static string Dev12M49Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M49Desc), culture);

    public static string Dev12M50Desc() => Milestone.Get(nameof (Dev12M50Desc));

    public static string Dev12M50Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M50Desc), culture);

    public static string Dev12M51Desc() => Milestone.Get(nameof (Dev12M51Desc));

    public static string Dev12M51Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M51Desc), culture);

    public static string Dev12M52Desc() => Milestone.Get(nameof (Dev12M52Desc));

    public static string Dev12M52Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M52Desc), culture);

    public static string Dev12M53Desc() => Milestone.Get(nameof (Dev12M53Desc));

    public static string Dev12M53Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M53Desc), culture);

    public static string Dev12M54Desc() => Milestone.Get(nameof (Dev12M54Desc));

    public static string Dev12M54Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M54Desc), culture);

    public static string Dev12M55Desc() => Milestone.Get(nameof (Dev12M55Desc));

    public static string Dev12M55Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M55Desc), culture);

    public static string Dev12M56Desc() => Milestone.Get(nameof (Dev12M56Desc));

    public static string Dev12M56Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M56Desc), culture);

    public static string Dev12M57Desc() => Milestone.Get(nameof (Dev12M57Desc));

    public static string Dev12M57Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M57Desc), culture);

    public static string Dev12M58Desc() => Milestone.Get(nameof (Dev12M58Desc));

    public static string Dev12M58Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M58Desc), culture);

    public static string Dev12M60Desc() => Milestone.Get(nameof (Dev12M60Desc));

    public static string Dev12M60Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M60Desc), culture);

    public static string Dev12M61Desc() => Milestone.Get(nameof (Dev12M61Desc));

    public static string Dev12M61Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M61Desc), culture);

    public static string Dev12M62Desc() => Milestone.Get(nameof (Dev12M62Desc));

    public static string Dev12M62Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M62Desc), culture);

    public static string Dev12M65Desc() => Milestone.Get(nameof (Dev12M65Desc));

    public static string Dev12M65Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M65Desc), culture);

    public static string Dev12M66Desc() => Milestone.Get(nameof (Dev12M66Desc));

    public static string Dev12M66Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M66Desc), culture);

    public static string Dev12M67Desc() => Milestone.Get(nameof (Dev12M67Desc));

    public static string Dev12M67Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M67Desc), culture);

    public static string Dev12M68Desc() => Milestone.Get(nameof (Dev12M68Desc));

    public static string Dev12M68Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M68Desc), culture);

    public static string Dev12M69Desc() => Milestone.Get(nameof (Dev12M69Desc));

    public static string Dev12M69Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M69Desc), culture);

    public static string Dev12M70Desc() => Milestone.Get(nameof (Dev12M70Desc));

    public static string Dev12M70Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M70Desc), culture);

    public static string Dev12M71Desc() => Milestone.Get(nameof (Dev12M71Desc));

    public static string Dev12M71Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M71Desc), culture);

    public static string Dev12M72Desc() => Milestone.Get(nameof (Dev12M72Desc));

    public static string Dev12M72Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M72Desc), culture);

    public static string Dev12M73Desc() => Milestone.Get(nameof (Dev12M73Desc));

    public static string Dev12M73Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M73Desc), culture);

    public static string Dev12M77Desc() => Milestone.Get(nameof (Dev12M77Desc));

    public static string Dev12M77Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M77Desc), culture);

    public static string Dev12M83Desc() => Milestone.Get(nameof (Dev12M83Desc));

    public static string Dev12M83Desc(CultureInfo culture) => Milestone.Get(nameof (Dev12M83Desc), culture);

    public static string Dev14M62Desc() => Milestone.Get(nameof (Dev14M62Desc));

    public static string Dev14M62Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M62Desc), culture);

    public static string Dev14M63Desc() => Milestone.Get(nameof (Dev14M63Desc));

    public static string Dev14M63Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M63Desc), culture);

    public static string Dev14M64Desc() => Milestone.Get(nameof (Dev14M64Desc));

    public static string Dev14M64Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M64Desc), culture);

    public static string Dev14M65Desc() => Milestone.Get(nameof (Dev14M65Desc));

    public static string Dev14M65Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M65Desc), culture);

    public static string Dev14M66Desc() => Milestone.Get(nameof (Dev14M66Desc));

    public static string Dev14M66Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M66Desc), culture);

    public static string Dev14M67Desc() => Milestone.Get(nameof (Dev14M67Desc));

    public static string Dev14M67Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M67Desc), culture);

    public static string Dev14M68Desc() => Milestone.Get(nameof (Dev14M68Desc));

    public static string Dev14M68Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M68Desc), culture);

    public static string Dev14M69Desc() => Milestone.Get(nameof (Dev14M69Desc));

    public static string Dev14M69Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M69Desc), culture);

    public static string Dev14M70Desc() => Milestone.Get(nameof (Dev14M70Desc));

    public static string Dev14M70Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M70Desc), culture);

    public static string Dev14M71Desc() => Milestone.Get(nameof (Dev14M71Desc));

    public static string Dev14M71Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M71Desc), culture);

    public static string Dev14M72Desc() => Milestone.Get(nameof (Dev14M72Desc));

    public static string Dev14M72Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M72Desc), culture);

    public static string Dev14M73Desc() => Milestone.Get(nameof (Dev14M73Desc));

    public static string Dev14M73Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M73Desc), culture);

    public static string Dev14M74Desc() => Milestone.Get(nameof (Dev14M74Desc));

    public static string Dev14M74Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M74Desc), culture);

    public static string Dev14M75Desc() => Milestone.Get(nameof (Dev14M75Desc));

    public static string Dev14M75Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M75Desc), culture);

    public static string Dev14M76Desc() => Milestone.Get(nameof (Dev14M76Desc));

    public static string Dev14M76Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M76Desc), culture);

    public static string Dev14M78Desc() => Milestone.Get(nameof (Dev14M78Desc));

    public static string Dev14M78Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M78Desc), culture);

    public static string Dev14M79Desc() => Milestone.Get(nameof (Dev14M79Desc));

    public static string Dev14M79Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M79Desc), culture);

    public static string Dev14M80Desc() => Milestone.Get(nameof (Dev14M80Desc));

    public static string Dev14M80Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M80Desc), culture);

    public static string Dev14M81Desc() => Milestone.Get(nameof (Dev14M81Desc));

    public static string Dev14M81Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M81Desc), culture);

    public static string Dev14M82Desc() => Milestone.Get(nameof (Dev14M82Desc));

    public static string Dev14M82Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M82Desc), culture);

    public static string Dev14M83Part2Desc() => Milestone.Get(nameof (Dev14M83Part2Desc));

    public static string Dev14M83Part2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M83Part2Desc), culture);

    public static string Dev14M83Part3Desc() => Milestone.Get(nameof (Dev14M83Part3Desc));

    public static string Dev14M83Part3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M83Part3Desc), culture);

    public static string Dev14M83Part4Desc() => Milestone.Get(nameof (Dev14M83Part4Desc));

    public static string Dev14M83Part4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M83Part4Desc), culture);

    public static string Dev14M83Part5Desc() => Milestone.Get(nameof (Dev14M83Part5Desc));

    public static string Dev14M83Part5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M83Part5Desc), culture);

    public static string Dev14M83Part6Desc() => Milestone.Get(nameof (Dev14M83Part6Desc));

    public static string Dev14M83Part6Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M83Part6Desc), culture);

    public static string Dev14M84Desc() => Milestone.Get(nameof (Dev14M84Desc));

    public static string Dev14M84Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M84Desc), culture);

    public static string Dev14M85Desc() => Milestone.Get(nameof (Dev14M85Desc));

    public static string Dev14M85Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M85Desc), culture);

    public static string Dev14M86Desc() => Milestone.Get(nameof (Dev14M86Desc));

    public static string Dev14M86Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M86Desc), culture);

    public static string Dev14M87Desc() => Milestone.Get(nameof (Dev14M87Desc));

    public static string Dev14M87Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M87Desc), culture);

    public static string Dev14M88Desc() => Milestone.Get(nameof (Dev14M88Desc));

    public static string Dev14M88Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M88Desc), culture);

    public static string Dev14M89Desc() => Milestone.Get(nameof (Dev14M89Desc));

    public static string Dev14M89Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M89Desc), culture);

    public static string Dev14M89Part2Desc() => Milestone.Get(nameof (Dev14M89Part2Desc));

    public static string Dev14M89Part2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M89Part2Desc), culture);

    public static string Dev14M89Part3Desc() => Milestone.Get(nameof (Dev14M89Part3Desc));

    public static string Dev14M89Part3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M89Part3Desc), culture);

    public static string Dev14M89Part4Desc() => Milestone.Get(nameof (Dev14M89Part4Desc));

    public static string Dev14M89Part4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M89Part4Desc), culture);

    public static string Dev14M89Part5Desc() => Milestone.Get(nameof (Dev14M89Part5Desc));

    public static string Dev14M89Part5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M89Part5Desc), culture);

    public static string Dev14M89Part6Desc() => Milestone.Get(nameof (Dev14M89Part6Desc));

    public static string Dev14M89Part6Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M89Part6Desc), culture);

    public static string Dev14M89Part7Desc() => Milestone.Get(nameof (Dev14M89Part7Desc));

    public static string Dev14M89Part7Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M89Part7Desc), culture);

    public static string Dev14M90Desc() => Milestone.Get(nameof (Dev14M90Desc));

    public static string Dev14M90Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M90Desc), culture);

    public static string Dev14M91Desc() => Milestone.Get(nameof (Dev14M91Desc));

    public static string Dev14M91Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M91Desc), culture);

    public static string Dev14M92Desc() => Milestone.Get(nameof (Dev14M92Desc));

    public static string Dev14M92Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M92Desc), culture);

    public static string Dev14M93Desc() => Milestone.Get(nameof (Dev14M93Desc));

    public static string Dev14M93Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M93Desc), culture);

    public static string Dev14M94Desc() => Milestone.Get(nameof (Dev14M94Desc));

    public static string Dev14M94Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M94Desc), culture);

    public static string Dev14M95Desc() => Milestone.Get(nameof (Dev14M95Desc));

    public static string Dev14M95Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M95Desc), culture);

    public static string Dev14M95_1Desc() => Milestone.Get(nameof (Dev14M95_1Desc));

    public static string Dev14M95_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M95_1Desc), culture);

    public static string Dev14M95_2Desc() => Milestone.Get(nameof (Dev14M95_2Desc));

    public static string Dev14M95_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M95_2Desc), culture);

    public static string Dev14M95_3Desc() => Milestone.Get(nameof (Dev14M95_3Desc));

    public static string Dev14M95_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M95_3Desc), culture);

    public static string Dev14M95_4Desc() => Milestone.Get(nameof (Dev14M95_4Desc));

    public static string Dev14M95_4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M95_4Desc), culture);

    public static string Dev14M98Desc() => Milestone.Get(nameof (Dev14M98Desc));

    public static string Dev14M98Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M98Desc), culture);

    public static string Dev14M102Desc() => Milestone.Get(nameof (Dev14M102Desc));

    public static string Dev14M102Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M102Desc), culture);

    public static string Dev14M114Desc() => Milestone.Get(nameof (Dev14M114Desc));

    public static string Dev14M114Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M114Desc), culture);

    public static string Dev14M114_1Desc() => Milestone.Get(nameof (Dev14M114_1Desc));

    public static string Dev14M114_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev14M114_1Desc), culture);

    public static string Dev15M96Desc() => Milestone.Get(nameof (Dev15M96Desc));

    public static string Dev15M96Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M96Desc), culture);

    public static string Dev15M97Desc() => Milestone.Get(nameof (Dev15M97Desc));

    public static string Dev15M97Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M97Desc), culture);

    public static string Dev15M98Desc() => Milestone.Get(nameof (Dev15M98Desc));

    public static string Dev15M98Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M98Desc), culture);

    public static string Dev15M99Desc() => Milestone.Get(nameof (Dev15M99Desc));

    public static string Dev15M99Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M99Desc), culture);

    public static string Dev15M100Desc() => Milestone.Get(nameof (Dev15M100Desc));

    public static string Dev15M100Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M100Desc), culture);

    public static string Dev15M101Desc() => Milestone.Get(nameof (Dev15M101Desc));

    public static string Dev15M101Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M101Desc), culture);

    public static string Dev15M102Desc() => Milestone.Get(nameof (Dev15M102Desc));

    public static string Dev15M102Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M102Desc), culture);

    public static string Dev15M103Desc() => Milestone.Get(nameof (Dev15M103Desc));

    public static string Dev15M103Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M103Desc), culture);

    public static string Dev15M104Desc() => Milestone.Get(nameof (Dev15M104Desc));

    public static string Dev15M104Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M104Desc), culture);

    public static string Dev15M105Desc() => Milestone.Get(nameof (Dev15M105Desc));

    public static string Dev15M105Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M105Desc), culture);

    public static string Dev15M105_1Desc() => Milestone.Get(nameof (Dev15M105_1Desc));

    public static string Dev15M105_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M105_1Desc), culture);

    public static string Dev15M105_2Desc() => Milestone.Get(nameof (Dev15M105_2Desc));

    public static string Dev15M105_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M105_2Desc), culture);

    public static string Dev15M105_3Desc() => Milestone.Get(nameof (Dev15M105_3Desc));

    public static string Dev15M105_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M105_3Desc), culture);

    public static string Dev15M105_4Desc() => Milestone.Get(nameof (Dev15M105_4Desc));

    public static string Dev15M105_4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M105_4Desc), culture);

    public static string Dev15M105_5Desc() => Milestone.Get(nameof (Dev15M105_5Desc));

    public static string Dev15M105_5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M105_5Desc), culture);

    public static string Dev15M105_6Desc() => Milestone.Get(nameof (Dev15M105_6Desc));

    public static string Dev15M105_6Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M105_6Desc), culture);

    public static string Dev15M106Desc() => Milestone.Get(nameof (Dev15M106Desc));

    public static string Dev15M106Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M106Desc), culture);

    public static string Dev15M107Desc() => Milestone.Get(nameof (Dev15M107Desc));

    public static string Dev15M107Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M107Desc), culture);

    public static string Dev15M108Desc() => Milestone.Get(nameof (Dev15M108Desc));

    public static string Dev15M108Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M108Desc), culture);

    public static string Dev15M109Desc() => Milestone.Get(nameof (Dev15M109Desc));

    public static string Dev15M109Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M109Desc), culture);

    public static string Dev15M111Desc() => Milestone.Get(nameof (Dev15M111Desc));

    public static string Dev15M111Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M111Desc), culture);

    public static string Dev15M111_1Desc() => Milestone.Get(nameof (Dev15M111_1Desc));

    public static string Dev15M111_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M111_1Desc), culture);

    public static string Dev15M111_2Desc() => Milestone.Get(nameof (Dev15M111_2Desc));

    public static string Dev15M111_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M111_2Desc), culture);

    public static string Dev15M112Desc() => Milestone.Get(nameof (Dev15M112Desc));

    public static string Dev15M112Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M112Desc), culture);

    public static string Dev15M112_1Desc() => Milestone.Get(nameof (Dev15M112_1Desc));

    public static string Dev15M112_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M112_1Desc), culture);

    public static string Dev15M112_2Desc() => Milestone.Get(nameof (Dev15M112_2Desc));

    public static string Dev15M112_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M112_2Desc), culture);

    public static string Dev15M112_3Desc() => Milestone.Get(nameof (Dev15M112_3Desc));

    public static string Dev15M112_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M112_3Desc), culture);

    public static string Dev15M112_4Desc() => Milestone.Get(nameof (Dev15M112_4Desc));

    public static string Dev15M112_4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M112_4Desc), culture);

    public static string Dev15M113Desc() => Milestone.Get(nameof (Dev15M113Desc));

    public static string Dev15M113Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M113Desc), culture);

    public static string Dev15M114Desc() => Milestone.Get(nameof (Dev15M114Desc));

    public static string Dev15M114Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M114Desc), culture);

    public static string Dev15M115Desc() => Milestone.Get(nameof (Dev15M115Desc));

    public static string Dev15M115Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M115Desc), culture);

    public static string Dev15M116Desc() => Milestone.Get(nameof (Dev15M116Desc));

    public static string Dev15M116Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M116Desc), culture);

    public static string Dev15M117Desc() => Milestone.Get(nameof (Dev15M117Desc));

    public static string Dev15M117Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M117Desc), culture);

    public static string Dev15M117_1Desc() => Milestone.Get(nameof (Dev15M117_1Desc));

    public static string Dev15M117_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M117_1Desc), culture);

    public static string Dev15M117_2Desc() => Milestone.Get(nameof (Dev15M117_2Desc));

    public static string Dev15M117_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M117_2Desc), culture);

    public static string Dev15M117_3Desc() => Milestone.Get(nameof (Dev15M117_3Desc));

    public static string Dev15M117_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M117_3Desc), culture);

    public static string Dev15M117_4Desc() => Milestone.Get(nameof (Dev15M117_4Desc));

    public static string Dev15M117_4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M117_4Desc), culture);

    public static string Dev15M117_5Desc() => Milestone.Get(nameof (Dev15M117_5Desc));

    public static string Dev15M117_5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M117_5Desc), culture);

    public static string Dev15M124Desc() => Milestone.Get(nameof (Dev15M124Desc));

    public static string Dev15M124Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M124Desc), culture);

    public static string Dev15M125Desc() => Milestone.Get(nameof (Dev15M125Desc));

    public static string Dev15M125Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M125Desc), culture);

    public static string Dev15M126Desc() => Milestone.Get(nameof (Dev15M126Desc));

    public static string Dev15M126Desc(CultureInfo culture) => Milestone.Get(nameof (Dev15M126Desc), culture);

    public static string Dev16M118Desc() => Milestone.Get(nameof (Dev16M118Desc));

    public static string Dev16M118Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M118Desc), culture);

    public static string Dev16M119Desc() => Milestone.Get(nameof (Dev16M119Desc));

    public static string Dev16M119Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M119Desc), culture);

    public static string Dev16M120Desc() => Milestone.Get(nameof (Dev16M120Desc));

    public static string Dev16M120Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M120Desc), culture);

    public static string Dev16M121Desc() => Milestone.Get(nameof (Dev16M121Desc));

    public static string Dev16M121Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M121Desc), culture);

    public static string Dev16M121_1Desc() => Milestone.Get(nameof (Dev16M121_1Desc));

    public static string Dev16M121_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M121_1Desc), culture);

    public static string Dev16M121_2Desc() => Milestone.Get(nameof (Dev16M121_2Desc));

    public static string Dev16M121_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M121_2Desc), culture);

    public static string Dev16M122Desc() => Milestone.Get(nameof (Dev16M122Desc));

    public static string Dev16M122Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122Desc), culture);

    public static string Dev16M122_1Desc() => Milestone.Get(nameof (Dev16M122_1Desc));

    public static string Dev16M122_1Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_1Desc), culture);

    public static string Dev16M122_2Desc() => Milestone.Get(nameof (Dev16M122_2Desc));

    public static string Dev16M122_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_2Desc), culture);

    public static string Dev16M122_3Desc() => Milestone.Get(nameof (Dev16M122_3Desc));

    public static string Dev16M122_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_3Desc), culture);

    public static string Dev16M122_4Desc() => Milestone.Get(nameof (Dev16M122_4Desc));

    public static string Dev16M122_4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_4Desc), culture);

    public static string Dev16M122_5Desc() => Milestone.Get(nameof (Dev16M122_5Desc));

    public static string Dev16M122_5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_5Desc), culture);

    public static string Dev16M122_6Desc() => Milestone.Get(nameof (Dev16M122_6Desc));

    public static string Dev16M122_6Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_6Desc), culture);

    public static string Dev16M122_7Desc() => Milestone.Get(nameof (Dev16M122_7Desc));

    public static string Dev16M122_7Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_7Desc), culture);

    public static string Dev16M122_8Desc() => Milestone.Get(nameof (Dev16M122_8Desc));

    public static string Dev16M122_8Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_8Desc), culture);

    public static string Dev16M122_9Desc() => Milestone.Get(nameof (Dev16M122_9Desc));

    public static string Dev16M122_9Desc(CultureInfo culture) => Milestone.Get(nameof (Dev16M122_9Desc), culture);

    public static string Dev16Update2RC1() => Milestone.Get(nameof (Dev16Update2RC1));

    public static string Dev16Update2RC1(CultureInfo culture) => Milestone.Get(nameof (Dev16Update2RC1), culture);

    public static string Dev16Update2RC2() => Milestone.Get(nameof (Dev16Update2RC2));

    public static string Dev16Update2RC2(CultureInfo culture) => Milestone.Get(nameof (Dev16Update2RC2), culture);

    public static string Dev16Update2() => Milestone.Get(nameof (Dev16Update2));

    public static string Dev16Update2(CultureInfo culture) => Milestone.Get(nameof (Dev16Update2), culture);

    public static string Dev16Update3() => Milestone.Get(nameof (Dev16Update3));

    public static string Dev16Update3(CultureInfo culture) => Milestone.Get(nameof (Dev16Update3), culture);

    public static string DevOpsServer2019RC1() => Milestone.Get(nameof (DevOpsServer2019RC1));

    public static string DevOpsServer2019RC1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019RC1), culture);

    public static string DevOpsServer2019RC2() => Milestone.Get(nameof (DevOpsServer2019RC2));

    public static string DevOpsServer2019RC2(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019RC2), culture);

    public static string DevOpsServer2019() => Milestone.Get(nameof (DevOpsServer2019));

    public static string DevOpsServer2019(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019), culture);

    public static string DevOpsServer2019_0_1RC() => Milestone.Get(nameof (DevOpsServer2019_0_1RC));

    public static string DevOpsServer2019_0_1RC(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019_0_1RC), culture);

    public static string DevOpsServer2019_0_1() => Milestone.Get(nameof (DevOpsServer2019_0_1));

    public static string DevOpsServer2019_0_1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019_0_1), culture);

    public static string DevOpsServer2019Update1RC1() => Milestone.Get(nameof (DevOpsServer2019Update1RC1));

    public static string DevOpsServer2019Update1RC1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019Update1RC1), culture);

    public static string DevOpsServer2019Update1RC2() => Milestone.Get(nameof (DevOpsServer2019Update1RC2));

    public static string DevOpsServer2019Update1RC2(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019Update1RC2), culture);

    public static string DevOpsServer2019Update1() => Milestone.Get(nameof (DevOpsServer2019Update1));

    public static string DevOpsServer2019Update1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019Update1), culture);

    public static string DevOpsServer2019Update1_1RC() => Milestone.Get(nameof (DevOpsServer2019Update1_1RC));

    public static string DevOpsServer2019Update1_1RC(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019Update1_1RC), culture);

    public static string DevOpsServer2019Update1_1() => Milestone.Get(nameof (DevOpsServer2019Update1_1));

    public static string DevOpsServer2019Update1_1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019Update1_1), culture);

    public static string DevOpsServer2019Update1_2() => Milestone.Get(nameof (DevOpsServer2019Update1_2));

    public static string DevOpsServer2019Update1_2(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2019Update1_2), culture);

    public static string Dev18M170_2Desc() => Milestone.Get(nameof (Dev18M170_2Desc));

    public static string Dev18M170_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M170_2Desc), culture);

    public static string Dev18M170_3Desc() => Milestone.Get(nameof (Dev18M170_3Desc));

    public static string Dev18M170_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M170_3Desc), culture);

    public static string Dev18M170_4Desc() => Milestone.Get(nameof (Dev18M170_4Desc));

    public static string Dev18M170_4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M170_4Desc), culture);

    public static string Dev18M170_5Desc() => Milestone.Get(nameof (Dev18M170_5Desc));

    public static string Dev18M170_5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M170_5Desc), culture);

    public static string Dev18M170_6Desc() => Milestone.Get(nameof (Dev18M170_6Desc));

    public static string Dev18M170_6Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M170_6Desc), culture);

    public static string Dev18M170_7Desc() => Milestone.Get(nameof (Dev18M170_7Desc));

    public static string Dev18M170_7Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M170_7Desc), culture);

    public static string Dev18M170_8Desc() => Milestone.Get(nameof (Dev18M170_8Desc));

    public static string Dev18M170_8Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M170_8Desc), culture);

    public static string DevOpsServer2020RTM_2() => Milestone.Get(nameof (DevOpsServer2020RTM_2));

    public static string DevOpsServer2020RTM_2(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2020RTM_2), culture);

    public static string Dev18M181_2Desc() => Milestone.Get(nameof (Dev18M181_2Desc));

    public static string Dev18M181_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M181_2Desc), culture);

    public static string Dev18M181_3Desc() => Milestone.Get(nameof (Dev18M181_3Desc));

    public static string Dev18M181_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M181_3Desc), culture);

    public static string Dev18M181_4Desc() => Milestone.Get(nameof (Dev18M181_4Desc));

    public static string Dev18M181_4Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M181_4Desc), culture);

    public static string Dev18M181_5Desc() => Milestone.Get(nameof (Dev18M181_5Desc));

    public static string Dev18M181_5Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M181_5Desc), culture);

    public static string Dev18M181_6Desc() => Milestone.Get(nameof (Dev18M181_6Desc));

    public static string Dev18M181_6Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M181_6Desc), culture);

    public static string Dev18M181_7Desc() => Milestone.Get(nameof (Dev18M181_7Desc));

    public static string Dev18M181_7Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M181_7Desc), culture);

    public static string DevOpsServer2020Update1_1() => Milestone.Get(nameof (DevOpsServer2020Update1_1));

    public static string DevOpsServer2020Update1_1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2020Update1_1), culture);

    public static string DevOpsServer2020Update1_2() => Milestone.Get(nameof (DevOpsServer2020Update1_2));

    public static string DevOpsServer2020Update1_2(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2020Update1_2), culture);

    public static string Dev18M193Desc() => Milestone.Get(nameof (Dev18M193Desc));

    public static string Dev18M193Desc(CultureInfo culture) => Milestone.Get(nameof (Dev18M193Desc), culture);

    public static string DevOpsServer2022RC1() => Milestone.Get(nameof (DevOpsServer2022RC1));

    public static string DevOpsServer2022RC1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022RC1), culture);

    public static string DevOpsServer2022RC2() => Milestone.Get(nameof (DevOpsServer2022RC2));

    public static string DevOpsServer2022RC2(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022RC2), culture);

    public static string DevOpsServer2022RTW() => Milestone.Get(nameof (DevOpsServer2022RTW));

    public static string DevOpsServer2022RTW(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022RTW), culture);

    public static string DevOpsServer2022_0_1RC1() => Milestone.Get(nameof (DevOpsServer2022_0_1RC1));

    public static string DevOpsServer2022_0_1RC1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022_0_1RC1), culture);

    public static string DevOpsServer2022_0_1RTW() => Milestone.Get(nameof (DevOpsServer2022_0_1RTW));

    public static string DevOpsServer2022_0_1RTW(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022_0_1RTW), culture);

    public static string DevOpsServer2022_1RC1() => Milestone.Get(nameof (DevOpsServer2022_1RC1));

    public static string DevOpsServer2022_1RC1(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022_1RC1), culture);

    public static string DevOpsServer2022_1RC2() => Milestone.Get(nameof (DevOpsServer2022_1RC2));

    public static string DevOpsServer2022_1RC2(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022_1RC2), culture);

    public static string DevOpsServer2022_1RTW() => Milestone.Get(nameof (DevOpsServer2022_1RTW));

    public static string DevOpsServer2022_1RTW(CultureInfo culture) => Milestone.Get(nameof (DevOpsServer2022_1RTW), culture);

    public static string Dev19M225_2Desc() => Milestone.Get(nameof (Dev19M225_2Desc));

    public static string Dev19M225_2Desc(CultureInfo culture) => Milestone.Get(nameof (Dev19M225_2Desc), culture);

    public static string Dev19M225_3Desc() => Milestone.Get(nameof (Dev19M225_3Desc));

    public static string Dev19M225_3Desc(CultureInfo culture) => Milestone.Get(nameof (Dev19M225_3Desc), culture);

    public static string AzureDevOpsServer2022_2RC() => Milestone.Get(nameof (AzureDevOpsServer2022_2RC));

    public static string AzureDevOpsServer2022_2RC(CultureInfo culture) => Milestone.Get(nameof (AzureDevOpsServer2022_2RC), culture);

    public static string AzureDevOpsServer2022_2RTW() => Milestone.Get(nameof (AzureDevOpsServer2022_2RTW));

    public static string AzureDevOpsServer2022_2RTW(CultureInfo culture) => Milestone.Get(nameof (AzureDevOpsServer2022_2RTW), culture);

    public static string Current() => Milestone.Get(nameof (Current));

    public static string Current(CultureInfo culture) => Milestone.Get(nameof (Current), culture);
  }
}
