// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Extensions
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Common
{
  internal static class Extensions
  {
    internal static bool GetOrAdd<TKey, TValue>(
      this ConcurrentDictionary<TKey, TValue> dict,
      TKey key,
      TValue value,
      out TValue finalValue)
    {
      while (!dict.TryGetValue(key, out finalValue))
      {
        if (dict.TryAdd(key, value))
        {
          finalValue = value;
          return true;
        }
      }
      return false;
    }
  }
}
