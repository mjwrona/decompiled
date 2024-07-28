// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.IcuCollationKeywordPropertyProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Nest;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public static class IcuCollationKeywordPropertyProvider
  {
    internal static FriendlyDictionary<int, Language> LcidToLanguage = new FriendlyDictionary<int, Language>()
    {
      [1033] = Language.English,
      [2057] = Language.English,
      [1036] = Language.French,
      [1031] = Language.German,
      [1040] = Language.Italian,
      [3082] = Language.Spanish,
      [1049] = Language.Russian,
      [1053] = Language.Swedish,
      [1035] = Language.Finnish,
      [1055] = Language.Turkish,
      [1025] = Language.Arabic,
      [1041] = Language.Cjk,
      [1042] = Language.Cjk,
      [3076] = Language.Cjk,
      [5124] = Language.Cjk,
      [4100] = Language.Cjk,
      [2052] = Language.Cjk,
      [1028] = Language.Cjk
    };
    private static readonly FriendlyDictionary<Language, IcuCollationKeywordProperty> s_getKeywordProperty = new FriendlyDictionary<Language, IcuCollationKeywordProperty>()
    {
      [Language.English] = new EnglishIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.French] = new FrenchIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.German] = new GermanIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Turkish] = new TurkishIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Spanish] = new SpanishIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Italian] = new ItalianIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Russian] = new RussianIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Swedish] = new SwedishIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Finnish] = new FinnishIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Cjk] = new EnglishIcuCollationKeyword().LanguageSpecificIcuCollationKeyword,
      [Language.Arabic] = new ArabicIcuCollationKeyword().LanguageSpecificIcuCollationKeyword
    };

    public static IcuCollationKeywordProperty GetIcuCollationKeywordProperty(int lcid)
    {
      Language key = Language.English;
      IcuCollationKeywordPropertyProvider.LcidToLanguage.TryGetValue(lcid, out key);
      return IcuCollationKeywordPropertyProvider.s_getKeywordProperty[key];
    }
  }
}
