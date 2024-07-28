// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.FolderItemExtensions
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  internal static class FolderItemExtensions
  {
    private static readonly ThreadLocal<System.Random> Random;
    private static int rndSeed = Environment.TickCount;

    static FolderItemExtensions() => FolderItemExtensions.Random = new ThreadLocal<System.Random>((Func<System.Random>) (() => new System.Random(Interlocked.Increment(ref FolderItemExtensions.rndSeed))));

    public static T UpdateChildEtag<T>(this T item) where T : StoredItem
    {
      item.StorageETag = item["cr:childEtag"];
      return item;
    }

    public static void SetChildEtag(this StoredItem item, string etag)
    {
      item.StorageETag = etag;
      item["cr:childEtag"] = etag;
    }

    internal static Dictionary<Locator, ContainedItem> GetChildrenItems(this FolderItem parent)
    {
      Dictionary<Locator, ContainedItem> childrenItems = new Dictionary<Locator, ContainedItem>();
      if (!parent.HasChildren)
        return childrenItems;
      foreach (ContainedItem containedItem in parent.Children.Select<Item, ContainedItem>((Func<Item, ContainedItem>) (c => c.Convert<ContainedItem>())))
      {
        containedItem.UpdateChildEtag<ContainedItem>();
        childrenItems.Add(containedItem.InternalPath, containedItem);
      }
      return childrenItems;
    }

    internal static bool AddOrReplaceChild(
      this FolderItem parent,
      Locator path,
      StoredItem item,
      out bool itemModified)
    {
      Dictionary<Locator, StoredItem> newChildren = new Dictionary<Locator, StoredItem>()
      {
        {
          path,
          item
        }
      };
      return parent.AddOrReplaceChildren((IReadOnlyDictionary<Locator, StoredItem>) newChildren, false, out itemModified).Single<KeyValuePair<Locator, bool>>().Value;
    }

    internal static Dictionary<Locator, bool> AddOrReplaceChildren(
      this FolderItem parent,
      IReadOnlyDictionary<Locator, StoredItem> newChildren,
      bool atomicDirectory,
      out bool itemModified)
    {
      itemModified = false;
      Dictionary<Locator, bool> dictionary1 = new Dictionary<Locator, bool>();
      Dictionary<Locator, ContainedItem> dictionary2 = parent.HasChildren ? parent.GetChildrenItems() : new Dictionary<Locator, ContainedItem>();
      bool flag1 = false;
      if (atomicDirectory)
      {
        flag1 = true;
        foreach (KeyValuePair<Locator, StoredItem> newChild in (IEnumerable<KeyValuePair<Locator, StoredItem>>) newChildren)
        {
          Locator key = newChild.Key;
          flag1 &= FolderItemExtensions.EtagMatches(dictionary2, key, newChild.Value.StorageETag);
        }
      }
      foreach (KeyValuePair<Locator, StoredItem> newChild in (IEnumerable<KeyValuePair<Locator, StoredItem>>) newChildren)
      {
        Locator key = newChild.Key;
        bool flag2 = false;
        if ((atomicDirectory ? (flag1 ? 1 : 0) : (FolderItemExtensions.EtagMatches(dictionary2, key, newChild.Value.StorageETag) ? 1 : 0)) != 0)
        {
          ContainedItem containedItem = newChild.Value.Convert<ContainedItem>();
          containedItem.InternalPath = key;
          containedItem.SetChildEtag(FolderItemExtensions.Random.Value.Next().ToString());
          newChild.Value.StorageETag = containedItem.StorageETag;
          dictionary2[key] = containedItem;
          flag2 = true;
          itemModified = true;
        }
        dictionary1[newChild.Key] = flag2;
      }
      parent.Children = (IEnumerable<Item>) dictionary2.Values<Locator, ContainedItem>();
      return dictionary1;
    }

    internal static IDictionary<ShardableLocator, bool> RemoveChildren(
      this FolderItem parent,
      IReadOnlyDictionary<ShardableLocator, string> removeChildren,
      out bool itemModified)
    {
      itemModified = false;
      if (!parent.HasChildren)
        return (IDictionary<ShardableLocator, bool>) removeChildren.ToDictionary<KeyValuePair<ShardableLocator, string>, ShardableLocator, bool>((Func<KeyValuePair<ShardableLocator, string>, ShardableLocator>) (kvp => kvp.Key), (Func<KeyValuePair<ShardableLocator, string>, bool>) (kvp => false));
      Dictionary<ShardableLocator, bool> dictionary = new Dictionary<ShardableLocator, bool>();
      Dictionary<Locator, ContainedItem> childrenItems = parent.GetChildrenItems();
      foreach (KeyValuePair<ShardableLocator, string> removeChild in (IEnumerable<KeyValuePair<ShardableLocator, string>>) removeChildren)
      {
        Locator locator = removeChild.Key.Locator;
        bool flag = false;
        if (removeChild.Value == null || FolderItemExtensions.EtagMatches(childrenItems, locator, removeChild.Value))
        {
          flag = childrenItems.Remove(locator);
          itemModified = true;
        }
        dictionary[removeChild.Key] = flag;
      }
      parent.Children = (IEnumerable<Item>) childrenItems.Values<Locator, ContainedItem>();
      return (IDictionary<ShardableLocator, bool>) dictionary;
    }

    internal static IDictionary<ShardableLocator, T> GetChildren<T>(
      this FolderItem folder,
      IEnumerable<ShardableLocator> paths)
      where T : StoredItem
    {
      Dictionary<ShardableLocator, T> children = new Dictionary<ShardableLocator, T>();
      if (!folder.HasChildren)
        return (IDictionary<ShardableLocator, T>) paths.ToDictionary<ShardableLocator, ShardableLocator, T>((Func<ShardableLocator, ShardableLocator>) (path => path), (Func<ShardableLocator, T>) (path => default (T)));
      Dictionary<Locator, ContainedItem> childrenItems = folder.GetChildrenItems();
      foreach (ShardableLocator path in paths)
        children[path] = childrenItems.ContainsKey(path.Locator) ? childrenItems[path.Locator].Convert<T>().UpdateChildEtag<T>() : default (T);
      return (IDictionary<ShardableLocator, T>) children;
    }

    private static bool EtagMatches(
      Dictionary<Locator, ContainedItem> children,
      Locator path,
      string previousEtag)
    {
      ContainedItem containedItem;
      return children.TryGetValue(path, out containedItem) ? containedItem.StorageETag.Equals(previousEtag) : previousEtag == null;
    }
  }
}
