// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExecutionLogger
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryExecutionLogger : IQueryExecutionLogger, IVssFrameworkService
  {
    private static readonly RegistryQuery s_SaveQueryExecutionInformationTimeoutQuery = new RegistryQuery("/Service/WorkItemTracking/WIQL/SaveQueryExecutionInformationTimeout");
    private FlushableBuffer<QueryExecutionInformation> m_queryInfoFlusher;
    private DateTime m_lastGettingRowCountTime;
    private int m_lastInfoTableRowCount;
    private int m_lastDetailTableRowCount;
    private int? m_saveQueryExecutionInformationTimeout;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.TraceBlock(913600, 913601, "Query", nameof (QueryExecutionLogger), nameof (ServiceStart), (Action) (() =>
      {
        IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
        this.m_saveQueryExecutionInformationTimeout = service.GetValue<int?>(systemRequestContext, in QueryExecutionLogger.s_SaveQueryExecutionInformationTimeoutQuery, true);
        service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSaveQueryExecutionInformationTimeoutChanged), true, in QueryExecutionLogger.s_SaveQueryExecutionInformationTimeoutQuery);
        this.m_queryInfoFlusher = new FlushableBuffer<QueryExecutionInformation>(systemRequestContext, "/Service/WorkItemTracking/Settings/QueryExecutionLogging", "QueryExecutionInfo", (IEqualityComparer<QueryExecutionInformation>) QueryExecutionInformationComparer.Instance, new Action<IVssRequestContext, ICollection<QueryExecutionInformation>>(this.FlushQueryExecutionInformation));
      }));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.TraceBlock(913602, 913603, "Query", nameof (QueryExecutionLogger), nameof (ServiceEnd), (Action) (() =>
    {
      this.m_queryInfoFlusher.Dispose(systemRequestContext);
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSaveQueryExecutionInformationTimeoutChanged));
    }));

    public QueryExecutionInformation GetQueryExecutionInformation(
      IVssRequestContext requestContext,
      Guid queryId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(queryId, nameof (queryId));
      requestContext.TraceEnter(913501, "Query", nameof (QueryExecutionLogger), nameof (GetQueryExecutionInformation));
      try
      {
        using (QuerySqlComponent component = requestContext.CreateComponent<QuerySqlComponent>())
          return component.GetQueryExecutionInformation((IEnumerable<Guid>) new Guid[1]
          {
            queryId
          }).FirstOrDefault<QueryExecutionInformation>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(913503, "Query", nameof (QueryExecutionLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(913502, "Query", nameof (QueryExecutionLogger), nameof (GetQueryExecutionInformation));
      }
    }

    public int CleanupQueryExecutionDetails(IVssRequestContext requestContext)
    {
      (DateTime, int, int) executionDetailsSettings = this.GetQueryExecutionDetailsSettings(requestContext);
      return this.CleanupQueryExecutionDetails(requestContext, executionDetailsSettings);
    }

    public int CleanupQueryExecutionDetails(
      IVssRequestContext requestContext,
      (DateTime cutOffTime, int maxRowCount, int batchSize) queryExecutionDetailsSettings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(913510, "Query", nameof (QueryExecutionLogger), nameof (CleanupQueryExecutionDetails));
      DateTime cutOffTime = queryExecutionDetailsSettings.cutOffTime;
      int maxRowCount = queryExecutionDetailsSettings.maxRowCount;
      int batchSize = queryExecutionDetailsSettings.batchSize;
      try
      {
        using (QuerySqlComponent component = requestContext.CreateComponent<QuerySqlComponent>())
          return component.CleanupQueryExecutionDetails(cutOffTime, maxRowCount, batchSize);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(913512, "Query", nameof (QueryExecutionLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(913511, "Query", nameof (QueryExecutionLogger), nameof (CleanupQueryExecutionDetails));
      }
    }

    protected virtual (DateTime cutOffTime, int maxRowCount, int batchSize) GetQueryExecutionDetailsSettings(
      IVssRequestContext requestContext)
    {
      WorkItemTrackingQueryExecutionLoggingConfiguration executionLoggingConfig = requestContext.WitContext().ServerSettings.QueryExecutionLoggingConfig;
      DateTime dateTime = executionLoggingConfig.DetailTableCutoffDaysOffset <= -1 ? DateTime.UtcNow.AddDays((double) executionLoggingConfig.DetailTableCutoffDaysOffset) : DateTime.UtcNow.AddDays(-1.0);
      int num1 = executionLoggingConfig.DetailTableMaxRowCount;
      if (num1 < 10000)
        num1 = 10000;
      int num2 = executionLoggingConfig.CleanupBatchSize;
      if (num2 < 1)
        num2 = 1;
      return (dateTime, num1, num2);
    }

    public int CleanupQueryExecutionInformation(IVssRequestContext requestContext)
    {
      (DateTime, int, int) informationSettings = this.GetQueryExecutionInformationSettings(requestContext);
      return this.CleanupQueryExecutionInformation(requestContext, informationSettings);
    }

    public int CleanupQueryExecutionInformation(
      IVssRequestContext requestContext,
      (DateTime cutOffTime, int maxAdhocQueriesRowCount, int batchSize) queryExecutionInformationSettings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(913513, "Query", nameof (QueryExecutionLogger), nameof (CleanupQueryExecutionInformation));
      DateTime cutOffTime = queryExecutionInformationSettings.cutOffTime;
      int adhocQueriesRowCount = queryExecutionInformationSettings.maxAdhocQueriesRowCount;
      int batchSize = queryExecutionInformationSettings.batchSize;
      try
      {
        using (QuerySqlComponent component = requestContext.CreateComponent<QuerySqlComponent>())
          return component.CleanupQueryExecutionInformation(cutOffTime, adhocQueriesRowCount, batchSize);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(913509, "Query", nameof (QueryExecutionLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(913508, "Query", nameof (QueryExecutionLogger), nameof (CleanupQueryExecutionInformation));
      }
    }

    protected virtual (DateTime cutOffTime, int maxAdhocQueriesRowCount, int batchSize) GetQueryExecutionInformationSettings(
      IVssRequestContext requestContext)
    {
      WorkItemTrackingQueryExecutionLoggingConfiguration executionLoggingConfig = requestContext.WitContext().ServerSettings.QueryExecutionLoggingConfig;
      DateTime dateTime = executionLoggingConfig.InfoTableCutoffDaysOffset <= -1 ? DateTime.UtcNow.AddDays((double) executionLoggingConfig.InfoTableCutoffDaysOffset) : DateTime.UtcNow.AddDays(-1.0);
      int num1 = executionLoggingConfig.InfoTableMaxAdhocQueryRowCount;
      if (num1 < 0)
        num1 = 0;
      int num2 = executionLoggingConfig.CleanupBatchSize;
      if (num2 < 1)
        num2 = 1;
      return (dateTime, num1, num2);
    }

    public void RecordQueryExecutionInformation(
      IVssRequestContext requestContext,
      QueryExpression queryExpression,
      DateTime runTime,
      Guid runByVsid,
      int executionTimeInMilliseconds,
      int? resultCount,
      QueryType queryType,
      string sqlText,
      bool IsExecutedOnReadReplica,
      QueryCategory queryCategory,
      QueryOptimizationInstance optimizationInstance)
    {
      if (string.IsNullOrEmpty(queryExpression.QueryHash) || !CommonWITUtils.HasRecordQueryExecutionInfoPermission(requestContext) || string.IsNullOrEmpty(queryExpression.Wiql) || string.IsNullOrEmpty(sqlText))
        return;
      requestContext.TraceEnter(913504, "Query", nameof (QueryExecutionLogger), nameof (RecordQueryExecutionInformation));
      WorkItemTrackingQueryExecutionLoggingConfiguration executionLoggingConfig = requestContext.WitContext().ServerSettings.QueryExecutionLoggingConfig;
      int maxTextLength = executionLoggingConfig.MaxTextLength;
      double bucketSizeInMinutes = executionLoggingConfig.SampleBucketSizeInMinutes;
      string str = string.Empty;
      bool needResetHistory = false;
      if (queryExpression.OptimizationSource == QueryOptimizationSource.DynamicSwitch && optimizationInstance != null)
        this.ProcessOptimizationInstance(requestContext, executionTimeInMilliseconds, optimizationInstance, runTime, IsExecutedOnReadReplica, out needResetHistory);
      else if (WorkItemTrackingFeatureFlags.IsQueryAutoOptimizationEnabled(requestContext) && queryExpression.OptimizationSource == QueryOptimizationSource.None)
      {
        QueryOptimizationCacheService service = requestContext.GetService<QueryOptimizationCacheService>();
        if (service.IsCacheInitializationCompleted)
        {
          QueryOptimizationStrategy instance = QueryOptimizationStrategy.GetInstance(queryCategory);
          optimizationInstance = service.GetQueryOptimizationInstance(requestContext, queryExpression.QueryId, queryExpression.QueryHash, instance, false);
          if (optimizationInstance == null)
          {
            optimizationInstance = new QueryOptimizationInstance(queryExpression.QueryId, queryExpression.QueryHash, instance, QueryOptimizationState.None);
            this.ProcessOptimizationInstance(requestContext, executionTimeInMilliseconds, optimizationInstance, runTime, IsExecutedOnReadReplica, out needResetHistory);
            str = "New";
          }
          else
            str = "FromCache";
        }
      }
      List<QueryExecutionRecord> queryExecutionRecordList = new List<QueryExecutionRecord>();
      double num1 = bucketSizeInMinutes;
      DateTime dateTime = DateTime.MinValue;
      if (WorkItemTrackingFeatureFlags.IsQueryAutoOptimizationEnabled(requestContext))
      {
        int num2 = requestContext.WitContext().ServerSettings.GetWorkItemQueryTimeoutInSecond(IsExecutedOnReadReplica) * 1000;
        if (executionTimeInMilliseconds >= num2)
          num1 = 1.0;
        dateTime = runTime.AddTicks(-(runTime.Ticks % TimeSpan.FromMinutes(num1).Ticks));
        queryExecutionRecordList.Add(new QueryExecutionRecord()
        {
          Bucket = dateTime,
          DurationInMs = executionTimeInMilliseconds
        });
      }
      QueryExecutionInformation executionInformation = new QueryExecutionInformation()
      {
        QueryId = new Guid?(queryExpression.QueryId.GetValueOrDefault()),
        QueryHash = queryExpression.QueryHash,
        LastRunByVsid = new Guid?(runByVsid),
        LastRunTime = new DateTime?(runTime),
        LastExecutionTimeMilliseconds = new int?(executionTimeInMilliseconds),
        LastExecutionResultCount = resultCount,
        QueryType = new QueryType?(queryType),
        IsExecutedOnReadReplica = new bool?(IsExecutedOnReadReplica),
        WiqlText = QueryExecutionLogger.TrancateTextIfTooLong(maxTextLength, queryExpression.Wiql),
        SqlText = QueryExecutionLogger.TrancateTextIfTooLong(maxTextLength, sqlText),
        QueryCategory = new QueryCategory?(queryCategory),
        OptimizationInstance = optimizationInstance,
        ExecutionHistory = (IList<QueryExecutionRecord>) queryExecutionRecordList,
        NeedResetHistory = needResetHistory
      };
      CustomerIntelligenceService service1 = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("QueryId", (object) executionInformation.QueryId);
      intelligenceData.Add("QueryHash", executionInformation.QueryHash);
      intelligenceData.Add("WiqlTextLength", (double) queryExpression.Wiql.Length);
      intelligenceData.Add("SqlTextLength", (double) sqlText.Length);
      intelligenceData.Add("Bucket", (object) dateTime);
      intelligenceData.Add("InstanceCreationMode", str);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service1.Publish(requestContext1, nameof (QueryExecutionLogger), nameof (RecordQueryExecutionInformation), properties);
      try
      {
        this.m_queryInfoFlusher.Add(requestContext, (ICollection<QueryExecutionInformation>) new QueryExecutionInformation[1]
        {
          executionInformation
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(913506, "Query", nameof (QueryExecutionLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(913505, "Query", nameof (QueryExecutionLogger), nameof (RecordQueryExecutionInformation));
      }
    }

    public IEnumerable<QueryOptimizationInstance> ResetNonOptimizableQueries(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(913516, "Query", nameof (QueryExecutionLogger), nameof (ResetNonOptimizableQueries));
      DateTime lookBackLastStateChangeTime = DateTime.UtcNow.AddDays((double) -requestContext.WitContext().ServerSettings.QueryExecutionLoggingConfig.LookBackInDaysForResetNonOptimizable);
      try
      {
        using (QuerySqlComponent component = requestContext.CreateComponent<QuerySqlComponent>())
          return component.ResetNonOptimizableQueries(lookBackLastStateChangeTime);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(913518, "Query", nameof (QueryExecutionLogger), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(913517, "Query", nameof (QueryExecutionLogger), nameof (ResetNonOptimizableQueries));
      }
    }

    private void ProcessOptimizationInstance(
      IVssRequestContext requestContext,
      int executionTimeInMilliseconds,
      QueryOptimizationInstance optimizationInstance,
      DateTime runTime,
      bool isExecutedOnReadReplica,
      out bool needResetHistory)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<QueryOptimizationInstance>(optimizationInstance, nameof (optimizationInstance));
      IQueryOptimizationCacheService service1 = requestContext.GetService<IQueryOptimizationCacheService>();
      CustomerIntelligenceService service2 = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData ciData = new CustomerIntelligenceData();
      this.RecordSerializedOjectForCI(ciData, "OptimizationInstance", (object) optimizationInstance);
      int queryTimeoutInSecond = requestContext.WitContext().ServerSettings.GetWorkItemQueryTimeoutInSecond(isExecutedOnReadReplica);
      optimizationInstance.MoveNext(requestContext, executionTimeInMilliseconds, runTime, out needResetHistory, (queryTimeoutInSecond - 1) * 1000);
      this.RecordSerializedOjectForCI(ciData, "OptimizationInstanceAfterMove", (object) optimizationInstance);
      ciData.Add("NeedResetHistory", needResetHistory);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = ciData;
      service2.Publish(requestContext1, nameof (QueryExecutionLogger), nameof (ProcessOptimizationInstance), properties);
      IVssRequestContext requestContext2 = requestContext;
      QueryOptimizationInstance instance = optimizationInstance;
      service1.AddOrUpdate(requestContext2, instance);
    }

    private void RecordSerializedOjectForCI(
      CustomerIntelligenceData ciData,
      string propertyName,
      object obj)
    {
      try
      {
        ciData.Add(propertyName, JsonUtilities.Serialize(obj));
      }
      catch (Exception ex)
      {
        ciData.Add("Exception", ex.Message);
      }
    }

    private void OnSaveQueryExecutionInformationTimeoutChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_saveQueryExecutionInformationTimeout = requestContext.GetService<IVssRegistryService>().GetValue<int?>(requestContext, in QueryExecutionLogger.s_SaveQueryExecutionInformationTimeoutQuery, true);
    }

    private void FlushQueryExecutionInformation(
      IVssRequestContext requestContext,
      ICollection<QueryExecutionInformation> queryExecutionInfo)
    {
      if (WorkItemTrackingFeatureFlags.IsQueryExecutionInformationSprocDisabled(requestContext))
        return;
      requestContext.TraceEnter(913507, "Query", nameof (QueryExecutionLogger), nameof (FlushQueryExecutionInformation));
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      Stopwatch stopwatch2 = new Stopwatch();
      QueryRecordingTableInfo recordingTableInfo = (QueryRecordingTableInfo) null;
      IEnumerable<QueryOptimizationInstance> optimizationInstances = (IEnumerable<QueryOptimizationInstance>) null;
      IEnumerable<QueryExecutionHistory> executionHistories = (IEnumerable<QueryExecutionHistory>) null;
      bool hostedDeployment = requestContext.ExecutionEnvironment.IsHostedDeployment;
      bool flag1 = WorkItemTrackingFeatureFlags.IsQueryAutoOptimizationEnabled(requestContext);
      WorkItemTrackingQueryExecutionLoggingConfiguration executionLoggingConfig = requestContext.WitContext().ServerSettings.QueryExecutionLoggingConfig;
      int tableRowCountLimit = executionLoggingConfig.InfoTableRowCountLimit;
      int tableMaxRowCount = executionLoggingConfig.DetailTableMaxRowCount;
      int recordCountLimit = executionLoggingConfig.HistoryRecordCountLimit;
      int percentileForThreshold = executionLoggingConfig.PercentileForThreshold;
      int countIntervalInHours = executionLoggingConfig.GetRowCountIntervalInHours;
      int recordCountForOpt = executionLoggingConfig.MinRecordCountForOpt;
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      bool flag2 = WorkItemTrackingFeatureFlags.IsSaveQueryExecutionInformationSprocV2Enabled(requestContext);
      IQueryOptimizationCacheService optimizationCacheService = (IQueryOptimizationCacheService) null;
      if (flag1)
        optimizationCacheService = requestContext.GetService<IQueryOptimizationCacheService>();
      try
      {
        bool getRowCount = this.m_lastGettingRowCountTime.AddHours((double) countIntervalInHours) < DateTime.UtcNow;
        if (((this.m_lastInfoTableRowCount >= tableRowCountLimit ? 0 : (this.m_lastDetailTableRowCount < tableMaxRowCount ? 1 : 0)) | (getRowCount ? 1 : 0)) != 0)
        {
          HashSet<string> localQueryHashes = hostedDeployment ? queryExecutionInfo.Select<QueryExecutionInformation, string>((Func<QueryExecutionInformation, string>) (q => q.QueryHash)).ToHashSet<string>() : (HashSet<string>) null;
          QueryExecutionInfoReturnedPayload infoReturnedPayload = (QueryExecutionInfoReturnedPayload) null;
          using (QuerySqlComponent component = requestContext.Elevate().CreateComponent<QuerySqlComponent>())
          {
            component.SetSaveQueryExecutionInformationTimeout(this.m_saveQueryExecutionInformationTimeout);
            DateTime mostRecentCacheUpdatedTime = DateTime.MinValue;
            if (flag1 && optimizationCacheService != null)
            {
              mostRecentCacheUpdatedTime = optimizationCacheService.MostRecentUpdatedFromDBTime == DateTime.MinValue ? optimizationCacheService.MostRecentUpdatedFromDBTime : optimizationCacheService.MostRecentUpdatedFromDBTime.AddSeconds(-130.0);
              infoReturnedPayload = !flag2 ? component.SaveQueryExecutionInformationIncludingAdhocAndOptimization((IEnumerable<QueryExecutionInformation>) queryExecutionInfo, mostRecentCacheUpdatedTime, tableRowCountLimit, tableMaxRowCount, getRowCount) : component.SaveQueryExecutionInformationIncludingAdhocAndOptimizationV2((IEnumerable<QueryExecutionInformation>) queryExecutionInfo, mostRecentCacheUpdatedTime, tableRowCountLimit, tableMaxRowCount, getRowCount);
              recordingTableInfo = infoReturnedPayload.QueryExecutionTableInfo;
            }
            else if (hostedDeployment)
            {
              infoReturnedPayload = !flag2 ? component.SaveQueryExecutionInformationIncludingAdhoc((IEnumerable<QueryExecutionInformation>) queryExecutionInfo, tableRowCountLimit, tableMaxRowCount, (IEnumerable<string>) localQueryHashes, getRowCount) : component.SaveQueryExecutionInformationIncludingAdhocV2((IEnumerable<QueryExecutionInformation>) queryExecutionInfo, tableRowCountLimit, tableMaxRowCount, (IEnumerable<string>) localQueryHashes, getRowCount);
              recordingTableInfo = infoReturnedPayload.QueryExecutionTableInfo;
            }
            else
              recordingTableInfo = component.SaveQueryExecutionInformation((IEnumerable<QueryExecutionInformation>) queryExecutionInfo, tableRowCountLimit, tableMaxRowCount);
            optimizationInstances = infoReturnedPayload?.QueryOptimizationInstances;
            executionHistories = infoReturnedPayload?.QueryHistories;
            if (executionHistories != null)
            {
              stopwatch2.Start();
              CustomerIntelligenceData intelligenceData1 = new CustomerIntelligenceData();
              intelligenceData1.Add("CacheTimeStamp", (object) mostRecentCacheUpdatedTime);
              this.RecordSerializedOjectForCI(intelligenceData1, "RemoteHistories", (object) executionHistories.Select(h => new
              {
                Hash = h.QueryHash,
                Count = h.ExecutionRecords.Count<QueryExecutionRecord>()
              }));
              service.Publish(requestContext, nameof (QueryExecutionLogger), "PreMergeRemoteHistories", intelligenceData1);
              List<QueryExecutionHistory> list = QueryExecutionInformationHelper.MergeWithQueryHistories(queryExecutionInfo, executionHistories, recordCountLimit).ToList<QueryExecutionHistory>();
              CustomerIntelligenceData intelligenceData2 = new CustomerIntelligenceData();
              this.RecordSerializedOjectForCI(intelligenceData2, "LocalHistories", (object) list.Select(h => new
              {
                Hash = h.QueryHash,
                Count = h.ExecutionRecords.Count<QueryExecutionRecord>()
              }));
              service.Publish(requestContext, nameof (QueryExecutionLogger), "PostMegeLocalHistories", intelligenceData2);
              if (flag1)
              {
                QueryOptimizationInstancesHelper.GetSlownessThresholdFromHistories(optimizationInstances, list.Concat<QueryExecutionHistory>(executionHistories.Where<QueryExecutionHistory>((Func<QueryExecutionHistory, bool>) (history => localQueryHashes == null || !localQueryHashes.Contains(history.QueryHash)))), recordCountForOpt, percentileForThreshold, false);
                CustomerIntelligenceData intelligenceData3 = new CustomerIntelligenceData();
                this.RecordSerializedOjectForCI(intelligenceData3, "Threshold", (object) optimizationInstances.Select(i => new
                {
                  Hash = i.QueryHash,
                  Id = i.QueryId,
                  Threshold = i.SlownessThresholdInMsFromHistory
                }));
                service.Publish(requestContext, nameof (QueryExecutionLogger), "GetSlownessThreshold", intelligenceData3);
              }
              component.SaveQueryHistories((IEnumerable<QueryExecutionHistory>) list);
              stopwatch2.Stop();
            }
          }
          if (flag1 && optimizationCacheService != null && optimizationInstances != null)
            optimizationCacheService.AddOrUpdateFromDB(requestContext, optimizationInstances);
        }
        if (recordingTableInfo != null)
        {
          this.m_lastInfoTableRowCount = recordingTableInfo.InfoTableRowCount;
          this.m_lastDetailTableRowCount = recordingTableInfo.DetailTableRowCount;
        }
        if (!getRowCount)
          return;
        this.m_lastGettingRowCountTime = DateTime.UtcNow;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(913509, "Query", nameof (QueryExecutionLogger), ex);
        throw;
      }
      finally
      {
        stopwatch1.Stop();
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("ElapsedTime", (double) stopwatch1.ElapsedMilliseconds);
        properties.Add("SamplingElapsedTime", (double) stopwatch2.ElapsedMilliseconds);
        properties.Add("BufferSize", (double) queryExecutionInfo.Count);
        properties.Add("InfoTableRowCount", (object) recordingTableInfo?.InfoTableRowCount);
        properties.Add("InfoTableRowCountLimit", (double) tableRowCountLimit);
        properties.Add("DetailTableRowCount", (object) recordingTableInfo?.DetailTableRowCount);
        properties.Add("DetailTableRowCountLimit", (double) tableMaxRowCount);
        if (flag1 && optimizationInstances != null)
          properties.Add("OptimizationInstancesCount", (double) optimizationInstances.Count<QueryOptimizationInstance>());
        if (flag1 && executionHistories != null)
        {
          properties.Add("QueryHistoriesCount", (double) executionHistories.Count<QueryExecutionHistory>());
          properties.Add("QueryHistoriesMaxBucketCnt", (double) executionHistories.Max<QueryExecutionHistory>((Func<QueryExecutionHistory, int>) (history => history.ExecutionRecords.Count<QueryExecutionRecord>())));
          properties.Add("QueryHistoriesAvgBucketCnt", executionHistories.Average<QueryExecutionHistory>((Func<QueryExecutionHistory, int>) (history => history.ExecutionRecords.Count<QueryExecutionRecord>())));
        }
        service.Publish(requestContext, nameof (QueryExecutionLogger), nameof (FlushQueryExecutionInformation), properties);
        requestContext.TraceLeave(913508, "Query", nameof (QueryExecutionLogger), nameof (FlushQueryExecutionInformation));
      }
    }

    private static string TrancateTextIfTooLong(int maxTextLength, string text) => maxTextLength < 0 || string.IsNullOrEmpty(text) || text.Length <= maxTextLength ? text : text.Substring(0, maxTextLength);
  }
}
