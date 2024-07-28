// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.RegistryService.Server.RegistryNotificationSet
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.RegistryService.Server
{
  public class RegistryNotificationSet : IEnumerable<RegistryCallbackEntry>, IEnumerable
  {
    private readonly Dictionary<RegistrySettingsChangedCallback, RegistryCallbackEntry> m_notifications;
    private readonly SparseTree<ICollection<RegistryNotificationSet.DispatchIndexEntry>> m_notificationsTree;
    private const int c_largeCollectionThreshold = 12;

    public RegistryNotificationSet()
    {
      this.m_notifications = new Dictionary<RegistrySettingsChangedCallback, RegistryCallbackEntry>(RegistrySettingsChangedCallbackComparer.Instance);
      this.m_notificationsTree = new SparseTree<ICollection<RegistryNotificationSet.DispatchIndexEntry>>('/', StringComparison.OrdinalIgnoreCase);
    }

    public Dictionary<RegistrySettingsChangedCallback, RegistryCallbackEntry>.ValueCollection.Enumerator GetEnumerator() => this.m_notifications.Values.GetEnumerator();

    IEnumerator<RegistryCallbackEntry> IEnumerable<RegistryCallbackEntry>.GetEnumerator() => (IEnumerator<RegistryCallbackEntry>) this.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public int Count => this.m_notifications.Count;

    public void AddNotification(
      RegistrySettingsChangedCallback callback,
      Guid serviceHostId,
      IEnumerable<RegistryQuery> filters,
      bool fallThru)
    {
      RegistryCallbackEntry registryCallbackEntry;
      if (this.m_notifications.TryGetValue(callback, out registryCallbackEntry))
      {
        registryCallbackEntry.IsFallThru |= fallThru;
      }
      else
      {
        registryCallbackEntry = new RegistryCallbackEntry(callback, serviceHostId, fallThru);
        this.m_notifications.Add(callback, registryCallbackEntry);
      }
      foreach (RegistryQuery filter in filters)
      {
        if (registryCallbackEntry.Filters.Add(filter))
          this.AddToDispatchIndex(callback, in filter);
      }
    }

    public bool RemoveNotification(RegistrySettingsChangedCallback callback, out bool isFallThru)
    {
      RegistryCallbackEntry registryCallbackEntry;
      if (!this.m_notifications.TryGetValue(callback, out registryCallbackEntry))
      {
        isFallThru = false;
        return false;
      }
      this.m_notifications.Remove(callback);
      foreach (RegistryQuery query in registryCallbackEntry.Filters)
      {
        ICollection<RegistryNotificationSet.DispatchIndexEntry> dispatchIndexEntries = this.m_notificationsTree[query.Path];
        dispatchIndexEntries.Remove(new RegistryNotificationSet.DispatchIndexEntry(in callback, in query));
        if (dispatchIndexEntries.Count == 0)
          this.m_notificationsTree.Remove(query.Path, false);
      }
      isFallThru = registryCallbackEntry.IsFallThru;
      return true;
    }

    public void ProcessNotificationsForRegistryItem(
      RegistryItem item,
      RegistryNotificationSet.ProcessNotificationsEvaluator evaluate)
    {
      this.m_notificationsTree.EnumAndEvaluateParents(item.Path, EnumParentsOptions.None, (SparseTree<ICollection<RegistryNotificationSet.DispatchIndexEntry>>.EnumNodeCallback) ((token, collection, noChildrenBelow, IsExactMatch) =>
      {
        foreach (RegistryNotificationSet.DispatchIndexEntry dispatchIndexEntry in (IEnumerable<RegistryNotificationSet.DispatchIndexEntry>) collection)
        {
          if (dispatchIndexEntry.Query.Matches(item.Path))
            evaluate(new RegistryCallbackAndServiceHost(dispatchIndexEntry.Callback, this.m_notifications[dispatchIndexEntry.Callback].ServiceHostId));
        }
        return true;
      }));
    }

    private void AddToDispatchIndex(
      RegistrySettingsChangedCallback callback,
      in RegistryQuery filter)
    {
      ICollection<RegistryNotificationSet.DispatchIndexEntry> collection = this.m_notificationsTree.GetOrAdd<object>(filter.Path, (Func<object, ICollection<RegistryNotificationSet.DispatchIndexEntry>>) (x => (ICollection<RegistryNotificationSet.DispatchIndexEntry>) new List<RegistryNotificationSet.DispatchIndexEntry>(1)));
      if (collection.Count == 12 && collection is List<RegistryNotificationSet.DispatchIndexEntry>)
      {
        collection = (ICollection<RegistryNotificationSet.DispatchIndexEntry>) new HashSet<RegistryNotificationSet.DispatchIndexEntry>((IEnumerable<RegistryNotificationSet.DispatchIndexEntry>) collection);
        this.m_notificationsTree[filter.Path] = collection;
      }
      collection.Add(new RegistryNotificationSet.DispatchIndexEntry(in callback, in filter));
    }

    public delegate void ProcessNotificationsEvaluator(in RegistryCallbackAndServiceHost entry);

    private readonly struct DispatchIndexEntry : 
      IEquatable<RegistryNotificationSet.DispatchIndexEntry>
    {
      public readonly RegistrySettingsChangedCallback Callback;
      public readonly RegistryQuery Query;

      public DispatchIndexEntry(in RegistrySettingsChangedCallback callback, in RegistryQuery query)
      {
        this.Callback = callback;
        this.Query = query;
      }

      public bool Equals(RegistryNotificationSet.DispatchIndexEntry other) => RegistrySettingsChangedCallbackComparer.Instance.Equals(this.Callback, other.Callback) && RegistryQueryComparer.Instance.Equals(this.Query, other.Query);

      public override int GetHashCode() => RegistrySettingsChangedCallbackComparer.Instance.GetHashCode(this.Callback) ^ RegistryQueryComparer.Instance.GetHashCode(this.Query);
    }
  }
}
