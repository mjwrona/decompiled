// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.SerializationHelper
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal static class SerializationHelper
  {
    public static void Copy<T>(ref List<T> source, ref List<T> target, bool clearSource = false)
    {
      if (source == null || source.Count <= 0)
        return;
      target = new List<T>((IEnumerable<T>) source);
      if (!clearSource)
        return;
      source = (List<T>) null;
    }

    public static void Copy<TKey, TValue>(
      ref IDictionary<TKey, TValue> source,
      ref IDictionary<TKey, TValue> target,
      IEqualityComparer<TKey> comparer,
      bool clearSource = false)
    {
      if (source == null || source.Count <= 0)
        return;
      target = (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>(source, comparer);
      if (!clearSource)
        return;
      source = (IDictionary<TKey, TValue>) null;
    }
  }
}
