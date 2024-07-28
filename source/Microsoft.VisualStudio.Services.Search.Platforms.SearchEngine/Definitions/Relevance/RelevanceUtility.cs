// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RelevanceUtility
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance
{
  public static class RelevanceUtility
  {
    private static readonly Dictionary<string, int> s_fileExtensionId = RelevanceUtility.InitializeFileExtensionIds();
    private static readonly DateTime epoch = new DateTime(1970, 1, 1);

    private static Dictionary<string, int> InitializeFileExtensionIds()
    {
      Dictionary<string, int> dictionary = new Dictionary<string, int>(16);
      foreach (FileExtensionType fileExtensionType in Enum.GetValues(typeof (FileExtensionType)))
        dictionary.Add(fileExtensionType.ToString().ToLowerInvariant(), (int) fileExtensionType);
      return dictionary;
    }

    public static int GetFileExtensionId(string fileExtension)
    {
      int num;
      return RelevanceUtility.s_fileExtensionId.TryGetValue(fileExtension, out num) ? num : 0;
    }

    public static int GetCurrentTimeInEpoch() => (int) DateTime.UtcNow.Subtract(RelevanceUtility.epoch).TotalSeconds;
  }
}
