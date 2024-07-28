// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.EntityDiffLogger
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal abstract class EntityDiffLogger
  {
    internal abstract void LogDiff(IVssRequestContext requestContext);

    protected static IEnumerable<Tuple<T, T, IList<string>>> CompareCollections<T, TKey>(
      IEnumerable<T> lhsSet,
      IEnumerable<T> rhsSet,
      Func<T, TKey> keySelector,
      Func<T, T, Tuple<bool, IList<string>>> isMatch)
    {
      Dictionary<TKey, T> rhsMap = rhsSet.ToDictionary<T, TKey>((Func<T, TKey>) (x => keySelector(x)));
      foreach (T lhs in lhsSet)
      {
        TKey key = keySelector(lhs);
        T obj;
        if (rhsMap.TryGetValue(key, out obj))
        {
          Tuple<bool, IList<string>> tuple = isMatch(lhs, obj);
          if (!tuple.Item1)
            yield return Tuple.Create<T, T, IList<string>>(lhs, obj, tuple.Item2);
        }
        else
          yield return Tuple.Create<T, T, IList<string>>(lhs, default (T), (IList<string>) null);
        rhsMap.Remove(key);
        key = default (TKey);
      }
      foreach (T obj in rhsMap.Values)
        yield return Tuple.Create<T, T, IList<string>>(default (T), obj, (IList<string>) null);
    }

    protected static void Log<T>(
      IVssRequestContext requestContext,
      IEnumerable<Tuple<T, T, IList<string>>> diffs,
      string area,
      string layer)
    {
      if (diffs.Any<Tuple<T, T, IList<string>>>((Func<Tuple<T, T, IList<string>>, bool>) (x => (object) x.Item2 == null)))
      {
        requestContext.Trace(251261, TraceLevel.Info, area, layer, "These entries exist only in SOURCE:");
        foreach (T obj in diffs.Where<Tuple<T, T, IList<string>>>((Func<Tuple<T, T, IList<string>>, bool>) (x => (object) x.Item2 == null)).Select<Tuple<T, T, IList<string>>, T>((Func<Tuple<T, T, IList<string>>, T>) (x => x.Item1)))
          requestContext.Trace(241262, TraceLevel.Info, area, layer, "SOURCE = " + EntityDiffLogger.ToString<T>(obj));
      }
      if (diffs.Any<Tuple<T, T, IList<string>>>((Func<Tuple<T, T, IList<string>>, bool>) (x => (object) x.Item1 == null)))
      {
        requestContext.Trace(251263, TraceLevel.Info, area, layer, "These entries exist only in TARGET:");
        foreach (T obj in diffs.Where<Tuple<T, T, IList<string>>>((Func<Tuple<T, T, IList<string>>, bool>) (x => (object) x.Item1 == null)).Select<Tuple<T, T, IList<string>>, T>((Func<Tuple<T, T, IList<string>>, T>) (x => x.Item2)))
          requestContext.Trace(241264, TraceLevel.Info, area, layer, "TARGET = " + EntityDiffLogger.ToString<T>(obj));
      }
      if (!diffs.Any<Tuple<T, T, IList<string>>>((Func<Tuple<T, T, IList<string>>, bool>) (x => (object) x.Item1 != null && (object) x.Item2 != null)))
        return;
      requestContext.Trace(251265, TraceLevel.Info, area, layer, "These entries exist in BOTH but do not match:");
      foreach (Tuple<T, T, IList<string>> tuple in diffs.Where<Tuple<T, T, IList<string>>>((Func<Tuple<T, T, IList<string>>, bool>) (x => (object) x.Item1 != null && (object) x.Item2 != null)))
      {
        requestContext.Trace(251266, TraceLevel.Info, area, layer, "SOURCE = " + EntityDiffLogger.ToString<T>(tuple.Item1) + ", TARGET = " + EntityDiffLogger.ToString<T>(tuple.Item2));
        if (!tuple.Item3.IsNullOrEmpty<string>())
          requestContext.Trace(251267, TraceLevel.Info, area, layer, "Mismatches = " + tuple.Item3.Serialize<IList<string>>());
      }
    }

    private static string ToString<T>(T value) => JsonConvert.SerializeObject((object) value, Formatting.None, new JsonSerializerSettings()
    {
      TypeNameHandling = TypeNameHandling.None
    });
  }
}
