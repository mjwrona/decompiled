// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.TyposquattingSkipList
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public static class TyposquattingSkipList
  {
    public const char Delimiter = ',';

    public static HashSet<string> LoadSkipList(
      RegistryEntryCollection registryEntryCollection,
      string skipListStringPathInRegistry)
    {
      string valueFromPath = registryEntryCollection.GetValueFromPath<string>(skipListStringPathInRegistry, string.Empty);
      if (string.IsNullOrWhiteSpace(valueFromPath))
        return new HashSet<string>();
      return ((IEnumerable<string>) valueFromPath.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (i => i.Trim())).Where<string>((Func<string, bool>) (i => !string.IsNullOrWhiteSpace(i))).ToHashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
