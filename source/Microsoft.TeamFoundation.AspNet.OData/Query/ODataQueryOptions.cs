// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ODataQueryOptions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Adapters;
using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using Microsoft.AspNet.OData.Query.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.OData.UriParser.Aggregation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace Microsoft.AspNet.OData.Query
{
  [ODataQueryParameterBinding]
  public class ODataQueryOptions
  {
    private static readonly MethodInfo _limitResultsGenericMethod = ((IEnumerable<MethodInfo>) typeof (ODataQueryOptions).GetMethods(BindingFlags.Static | BindingFlags.Public)).Single<MethodInfo>((Func<MethodInfo, bool>) (mi => mi.Name == "LimitResults" && mi.ContainsGenericParameters && mi.GetParameters().Length == 4));
    private ODataQueryOptionParser _queryOptionParser;
    private AllowedQueryOptions _ignoreQueryOptions;
    private ETag _etagIfMatch;
    private bool _etagIfMatchChecked;
    private ETag _etagIfNoneMatch;
    private bool _etagIfNoneMatchChecked;
    private bool _enableNoDollarSignQueryOptions;
    private OrderByQueryOption _stableOrderBy;
    private static readonly Func<TransformationNode, bool> _aggregateTransformPredicate = (Func<TransformationNode, bool>) (t => t.Kind == TransformationNodeKind.Aggregate || t.Kind == TransformationNodeKind.GroupBy);

    public ODataQueryOptions(ODataQueryContext context, HttpRequestMessage request)
    {
      if (context == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (context));
      context.RequestContainer = request != null ? request.GetRequestContainer() : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      this.Context = context;
      this.Request = request;
      this.InternalRequest = (IWebApiRequestMessage) new WebApiRequestMessage(request);
      this.InternalHeaders = (IWebApiHeaders) new WebApiRequestHeaders(request.Headers);
      this.Initialize(context);
    }

    public HttpRequestMessage Request { get; private set; }

    private void Initialize(ODataQueryContext context)
    {
      ODataUriResolver requiredService = ServiceProviderServiceExtensions.GetRequiredService<ODataUriResolver>(context.RequestContainer);
      if (requiredService != null)
        this._enableNoDollarSignQueryOptions = requiredService.EnableNoDollarQueryOptions;
      this.RawValues = new ODataRawQueryOptions();
      IDictionary<string, string> odataQueryParameters = this.GetODataQueryParameters();
      this._queryOptionParser = new ODataQueryOptionParser(context.Model, context.ElementType, context.NavigationSource, odataQueryParameters);
      this._queryOptionParser.Resolver = ServiceProviderServiceExtensions.GetRequiredService<ODataUriResolver>(context.RequestContainer);
      this.BuildQueryOptions(odataQueryParameters);
      this.Validator = ODataQueryValidator.GetODataQueryValidator(context);
    }

    internal IWebApiRequestMessage InternalRequest { get; private set; }

    public ODataQueryContext Context { get; private set; }

    public ODataRawQueryOptions RawValues { get; private set; }

    public SelectExpandQueryOption SelectExpand { get; private set; }

    public ApplyQueryOption Apply { get; private set; }

    public FilterQueryOption Filter { get; private set; }

    public OrderByQueryOption OrderBy { get; private set; }

    public SkipQueryOption Skip { get; private set; }

    public SkipTokenQueryOption SkipToken { get; private set; }

    public TopQueryOption Top { get; private set; }

    public CountQueryOption Count { get; private set; }

    public ComputeQueryOption Compute { get; private set; }

    public ODataQueryValidator Validator { get; set; }

    private IWebApiHeaders InternalHeaders { get; set; }

    public static bool IsSystemQueryOption(string queryOptionName) => ODataQueryOptions.IsSystemQueryOption(queryOptionName, false);

    public static bool IsSystemQueryOption(string queryOptionName, bool isDollarSignOptional)
    {
      string str = queryOptionName;
      if (isDollarSignOptional && !queryOptionName.StartsWith("$", StringComparison.Ordinal))
        str = "$" + queryOptionName;
      return str.Equals("$orderby", StringComparison.Ordinal) || str.Equals("$filter", StringComparison.Ordinal) || str.Equals("$top", StringComparison.Ordinal) || str.Equals("$skip", StringComparison.Ordinal) || str.Equals("$count", StringComparison.Ordinal) || str.Equals("$expand", StringComparison.Ordinal) || str.Equals("$select", StringComparison.Ordinal) || str.Equals("$format", StringComparison.Ordinal) || str.Equals("$skiptoken", StringComparison.Ordinal) || str.Equals("$deltatoken", StringComparison.Ordinal) || str.Equals("$apply", StringComparison.Ordinal) || str.Equals("$compute", StringComparison.Ordinal);
    }

    public virtual ETag IfMatch
    {
      get
      {
        IEnumerable<string> values;
        if (!this._etagIfMatchChecked && this._etagIfMatch == null && this.InternalHeaders.TryGetValues("If-Match", out values))
        {
          this._etagIfMatch = this.GetETag(EntityTagHeaderValue.Parse(values.SingleOrDefault<string>()));
          this._etagIfMatchChecked = true;
        }
        return this._etagIfMatch;
      }
    }

    public virtual ETag IfNoneMatch
    {
      get
      {
        if (!this._etagIfNoneMatchChecked && this._etagIfNoneMatch == null)
        {
          IEnumerable<string> values;
          if (this.InternalHeaders.TryGetValues("If-None-Match", out values))
          {
            this._etagIfNoneMatch = this.GetETag(EntityTagHeaderValue.Parse(values.SingleOrDefault<string>()));
            if (this._etagIfNoneMatch != null)
              this._etagIfNoneMatch.IsIfNoneMatch = true;
            this._etagIfNoneMatchChecked = true;
          }
          this._etagIfNoneMatchChecked = true;
        }
        return this._etagIfNoneMatch;
      }
    }

    internal virtual ETag GetETag(EntityTagHeaderValue etagHeaderValue) => this.InternalRequest.GetETag(etagHeaderValue);

    public bool IsSupportedQueryOption(string queryOptionName) => !(this._queryOptionParser != null ? this._queryOptionParser.Resolver : ServiceProviderServiceExtensions.GetRequiredService<ODataUriResolver>(this.Request.GetRequestContainer())).EnableCaseInsensitive ? ODataQueryOptions.IsSystemQueryOption(queryOptionName, this._enableNoDollarSignQueryOptions) : ODataQueryOptions.IsSystemQueryOption(queryOptionName.ToLowerInvariant(), this._enableNoDollarSignQueryOptions);

    public virtual IQueryable ApplyTo(IQueryable query) => this.ApplyTo(query, new ODataQuerySettings());

    public virtual IQueryable ApplyTo(IQueryable query, AllowedQueryOptions ignoreQueryOptions)
    {
      this._ignoreQueryOptions = ignoreQueryOptions;
      return this.ApplyTo(query, new ODataQuerySettings());
    }

    public virtual IQueryable ApplyTo(
      IQueryable query,
      ODataQuerySettings querySettings,
      AllowedQueryOptions ignoreQueryOptions)
    {
      this._ignoreQueryOptions = ignoreQueryOptions;
      return this.ApplyTo(query, querySettings);
    }

    public virtual IQueryable ApplyTo(IQueryable query, ODataQuerySettings querySettings)
    {
      if (query == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (query));
      if (querySettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (querySettings));
      IQueryable queryable1 = query;
      if (this.IsAvailableODataQueryOption((object) this.Apply, AllowedQueryOptions.Apply))
      {
        queryable1 = this.Apply.ApplyTo(queryable1, querySettings);
        this.InternalRequest.Context.ApplyClause = this.Apply.ApplyClause;
        if (this.Apply.SelectExpandClause != null)
          this.InternalRequest.Context.ProcessedSelectExpandClause = this.Apply.SelectExpandClause;
        this.Context.ElementClrType = this.Apply.ResultClrType;
      }
      if (this.IsAvailableODataQueryOption((object) this.Compute, AllowedQueryOptions.Compute))
        queryable1 = this.Compute.ApplyTo(queryable1, querySettings);
      if (this.IsAvailableODataQueryOption((object) this.Filter, AllowedQueryOptions.Filter))
        queryable1 = this.Filter.ApplyTo(queryable1, querySettings);
      if (this.IsAvailableODataQueryOption((object) this.Count, AllowedQueryOptions.Count))
      {
        if (this.InternalRequest.Context.TotalCountFunc == null)
        {
          Func<long> entityCountFunc = this.Count.GetEntityCountFunc(queryable1);
          if (entityCountFunc != null)
            this.InternalRequest.Context.TotalCountFunc = entityCountFunc;
        }
        if (this.InternalRequest.IsCountRequest())
          return queryable1;
      }
      OrderByQueryOption queryOption = this.OrderBy;
      if (querySettings.EnsureStableOrdering && (this.IsAvailableODataQueryOption((object) this.Skip, AllowedQueryOptions.Skip) || this.IsAvailableODataQueryOption((object) this.Top, AllowedQueryOptions.Top) || querySettings.PageSize.HasValue))
        queryOption = this.GenerateStableOrder();
      if (this.IsAvailableODataQueryOption((object) queryOption, AllowedQueryOptions.OrderBy))
        queryable1 = (IQueryable) queryOption.ApplyTo(queryable1, querySettings);
      if (this.IsAvailableODataQueryOption((object) this.SkipToken, AllowedQueryOptions.SkipToken))
        queryable1 = this.SkipToken.ApplyTo(queryable1, querySettings, this);
      if (!ODataQueryOptions.IsAggregated(this.Apply?.ApplyClause))
        this.AddAutoSelectExpandProperties();
      if (this.SelectExpand != null)
      {
        IQueryable queryable2 = this.ApplySelectExpand<IQueryable>(queryable1, querySettings);
        if (queryable2 != null)
          queryable1 = queryable2;
      }
      if (this.IsAvailableODataQueryOption((object) this.Skip, AllowedQueryOptions.Skip))
        queryable1 = this.Skip.ApplyTo(queryable1, querySettings);
      if (this.IsAvailableODataQueryOption((object) this.Top, AllowedQueryOptions.Top))
        queryable1 = this.Top.ApplyTo(queryable1, querySettings);
      return this.ApplyPaging(queryable1, querySettings);
    }

    internal IQueryable ApplyPaging(IQueryable result, ODataQuerySettings querySettings)
    {
      if (!querySettings.PostponePaging)
      {
        int num = -1;
        if (querySettings.PageSize.HasValue)
          num = querySettings.PageSize.Value;
        else if (querySettings.ModelBoundPageSize.HasValue)
          num = querySettings.ModelBoundPageSize.Value;
        int pageSize = -1;
        if (RequestPreferenceHelpers.RequestPrefersMaxPageSize(this.InternalRequest.Headers, out pageSize))
          num = Math.Min(num, pageSize);
        if (num > 0)
        {
          bool resultsLimited;
          result = ODataQueryOptions.LimitResults(result, num, querySettings.EnableConstantParameterization, out resultsLimited);
          if (resultsLimited && this.InternalRequest.RequestUri != (Uri) null && this.InternalRequest.Context.NextLink == (Uri) null)
            this.InternalRequest.Context.PageSize = num;
        }
      }
      this.InternalRequest.Context.QueryOptions = this;
      return result;
    }

    public virtual OrderByQueryOption GenerateStableOrder()
    {
      if (this._stableOrderBy != null)
        return this._stableOrderBy;
      ApplyClause applyClause = this.Apply != null ? this.Apply.ApplyClause : (ApplyClause) null;
      if (ODataQueryOptions.IsAggregatedToSingleResult(applyClause))
        return (OrderByQueryOption) null;
      List<string> applySortOptions = ODataQueryOptions.GetApplySortOptions(applyClause);
      this._stableOrderBy = this.OrderBy == null ? this.GenerateDefaultOrderBy(this.Context, applySortOptions) : this.EnsureStableSortOrderBy(this.OrderBy, this.Context, applySortOptions);
      return this._stableOrderBy;
    }

    private static bool IsAggregated(ApplyClause apply) => apply != null && apply.Transformations.Any<TransformationNode>(ODataQueryOptions._aggregateTransformPredicate);

    private static bool IsAggregatedToSingleResult(ApplyClause apply) => apply != null && apply.Transformations.Any<TransformationNode>((Func<TransformationNode, bool>) (t => t.Kind == TransformationNodeKind.Aggregate));

    private static List<string> GetApplySortOptions(ApplyClause apply)
    {
      if (!ODataQueryOptions.IsAggregated(apply))
        return (List<string>) null;
      List<string> result = new List<string>();
      TransformationNode transformationNode = apply.Transformations.Last<TransformationNode>(ODataQueryOptions._aggregateTransformPredicate);
      if (transformationNode.Kind == TransformationNodeKind.Aggregate)
      {
        foreach (AggregateExpression aggregateExpression in (transformationNode as AggregateTransformationNode).AggregateExpressions.OfType<AggregateExpression>())
          result.Add(aggregateExpression.Alias);
      }
      else if (transformationNode.Kind == TransformationNodeKind.GroupBy)
      {
        IEnumerable<GroupByPropertyNode> groupingProperties = (transformationNode as GroupByTransformationNode).GroupingProperties;
        ODataQueryOptions.ExtractGroupingProperties(result, groupingProperties);
      }
      return result;
    }

    private static void ExtractGroupingProperties(
      List<string> result,
      IEnumerable<GroupByPropertyNode> groupingProperties,
      string prefix = null)
    {
      foreach (GroupByPropertyNode groupingProperty in groupingProperties)
      {
        string prefix1 = prefix != null ? prefix + "/" + groupingProperty.Name : groupingProperty.Name;
        if (groupingProperty.ChildTransformations != null && groupingProperty.ChildTransformations.Any<GroupByPropertyNode>())
          ODataQueryOptions.ExtractGroupingProperties(result, (IEnumerable<GroupByPropertyNode>) groupingProperty.ChildTransformations, prefix1);
        else
          result.Add(prefix1);
      }
    }

    public virtual object ApplyTo(
      object entity,
      ODataQuerySettings querySettings,
      AllowedQueryOptions ignoreQueryOptions)
    {
      this._ignoreQueryOptions = ignoreQueryOptions;
      return this.ApplyTo(entity, new ODataQuerySettings());
    }

    public virtual object ApplyTo(object entity, ODataQuerySettings querySettings)
    {
      if (entity == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entity));
      if (querySettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (querySettings));
      if (this.Filter != null || this.OrderBy != null || this.Top != null || this.Skip != null || this.Count != null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.NonSelectExpandOnSingleEntity);
      this.AddAutoSelectExpandProperties();
      if (this.SelectExpand != null)
      {
        object obj = this.ApplySelectExpand<object>(entity, querySettings);
        if (obj != null)
          return obj;
      }
      return entity;
    }

    public virtual void Validate(ODataValidationSettings validationSettings)
    {
      if (validationSettings == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (validationSettings));
      if (this.Validator == null)
        return;
      this.Validator.Validate(this, validationSettings);
    }

    private static void ThrowIfEmpty(string queryValue, string queryName)
    {
      if (string.IsNullOrWhiteSpace(queryValue))
        throw new ODataException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.QueryCannotBeEmpty, (object) queryName));
    }

    private static IEnumerable<IEdmStructuralProperty> GetAvailableOrderByProperties(
      ODataQueryContext context)
    {
      return !(context.ElementType is IEdmEntityType elementType) ? Enumerable.Empty<IEdmStructuralProperty>() : (IEnumerable<IEdmStructuralProperty>) (elementType.Key().Any<IEdmStructuralProperty>() ? elementType.Key() : (IEnumerable<IEdmStructuralProperty>) elementType.StructuralProperties().Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (property => property.Type.IsPrimitive() && !property.Type.IsStream())).OrderBy<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (p => p.Name))).ToList<IEdmStructuralProperty>();
    }

    private OrderByQueryOption GenerateDefaultOrderBy(
      ODataQueryContext context,
      List<string> applySortOptions)
    {
      string empty = string.Empty;
      if (applySortOptions != null)
        return new OrderByQueryOption(string.Join(",", (IEnumerable<string>) applySortOptions), context, this.Apply.RawValue);
      string rawValue = string.Join(",", ODataQueryOptions.GetAvailableOrderByProperties(context).Select<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (property => property.Name)));
      return !string.IsNullOrEmpty(rawValue) ? new OrderByQueryOption(rawValue, context) : (OrderByQueryOption) null;
    }

    private OrderByQueryOption EnsureStableSortOrderBy(
      OrderByQueryOption orderBy,
      ODataQueryContext context,
      List<string> applySortOptions)
    {
      Func<OrderByPropertyNode, string> selector = applySortOptions == null ? (Func<OrderByPropertyNode, string>) (node => node.Property.Name) : (Func<OrderByPropertyNode, string>) (node => node.PropertyPath);
      HashSet<string> usedPropertyNames = new HashSet<string>(orderBy.OrderByNodes.OfType<OrderByPropertyNode>().Select<OrderByPropertyNode, string>(selector).Concat<string>(orderBy.OrderByNodes.OfType<OrderByOpenPropertyNode>().Select<OrderByOpenPropertyNode, string>((Func<OrderByOpenPropertyNode, string>) (p => p.PropertyName))));
      if (applySortOptions != null)
      {
        IOrderedEnumerable<string> orderedEnumerable = applySortOptions.Where<string>((Func<string, bool>) (p => !usedPropertyNames.Contains(p))).OrderBy<string, string>((Func<string, string>) (p => p));
        if (orderedEnumerable.Any<string>())
          orderBy = new OrderByQueryOption(orderBy.RawValue + "," + string.Join(",", (IEnumerable<string>) orderedEnumerable), context, this.Apply.RawValue);
      }
      else
      {
        IEnumerable<IEdmStructuralProperty> source = ODataQueryOptions.GetAvailableOrderByProperties(context).Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (prop => !usedPropertyNames.Contains(prop.Name)));
        if (source.Any<IEdmStructuralProperty>())
        {
          orderBy = new OrderByQueryOption(orderBy);
          foreach (IEdmStructuralProperty property in source)
            orderBy.OrderByNodes.Add((OrderByNode) new OrderByPropertyNode((IEdmProperty) property, OrderByDirection.Ascending));
        }
      }
      return orderBy;
    }

    internal static IQueryable LimitResults(
      IQueryable queryable,
      int limit,
      bool parameterize,
      out bool resultsLimited)
    {
      MethodInfo methodInfo = ODataQueryOptions._limitResultsGenericMethod.MakeGenericMethod(queryable.ElementType);
      object[] parameters = new object[4]
      {
        (object) queryable,
        (object) limit,
        (object) parameterize,
        null
      };
      IQueryable queryable1 = methodInfo.Invoke((object) null, parameters) as IQueryable;
      resultsLimited = (bool) parameters[3];
      return queryable1;
    }

    public static IQueryable<T> LimitResults<T>(
      IQueryable<T> queryable,
      int limit,
      out bool resultsLimited)
    {
      return ODataQueryOptions.LimitResults<T>(queryable, limit, false, out resultsLimited);
    }

    public static IQueryable<T> LimitResults<T>(
      IQueryable<T> queryable,
      int limit,
      bool parameterize,
      out bool resultsLimited)
    {
      TruncatedCollection<T> source = new TruncatedCollection<T>(queryable, limit, parameterize);
      resultsLimited = source.IsTruncated;
      return source.AsQueryable<T>();
    }

    internal void AddAutoSelectExpandProperties()
    {
      bool flag = false;
      string expand = this.GetAutoExpandRawValue();
      string select = this.GetAutoSelectRawValue();
      IDictionary<string, string> odataQueryParameters = this.GetODataQueryParameters();
      if (!string.IsNullOrEmpty(expand) && !expand.Equals(this.RawValues.Expand))
      {
        odataQueryParameters["$expand"] = expand;
        flag = true;
      }
      else
        expand = this.RawValues.Expand;
      if (!string.IsNullOrEmpty(select) && !select.Equals(this.RawValues.Select))
      {
        odataQueryParameters["$select"] = select;
        flag = true;
      }
      else
        select = this.RawValues.Select;
      if (!flag)
        return;
      this._queryOptionParser = new ODataQueryOptionParser(this.Context.Model, this.Context.ElementType, this.Context.NavigationSource, odataQueryParameters, this.Context.RequestContainer);
      if (this.Apply != null)
        this._queryOptionParser.ParseApply();
      if (this.Compute != null)
        this._queryOptionParser.ParseCompute();
      SelectExpandQueryOption selectExpand = this.SelectExpand;
      this.SelectExpand = new SelectExpandQueryOption(select, expand, this.Context, this._queryOptionParser);
      ModelBoundQuerySettings boundQuerySettings = EdmLibHelpers.GetModelBoundQuerySettings<IEdmType>(this.Context.ElementType, this.Context.Model, this.Context.DefaultQuerySettings);
      SelectExpandClause selectExpandClause = this.SelectExpand.SelectExpandClause;
      int num;
      if (boundQuerySettings != null)
      {
        SelectExpandType? defaultSelectType = boundQuerySettings.DefaultSelectType;
        SelectExpandType selectExpandType = SelectExpandType.Automatic;
        num = defaultSelectType.GetValueOrDefault() == selectExpandType & defaultSelectType.HasValue ? 1 : 0;
      }
      else
        num = 0;
      selectExpandClause.AllAutoSelected = num != 0;
      if (selectExpand == null || selectExpand.LevelsMaxLiteralExpansionDepth <= 0)
        return;
      this.SelectExpand.LevelsMaxLiteralExpansionDepth = selectExpand.LevelsMaxLiteralExpansionDepth;
    }

    private IDictionary<string, string> GetODataQueryParameters()
    {
      Dictionary<string, string> odataQueryParameters = new Dictionary<string, string>();
      foreach (KeyValuePair<string, string> queryParameter in (IEnumerable<KeyValuePair<string, string>>) this.InternalRequest.QueryParameters)
      {
        string key = queryParameter.Key.Trim();
        if (!this._enableNoDollarSignQueryOptions)
        {
          if (key.StartsWith("$", StringComparison.Ordinal))
            odataQueryParameters.Add(key, queryParameter.Value);
        }
        else if (this.IsSupportedQueryOption(queryParameter.Key))
          odataQueryParameters.Add(!key.StartsWith("$", StringComparison.Ordinal) ? "$" + key : key, queryParameter.Value);
        if (key.StartsWith("@", StringComparison.Ordinal))
          odataQueryParameters.Add(key, queryParameter.Value);
      }
      return (IDictionary<string, string>) odataQueryParameters;
    }

    private string GetAutoSelectRawValue()
    {
      string autoSelectRawValue = this.RawValues.Select;
      string str1 = string.Empty;
      IEdmEntityType targetStructuredType = this.Context.TargetStructuredType as IEdmEntityType;
      if (string.IsNullOrEmpty(autoSelectRawValue))
      {
        foreach (IEdmStructuralProperty autoSelectProperty in EdmLibHelpers.GetAutoSelectProperties(this.Context.TargetProperty, this.Context.TargetStructuredType, this.Context.Model))
        {
          if (!string.IsNullOrEmpty(str1))
            str1 += ",";
          if (targetStructuredType != null && autoSelectProperty.DeclaringType != targetStructuredType)
            str1 += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/", new object[1]
            {
              (object) autoSelectProperty.DeclaringType.FullTypeName()
            });
          str1 += autoSelectProperty.Name;
        }
        if (!string.IsNullOrEmpty(str1))
        {
          if (this.Compute != null)
          {
            string str2 = string.Join(",", this.Compute.ComputeClause.ComputedItems.Select<ComputeExpression, string>((Func<ComputeExpression, string>) (c => c.Alias)));
            if (!string.IsNullOrEmpty(str2))
              str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
              {
                (object) str1,
                (object) str2
              });
          }
          if (this.Apply != null)
          {
            string str3 = string.Join(",", this.Apply.ApplyClause.Transformations.OfType<ComputeTransformationNode>().SelectMany<ComputeTransformationNode, ComputeExpression>((Func<ComputeTransformationNode, IEnumerable<ComputeExpression>>) (c => c.Expressions)).Select<ComputeExpression, string>((Func<ComputeExpression, string>) (c => c.Alias)));
            if (!string.IsNullOrEmpty(str3))
              str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
              {
                (object) str1,
                (object) str3
              });
          }
          if (!string.IsNullOrEmpty(autoSelectRawValue))
            autoSelectRawValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
            {
              (object) str1,
              (object) autoSelectRawValue
            });
          else
            autoSelectRawValue = str1;
        }
      }
      return autoSelectRawValue;
    }

    private string GetAutoExpandRawValue()
    {
      string autoExpandRawValue = this.RawValues.Expand;
      IEdmEntityType targetStructuredType = this.Context.TargetStructuredType as IEdmEntityType;
      string empty = string.Empty;
      foreach (IEdmNavigationProperty navigationProperty in EdmLibHelpers.GetAutoExpandNavigationProperties(this.Context.TargetProperty, this.Context.TargetStructuredType, this.Context.Model, !string.IsNullOrEmpty(this.RawValues.Select)))
      {
        if (!string.IsNullOrEmpty(empty))
          empty += ",";
        if (navigationProperty.DeclaringEntityType() != targetStructuredType)
          empty += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/", new object[1]
          {
            (object) navigationProperty.DeclaringEntityType().FullTypeName()
          });
        empty += navigationProperty.Name;
      }
      if (!string.IsNullOrEmpty(empty))
      {
        if (!string.IsNullOrEmpty(autoExpandRawValue))
          autoExpandRawValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},{1}", new object[2]
          {
            (object) empty,
            (object) autoExpandRawValue
          });
        else
          autoExpandRawValue = empty;
      }
      return autoExpandRawValue;
    }

    private void BuildQueryOptions(IDictionary<string, string> queryParameters)
    {
      foreach (KeyValuePair<string, string> queryParameter in (IEnumerable<KeyValuePair<string, string>>) queryParameters)
      {
        switch (queryParameter.Key.ToLowerInvariant())
        {
          case "$apply":
            ODataQueryOptions.ThrowIfEmpty(queryParameter.Value, "$apply");
            this.RawValues.Apply = queryParameter.Value;
            this.Apply = new ApplyQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          case "$compute":
            ODataQueryOptions.ThrowIfEmpty(queryParameter.Value, "$compute");
            this.RawValues.Compute = queryParameter.Value;
            this.Compute = new ComputeQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          case "$count":
            ODataQueryOptions.ThrowIfEmpty(queryParameter.Value, "$count");
            this.RawValues.Count = queryParameter.Value;
            this.Count = new CountQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          case "$deltatoken":
            this.RawValues.DeltaToken = queryParameter.Value;
            continue;
          case "$expand":
            this.RawValues.Expand = queryParameter.Value;
            continue;
          case "$filter":
            ODataQueryOptions.ThrowIfEmpty(queryParameter.Value, "$filter");
            this.RawValues.Filter = queryParameter.Value;
            this.Filter = new FilterQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          case "$format":
            this.RawValues.Format = queryParameter.Value;
            continue;
          case "$orderby":
            ODataQueryOptions.ThrowIfEmpty(queryParameter.Value, "$orderby");
            this.RawValues.OrderBy = queryParameter.Value;
            this.OrderBy = new OrderByQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          case "$select":
            this.RawValues.Select = queryParameter.Value;
            continue;
          case "$skip":
            ODataQueryOptions.ThrowIfEmpty(queryParameter.Value, "$skip");
            this.RawValues.Skip = queryParameter.Value;
            this.Skip = new SkipQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          case "$skiptoken":
            this.RawValues.SkipToken = queryParameter.Value;
            this.SkipToken = new SkipTokenQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          case "$top":
            ODataQueryOptions.ThrowIfEmpty(queryParameter.Value, "$top");
            this.RawValues.Top = queryParameter.Value;
            this.Top = new TopQueryOption(queryParameter.Value, this.Context, this._queryOptionParser);
            continue;
          default:
            continue;
        }
      }
      if (this.RawValues.Select != null || this.RawValues.Expand != null)
        this.SelectExpand = new SelectExpandQueryOption(this.RawValues.Select, this.RawValues.Expand, this.Context, this._queryOptionParser);
      if (!this.InternalRequest.IsCountRequest())
        return;
      ODataQueryContext context = this.Context;
      IEdmModel model = this.Context.Model;
      IEdmType elementType = this.Context.ElementType;
      IEdmNavigationSource navigationSource = this.Context.NavigationSource;
      Dictionary<string, string> queryOptions = new Dictionary<string, string>();
      queryOptions.Add("$count", "true");
      IServiceProvider requestContainer = this.Context.RequestContainer;
      ODataQueryOptionParser queryOptionParser = new ODataQueryOptionParser(model, elementType, navigationSource, (IDictionary<string, string>) queryOptions, requestContainer);
      this.Count = new CountQueryOption("true", context, queryOptionParser);
    }

    private bool IsAvailableODataQueryOption(
      object queryOption,
      AllowedQueryOptions queryOptionFlag)
    {
      return queryOption != null && (this._ignoreQueryOptions & queryOptionFlag) == AllowedQueryOptions.None;
    }

    private T ApplySelectExpand<T>(T entity, ODataQuerySettings querySettings)
    {
      T obj = default (T);
      bool flag1 = this.IsAvailableODataQueryOption((object) this.SelectExpand.RawSelect, AllowedQueryOptions.Select);
      bool flag2 = this.IsAvailableODataQueryOption((object) this.SelectExpand.RawExpand, AllowedQueryOptions.Expand);
      if (flag1 | flag2)
      {
        if (!flag1 && this.SelectExpand.RawSelect != null || !flag2 && this.SelectExpand.RawExpand != null)
          this.SelectExpand = new SelectExpandQueryOption(flag1 ? this.RawValues.Select : (string) null, flag2 ? this.RawValues.Expand : (string) null, this.SelectExpand.Context);
        SelectExpandClause selectExpandClause = this.SelectExpand.ProcessedSelectExpandClause;
        SelectExpandQueryOption expandQueryOption = new SelectExpandQueryOption(this.SelectExpand.RawSelect, this.SelectExpand.RawExpand, this.SelectExpand.Context, selectExpandClause);
        this.InternalRequest.Context.ProcessedSelectExpandClause = selectExpandClause;
        this.InternalRequest.Context.QueryOptions = this;
        Type type = typeof (T);
        if (type == typeof (IQueryable))
          obj = (T) expandQueryOption.ApplyTo((IQueryable) (object) entity, querySettings);
        else if (type == typeof (object))
          obj = (T) expandQueryOption.ApplyTo((object) entity, querySettings);
      }
      return obj;
    }
  }
}
