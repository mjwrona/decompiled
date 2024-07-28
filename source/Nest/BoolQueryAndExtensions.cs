// Decompiled with JetBrains decompiler
// Type: Nest.BoolQueryAndExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  internal static class BoolQueryAndExtensions
  {
    internal static QueryContainer CombineAsMust(
      this QueryContainer leftContainer,
      QueryContainer rightContainer)
    {
      IBoolQuery leftBool = leftContainer.Self()?.Bool;
      IBoolQuery rightBool = rightContainer.Self()?.Bool;
      if (leftBool == null && rightBool == null)
        return BoolQueryAndExtensions.CreateMustContainer(new List<QueryContainer>()
        {
          leftContainer,
          rightContainer
        });
      QueryContainer c;
      if (BoolQueryAndExtensions.TryHandleBoolsWithOnlyShouldClauses(leftContainer, rightContainer, leftBool, rightBool, out c) || BoolQueryAndExtensions.TryHandleUnmergableBools(leftContainer, rightContainer, leftBool, rightBool, out c))
        return c;
      List<QueryContainer> mustNotClauses = BoolQueryAndExtensions.OrphanMustNots((IQueryContainer) leftContainer).EagerConcat<QueryContainer>(BoolQueryAndExtensions.OrphanMustNots((IQueryContainer) rightContainer));
      List<QueryContainer> filters = BoolQueryAndExtensions.OrphanFilters((IQueryContainer) leftContainer).EagerConcat<QueryContainer>(BoolQueryAndExtensions.OrphanFilters((IQueryContainer) rightContainer));
      return BoolQueryAndExtensions.CreateMustContainer(BoolQueryAndExtensions.OrphanMusts(leftContainer).EagerConcat<QueryContainer>(BoolQueryAndExtensions.OrphanMusts(rightContainer)), mustNotClauses, filters);
    }

    private static bool TryHandleUnmergableBools(
      QueryContainer leftContainer,
      QueryContainer rightContainer,
      IBoolQuery leftBool,
      IBoolQuery rightBool,
      out QueryContainer c)
    {
      c = (QueryContainer) null;
      bool flag1 = leftBool != null && !leftBool.CanMergeAnd();
      bool flag2 = rightBool != null && !rightBool.CanMergeAnd();
      if (!flag1 && !flag2)
        return false;
      if (flag1 & flag2)
        c = BoolQueryAndExtensions.CreateMustContainer(leftContainer, rightContainer);
      else if (((flag1 ? 0 : (leftBool != null ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
      {
        leftBool.Must = leftBool.Must.AddIfNotNull<QueryContainer>(rightContainer);
        c = leftContainer;
      }
      else if (((flag1 ? 0 : (leftBool == null ? 1 : 0)) & (flag2 ? 1 : 0)) != 0)
        c = BoolQueryAndExtensions.CreateMustContainer(leftContainer, rightContainer);
      else if (flag1 && !flag2 && rightBool != null)
      {
        rightBool.Must = rightBool.Must.AddIfNotNull<QueryContainer>(leftContainer);
        c = rightContainer;
      }
      else if (flag1 && !flag2 && rightBool == null)
        c = BoolQueryAndExtensions.CreateMustContainer(new List<QueryContainer>()
        {
          leftContainer,
          rightContainer
        });
      return c != null;
    }

    private static bool TryHandleBoolsWithOnlyShouldClauses(
      QueryContainer leftContainer,
      QueryContainer rightContainer,
      IBoolQuery leftBool,
      IBoolQuery rightBool,
      out QueryContainer c)
    {
      c = (QueryContainer) null;
      bool flag1 = leftBool.HasOnlyShouldClauses();
      bool flag2 = rightBool.HasOnlyShouldClauses();
      if (!flag1 && !flag2)
        return false;
      if (leftContainer.HoldsOnlyShouldMusts & flag2)
      {
        leftBool.Must = leftBool.Must.AddIfNotNull<QueryContainer>(rightContainer);
        c = leftContainer;
      }
      else if (rightContainer.HoldsOnlyShouldMusts & flag1)
      {
        rightBool.Must = rightBool.Must.AddIfNotNull<QueryContainer>(leftContainer);
        c = rightContainer;
      }
      else
      {
        c = BoolQueryAndExtensions.CreateMustContainer(new List<QueryContainer>()
        {
          leftContainer,
          rightContainer
        });
        c.HoldsOnlyShouldMusts = flag2 & flag1;
      }
      return true;
    }

    private static QueryContainer CreateMustContainer(QueryContainer left, QueryContainer right) => BoolQueryAndExtensions.CreateMustContainer(new List<QueryContainer>()
    {
      left,
      right
    });

    private static QueryContainer CreateMustContainer(List<QueryContainer> mustClauses) => new QueryContainer((QueryBase) new BoolQuery()
    {
      Must = (IEnumerable<QueryContainer>) mustClauses.ToListOrNullIfEmpty<QueryContainer>()
    });

    private static QueryContainer CreateMustContainer(
      List<QueryContainer> mustClauses,
      List<QueryContainer> mustNotClauses,
      List<QueryContainer> filters)
    {
      return new QueryContainer((QueryBase) new BoolQuery()
      {
        Must = (IEnumerable<QueryContainer>) mustClauses.ToListOrNullIfEmpty<QueryContainer>(),
        MustNot = (IEnumerable<QueryContainer>) mustNotClauses.ToListOrNullIfEmpty<QueryContainer>(),
        Filter = (IEnumerable<QueryContainer>) filters.ToListOrNullIfEmpty<QueryContainer>()
      });
    }

    private static bool CanMergeAnd(this IBoolQuery boolQuery) => boolQuery != null && !boolQuery.Locked && !boolQuery.Should.HasAny<QueryContainer>();

    private static IEnumerable<QueryContainer> OrphanMusts(QueryContainer container)
    {
      IBoolQuery boolQuery = container.Self().Bool;
      if (boolQuery == null)
        return (IEnumerable<QueryContainer>) new QueryContainer[1]
        {
          container
        };
      IEnumerable<QueryContainer> must = boolQuery.Must;
      return must == null ? (IEnumerable<QueryContainer>) null : (IEnumerable<QueryContainer>) must.AsInstanceOrToListOrNull<QueryContainer>();
    }

    private static IEnumerable<QueryContainer> OrphanMustNots(IQueryContainer container)
    {
      IBoolQuery boolQuery = container.Bool;
      if (boolQuery == null)
        return (IEnumerable<QueryContainer>) null;
      IEnumerable<QueryContainer> mustNot = boolQuery.MustNot;
      return mustNot == null ? (IEnumerable<QueryContainer>) null : (IEnumerable<QueryContainer>) mustNot.AsInstanceOrToListOrNull<QueryContainer>();
    }

    private static IEnumerable<QueryContainer> OrphanFilters(IQueryContainer container)
    {
      IBoolQuery boolQuery = container.Bool;
      if (boolQuery == null)
        return (IEnumerable<QueryContainer>) null;
      IEnumerable<QueryContainer> filter = boolQuery.Filter;
      return filter == null ? (IEnumerable<QueryContainer>) null : (IEnumerable<QueryContainer>) filter.AsInstanceOrToListOrNull<QueryContainer>();
    }
  }
}
