// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.ParsingUtilities
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public static class ParsingUtilities
  {
    private static readonly Dictionary<string, ReleaseType> s_stringToReleaseTypeMap = ParsingUtilities.CreateStringToReleaseTypeMap();
    private static readonly Dictionary<string, VisualStudioFamily> s_stringToVisualStudioFamilyMap = ParsingUtilities.CreateStringToVisualStudioFamilyMap();
    private static readonly Dictionary<string, VisualStudioEdition> s_stringToVisualStudioEditionMap = ParsingUtilities.CreateStringToVisualStudioEditionMap();

    public static bool TryParseProductFamily(string s, out VisualStudioFamily family)
    {
      family = VisualStudioFamily.Invalid;
      return !string.IsNullOrEmpty(s) && ParsingUtilities.s_stringToVisualStudioFamilyMap.TryGetValue(s, out family);
    }

    public static bool TryParseProductEdition(string s, out VisualStudioEdition edition)
    {
      edition = VisualStudioEdition.Unspecified;
      return !string.IsNullOrEmpty(s) && ParsingUtilities.s_stringToVisualStudioEditionMap.TryGetValue(s, out edition);
    }

    public static bool TryParseReleaseType(string s, out ReleaseType releaseType)
    {
      releaseType = ReleaseType.Release;
      return !string.IsNullOrEmpty(s) && ParsingUtilities.s_stringToReleaseTypeMap.TryGetValue(s, out releaseType);
    }

    private static Dictionary<string, ReleaseType> CreateStringToReleaseTypeMap() => new Dictionary<string, ReleaseType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Prerelease",
        ReleaseType.Prerelease
      },
      {
        "Release",
        ReleaseType.Release
      }
    };

    private static Dictionary<string, VisualStudioFamily> CreateStringToVisualStudioFamilyMap() => new Dictionary<string, VisualStudioFamily>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "TestProfessional",
        VisualStudioFamily.VisualStudioTestProfessional
      },
      {
        "VisualStudio",
        VisualStudioFamily.VisualStudio
      },
      {
        "VSonMac",
        VisualStudioFamily.VisualStudioMac
      },
      {
        "TestManager",
        VisualStudioFamily.TestManager
      },
      {
        "VSWinExpress",
        VisualStudioFamily.VisualStudioExpressWindows
      },
      {
        "VWDExpress",
        VisualStudioFamily.VisualStudioExpressWeb
      },
      {
        "VPDExpress",
        VisualStudioFamily.VisualStudioExpressPhone
      },
      {
        "WDExpress",
        VisualStudioFamily.VisualStudioExpressDesktop
      },
      {
        "VisualStudioEmulatorAndroid",
        VisualStudioFamily.VisualStudioEmulatorAndroid
      }
    };

    private static Dictionary<string, VisualStudioEdition> CreateStringToVisualStudioEditionMap() => new Dictionary<string, VisualStudioEdition>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Professional",
        VisualStudioEdition.Professional
      },
      {
        "Premium",
        VisualStudioEdition.Premium
      },
      {
        "Ultimate",
        VisualStudioEdition.Ultimate
      },
      {
        "Community",
        VisualStudioEdition.Community
      },
      {
        "Enterprise",
        VisualStudioEdition.Enterprise
      }
    };

    public static string EscapeSingleQuote(string literal) => literal.Replace("'", "''");
  }
}
