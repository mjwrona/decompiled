// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssMemoryCacheListExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class VssMemoryCacheListExtensions
  {
    public static bool TryGetValueKeepTtl<TKey, TValue>(
      this VssMemoryCacheList<TKey, TValue> cache,
      TKey key,
      out TValue value)
    {
      DateTime dateTime;
      return cache.TryGetValue(key, out value, out dateTime, out dateTime, false);
    }

    public static bool TryGetValueKeepTtl<TKey, TValue>(
      this VssMemoryCacheList<TKey, TValue> cache,
      TKey key,
      out TValue value,
      out DateTime modifiedOn,
      out DateTime accessedOn)
    {
      return cache.TryGetValue(key, out value, out modifiedOn, out accessedOn, false);
    }

    public static bool TryGetValueUpdateTtl<TKey, TValue>(
      this VssMemoryCacheList<TKey, TValue> cache,
      TKey key,
      out TValue value)
    {
      DateTime dateTime;
      return cache.TryGetValue(key, out value, out dateTime, out dateTime, true);
    }

    public static bool TryGetValueUpdateTtl<TKey, TValue>(
      this VssMemoryCacheList<TKey, TValue> cache,
      TKey key,
      out TValue value,
      out DateTime modifiedOn,
      out DateTime accessedOn)
    {
      return cache.TryGetValue(key, out value, out modifiedOn, out accessedOn, true);
    }
  }
}
