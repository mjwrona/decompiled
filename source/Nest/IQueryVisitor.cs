// Decompiled with JetBrains decompiler
// Type: Nest.IQueryVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public interface IQueryVisitor
  {
    int Depth { get; set; }

    VisitorScope Scope { get; set; }

    void Visit(IQueryContainer queryDescriptor);

    void Visit(IQuery query);

    void Visit(IBoolQuery query);

    void Visit(IBoostingQuery query);

    void Visit(ICommonTermsQuery query);

    void Visit(IConstantScoreQuery query);

    void Visit(IDisMaxQuery query);

    void Visit(IDistanceFeatureQuery query);

    void Visit(IFunctionScoreQuery query);

    void Visit(IFuzzyQuery query);

    void Visit(IFuzzyNumericQuery query);

    void Visit(IFuzzyDateQuery query);

    void Visit(IFuzzyStringQuery query);

    void Visit(IHasChildQuery query);

    void Visit(IHasParentQuery query);

    void Visit(IIdsQuery query);

    void Visit(IIntervalsQuery query);

    void Visit(IMatchQuery query);

    void Visit(IMatchPhraseQuery query);

    void Visit(IMatchPhrasePrefixQuery query);

    void Visit(IMatchAllQuery query);

    void Visit(IMatchBoolPrefixQuery query);

    void Visit(IMatchNoneQuery query);

    void Visit(IMoreLikeThisQuery query);

    void Visit(IMultiMatchQuery query);

    void Visit(INestedQuery query);

    void Visit(IPrefixQuery query);

    void Visit(IQueryStringQuery query);

    void Visit(IRankFeatureQuery query);

    void Visit(IRangeQuery query);

    void Visit(IRegexpQuery query);

    void Visit(ISimpleQueryStringQuery query);

    void Visit(ITermQuery query);

    void Visit(IWildcardQuery query);

    void Visit(ITermsQuery query);

    void Visit(IScriptQuery query);

    void Visit(IScriptScoreQuery query);

    void Visit(IGeoPolygonQuery query);

    void Visit(IGeoDistanceQuery query);

    void Visit(IGeoBoundingBoxQuery query);

    void Visit(IExistsQuery query);

    void Visit(IDateRangeQuery query);

    void Visit(INumericRangeQuery query);

    void Visit(ILongRangeQuery query);

    void Visit(ITermRangeQuery query);

    void Visit(ISpanFirstQuery query);

    void Visit(ISpanNearQuery query);

    void Visit(ISpanNotQuery query);

    void Visit(ISpanOrQuery query);

    void Visit(ISpanTermQuery query);

    void Visit(ISpanQuery query);

    void Visit(ISpanSubQuery query);

    void Visit(ISpanContainingQuery query);

    void Visit(ISpanWithinQuery query);

    void Visit(ISpanMultiTermQuery query);

    void Visit(ISpanFieldMaskingQuery query);

    void Visit(IGeoShapeQuery query);

    void Visit(IShapeQuery query);

    void Visit(IRawQuery query);

    void Visit(IPercolateQuery query);

    void Visit(IParentIdQuery query);

    void Visit(ITermsSetQuery query);

    void Visit(IPinnedQuery query);

    void Visit(ICombinedFieldsQuery query);
  }
}
