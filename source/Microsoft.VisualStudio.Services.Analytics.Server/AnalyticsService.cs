// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Query;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Common;
using Microsoft.VisualStudio.Services.Analytics.DataQuality;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OData;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using Microsoft.VisualStudio.Services.Analytics.OnPrem;
using Microsoft.VisualStudio.Services.Analytics.Transform;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsService : IAnalyticsService, IVssFrameworkService
  {
    private static readonly string s_layer = nameof (AnalyticsService);
    private const string c_modelNotReadyRegistryPath = "/Service/Analytics/State/ModelNotReady";
    private const string c_syncDatesMinPriorityRegistryPath = "/Service/Analytics/SyncDatesMinimumTransformPriority";
    private static readonly RegistryQuery s_modelNotReadyQuery = new RegistryQuery("/Service/Analytics/State/ModelNotReady");
    private static readonly RegistryQuery s_syncDatesMinPriorityQuery = new RegistryQuery("/Service/Analytics/SyncDatesMinimumTransformPriority");
    private static readonly RegistryQuery s_rollupMaxdop = new RegistryQuery("/Service/Analytics/Settings/OData/SqlHint/RollupMaxDop");
    private static readonly RegistryQuery s_burndownMaxdop = new RegistryQuery("/Service/Analytics/Settings/OData/SqlHint/BurndownMaxDop");
    private static readonly RegistryQuery s_mashupMaxdop = new RegistryQuery("/Service/Analytics/Settings/OData/SqlHint/MashupMaxDop");
    private static readonly RegistryQuery s_rollupForceOrder = new RegistryQuery("/Service/Analytics/Settings/OData/SqlHint/RollupForceOrder");
    private static readonly RegistryQuery s_enableParallelPlan = new RegistryQuery("/Service/Analytics/Settings/OData/SqlHint/EnableParallelPlan");

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public virtual IList<TResult> QueryTable<T, TResult>(
      IVssRequestContext requestContext,
      Func<IQueryable<T>, IQueryable<TResult>> func,
      ProjectInfo project = null)
      where T : class, IPartitionScoped
    {
      ArgumentUtility.CheckForNull<Func<IQueryable<T>, IQueryable<TResult>>>(func, nameof (func));
      using (AnalyticsComponent component = AnalyticsService.CreateComponent(requestContext, project))
      {
        IQueryable<T> table = component.GetTable<T>(project);
        return (IList<TResult>) func(table).ToList<TResult>();
      }
    }

    public virtual IList<T> QueryTable<T>(
      IVssRequestContext requestContext,
      Func<IQueryable<T>, IQueryable<T>> func = null,
      ProjectInfo project = null)
      where T : class, IPartitionScoped
    {
      if (func == null)
        func = (Func<IQueryable<T>, IQueryable<T>>) (t => t);
      return this.QueryTable<T, T>(requestContext, func, project);
    }

    public virtual TResult QueryTable<T, TResult>(
      IVssRequestContext requestContext,
      Func<IQueryable<T>, TResult> func,
      ProjectInfo project = null)
      where T : class, IPartitionScoped
    {
      ArgumentUtility.CheckForNull<Func<IQueryable<T>, TResult>>(func, nameof (func));
      using (AnalyticsComponent component = AnalyticsService.CreateComponent(requestContext, project))
        return func(component.GetTable<T>(project));
    }

    public virtual int QueryTable<T>(
      IVssRequestContext requestContext,
      Func<IQueryable<T>, int> func,
      ProjectInfo project = null)
      where T : class, IPartitionScoped
    {
      return this.QueryTable<T, int>(requestContext, func, project);
    }

    public virtual T QueryTable<T>(
      IVssRequestContext requestContext,
      Func<IQueryable<T>, T> func,
      ProjectInfo project = null)
      where T : class, IPartitionScoped
    {
      return this.QueryTable<T, T>(requestContext, func, project);
    }

    internal virtual IQueryable GetTable(
      AnalyticsComponent component,
      string entityTypeName,
      ProjectInfo project)
    {
      Type type;
      if (!component.TryGetEntityType(entityTypeName, out type))
        return (IQueryable) null;
      return (IQueryable) typeof (AnalyticsComponent).GetMethod(nameof (GetTable)).MakeGenericMethod(type).Invoke((object) component, new object[1]
      {
        (object) project
      });
    }

    public HttpResponseMessage QueryEntity(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      MediaTypeFormatterCollection formatters,
      IEdmEntityType entityType,
      ProjectInfo projectInfo)
    {
      if (projectInfo == null)
        throw new AnalyticsAccessCheckException(AnalyticsResources.QUERY_TOO_WIDE_EntityName((object) entityType), (Exception) null);
      using (AnalyticsComponent component = AnalyticsService.CreateComponent(requestContext, projectInfo))
      {
        IQueryable queryable = QueryableExtensions.ApplyProjectScopeFilterInternal(this.GetTable(component, entityType.Name, projectInfo) ?? throw new EntitySetNotFoundException(entityType.Name), projectInfo);
        Expression expression = queryable.Expression;
        foreach (ODataPathSegment odataPathSegment in request.ODataProperties().Path.Segments.Skip<ODataPathSegment>(1))
        {
          switch (odataPathSegment)
          {
            case KeySegment segment1:
              expression = expression.ApplySegment(segment1);
              continue;
            case NavigationPropertySegment segment2:
              expression = expression.ApplySegment(segment2);
              continue;
            default:
              throw new ArgumentException(string.Format("Not supported uri segment {0}", (object) odataPathSegment));
          }
        }
        object obj;
        if (typeof (IQueryable).IsAssignableFrom(expression.Type))
        {
          IQueryable query = queryable.Provider.CreateQuery(expression);
          obj = (object) ExpressionHelpers.ToList(query, query.ElementType);
        }
        else
          obj = queryable.Provider.Execute(expression);
        if (obj == null)
          throw new EntityNotFoundException(request.ODataProperties().Path.ToString());
        return new HttpResponseMessage()
        {
          Content = (HttpContent) new ObjectContent(obj.GetType(), obj, this.ChooseFormatter(obj.GetType(), request, formatters), request.GetMediaTypeHeader())
          {
            Value = obj
          }
        };
      }
    }

    public HttpResponseMessage QueryEntitySet(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      MediaTypeFormatterCollection formatters,
      IEdmEntityType entityType,
      ODataQueryOptions oDataQueryOptions,
      ODataQuerySizeOptions querySizeOptions,
      ProjectInfo projectInfo = null)
    {
      requestContext.TraceEnter(12012001, AnalyticsService.Area, AnalyticsService.Layer, nameof (QueryEntitySet));
      bool flag = requestContext.IsFeatureEnabled("Analytics.OData.DataQualityWarnings");
      IDataQualityService service1 = requestContext.GetService<IDataQualityService>();
      IAnalyticsService service2 = requestContext.GetService<IAnalyticsService>();
      IReadOnlyCollection<DataQualityResult> source1 = requestContext.IsFeatureEnabled("Analytics.OData.TraceDataQualityIssues") || requestContext.IsFeatureEnabled("Analytics.OData.DataQualityWarnings") ? service1.GetCachedLatestDataQualityResults(requestContext) : (IReadOnlyCollection<DataQualityResult>) new List<DataQualityResult>();
      bool getSyncDatesHeader = this.ExtractGetSyncDatesHeader(request);
      IEnumerable<ProviderSyncDataCondition> conditionsHeader;
      try
      {
        conditionsHeader = this.ExtractGetSyncDatesConditionsHeader(request);
      }
      catch (JsonSerializationException ex)
      {
        return request.CreateErrorResponse(HttpStatusCode.BadRequest, AnalyticsResources.UNABLE_TO_PARSE_HEADER((object) "X-TFS-If-SyncDates-Newer"));
      }
      IReadOnlyCollection<ProviderSyncData> providerSyncDatas;
      if (getSyncDatesHeader || conditionsHeader != null)
      {
        int minTransformPriority = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in AnalyticsService.s_syncDatesMinPriorityQuery, true, 7);
        providerSyncDatas = service2.GetSyncDates(requestContext, minTransformPriority);
        if (conditionsHeader != null && !AnalyticsService.IsSyncDateSatisfied(conditionsHeader, providerSyncDatas))
          return request.CreateResponse(HttpStatusCode.NotModified);
      }
      else
        providerSyncDatas = (IReadOnlyCollection<ProviderSyncData>) new List<ProviderSyncData>();
      ODataQuery response = (ODataQuery) null;
      HttpResponseMessage httpResponseMessage = (HttpResponseMessage) null;
      IEnumerable<DataQualityResult> dataQualityResults = (IEnumerable<DataQualityResult>) new HashSet<DataQualityResult>();
      try
      {
        response = new ODataQuery(requestContext, oDataQueryOptions, querySizeOptions, request, entityType, projectInfo, (Func<AnalyticsComponent>) (() => AnalyticsService.CreateComponent(requestContext, projectInfo)));
        response.EnablePageOptimization = requestContext.IsFeatureEnabled("Analytics.SqlOption.TestResultPageOptimization");
        response.PreComponent(requestContext);
        SqlOptions supportedSqlOptions = AnalyticsService.SupportedSqlOptions(requestContext);
        SqlHints supportedSqlHints = AnalyticsService.SupportedSqlHints(requestContext);
        using (AnalyticsComponent component = AnalyticsService.CreateComponent(requestContext, projectInfo))
        {
          IQueryable table = this.GetTable(component, entityType.Name, projectInfo);
          if (table != null)
          {
            response.Process(requestContext, table, component, (Action<QueryType, HintStrategyFactory, Action>) ((queryType, factory, query) =>
            {
              SqlOptions sqlOptions = component.SqlOptions;
              component.SqlOptions |= factory.ApplyOptions(queryType, supportedSqlOptions);
              component.SqlHints = supportedSqlHints;
              query();
              if (queryType == QueryType.Raw)
                return;
              component.SqlOptions = sqlOptions;
            }));
            IReadOnlySet<string> queriedModeltables = response.QueriedModelTables;
            requestContext.TraceAlways(12012018, TraceLevel.Info, AnalyticsService.Area, AnalyticsService.Layer, JsonConvert.SerializeObject((object) queriedModeltables).ToString());
            dataQualityResults = source1.Where<DataQualityResult>((Func<DataQualityResult, bool>) (x => x.Failed && queriedModeltables.Contains(x.TargetTable)));
            IEnumerable<ProviderSyncData> source2 = providerSyncDatas.Where<ProviderSyncData>((Func<ProviderSyncData, bool>) (x => queriedModeltables.Contains(x.ModelTableName)));
            if (flag)
            {
              IReadOnlyCollection<string> dataQualityWarnings = service1.GetDataQualityWarnings(requestContext, dataQualityResults.Where<DataQualityResult>((Func<DataQualityResult, bool>) (x => x.Failed)));
              request.ODataWarnings().AddRange((IEnumerable<string>) dataQualityWarnings);
            }
            if (source2.Count<ProviderSyncData>() > 0)
              request.ODataWarnings().Add("ProviderSyncDates:" + JsonConvert.SerializeObject((object) source2));
            httpResponseMessage = this.PrepareResponseMessage(requestContext, request, formatters, response);
          }
        }
        if (!this.IsModelReady(requestContext) && dataQualityResults.Any<DataQualityResult>((Func<DataQualityResult, bool>) (x => x.Name == "ModelReady")))
        {
          service1.ExpireCachedLatestDataQualityResults(requestContext);
          requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            Constants.AnalyticsDataQualityJobId
          }, 30);
          throw new ModelNotReadyException(AnalyticsResources.MICROSERVICE_MODEL_READY());
        }
        ODataDataQualityTelemetryHandler.Handle(requestContext, dataQualityResults);
        return httpResponseMessage;
      }
      finally
      {
        ODataQueryFinishedEventArgs args = new ODataQueryFinishedEventArgs()
        {
          RequestUri = request.RequestUri,
          ResponseCount = response != null ? response.ResponseRecordsCount : 0,
          ResponseLength = (long?) httpResponseMessage?.Content?.Headers?.ContentLength
        };
        ODataEndOfRequestTelemetryHandler.ProcessEvent(requestContext, args);
        requestContext.TraceLeave(12012002, AnalyticsService.Area, AnalyticsService.Layer, nameof (QueryEntitySet));
      }
    }

    public IReadOnlyCollection<ProviderSyncData> GetSyncDates(
      IVssRequestContext requestContext,
      int minTransformPriority,
      IEnumerable<string> modelTableNames = null)
    {
      if (modelTableNames == null)
        modelTableNames = ((IEnumerable<TransformDefinition>) TransformDefinitions.All).Select<TransformDefinition, string>((Func<TransformDefinition, string>) (d => d.TargetTable)).Distinct<string>();
      using (AnalyticsComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<AnalyticsComponent>("Analytics"))
        return replicaAwareComponent.GetSyncDates(minTransformPriority, modelTableNames);
    }

    public static bool IsSyncDateSatisfied(
      IEnumerable<ProviderSyncDataCondition> syncDataConditions,
      IReadOnlyCollection<ProviderSyncData> latestSyncDates)
    {
      if (syncDataConditions == null || syncDataConditions.Count<ProviderSyncDataCondition>() == 0)
        return true;
      if (latestSyncDates == null || latestSyncDates.Count<ProviderSyncData>() == 0)
        return false;
      Dictionary<string, DateTimeOffset> dictionary1 = new Dictionary<string, DateTimeOffset>();
      foreach (ProviderSyncData latestSyncDate in (IEnumerable<ProviderSyncData>) latestSyncDates)
      {
        DateTime? providerSyncDate = latestSyncDate.ProviderSyncDate;
        if (providerSyncDate.HasValue)
        {
          Dictionary<string, DateTimeOffset> dictionary2 = dictionary1;
          string providerTableName = latestSyncDate.ProviderTableName;
          providerSyncDate = latestSyncDate.ProviderSyncDate;
          DateTimeOffset addValue = (DateTimeOffset) providerSyncDate.Value;
          dictionary2.AddOrUpdate<string, DateTimeOffset>(providerTableName, addValue, (Func<DateTimeOffset, DateTimeOffset, DateTimeOffset>) ((oldValue, newValue) => !(oldValue > newValue) ? oldValue : newValue));
        }
      }
      foreach (ProviderSyncDataCondition syncDataCondition in syncDataConditions)
      {
        DateTimeOffset dateTimeOffset;
        if (!dictionary1.TryGetValue(syncDataCondition.ProviderTableName, out dateTimeOffset) || dateTimeOffset < syncDataCondition.ExpectedSyncDate)
          return false;
      }
      return true;
    }

    private bool ExtractGetSyncDatesHeader(HttpRequestMessage request)
    {
      IEnumerable<string> values;
      if (!request.Headers.TryGetValues("X-TFS-GetSyncDates", out values))
        request.Headers.TryGetValues("GetSyncDates", out values);
      return values != null && values.First<string>().ToLower().Equals("true");
    }

    private IEnumerable<ProviderSyncDataCondition> ExtractGetSyncDatesConditionsHeader(
      HttpRequestMessage request)
    {
      IEnumerable<string> values;
      return request.Headers.TryGetValues("X-TFS-If-SyncDates-Newer", out values) ? JsonConvert.DeserializeObject<IEnumerable<ProviderSyncDataCondition>>(values.First<string>()) : (IEnumerable<ProviderSyncDataCondition>) null;
    }

    private HttpResponseMessage PrepareResponseMessage(
      IVssRequestContext context,
      HttpRequestMessage request,
      MediaTypeFormatterCollection formatters,
      ODataQuery response)
    {
      context.TraceEnter(12013001, AnalyticsService.Area, AnalyticsService.Layer, nameof (PrepareResponseMessage));
      try
      {
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage()
        {
          Content = (HttpContent) new ObjectContent(response.PayloadType, response.Baseline, this.ChooseFormatter(response.PayloadType, request, formatters), request.GetMediaTypeHeader())
          {
            Value = response.Response
          }
        };
        if (response.Response is IODataEnumerable)
        {
          try
          {
            httpResponseMessage.Content.LoadIntoBufferAsync().Wait();
          }
          catch (AggregateException ex)
          {
            ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
          }
        }
        return httpResponseMessage;
      }
      finally
      {
        context.TraceLeave(12013002, AnalyticsService.Area, AnalyticsService.Layer, nameof (PrepareResponseMessage));
      }
    }

    private MediaTypeFormatter ChooseFormatter(
      Type type,
      HttpRequestMessage request,
      MediaTypeFormatterCollection formatters)
    {
      return formatters.OfType<ODataMediaTypeFormatter>().Select<ODataMediaTypeFormatter, MediaTypeFormatter>((Func<ODataMediaTypeFormatter, MediaTypeFormatter>) (f => f.GetPerRequestFormatterInstance(type, request, (MediaTypeHeaderValue) null))).First<MediaTypeFormatter>((Func<MediaTypeFormatter, bool>) (c => c.CanWriteType(type)));
    }

    private static SqlHints SupportedSqlHints(IVssRequestContext requestContext)
    {
      SqlHints sqlHints = new SqlHints();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      sqlHints.RollupMaxdop = service.GetValue<string>(requestContext, in AnalyticsService.s_rollupMaxdop, true, "");
      sqlHints.BurndownMaxdop = service.GetValue<string>(requestContext, in AnalyticsService.s_burndownMaxdop, true, "");
      sqlHints.MashupMaxdop = service.GetValue<string>(requestContext, in AnalyticsService.s_mashupMaxdop, true, "");
      sqlHints.RollupForceOrder = service.GetValue<string>(requestContext, in AnalyticsService.s_rollupForceOrder, true, "");
      sqlHints.EnableParallelPlan = service.GetValue<string>(requestContext, in AnalyticsService.s_enableParallelPlan, true, "");
      return sqlHints;
    }

    private static SqlOptions SupportedSqlOptions(IVssRequestContext requestContext)
    {
      int num = requestContext.IsFeatureEnabled("Analytics.SqlOption.ForcePartitionFilter") ? 1 : 0;
      bool flag1 = requestContext.IsFeatureEnabled("Analytics.SqlOption.TestResultJoinPredicateOptimization");
      bool flag2 = requestContext.IsFeatureEnabled("Analytics.SqlOption.TestResultRecompile");
      bool flag3 = requestContext.IsFeatureEnabled("Analytics.SqlOption.AssumeJoinPredicateDependsOnFilter");
      bool flag4 = requestContext.IsFeatureEnabled("Analytics.SqlOption.HashJoinOnFilter");
      bool flag5 = requestContext.IsFeatureEnabled("Analytics.SqlOption.HashJoinForBurnDown");
      bool flag6 = requestContext.IsFeatureEnabled("Analytics.SqlOption.NoHintViewsForRollup");
      bool flag7 = requestContext.IsFeatureEnabled("Analytics.SqlOption.LoopJoinForRollup");
      SqlOptions sqlOptions = ~SqlOptions.None;
      if (num == 0)
        sqlOptions &= ~SqlOptions.ForcePartitionFilter;
      if (!flag1)
        sqlOptions &= ~SqlOptions.TestResultJoinOptimization;
      if (!flag2)
        sqlOptions &= ~SqlOptions.TestResultRecompile;
      if (!flag3)
        sqlOptions &= ~SqlOptions.AssumeJoinPredicateDependsOnFilter;
      if (!flag4)
        sqlOptions &= ~SqlOptions.HashJoinFilterHint;
      if (!flag5)
        sqlOptions &= ~SqlOptions.HashJoinForBurnDownHint;
      if (!flag6)
        sqlOptions &= ~SqlOptions.NoHintViewForRollup;
      if (!flag7)
        sqlOptions &= ~SqlOptions.LoopJoinForRollupHint;
      return sqlOptions;
    }

    private static AnalyticsComponent CreateComponent(
      IVssRequestContext requestContext,
      ProjectInfo project = null)
    {
      int num = requestContext.IsFeatureEnabled("Analytics.SqlOption.UnknownPartitionId") ? 1 : 0;
      bool flag1 = requestContext.IsFeatureEnabled("Analytics.SqlOption.DisableAdaptiveJoin");
      bool flag2 = requestContext.IsFeatureEnabled("Analytics.SqlOption.AssumeJoinPredicateDependsOnFilter");
      MetadataModel model = requestContext.GetService<AnalyticsMetadataService>().GetModel(requestContext, project);
      AnalyticsComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<AnalyticsComponent>("Analytics");
      replicaAwareComponent.ContextType = model.Type;
      replicaAwareComponent.CompiledModel = model.EFModel;
      replicaAwareComponent.Level = model.Level;
      replicaAwareComponent.EdmModel = model.EdmModel;
      if (num != 0)
        replicaAwareComponent.SqlOptions |= SqlOptions.UnknownPartitionId;
      if (flag1)
        replicaAwareComponent.SqlOptions |= SqlOptions.DisableAdaptiveJoin;
      if (flag2)
        replicaAwareComponent.SqlOptions |= SqlOptions.AssumeJoinPredicateDependsOnFilter;
      return replicaAwareComponent;
    }

    public virtual bool IsModelReady(IVssRequestContext requestContext) => !requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in AnalyticsService.s_modelNotReadyQuery, false);

    public void CheckModelReady(IVssRequestContext requestContext)
    {
      if (!this.IsModelReady(requestContext))
        throw new ModelNotReadyException(AnalyticsResources.MODEL_NOT_READY());
    }

    public virtual void SetModelReady(IVssRequestContext requestContext, bool ready)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (ready)
        service.DeleteEntries(requestContext, "/Service/Analytics/State/ModelNotReady");
      else
        service.SetValue<bool>(requestContext, "/Service/Analytics/State/ModelNotReady", true);
    }

    public IReadOnlyCollection<SpaceRequirements> GetSpaceRequirements(
      IVssRequestContext requestContext)
    {
      using (AnalyticsComponent component = requestContext.CreateComponent<AnalyticsComponent>())
        return component.GetSpaceEstimate();
    }

    public static string Area => "AnalyticsModel";

    public static string Layer => AnalyticsService.s_layer;
  }
}
