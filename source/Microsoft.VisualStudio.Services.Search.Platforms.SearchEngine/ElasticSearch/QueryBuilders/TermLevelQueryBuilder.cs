// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders.TermLevelQueryBuilder
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders
{
  internal static class TermLevelQueryBuilder
  {
    public static QueryContainer ToTermLevelQuery(this IExpression expression) => expression != null ? TermLevelQueryBuilder.Build(new QueryContainerDescriptor<object>(), expression) : throw new ArgumentNullException(nameof (expression));

    private static QueryContainer Build(
      QueryContainerDescriptor<object> queryDescriptor,
      IExpression expression)
    {
      TermExpression termExpression = expression as TermExpression;
      if (termExpression != null)
      {
        if (termExpression.Type == "*")
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} does not support creating queries for generic type.", (object) nameof (TermLevelQueryBuilder))));
        if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals)
          return queryDescriptor.Term((Field) termExpression.Type, (object) termExpression.Value);
        if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.NotEquals)
          return queryDescriptor.Bool((Func<BoolQueryDescriptor<object>, IBoolQuery>) (b => (IBoolQuery) b.MustNot((Func<QueryContainerDescriptor<object>, QueryContainer>) (qd => qd.Term((Field) termExpression.Type, (object) termExpression.Value)))));
        if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.LessThan)
          return queryDescriptor.Range((Func<NumericRangeQueryDescriptor<object>, INumericRangeQuery>) (r => (INumericRangeQuery) r.Field((Field) termExpression.Type).LessThan(new double?((double) long.Parse(termExpression.Value, (IFormatProvider) CultureInfo.InvariantCulture)))));
        if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.LessThanOrEqual)
          return queryDescriptor.Range((Func<NumericRangeQueryDescriptor<object>, INumericRangeQuery>) (r => (INumericRangeQuery) r.Field((Field) termExpression.Type).LessThanOrEquals(new double?((double) long.Parse(termExpression.Value, (IFormatProvider) CultureInfo.InvariantCulture)))));
        if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.GreaterThan)
          return queryDescriptor.Range((Func<NumericRangeQueryDescriptor<object>, INumericRangeQuery>) (r => (INumericRangeQuery) r.Field((Field) termExpression.Type).GreaterThan(new double?((double) long.Parse(termExpression.Value, (IFormatProvider) CultureInfo.InvariantCulture)))));
        if (termExpression.Operator == Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.GreaterThanOrEqual)
          return queryDescriptor.Range((Func<NumericRangeQueryDescriptor<object>, INumericRangeQuery>) (r => (INumericRangeQuery) r.Field((Field) termExpression.Type).GreaterThanOrEquals(new double?((double) long.Parse(termExpression.Value, (IFormatProvider) CultureInfo.InvariantCulture)))));
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} does not support operator [{1}] for generating term query.", (object) nameof (TermLevelQueryBuilder), (object) termExpression.Operator)));
      }
      TermsExpression termsExpression = expression as TermsExpression;
      if (termsExpression != null)
      {
        if (termsExpression.Type == "*")
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} does not support creating queries for generic type.", (object) nameof (TermLevelQueryBuilder))));
        return queryDescriptor.Terms((Func<TermsQueryDescriptor<object>, ITermsQuery>) (q =>
        {
          return (ITermsQuery) new TermsQuery()
          {
            Field = (Field) termsExpression.Type,
            Terms = (IEnumerable<object>) termsExpression.Terms
          };
        }));
      }
      MissingFieldExpression missingExpression = expression as MissingFieldExpression;
      if (missingExpression != null)
      {
        if (string.IsNullOrWhiteSpace(missingExpression.FieldName))
          throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Missing field expression [{0}] contains invalid field name.", (object) missingExpression)));
        if (missingExpression.FieldName == "*")
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} does not support creating queries for generic type.", (object) nameof (TermLevelQueryBuilder))));
        return queryDescriptor.Bool((Func<BoolQueryDescriptor<object>, IBoolQuery>) (b => (IBoolQuery) b.MustNot((QueryContainer) (QueryBase) new ExistsQuery()
        {
          Field = (Field) missingExpression.FieldName
        })));
      }
      AndExpression andExpression = expression as AndExpression;
      if (andExpression != null)
        return queryDescriptor.Bool((Func<BoolQueryDescriptor<object>, IBoolQuery>) (b => (IBoolQuery) b.Must(((IEnumerable<IExpression>) andExpression.Children).Select<IExpression, Func<QueryContainerDescriptor<object>, QueryContainer>>((Func<IExpression, Func<QueryContainerDescriptor<object>, QueryContainer>>) (exp => (Func<QueryContainerDescriptor<object>, QueryContainer>) (qd => TermLevelQueryBuilder.Build(qd, exp)))))));
      OrExpression orExpression = expression as OrExpression;
      if (orExpression != null)
        return queryDescriptor.Bool((Func<BoolQueryDescriptor<object>, IBoolQuery>) (b => (IBoolQuery) b.Should(((IEnumerable<IExpression>) orExpression.Children).Select<IExpression, Func<QueryContainerDescriptor<object>, QueryContainer>>((Func<IExpression, Func<QueryContainerDescriptor<object>, QueryContainer>>) (exp => (Func<QueryContainerDescriptor<object>, QueryContainer>) (qd => TermLevelQueryBuilder.Build(qd, exp)))))));
      NotExpression notExpression = expression as NotExpression;
      if (notExpression != null)
        return queryDescriptor.Bool((Func<BoolQueryDescriptor<object>, IBoolQuery>) (b => (IBoolQuery) b.MustNot(((IEnumerable<IExpression>) notExpression.Children).Select<IExpression, Func<QueryContainerDescriptor<object>, QueryContainer>>((Func<IExpression, Func<QueryContainerDescriptor<object>, QueryContainer>>) (exp => (Func<QueryContainerDescriptor<object>, QueryContainer>) (qd => TermLevelQueryBuilder.Build(qd, exp)))))));
      if (expression is EmptyExpression)
        return (QueryContainer) queryDescriptor;
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Expression of type [{0}] is not supported by {1}.", (object) expression.GetType(), (object) nameof (TermLevelQueryBuilder))));
    }
  }
}
