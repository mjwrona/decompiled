// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PropertyContainerCacheBase`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Organization
{
  public abstract class PropertyContainerCacheBase<T> where T : ICloneable, IPropertyContainer
  {
    private const string c_area = "Organization";
    private const string c_layer = "PropertyContainerCacheBase";

    internal abstract bool TryGet(
      IVssRequestContext context,
      Guid id,
      out PropertyContainerCacheEntry<T> cachedEntry);

    internal abstract void Set(
      IVssRequestContext context,
      Guid id,
      PropertyContainerCacheEntry<T> cacheEntry);

    public abstract bool Remove(IVssRequestContext context, Guid id);

    public T Get(IVssRequestContext context, Guid id, IEnumerable<string> propertyNames)
    {
      T container;
      this.TryGet(context, id, propertyNames, out container);
      return container;
    }

    public bool TryGet(
      IVssRequestContext context,
      Guid id,
      IEnumerable<string> propertyNames,
      out T container)
    {
      container = default (T);
      try
      {
        PropertyContainerCacheEntry<T> cachedEntry;
        if (PropertyContainerCacheBase<T>.CacheBypassRequested(context) || !this.TryGet(context, id, out cachedEntry) || (propertyNames.IsNullOrEmpty<string>() ? 1 : (propertyNames.All<string>((Func<string, bool>) (x => cachedEntry.PropertyNames.Contains(x))) ? 1 : 0)) == 0)
          return false;
        container = PropertyContainerCacheUtils.CloneAndTrimProperties<T>(cachedEntry.PropertyContainer, propertyNames);
        return true;
      }
      catch (Exception ex)
      {
        context.TraceException(5004248, "Organization", nameof (PropertyContainerCacheBase<T>), ex);
        return false;
      }
    }

    public void Update(IVssRequestContext context, T container, IEnumerable<string> propertyNames)
    {
      if ((object) container == null)
        return;
      try
      {
        PropertyContainerCacheEntry<T> cachedEntry;
        if (this.TryGet(context, container.Id, out cachedEntry))
        {
          PropertyContainerCacheEntry<T> mergedCacheEntry;
          PropertyContainerCacheBase<T>.MergeLatestWithCached(container, propertyNames, cachedEntry, out mergedCacheEntry);
          this.Set(context, container.Id, mergedCacheEntry);
        }
        else
        {
          HashSet<string> propertyNames1 = new HashSet<string>(propertyNames ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) PropertyContainerCacheUtils.PropertyNameComparer);
          this.Set(context, container.Id, new PropertyContainerCacheEntry<T>((T) container.Clone(), propertyNames1));
        }
      }
      catch (Exception ex)
      {
        context.TraceException(5004238, "Organization", nameof (PropertyContainerCacheBase<T>), ex);
      }
    }

    private static void MergeLatestWithCached(
      T latestContainer,
      IEnumerable<string> requestedPropertyNames,
      PropertyContainerCacheEntry<T> cachedEntry,
      out PropertyContainerCacheEntry<T> mergedCacheEntry)
    {
      HashSet<string> propertyNames = new HashSet<string>(requestedPropertyNames ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) PropertyContainerCacheUtils.PropertyNameComparer);
      propertyNames.UnionWith((IEnumerable<string>) cachedEntry.PropertyNames ?? Enumerable.Empty<string>());
      if (cachedEntry.PropertyContainer.Properties != null)
      {
        T propertyContainer1 = cachedEntry.PropertyContainer;
        if (propertyContainer1.Properties.Count != 0)
        {
          HashSet<string> enumerable = new HashSet<string>((IEnumerable<string>) cachedEntry.PropertyNames ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) PropertyContainerCacheUtils.PropertyNameComparer);
          enumerable.ExceptWith(requestedPropertyNames ?? Enumerable.Empty<string>());
          if (enumerable.IsNullOrEmpty<string>())
          {
            mergedCacheEntry = new PropertyContainerCacheEntry<T>((T) latestContainer.Clone(), propertyNames);
            return;
          }
          T propertyContainer2 = (T) latestContainer.Clone();
          bool flag = false;
          foreach (string key in enumerable)
          {
            propertyContainer1 = cachedEntry.PropertyContainer;
            object obj;
            if (propertyContainer1.Properties.TryGetValue(key, out obj))
            {
              propertyContainer2.Properties[key] = obj;
              flag = true;
            }
          }
          if (flag)
            propertyContainer2.Properties.Reset((IDictionary<string, object>) propertyContainer2.Properties.ToPropertiesCollection());
          mergedCacheEntry = new PropertyContainerCacheEntry<T>(propertyContainer2, propertyNames);
          return;
        }
      }
      mergedCacheEntry = new PropertyContainerCacheEntry<T>((T) latestContainer.Clone(), propertyNames);
    }

    private static bool CacheBypassRequested(IVssRequestContext context)
    {
      object obj;
      return context.IsSystemContext && context.Items != null && context.Items.TryGetValue("Organization.Cache.Bypass", out obj) && obj is bool flag && flag;
    }
  }
}
