// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.OrderByEnumeratorComparer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal sealed class OrderByEnumeratorComparer : 
    IComparer<OrderByQueryPartitionRangePageAsyncEnumerator>
  {
    private readonly IReadOnlyList<SortOrder> sortOrders;

    public OrderByEnumeratorComparer(IReadOnlyList<SortOrder> sortOrders)
    {
      if (sortOrders == null)
        throw new ArgumentNullException("Sort Orders array can not be null for an order by comparer.");
      this.sortOrders = sortOrders.Count != 0 ? (IReadOnlyList<SortOrder>) new List<SortOrder>((IEnumerable<SortOrder>) sortOrders) : throw new ArgumentException("Sort Orders array can not be empty for an order by comparer.");
    }

    public int Compare(
      OrderByQueryPartitionRangePageAsyncEnumerator enumerator1,
      OrderByQueryPartitionRangePageAsyncEnumerator enumerator2)
    {
      if (enumerator1 == enumerator2)
        return 0;
      if (enumerator1.Current.Failed && !enumerator2.Current.Failed)
        return -1;
      if (!enumerator1.Current.Failed && enumerator2.Current.Failed)
        return 1;
      if (enumerator1.Current.Failed && enumerator2.Current.Failed)
        return string.CompareOrdinal(((FeedRangeEpk) enumerator1.FeedRangeState.FeedRange).Range.Min, ((FeedRangeEpk) enumerator2.FeedRangeState.FeedRange).Range.Min);
      OrderByQueryResult orderByQueryResult1;
      ref OrderByQueryResult local1 = ref orderByQueryResult1;
      TryCatch<OrderByQueryPage> current1 = enumerator1.Current;
      CosmosElement current2 = current1.Result.Enumerator.Current;
      local1 = new OrderByQueryResult(current2);
      OrderByQueryResult orderByQueryResult2;
      ref OrderByQueryResult local2 = ref orderByQueryResult2;
      current1 = enumerator2.Current;
      CosmosElement current3 = current1.Result.Enumerator.Current;
      local2 = new OrderByQueryResult(current3);
      int num = this.CompareOrderByItems(orderByQueryResult1.OrderByItems, orderByQueryResult2.OrderByItems);
      return num != 0 ? num : string.CompareOrdinal(((FeedRangeEpk) enumerator1.FeedRangeState.FeedRange).Range.Min, ((FeedRangeEpk) enumerator2.FeedRangeState.FeedRange).Range.Min);
    }

    public int CompareOrderByItems(
      IReadOnlyList<OrderByItem> items1,
      IReadOnlyList<OrderByItem> items2)
    {
      if (items1 == items2)
        return 0;
      for (int index = 0; index < this.sortOrders.Count; ++index)
      {
        int num = ItemComparer.Instance.Compare(items1[index].Item, items2[index].Item);
        if (num != 0)
          return this.sortOrders[index] == SortOrder.Descending ? -num : num;
      }
      return 0;
    }
  }
}
