// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.MsalUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class MsalUtility
  {
    private static readonly ConcurrentDictionary<string, string[]> s_scopeDictionary = new ConcurrentDictionary<string, string[]>((IEqualityComparer<string>) StringComparer.Ordinal);

    public static string[] GetScopes(string resource)
    {
      string[] scopes;
      if (!MsalUtility.s_scopeDictionary.TryGetValue(resource, out scopes))
      {
        scopes = new string[1]{ resource + "/.default" };
        MsalUtility.s_scopeDictionary.TryAdd(resource, scopes);
      }
      return scopes;
    }
  }
}
