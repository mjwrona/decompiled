// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RegistryServiceExtensions
  {
    public static void RegisterNotification(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      in RegistryQuery filter)
    {
      registryService.RegisterNotification(requestContext, callback, false, (IEnumerable<RegistryQuery>) new RegistryQuery[1]
      {
        filter
      });
    }

    public static void RegisterNotification(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      in RegistryQuery filter)
    {
      registryService.RegisterNotification(requestContext, callback, (fallThru ? 1 : 0) != 0, (IEnumerable<RegistryQuery>) new RegistryQuery[1]
      {
        filter
      });
    }

    public static void RegisterNotification(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      params string[] filters)
    {
      registryService.RegisterNotification(requestContext, callback, false, (IEnumerable<string>) filters);
    }

    public static void RegisterNotification(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      params string[] filters)
    {
      registryService.RegisterNotification(requestContext, callback, fallThru, (IEnumerable<string>) filters);
    }

    public static void RegisterNotification(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      IEnumerable<string> filters)
    {
      registryService.RegisterNotification(requestContext, callback, false, filters);
    }

    public static void RegisterNotification(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      IEnumerable<string> filters,
      Guid serviceHostId = default (Guid))
    {
      registryService.RegisterNotification(requestContext, callback, fallThru, filters.Select<string, RegistryQuery>((Func<string, RegistryQuery>) (s => new RegistryQuery(s))), serviceHostId);
    }

    public static string GetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      string defaultValue)
    {
      return registryService.GetValue(requestContext, in query, false, defaultValue);
    }

    public static string GetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      bool fallThru = false,
      string defaultValue = null)
    {
      if (query.Depth == 0)
      {
        RegistryItem registryItem;
        while (true)
        {
          registryItem = registryService.Read(requestContext, in query).FirstOrDefault<RegistryItem>();
          if (registryItem.Path == null)
          {
            if (fallThru)
            {
              registryService = registryService.GetParent(requestContext);
              if (registryService != null)
                requestContext = requestContext.To(TeamFoundationHostType.Parent);
              else
                goto label_6;
            }
            else
              goto label_6;
          }
          else
            break;
        }
        return registryItem.Value;
      }
label_6:
      return defaultValue;
    }

    public static string GetSingleValue(
      this IEnumerable<RegistryItem> queryResult,
      string defaultValue = null)
    {
      string str = (string) null;
      bool flag = true;
      foreach (RegistryItem registryItem in queryResult)
      {
        flag = flag ? false : throw new InvalidOperationException();
        str = registryItem.Value;
      }
      return str ?? defaultValue;
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      T defaultValue)
    {
      return registryService.GetValue<T>(requestContext, in query, false, defaultValue);
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      bool fallThru = false,
      T defaultValue = null)
    {
      return RegistryUtility.FromString<T>(registryService.GetValue(requestContext, in query, fallThru), defaultValue);
    }

    public static T GetSingleValue<T>(this IEnumerable<RegistryItem> queryResult, T defaultValue = null) => RegistryUtility.FromString<T>(queryResult.GetSingleValue(), defaultValue);

    public static RegistryEntryCollection ReadEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      RegistryQuery query,
      bool includeFolders = false)
    {
      IEnumerable<RegistryItem> source;
      if (includeFolders && query.Depth > 0)
      {
        IEnumerable<RegistryItem> registryItems = registryService.Read(requestContext, new RegistryQuery(query.Path, (string) null, int.MaxValue));
        List<RegistryItem> second = new List<RegistryItem>();
        HashSet<string> stringSet = new HashSet<string>(registryItems.Select<RegistryItem, string>((Func<RegistryItem, string>) (s => s.Path)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (RegistryItem registryItem in registryItems)
        {
          foreach (string atIncreasingDepth in RegistryServiceExtensions.PathAtIncreasingDepths(query.Path, registryItem.Path, query.Depth))
          {
            if (!stringSet.Contains(atIncreasingDepth))
            {
              second.Add(new RegistryItem(atIncreasingDepth, (string) null));
              stringSet.Add(atIncreasingDepth);
            }
          }
        }
        source = registryItems.Concat<RegistryItem>((IEnumerable<RegistryItem>) second).Where<RegistryItem>((Func<RegistryItem, bool>) (s => query.Matches(s.Path)));
      }
      else
        source = registryService.Read(requestContext, in query);
      return new RegistryEntryCollection(query.Path, source.Select<RegistryItem, RegistryEntry>((Func<RegistryItem, RegistryEntry>) (s => new RegistryEntry(s.Path, s.Value))));
    }

    public static void WriteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      IEnumerable<RegistryEntry> registryEntries)
    {
      registryService.Write(requestContext, registryEntries.Select<RegistryEntry, RegistryItem>((Func<RegistryEntry, RegistryItem>) (s => new RegistryItem(s.Path, s.Value))));
    }

    public static RegistryEntryCollection ReadEntriesFallThru(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query)
    {
      RegistryEntryCollection collection = (RegistryEntryCollection) null;
      while (true)
      {
        RegistryEntryCollection registryEntryCollection = registryService.ReadEntries(requestContext, query);
        if (collection == null)
          collection = registryEntryCollection;
        else if (registryEntryCollection != null && registryEntryCollection.Count > 0)
        {
          List<RegistryEntry> entries = new List<RegistryEntry>((IEnumerable<RegistryEntry>) collection);
          foreach (RegistryEntry registryEntry in registryEntryCollection)
          {
            if (!collection.ContainsPath(registryEntry.Path))
              entries.Add(registryEntry);
          }
          collection = new RegistryEntryCollection(query.Path, (IEnumerable<RegistryEntry>) entries);
        }
        registryService = registryService.GetParent(requestContext);
        if (registryService != null)
          requestContext = requestContext.To(TeamFoundationHostType.Parent);
        else
          break;
      }
      return collection;
    }

    public static void SetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string path,
      object value)
    {
      registryService.SetValue<object>(requestContext, path, value);
    }

    public static void SetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string path,
      T value)
    {
      string str = (object) value != null ? RegistryUtility.ToString<T>(value) : (string) null;
      registryService.Write(requestContext, (IEnumerable<RegistryItem>) new RegistryItem[1]
      {
        new RegistryItem(path, str)
      });
    }

    public static int DeleteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      string registryPathPattern)
    {
      return registryService.DeleteEntries(requestContext, new string[1]
      {
        registryPathPattern
      });
    }

    public static int DeleteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      params string[] registryPathPatterns)
    {
      return registryService.DeleteEntries(requestContext, (IEnumerable<string>) registryPathPatterns);
    }

    public static int DeleteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      IEnumerable<string> registryPathPatterns)
    {
      ArgumentUtility.CheckForNull<IEnumerable<string>>(registryPathPatterns, nameof (registryPathPatterns));
      List<string> list = registryPathPatterns.SelectMany<string, RegistryItem>((Func<string, IEnumerable<RegistryItem>>) (s => registryService.Read(requestContext, new RegistryQuery(s)))).Select<RegistryItem, string>((Func<RegistryItem, string>) (s => s.Path)).Distinct<string>().ToList<string>();
      registryService.Write(requestContext, list.Select<string, RegistryItem>((Func<string, RegistryItem>) (s => new RegistryItem(s, (string) null))));
      return list.Count;
    }

    public static int DeleteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      Predicate<RegistryItem> predicate)
    {
      List<RegistryItem> items = new List<RegistryItem>();
      foreach (RegistryItem registryItem in registryService.Read(requestContext, in query))
      {
        if (predicate(registryItem))
          items.Add(new RegistryItem(registryItem.Path, (string) null));
      }
      if (items.Count > 0)
        registryService.Write(requestContext, (IEnumerable<RegistryItem>) items);
      return items.Count;
    }

    public static void UpdateOrDeleteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      IEnumerable<RegistryEntry> registryEntries)
    {
      registryService.WriteEntries(requestContext, registryEntries);
    }

    public static void CopyEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      Func<string, string> transformRegistryKey)
    {
      registryService.CopyOrRenameEntries(requestContext, in query, false, transformRegistryKey);
    }

    public static void RenameEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      Func<string, string> transformRegistryKey)
    {
      registryService.CopyOrRenameEntries(requestContext, in query, true, transformRegistryKey);
    }

    public static IEnumerable<RegistryItem> Filter(
      this IEnumerable<RegistryItem> entries,
      RegistryQuery query)
    {
      foreach (RegistryItem entry in entries)
      {
        if (query.Matches(entry.Path))
          yield return entry;
      }
    }

    public static IEnumerable<RegistryItem> Filter(
      this IEnumerable<RegistryItem> entries,
      IEnumerable<RegistryQuery> queries)
    {
      foreach (RegistryItem entry in entries)
      {
        foreach (RegistryQuery query in queries)
        {
          if (query.Matches(entry.Path))
          {
            yield return entry;
            break;
          }
        }
      }
    }

    private static IEnumerable<string> PathAtIncreasingDepths(
      string rootPath,
      string childPath,
      int maxDepth)
    {
      int depth = 0;
      int i = rootPath.Length;
      while (true)
      {
        do
        {
          yield return childPath.Substring(0, i);
          if (i != childPath.Length && ++depth <= maxDepth)
            i = childPath.IndexOf('/', i + 1);
          else
            goto label_2;
        }
        while (i >= 0);
        i = childPath.Length;
      }
label_2:;
    }

    private static void CopyOrRenameEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      in RegistryQuery query,
      bool deleteOriginal,
      Func<string, string> transformRegistryKey)
    {
      List<RegistryItem> items = new List<RegistryItem>();
      foreach (RegistryItem registryItem in registryService.Read(requestContext, in query))
      {
        string str = transformRegistryKey(registryItem.Path);
        if (!StringComparer.OrdinalIgnoreCase.Equals(str, registryItem.Path))
        {
          items.Add(new RegistryItem(str, registryItem.Value));
          if (deleteOriginal)
            items.Add(new RegistryItem(registryItem.Path, (string) null));
        }
      }
      if (items.Count <= 0)
        return;
      registryService.Write(requestContext, (IEnumerable<RegistryItem>) items);
    }
  }
}
