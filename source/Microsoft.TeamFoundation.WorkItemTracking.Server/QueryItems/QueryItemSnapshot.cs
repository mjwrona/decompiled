// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemSnapshot
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemSnapshot : ICloneable
  {
    internal QueryItemSnapshot() => this.QueryItemsDictionary = new Dictionary<Guid, QueryItem>();

    internal virtual long Watermark { get; private set; }

    internal virtual QueryFolder Root { get; private set; }

    internal virtual void Initialize(QueryItemEntry rootData, long newwatermark)
    {
      ArgumentUtility.CheckForNull<QueryItemEntry>(rootData, "rootsData");
      this.Root = QueryItem.Create(rootData) as QueryFolder;
      this.QueryItemsDictionary = new Dictionary<Guid, QueryItem>();
      this.BuildDictionary((QueryItem) this.Root);
      this.Watermark = newwatermark;
    }

    internal virtual void Merge(IEnumerable<QueryItemEntry> queryItemsToMerge, long newwatermark)
    {
      this.Watermark = newwatermark;
      if (queryItemsToMerge == null)
        return;
      Dictionary<Guid, QueryItemEntry> dictionary = new Dictionary<Guid, QueryItemEntry>();
      foreach (QueryItemEntry queryItemEntry in queryItemsToMerge)
      {
        queryItemEntry.Path = string.Empty;
        queryItemEntry.SecurityToken = string.Empty;
        dictionary.GetOrAdd<Guid, QueryItemEntry>(queryItemEntry.Id, queryItemEntry);
      }
      HashSet<Guid> mergedQueryItems = new HashSet<Guid>();
      foreach (KeyValuePair<Guid, QueryItemEntry> keyValuePair in dictionary)
      {
        if (!mergedQueryItems.Contains(keyValuePair.Key))
          this.AddQueryItemEntry(dictionary, keyValuePair.Value, mergedQueryItems);
      }
    }

    public virtual object Clone()
    {
      QueryItemSnapshot queryItemSnapshot = new QueryItemSnapshot();
      if (this.Root != null)
        queryItemSnapshot.Root = this.Root.Clone() as QueryFolder;
      queryItemSnapshot.BuildDictionary((QueryItem) queryItemSnapshot.Root);
      queryItemSnapshot.Watermark = this.Watermark;
      return (object) queryItemSnapshot;
    }

    protected virtual Dictionary<Guid, QueryItem> QueryItemsDictionary { get; set; }

    public int TotalQueryItemCount => this.QueryItemsDictionary.Count;

    protected virtual void AddQueryItemEntry(
      Dictionary<Guid, QueryItemEntry> dictionaryOfQueryItemsToMerge,
      QueryItemEntry queryItemEntryToMerge,
      HashSet<Guid> mergedQueryItems)
    {
      QueryItem newQueryItem = QueryItem.Create(queryItemEntryToMerge);
      QueryItem valueOrDefault1 = this.QueryItemsDictionary.GetValueOrDefault<Guid, QueryItem>(newQueryItem.Id, (QueryItem) null);
      if (queryItemEntryToMerge.IsDeleted || !queryItemEntryToMerge.IsPublic)
      {
        if (valueOrDefault1 != null)
          this.DeleteQueryItemFromTree(valueOrDefault1.Id);
        mergedQueryItems.Add(queryItemEntryToMerge.Id);
      }
      else
      {
        this.AddOrUpdateParent(dictionaryOfQueryItemsToMerge, newQueryItem, valueOrDefault1, mergedQueryItems);
        mergedQueryItems.Add(queryItemEntryToMerge.Id);
        QueryItem valueOrDefault2 = this.QueryItemsDictionary.GetValueOrDefault<Guid, QueryItem>(newQueryItem.ParentId, (QueryItem) null);
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        string str1;
        string str2;
        Guid guid;
        if (valueOrDefault2 != null)
        {
          str1 = valueOrDefault2.Path + "/";
          str2 = valueOrDefault2.SecurityToken + "/";
        }
        else
        {
          str1 = string.Empty;
          guid = newQueryItem.ProjectId;
          str2 = "$/" + guid.ToString() + "/";
        }
        newQueryItem.Path = str1 + newQueryItem.Name;
        QueryItem queryItem = newQueryItem;
        string str3 = str2;
        guid = newQueryItem.Id;
        string str4 = guid.ToString();
        string str5 = str3 + str4;
        queryItem.SecurityToken = str5;
        if (valueOrDefault1 != null)
        {
          QueryFolder queryFolder1 = newQueryItem as QueryFolder;
          QueryFolder queryFolder2 = valueOrDefault1 as QueryFolder;
          if (queryFolder1 != null && queryFolder2 != null)
          {
            queryFolder1.Children = queryFolder2.Children;
            queryFolder1.HasChildren = queryFolder2.HasChildren;
          }
        }
        this.QueryItemsDictionary.AddOrUpdate<Guid, QueryItem>(queryItemEntryToMerge.Id, newQueryItem, (Func<Guid, QueryItem, QueryItem>) ((key, oldvalue) => newQueryItem));
        if (!(this.Root.Id == newQueryItem.Id))
          return;
        this.Root = newQueryItem as QueryFolder;
      }
    }

    protected virtual void DeleteQueryItemFromTree(Guid queryItemToRemove)
    {
      QueryItem queryItem1;
      QueryItem queryItem2;
      if (!this.QueryItemsDictionary.TryRemove<Guid, QueryItem>(queryItemToRemove, out queryItem1) || !(queryItem1.ParentId != Guid.Empty) || !this.QueryItemsDictionary.TryGetValue(queryItem1.ParentId, out queryItem2) || !(queryItem2 is QueryFolder queryFolder) || queryFolder.Children == null)
        return;
      queryFolder.Children = (IList<QueryItem>) queryFolder.Children.Where<QueryItem>((Func<QueryItem, bool>) (c => c.Id != queryItemToRemove)).ToList<QueryItem>();
    }

    protected virtual void AddOrUpdateParent(
      Dictionary<Guid, QueryItemEntry> changedQueryItemDictionary,
      QueryItem newQueryItem,
      QueryItem oldQueryItem,
      HashSet<Guid> mergedQueryItems)
    {
      if (newQueryItem.ParentId == Guid.Empty)
        return;
      if (oldQueryItem != null && newQueryItem.ParentId == oldQueryItem.ParentId)
      {
        QueryFolder valueOrDefault = this.QueryItemsDictionary.GetValueOrDefault<Guid, QueryItem>(newQueryItem.ParentId, (QueryItem) null) as QueryFolder;
        valueOrDefault.Children.Remove(oldQueryItem);
        valueOrDefault.Children.Add(newQueryItem);
      }
      else
      {
        if (!(this.QueryItemsDictionary.GetValueOrDefault<Guid, QueryItem>(newQueryItem.ParentId, (QueryItem) null) is QueryFolder valueOrDefault1))
        {
          QueryItemEntry queryItemEntryToMerge;
          if (!changedQueryItemDictionary.TryGetValue(newQueryItem.ParentId, out queryItemEntryToMerge))
            throw new QueryItemCacheParentNotFoundException(newQueryItem.Id, newQueryItem.ParentId, this.Watermark);
          this.AddQueryItemEntry(changedQueryItemDictionary, queryItemEntryToMerge, mergedQueryItems);
          valueOrDefault1 = this.QueryItemsDictionary.GetValueOrDefault<Guid, QueryItem>(newQueryItem.ParentId, (QueryItem) null) as QueryFolder;
        }
        if (oldQueryItem != null && oldQueryItem.ParentId != newQueryItem.ParentId && this.QueryItemsDictionary.GetValueOrDefault<Guid, QueryItem>(oldQueryItem.ParentId, (QueryItem) null) is QueryFolder valueOrDefault2)
          valueOrDefault2.Children = (IList<QueryItem>) valueOrDefault2.Children.Where<QueryItem>((Func<QueryItem, bool>) (c => c.Id != newQueryItem.Id)).ToList<QueryItem>();
        if (valueOrDefault1.Children.Any<QueryItem>((Func<QueryItem, bool>) (c => c.Id == newQueryItem.Id)))
          return;
        valueOrDefault1.Children.Add(newQueryItem);
      }
    }

    protected virtual void BuildDictionary(QueryItem queryItem)
    {
      this.QueryItemsDictionary.AddOrUpdate<Guid, QueryItem>(queryItem.Id, queryItem, (Func<Guid, QueryItem, QueryItem>) ((key, oldvalue) => queryItem));
      if (!(queryItem is QueryFolder queryFolder))
        return;
      foreach (QueryItem child in (IEnumerable<QueryItem>) queryFolder.Children)
        this.BuildDictionary(child);
    }
  }
}
