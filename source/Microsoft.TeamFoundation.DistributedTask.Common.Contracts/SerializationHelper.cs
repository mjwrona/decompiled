// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Common.Contracts.SerializationHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Common.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F0CB8220-D93B-49F7-B603-A5F8DA1FAAC3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Common.Contracts
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
      bool clearSource = false)
    {
      SerializationHelper.Copy<TKey, TValue>(ref source, ref target, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default, clearSource);
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
