// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AfdRouteKeyUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class AfdRouteKeyUtils
  {
    private static readonly char[] s_routeKeySeparators = new char[1]
    {
      '|'
    };

    public static string ExtractUnversionedRouteKey(string routeKeys, int version)
    {
      foreach (string splitRouteKey in (IEnumerable<string>) AfdRouteKeyUtils.SplitRouteKeys(routeKeys))
      {
        int version1;
        string unversionedRouteKey;
        if (AfdRouteKeyUtils.TryParseRouteKey(splitRouteKey, out version1, out unversionedRouteKey) && version1 == version)
          return unversionedRouteKey;
      }
      return (string) null;
    }

    public static IList<VersionedRouteKey> ExtractAllRouteKeys(string routeKeys)
    {
      List<VersionedRouteKey> source = new List<VersionedRouteKey>();
      foreach (string splitRouteKey in (IEnumerable<string>) AfdRouteKeyUtils.SplitRouteKeys(routeKeys))
      {
        int version;
        string unversionedRouteKey;
        if (AfdRouteKeyUtils.TryParseRouteKey(splitRouteKey, out version, out unversionedRouteKey))
          source.Add(new VersionedRouteKey(version, unversionedRouteKey));
      }
      return (IList<VersionedRouteKey>) source.OrderByDescending<VersionedRouteKey, int>((Func<VersionedRouteKey, int>) (k => k.Version)).ToList<VersionedRouteKey>();
    }

    private static IList<string> SplitRouteKeys(string routeKeys) => string.IsNullOrEmpty(routeKeys) ? (IList<string>) Array.Empty<string>() : (IList<string>) routeKeys.Split(AfdRouteKeyUtils.s_routeKeySeparators, StringSplitOptions.RemoveEmptyEntries);

    private static bool TryParseRouteKey(
      string routeKey,
      out int version,
      out string unversionedRouteKey)
    {
      if (string.IsNullOrEmpty(routeKey))
      {
        version = -1;
        unversionedRouteKey = (string) null;
      }
      else if (AfdRouteKeyUtils.IsUnversioned(routeKey))
      {
        version = 0;
        unversionedRouteKey = routeKey;
      }
      else
      {
        version = int.Parse(routeKey.Substring(1, 2));
        unversionedRouteKey = routeKey.Substring(4);
      }
      return !string.IsNullOrEmpty(unversionedRouteKey);
    }

    private static bool IsUnversioned(string routeKey) => routeKey.Length < 4 || routeKey[0] != 'v' || !char.IsDigit(routeKey[1]) || !char.IsDigit(routeKey[2]) || routeKey[3] != '=';
  }
}
