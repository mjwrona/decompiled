// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[11]
    {
      (IComponentCreator) new ComponentCreator<QuerySqlComponent>(1, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent2>(2, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent3>(3, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent4>(4, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent5>(5, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent6>(6, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent7>(7, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent8>(8, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent9>(9, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent10>(10, false),
      (IComponentCreator) new ComponentCreator<QuerySqlComponent11>(11, false)
    }, "WorkItemQuery", "WorkItem");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    internal const string s_Area = "WorkItemTracking";
    internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForWITQuery = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithExecutionTimeout(TimeSpan.MaxValue);
    internal Guid? FilterUnderProjectId;

    static QuerySqlComponent()
    {
      QuerySqlComponent.s_sqlExceptionFactories.Add(602016, new SqlExceptionFactory(typeof (WorkItemTrackingQueryTooComplexException)));
      QuerySqlComponent.s_sqlExceptionFactories.Add(-2, new SqlExceptionFactory(typeof (WorkItemTrackingQueryTimeoutException)));
    }

    public QuerySqlComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public static QuerySqlComponent CreateComponent(
      IVssRequestContext requestContext,
      WITQueryApplicationIntentOverride applicationIntentOverride = WITQueryApplicationIntentOverride.Default,
      Guid? filterUnderProjectId = null)
    {
      IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
      QuerySqlComponent component = applicationIntentOverride != WITQueryApplicationIntentOverride.ReadWrite ? requestContext.CreateReadReplicaAwareComponent<QuerySqlComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))) : requestContext.CreateComponent<QuerySqlComponent>();
      int queryTimeoutInSecond = configurationInfo.GetWorkItemQueryTimeoutInSecond(component.ConnectionInfo.ApplicationIntent == ApplicationIntent.ReadOnly);
      int timeoutAnonymousUser = configurationInfo.WorkItemQueryTimeoutAnonymousUser;
      component.CommandTimeoutOverride = new int?(queryTimeoutInSecond);
      component.CommandTimeoutOverrideAnonymousUser = new int?(timeoutAnonymousUser);
      component.FilterUnderProjectId = filterUnderProjectId;
      return component;
    }

    public virtual IEnumerable<AsOfQueryResultEntry> QueryWorkItem(QueryExecutionDetail detail)
    {
      Func<IEnumerable<AsOfQueryResultEntry>> run = (Func<IEnumerable<AsOfQueryResultEntry>>) (() =>
      {
        try
        {
          return (IEnumerable<AsOfQueryResultEntry>) this.ExecuteSql<List<AsOfQueryResultEntry>>(detail, (System.Func<IDataReader, List<AsOfQueryResultEntry>>) (reader => new QuerySqlComponent.AsOfQueryResultBinder().BindAll(reader).ToList<AsOfQueryResultEntry>())).ToArray();
        }
        catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "WorkItemTracking.").AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) ("QuerySqlComponent.QueryWorkItem-" + QuerySqlComponent.RemoveIllegalCharacters(this.InitialCatalog) + "-" + this.ApplicationIntent.ToString())).AndCommandPropertiesDefaults(QuerySqlComponent.DefaultCommandPropertiesForWITQuery);
      using (this.RequestContext.AcquireExemptionLock())
        return new CommandService<IEnumerable<AsOfQueryResultEntry>>(this.RequestContext, setter, run).Execute();
    }

    private List<TResult> Query<TResult, TBinder>(
      QueryExecutionDetail detail,
      out ExtendedQueryExecutionResult extendedResult)
      where TBinder : WorkItemTrackingObjectBinder<TResult>, new()
    {
      extendedResult = (ExtendedQueryExecutionResult) null;
      bool throwOnQueryLimitReached = false;
      ExtendedQueryExecutionResult internalExtendedResult = new ExtendedQueryExecutionResult();
      List<TResult> resultList1 = this.ExecuteSql<List<TResult>>(detail, (System.Func<IDataReader, List<TResult>>) (reader =>
      {
        bool flag = detail.Top == 0 || detail.Top >= detail.SizeLimit;
        int num = flag ? detail.SizeLimit : detail.Top;
        TBinder binder = new TBinder();
        List<TResult> resultList2 = new List<TResult>();
        while (reader.Read())
        {
          if (num-- > 0)
          {
            resultList2.Add(binder.Bind(reader));
          }
          else
          {
            internalExtendedResult.HasMoreResult = true;
            break;
          }
        }
        if (internalExtendedResult.HasMoreResult & flag)
        {
          throwOnQueryLimitReached = true;
          return (List<TResult>) null;
        }
        reader.NextResult();
        internalExtendedResult.HasPendingReclassification = WorkItemTrackingResourceComponent.Bind<bool>(reader, (System.Func<IDataReader, bool>) (r => r.GetBoolean(0))).First<bool>();
        reader.NextResult();
        internalExtendedResult.AsOfDateTime = WorkItemTrackingResourceComponent.Bind<DateTime>(reader, (System.Func<IDataReader, DateTime>) (r => r.GetDateTime(0))).First<DateTime>();
        return resultList2;
      }));
      if (throwOnQueryLimitReached)
        throw new WorkItemTrackingQueryResultSizeLimitExceededException(detail.SizeLimit);
      extendedResult = internalExtendedResult;
      return resultList1;
    }

    public virtual List<WorkItemQueryResultEntry> QueryWorkItem(
      QueryExecutionDetail detail,
      out ExtendedQueryExecutionResult extendedResult)
    {
      extendedResult = (ExtendedQueryExecutionResult) null;
      Func<Tuple<List<WorkItemQueryResultEntry>, ExtendedQueryExecutionResult>> run = (Func<Tuple<List<WorkItemQueryResultEntry>, ExtendedQueryExecutionResult>>) (() =>
      {
        try
        {
          ExtendedQueryExecutionResult extendedResult1 = (ExtendedQueryExecutionResult) null;
          return Tuple.Create<List<WorkItemQueryResultEntry>, ExtendedQueryExecutionResult>(this.Query<WorkItemQueryResultEntry, QuerySqlComponent.WorkItemQueryResultBinder>(detail, out extendedResult1), extendedResult1);
        }
        catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "WorkItemTracking.").AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) ("QuerySqlComponent.QueryWorkItem-" + QuerySqlComponent.RemoveIllegalCharacters(this.InitialCatalog) + "-" + this.ApplicationIntent.ToString())).AndCommandPropertiesDefaults(QuerySqlComponent.DefaultCommandPropertiesForWITQuery);
      Tuple<List<WorkItemQueryResultEntry>, ExtendedQueryExecutionResult> tuple;
      using (this.RequestContext.AcquireExemptionLock())
        tuple = new CommandService<Tuple<List<WorkItemQueryResultEntry>, ExtendedQueryExecutionResult>>(this.RequestContext, setter, run).Execute();
      extendedResult = tuple.Item2;
      return tuple.Item1;
    }

    public virtual List<LinkQueryResultEntry> QueryLink(
      QueryExecutionDetail detail,
      out ExtendedQueryExecutionResult extendedResult)
    {
      extendedResult = (ExtendedQueryExecutionResult) null;
      Func<Tuple<List<LinkQueryResultEntry>, ExtendedQueryExecutionResult>> run = (Func<Tuple<List<LinkQueryResultEntry>, ExtendedQueryExecutionResult>>) (() =>
      {
        try
        {
          ExtendedQueryExecutionResult extendedResult1 = (ExtendedQueryExecutionResult) null;
          return Tuple.Create<List<LinkQueryResultEntry>, ExtendedQueryExecutionResult>(this.Query<LinkQueryResultEntry, QuerySqlComponent.LinkQueryResultBinder>(detail, out extendedResult1), extendedResult1);
        }
        catch (WorkItemTrackingQueryResultSizeLimitExceededException ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "WorkItemTracking.").AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) ("QuerySqlComponent.QueryWorkItem-" + QuerySqlComponent.RemoveIllegalCharacters(this.InitialCatalog) + "-" + this.ApplicationIntent.ToString())).AndCommandPropertiesDefaults(QuerySqlComponent.DefaultCommandPropertiesForWITQuery);
      Tuple<List<LinkQueryResultEntry>, ExtendedQueryExecutionResult> tuple;
      using (this.RequestContext.AcquireExemptionLock())
        tuple = new CommandService<Tuple<List<LinkQueryResultEntry>, ExtendedQueryExecutionResult>>(this.RequestContext, setter, run).Execute();
      extendedResult = tuple.Item2;
      return tuple.Item1;
    }

    public virtual IList<QueryExecutionInformation> GetQueryExecutionInformation(
      IEnumerable<Guid> queryIds)
    {
      return (IList<QueryExecutionInformation>) new List<QueryExecutionInformation>();
    }

    public virtual QueryRecordingTableInfo SaveQueryExecutionInformation(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit)
    {
      return new QueryRecordingTableInfo();
    }

    public virtual QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhoc(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      IEnumerable<string> queryHashes,
      bool getRowCount)
    {
      return new QueryExecutionInfoReturnedPayload();
    }

    public virtual QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhocV2(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      IEnumerable<string> queryHashes,
      bool getRowCount)
    {
      return new QueryExecutionInfoReturnedPayload();
    }

    public virtual QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhocAndOptimization(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      DateTime mostRecentCacheUpdatedTime,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      bool getRowCount = false)
    {
      return new QueryExecutionInfoReturnedPayload();
    }

    public virtual QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhocAndOptimizationV2(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      DateTime mostRecentCacheUpdatedTime,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      bool getRowCount = false)
    {
      return new QueryExecutionInfoReturnedPayload();
    }

    public virtual void SaveQueryHistories(
      IEnumerable<QueryExecutionHistory> queryExecutionHistories)
    {
    }

    public virtual QueryExecutionInfoReturnedPayload LoadQueryOptimizationEntriesForCacheInitialization(
      int loadCount,
      DateTime cacheLoadCutOffTime)
    {
      return new QueryExecutionInfoReturnedPayload();
    }

    public virtual int CleanupQueryExecutionDetails(
      DateTime cutOffTime,
      int maxRowRount,
      int batchSize)
    {
      return 0;
    }

    public virtual int CleanupQueryExecutionInformation(
      DateTime cutOffTime,
      int maxRowRount,
      int batchSize)
    {
      return 0;
    }

    public virtual IEnumerable<QueryOptimizationInstance> ResetNonOptimizableQueries(
      DateTime lookBackLastStateChangeTime)
    {
      return (IEnumerable<QueryOptimizationInstance>) new List<QueryOptimizationInstance>();
    }

    protected virtual void BindQueryExecutionInformationTable(
      string parameterName,
      IEnumerable<QueryExecutionInformation> queryExecutionInformation)
    {
      new QuerySqlComponent.QueryExecutionInformationTable(parameterName, queryExecutionInformation).BindTable(this);
    }

    protected virtual void BindQueryExecutionHistoryTable(
      string parameterName,
      IEnumerable<QueryExecutionHistory> executionHistories)
    {
      new QuerySqlComponent.QueryExecutionHistoryTable(parameterName, executionHistories).BindTable(this);
    }

    protected virtual WorkItemTrackingObjectBinder<QueryExecutionDetailRowModel> GetQueryExecutionDetailBinder() => (WorkItemTrackingObjectBinder<QueryExecutionDetailRowModel>) new QuerySqlComponent.QueryExecutionDetailBinder();

    public virtual QueryExecutionDetailRowModel GetQueryExecutionDetailsByQueryHash(string queryHash) => (QueryExecutionDetailRowModel) null;

    public virtual QueryExecutionDetailRowModel GetQueryExecutionDetailsByQueryId(Guid queryId) => (QueryExecutionDetailRowModel) null;

    public virtual void SetSaveQueryExecutionInformationTimeout(int? timeoutInSeconds)
    {
    }

    private TResult ExecuteSql<TResult>(
      QueryExecutionDetail executionDetail,
      System.Func<IDataReader, TResult> binder)
    {
      if (this.FilterUnderProjectId.HasValue)
        this.GetDataspaceId(this.FilterUnderProjectId.Value);
      using (SqlCommand sqlCommand = this.PrepareDynamicProcedure("dynprc_ExecuteSql", executionDetail.QueryText, addStatementIndex: false))
      {
        if (executionDetail.Parameters != null)
        {
          foreach (SqlParameter parameter in executionDetail.Parameters)
            sqlCommand.Parameters.Add(parameter);
        }
        if (executionDetail.TableValuedParameters != null)
        {
          foreach (ITableValuedParameter tableValuedParameter in executionDetail.TableValuedParameters)
            tableValuedParameter.BindTable(this);
        }
        return this.ExecuteUnknown<TResult>(binder);
      }
    }

    private static string RemoveIllegalCharacters(string input) => !string.IsNullOrWhiteSpace(input) ? input.Replace('_', '-') : input;

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) QuerySqlComponent.s_sqlExceptionFactories;

    private class AsOfQueryResultBinder : WorkItemTrackingObjectBinder<AsOfQueryResultEntry>
    {
      protected SqlColumnBinder m_Id = new SqlColumnBinder("Id");
      protected SqlColumnBinder m_AreaId = new SqlColumnBinder("LatestAreaId");
      protected SqlColumnBinder m_Revision = new SqlColumnBinder("Revision");
      protected SqlColumnBinder m_AsOf = new SqlColumnBinder("AsOf");

      public override AsOfQueryResultEntry Bind(IDataReader reader)
      {
        AsOfQueryResultEntry queryResultEntry = new AsOfQueryResultEntry();
        queryResultEntry.Id = this.m_Id.GetInt32(reader);
        queryResultEntry.AreaId = this.m_AreaId.GetInt32(reader);
        queryResultEntry.Revision = this.m_Revision.GetInt32(reader);
        queryResultEntry.AsOfDateTime = this.m_AsOf.GetDateTime(reader);
        return queryResultEntry;
      }
    }

    private class WorkItemQueryResultBinder : WorkItemTrackingObjectBinder<WorkItemQueryResultEntry>
    {
      protected SqlColumnBinder m_id = new SqlColumnBinder("Id");
      protected SqlColumnBinder m_areaId = new SqlColumnBinder("LatestAreaId");

      public override WorkItemQueryResultEntry Bind(IDataReader reader) => new WorkItemQueryResultEntry()
      {
        Id = this.m_id.GetInt32(reader),
        AreaId = this.m_areaId.GetInt32(reader)
      };
    }

    internal class LinkQueryResultBinder : WorkItemTrackingObjectBinder<LinkQueryResultEntry>
    {
      private bool? m_typeExists;
      private bool? m_rhsExists;
      protected SqlColumnBinder m_sourceId = new SqlColumnBinder("SourceId");
      protected SqlColumnBinder m_targetId = new SqlColumnBinder("TargetId");
      protected SqlColumnBinder m_linkTypeId = new SqlColumnBinder("LinkTypeId");
      protected SqlColumnBinder m_isLocked = new SqlColumnBinder("IsLocked");
      protected SqlColumnBinder m_sourceAreaId = new SqlColumnBinder("SourceLatestAreaId");
      protected SqlColumnBinder m_targetAreaId = new SqlColumnBinder("TargetLatestAreaId");
      protected SqlColumnBinder m_meetsParentCriteria = new SqlColumnBinder("MeetsParentCriteria");
      protected SqlColumnBinder m_meetsChildCriteria = new SqlColumnBinder("MeetsChildCriteria");

      public override LinkQueryResultEntry Bind(IDataReader reader)
      {
        LinkQueryResultEntry queryResultEntry = new LinkQueryResultEntry();
        queryResultEntry.SourceId = this.m_sourceId.GetInt32(reader);
        if (!this.m_targetId.IsNull(reader))
          queryResultEntry.TargetId = this.m_targetId.GetInt32(reader);
        if (!this.m_isLocked.IsNull(reader))
          queryResultEntry.IsLocked = this.m_isLocked.GetBoolean(reader);
        if (!this.m_typeExists.HasValue)
          this.m_typeExists = new bool?(this.m_linkTypeId.ColumnExists(reader));
        if (this.m_typeExists.Value)
        {
          if (!this.m_linkTypeId.IsNull(reader))
            queryResultEntry.LinkTypeId = this.m_linkTypeId.GetInt16(reader);
          queryResultEntry.SourceAreaId = this.m_sourceAreaId.GetInt32(reader);
        }
        else
        {
          queryResultEntry.MeetsChildCriteria = this.m_meetsChildCriteria.GetBoolean(reader);
          queryResultEntry.MeetsParentCriteria = this.m_meetsParentCriteria.GetBoolean(reader);
          queryResultEntry.TargetAreaId = this.m_targetAreaId.GetInt32(reader);
        }
        if (!this.m_rhsExists.HasValue)
          this.m_rhsExists = new bool?(this.m_targetAreaId.ColumnExists(reader));
        if (this.m_rhsExists.Value && this.m_typeExists.Value && !this.m_targetAreaId.IsNull(reader))
          queryResultEntry.TargetAreaId = this.m_targetAreaId.GetInt32(reader);
        return queryResultEntry;
      }
    }

    protected class QueryRecordingTableInfoBinder : ObjectBinder<QueryRecordingTableInfo>
    {
      private SqlColumnBinder m_InfoTableRowCountColumn = new SqlColumnBinder("InfoTableRowCount");
      private SqlColumnBinder m_DetailTableRowCountColumn = new SqlColumnBinder("DetailTableRowCount");

      protected override QueryRecordingTableInfo Bind() => new QueryRecordingTableInfo()
      {
        InfoTableRowCount = this.m_InfoTableRowCountColumn.GetInt32((IDataReader) this.Reader),
        DetailTableRowCount = this.m_DetailTableRowCountColumn.GetInt32((IDataReader) this.Reader)
      };
    }

    protected class QueryUniqueIdentifierBinder : ObjectBinder<QueryOptimizationInstance>
    {
      private SqlColumnBinder m_QueryIdColumn = new SqlColumnBinder("QueryId");
      private SqlColumnBinder m_QueryHashColumn = new SqlColumnBinder("QueryHash");

      protected override QueryOptimizationInstance Bind() => new QueryOptimizationInstance(this.m_QueryIdColumn.GetNullableGuid((IDataReader) this.Reader), this.m_QueryHashColumn.GetString((IDataReader) this.Reader, false), QueryOptimizationStrategy.GetInstance(QueryCategory.None));
    }

    protected class QueryOptimizationInstanceBinder : ObjectBinder<QueryOptimizationInstance>
    {
      private SqlColumnBinder m_QueryIdColumn = new SqlColumnBinder("QueryId");
      private SqlColumnBinder m_QueryHashColumn = new SqlColumnBinder("QueryHash");
      private SqlColumnBinder m_LastRunTimeColumn = new SqlColumnBinder("LastRunTime");
      private SqlColumnBinder m_QueryCategoryColumn = new SqlColumnBinder("QueryCategory");
      private SqlColumnBinder m_StrategyIndexColumn = new SqlColumnBinder("StrategyIndex");
      private SqlColumnBinder m_OptimizationStateColumn = new SqlColumnBinder("OptimizationState");
      private SqlColumnBinder m_NormalwRunCountInCurrentOptColumn = new SqlColumnBinder("NormalRunCountInCurrentOpt");
      private SqlColumnBinder m_SlowRunCountInCurrentOptColumn = new SqlColumnBinder("SlowRunCountInCurrentOpt");
      private SqlColumnBinder m_LastStateChangeTimeColumn = new SqlColumnBinder("LastStateChangeTime");

      protected override QueryOptimizationInstance Bind()
      {
        short? nullableInt16 = this.m_OptimizationStateColumn.GetNullableInt16((IDataReader) this.Reader);
        long? nullableInt64 = this.m_QueryCategoryColumn.GetNullableInt64((IDataReader) this.Reader);
        if (!nullableInt16.HasValue || !nullableInt64.HasValue)
          return (QueryOptimizationInstance) null;
        QueryOptimizationInstance optimizationInstance = new QueryOptimizationInstance(this.m_QueryIdColumn.GetNullableGuid((IDataReader) this.Reader), this.m_QueryHashColumn.GetString((IDataReader) this.Reader, false), QueryOptimizationStrategy.GetInstance((QueryCategory) nullableInt64.Value));
        optimizationInstance.SetOptimizationState((QueryOptimizationState) nullableInt16.Value, this.m_StrategyIndexColumn.GetNullableInt16((IDataReader) this.Reader), this.m_NormalwRunCountInCurrentOptColumn.GetNullableInt16((IDataReader) this.Reader), this.m_SlowRunCountInCurrentOptColumn.GetNullableInt16((IDataReader) this.Reader), new DateTime?(this.m_LastRunTimeColumn.GetDateTime((IDataReader) this.Reader)), this.m_LastStateChangeTimeColumn.GetNullableDateTime((IDataReader) this.Reader));
        return optimizationInstance;
      }
    }

    protected class QueryExecutionHistorysBinder : ObjectBinder<QueryExecutionHistory>
    {
      private SqlColumnBinder m_QueryHashColumn = new SqlColumnBinder("QueryHash");
      private SqlColumnBinder m_ExecutionHistoryColumn = new SqlColumnBinder("ExecutionRecords");

      protected override QueryExecutionHistory Bind()
      {
        string str = this.m_QueryHashColumn.GetString((IDataReader) this.Reader, false);
        string json = this.m_ExecutionHistoryColumn.GetString((IDataReader) this.Reader, true);
        IEnumerable<QueryExecutionRecord> queryExecutionRecords;
        return new QueryExecutionHistory()
        {
          QueryHash = str,
          ExecutionRecords = string.IsNullOrEmpty(json) || !JsonUtilities.TryDeserialize<IEnumerable<QueryExecutionRecord>>(json, out queryExecutionRecords) ? (IEnumerable<QueryExecutionRecord>) new List<QueryExecutionRecord>() : queryExecutionRecords
        };
      }
    }

    protected class QueryExecutionDetailBinder : 
      WorkItemTrackingObjectBinder<QueryExecutionDetailRowModel>
    {
      private SqlColumnBinder m_queryHashColumn = new SqlColumnBinder("QueryHash");
      private SqlColumnBinder m_wiqlTextColumn = new SqlColumnBinder("WiqlText");
      private SqlColumnBinder m_sqlTextColumn = new SqlColumnBinder("SqlText");
      private SqlColumnBinder m_lastRunTimeColumn = new SqlColumnBinder("LastRunTime");
      private SqlColumnBinder m_executionHistoryColumn = new SqlColumnBinder("ExecutionRecords");

      public override QueryExecutionDetailRowModel Bind(IDataReader reader)
      {
        string str1 = this.m_queryHashColumn.GetString(reader, false);
        string str2 = this.m_wiqlTextColumn.GetString(reader, false);
        string str3 = this.m_sqlTextColumn.GetString(reader, true);
        DateTime dateTime = this.m_lastRunTimeColumn.GetDateTime(reader);
        string json = this.m_executionHistoryColumn.GetString(reader, true);
        QueryExecutionDetailRowModel executionDetailRowModel = new QueryExecutionDetailRowModel()
        {
          QueryHash = str1,
          WiqlText = str2,
          SqlText = str3,
          LastRunTime = dateTime
        };
        List<QueryExecutionRecord> queryExecutionRecordList;
        executionDetailRowModel.ExecutionRecords = string.IsNullOrEmpty(json) || !JsonUtilities.TryDeserialize<List<QueryExecutionRecord>>(json, out queryExecutionRecordList) ? (IEnumerable<QueryExecutionRecord>) new List<QueryExecutionRecord>() : (IEnumerable<QueryExecutionRecord>) queryExecutionRecordList;
        return executionDetailRowModel;
      }
    }

    public class ClassificationNodeIdTable : ITableValuedParameter
    {
      private string m_parameterName;
      private IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId> m_nodes;

      public ClassificationNodeIdTable(
        string parameterName,
        IEnumerable<WorkItemTrackingTreeService.ClassificationNodeId> nodes)
      {
        this.m_parameterName = parameterName;
        this.m_nodes = nodes;
      }

      public void BindTable(QuerySqlComponent component) => component.BindKeyValuePairInt32Int32Table(this.m_parameterName, (IEnumerable<KeyValuePair<int, int>>) this.m_nodes.Select<WorkItemTrackingTreeService.ClassificationNodeId, KeyValuePair<int, int>>((System.Func<WorkItemTrackingTreeService.ClassificationNodeId, KeyValuePair<int, int>>) (n => new KeyValuePair<int, int>(component.GetDataspaceId(n.ProjectId), n.NodeId))).ToList<KeyValuePair<int, int>>());
    }

    public class QueryExecutionHistoryTable : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_QueryExecutionHistoryTable = new SqlMetaData[2]
      {
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L),
        new SqlMetaData("ExecutionRecords", SqlDbType.NVarChar, SqlMetaData.Max)
      };
      private string m_parameterName;
      private IEnumerable<QueryExecutionHistory> m_histories;

      public QueryExecutionHistoryTable(
        string parameterName,
        IEnumerable<QueryExecutionHistory> histories)
      {
        this.m_parameterName = parameterName;
        this.m_histories = histories;
      }

      public void BindTable(QuerySqlComponent component) => component.Bind<QueryExecutionHistory>(this.m_parameterName, this.m_histories, QuerySqlComponent.QueryExecutionHistoryTable.typ_QueryExecutionHistoryTable, "typ_QueryExecutionHistoryTable", (Action<SqlDataRecord, QueryExecutionHistory>) ((record, history) =>
      {
        if (!string.IsNullOrEmpty(history.QueryHash))
          record.SetString(0, history.QueryHash);
        else
          record.SetDBNull(0);
        if (history.ExecutionRecords != null)
          record.SetString(1, history.ExecutionRecords.Serialize<IEnumerable<QueryExecutionRecord>>(true));
        else
          record.SetDBNull(1);
      }));
    }

    public class QueryExecutionInformationTable : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_QueryExecutionInformationTable = new SqlMetaData[8]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L),
        new SqlMetaData("LastRunTime", SqlDbType.DateTime),
        new SqlMetaData("LastRunByVsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LastExecutionTimeMilliseconds", SqlDbType.Int),
        new SqlMetaData("LastExecutionResultCount", SqlDbType.Int),
        new SqlMetaData("QueryType", SqlDbType.Int),
        new SqlMetaData("IsExecutedOnReadReplica", SqlDbType.Bit)
      };
      private string m_parameterName;
      private IEnumerable<QueryExecutionInformation> m_queryExecutionInformation;

      public QueryExecutionInformationTable(
        string parameterName,
        IEnumerable<QueryExecutionInformation> queryExecutionInformation)
      {
        this.m_parameterName = parameterName;
        this.m_queryExecutionInformation = queryExecutionInformation;
      }

      public void BindTable(QuerySqlComponent component) => component.Bind<QueryExecutionInformation>(this.m_parameterName, this.m_queryExecutionInformation, QuerySqlComponent.QueryExecutionInformationTable.typ_QueryExecutionInformationTable, "typ_QueryExecutionInformationTable", (Action<SqlDataRecord, QueryExecutionInformation>) ((record, info) =>
      {
        Guid? nullable1;
        if (info.QueryId.HasValue && info.QueryId.Value != Guid.Empty)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable1 = info.QueryId;
          Guid guid = nullable1.Value;
          sqlDataRecord.SetGuid(0, guid);
        }
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(info.QueryHash))
          record.SetString(1, info.QueryHash);
        else
          record.SetDBNull(1);
        if (info.LastRunTime.HasValue)
          record.SetDateTime(2, info.LastRunTime.Value);
        else
          record.SetDBNull(2);
        nullable1 = info.LastRunByVsid;
        if (nullable1.HasValue)
        {
          nullable1 = info.LastRunByVsid;
          if (nullable1.Value != Guid.Empty)
          {
            SqlDataRecord sqlDataRecord = record;
            nullable1 = info.LastRunByVsid;
            Guid guid = nullable1.Value;
            sqlDataRecord.SetGuid(3, guid);
            goto label_13;
          }
        }
        record.SetDBNull(3);
label_13:
        int? nullable2;
        if (info.LastExecutionTimeMilliseconds.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionTimeMilliseconds;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(4, num);
        }
        else
          record.SetDBNull(4);
        nullable2 = info.LastExecutionResultCount;
        if (nullable2.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionResultCount;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(5, num);
        }
        else
          record.SetDBNull(5);
        if (info.QueryType.HasValue)
          record.SetInt32(6, (int) info.QueryType.Value);
        else
          record.SetDBNull(6);
        if (info.IsExecutedOnReadReplica.HasValue)
          record.SetBoolean(7, info.IsExecutedOnReadReplica.Value);
        else
          record.SetDBNull(7);
      }));
    }

    public class QueryExecutionInformationTable2 : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_QueryExecutionInformationTable2 = new SqlMetaData[10]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L),
        new SqlMetaData("LastRunTime", SqlDbType.DateTime),
        new SqlMetaData("LastRunByVsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LastExecutionTimeMilliseconds", SqlDbType.Int),
        new SqlMetaData("LastExecutionResultCount", SqlDbType.Int),
        new SqlMetaData("QueryType", SqlDbType.Int),
        new SqlMetaData("IsExecutedOnReadReplica", SqlDbType.Bit),
        new SqlMetaData("WiqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("SqlText", SqlDbType.NVarChar, SqlMetaData.Max)
      };
      private string m_parameterName;
      private IEnumerable<QueryExecutionInformation> m_queryExecutionInformation;

      public QueryExecutionInformationTable2(
        string parameterName,
        IEnumerable<QueryExecutionInformation> queryExecutionInformation)
      {
        this.m_parameterName = parameterName;
        this.m_queryExecutionInformation = queryExecutionInformation;
      }

      public void BindTable(QuerySqlComponent component) => component.Bind<QueryExecutionInformation>(this.m_parameterName, this.m_queryExecutionInformation, QuerySqlComponent.QueryExecutionInformationTable2.typ_QueryExecutionInformationTable2, "typ_QueryExecutionInformationTable2", (Action<SqlDataRecord, QueryExecutionInformation>) ((record, info) =>
      {
        Guid? nullable1;
        if (info.QueryId.HasValue && info.QueryId.Value != Guid.Empty)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable1 = info.QueryId;
          Guid guid = nullable1.Value;
          sqlDataRecord.SetGuid(0, guid);
        }
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(info.QueryHash))
          record.SetString(1, info.QueryHash);
        else
          record.SetDBNull(1);
        if (info.LastRunTime.HasValue)
          record.SetDateTime(2, info.LastRunTime.Value);
        else
          record.SetDBNull(2);
        nullable1 = info.LastRunByVsid;
        if (nullable1.HasValue)
        {
          nullable1 = info.LastRunByVsid;
          if (nullable1.Value != Guid.Empty)
          {
            SqlDataRecord sqlDataRecord = record;
            nullable1 = info.LastRunByVsid;
            Guid guid = nullable1.Value;
            sqlDataRecord.SetGuid(3, guid);
            goto label_13;
          }
        }
        record.SetDBNull(3);
label_13:
        int? nullable2;
        if (info.LastExecutionTimeMilliseconds.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionTimeMilliseconds;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(4, num);
        }
        else
          record.SetDBNull(4);
        nullable2 = info.LastExecutionResultCount;
        if (nullable2.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionResultCount;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(5, num);
        }
        else
          record.SetDBNull(5);
        if (info.QueryType.HasValue)
          record.SetInt32(6, (int) info.QueryType.Value);
        else
          record.SetDBNull(6);
        if (info.IsExecutedOnReadReplica.HasValue)
          record.SetBoolean(7, info.IsExecutedOnReadReplica.Value);
        else
          record.SetDBNull(7);
        if (!string.IsNullOrEmpty(info.WiqlText))
          record.SetValue(8, (object) info.WiqlText);
        else
          record.SetDBNull(8);
        if (!string.IsNullOrEmpty(info.SqlText))
          record.SetValue(9, (object) info.SqlText);
        else
          record.SetDBNull(9);
      }));
    }

    public class QueryExecutionInformationTable3 : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_QueryExecutionInformationTable3 = new SqlMetaData[11]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L),
        new SqlMetaData("LastRunTime", SqlDbType.DateTime),
        new SqlMetaData("LastRunByVsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LastExecutionTimeMilliseconds", SqlDbType.Int),
        new SqlMetaData("LastExecutionResultCount", SqlDbType.Int),
        new SqlMetaData("QueryType", SqlDbType.Int),
        new SqlMetaData("IsExecutedOnReadReplica", SqlDbType.Bit),
        new SqlMetaData("WiqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("SqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("QueryCategory", SqlDbType.Int)
      };
      private string m_parameterName;
      private IEnumerable<QueryExecutionInformation> m_queryExecutionInformation;

      public QueryExecutionInformationTable3(
        string parameterName,
        IEnumerable<QueryExecutionInformation> queryExecutionInformation)
      {
        this.m_parameterName = parameterName;
        this.m_queryExecutionInformation = queryExecutionInformation;
      }

      public void BindTable(QuerySqlComponent component) => component.Bind<QueryExecutionInformation>(this.m_parameterName, this.m_queryExecutionInformation, QuerySqlComponent.QueryExecutionInformationTable3.typ_QueryExecutionInformationTable3, "typ_QueryExecutionInformationTable3", (Action<SqlDataRecord, QueryExecutionInformation>) ((record, info) =>
      {
        Guid? nullable1;
        if (info.QueryId.HasValue && info.QueryId.Value != Guid.Empty)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable1 = info.QueryId;
          Guid guid = nullable1.Value;
          sqlDataRecord.SetGuid(0, guid);
        }
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(info.QueryHash))
          record.SetString(1, info.QueryHash);
        else
          record.SetDBNull(1);
        if (info.LastRunTime.HasValue)
          record.SetDateTime(2, info.LastRunTime.Value);
        else
          record.SetDBNull(2);
        nullable1 = info.LastRunByVsid;
        if (nullable1.HasValue)
        {
          nullable1 = info.LastRunByVsid;
          if (nullable1.Value != Guid.Empty)
          {
            SqlDataRecord sqlDataRecord = record;
            nullable1 = info.LastRunByVsid;
            Guid guid = nullable1.Value;
            sqlDataRecord.SetGuid(3, guid);
            goto label_13;
          }
        }
        record.SetDBNull(3);
label_13:
        int? nullable2;
        if (info.LastExecutionTimeMilliseconds.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionTimeMilliseconds;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(4, num);
        }
        else
          record.SetDBNull(4);
        nullable2 = info.LastExecutionResultCount;
        if (nullable2.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionResultCount;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(5, num);
        }
        else
          record.SetDBNull(5);
        if (info.QueryType.HasValue)
          record.SetInt32(6, (int) info.QueryType.Value);
        else
          record.SetDBNull(6);
        if (info.IsExecutedOnReadReplica.HasValue)
          record.SetBoolean(7, info.IsExecutedOnReadReplica.Value);
        else
          record.SetDBNull(7);
        if (!string.IsNullOrEmpty(info.WiqlText))
          record.SetValue(8, (object) info.WiqlText);
        else
          record.SetDBNull(8);
        if (!string.IsNullOrEmpty(info.SqlText))
          record.SetValue(9, (object) info.SqlText);
        else
          record.SetDBNull(9);
        if (info.QueryCategory.HasValue)
          record.SetInt32(10, (int) info.QueryCategory.Value);
        else
          record.SetDBNull(10);
      }));
    }

    public class QueryExecutionInformationTable4 : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_QueryExecutionInformationTable4 = new SqlMetaData[17]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L),
        new SqlMetaData("LastRunTime", SqlDbType.DateTime),
        new SqlMetaData("LastRunByVsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LastExecutionTimeMilliseconds", SqlDbType.Int),
        new SqlMetaData("LastExecutionResultCount", SqlDbType.Int),
        new SqlMetaData("QueryType", SqlDbType.Int),
        new SqlMetaData("IsExecutedOnReadReplica", SqlDbType.Bit),
        new SqlMetaData("WiqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("SqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("QueryCategory", SqlDbType.Int),
        new SqlMetaData("OptimizationState", SqlDbType.SmallInt),
        new SqlMetaData("StrategyIndex", SqlDbType.SmallInt),
        new SqlMetaData("NormalRunCountInCurrentOpt", SqlDbType.SmallInt),
        new SqlMetaData("SlowRunCountInCurrentOpt", SqlDbType.SmallInt),
        new SqlMetaData("LastStateChangeTime", SqlDbType.DateTime),
        new SqlMetaData("AvgSlowRunTimeInMs", SqlDbType.Int)
      };
      private string m_parameterName;
      private IEnumerable<QueryExecutionInformation> m_queryExecutionInformation;

      public QueryExecutionInformationTable4(
        string parameterName,
        IEnumerable<QueryExecutionInformation> queryExecutionInformation)
      {
        this.m_parameterName = parameterName;
        this.m_queryExecutionInformation = queryExecutionInformation;
      }

      public void BindTable(QuerySqlComponent component) => component.Bind<QueryExecutionInformation>(this.m_parameterName, this.m_queryExecutionInformation, QuerySqlComponent.QueryExecutionInformationTable4.typ_QueryExecutionInformationTable4, "typ_QueryExecutionInformationTable4", (Action<SqlDataRecord, QueryExecutionInformation>) ((record, info) =>
      {
        Guid? nullable1;
        if (info.QueryId.HasValue && info.QueryId.Value != Guid.Empty)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable1 = info.QueryId;
          Guid guid = nullable1.Value;
          sqlDataRecord.SetGuid(0, guid);
        }
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(info.QueryHash))
          record.SetString(1, info.QueryHash);
        else
          record.SetDBNull(1);
        if (info.LastRunTime.HasValue)
          record.SetDateTime(2, info.LastRunTime.Value);
        else
          record.SetDBNull(2);
        nullable1 = info.LastRunByVsid;
        if (nullable1.HasValue)
        {
          nullable1 = info.LastRunByVsid;
          if (nullable1.Value != Guid.Empty)
          {
            SqlDataRecord sqlDataRecord = record;
            nullable1 = info.LastRunByVsid;
            Guid guid = nullable1.Value;
            sqlDataRecord.SetGuid(3, guid);
            goto label_13;
          }
        }
        record.SetDBNull(3);
label_13:
        int? nullable2;
        if (info.LastExecutionTimeMilliseconds.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionTimeMilliseconds;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(4, num);
        }
        else
          record.SetDBNull(4);
        nullable2 = info.LastExecutionResultCount;
        if (nullable2.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionResultCount;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(5, num);
        }
        else
          record.SetDBNull(5);
        if (info.QueryType.HasValue)
          record.SetInt32(6, (int) info.QueryType.Value);
        else
          record.SetDBNull(6);
        if (info.IsExecutedOnReadReplica.HasValue)
          record.SetBoolean(7, info.IsExecutedOnReadReplica.Value);
        else
          record.SetDBNull(7);
        if (!string.IsNullOrEmpty(info.WiqlText))
          record.SetValue(8, (object) info.WiqlText);
        else
          record.SetDBNull(8);
        if (!string.IsNullOrEmpty(info.SqlText))
          record.SetValue(9, (object) info.SqlText);
        else
          record.SetDBNull(9);
        if (info.QueryCategory.HasValue)
          record.SetInt32(10, (int) info.QueryCategory.Value);
        else
          record.SetDBNull(10);
        if (info.OptimizationInstance == null)
          return;
        record.SetInt16(11, (short) info.OptimizationInstance.OptimizationState);
        record.SetInt16(12, info.OptimizationInstance.StrategyIndex);
        record.SetInt16(13, info.OptimizationInstance.GetCurrentNormalRunCount());
        record.SetInt16(14, info.OptimizationInstance.GetCurrentSlowRunCount());
        record.SetDateTime(15, info.OptimizationInstance.LastStateChangeTime);
        record.SetInt32(16, 0);
      }));
    }

    public class QueryExecutionInformationTable5 : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_QueryExecutionInformationTable5 = new SqlMetaData[18]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L),
        new SqlMetaData("LastRunTime", SqlDbType.DateTime),
        new SqlMetaData("LastRunByVsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LastExecutionTimeMilliseconds", SqlDbType.Int),
        new SqlMetaData("LastExecutionResultCount", SqlDbType.Int),
        new SqlMetaData("QueryType", SqlDbType.Int),
        new SqlMetaData("IsExecutedOnReadReplica", SqlDbType.Bit),
        new SqlMetaData("WiqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("SqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("QueryCategory", SqlDbType.Int),
        new SqlMetaData("OptimizationState", SqlDbType.SmallInt),
        new SqlMetaData("StrategyIndex", SqlDbType.SmallInt),
        new SqlMetaData("DeltaNormalRunCount", SqlDbType.SmallInt),
        new SqlMetaData("DeltaSlowRunCount", SqlDbType.SmallInt),
        new SqlMetaData("IsNormalRunCountReset", SqlDbType.Bit),
        new SqlMetaData("IsSlowRunCountReset", SqlDbType.Bit),
        new SqlMetaData("LastStateChangeTime", SqlDbType.DateTime)
      };
      private string m_parameterName;
      private IEnumerable<QueryExecutionInformation> m_queryExecutionInformation;

      public QueryExecutionInformationTable5(
        string parameterName,
        IEnumerable<QueryExecutionInformation> queryExecutionInformation)
      {
        this.m_parameterName = parameterName;
        this.m_queryExecutionInformation = queryExecutionInformation;
      }

      public void BindTable(QuerySqlComponent component) => component.Bind<QueryExecutionInformation>(this.m_parameterName, this.m_queryExecutionInformation, QuerySqlComponent.QueryExecutionInformationTable5.typ_QueryExecutionInformationTable5, "typ_QueryExecutionInformationTable5", (Action<SqlDataRecord, QueryExecutionInformation>) ((record, info) =>
      {
        Guid? nullable1;
        if (info.QueryId.HasValue && info.QueryId.Value != Guid.Empty)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable1 = info.QueryId;
          Guid guid = nullable1.Value;
          sqlDataRecord.SetGuid(0, guid);
        }
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(info.QueryHash))
          record.SetString(1, info.QueryHash);
        else
          record.SetDBNull(1);
        if (info.LastRunTime.HasValue)
          record.SetDateTime(2, info.LastRunTime.Value);
        else
          record.SetDBNull(2);
        nullable1 = info.LastRunByVsid;
        if (nullable1.HasValue)
        {
          nullable1 = info.LastRunByVsid;
          if (nullable1.Value != Guid.Empty)
          {
            SqlDataRecord sqlDataRecord = record;
            nullable1 = info.LastRunByVsid;
            Guid guid = nullable1.Value;
            sqlDataRecord.SetGuid(3, guid);
            goto label_13;
          }
        }
        record.SetDBNull(3);
label_13:
        int? nullable2;
        if (info.LastExecutionTimeMilliseconds.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionTimeMilliseconds;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(4, num);
        }
        else
          record.SetDBNull(4);
        nullable2 = info.LastExecutionResultCount;
        if (nullable2.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionResultCount;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(5, num);
        }
        else
          record.SetDBNull(5);
        if (info.QueryType.HasValue)
          record.SetInt32(6, (int) info.QueryType.Value);
        else
          record.SetDBNull(6);
        if (info.IsExecutedOnReadReplica.HasValue)
          record.SetBoolean(7, info.IsExecutedOnReadReplica.Value);
        else
          record.SetDBNull(7);
        if (!string.IsNullOrEmpty(info.WiqlText))
          record.SetValue(8, (object) info.WiqlText);
        else
          record.SetDBNull(8);
        if (!string.IsNullOrEmpty(info.SqlText))
          record.SetValue(9, (object) info.SqlText);
        else
          record.SetDBNull(9);
        if (info.QueryCategory.HasValue)
          record.SetInt32(10, (int) info.QueryCategory.Value);
        else
          record.SetDBNull(10);
        if (info.OptimizationInstance == null)
          return;
        record.SetInt16(11, (short) info.OptimizationInstance.OptimizationState);
        record.SetInt16(12, info.OptimizationInstance.StrategyIndex);
        record.SetInt16(13, info.OptimizationInstance.DeltaNormalRunCount);
        record.SetInt16(14, info.OptimizationInstance.DeltaSlowRunCount);
        record.SetBoolean(15, info.OptimizationInstance.IsNormalRunCountReset);
        record.SetBoolean(16, info.OptimizationInstance.IsSlowRunCountReset);
        record.SetDateTime(17, info.OptimizationInstance.LastStateChangeTime);
      }));
    }

    public class QueryExecutionInformationTable6 : ITableValuedParameter
    {
      private static readonly SqlMetaData[] typ_QueryExecutionInformationTable6 = new SqlMetaData[19]
      {
        new SqlMetaData("QueryId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("QueryHash", SqlDbType.VarChar, 64L),
        new SqlMetaData("LastRunTime", SqlDbType.DateTime),
        new SqlMetaData("LastRunByVsid", SqlDbType.UniqueIdentifier),
        new SqlMetaData("LastExecutionTimeMilliseconds", SqlDbType.Int),
        new SqlMetaData("LastExecutionResultCount", SqlDbType.Int),
        new SqlMetaData("QueryType", SqlDbType.Int),
        new SqlMetaData("IsExecutedOnReadReplica", SqlDbType.Bit),
        new SqlMetaData("WiqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("SqlText", SqlDbType.NVarChar, SqlMetaData.Max),
        new SqlMetaData("QueryCategory", SqlDbType.Int),
        new SqlMetaData("OptimizationState", SqlDbType.SmallInt),
        new SqlMetaData("StrategyIndex", SqlDbType.SmallInt),
        new SqlMetaData("DeltaNormalRunCount", SqlDbType.SmallInt),
        new SqlMetaData("DeltaSlowRunCount", SqlDbType.SmallInt),
        new SqlMetaData("IsNormalRunCountReset", SqlDbType.Bit),
        new SqlMetaData("IsSlowRunCountReset", SqlDbType.Bit),
        new SqlMetaData("LastStateChangeTime", SqlDbType.DateTime),
        new SqlMetaData("IsStateForkedFromOtherInstance", SqlDbType.Bit)
      };
      private string m_parameterName;
      private IEnumerable<QueryExecutionInformation> m_queryExecutionInformation;

      public QueryExecutionInformationTable6(
        string parameterName,
        IEnumerable<QueryExecutionInformation> queryExecutionInformation)
      {
        this.m_parameterName = parameterName;
        this.m_queryExecutionInformation = queryExecutionInformation;
      }

      public void BindTable(QuerySqlComponent component) => component.Bind<QueryExecutionInformation>(this.m_parameterName, this.m_queryExecutionInformation, QuerySqlComponent.QueryExecutionInformationTable6.typ_QueryExecutionInformationTable6, "typ_QueryExecutionInformationTable6", (Action<SqlDataRecord, QueryExecutionInformation>) ((record, info) =>
      {
        Guid? nullable1;
        if (info.QueryId.HasValue && info.QueryId.Value != Guid.Empty)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable1 = info.QueryId;
          Guid guid = nullable1.Value;
          sqlDataRecord.SetGuid(0, guid);
        }
        else
          record.SetDBNull(0);
        if (!string.IsNullOrEmpty(info.QueryHash))
          record.SetString(1, info.QueryHash);
        else
          record.SetDBNull(1);
        if (info.LastRunTime.HasValue)
          record.SetDateTime(2, info.LastRunTime.Value);
        else
          record.SetDBNull(2);
        nullable1 = info.LastRunByVsid;
        if (nullable1.HasValue)
        {
          nullable1 = info.LastRunByVsid;
          if (nullable1.Value != Guid.Empty)
          {
            SqlDataRecord sqlDataRecord = record;
            nullable1 = info.LastRunByVsid;
            Guid guid = nullable1.Value;
            sqlDataRecord.SetGuid(3, guid);
            goto label_13;
          }
        }
        record.SetDBNull(3);
label_13:
        int? nullable2;
        if (info.LastExecutionTimeMilliseconds.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionTimeMilliseconds;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(4, num);
        }
        else
          record.SetDBNull(4);
        nullable2 = info.LastExecutionResultCount;
        if (nullable2.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          nullable2 = info.LastExecutionResultCount;
          int num = nullable2.Value;
          sqlDataRecord.SetInt32(5, num);
        }
        else
          record.SetDBNull(5);
        if (info.QueryType.HasValue)
          record.SetInt32(6, (int) info.QueryType.Value);
        else
          record.SetDBNull(6);
        if (info.IsExecutedOnReadReplica.HasValue)
          record.SetBoolean(7, info.IsExecutedOnReadReplica.Value);
        else
          record.SetDBNull(7);
        if (!string.IsNullOrEmpty(info.WiqlText))
          record.SetValue(8, (object) info.WiqlText);
        else
          record.SetDBNull(8);
        if (!string.IsNullOrEmpty(info.SqlText))
          record.SetValue(9, (object) info.SqlText);
        else
          record.SetDBNull(9);
        if (info.QueryCategory.HasValue)
          record.SetInt32(10, (int) info.QueryCategory.Value);
        else
          record.SetDBNull(10);
        if (info.OptimizationInstance == null)
          return;
        record.SetInt16(11, (short) info.OptimizationInstance.OptimizationState);
        record.SetInt16(12, info.OptimizationInstance.StrategyIndex);
        record.SetInt16(13, info.OptimizationInstance.DeltaNormalRunCount);
        record.SetInt16(14, info.OptimizationInstance.DeltaSlowRunCount);
        record.SetBoolean(15, info.OptimizationInstance.IsNormalRunCountReset);
        record.SetBoolean(16, info.OptimizationInstance.IsSlowRunCountReset);
        record.SetDateTime(17, info.OptimizationInstance.LastStateChangeTime);
        record.SetBoolean(18, info.OptimizationInstance.IsStateForkedFromOtherInstance);
      }));
    }
  }
}
