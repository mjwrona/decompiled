// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.ParallelQuery.OrderByConsumeComparer
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents.Query.ParallelQuery
{
  internal sealed class OrderByConsumeComparer : IComparer<DocumentProducerTree<OrderByQueryResult>>
  {
    [ThreadStatic]
    public static bool AllowMixedTypeOrderByTestFlag;
    private readonly IReadOnlyList<SortOrder> sortOrders;

    public OrderByConsumeComparer(SortOrder[] sortOrders)
    {
      if (sortOrders == null)
        throw new ArgumentNullException("Sort Orders array can not be null for an order by comparer.");
      this.sortOrders = sortOrders.Length != 0 ? (IReadOnlyList<SortOrder>) new List<SortOrder>((IEnumerable<SortOrder>) sortOrders) : throw new ArgumentException("Sort Orders array can not be empty for an order by comparerer.");
    }

    public int Compare(
      DocumentProducerTree<OrderByQueryResult> producer1,
      DocumentProducerTree<OrderByQueryResult> producer2)
    {
      if (producer1 == producer2)
        return 0;
      if (producer1.HasMoreResults && !producer2.HasMoreResults)
        return -1;
      if (!producer1.HasMoreResults && producer2.HasMoreResults)
        return 1;
      if (!producer1.HasMoreResults && !producer2.HasMoreResults)
        return string.CompareOrdinal(producer1.PartitionKeyRange.MinInclusive, producer2.PartitionKeyRange.MinInclusive);
      OrderByQueryResult current1 = producer1.Current;
      OrderByQueryResult current2 = producer2.Current;
      if (current1 == null)
        throw new InvalidOperationException("DEBUG: result1 == null");
      if (current2 == null)
        throw new InvalidOperationException("DEBUG: result2 == null");
      int num = this.CompareOrderByItems(current1.OrderByItems, current2.OrderByItems);
      return num != 0 ? num : string.CompareOrdinal(producer1.PartitionKeyRange.MinInclusive, producer2.PartitionKeyRange.MinInclusive);
    }

    public int CompareOrderByItems(QueryItem[] items1, QueryItem[] items2)
    {
      if (items1 == items2)
        return 0;
      if (!OrderByConsumeComparer.AllowMixedTypeOrderByTestFlag)
        this.CheckTypeMatching(items1, items2);
      for (int index = 0; index < this.sortOrders.Count; ++index)
      {
        int num = ItemComparer.Instance.Compare(items1[index].GetItem(), items2[index].GetItem());
        if (num != 0)
          return this.sortOrders[index] == SortOrder.Descending ? -num : num;
      }
      return 0;
    }

    private void CheckTypeMatching(QueryItem[] items1, QueryItem[] items2)
    {
      for (int index = 0; index < items1.Length; ++index)
      {
        ItemType itemType1 = ItemTypeHelper.GetItemType(items1[index].GetItem());
        ItemType itemType2 = ItemTypeHelper.GetItemType(items1[index].GetItem());
        if (itemType1 != itemType2)
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnsupportedCrossPartitionOrderByQueryOnMixedTypes, (object) itemType1, (object) ItemTypeHelper.GetItemType(items1[index].GetItem()), items1[index].GetItem()));
      }
    }
  }
}
