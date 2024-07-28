// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildQueueComponent10
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class BuildQueueComponent10 : BuildQueueComponent9
  {
    protected static SqlMetaData[] typ_BuildIdDataspaceTable = new SqlMetaData[2]
    {
      new SqlMetaData("Id", SqlDbType.Int),
      new SqlMetaData("DataspaceId", SqlDbType.Int)
    };
    protected static SqlMetaData[] typ_BuildRequestTableV3 = new SqlMetaData[15]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("DefinitionId", SqlDbType.Int),
      new SqlMetaData("Priority", SqlDbType.TinyInt),
      new SqlMetaData("Postponed", SqlDbType.Bit),
      new SqlMetaData("BatchId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("GetOption", SqlDbType.Int),
      new SqlMetaData("GetVersion", SqlDbType.NVarChar, 326L),
      new SqlMetaData("MaxPosition", SqlDbType.Int),
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("DropLocation", SqlDbType.NVarChar, 400L),
      new SqlMetaData("RequestedFor", SqlDbType.NVarChar, 38L),
      new SqlMetaData("RequestedBy", SqlDbType.NVarChar, 38L),
      new SqlMetaData("ShelvesetName", SqlDbType.NVarChar, 1024L),
      new SqlMetaData("Reason", SqlDbType.Int),
      new SqlMetaData("ProcessParameters", SqlDbType.NVarChar, -1L)
    };
    protected static SqlMetaData[] typ_QueuedBuildUpdateTableV3 = new SqlMetaData[7]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("Fields", SqlDbType.Int),
      new SqlMetaData("BatchId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Postponed", SqlDbType.Bit),
      new SqlMetaData("Priority", SqlDbType.TinyInt),
      new SqlMetaData("Retry", SqlDbType.TinyInt)
    };

    internal override ResultCollection CancelBuilds(ICollection<QueuedBuild> queuedBuilds)
    {
      this.TraceEnter(0, nameof (CancelBuilds));
      this.PrepareStoredProcedure("prc_CancelBuilds");
      this.BindQueuedBuildIdDataspaceTable("@queuedBuildTable", (IEnumerable<QueuedBuild>) queuedBuilds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (CancelBuilds));
      return resultCollection;
    }

    internal override ResultCollection QueueBuilds(
      ICollection<BuildRequest> requests,
      QueueOptions options)
    {
      this.TraceEnter(0, nameof (QueueBuilds));
      this.PrepareStoredProcedure("prc_QueueBuilds");
      this.BindBuildRequestTable("@buildRequestTable", (IEnumerable<BuildRequest>) requests);
      this.BindBoolean("@preview", (options & QueueOptions.Preview) == QueueOptions.Preview);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (QueueBuilds));
      return resultCollection;
    }

    internal override ResultCollection QueryBuilds(
      IVssRequestContext requestContext,
      BuildQueueSpec spec,
      QueryOptions options)
    {
      this.TraceEnter(0, nameof (QueryBuilds));
      this.PrepareStoredProcedure("prc_QueryQueuedBuilds");
      if (spec.DefinitionFilterType == DefinitionFilterType.DefinitionSpec)
      {
        this.BindTeamProjectDataspace(requestContext, "@dataspaceId", ((BuildDefinitionSpec) spec.DefinitionFilter).TeamProject, false);
        this.BindItemPath("@definitionPath", ((BuildDefinitionSpec) spec.DefinitionFilter).Path, false);
      }
      else
        this.BindUrisToDistinctInt32Table("@definitionIdTable", (IEnumerable<string>) spec.DefinitionFilter);
      this.BindItemName("@controllerName", spec.ControllerSpec.Name, 256, false);
      this.BindItemName("@serviceHostName", spec.ControllerSpec.ServiceHostName, 256, false);
      this.BindIdentityFilter("@requestedFor", spec.RequestedFor, spec.RequestedForIdentity);
      this.BindByte("@statusFilter", (byte) spec.Status);
      this.BindInt("@completedAge", spec.CompletedAge);
      this.BindByte("@options", (byte) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuilds));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildsById(IList<int> ids, QueryOptions options)
    {
      this.TraceEnter(0, nameof (QueryBuildsById));
      this.PrepareStoredProcedure("prc_QueryQueuedBuildsById");
      this.BindIntsToOrderedIntsTable("@queuedBuildIdTable", (IEnumerable<int>) ids);
      this.BindInt("@options", (int) options);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<RetentionPolicy>((ObjectBinder<RetentionPolicy>) new RetentionPolicyBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<Schedule>((ObjectBinder<Schedule>) new ScheduleBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinitionSourceProvider>((ObjectBinder<BuildDefinitionSourceProvider>) new BuildDefinitionSourceProviderBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<ProcessTemplate>((ObjectBinder<ProcessTemplate>) new ProcessTemplateBinder3(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceTemplate>((ObjectBinder<WorkspaceTemplate>) new WorkspaceTemplateBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<WorkspaceMapping>((ObjectBinder<WorkspaceMapping>) new WorkspaceMappingBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildsById));
      return resultCollection;
    }

    internal override IDictionary<int, List<int>> GetQueueIdsByBuildIds(
      Guid projectId,
      IList<int> buildIds)
    {
      this.TraceEnter(0, "GetQueuedBuildIdsByBuildId");
      this.PrepareStoredProcedure("prc_GetQueuedBuildIdsByBuildId");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt32Table("@buildIds", buildIds.Distinct<int>());
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<int, int>>((ObjectBinder<KeyValuePair<int, int>>) new Int32PairBinder());
      Dictionary<int, List<int>> dictionary = resultCollection.GetCurrent<KeyValuePair<int, int>>().Items.ToLookup<KeyValuePair<int, int>, int, int>((System.Func<KeyValuePair<int, int>, int>) (i => i.Key), (System.Func<KeyValuePair<int, int>, int>) (i => i.Value)).ToDictionary<IGrouping<int, int>, int, List<int>>((System.Func<IGrouping<int, int>, int>) (l => l.Key), (System.Func<IGrouping<int, int>, List<int>>) (l => l.ToList<int>()));
      this.TraceLeave(0, "GetQueuedBuildIdsByBuildId");
      return (IDictionary<int, List<int>>) dictionary;
    }

    internal override ResultCollection StartPendingBuilds(
      Guid projectId,
      string definitionUri,
      DefinitionTriggerType triggerType,
      TeamFoundationIdentity buildOwner,
      string changesetVersion)
    {
      this.TraceEnter(0, nameof (StartPendingBuilds));
      this.PrepareStoredProcedure("prc_StartPendingBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindItemUriToInt32("@definitionId", definitionUri);
      this.BindInt("@triggerType", (int) triggerType);
      this.BindIdentity("@buildOwner", buildOwner);
      this.BindString("@changesetVersion", changesetVersion, 326, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (StartPendingBuilds));
      return resultCollection;
    }

    internal override ResultCollection StartQueuedBuildsNow(ICollection<QueuedBuild> queuedBuilds)
    {
      this.TraceEnter(0, nameof (StartQueuedBuildsNow));
      this.PrepareStoredProcedure("prc_StartQueuedBuildsNow");
      this.BindQueuedBuildIdDataspaceTable("@queuedBuildTable", (IEnumerable<QueuedBuild>) queuedBuilds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (StartQueuedBuildsNow));
      return resultCollection;
    }

    internal override ResultCollection StopBuildRequest(
      Guid projectId,
      int queuedId,
      TeamFoundationIdentity requestedBy,
      Guid writerId,
      string errorMessage)
    {
      this.TraceEnter(0, "KillBuildRequest");
      this.PrepareStoredProcedure("prc_KillBuildRequest");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@queuedId", queuedId);
      this.BindIdentity("@requestedBy", requestedBy);
      this.BindGuid("@writerId", writerId);
      this.BindString("@errorMessage", errorMessage, -1, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDefinition>((ObjectBinder<BuildDefinition>) new BuildDefinitionBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, "KillBuildRequest");
      return resultCollection;
    }

    internal override ResultCollection UpdateBuilds(IList<QueuedBuildUpdateOptions> updates)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateQueuedBuilds");
      this.BindQueuedBuildUpdateTable("@buildUpdateTable", (IEnumerable<QueuedBuildUpdateOptions>) updates);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuilds(
      IList<QueuedBuildUpdateOptions> updates,
      bool resetQueueTime)
    {
      this.TraceEnter(0, nameof (UpdateBuilds));
      this.PrepareStoredProcedure("prc_UpdateQueuedBuilds");
      this.BindQueuedBuildUpdateTable("@buildUpdateTable", (IEnumerable<QueuedBuildUpdateOptions>) updates);
      this.BindBoolean("@resetQueueTime", resetQueueTime);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<QueuedBuild>((ObjectBinder<QueuedBuild>) new QueuedBuildBinder(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildDetail>((ObjectBinder<BuildDetail>) new BuildDetailBinder2(this.RequestContext, (BuildSqlResourceComponent) this));
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuilds));
      return resultCollection;
    }

    protected virtual SqlParameter BindQueuedBuildIdDataspaceTable(
      string parameterName,
      IEnumerable<QueuedBuild> rows)
    {
      return this.BindTable(parameterName, "typ_BuildIdDataspaceTable", (rows ?? Enumerable.Empty<QueuedBuild>()).Select<QueuedBuild, SqlDataRecord>(new System.Func<QueuedBuild, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindBuildRequestTable(
      string parameterName,
      IEnumerable<BuildRequest> rows)
    {
      return this.BindTable(parameterName, "typ_BuildRequestTableV3", (rows ?? Enumerable.Empty<BuildRequest>()).Select<BuildRequest, SqlDataRecord>(new System.Func<BuildRequest, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindQueuedBuildUpdateTable(
      string parameterName,
      IEnumerable<QueuedBuildUpdateOptions> rows)
    {
      return this.BindTable(parameterName, "typ_QueuedBuildUpdateTableV3", (rows ?? Enumerable.Empty<QueuedBuildUpdateOptions>()).Select<QueuedBuildUpdateOptions, SqlDataRecord>(new System.Func<QueuedBuildUpdateOptions, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(QueuedBuild row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildQueueComponent10.typ_BuildIdDataspaceTable);
      sqlDataRecord.SetInt32(0, row.Id);
      sqlDataRecord.SetInt32(1, this.GetDataspaceId(row.ProjectId));
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildRequest row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildQueueComponent10.typ_BuildRequestTableV3);
      sqlDataRecord.SetInt32(0, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetInt32(1, int.Parse(DBHelper.ExtractDbId(row.BuildDefinitionUri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetByte(2, (byte) row.Priority);
      sqlDataRecord.SetBoolean(3, row.Postponed);
      sqlDataRecord.SetGuid(4, row.BatchId);
      sqlDataRecord.SetInt32(5, (int) row.GetOption);
      sqlDataRecord.SetString(6, row.CustomGetVersion);
      sqlDataRecord.SetInt32(7, row.MaxQueuePosition);
      if (!string.IsNullOrEmpty(row.BuildControllerUri))
        sqlDataRecord.SetInt32(8, int.Parse(DBHelper.ExtractDbId(row.BuildControllerUri), (IFormatProvider) CultureInfo.InvariantCulture));
      else
        sqlDataRecord.SetDBNull(8);
      if (!string.IsNullOrEmpty(row.DropLocation))
        sqlDataRecord.SetString(9, row.DropLocation);
      else
        sqlDataRecord.SetDBNull(9);
      sqlDataRecord.SetString(10, row.RequestedForIdentity.TeamFoundationId.ToString("B"));
      sqlDataRecord.SetString(11, row.RequestedByIdentity.TeamFoundationId.ToString("B"));
      if (!string.IsNullOrEmpty(row.ShelvesetName))
        sqlDataRecord.SetString(12, row.ShelvesetName);
      else
        sqlDataRecord.SetDBNull(12);
      sqlDataRecord.SetInt32(13, (int) row.Reason);
      if (!string.IsNullOrEmpty(row.ProcessParameters))
        sqlDataRecord.SetString(14, row.ProcessParameters);
      else
        sqlDataRecord.SetDBNull(14);
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(QueuedBuildUpdateOptions row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildQueueComponent10.typ_QueuedBuildUpdateTableV3);
      sqlDataRecord.SetInt32(0, this.GetDataspaceId(row.ProjectId));
      sqlDataRecord.SetInt32(1, row.QueueId);
      sqlDataRecord.SetInt32(2, (int) row.Fields);
      if ((row.Fields & QueuedBuildUpdate.BatchId) == QueuedBuildUpdate.BatchId)
        sqlDataRecord.SetGuid(3, row.BatchId);
      else
        sqlDataRecord.SetDBNull(3);
      if ((row.Fields & QueuedBuildUpdate.Postponed) == QueuedBuildUpdate.Postponed)
        sqlDataRecord.SetBoolean(4, row.Postponed);
      else
        sqlDataRecord.SetDBNull(4);
      if ((row.Fields & QueuedBuildUpdate.Priority) == QueuedBuildUpdate.Priority)
        sqlDataRecord.SetByte(5, (byte) row.Priority);
      else
        sqlDataRecord.SetDBNull(5);
      if (!row.RetryOption.Equals((object) QueuedBuildRetryOption.None) && row.Fields.HasFlag((Enum) QueuedBuildUpdate.Requeue))
        sqlDataRecord.SetByte(6, (byte) row.RetryOption);
      else if ((row.Fields & QueuedBuildUpdate.Retry) == QueuedBuildUpdate.Retry && row.Retry)
        sqlDataRecord.SetByte(6, (byte) 1);
      else
        sqlDataRecord.SetDBNull(6);
      return sqlDataRecord;
    }
  }
}
