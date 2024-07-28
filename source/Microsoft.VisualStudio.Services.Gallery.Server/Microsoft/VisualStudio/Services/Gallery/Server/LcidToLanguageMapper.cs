// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.LcidToLanguageMapper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class LcidToLanguageMapper
  {
    private static Dictionary<int, string> lcidToLanguageMap = new Dictionary<int, string>()
    {
      {
        4,
        "zh-hans"
      },
      {
        5,
        "cs"
      },
      {
        7,
        "de"
      },
      {
        9,
        "en"
      },
      {
        10,
        "es"
      },
      {
        12,
        "fr"
      },
      {
        16,
        "it"
      },
      {
        17,
        "ja"
      },
      {
        18,
        "ko"
      },
      {
        21,
        "pl"
      },
      {
        25,
        "ru"
      },
      {
        31,
        "tr"
      },
      {
        (int) sbyte.MaxValue,
        "reserved for invariant locale behavior"
      },
      {
        1028,
        "zh-tw"
      },
      {
        1029,
        "cs-cz"
      },
      {
        1031,
        "de-de"
      },
      {
        1033,
        "en-us"
      },
      {
        1036,
        "fr-fr"
      },
      {
        1040,
        "it-it"
      },
      {
        1041,
        "ja-jp"
      },
      {
        1042,
        "ko-kr"
      },
      {
        1045,
        "pl-pl"
      },
      {
        1046,
        "pt-br"
      },
      {
        1049,
        "ru-ru"
      },
      {
        1055,
        "tr-tr"
      },
      {
        2052,
        "zh-cn"
      },
      {
        2055,
        "de-ch"
      },
      {
        3082,
        "es-es"
      },
      {
        11274,
        "es-ar"
      },
      {
        31748,
        "zh-hant"
      }
    };

    public static string GetLanguageCodeForLcid(int lcid)
    {
      string languageCodeForLcid;
      LcidToLanguageMapper.lcidToLanguageMap.TryGetValue(lcid, out languageCodeForLcid);
      return languageCodeForLcid;
    }
  }
}
