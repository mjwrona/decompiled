// Decompiled with JetBrains decompiler
// Type: Nest.BoolQueryOrExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  internal static class BoolQueryOrExtensions
  {
    internal static QueryContainer CombineAsShould(
      this QueryContainer leftContainer,
      QueryContainer rightContainer)
    {
      QueryContainer c = (QueryContainer) null;
      IBoolQuery leftBool = leftContainer.Self()?.Bool;
      IBoolQuery rightBool = rightContainer.Self()?.Bool;
      if (BoolQueryOrExtensions.TryFlattenShould(leftContainer, rightContainer, leftBool, rightBool, out c))
        return c;
      IBoolQuery boolQuery1 = leftContainer.Self().Bool;
      IBoolQuery boolQuery2 = rightContainer.Self().Bool;
      bool flag1 = boolQuery1 != null && boolQuery1.Should.HasAny<QueryContainer>();
      bool flag2 = boolQuery2 != null && boolQuery2.Should.HasAny<QueryContainer>();
      IEnumerable<QueryContainer> queryContainers1;
      if (!flag1)
        queryContainers1 = (IEnumerable<QueryContainer>) new QueryContainer[1]
        {
          leftContainer
        };
      else
        queryContainers1 = boolQuery1.Should;
      IEnumerable<QueryContainer> list = queryContainers1;
      IEnumerable<QueryContainer> queryContainers2;
      if (!flag2)
        queryContainers2 = (IEnumerable<QueryContainer>) new QueryContainer[1]
        {
          rightContainer
        };
      else
        queryContainers2 = boolQuery2.Should;
      IEnumerable<QueryContainer> other = queryContainers2;
      return BoolQueryOrExtensions.CreateShouldContainer(list.EagerConcat<QueryContainer>(other));
    }

    private static bool TryFlattenShould(
      QueryContainer leftContainer,
      QueryContainer rightContainer,
      IBoolQuery leftBool,
      IBoolQuery rightBool,
      out QueryContainer c)
    {
      c = (QueryContainer) null;
      bool flag1 = leftContainer.CanMergeShould();
      bool flag2 = rightContainer.CanMergeShould();
      if (!flag1 && !flag2)
        c = BoolQueryOrExtensions.CreateShouldContainer(new List<QueryContainer>()
        {
          leftContainer,
          rightContainer
        });
      else if (flag1 && !flag2 && rightBool != null)
      {
        leftBool.Should = leftBool.Should.AddIfNotNull<QueryContainer>(rightContainer);
        c = leftContainer;
      }
      else if (flag2 && !flag1 && leftBool != null)
      {
        rightBool.Should = rightBool.Should.AddIfNotNull<QueryContainer>(leftContainer);
        c = rightContainer;
      }
      return c != null;
    }

    private static bool CanMergeShould(this IQueryContainer container) => container.Bool.CanMergeShould();

    private static bool CanMergeShould(this IBoolQuery boolQuery) => boolQuery != null && boolQuery.IsWritable && !boolQuery.Locked && boolQuery.HasOnlyShouldClauses();

    private static QueryContainer CreateShouldContainer(List<QueryContainer> shouldClauses) => (QueryContainer) (QueryBase) new BoolQuery()
    {
      Should = (IEnumerable<QueryContainer>) shouldClauses.ToListOrNullIfEmpty<QueryContainer>()
    };
  }
}
