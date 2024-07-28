// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.Language
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Admin
{
  public sealed class Language : IEquatable<Language>
  {
    private static readonly Language[] s_Langs = new Language[10]
    {
      new Language(1033, nameof (English)),
      new Language(1031, "Deutsch"),
      new Language(3082, "Español"),
      new Language(1036, "Français"),
      new Language(1040, "Italiano"),
      new Language(1049, "Pусский"),
      new Language(1041, "日本語"),
      new Language(2052, "简体中文"),
      new Language(1028, "繁體中文"),
      new Language(1042, "한국어")
    };

    private Language(int lcid, string displayName)
    {
      this.LCID = lcid;
      this.Culture = CultureInfo.GetCultureInfo(this.LCID);
      this.DisplayName = displayName;
    }

    public int LCID { get; }

    public string DisplayName { get; }

    public CultureInfo Culture { get; }

    public static Language[] Languages => Language.s_Langs;

    public static Language[] GetLanguages(Language preferredLanguage)
    {
      ArgumentUtility.CheckForNull<Language>(preferredLanguage, nameof (preferredLanguage));
      Language[] languages = new Language[Language.Languages.Length];
      int num = 1;
      for (int index = 0; index < Language.s_Langs.Length; ++index)
      {
        Language lang = Language.s_Langs[index];
        if (lang.LCID == preferredLanguage.LCID)
          languages[0] = lang;
        else
          languages[num++] = lang;
      }
      return languages;
    }

    public static Language GetPreferred(CultureInfo lang)
    {
      ArgumentUtility.CheckForNull<CultureInfo>(lang, nameof (lang));
      string letterIsoLanguageName = lang.TwoLetterISOLanguageName;
      if (string.Equals(letterIsoLanguageName, "zh", StringComparison.OrdinalIgnoreCase))
      {
        if (string.Equals(lang.ThreeLetterWindowsLanguageName, "CHT", StringComparison.OrdinalIgnoreCase))
          return Language.TraditionalChinese;
        return string.Equals(lang.ThreeLetterWindowsLanguageName, "CHS", StringComparison.OrdinalIgnoreCase) ? Language.SimplifiedChinese : Language.GetPreferred(lang.Parent);
      }
      for (int index = 0; index < Language.s_Langs.Length; ++index)
      {
        if (string.Equals(letterIsoLanguageName, Language.s_Langs[index].Culture.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase))
          return Language.s_Langs[index];
      }
      return Language.English;
    }

    public static Language FindLanguage(int lcid) => ((IEnumerable<Language>) Language.s_Langs).FirstOrDefault<Language>((Func<Language, bool>) (lang => lang.LCID == lcid));

    public override bool Equals(object obj) => this.Equals(obj as Language);

    public bool Equals(Language lang) => !(lang == (Language) null) && this.LCID == lang.LCID;

    public static bool operator ==(Language x, Language y) => (object) x == null ? (object) y == null : x.Equals(y);

    public static bool operator !=(Language x, Language y) => !(x == y);

    public override int GetHashCode() => this.LCID;

    private static Language English => ((IEnumerable<Language>) Language.s_Langs).Single<Language>((Func<Language, bool>) (l => l.LCID == 1033));

    private static Language SimplifiedChinese => ((IEnumerable<Language>) Language.s_Langs).Single<Language>((Func<Language, bool>) (l => l.LCID == 2052));

    private static Language TraditionalChinese => ((IEnumerable<Language>) Language.s_Langs).Single<Language>((Func<Language, bool>) (l => l.LCID == 1028));
  }
}
