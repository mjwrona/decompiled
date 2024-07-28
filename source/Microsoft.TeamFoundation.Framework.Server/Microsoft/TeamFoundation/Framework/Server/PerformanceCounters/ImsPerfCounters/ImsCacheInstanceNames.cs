// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.ImsPerfCounters.ImsCacheInstanceNames
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.ImsPerfCounters
{
  internal static class ImsCacheInstanceNames
  {
    private static ConcurrentDictionary<Type, ConcurrentDictionary<Type, string>> instanceNames = new ConcurrentDictionary<Type, ConcurrentDictionary<Type, string>>();

    internal static string GetInstancename<K, V>()
      where K : ImsCacheKey
      where V : ImsCacheObject
    {
      return ImsCacheInstanceNames.GetInstancename<V>(typeof (K));
    }

    internal static string GetInstancename<V>(Type keyType) where V : ImsCacheObject
    {
      return ImsCacheInstanceNames.instanceNames.GetOrAdd(keyType, (Func<Type, ConcurrentDictionary<Type, string>>) (_ => new ConcurrentDictionary<Type, string>())).GetOrAdd(typeof (V), (Func<Type, string>) (v => toFriendlyName(keyType) + "_" + toFriendlyName(v)));

      static string toFriendlyName(Type type) => ((IEnumerable<string>) type.ToString().Split('.')).Last<string>();
    }
  }
}
