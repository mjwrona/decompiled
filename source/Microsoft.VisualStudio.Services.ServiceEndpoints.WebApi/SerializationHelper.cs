// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.SerializationHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  public static class SerializationHelper
  {
    public static void Copy<T>(ref List<T> source, ref List<T> target, bool clearSource = false)
    {
      if (source != null && source.Count > 0)
        target = new List<T>((IEnumerable<T>) source);
      if (!clearSource)
        return;
      source = (List<T>) null;
    }

    public static void Copy<T>(
      ref IList<T> source,
      ref ISet<T> target,
      IEqualityComparer<T> comparer,
      bool clearSource = false)
    {
      if (source != null && source.Count > 0)
        target = (ISet<T>) new HashSet<T>((IEnumerable<T>) source, comparer);
      if (!clearSource)
        return;
      source = (IList<T>) null;
    }

    public static void Copy<T>(ref ISet<T> source, ref IList<T> target, bool clearSource = false)
    {
      if (source != null && source.Count > 0)
        target = (IList<T>) new List<T>((IEnumerable<T>) source);
      if (!clearSource)
        return;
      source = (ISet<T>) null;
    }

    public static void Copy<TKey, TValue>(
      ref Dictionary<TKey, TValue> source,
      ref Dictionary<TKey, TValue> target,
      bool clearSource = false)
    {
      SerializationHelper.Copy<TKey, TValue>(ref source, ref target, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default, clearSource);
    }

    public static void Copy<TKey, TValue>(
      ref IDictionary<TKey, TValue> source,
      ref IDictionary<TKey, TValue> target,
      bool clearSource = false)
    {
      SerializationHelper.Copy<TKey, TValue>(ref source, ref target, (IEqualityComparer<TKey>) EqualityComparer<TKey>.Default, clearSource);
    }

    public static void Copy<TKey, TValue>(
      ref Dictionary<TKey, TValue> source,
      ref Dictionary<TKey, TValue> target,
      IEqualityComparer<TKey> comparer,
      bool clearSource = false)
    {
      if (source != null && source.Count > 0)
        target = new Dictionary<TKey, TValue>((IDictionary<TKey, TValue>) source, comparer);
      if (!clearSource)
        return;
      source = (Dictionary<TKey, TValue>) null;
    }

    public static void Copy<TKey, TValue>(
      ref IDictionary<TKey, TValue> source,
      ref IDictionary<TKey, TValue> target,
      IEqualityComparer<TKey> comparer,
      bool clearSource = false)
    {
      if (source != null && source.Count > 0)
        target = (IDictionary<TKey, TValue>) new Dictionary<TKey, TValue>(source, comparer);
      if (!clearSource)
        return;
      source = (IDictionary<TKey, TValue>) null;
    }
  }
}
