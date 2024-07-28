// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimizationCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryOptimizationCacheService : 
    VssMemoryCacheService<QueryOptimizationInstance, QueryOptimizationInstance>,
    IQueryOptimizationCacheService,
    IVssFrameworkService
  {
    private HashSet<QueryOptimizationState> m_InvalidStateInCache = new HashSet<QueryOptimizationState>()
    {
      QueryOptimizationState.None,
      QueryOptimizationState.NotOptimizable
    };
    private ILockName m_lock;
    private IVssMemoryCacheGrouping<QueryOptimizationInstance, QueryOptimizationInstance, QueryOptimizationState> m_optimizationGrouping;
    private IVssMemoryCacheGrouping<QueryOptimizationInstance, QueryOptimizationInstance, QueryOptimizationInstance> m_QueryIdGrouping;
    private IVssMemoryCacheGrouping<QueryOptimizationInstance, QueryOptimizationInstance, QueryOptimizationInstance> m_QueryHashGrouping;
    private ConcurrentDictionary<QueryOptimizationInstance, QueryOptimizationInstance> m_forkedInstanceHandleSet;
    private Capture<int> m_initialCacheLoadInDays = Capture.Create<int>(8);
    private const string c_cacheRregistryPath = "/Service/WorkItemTracking/Settings/QueryOptimizationCache";
    private const string c_trialLimitRegistryKeySuffix = "/TrialLimit";
    private const string c_initialCacheLoadInDaysKeySuffix = "/InitialCacheLoadInDays";
    private const string c_slowRunTimeMinThresholdInMsKeySuffix = "/SlowRunTimeMinThresholdInMs";
    private const string c_desiredOptimizationRatioKeySuffix = "/DesiredOptimizationRatio";
    private const string c_optimizedNormalCriteriaSlackRatioSuffix = "/OptimizedNormalCriteriaSlackRatio";
    private const string c_toleratedSlownessInMsKeySuffix = "/ToleratedSlownessInMs";
    private const string c_maxDaysInNotOptimizable = "/MaxDaysInNotOptimizable";
    private const int c_defaultCacheCount = 20000;
    private const int c_defaultInitialCacheLoadInDays = 8;

    public DateTime MostRecentUpdatedFromDBTime { get; private set; } = DateTime.MinValue;

    public bool IsCacheInitializationCompleted { get; private set; }

    protected QueryOptimizationCacheService()
      : base((IEqualityComparer<QueryOptimizationInstance>) QueryOptimizationInstanceComparer.Instance, MemoryCacheConfiguration<QueryOptimizationInstance, QueryOptimizationInstance>.Default.Clone().WithMaxElements(20000))
    {
    }

    protected override void ServiceStart(IVssRequestContext requestContext) => requestContext.TraceBlock(914600, 914601, 914602, "Query", nameof (QueryOptimizationCacheService), nameof (ServiceStart), (Action) (() =>
    {
      this.m_lock = this.CreateLockName(requestContext, "write_lock");
      this.GetStrategySettingsFromRegistryAndApply(requestContext);
      this.RegisterNotification(requestContext);
      this.m_optimizationGrouping = VssMemoryCacheGroupingFactory.Create<QueryOptimizationInstance, QueryOptimizationInstance, QueryOptimizationState>(requestContext, this.MemoryCache, (Func<QueryOptimizationInstance, QueryOptimizationInstance, IEnumerable<QueryOptimizationState>>) ((key, value) => (IEnumerable<QueryOptimizationState>) new QueryOptimizationState[1]
      {
        value.OptimizationState
      }));
      this.m_QueryHashGrouping = VssMemoryCacheGroupingFactory.Create<QueryOptimizationInstance, QueryOptimizationInstance, QueryOptimizationInstance>(requestContext, this.MemoryCache, (Func<QueryOptimizationInstance, QueryOptimizationInstance, IEnumerable<QueryOptimizationInstance>>) ((key, value) => (IEnumerable<QueryOptimizationInstance>) new QueryOptimizationInstance[1]
      {
        value
      }), (IEqualityComparer<QueryOptimizationInstance>) QueryOptimizationInstanceHashComparer.Instance);
      this.m_QueryIdGrouping = VssMemoryCacheGroupingFactory.Create<QueryOptimizationInstance, QueryOptimizationInstance, QueryOptimizationInstance>(requestContext, this.MemoryCache, (Func<QueryOptimizationInstance, QueryOptimizationInstance, IEnumerable<QueryOptimizationInstance>>) ((key, value) => (IEnumerable<QueryOptimizationInstance>) new QueryOptimizationInstance[1]
      {
        value
      }), (IEqualityComparer<QueryOptimizationInstance>) QueryOptimizationInstanceIdComparer.Instance);
      this.m_forkedInstanceHandleSet = new ConcurrentDictionary<QueryOptimizationInstance, QueryOptimizationInstance>((IEqualityComparer<QueryOptimizationInstance>) QueryOptimizationInstanceComparer.Instance);
      this.InitializeCache(requestContext);
    }));

    protected virtual void InitializeCache(IVssRequestContext requestContext)
    {
      WorkItemTrackingQueryExecutionLoggingConfiguration config = requestContext.WitContext().ServerSettings.QueryExecutionLoggingConfig;
      bool filterDuplicateQueryExecutionHistories = WorkItemTrackingFeatureFlags.IsFilterDuplicateQueryExecutionHistoriesEnabled(requestContext);
      requestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(requestContext.Elevate(), new TeamFoundationTask((TeamFoundationTaskCallback) ((context, args) =>
      {
        QueryExecutionInfoReturnedPayload infoReturnedPayload = (QueryExecutionInfoReturnedPayload) null;
        using (QuerySqlComponent component = context.CreateComponent<QuerySqlComponent>())
          infoReturnedPayload = component.LoadQueryOptimizationEntriesForCacheInitialization(this.MaxCacheLength.Value, DateTime.UtcNow.AddDays((double) -this.m_initialCacheLoadInDays.Value));
        IEnumerable<QueryOptimizationInstance> optimizationInstances = infoReturnedPayload != null ? infoReturnedPayload.QueryOptimizationInstances.Where<QueryOptimizationInstance>((Func<QueryOptimizationInstance, bool>) (instance => instance != null)) : (IEnumerable<QueryOptimizationInstance>) null;
        int percentileForThreshold = config.PercentileForThreshold;
        int recordCountForOpt = config.MinRecordCountForOpt;
        QueryOptimizationInstancesHelper.GetSlownessThresholdFromHistories(optimizationInstances, infoReturnedPayload?.QueryHistories, recordCountForOpt, percentileForThreshold, filterDuplicateQueryExecutionHistories);
        this.AddOrUpdateFromDB(context, optimizationInstances);
        this.IsCacheInitializationCompleted = true;
      }), (object) null, 0));
    }

    protected override void ServiceEnd(IVssRequestContext requestContext) => requestContext.TraceBlock(914603, 914604, "Query", nameof (QueryOptimizationCacheService), nameof (ServiceEnd), (Action) (() => this.UnregisterNotification(requestContext)));

    protected virtual void RegisterNotification(IVssRequestContext requestContext) => requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, "/Service/WorkItemTracking/Settings/QueryOptimizationCache/*");

    protected virtual void UnregisterNotification(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.GetStrategySettingsFromRegistryAndApply(requestContext);
    }

    protected virtual void GetStrategySettingsFromRegistryAndApply(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/QueryOptimizationCache/*");
      QueryOptimizationStrategy.SlowRunTimeMinThresholdInMs.Value = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryOptimizationCache/SlowRunTimeMinThresholdInMs", 3000);
      QueryOptimizationStrategy.DesiredOptimizationRatio.Value = registryEntryCollection.GetValueFromPath<float>("/Service/WorkItemTracking/Settings/QueryOptimizationCache/DesiredOptimizationRatio", 0.7f);
      QueryOptimizationStrategy.OptimizedNormalCriteriaSlackRatio.Value = registryEntryCollection.GetValueFromPath<float>("/Service/WorkItemTracking/Settings/QueryOptimizationCache/OptimizedNormalCriteriaSlackRatio", 0.85f);
      QueryOptimizationStrategy.ToleratedSlownessInMs.Value = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryOptimizationCache/ToleratedSlownessInMs", 5000);
      QueryOptimizationStrategy.TrialLimit.Value = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryOptimizationCache/TrialLimit", 5);
      QueryOptimizationStrategy.MaxDaysInNotOptimizable.Value = registryEntryCollection.GetValueFromPath<int>("/MaxDaysInNotOptimizable/TrialLimit", 7);
      this.m_initialCacheLoadInDays.Value = registryEntryCollection.GetValueFromPath<int>("/Service/WorkItemTracking/Settings/QueryOptimizationCache/InitialCacheLoadInDays", 8);
    }

    public QueryOptimizationInstance GetQueryOptimizationInstance(
      IVssRequestContext requestContext,
      Guid? queryId,
      string queryHash,
      QueryOptimizationStrategy strategyToMatchForFuzzMatchOnId,
      bool exactMatch = false)
    {
      ArgumentUtility.CheckForNull<string>(queryHash, nameof (queryHash));
      return requestContext.TraceBlock<QueryOptimizationInstance>(914501, 914502, "Query", nameof (QueryOptimizationCacheService), nameof (GetQueryOptimizationInstance), (Func<QueryOptimizationInstance>) (() =>
      {
        QueryOptimizationInstance optimizationInstance1 = new QueryOptimizationInstance(queryId, queryHash, strategyToMatchForFuzzMatchOnId);
        bool flag = WorkItemTrackingFeatureFlags.IsQueryAutoOptimizationFuzzMatchEnabled(requestContext);
        QueryOptimizationInstance optimizationInstance2 = optimizationInstance1;
        DateTime? nullable = new DateTime?(DateTime.UtcNow);
        short? currentIndex = new short?();
        short? normalRunCount = new short?();
        short? slowRunCount = new short?();
        DateTime? lastRunTime = nullable;
        DateTime? lastStateChangeTime = new DateTime?();
        optimizationInstance2.SetOptimizationState(QueryOptimizationState.None, currentIndex, normalRunCount, slowRunCount, lastRunTime, lastStateChangeTime);
        if ((long) this.m_forkedInstanceHandleSet.Count >= (long) (Capture<long>) this.MaxCacheSize || !flag)
          exactMatch = true;
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        string str = string.Empty;
        int num = -1;
        QueryOptimizationInstance optimizationInstance3;
        if (this.TryGetValue(requestContext, optimizationInstance1, out optimizationInstance3))
        {
          QueryOptimizationInstance optimizationInstance4;
          if (flag && this.m_forkedInstanceHandleSet.TryGetValue(optimizationInstance1, out optimizationInstance4) && optimizationInstance4.LastRunTime < DateTime.UtcNow.AddDays(-2.0))
            this.m_forkedInstanceHandleSet.TryRemove(optimizationInstance1, out QueryOptimizationInstance _);
          str = "ExactMatch";
          num = (int) optimizationInstance3.OptimizationState;
        }
        else
        {
          IEnumerable<QueryOptimizationInstance> keys1;
          if (!exactMatch && !this.m_forkedInstanceHandleSet.ContainsKey(optimizationInstance1) && this.m_QueryHashGrouping.TryGetKeys(optimizationInstance1, out keys1) && keys1.Any<QueryOptimizationInstance>())
          {
            this.m_forkedInstanceHandleSet.TryAdd(optimizationInstance1, optimizationInstance1);
            optimizationInstance3 = this.ForkQueryOptimizationInstanceWithBestFit(requestContext, keys1, queryId, queryHash);
            str = "MatchByHash";
            num = (int) optimizationInstance3.OptimizationState;
          }
          else
          {
            IEnumerable<QueryOptimizationInstance> keys2;
            if (!exactMatch && !this.m_forkedInstanceHandleSet.ContainsKey(optimizationInstance1) && this.m_QueryIdGrouping.TryGetKeys(optimizationInstance1, out keys2) && keys2.Any<QueryOptimizationInstance>() && optimizationInstance1.OptimizationStrategy != null && keys2.First<QueryOptimizationInstance>().OptimizationStrategy.Equals(optimizationInstance1.OptimizationStrategy))
            {
              this.m_forkedInstanceHandleSet.TryAdd(optimizationInstance1, optimizationInstance1);
              optimizationInstance3 = this.ForkQueryOptimizationInstanceWithBestFit(requestContext, keys2, queryId, queryHash);
              str = "MatchById";
              num = (int) optimizationInstance3.OptimizationState;
            }
          }
        }
        intelligenceData.Add("MatchingApproach", str);
        intelligenceData.Add("StateReturn", (double) num);
        if (optimizationInstance3 != null)
          intelligenceData.Add("InstanceReturned", optimizationInstance3.Serialize<QueryOptimizationInstance>());
        IVssRequestContext requestContext1 = requestContext;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, nameof (QueryOptimizationCacheService), nameof (GetQueryOptimizationInstance), properties);
        return optimizationInstance3?.Clone();
      }));
    }

    private QueryOptimizationInstance ForkQueryOptimizationInstanceWithBestFit(
      IVssRequestContext requestContext,
      IEnumerable<QueryOptimizationInstance> cacheKeys,
      Guid? queryId,
      string queryHash)
    {
      QueryOptimizationInstance key = cacheKeys.FirstOrDefault<QueryOptimizationInstance>((Func<QueryOptimizationInstance, bool>) (f => f.OptimizationState == QueryOptimizationState.Optimized)) ?? cacheKeys.FirstOrDefault<QueryOptimizationInstance>((Func<QueryOptimizationInstance, bool>) (f => f.OptimizationState == QueryOptimizationState.InOptimization)) ?? cacheKeys.FirstOrDefault<QueryOptimizationInstance>((Func<QueryOptimizationInstance, bool>) (f => f.OptimizationState == QueryOptimizationState.InEvaluation)) ?? cacheKeys.FirstOrDefault<QueryOptimizationInstance>();
      QueryOptimizationInstance source;
      this.TryGetValue(requestContext, key, out source);
      QueryOptimizationInstance optimizationInstance = new QueryOptimizationInstance(queryId, queryHash, source.OptimizationStrategy);
      optimizationInstance.GetStateClonedFrom(source);
      return optimizationInstance;
    }

    public void AddOrUpdate(IVssRequestContext requestContext, QueryOptimizationInstance instance) => this.AddOrUpdateInternal(requestContext, (IEnumerable<QueryOptimizationInstance>) new QueryOptimizationInstance[1]
    {
      instance
    });

    public void AddOrUpdateFromDB(
      IVssRequestContext requestContext,
      IEnumerable<QueryOptimizationInstance> queryOptimizationInstances)
    {
      this.MostRecentUpdatedFromDBTime = DateTime.UtcNow;
      this.AddOrUpdateInternal(requestContext, queryOptimizationInstances, true);
    }

    public void AddOrUpdateInternal(
      IVssRequestContext requestContext,
      IEnumerable<QueryOptimizationInstance> queryOptimizationInstances,
      bool resetDeltaRunCounts = false)
    {
      requestContext.TraceBlock(914504, 914505, 914506, "Query", nameof (QueryOptimizationCacheService), "AddOrUpdate", (Action) (() =>
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        using (requestContext.AcquireWriterLock(this.m_lock))
        {
          queryOptimizationInstances.Where<QueryOptimizationInstance>((Func<QueryOptimizationInstance, bool>) (instance => this.m_InvalidStateInCache.Contains(instance.OptimizationState))).ForEach<QueryOptimizationInstance>((Action<QueryOptimizationInstance>) (candidate => this.Remove(requestContext, candidate)));
          List<QueryOptimizationInstance> list = queryOptimizationInstances.Where<QueryOptimizationInstance>((Func<QueryOptimizationInstance, bool>) (instance => !this.m_InvalidStateInCache.Contains(instance.OptimizationState))).ToList<QueryOptimizationInstance>();
          int roomNeeded = 0;
          foreach (QueryOptimizationInstance key in list)
          {
            if (!this.TryPeekValue(requestContext, key, out QueryOptimizationInstance _))
              ++roomNeeded;
          }
          this.EnsureSpace(requestContext, roomNeeded);
          List<(QueryOptimizationInstance, QueryOptimizationInstance, bool)> valueTupleList = new List<(QueryOptimizationInstance, QueryOptimizationInstance, bool)>();
          foreach (QueryOptimizationInstance optimizationInstance in list)
          {
            QueryOptimizationInstance existingInstance;
            this.TryPeekValue(requestContext, optimizationInstance, out existingInstance);
            bool flag = false;
            if (existingInstance == null || QueryOptimizationInstance.IsOldInstanceReplaceable(existingInstance, optimizationInstance))
            {
              if (resetDeltaRunCounts)
                optimizationInstance.ResetDeltaRunCounts();
              flag = true;
              this.Set(requestContext, optimizationInstance, optimizationInstance);
            }
            else if (existingInstance != null & resetDeltaRunCounts)
            {
              flag = false;
              existingInstance.ResetDeltaRunCounts();
            }
            valueTupleList.Add((optimizationInstance, existingInstance, flag));
          }
          stopwatch.Stop();
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("ElapsedTime", (double) stopwatch.ElapsedMilliseconds);
          properties.Add("AddOrUpdateDetail", valueTupleList.Serialize<List<(QueryOptimizationInstance, QueryOptimizationInstance, bool)>>());
          properties.Add("AddOrUpdateCount", (double) queryOptimizationInstances.Count<QueryOptimizationInstance>());
          properties.Add("CacheSize", (double) this.MemoryCache.Count);
          properties.Add("ForkedInstanceHandleSetSize", (double) this.m_forkedInstanceHandleSet.Count);
          service.Publish(requestContext, nameof (QueryOptimizationCacheService), "AddOrUpdate", properties);
        }
      }));
    }

    private bool EnsureSpace(IVssRequestContext requestContext, int roomNeeded = 1)
    {
      if (this.MemoryCache.Count <= this.MaxCacheLength.Value - roomNeeded)
        return true;
      Stopwatch stopwatch = Stopwatch.StartNew();
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int roomNeeded1 = roomNeeded;
      IEnumerable<QueryOptimizationInstance> keys1;
      if (this.m_optimizationGrouping.TryGetKeys(QueryOptimizationState.InEvaluation, out keys1))
      {
        num2 = this.Evict(requestContext, keys1, roomNeeded1);
        roomNeeded1 -= num2;
      }
      IEnumerable<QueryOptimizationInstance> keys2;
      if (roomNeeded1 > 0 && this.m_optimizationGrouping.TryGetKeys(QueryOptimizationState.InOptimization, out keys2))
      {
        num1 = this.Evict(requestContext, keys2, roomNeeded1);
        roomNeeded1 -= num1;
      }
      IEnumerable<QueryOptimizationInstance> keys3;
      if (roomNeeded1 > 0 && this.m_optimizationGrouping.TryGetKeys(QueryOptimizationState.Optimized, out keys3))
      {
        num3 = this.Evict(requestContext, keys3, roomNeeded1);
        roomNeeded1 -= num3;
      }
      stopwatch.Stop();
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("ElapsedTime", (double) stopwatch.ElapsedMilliseconds);
      intelligenceData.Add("EvictedOptimizedCount", (double) num3);
      intelligenceData.Add("EvictedInEvaluationCount", (double) num2);
      intelligenceData.Add("EvictedInOptimizationCount", (double) num1);
      intelligenceData.Add("RoomNeeded", (double) roomNeeded);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, nameof (QueryOptimizationCacheService), nameof (EnsureSpace), properties);
      return roomNeeded1 <= 0;
    }

    private int Evict(
      IVssRequestContext requestContext,
      IEnumerable<QueryOptimizationInstance> instances,
      int roomNeeded)
    {
      List<QueryOptimizationInstance> list = instances.OrderBy<QueryOptimizationInstance, DateTime>((Func<QueryOptimizationInstance, DateTime>) (instance => instance.LastRunTime)).Take<QueryOptimizationInstance>(roomNeeded).ToList<QueryOptimizationInstance>();
      int count = list.Count;
      foreach (QueryOptimizationInstance key in list)
        this.Remove(requestContext, key);
      return count;
    }
  }
}
