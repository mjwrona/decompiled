// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.WorkItemIndexAnalyzersProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public static class WorkItemIndexAnalyzersProvider
  {
    internal static IReadOnlyDictionary<int, Language> LcidToLanguage = (IReadOnlyDictionary<int, Language>) new Dictionary<int, Language>()
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

    public static WorkItemIndexAnalyzers GetWorkItemIndexAnalyzers(int lcid)
    {
      Language language = Language.English;
      if (WorkItemIndexAnalyzersProvider.LcidToLanguage.ContainsKey(lcid))
        language = WorkItemIndexAnalyzersProvider.LcidToLanguage[lcid];
      return WorkItemIndexAnalyzersProvider.GetAnalyzers(language);
    }

    private static WorkItemIndexAnalyzers GetAnalyzers(Language language)
    {
      switch (language)
      {
        case Language.Arabic:
          return (WorkItemIndexAnalyzers) new ArabicAnalyzers();
        case Language.Cjk:
          return (WorkItemIndexAnalyzers) new CjkAnalyzers();
        case Language.English:
          return (WorkItemIndexAnalyzers) new EnglishAnalyzers();
        case Language.Finnish:
          return (WorkItemIndexAnalyzers) new FinnishAnalyzers();
        case Language.French:
          return (WorkItemIndexAnalyzers) new FrenchAnalyzers();
        case Language.German:
          return (WorkItemIndexAnalyzers) new GermanAnalyzers();
        case Language.Italian:
          return (WorkItemIndexAnalyzers) new ItalianAnalyzers();
        case Language.Russian:
          return (WorkItemIndexAnalyzers) new RussianAnalyzers();
        case Language.Spanish:
          return (WorkItemIndexAnalyzers) new SpanishAnalyzers();
        case Language.Swedish:
          return (WorkItemIndexAnalyzers) new SwedishAnalyzers();
        case Language.Turkish:
          return (WorkItemIndexAnalyzers) new TurkishAnalyzers();
        default:
          return (WorkItemIndexAnalyzers) new EnglishAnalyzers();
      }
    }
  }
}
