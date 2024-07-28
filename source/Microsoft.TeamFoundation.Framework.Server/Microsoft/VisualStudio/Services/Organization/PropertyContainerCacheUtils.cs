// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyContainerCacheUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  internal static class PropertyContainerCacheUtils
  {
    private const string c_wildCardAll = "*";
    internal static readonly StringComparer PropertyNameComparer = StringComparer.OrdinalIgnoreCase;

    internal static T CloneAndTrimProperties<T>(T container, IEnumerable<string> propertiesToKeep) where T : ICloneable, IPropertyContainer
    {
      T container1 = (T) container.Clone();
      PropertyContainerCacheUtils.TrimProperties<T>(container1, propertiesToKeep);
      return container1;
    }

    private static void TrimProperties<T>(T container, IEnumerable<string> propertiesToKeep) where T : IPropertyContainer
    {
      if ((object) container == null || container.Properties == null || container.Properties.Count == 0)
        return;
      if (propertiesToKeep.IsNullOrEmpty<string>())
      {
        container.Properties.Clear();
      }
      else
      {
        if (propertiesToKeep.Contains<string>("*"))
          return;
        HashSet<string> stringSet = new HashSet<string>(container.Properties.Keys, (IEqualityComparer<string>) PropertyContainerCacheUtils.PropertyNameComparer);
        stringSet.ExceptWith(propertiesToKeep);
        foreach (string key in stringSet)
          container.Properties.Remove(key);
      }
    }

    internal static IEnumerable<string> MergeWithPrefetchedProperties(
      IEnumerable<string> prefetchedPropertyNames,
      IEnumerable<string> requestedPropertyNames)
    {
      if (prefetchedPropertyNames.IsNullOrEmpty<string>())
        return requestedPropertyNames;
      if (requestedPropertyNames.IsNullOrEmpty<string>())
        return prefetchedPropertyNames;
      if (requestedPropertyNames.Contains<string>("*"))
        return requestedPropertyNames;
      HashSet<string> stringSet = new HashSet<string>(requestedPropertyNames, (IEqualityComparer<string>) PropertyContainerCacheUtils.PropertyNameComparer);
      stringSet.UnionWith(prefetchedPropertyNames);
      return (IEnumerable<string>) stringSet;
    }
  }
}
