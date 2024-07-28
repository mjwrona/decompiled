// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataQuery
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Common;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Text;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class ODataQuery
  {
    public const string Area = "OData";
    public const string Layer = "ODataQuery";
    private IQueryable _queryable;
    private HttpRequestMessage _request;
    private ODataQuerySettings _querySettings;
    private long? _newSkipToken;
    private bool _skipTokenApplied;
    private bool _emptyPageFound;
    private Action<QueryType, HintStrategyFactory, Action> _queryWrapper;
    private HintStrategyFactory _hintFactory;
    private ODataQueryOptions _queryOptions;
    private ProjectInfo _projectInfo;
    private Type _clrType;
    private bool _userHasAccessToAllProjects;
    private IList<Expression> _currentProjectBooleanFilterExpressions;
    private bool _forceSnapshotSinglePage;
    private bool _hasReadEuiiPermission;
    private ISet<string> _queriedModelTables = (ISet<string>) new HashSet<string>();
    private HashSet<Guid> _accessibleProjectGuids;
    private List<Project> _allProjects;

    public ODataQuerySizeOptions QuerySizeOptions { get; }

    public IEnumerable<DataQualityResult> DataQualityResults { get; private set; }

    public bool EnablePageOptimization { get; internal set; }

    public bool EnablePageOptimizationMultiplier { get; internal set; }

    private bool IsProjectScopeQuery => this._projectInfo != null;

    public IReadOnlySet<string> QueriedModelTables => (IReadOnlySet<string>) new ReadOnlySet<string>(this._queriedModelTables);

    private HashSet<Guid> GetAccessibleProjectGuids(IVssRequestContext requestContext)
    {
      if (this._accessibleProjectGuids == null)
        this._accessibleProjectGuids = new HashSet<Guid>(requestContext.GetService<IAnalyticsSecurityService>().GetAccessibleProjects(requestContext).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.Id)));
      return this._accessibleProjectGuids;
    }

    private List<Project> GetAllProjects(AnalyticsComponent component)
    {
      if (this._allProjects == null)
        this._allProjects = component.GetTable<Project>().ToList<Project>();
      return this._allProjects;
    }

    internal ODataQuery(
      IVssRequestContext requestContext,
      ODataQueryOptions oDataQueryOptions,
      ODataQuerySizeOptions querySizeOptions,
      HttpRequestMessage request,
      IEdmEntityType entityType,
      ProjectInfo projectInfo,
      Func<AnalyticsComponent> getComponentFunc)
    {
      this._request = request;
      this._clrType = oDataQueryOptions.Context.ElementClrType;
      this._queryOptions = oDataQueryOptions;
      this._projectInfo = projectInfo;
      this._hasReadEuiiPermission = requestContext.GetService<IAnalyticsSecurityService>().HasReadEuiiPermission(requestContext);
      this.ValidateQuery(requestContext);
      this._querySettings = new ODataQuerySettings();
      this._querySettings.HandleNullPropagation = HandleNullPropagationOption.False;
      this._querySettings.HandleReferenceNavigationPropertyExpandFilter = true;
      this.QuerySizeOptions = querySizeOptions ?? new ODataQuerySizeOptions();
      this.ValidatePermission(requestContext, getComponentFunc);
    }

    private void ValidatePermission(
      IVssRequestContext requestContext,
      Func<AnalyticsComponent> getComponentFunc)
    {
      PermissionsValidator permissionsValidator = new PermissionsValidator(requestContext, this._queryOptions, (Func<HashSet<Guid>>) (() => this.GetAccessibleProjectGuids(requestContext)), (Func<List<Project>>) (() =>
      {
        using (AnalyticsComponent component = getComponentFunc())
          return this.GetAllProjects(component);
      }), this._querySettings, this._projectInfo != null);
      permissionsValidator.Validate(requestContext);
      this._userHasAccessToAllProjects = permissionsValidator.UserHasAccessToAllProjects;
      this._currentProjectBooleanFilterExpressions = permissionsValidator.CurrentProjectFilterExpressions;
    }

    internal object Response { get; private set; }

    public int ResponseRecordsCount => !(this.Response is IODataEnumerable response) ? 0 : response.GetCurrentCount();

    public Type PayloadType { get; private set; }

    public object Baseline => this.PayloadType.IsValueType ? this.Response : (object) this._queryable;

    internal void PreComponent(IVssRequestContext requestContext)
    {
      if (!this.AppendProjectFilter)
        return;
      this.GetAccessibleProjectGuids(requestContext);
    }

    private bool AppendProjectFilter => !this.IsProjectScopeQuery && this._clrType == typeof (Project);

    internal void Process(
      IVssRequestContext requestContext,
      IQueryable queryable,
      AnalyticsComponent component,
      Action<QueryType, HintStrategyFactory, Action> queryWrapper)
    {
      this._queryable = queryable;
      this._queryWrapper = queryWrapper;
      object obj = this.PrepareQuery(requestContext, this._queryable, component);
      if (obj is IQueryable queryable1)
      {
        object enumerator = this.GetEnumerator(requestContext, queryable1);
        this.PayloadType = typeof (IEnumerable<>).MakeGenericType(this._queryable.ElementType);
        this.Response = enumerator;
      }
      else
      {
        this.PayloadType = obj.GetType();
        this.Response = obj;
      }
    }

    private object PrepareQuery(
      IVssRequestContext requestContext,
      IQueryable queryable,
      AnalyticsComponent component)
    {
      requestContext.TraceEnter(12013005, AnalyticsService.Area, AnalyticsService.Layer, nameof (PrepareQuery));
      try
      {
        Type elementType = queryable.ElementType;
        ODataQuery.PagingStyle pagingStyle = ODataQuery.UseSkipTokenPaging(queryable, this._queryOptions);
        if (pagingStyle == ODataQuery.PagingStyle.Regular)
        {
          this._querySettings.PostponePaging = true;
          this._querySettings.PageSize = (int?) this.QuerySizeOptions?.PageSize;
        }
        if (this.AppendProjectFilter && !this._userHasAccessToAllProjects)
        {
          ArgumentUtility.CheckForNull<HashSet<Guid>>(this._accessibleProjectGuids, "_accessibleProjectGuids");
          queryable = (IQueryable) (queryable as IQueryable<Project>).Where<Project>((Expression<Func<Project, bool>>) (i => this._accessibleProjectGuids.Contains(i.ProjectSK)));
        }
        queryable = QueryableExtensions.ApplyProjectScopeFilterInternal(queryable, this._projectInfo);
        if (typeof (ICurrentProjectScoped).IsAssignableFrom(this._clrType))
          queryable = CurrentProjectFilter.ApplyCurrentProjectFilter(requestContext, queryable, this._projectInfo, this._clrType, this._currentProjectBooleanFilterExpressions);
        queryable = this.ApplyQuery(requestContext, queryable, this._queryOptions);
        this._queriedModelTables.AddRange<string, ISet<string>>((IEnumerable<string>) ExpressionQueriedEntityExtractor.Extract(queryable.Expression));
        if (!this._hasReadEuiiPermission)
          EuiiValidator.Validate(requestContext, this._queryOptions, this._querySettings, queryable);
        queryable = PredictRewrite.Rewrite(queryable, component);
        queryable = ManyToManyCountRewrite.Rewrite(elementType, queryable);
        this._hintFactory = new HintStrategyFactory(elementType, queryable, this._queryOptions);
        if (this._queryOptions.Count != null)
        {
          ODataQuerySizeOptions querySizeOptions = this.QuerySizeOptions;
          if ((querySizeOptions != null ? (!querySizeOptions.MaxSize.HasValue ? 1 : 0) : 1) != 0 && this._request.ODataProperties().TotalCount.HasValue)
            this._queryWrapper(QueryType.Count, this._hintFactory, (Action) (() => this._request.ODataProperties().TotalCount = new long?((long) ExpressionHelpers.Count(queryable))));
        }
        if (ODataQuery.IsCountRequest(this._request))
        {
          long? totalCount = this._request.ODataProperties().TotalCount;
          if (totalCount.HasValue)
            return (object) totalCount.Value;
        }
        if (pagingStyle != ODataQuery.PagingStyle.Regular)
          queryable = pagingStyle == ODataQuery.PagingStyle.IntSkipToken ? this.ApplyPaging<int>(requestContext, elementType, queryable, this._queryOptions.Context, this._queryOptions) : this.ApplyPaging<long>(requestContext, elementType, queryable, this._queryOptions.Context, this._queryOptions);
        return (object) queryable;
      }
      finally
      {
        requestContext.TraceLeave(12013006, AnalyticsService.Area, AnalyticsService.Layer, nameof (PrepareQuery));
      }
    }

    private T AddPageLimit<T>(T value, int multiplier) where T : struct => (T) Convert.ChangeType((object) (Convert.ToInt64((object) value) + (long) (1048576 * multiplier)), typeof (T));

    private ODataQuery.CountCheckResult<T> ValidateMaxSizePreferences<T>(
      IVssRequestContext requestContext,
      IQueryable<IContinuation<T>> queryable,
      ODataQueryOptions queryOptions)
      where T : struct
    {
      requestContext.TraceEnter(12013013, AnalyticsService.Area, AnalyticsService.Layer, nameof (ValidateMaxSizePreferences));
      try
      {
        ODataQuerySizeOptions querySizeOptions = this.QuerySizeOptions;
        int? maxSize1;
        int num1;
        if (querySizeOptions == null)
        {
          num1 = 0;
        }
        else
        {
          maxSize1 = querySizeOptions.MaxSize;
          num1 = maxSize1.HasValue ? 1 : 0;
        }
        if (num1 != 0)
        {
          if (queryOptions.RawValues.SkipToken == null)
          {
            TopQueryOption top = queryOptions.Top;
            int num2;
            if (top == null)
            {
              num2 = 1;
            }
            else
            {
              int num3 = top.Value;
              num2 = 0;
            }
            if (num2 == 0)
            {
              maxSize1 = queryOptions.Top?.Value;
              int? maxSize2 = this.QuerySizeOptions.MaxSize;
              if (!(maxSize1.GetValueOrDefault() > maxSize2.GetValueOrDefault() & maxSize1.HasValue & maxSize2.HasValue))
                goto label_16;
            }
            ODataQuery.CountCheckResult<T> queryCount = (ODataQuery.CountCheckResult<T>) null;
            ParameterExpression parameterExpression;
            // ISSUE: method reference
            // ISSUE: type reference
            // ISSUE: method reference
            // ISSUE: type reference
            // ISSUE: method reference
            // ISSUE: type reference
            this._queryWrapper(QueryType.Count, this._hintFactory, (Action) (() => queryCount = ((IQueryable<IContinuation<T>>) queryable).GroupBy(i => Expression.New(typeof (\u003C\u003Ef__AnonymousType5))).Select<IGrouping<\u003C\u003Ef__AnonymousType5, IContinuation<T>>, ODataQuery.CountCheckResult<T>>(Expression.Lambda<Func<IGrouping<\u003C\u003Ef__AnonymousType5, IContinuation<T>>, ODataQuery.CountCheckResult<T>>>((Expression) Expression.MemberInit(Expression.New(typeof (ODataQuery.CountCheckResult<T>)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ODataQuery.CountCheckResult<T>.set_Min), __typeref (ODataQuery.CountCheckResult<T>)), )))))); // Unable to render the statement
            if (queryCount != null)
            {
              int count1 = queryCount.Count;
              int? maxSize3 = this.QuerySizeOptions.MaxSize;
              int valueOrDefault = maxSize3.GetValueOrDefault();
              if (count1 > valueOrDefault & maxSize3.HasValue)
              {
                int count2 = queryCount.Count;
                maxSize3 = this.QuerySizeOptions.MaxSize;
                int limit = maxSize3.Value;
                throw new QueryExceedsPreferedMaxSizeException(count2, limit);
              }
            }
            HttpRequestMessageProperties messageProperties = this._request.ODataProperties();
            ODataQuery.CountCheckResult<T> countCheckResult = queryCount;
            long? nullable = new long?(countCheckResult != null ? (long) countCheckResult.Count : 0L);
            messageProperties.TotalCount = nullable;
            return queryCount;
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(12013014, AnalyticsService.Area, AnalyticsService.Layer, nameof (ValidateMaxSizePreferences));
      }
label_16:
      return (ODataQuery.CountCheckResult<T>) null;
    }

    private static ODataQuery.PagingStyle UseSkipTokenPaging(
      IQueryable queryable,
      ODataQueryOptions queryOptions)
    {
      if (queryOptions.ContainsRawComputeAndFilter())
        return ODataQuery.PagingStyle.Regular;
      Type type = ((IEnumerable<Type>) queryable.ElementType.GetInterfaces()).FirstOrDefault<Type>((Func<Type, bool>) (i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof (IContinuation<>)));
      if (type != (Type) null)
      {
        bool? nullable;
        if (queryOptions == null)
        {
          nullable = new bool?();
        }
        else
        {
          ApplyQueryOption apply = queryOptions.Apply;
          nullable = apply != null ? new bool?(apply.ApplyClause.IsAggregated()) : new bool?();
        }
        if (!nullable.GetValueOrDefault() && (queryOptions == null || queryOptions.RawValues.OrderBy == null))
          return !(type.GetGenericArguments()[0] == typeof (int)) ? ODataQuery.PagingStyle.LongSkipToken : ODataQuery.PagingStyle.IntSkipToken;
      }
      return ODataQuery.PagingStyle.Regular;
    }

    private IQueryable ApplyPaging<T>(
      IVssRequestContext requestContext,
      Type entitySetType,
      IQueryable queryable,
      ODataQueryContext queryContext,
      ODataQueryOptions queryOptions)
      where T : struct, IComparable<T>
    {
      requestContext.TraceEnter(12013015, AnalyticsService.Area, AnalyticsService.Layer, nameof (ApplyPaging));
      try
      {
        if (!this.QuerySizeOptions.PageSize.HasValue)
          return queryable;
        if (this.QuerySizeOptions.ExpectSinglePage && queryOptions.RawValues.Top == null && queryOptions.RawValues.Skip == null && queryOptions.RawValues.SkipToken == null || this._forceSnapshotSinglePage)
        {
          if (!this._forceSnapshotSinglePage || queryOptions.RawValues.Top == null)
            return ExpressionHelpers.Take(queryable, this.QuerySizeOptions.PageSize.Value, queryable.ElementType);
          ExpressionHelpers.PreSelectExpression preSelect;
          (preSelect, queryable) = ExpressionHelpers.ExtractPreSelectExpression<T>(queryable);
          queryable = ExpressionHelpers.Cast((IQueryable) (ExpressionHelpers.RemoveTopSkipSorting((IQueryable) (queryable as IQueryable<IContinuation<T>>)) as IQueryable<IContinuation<T>>), queryable.ElementType);
          queryable = ExpressionHelpers.Take(queryable, queryOptions.Top.Value, queryable.ElementType);
          if (preSelect == null)
            return queryable;
          MethodCallExpression methodCallExpression = Expression.Call((Expression) null, preSelect.Method, new Expression[2]
          {
            queryable.Expression,
            preSelect.Lambda
          });
          return queryable.Provider.CreateQuery((Expression) methodCallExpression);
        }
        ExpressionHelpers.PreSelectExpression next;
        (next, queryable) = ExpressionHelpers.ExtractPreSelectExpression<T>(queryable);
        if (!(queryable is IQueryable<IContinuation<T>> queryable1))
          throw new ArgumentException(AnalyticsResources.UNSUPPORTED_SKIP_TOKEN_PAGING((object) queryable.ElementType));
        T skipToken = default (T);
        if (queryOptions.RawValues.SkipToken != null)
          ODataQuery.TryParse<T>(queryOptions.RawValues.SkipToken, out skipToken);
        int? top = queryOptions.Top?.Value;
        int? skip = queryOptions.Skip?.Value;
        ODataQuery.CountCheckResult<T> range = this.ValidateMaxSizePreferences<T>(requestContext, queryable1, queryOptions);
        if (top.HasValue || skip.HasValue)
          queryable1 = (IQueryable<IContinuation<T>>) ExpressionHelpers.RemoveTopSkipSorting((IQueryable) queryable1);
        int? nullable;
        if (range != null)
        {
          int count = range.Count;
          nullable = this.QuerySizeOptions.PageSize;
          int num = nullable.Value;
          if (count <= num)
            goto label_28;
        }
        IQueryable<IContinuation<T>> queryable2 = queryable1.Where<IContinuation<T>>((Expression<Func<IContinuation<T>, bool>>) (i => i.SkipToken.CompareTo(skipToken) >= 0));
        if (this.EnablePageOptimization && !skipToken.Equals((object) default (T)) && entitySetType == typeof (TestResult))
        {
          ODataQuery.QueryOptimizerCache service = requestContext.GetService<ODataQuery.QueryOptimizerCache>();
          string key = string.Format("PageSizeMultiplier_{0}", (object) entitySetType.Name);
          int multiplier = 1;
          object obj1;
          if (service.TryGetValue(requestContext, key, out obj1))
          {
            nullable = obj1 as int?;
            multiplier = nullable ?? 1;
          }
          T pageLimit = this.AddPageLimit<T>(skipToken, multiplier);
          range = this.CalculateRange<T>(requestContext, queryable2.Where<IContinuation<T>>((Expression<Func<IContinuation<T>, bool>>) (i => i.SkipToken.CompareTo(pageLimit) <= 0)), top, skip);
          if (range != null)
          {
            int count = range.Count;
            nullable = this.QuerySizeOptions.PageSize;
            int num = nullable.Value;
            if (count == num)
              goto label_28;
          }
          range = this.CalculateRange<T>(requestContext, queryable2, top, skip);
          if (this.EnablePageOptimizationMultiplier)
          {
            int count = range.Count;
            nullable = this.QuerySizeOptions.PageSize;
            int num1 = nullable.Value;
            if (count == num1)
            {
              service.TryGetValue(requestContext, key, out obj1);
              nullable = obj1 as int?;
              object obj2 = (object) (nullable.GetValueOrDefault() + 5);
              nullable = obj2 as int?;
              int num2 = 50;
              if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
                obj2 = (object) 50;
              requestContext.Trace(12013040, TraceLevel.Info, "OData", nameof (ODataQuery), string.Format("New multiplier {0}", obj2));
              service.Set(requestContext, key, obj2);
            }
          }
        }
        else
          range = this.CalculateRange<T>(requestContext, queryable2, top, skip);
label_28:
        if (range != null)
        {
          queryable1 = queryable1.Where<IContinuation<T>>((Expression<Func<IContinuation<T>, bool>>) (i => i.SkipToken.CompareTo(range.Min) >= 0));
          int count1 = range.Count;
          nullable = top;
          int valueOrDefault = nullable.GetValueOrDefault();
          if (count1 == valueOrDefault & nullable.HasValue)
          {
            queryable1 = queryable1.Where<IContinuation<T>>((Expression<Func<IContinuation<T>, bool>>) (i => i.SkipToken.CompareTo(range.Max) <= 0));
          }
          else
          {
            int count2 = range.Count;
            nullable = this.QuerySizeOptions.PageSize;
            int num = nullable.Value;
            if (count2 >= num)
            {
              this._newSkipToken = new long?(Convert.ToInt64((object) range.Max) + 1L);
              queryable1 = queryable1.Where<IContinuation<T>>((Expression<Func<IContinuation<T>, bool>>) (i => i.SkipToken.CompareTo(range.Max) <= 0));
            }
          }
        }
        else
          this._emptyPageFound = true;
        this._skipTokenApplied = true;
        queryable = ExpressionHelpers.Cast((IQueryable) queryable1, queryable.ElementType);
        if (next == null)
          return queryable;
        Expression expression = queryable.Expression;
        for (; next != null; next = next.Next)
          expression = (Expression) Expression.Call((Expression) null, next.Method, new Expression[2]
          {
            expression,
            next.Lambda
          });
        return queryable.Provider.CreateQuery(expression);
      }
      finally
      {
        requestContext.TraceLeave(12013016, AnalyticsService.Area, AnalyticsService.Layer, nameof (ApplyPaging));
      }
    }

    private ODataQuery.CountCheckResult<T> CalculateRange<T>(
      IVssRequestContext requestContext,
      IQueryable<IContinuation<T>> rangeQuery,
      int? top,
      int? skip)
      where T : struct, IComparable<T>
    {
      requestContext.TraceLeave(12013034, AnalyticsService.Area, AnalyticsService.Layer, nameof (CalculateRange));
      try
      {
        ODataQuery.CountCheckResult<T> range = (ODataQuery.CountCheckResult<T>) null;
        ParameterExpression parameterExpression;
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: type reference
        // ISSUE: method reference
        // ISSUE: type reference
        this._queryWrapper(QueryType.Paging, this._hintFactory, (Action) (() => range = ODataQuery.RestoreTopSkip<T>(rangeQuery, top, skip).Take<IContinuation<T>>(this.QuerySizeOptions.PageSize.Value).GroupBy(i => Expression.New(typeof (\u003C\u003Ef__AnonymousType5))).Select<IGrouping<\u003C\u003Ef__AnonymousType5, IContinuation<T>>, ODataQuery.CountCheckResult<T>>(Expression.Lambda<Func<IGrouping<\u003C\u003Ef__AnonymousType5, IContinuation<T>>, ODataQuery.CountCheckResult<T>>>((Expression) Expression.MemberInit(Expression.New(typeof (ODataQuery.CountCheckResult<T>)), (MemberBinding) Expression.Bind((MethodInfo) MethodBase.GetMethodFromHandle(__methodref (ODataQuery.CountCheckResult<T>.set_Min), __typeref (ODataQuery.CountCheckResult<T>)), )))))); // Unable to render the statement
        return range;
      }
      finally
      {
        requestContext.TraceLeave(12013035, AnalyticsService.Area, AnalyticsService.Layer, nameof (CalculateRange));
      }
    }

    private static bool TryParse<T>(string s, out T res) where T : struct
    {
      res = default (T);
      long result;
      if (!long.TryParse(s, out result))
        return false;
      res = (T) Convert.ChangeType((object) result, typeof (T));
      return true;
    }

    private static IQueryable<IContinuation<T>> RestoreTopSkip<T>(
      IQueryable<IContinuation<T>> continuationQuery,
      int? top,
      int? skip)
      where T : struct
    {
      continuationQuery = (IQueryable<IContinuation<T>>) continuationQuery.OrderBy<IContinuation<T>, T>((Expression<Func<IContinuation<T>, T>>) (i => i.SkipToken));
      if (skip.HasValue)
        continuationQuery = continuationQuery.Skip<IContinuation<T>>(skip.Value);
      if (top.HasValue)
        continuationQuery = continuationQuery.Take<IContinuation<T>>(top.Value);
      return continuationQuery;
    }

    private IQueryable ApplyQuery(
      IVssRequestContext requestContext,
      IQueryable queryable,
      ODataQueryOptions queryOptions)
    {
      requestContext.TraceEnter(12013011, AnalyticsService.Area, AnalyticsService.Layer, nameof (ApplyQuery));
      try
      {
        return queryOptions.ApplyTo(queryable, this._querySettings);
      }
      catch (InvalidOperationException ex) when (ex.Message.Contains("Microsoft.OData.Edm.Date") && ex.Message.Contains("System.DateTimeOffset"))
      {
        requestContext.TraceLogAndThrowException(12016004, "OData", nameof (ODataQuery), (Exception) ex, (Exception) new ODataException(AnalyticsResources.DATE_FORMAT_ERROR()));
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12013012, AnalyticsService.Area, AnalyticsService.Layer, nameof (ApplyQuery));
      }
    }

    private static bool IsCountRequest(HttpRequestMessage request)
    {
      Microsoft.AspNet.OData.Routing.ODataPath path = request.ODataProperties().Path;
      return path != null && path.Segments.LastOrDefault<ODataPathSegment>() is CountSegment;
    }

    private void ValidateQuery(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<ODataQueryOptions>(this._queryOptions, "_queryOptions");
      foreach (KeyValuePair<string, string> queryNameValuePair in this._request.GetQueryNameValuePairs())
      {
        if (!this._queryOptions.IsSupportedQueryOption(queryNameValuePair.Key.ToLowerInvariant()) && queryNameValuePair.Key.StartsWith("$", StringComparison.Ordinal))
          throw new HttpResponseException(this._request.CreateErrorResponse(HttpStatusCode.BadRequest, AnalyticsResources.QUERY_PARAMETER_NOT_SUPPORTED((object) queryNameValuePair.Key)));
      }
      ODataValidationSettings requiredService = ServiceProviderServiceExtensions.GetRequiredService<ODataValidationSettings>(this._request.GetRequestContainer());
      try
      {
        this._queryOptions.Validate(requiredService);
      }
      catch (ODataErrorException ex) when (ex.Error?.ErrorCode == "10001")
      {
        string target1 = ex.Error.Target;
        string target2 = ex.Error.Details.First<ODataErrorDetail>().Target;
        if (requestContext.GetService<AnalyticsMetadataService>().IsCustomFieldInModelMarkedAsDeleted(requestContext, target1, target2, this._projectInfo))
        {
          requestContext.TraceAlways(12012012, TraceLevel.Info, "OData", nameof (ODataQuery), (string[]) null, AnalyticsResources.ODATA_PROPERTY_EXISTED_EARLIER((object) target1));
          throw new ODataException(AnalyticsResources.ODATA_PROPERTY_EXISTED_EARLIER((object) target1));
        }
        requestContext.TraceAlways(12012012, TraceLevel.Info, "OData", nameof (ODataQuery), (string[]) null, AnalyticsResources.ODATA_PROPERTY_NEVER_EXISTED((object) target1));
        throw new ODataException(AnalyticsResources.ODATA_PROPERTY_NEVER_EXISTED((object) target1));
      }
      catch (ODataErrorException ex) when (ex.Error?.ErrorCode == "10002")
      {
        string target = ex.Error.Target;
        requestContext.TraceAlways(12012012, TraceLevel.Info, "OData", nameof (ODataQuery), (string[]) null, AnalyticsResources.ODATA_PROPERTY_EXISTED_IN_AGGREGATE((object) target));
        throw new ODataException(AnalyticsResources.ODATA_PROPERTY_EXISTED_IN_AGGREGATE((object) target));
      }
      foreach (IODataValidator odataValidator in new List<IODataValidator>()
      {
        (IODataValidator) new ODataBuiltinWhitelistValidator(),
        (IODataValidator) new ODataQueryWarningsValidator(),
        (IODataValidator) new ODataQuerySnapshotValidator()
        {
          WarningCallback = (Action) (() => this._forceSnapshotSinglePage = true)
        },
        (IODataValidator) new ODataQueryCountDistinctValidator(),
        (IODataValidator) new ODataQueryParentDepthValidator(),
        (IODataValidator) new ODataQueryComputeUsageValidator(),
        (IODataValidator) new ODataWorkItemDescendantsExpandValidator(),
        (IODataValidator) new ODataWideSelectValidator()
      })
      {
        if (odataValidator.RequiredFeatureFlag == null || requestContext.IsFeatureEnabled(odataValidator.RequiredFeatureFlag))
          odataValidator.Validate(requestContext, this._request, this._queryOptions);
      }
    }

    private object GetEnumerator(IVssRequestContext requestContext, IQueryable queryable)
    {
      requestContext.TraceEnter(12013007, AnalyticsService.Area, AnalyticsService.Layer, nameof (GetEnumerator));
      try
      {
        Type odataEnumerableType = typeof (ODataEnumerable<>).MakeGenericType(queryable.ElementType);
        int? pageSize = this.QuerySizeOptions.PageSize;
        if (pageSize.HasValue && !this._skipTokenApplied)
        {
          if (this._queryOptions.Top != null)
          {
            int num = this._queryOptions.Top.Value;
            pageSize = this.QuerySizeOptions.PageSize;
            int valueOrDefault = pageSize.GetValueOrDefault();
            if (!(num > valueOrDefault & pageSize.HasValue))
              goto label_5;
          }
          IQueryable query = queryable;
          pageSize = this.QuerySizeOptions.PageSize;
          int count = pageSize.Value + 1;
          Type elementType = queryable.ElementType;
          queryable = ExpressionHelpers.Take(query, count, elementType);
          goto label_7;
        }
label_5:
        if (this._emptyPageFound)
          return (object) (Activator.CreateInstance(typeof (List<>).MakeGenericType(queryable.ElementType)) as IList);
label_7:
        requestContext.TraceLongTextConditionally(12013025, TraceLevel.Info, AnalyticsService.Area, AnalyticsService.Layer, (Func<string>) (() => queryable.ToString()));
        requestContext.TraceConditionally(12013026, TraceLevel.Info, AnalyticsService.Area, AnalyticsService.Layer, (Func<string>) (() => queryable.ElementType.ToString()));
        queryable = ExpressionHelpers.AsNoTracking(queryable);
        object result = (object) null;
        this._queryWrapper(QueryType.Raw, this._hintFactory, (Action) (() =>
        {
          if (this._newSkipToken.HasValue)
            result = Activator.CreateInstance(odataEnumerableType, (object) queryable, null, null, (object) this.GetSkipUri(this._newSkipToken));
          else
            result = Activator.CreateInstance(odataEnumerableType, (object) queryable, (object) this.QuerySizeOptions.PageSize, (object) this.GetSkipUri(new long?()), null);
        }));
        return result;
      }
      finally
      {
        requestContext.TraceLeave(12013008, AnalyticsService.Area, AnalyticsService.Layer, nameof (GetEnumerator));
      }
    }

    internal Uri GetSkipUri(long? skipToken)
    {
      FormDataCollection formDataCollection = new FormDataCollection(this._request.RequestUri);
      int num1 = this.QuerySizeOptions.PageSize.Value;
      Uri uri = new Uri(this._request.GetUrlHelper().CreateODataLink((IList<ODataPathSegment>) this._request.ODataProperties().Path.Segments));
      StringBuilder stringBuilder = new StringBuilder();
      int num2 = num1;
      foreach (KeyValuePair<string, string> keyValuePair in formDataCollection)
      {
        string key = keyValuePair.Key;
        string str1 = keyValuePair.Value;
        switch (key)
        {
          case "$top":
            int result1;
            if (int.TryParse(str1, out result1))
            {
              str1 = (result1 - num1).ToString((IFormatProvider) CultureInfo.InvariantCulture);
              break;
            }
            break;
          case "$skip":
            int result2;
            if (int.TryParse(str1, out result2))
            {
              num2 += result2;
              continue;
            }
            continue;
          case "$skiptoken":
            continue;
        }
        string str2 = key.Length <= 0 || key[0] != '$' ? Uri.EscapeDataString(key) : "$" + Uri.EscapeDataString(key.Substring(1));
        string str3 = Uri.EscapeDataString(str1);
        stringBuilder.Append(str2);
        stringBuilder.Append('=');
        stringBuilder.Append(str3);
        stringBuilder.Append('&');
      }
      if (!skipToken.HasValue)
        stringBuilder.AppendFormat("$skip={0}", (object) num2);
      else
        stringBuilder.AppendFormat(string.Format("$skiptoken={0}", (object) skipToken));
      return new UriBuilder(uri)
      {
        Query = stringBuilder.ToString()
      }.Uri;
    }

    public class CountCheckResult<T> where T : struct
    {
      public int Count { get; set; }

      public T Min { get; set; }

      public T Max { get; set; }
    }

    private enum PagingStyle
    {
      Regular,
      IntSkipToken,
      LongSkipToken,
    }

    internal class QueryOptimizerCache : VssMemoryCacheService<string, object>
    {
      public const string EntitySetMultiplierKey = "PageSizeMultiplier_{0}";

      protected override void ServiceStart(IVssRequestContext systemRequestContext) => base.ServiceStart(systemRequestContext);
    }
  }
}
