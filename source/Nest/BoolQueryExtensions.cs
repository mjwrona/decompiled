// Decompiled with JetBrains decompiler
// Type: Nest.BoolQueryExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  internal static class BoolQueryExtensions
  {
    internal static IQueryContainer Self(this QueryContainer q) => (IQueryContainer) q;

    internal static bool HasOnlyShouldClauses(this IBoolQuery boolQuery) => boolQuery != null && !boolQuery.IsVerbatim && boolQuery.Should.HasAny<QueryContainer>() && !boolQuery.Must.HasAny<QueryContainer>() && !boolQuery.MustNot.HasAny<QueryContainer>() && !boolQuery.Filter.HasAny<QueryContainer>();

    internal static bool HasOnlyFilterClauses(this IBoolQuery boolQuery) => boolQuery != null && !boolQuery.IsVerbatim && !boolQuery.Locked && !boolQuery.Should.HasAny<QueryContainer>() && !boolQuery.Must.HasAny<QueryContainer>() && !boolQuery.MustNot.HasAny<QueryContainer>() && boolQuery.Filter.HasAny<QueryContainer>();

    internal static bool HasOnlyMustNotClauses(this IBoolQuery boolQuery) => boolQuery != null && !boolQuery.IsVerbatim && !boolQuery.Locked && !boolQuery.Should.HasAny<QueryContainer>() && !boolQuery.Must.HasAny<QueryContainer>() && boolQuery.MustNot.HasAny<QueryContainer>() && !boolQuery.Filter.HasAny<QueryContainer>();
  }
}
