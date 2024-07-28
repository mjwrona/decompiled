// Decompiled with JetBrains decompiler
// Type: Nest.DslPrettyPrintVisitor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nest
{
  public class DslPrettyPrintVisitor : IQueryVisitor
  {
    private readonly Inferrer _infer;
    private readonly StringBuilder _sb;
    private string _final;

    public DslPrettyPrintVisitor(IConnectionSettingsValues settings)
    {
      this._sb = new StringBuilder();
      this._infer = settings.Inferrer;
    }

    public virtual int Depth { get; set; }

    public bool IsConditionless { get; set; }

    public bool IsStrict { get; set; }

    public bool IsVerbatim { get; set; }

    public string PrettyPrint
    {
      get
      {
        if (this._final.IsNullOrEmpty())
          this._final = this._sb.ToString();
        return this._final;
      }
    }

    public virtual VisitorScope Scope { get; set; }

    public virtual void Visit(IQueryContainer baseQuery)
    {
      this.IsConditionless = baseQuery.IsConditionless;
      this.IsStrict = baseQuery.IsStrict;
      this.IsVerbatim = baseQuery.IsVerbatim;
    }

    public virtual void Visit(IQuery query)
    {
    }

    public virtual void Visit(IBoolQuery query) => this.Write("bool");

    public virtual void Visit(IBoostingQuery query) => this.Write("boosting");

    public virtual void Visit(ICommonTermsQuery query) => this.Write("common_terms", query.Field);

    public virtual void Visit(IConstantScoreQuery query) => this.Write("constant_score");

    public virtual void Visit(IDisMaxQuery query) => this.Write("dis_max");

    public virtual void Visit(IDistanceFeatureQuery query) => this.Write("distance_feature");

    public virtual void Visit(ISpanContainingQuery query) => this.Write("span_containing");

    public virtual void Visit(ISpanWithinQuery query) => this.Write("span_within");

    public virtual void Visit(IDateRangeQuery query) => this.Write("date_range");

    public virtual void Visit(INumericRangeQuery query) => this.Write("numeric_range");

    public virtual void Visit(ILongRangeQuery query) => this.Write("long_range");

    public virtual void Visit(ITermRangeQuery query) => this.Write("term_range");

    public virtual void Visit(IFunctionScoreQuery query) => this.Write("function_core");

    public virtual void Visit(IFuzzyQuery query) => this.Write("fuzzy", query.Field);

    public virtual void Visit(IFuzzyNumericQuery query) => this.Write("fuzzy_numeric", query.Field);

    public virtual void Visit(IFuzzyDateQuery query) => this.Write("fuzzy_date", query.Field);

    public virtual void Visit(IFuzzyStringQuery query) => this.Write("fuzzy_string", query.Field);

    public virtual void Visit(IGeoShapeQuery query) => this.WriteShape(query.Shape, query.IndexedShape, query.Field, "geo_shape");

    public virtual void Visit(IShapeQuery query) => this.WriteShape(query.Shape, query.IndexedShape, query.Field, "shape");

    private void WriteShape(
      IGeoShape shape,
      IFieldLookup indexedField,
      Field field,
      string queryType)
    {
      switch (shape)
      {
        case null:
          if (indexedField != null)
          {
            this.Write("geo_indexed_shape");
            return;
          }
          break;
        case ICircleGeoShape _:
          this.Write("geo_shape_circle");
          return;
        case IEnvelopeGeoShape _:
          this.Write("geo_shape_envelope");
          return;
        case IGeometryCollection _:
          this.Write("geo_shape_geometrycollection");
          return;
        case ILineStringGeoShape _:
          this.Write("geo_shape_linestring");
          return;
        case IMultiLineStringGeoShape _:
          this.Write("geo_shape_multi_linestring");
          return;
        case IMultiPointGeoShape _:
          this.Write("geo_shape_multi_point");
          return;
        case IMultiPolygonGeoShape _:
          this.Write("geo_shape_multi_polygon");
          return;
        case IPointGeoShape _:
          this.Write("geo_shape_point");
          return;
        case IPolygonGeoShape _:
          this.Write("geo_shape_polygon");
          return;
      }
      this.Write(queryType, field);
    }

    public virtual void Visit(IHasChildQuery query) => this.Write("has_child");

    public virtual void Visit(IHasParentQuery query) => this.Write("has_parent");

    public virtual void Visit(IIdsQuery query) => this.Write("ids");

    public virtual void Visit(IIntervalsQuery query) => this.Write("intervals");

    public virtual void Visit(IMatchQuery query) => this.Write("match", query.Field);

    public virtual void Visit(IMatchPhraseQuery query) => this.Write("match_phrase", query.Field);

    public virtual void Visit(IMatchPhrasePrefixQuery query) => this.Write("match_phrase_prefix", query.Field);

    public virtual void Visit(IMatchAllQuery query) => this.Write("match_all");

    public virtual void Visit(IMatchBoolPrefixQuery query) => this.Write("match_bool_prefix");

    public virtual void Visit(IMatchNoneQuery query) => this.Write("match_none");

    public virtual void Visit(IMoreLikeThisQuery query) => this.Write("more_like_this");

    public virtual void Visit(IMultiMatchQuery query) => this.Write("multi_match");

    public virtual void Visit(INestedQuery query) => this.Write("nested");

    public virtual void Visit(IPrefixQuery query) => this.Write("prefix");

    public virtual void Visit(IQueryStringQuery query) => this.Write("query_string");

    public virtual void Visit(IRankFeatureQuery query) => this.Write("rank_feature");

    public virtual void Visit(IRangeQuery query) => this.Write("range");

    public virtual void Visit(IRegexpQuery query) => this.Write("regexp");

    public virtual void Visit(ISimpleQueryStringQuery query) => this.Write("simple_query_string");

    public virtual void Visit(ISpanFirstQuery query) => this.Write("span_first");

    public virtual void Visit(ISpanNearQuery query) => this.Write("span_near");

    public virtual void Visit(ISpanNotQuery query) => this.Write("span_not");

    public virtual void Visit(ISpanOrQuery query) => this.Write("span_or");

    public virtual void Visit(ISpanTermQuery query) => this.Write("span_term");

    public virtual void Visit(ISpanFieldMaskingQuery query) => this.Write("field_masking_span");

    public virtual void Visit(ITermQuery query) => this.Write("term", query.Field);

    public virtual void Visit(IWildcardQuery query) => this.Write("wildcard");

    public virtual void Visit(ITermsQuery query) => this.Write("terms");

    public virtual void Visit(IGeoPolygonQuery query) => this.Write("geo_polygon");

    public virtual void Visit(IGeoDistanceQuery query) => this.Write("geo_distance");

    public virtual void Visit(ISpanMultiTermQuery query) => this.Write("span_multi_term");

    public virtual void Visit(ISpanSubQuery query) => this.Write("span_sub");

    public virtual void Visit(ISpanQuery query) => this.Write("span");

    public virtual void Visit(IGeoBoundingBoxQuery query) => this.Write("geo_bounding_box");

    public virtual void Visit(IExistsQuery query) => this.Write("exists");

    public virtual void Visit(IScriptQuery query) => this.Write("script");

    public virtual void Visit(IScriptScoreQuery query) => this.Write("script_score");

    public virtual void Visit(IRawQuery query) => this.Write("raw");

    public virtual void Visit(IPercolateQuery query) => this.Write("percolate");

    public virtual void Visit(IParentIdQuery query) => this.Write("parent_id");

    public virtual void Visit(ITermsSetQuery query) => this.Write("terms_set");

    public virtual void Visit(IPinnedQuery query) => this.Write("pinned");

    public virtual void Visit(ICombinedFieldsQuery query) => this.Write("combined_fields");

    private void Write(string queryType, Dictionary<string, string> properties)
    {
      properties = properties ?? new Dictionary<string, string>();
      string str = string.Join(", ", properties.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (kv => kv.Key + ": " + kv.Value)));
      this._sb.AppendFormat("{0}{1}: {2} ({3}){4}", (object) new string('-', (this.Depth - 1) * 2), (object) this.Scope.GetStringValue().ToLowerInvariant(), (object) queryType, (object) str, (object) Environment.NewLine);
    }

    private void Write(string queryType, Field field = null)
    {
      string queryType1 = queryType;
      Dictionary<string, string> properties;
      if (!(field == (Field) null))
        properties = new Dictionary<string, string>()
        {
          {
            nameof (field),
            this._infer.Field(field)
          }
        };
      else
        properties = (Dictionary<string, string>) null;
      this.Write(queryType1, properties);
    }

    public virtual void Visit(IConditionlessQuery query) => this.Write("conditonless_query");
  }
}
