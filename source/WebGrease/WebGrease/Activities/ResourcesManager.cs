// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.ResourcesManager
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace WebGrease.Activities
{
  internal static class ResourcesManager
  {
    internal static void TryGetResources(
      string resourcesDirectoryPath,
      string localeOrThemeName,
      out Dictionary<string, string> resources)
    {
      resources = new Dictionary<string, string>();
      string resourcePath;
      if (!ResourcesManager.HasResources(resourcesDirectoryPath, localeOrThemeName, out resourcePath))
        return;
      using (ResXResourceReader resXresourceReader = new ResXResourceReader(resourcePath))
      {
        foreach (DictionaryEntry dictionaryEntry in resXresourceReader)
        {
          string key = dictionaryEntry.Key as string;
          string str = dictionaryEntry.Value as string;
          if (key != null)
            resources.Add(key, str);
        }
      }
    }

    private static bool HasResources(
      string resourcesDirectoryPath,
      string localeOrThemeName,
      out string resourcePath)
    {
      resourcePath = Path.Combine(resourcesDirectoryPath, localeOrThemeName + ".resx");
      return File.Exists(resourcePath);
    }
  }
}
