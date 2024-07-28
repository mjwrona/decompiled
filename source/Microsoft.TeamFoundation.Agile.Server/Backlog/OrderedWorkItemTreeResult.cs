// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Backlog.OrderedWorkItemTreeResult
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Server.Backlog
{
  internal class OrderedWorkItemTreeResult : OrderedWorkItemResult
  {
    private Dictionary<int, List<int>> m_tree;
    private Dictionary<int, int> m_linkType;
    private OrderedWorkItemTreeResult.LeafMode m_leafMode;

    internal OrderedWorkItemTreeResult(
      QueryResult queryResult,
      OrderedWorkItemTreeResult.LeafMode leafMode)
      : base(queryResult, false)
    {
      this.m_leafMode = leafMode;
      this.BuildTree();
    }

    public Dictionary<int, List<int>> Tree => this.m_tree;

    public Dictionary<int, int> LinkTypes => this.m_linkType;

    public HashSet<int> GetLeafs()
    {
      if (this.m_leafMode != OrderedWorkItemTreeResult.LeafMode.TopDown)
        return new HashSet<int>(this.m_tree.SelectMany<KeyValuePair<int, List<int>>, int>((Func<KeyValuePair<int, List<int>>, IEnumerable<int>>) (x => x.Value.Where<int>((Func<int, bool>) (childId => !this.m_tree.ContainsKey(childId))))));
      return this.m_tree.ContainsKey(0) ? new HashSet<int>((IEnumerable<int>) this.m_tree[0]) : new HashSet<int>();
    }

    public IEnumerable<int> GetItemsInBFSOrder(int maximumNumberOfItems, ISet<int> ignoreItems)
    {
      if (!this.Tree.Any<KeyValuePair<int, List<int>>>())
        return Enumerable.Empty<int>();
      List<int> itemsInBfsOrder = new List<int>();
      Queue<int> source = new Queue<int>((IEnumerable<int>) this.Tree[0]);
      while (source.Any<int>() && itemsInBfsOrder.Count < maximumNumberOfItems)
      {
        int key = source.Dequeue();
        if (ignoreItems == null || !ignoreItems.Contains(key))
          itemsInBfsOrder.Add(key);
        List<int> intList;
        if (this.Tree.TryGetValue(key, out intList))
        {
          foreach (int num in intList)
            source.Enqueue(num);
        }
      }
      return (IEnumerable<int>) itemsInBfsOrder;
    }

    public HashSet<int> GetNonLeafs() => this.m_leafMode == OrderedWorkItemTreeResult.LeafMode.TopDown ? new HashSet<int>(this.Order.Keys.Except<int>((IEnumerable<int>) this.GetLeafs())) : new HashSet<int>((IEnumerable<int>) this.m_tree.Keys);

    internal void TrimInternal(HashSet<int> trimTo) => this.TrimInternal(0, trimTo);

    private bool TrimInternal(int id, HashSet<int> trimTo)
    {
      bool flag1 = id == 0;
      bool flag2 = trimTo.Contains(id);
      bool flag3 = false;
      List<int> intList = (List<int>) null;
      if (this.m_tree.TryGetValue(id, out intList))
      {
        foreach (int num in intList.ToArray())
        {
          if (!this.TrimInternal(num, trimTo))
          {
            intList.Remove(num);
            if (this.m_linkType.ContainsKey(num))
              this.m_linkType.Remove(num);
            if (this.Order.ContainsKey(num))
              this.Order.Remove(num);
          }
          else
            flag3 = true;
        }
        if (!flag3 && !flag1)
          this.m_tree.Remove(id);
      }
      return flag2 | flag3;
    }

    public void Merge(OrderedWorkItemTreeResult treeToMerge)
    {
      foreach (int leaf in this.GetLeafs())
        this.MergeSubTree(treeToMerge, leaf);
    }

    private void MergeSubTree(OrderedWorkItemTreeResult source, int id)
    {
      List<int> intList = (List<int>) null;
      if (!source.Tree.TryGetValue(id, out intList))
        return;
      this.Tree[id] = intList;
      foreach (int num in intList)
      {
        this.m_linkType.Add(num, source.LinkTypes[num]);
        this.MergeSubTree(source, num);
      }
    }

    private void BuildTree()
    {
      int num = 0;
      Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
      Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
      Dictionary<int, List<int>> dictionary3 = new Dictionary<int, List<int>>();
      foreach (LinkQueryResultEntry workItemLink in this.Result.WorkItemLinks)
      {
        int sourceId = workItemLink.SourceId;
        int targetId = workItemLink.TargetId;
        if (dictionary2.ContainsKey(targetId))
        {
          if (dictionary2[targetId] == 0 && workItemLink.LinkTypeId != (short) 0)
          {
            List<int> intList1;
            if (dictionary3.TryGetValue(0, out intList1))
              intList1.Remove(targetId);
            List<int> intList2;
            if (!dictionary3.TryGetValue(sourceId, out intList2))
            {
              intList2 = new List<int>();
              dictionary3[sourceId] = intList2;
            }
            intList2.Add(targetId);
            dictionary2[targetId] = (int) workItemLink.LinkTypeId;
          }
        }
        else
        {
          List<int> intList;
          if (!dictionary3.TryGetValue(sourceId, out intList))
          {
            intList = new List<int>();
            dictionary3[sourceId] = intList;
          }
          intList.Add(targetId);
          dictionary1.Add(targetId, num++);
          dictionary2.Add(targetId, (int) workItemLink.LinkTypeId);
        }
      }
      this.m_tree = dictionary3;
      this.m_linkType = dictionary2;
      this.Order = dictionary1;
    }

    public static OrderedWorkItemTreeResult CreateTopDown(QueryResult queryResult)
    {
      ArgumentUtility.CheckForNull<QueryResult>(queryResult, nameof (queryResult));
      return new OrderedWorkItemTreeResult(queryResult, OrderedWorkItemTreeResult.LeafMode.TopDown);
    }

    public static OrderedWorkItemTreeResult CreateBottomUp(
      QueryResult queryResult,
      HashSet<int> validLeafs)
    {
      ArgumentUtility.CheckForNull<HashSet<int>>(validLeafs, nameof (validLeafs));
      OrderedWorkItemTreeResult bottomUp = new OrderedWorkItemTreeResult(queryResult, OrderedWorkItemTreeResult.LeafMode.BottomUp);
      bottomUp.TrimInternal(validLeafs);
      return bottomUp;
    }

    internal enum LeafMode
    {
      BottomUp,
      TopDown,
    }
  }
}
