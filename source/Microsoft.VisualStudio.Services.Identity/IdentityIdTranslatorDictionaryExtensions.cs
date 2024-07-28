// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdTranslatorDictionaryExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityIdTranslatorDictionaryExtensions
  {
    public static void TryAdd<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
    {
      if (dictionary.ContainsKey(key))
        return;
      dictionary.Add(key, value);
    }

    public static bool TryRemove<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, out T2 value)
    {
      value = default (T2);
      if (!dictionary.ContainsKey(key))
        return false;
      value = dictionary[key];
      dictionary.Remove(key);
      return true;
    }
  }
}
