// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PackagingUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class PackagingUtils
  {
    public static async Task CreateContainerIfNotExistsAsync<T>(
      IVssRequestContext requestContext,
      FeedCore feed)
      where T : class, IItemStore
    {
      Locator feedContainerName = PackagingUtils.ComputeFeedContainerName(feed);
      ContainerItem addContainerAsync = await requestContext.GetService<T>().GetOrAddContainerAsync(requestContext, new ContainerItem()
      {
        Name = feedContainerName
      });
    }

    public static Locator ComputeFeedContainerName(FeedCore feed) => new Locator(new string[1]
    {
      string.Format("feed/{0}", (object) feed.Id)
    });

    public static TEntry BinarySearch<TKey, TEntry>(
      TKey keyToSearchFor,
      IReadOnlyList<TEntry> entries,
      Func<TEntry, TKey> keyExtractor,
      IComparer<TKey> comparer,
      out int closestIndex)
    {
      int num1 = 0;
      int num2 = entries.Count - 1;
      int num3 = 0;
      closestIndex = 0;
      while (num1 <= num2)
      {
        closestIndex = (num2 + num1) / 2;
        num3 = comparer.Compare(keyToSearchFor, keyExtractor(entries[closestIndex]));
        if (num3 == 0)
          return entries[closestIndex];
        if (num3 > 0)
          num1 = closestIndex + 1;
        else if (num3 < 0)
          num2 = closestIndex - 1;
      }
      closestIndex = num3 < 0 ? closestIndex : Math.Min(closestIndex + 1, entries.Count);
      return default (TEntry);
    }
  }
}
