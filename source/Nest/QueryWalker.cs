// Decompiled with JetBrains decompiler
// Type: Nest.QueryWalker
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class QueryWalker
  {
    public void Walk(IQueryContainer qd, IQueryVisitor visitor)
    {
      visitor.Visit(qd);
      QueryWalker.VisitQuery<IMatchAllQuery>(qd.MatchAll, visitor, (Action<IQueryVisitor, IMatchAllQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IMatchNoneQuery>(qd.MatchNone, visitor, (Action<IQueryVisitor, IMatchNoneQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IMoreLikeThisQuery>(qd.MoreLikeThis, visitor, (Action<IQueryVisitor, IMoreLikeThisQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IMultiMatchQuery>(qd.MultiMatch, visitor, (Action<IQueryVisitor, IMultiMatchQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<ICommonTermsQuery>(qd.CommonTerms, visitor, (Action<IQueryVisitor, ICommonTermsQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IFuzzyQuery>(qd.Fuzzy, visitor, (Action<IQueryVisitor, IFuzzyQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.VisitQuery<IFuzzyStringQuery>(d as IFuzzyStringQuery, visitor, (Action<IQueryVisitor, IFuzzyStringQuery>) ((vv, dd) => v.Visit(dd)));
        QueryWalker.VisitQuery<IFuzzyNumericQuery>(d as IFuzzyNumericQuery, visitor, (Action<IQueryVisitor, IFuzzyNumericQuery>) ((vv, dd) => v.Visit(dd)));
        QueryWalker.VisitQuery<IFuzzyDateQuery>(d as IFuzzyDateQuery, visitor, (Action<IQueryVisitor, IFuzzyDateQuery>) ((vv, dd) => v.Visit(dd)));
      }));
      QueryWalker.VisitQuery<IRangeQuery>(qd.Range, visitor, (Action<IQueryVisitor, IRangeQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.VisitQuery<IDateRangeQuery>(d as IDateRangeQuery, visitor, (Action<IQueryVisitor, IDateRangeQuery>) ((vv, dd) => v.Visit(dd)));
        QueryWalker.VisitQuery<INumericRangeQuery>(d as INumericRangeQuery, visitor, (Action<IQueryVisitor, INumericRangeQuery>) ((vv, dd) => v.Visit(dd)));
        QueryWalker.VisitQuery<ILongRangeQuery>(d as ILongRangeQuery, visitor, (Action<IQueryVisitor, ILongRangeQuery>) ((vv, dd) => v.Visit(dd)));
        QueryWalker.VisitQuery<ITermRangeQuery>(d as ITermRangeQuery, visitor, (Action<IQueryVisitor, ITermRangeQuery>) ((vv, dd) => v.Visit(dd)));
      }));
      QueryWalker.VisitQuery<IGeoShapeQuery>(qd.GeoShape, visitor, (Action<IQueryVisitor, IGeoShapeQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IShapeQuery>(qd.Shape, visitor, (Action<IQueryVisitor, IShapeQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IIdsQuery>(qd.Ids, visitor, (Action<IQueryVisitor, IIdsQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IIntervalsQuery>(qd.Intervals, visitor, (Action<IQueryVisitor, IIntervalsQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IPrefixQuery>(qd.Prefix, visitor, (Action<IQueryVisitor, IPrefixQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IQueryStringQuery>(qd.QueryString, visitor, (Action<IQueryVisitor, IQueryStringQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IRangeQuery>(qd.Range, visitor, (Action<IQueryVisitor, IRangeQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IRankFeatureQuery>(qd.RankFeature, visitor, (Action<IQueryVisitor, IRankFeatureQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IRegexpQuery>(qd.Regexp, visitor, (Action<IQueryVisitor, IRegexpQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<ISimpleQueryStringQuery>(qd.SimpleQueryString, visitor, (Action<IQueryVisitor, ISimpleQueryStringQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<ITermQuery>(qd.Term, visitor, (Action<IQueryVisitor, ITermQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<ITermsQuery>(qd.Terms, visitor, (Action<IQueryVisitor, ITermsQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IWildcardQuery>(qd.Wildcard, visitor, (Action<IQueryVisitor, IWildcardQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IMatchQuery>(qd.Match, visitor, (Action<IQueryVisitor, IMatchQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IMatchPhraseQuery>(qd.MatchPhrase, visitor, (Action<IQueryVisitor, IMatchPhraseQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IMatchBoolPrefixQuery>(qd.MatchBoolPrefix, visitor, (Action<IQueryVisitor, IMatchBoolPrefixQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IMatchPhrasePrefixQuery>(qd.MatchPhrasePrefix, visitor, (Action<IQueryVisitor, IMatchPhrasePrefixQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IScriptQuery>(qd.Script, visitor, (Action<IQueryVisitor, IScriptQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IScriptScoreQuery>(qd.ScriptScore, visitor, (Action<IQueryVisitor, IScriptScoreQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IExistsQuery>(qd.Exists, visitor, (Action<IQueryVisitor, IExistsQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IGeoPolygonQuery>(qd.GeoPolygon, visitor, (Action<IQueryVisitor, IGeoPolygonQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IGeoDistanceQuery>(qd.GeoDistance, visitor, (Action<IQueryVisitor, IGeoDistanceQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IGeoBoundingBoxQuery>(qd.GeoBoundingBox, visitor, (Action<IQueryVisitor, IGeoBoundingBoxQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IRawQuery>(qd.RawQuery, visitor, (Action<IQueryVisitor, IRawQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IPercolateQuery>(qd.Percolate, visitor, (Action<IQueryVisitor, IPercolateQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IParentIdQuery>(qd.ParentId, visitor, (Action<IQueryVisitor, IParentIdQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<ITermsSetQuery>(qd.TermsSet, visitor, (Action<IQueryVisitor, ITermsSetQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IPinnedQuery>(qd.Pinned, visitor, (Action<IQueryVisitor, IPinnedQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<ICombinedFieldsQuery>(qd.CombinedFields, visitor, (Action<IQueryVisitor, ICombinedFieldsQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IBoolQuery>(qd.Bool, visitor, (Action<IQueryVisitor, IBoolQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IEnumerable<IQueryContainer>) d.Filter, VisitorScope.Filter);
        QueryWalker.Accept(v, (IEnumerable<IQueryContainer>) d.Must, VisitorScope.Must);
        QueryWalker.Accept(v, (IEnumerable<IQueryContainer>) d.MustNot, VisitorScope.MustNot);
        QueryWalker.Accept(v, (IEnumerable<IQueryContainer>) d.Should, VisitorScope.Should);
      }));
      QueryWalker.VisitSpan<IQueryContainer>(qd, visitor);
      QueryWalker.VisitQuery<IBoostingQuery>(qd.Boosting, visitor, (Action<IQueryVisitor, IBoostingQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IQueryContainer) d.PositiveQuery, VisitorScope.PositiveQuery);
        QueryWalker.Accept(v, (IQueryContainer) d.NegativeQuery, VisitorScope.NegativeQuery);
      }));
      QueryWalker.VisitQuery<IConstantScoreQuery>(qd.ConstantScore, visitor, (Action<IQueryVisitor, IConstantScoreQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IQueryContainer) d.Filter);
      }));
      QueryWalker.VisitQuery<IDisMaxQuery>(qd.DisMax, visitor, (Action<IQueryVisitor, IDisMaxQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IEnumerable<IQueryContainer>) d.Queries);
      }));
      QueryWalker.VisitQuery<IDistanceFeatureQuery>(qd.DistanceFeature, visitor, (Action<IQueryVisitor, IDistanceFeatureQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitQuery<IFunctionScoreQuery>(qd.FunctionScore, visitor, (Action<IQueryVisitor, IFunctionScoreQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IQueryContainer) d.Query);
      }));
      QueryWalker.VisitQuery<IHasChildQuery>(qd.HasChild, visitor, (Action<IQueryVisitor, IHasChildQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IQueryContainer) d.Query);
      }));
      QueryWalker.VisitQuery<IHasParentQuery>(qd.HasParent, visitor, (Action<IQueryVisitor, IHasParentQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IQueryContainer) d.Query);
      }));
      QueryWalker.VisitQuery<INestedQuery>(qd.Nested, visitor, (Action<IQueryVisitor, INestedQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(v, (IQueryContainer) d.Query);
      }));
    }

    public void Walk(ISpanQuery qd, IQueryVisitor visitor)
    {
      QueryWalker.VisitSpanSubQuery<ISpanFirstQuery>(qd.SpanFirst, visitor, (Action<IQueryVisitor, ISpanFirstQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Match);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanNearQuery>(qd.SpanNear, visitor, (Action<IQueryVisitor, ISpanNearQuery>) ((v, d) =>
      {
        v.Visit(d);
        foreach (ISpanQuery query in d.Clauses ?? Enumerable.Empty<ISpanQuery>())
          QueryWalker.Accept(visitor, query);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanNotQuery>(qd.SpanNot, visitor, (Action<IQueryVisitor, ISpanNotQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Include);
        QueryWalker.Accept(visitor, d.Exclude);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanOrQuery>(qd.SpanOr, visitor, (Action<IQueryVisitor, ISpanOrQuery>) ((v, d) =>
      {
        v.Visit(d);
        foreach (ISpanQuery query in d.Clauses ?? Enumerable.Empty<ISpanQuery>())
          QueryWalker.Accept(visitor, query);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanTermQuery>(qd.SpanTerm, visitor, (Action<IQueryVisitor, ISpanTermQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitSpanSubQuery<ISpanMultiTermQuery>(qd.SpanMultiTerm, visitor, (Action<IQueryVisitor, ISpanMultiTermQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, (IQueryContainer) d.Match);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanContainingQuery>(qd.SpanContaining, visitor, (Action<IQueryVisitor, ISpanContainingQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Big);
        QueryWalker.Accept(visitor, d.Little);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanWithinQuery>(qd.SpanWithin, visitor, (Action<IQueryVisitor, ISpanWithinQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Big);
        QueryWalker.Accept(visitor, d.Little);
      }));
    }

    private static void Accept(
      IQueryVisitor visitor,
      IEnumerable<IQueryContainer> queries,
      VisitorScope scope = VisitorScope.Query)
    {
      if (queries == null)
        return;
      foreach (IQueryContainer query in queries)
        QueryWalker.Accept(visitor, query, scope);
    }

    private static void Accept(IQueryVisitor visitor, IQueryContainer query, VisitorScope scope = VisitorScope.Query)
    {
      if (query == null)
        return;
      visitor.Scope = scope;
      query.Accept(visitor);
    }

    private static void Accept(IQueryVisitor visitor, ISpanQuery query, VisitorScope scope = VisitorScope.Span)
    {
      if (query == null)
        return;
      visitor.Scope = scope;
      query.Accept(visitor);
    }

    private static void VisitSpan<T>(T qd, IQueryVisitor visitor) where T : class, IQueryContainer
    {
      QueryWalker.VisitSpanSubQuery<ISpanFirstQuery>(qd.SpanFirst, visitor, (Action<IQueryVisitor, ISpanFirstQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Match);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanNearQuery>(qd.SpanNear, visitor, (Action<IQueryVisitor, ISpanNearQuery>) ((v, d) =>
      {
        v.Visit(d);
        foreach (ISpanQuery query in d.Clauses ?? Enumerable.Empty<ISpanQuery>())
          QueryWalker.Accept(visitor, query);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanNotQuery>(qd.SpanNot, visitor, (Action<IQueryVisitor, ISpanNotQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Include);
        QueryWalker.Accept(visitor, d.Exclude);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanOrQuery>(qd.SpanOr, visitor, (Action<IQueryVisitor, ISpanOrQuery>) ((v, d) =>
      {
        v.Visit(d);
        foreach (ISpanQuery query in d.Clauses ?? Enumerable.Empty<ISpanQuery>())
          QueryWalker.Accept(visitor, query);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanTermQuery>(qd.SpanTerm, visitor, (Action<IQueryVisitor, ISpanTermQuery>) ((v, d) => v.Visit(d)));
      QueryWalker.VisitSpanSubQuery<ISpanMultiTermQuery>(qd.SpanMultiTerm, visitor, (Action<IQueryVisitor, ISpanMultiTermQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, (IQueryContainer) d.Match);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanContainingQuery>(qd.SpanContaining, visitor, (Action<IQueryVisitor, ISpanContainingQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Big);
        QueryWalker.Accept(visitor, d.Little);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanWithinQuery>(qd.SpanWithin, visitor, (Action<IQueryVisitor, ISpanWithinQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Big);
        QueryWalker.Accept(visitor, d.Little);
      }));
      QueryWalker.VisitSpanSubQuery<ISpanFieldMaskingQuery>(qd.SpanFieldMasking, visitor, (Action<IQueryVisitor, ISpanFieldMaskingQuery>) ((v, d) =>
      {
        v.Visit(d);
        QueryWalker.Accept(visitor, d.Query);
      }));
    }

    private static void VisitQuery<T>(T qd, IQueryVisitor visitor, Action<IQueryVisitor, T> scoped) where T : class, IQuery
    {
      if ((object) qd == null)
        return;
      ++visitor.Depth;
      visitor.Visit((IQuery) qd);
      scoped(visitor, qd);
      --visitor.Depth;
    }

    private static void VisitSpanSubQuery<T>(
      T qd,
      IQueryVisitor visitor,
      Action<IQueryVisitor, T> scoped)
      where T : class, ISpanSubQuery
    {
      if ((object) qd == null)
        return;
      QueryWalker.VisitQuery<T>(qd, visitor, (Action<IQueryVisitor, T>) ((v, d) =>
      {
        visitor.Visit((ISpanSubQuery) qd);
        scoped(v, d);
      }));
    }
  }
}
