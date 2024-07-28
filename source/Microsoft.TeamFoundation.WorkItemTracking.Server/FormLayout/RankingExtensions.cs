// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.RankingExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public static class RankingExtensions
  {
    public static void InsertRankedItem<T>(
      this IList<T> deltaList,
      T itemToInsert,
      IList<T> combinedList,
      int? insertIndex = null)
      where T : IRankable
    {
      if (insertIndex.HasValue)
      {
        T obj1 = combinedList.FirstOrDefault<T>((Func<T, bool>) (item => item.IsEquivalentTo((IRankable) itemToInsert)));
        if ((object) obj1 != null)
          combinedList.Remove(obj1);
        T obj2 = deltaList.FirstOrDefault<T>((Func<T, bool>) (item => item.IsEquivalentTo((IRankable) itemToInsert)));
        if ((object) obj2 != null)
          deltaList.Remove(obj2);
        int? nullable1 = insertIndex;
        int num1 = 0;
        if (nullable1.GetValueOrDefault() < num1 & nullable1.HasValue)
          throw new ArgumentOutOfRangeException(nameof (insertIndex));
        nullable1 = insertIndex;
        int num2 = combinedList.Count - 1;
        if (nullable1.GetValueOrDefault() > num2 & nullable1.HasValue)
        {
          ref T local = ref itemToInsert;
          if ((object) default (T) == null)
          {
            T obj3 = local;
            local = ref obj3;
          }
          nullable1 = new int?();
          int? nullable2 = nullable1;
          local.Rank = nullable2;
          deltaList.InsertRankedItem<T>(itemToInsert, IfThereAreDuplicateRanks.InsertAfterItemsWithSameRank);
        }
        else
        {
          nullable1 = insertIndex;
          int num3 = 0;
          T obj4;
          if (nullable1.GetValueOrDefault() == num3 & nullable1.HasValue)
          {
            obj4 = default (T);
            int? rank = combinedList[0].Rank;
            if (rank.HasValue)
            {
              ref T local = ref itemToInsert;
              if ((object) default (T) == null)
              {
                T obj5 = local;
                local = ref obj5;
              }
              nullable1 = rank;
              int? nullable3 = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() - 1) : new int?();
              local.Rank = nullable3;
            }
            else
            {
              ref T local = ref itemToInsert;
              if ((object) default (T) == null)
              {
                T obj6 = local;
                local = ref obj6;
              }
              nullable1 = new int?();
              int? nullable4 = nullable1;
              local.Rank = nullable4;
            }
          }
          else
          {
            T previousItemInCombinedLayout = combinedList[insertIndex.Value - 1];
            obj4 = deltaList.FirstOrDefault<T>((Func<T, bool>) (i => i.IsEquivalentTo((IRankable) previousItemInCombinedLayout)));
            ref T local = ref itemToInsert;
            if ((object) default (T) == null)
            {
              T obj7 = local;
              local = ref obj7;
            }
            int? rank = previousItemInCombinedLayout.Rank;
            local.Rank = rank;
          }
          if ((object) obj4 != null)
          {
            int num4 = deltaList.IndexOf(obj4);
            deltaList.Insert(num4 + 1, itemToInsert);
          }
          else
            deltaList.InsertRankedItem<T>(itemToInsert, IfThereAreDuplicateRanks.InsertBeforeItemsWithSameRank);
        }
      }
      else
        deltaList.InsertInRankedOrderOrReplace<T>(itemToInsert);
    }

    private static void InsertInRankedOrderOrReplace<T>(this IList<T> list, T item) where T : IRankable
    {
      T obj = list.FirstOrDefault<T>((Func<T, bool>) (i => i.IsEquivalentTo((IRankable) item)));
      if ((object) obj == null)
        list.InsertRankedItem<T>(item, IfThereAreDuplicateRanks.InsertAfterItemsWithSameRank);
      else
        list[list.IndexOf(obj)] = item;
    }

    public static void InsertRankedItem<T>(
      this IList<T> mergedList,
      T insertedObject,
      IfThereAreDuplicateRanks howToInsert)
      where T : IRankable
    {
      int index = 0;
      foreach (T merged in (IEnumerable<T>) mergedList)
      {
        int? rank1 = merged.Rank;
        if (!rank1.HasValue)
        {
          rank1 = insertedObject.Rank;
          if (rank1.HasValue)
            break;
        }
        rank1 = merged.Rank;
        int? rank2 = insertedObject.Rank;
        if (!(rank1.GetValueOrDefault() > rank2.GetValueOrDefault() & rank1.HasValue & rank2.HasValue))
        {
          rank2 = merged.Rank;
          rank1 = insertedObject.Rank;
          if (rank2.GetValueOrDefault() == rank1.GetValueOrDefault() & rank2.HasValue == rank1.HasValue)
          {
            if (howToInsert == IfThereAreDuplicateRanks.InsertBeforeItemsWithSameRank)
              break;
          }
          ++index;
        }
        else
          break;
      }
      mergedList.Insert(index, insertedObject);
    }
  }
}
