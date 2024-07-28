// Decompiled with JetBrains decompiler
// Type: Nest.QueryVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class QueryVisitor : IQueryVisitor
  {
    public int Depth { get; set; }

    public VisitorScope Scope { get; set; }

    public virtual void Visit(IQueryContainer query)
    {
    }

    public virtual void Visit(IQuery query)
    {
    }

    public virtual void Visit(IBoolQuery query)
    {
    }

    public virtual void Visit(IBoostingQuery query)
    {
    }

    public virtual void Visit(ICommonTermsQuery query)
    {
    }

    public virtual void Visit(IConstantScoreQuery query)
    {
    }

    public virtual void Visit(IDisMaxQuery query)
    {
    }

    public virtual void Visit(IDistanceFeatureQuery query)
    {
    }

    public virtual void Visit(ISpanContainingQuery query)
    {
    }

    public virtual void Visit(ISpanWithinQuery query)
    {
    }

    public virtual void Visit(IDateRangeQuery query)
    {
    }

    public virtual void Visit(INumericRangeQuery query)
    {
    }

    public virtual void Visit(ILongRangeQuery query)
    {
    }

    public virtual void Visit(ITermRangeQuery query)
    {
    }

    public virtual void Visit(IFunctionScoreQuery query)
    {
    }

    public virtual void Visit(IFuzzyQuery query)
    {
    }

    public virtual void Visit(IFuzzyStringQuery query)
    {
    }

    public virtual void Visit(IFuzzyNumericQuery query)
    {
    }

    public virtual void Visit(IFuzzyDateQuery query)
    {
    }

    public virtual void Visit(IGeoShapeQuery query)
    {
    }

    public virtual void Visit(IShapeQuery query)
    {
    }

    public virtual void Visit(IHasChildQuery query)
    {
    }

    public virtual void Visit(IHasParentQuery query)
    {
    }

    public virtual void Visit(IIdsQuery query)
    {
    }

    public virtual void Visit(IIntervalsQuery query)
    {
    }

    public virtual void Visit(IMatchQuery query)
    {
    }

    public virtual void Visit(IMatchPhraseQuery query)
    {
    }

    public virtual void Visit(IMatchPhrasePrefixQuery query)
    {
    }

    public virtual void Visit(IMatchAllQuery query)
    {
    }

    public virtual void Visit(IMatchBoolPrefixQuery query)
    {
    }

    public virtual void Visit(IMatchNoneQuery query)
    {
    }

    public virtual void Visit(IMoreLikeThisQuery query)
    {
    }

    public virtual void Visit(IMultiMatchQuery query)
    {
    }

    public virtual void Visit(INestedQuery query)
    {
    }

    public virtual void Visit(IPrefixQuery query)
    {
    }

    public virtual void Visit(IQueryStringQuery query)
    {
    }

    public virtual void Visit(IRankFeatureQuery query)
    {
    }

    public virtual void Visit(IRangeQuery query)
    {
    }

    public virtual void Visit(IRegexpQuery query)
    {
    }

    public virtual void Visit(ISimpleQueryStringQuery query)
    {
    }

    public virtual void Visit(ISpanFirstQuery query)
    {
    }

    public virtual void Visit(ISpanNearQuery query)
    {
    }

    public virtual void Visit(ISpanNotQuery query)
    {
    }

    public virtual void Visit(ISpanOrQuery query)
    {
    }

    public virtual void Visit(ISpanTermQuery query)
    {
    }

    public virtual void Visit(ISpanSubQuery query)
    {
    }

    public virtual void Visit(ISpanMultiTermQuery query)
    {
    }

    public virtual void Visit(ISpanFieldMaskingQuery query)
    {
    }

    public virtual void Visit(ITermQuery query)
    {
    }

    public virtual void Visit(IWildcardQuery query)
    {
    }

    public virtual void Visit(ITermsQuery query)
    {
    }

    public virtual void Visit(IScriptQuery query)
    {
    }

    public virtual void Visit(IScriptScoreQuery query)
    {
    }

    public virtual void Visit(IGeoPolygonQuery query)
    {
    }

    public virtual void Visit(IGeoDistanceQuery query)
    {
    }

    public virtual void Visit(ISpanQuery query)
    {
    }

    public virtual void Visit(IGeoBoundingBoxQuery query)
    {
    }

    public virtual void Visit(IExistsQuery query)
    {
    }

    public virtual void Visit(IRawQuery query)
    {
    }

    public virtual void Visit(IPercolateQuery query)
    {
    }

    public virtual void Visit(IParentIdQuery query)
    {
    }

    public virtual void Visit(ITermsSetQuery query)
    {
    }

    public virtual void Visit(IPinnedQuery query)
    {
    }

    public virtual void Visit(ICombinedFieldsQuery query)
    {
    }

    public virtual void Visit(IQueryVisitor visitor)
    {
    }
  }
}
