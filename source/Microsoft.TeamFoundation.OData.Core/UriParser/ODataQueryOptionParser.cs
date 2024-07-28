// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataQueryOptionParser
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Metadata;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public class ODataQueryOptionParser
  {
    private readonly IEdmType targetEdmType;
    private readonly IDictionary<string, string> queryOptions;
    private FilterClause filterClause;
    private SelectExpandClause selectExpandClause;
    private OrderByClause orderByClause;
    private SearchClause searchClause;
    private ApplyClause applyClause;
    private ComputeClause computeClause;
    private ODataPathInfo odataPathInfo;

    public ODataQueryOptionParser(
      IEdmModel model,
      IEdmType targetEdmType,
      IEdmNavigationSource targetNavigationSource,
      IDictionary<string, string> queryOptions)
      : this(model, targetEdmType, targetNavigationSource, queryOptions, (IServiceProvider) null)
    {
    }

    public ODataQueryOptionParser(
      IEdmModel model,
      IEdmType targetEdmType,
      IEdmNavigationSource targetNavigationSource,
      IDictionary<string, string> queryOptions,
      IServiceProvider container)
    {
      ExceptionUtils.CheckArgumentNotNull<IDictionary<string, string>>(queryOptions, nameof (queryOptions));
      this.odataPathInfo = new ODataPathInfo(targetEdmType, targetNavigationSource);
      this.targetEdmType = this.odataPathInfo.TargetEdmType;
      this.queryOptions = queryOptions;
      this.Configuration = new ODataUriParserConfiguration(model, container)
      {
        ParameterAliasValueAccessor = new ParameterAliasValueAccessor((IDictionary<string, string>) queryOptions.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (_ => _.Key.StartsWith("@", StringComparison.Ordinal))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (_ => _.Key), (Func<KeyValuePair<string, string>, string>) (_ => _.Value)))
      };
    }

    public ODataQueryOptionParser(
      IEdmModel model,
      ODataPath odataPath,
      IDictionary<string, string> queryOptions)
      : this(model, odataPath, queryOptions, (IServiceProvider) null)
    {
    }

    public ODataQueryOptionParser(
      IEdmModel model,
      ODataPath odataPath,
      IDictionary<string, string> queryOptions,
      IServiceProvider container)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPath>(odataPath, nameof (odataPath));
      ExceptionUtils.CheckArgumentNotNull<IDictionary<string, string>>(queryOptions, nameof (queryOptions));
      this.odataPathInfo = new ODataPathInfo(odataPath);
      this.targetEdmType = this.odataPathInfo.TargetEdmType;
      this.queryOptions = queryOptions;
      this.Configuration = new ODataUriParserConfiguration(model, container)
      {
        ParameterAliasValueAccessor = new ParameterAliasValueAccessor((IDictionary<string, string>) queryOptions.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (_ => _.Key.StartsWith("@", StringComparison.Ordinal))).ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (_ => _.Key), (Func<KeyValuePair<string, string>, string>) (_ => _.Value)))
      };
    }

    public ODataUriParserSettings Settings => this.Configuration.Settings;

    public IDictionary<string, SingleValueNode> ParameterAliasNodes => this.Configuration.ParameterAliasValueAccessor.ParameterAliasValueNodesCached;

    public ODataUriResolver Resolver
    {
      get => this.Configuration.Resolver;
      set => this.Configuration.Resolver = value;
    }

    internal ODataUriParserConfiguration Configuration { get; set; }

    public FilterClause ParseFilter()
    {
      if (this.filterClause != null)
        return this.filterClause;
      string filter;
      if (!this.TryGetQueryOption("$filter", out filter) || string.IsNullOrEmpty(filter) || this.targetEdmType == null)
        return (FilterClause) null;
      this.filterClause = this.ParseFilterImplementation(filter, this.Configuration, this.odataPathInfo);
      return this.filterClause;
    }

    public ApplyClause ParseApply()
    {
      if (this.applyClause != null)
        return this.applyClause;
      string apply;
      if (!this.TryGetQueryOption("$apply", out apply) || string.IsNullOrEmpty(apply) || this.targetEdmType == null)
        return (ApplyClause) null;
      this.applyClause = ODataQueryOptionParser.ParseApplyImplementation(apply, this.Configuration, this.odataPathInfo);
      return this.applyClause;
    }

    public SelectExpandClause ParseSelectAndExpand()
    {
      if (this.selectExpandClause != null)
        return this.selectExpandClause;
      string select;
      string expand;
      if (((!this.TryGetQueryOption("$select", out select) ? 1 : (select == null ? 1 : 0)) & (!this.TryGetQueryOption("$expand", out expand) ? 1 : (expand == null ? 1 : 0))) != 0 || this.targetEdmType == null)
        return (SelectExpandClause) null;
      if (!(this.targetEdmType is IEdmStructuredType))
        throw new ODataException(Microsoft.OData.Strings.UriParser_TypeInvalidForSelectExpand((object) this.targetEdmType));
      this.selectExpandClause = this.ParseSelectAndExpandImplementation(select, expand, this.Configuration, this.odataPathInfo);
      return this.selectExpandClause;
    }

    public OrderByClause ParseOrderBy()
    {
      if (this.orderByClause != null)
        return this.orderByClause;
      string orderBy;
      if (!this.TryGetQueryOption("$orderby", out orderBy) || string.IsNullOrEmpty(orderBy) || this.targetEdmType == null)
        return (OrderByClause) null;
      this.orderByClause = this.ParseOrderByImplementation(orderBy, this.Configuration, this.odataPathInfo);
      return this.orderByClause;
    }

    public long? ParseTop()
    {
      string topQuery;
      return !this.TryGetQueryOption("$top", out topQuery) ? new long?() : ODataQueryOptionParser.ParseTop(topQuery);
    }

    public long? ParseSkip()
    {
      string skipQuery;
      return !this.TryGetQueryOption("$skip", out skipQuery) ? new long?() : ODataQueryOptionParser.ParseSkip(skipQuery);
    }

    public long? ParseIndex()
    {
      string indexQuery;
      return !this.TryGetQueryOption("$index", out indexQuery) ? new long?() : ODataQueryOptionParser.ParseIndex(indexQuery);
    }

    public bool? ParseCount()
    {
      string count;
      return !this.TryGetQueryOption("$count", out count) ? new bool?() : ODataQueryOptionParser.ParseCount(count);
    }

    public SearchClause ParseSearch()
    {
      if (this.searchClause != null)
        return this.searchClause;
      string search;
      if (!this.TryGetQueryOption("$search", out search) || search == null)
        return (SearchClause) null;
      this.searchClause = ODataQueryOptionParser.ParseSearchImplementation(search, this.Configuration);
      return this.searchClause;
    }

    public string ParseSkipToken()
    {
      string str;
      return !this.TryGetQueryOption("$skiptoken", out str) ? (string) null : str;
    }

    public string ParseDeltaToken()
    {
      string str;
      return !this.TryGetQueryOption("$deltatoken", out str) ? (string) null : str;
    }

    public ComputeClause ParseCompute()
    {
      if (this.computeClause != null)
        return this.computeClause;
      string compute;
      if (!this.TryGetQueryOption("$compute", out compute) || string.IsNullOrEmpty(compute) || this.targetEdmType == null)
        return (ComputeClause) null;
      this.computeClause = this.ParseComputeImplementation(compute, this.Configuration, this.odataPathInfo);
      return this.computeClause;
    }

    private FilterClause ParseFilterImplementation(
      string filter,
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      ExceptionUtils.CheckArgumentNotNull<ODataPathInfo>(odataPathInfo, nameof (odataPathInfo));
      ExceptionUtils.CheckArgumentNotNull<string>(filter, nameof (filter));
      QueryToken filter1 = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier).ParseFilter(filter);
      BindingState bindingState = this.CreateBindingState(configuration, odataPathInfo);
      return new FilterBinder(new MetadataBinder.QueryTokenVisitor(new MetadataBinder(bindingState).Bind), bindingState).BindFilter(filter1);
    }

    private static ApplyClause ParseApplyImplementation(
      string apply,
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      ExceptionUtils.CheckArgumentNotNull<string>(apply, nameof (apply));
      IEnumerable<QueryToken> apply1 = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier).ParseApply(apply);
      BindingState bindingState = new BindingState(configuration, odataPathInfo.Segments.ToList<ODataPathSegment>());
      bindingState.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
      bindingState.RangeVariables.Push(bindingState.ImplicitRangeVariable);
      return new ApplyBinder(new MetadataBinder.QueryTokenVisitor(new MetadataBinder(bindingState).Bind), bindingState, configuration, odataPathInfo).BindApply(apply1);
    }

    private SelectExpandClause ParseSelectAndExpandImplementation(
      string select,
      string expand,
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(configuration.Model, "model");
      ExpandToken expandTree;
      SelectToken selectTree;
      SelectExpandSyntacticParser.Parse(select, expand, odataPathInfo.TargetStructuredType, configuration, out expandTree, out selectTree);
      BindingState bindingState = this.CreateBindingState(configuration, odataPathInfo);
      return SelectExpandSemanticBinder.Bind(odataPathInfo, expandTree, selectTree, configuration, bindingState);
    }

    private OrderByClause ParseOrderByImplementation(
      string orderBy,
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(configuration.Model, "model");
      ExceptionUtils.CheckArgumentNotNull<string>(orderBy, nameof (orderBy));
      IEnumerable<OrderByToken> orderBy1 = new UriQueryExpressionParser(configuration.Settings.OrderByLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier).ParseOrderBy(orderBy);
      BindingState bindingState = this.CreateBindingState(configuration, odataPathInfo);
      return new OrderByBinder(new MetadataBinder.QueryTokenVisitor(new MetadataBinder(bindingState).Bind)).BindOrderBy(bindingState, orderBy1);
    }

    private BindingState CreateBindingState(
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo)
    {
      BindingState bindingState = new BindingState(configuration, odataPathInfo.Segments.ToList<ODataPathSegment>());
      bindingState.ImplicitRangeVariable = NodeFactory.CreateImplicitRangeVariable(odataPathInfo.TargetEdmType.ToTypeReference(), odataPathInfo.TargetNavigationSource);
      bindingState.RangeVariables.Push(bindingState.ImplicitRangeVariable);
      if (this.applyClause != null)
      {
        bindingState.AggregatedPropertyNames = this.applyClause.GetLastAggregatedPropertyNames();
        if (this.applyClause.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (x => x.Kind == TransformationNodeKind.GroupBy || x.Kind == TransformationNodeKind.Aggregate)))
          bindingState.IsCollapsed = true;
      }
      if (this.computeClause != null)
      {
        HashSet<EndPathToken> other = new HashSet<EndPathToken>(this.computeClause.ComputedItems.Select<ComputeExpression, EndPathToken>((Func<ComputeExpression, EndPathToken>) (i => new EndPathToken(i.Alias, (QueryToken) null))));
        if (bindingState.AggregatedPropertyNames == null)
          bindingState.AggregatedPropertyNames = other;
        else
          bindingState.AggregatedPropertyNames.UnionWith((IEnumerable<EndPathToken>) other);
      }
      return bindingState;
    }

    private static long? ParseTop(string topQuery)
    {
      if (topQuery == null)
        return new long?();
      long result;
      return long.TryParse(topQuery, out result) && result >= 0L ? new long?(result) : throw new ODataException(Microsoft.OData.Strings.SyntacticTree_InvalidTopQueryOptionValue((object) topQuery));
    }

    private static long? ParseSkip(string skipQuery)
    {
      if (skipQuery == null)
        return new long?();
      long result;
      return long.TryParse(skipQuery, out result) && result >= 0L ? new long?(result) : throw new ODataException(Microsoft.OData.Strings.SyntacticTree_InvalidSkipQueryOptionValue((object) skipQuery));
    }

    private static long? ParseIndex(string indexQuery)
    {
      if (indexQuery == null)
        return new long?();
      long result;
      if (!long.TryParse(indexQuery, out result))
        throw new ODataException(Microsoft.OData.Strings.SyntacticTree_InvalidIndexQueryOptionValue((object) indexQuery));
      return new long?(result);
    }

    private static bool? ParseCount(string count)
    {
      if (count == null)
        return new bool?();
      switch (count.Trim())
      {
        case "true":
          return new bool?(true);
        case "false":
          return new bool?(false);
        default:
          throw new ODataException(Microsoft.OData.Strings.ODataUriParser_InvalidCount((object) count));
      }
    }

    private static SearchClause ParseSearchImplementation(
      string search,
      ODataUriParserConfiguration configuration)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      ExceptionUtils.CheckArgumentNotNull<string>(search, nameof (search));
      QueryToken search1 = new SearchParser(configuration.Settings.SearchLimit).ParseSearch(search);
      return new SearchBinder(new MetadataBinder.QueryTokenVisitor(new MetadataBinder(new BindingState(configuration)).Bind)).BindSearch(search1);
    }

    private ComputeClause ParseComputeImplementation(
      string compute,
      ODataUriParserConfiguration configuration,
      ODataPathInfo odataPathInfo)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataUriParserConfiguration>(configuration, nameof (configuration));
      ExceptionUtils.CheckArgumentNotNull<string>(compute, nameof (compute));
      ComputeToken compute1 = new UriQueryExpressionParser(configuration.Settings.FilterLimit, configuration.EnableCaseInsensitiveUriFunctionIdentifier).ParseCompute(compute);
      return new ComputeBinder(new MetadataBinder.QueryTokenVisitor(new MetadataBinder(this.CreateBindingState(configuration, odataPathInfo)).Bind)).BindCompute(compute1);
    }

    private bool TryGetQueryOption(string name, out string value)
    {
      value = (string) null;
      if (name == null)
        return false;
      string trimmedName = name.Trim();
      bool enableCaseInsensitive = this.Resolver.EnableCaseInsensitive;
      bool dollarQueryOptions = this.Configuration.EnableNoDollarQueryOptions;
      if (!enableCaseInsensitive && !dollarQueryOptions)
        return this.queryOptions.TryGetValue(trimmedName, out value);
      StringComparison stringComparison = enableCaseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
      string nameWithoutDollarPrefix = !dollarQueryOptions || !trimmedName.StartsWith("$", StringComparison.Ordinal) ? (string) null : trimmedName.Substring(1);
      List<KeyValuePair<string, string>> list = this.queryOptions.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (pair =>
      {
        if (string.Equals(trimmedName, pair.Key, stringComparison))
          return true;
        return nameWithoutDollarPrefix != null && string.Equals(nameWithoutDollarPrefix, pair.Key, stringComparison);
      })).ToList<KeyValuePair<string, string>>();
      if (list.Count == 0)
        return false;
      if (list.Count == 1)
      {
        value = list.First<KeyValuePair<string, string>>().Value;
        return true;
      }
      string p0;
      if (!dollarQueryOptions)
        p0 = trimmedName;
      else
        p0 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "${0}/{0}", new object[1]
        {
          (object) (nameWithoutDollarPrefix ?? trimmedName)
        });
      throw new ODataException(Microsoft.OData.Strings.QueryOptionUtils_QueryParameterMustBeSpecifiedOnce((object) p0));
    }
  }
}
