// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.OrchestrationComponent2
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration.Server
{
  internal class OrchestrationComponent2 : OrchestrationComponent
  {
    private static readonly SqlMetaData[] typ_OrchestrationMessageIdTable = new SqlMetaData[1]
    {
      new SqlMetaData("MessageId", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_OrchestrationMessageTable = new SqlMetaData[5]
    {
      new SqlMetaData("MessageId", SqlDbType.Int),
      new SqlMetaData("SessionId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ScheduledDeliveryTime", SqlDbType.DateTime),
      new SqlMetaData("CompressionType", SqlDbType.TinyInt),
      new SqlMetaData("Content", SqlDbType.VarBinary, -1L)
    };
    private static readonly SqlMetaData[] typ_OrchestrationSessionTable = new SqlMetaData[4]
    {
      new SqlMetaData("SessionId", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Completed", SqlDbType.Bit),
      new SqlMetaData("CompressionType", SqlDbType.TinyInt),
      new SqlMetaData("State", SqlDbType.VarBinary, -1L)
    };
    private static readonly SqlMetaData[] typ_OrchestrationStateTable = new SqlMetaData[13]
    {
      new SqlMetaData("InstanceIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ExecutionIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ParentInstanceIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("ParentExecutionIdentifier", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Version", SqlDbType.NVarChar, 260L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("CreatedOn", SqlDbType.DateTime),
      new SqlMetaData("CompletedOn", SqlDbType.DateTime),
      new SqlMetaData("Size", SqlDbType.BigInt),
      new SqlMetaData("CompressedSize", SqlDbType.BigInt),
      new SqlMetaData("Output", SqlDbType.NVarChar, -1L),
      new SqlMetaData("State", SqlDbType.NVarChar, -1L)
    };
    private int m_messageId;

    public override async Task<int> CleanupMessageContentAsync(int sqlCommandTimeout)
    {
      OrchestrationComponent2 orchestrationComponent2 = this;
      orchestrationComponent2.TraceEnter(0, nameof (CleanupMessageContentAsync));
      orchestrationComponent2.PrepareStoredProcedure("prc_CleanupOrchestrationMessageContent", sqlCommandTimeout);
      int num = await orchestrationComponent2.ExecuteNonQueryAsync();
      orchestrationComponent2.TraceLeave(0, nameof (CleanupMessageContentAsync));
      return num;
    }

    public override OrchestrationHubDescription CreateHub(OrchestrationHubDescription description)
    {
      this.PrepareStoredProcedure("prc_AddOrchestrationHub");
      this.BindString("@hubType", description.HubType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@hubName", description.HubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindByte("@compressionStyle", (byte) description.CompressionSettings.Style);
      this.BindNullableInt("@compressionThreshold", description.CompressionSettings.ThresholdInBytes, 0);
      this.BindInt("@maxConcurrentActivities", description.MaxConcurrentActivities);
      this.BindInt("@maxConcurrentOrchestrations", description.MaxConcurrentOrchestrations);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationHubDescription>((ObjectBinder<OrchestrationHubDescription>) new OrchestrationHubDescriptionBinder());
        return resultCollection.GetCurrent<OrchestrationHubDescription>().First<OrchestrationHubDescription>();
      }
    }

    public override void CreateOrchestrationSession(
      OrchestrationHubDescription hub,
      string sessionId,
      IEnumerable<OrchestrationMessage> messages)
    {
      this.PrepareStoredProcedure("prc_CreateOrchestrationSession");
      this.BindString("@hubName", hub.HubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@sessionIdentifier", sessionId, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindOrchestrationMessageTable("@orchestrationMessages", messages);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<OrchestrationHubDescription> GetHubs(string hubName)
    {
      this.PrepareStoredProcedure("prc_GetOrchestrationHub");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationHubDescription>((ObjectBinder<OrchestrationHubDescription>) new OrchestrationHubDescriptionBinder());
        return (IEnumerable<OrchestrationHubDescription>) resultCollection.GetCurrent<OrchestrationHubDescription>().Items;
      }
    }

    public override async Task<IList<OrchestrationMessage>> GetActivityMessagesAsync(
      string hubName,
      string dispatcherType,
      int maxMessageCount,
      long? lastMessageId)
    {
      OrchestrationComponent2 orchestrationComponent2 = this;
      orchestrationComponent2.PrepareStoredProcedure("prc_GetRunnableActivityMessages");
      orchestrationComponent2.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent2.BindInt("@maxMessageCount", maxMessageCount);
      IList<OrchestrationMessage> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await orchestrationComponent2.ExecuteReaderAsync(), orchestrationComponent2.ProcedureName, orchestrationComponent2.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationMessage>((ObjectBinder<OrchestrationMessage>) new OrchestrationMessageBinder());
        items = (IList<OrchestrationMessage>) resultCollection.GetCurrent<OrchestrationMessage>().Items;
      }
      return items;
    }

    public override async Task<IList<RunnableOrchestrationSession>> GetRunnableSessionsAsync(
      string hubName,
      OrchestrationHubDispatcherType dispatcherType,
      int maxSessionCount)
    {
      OrchestrationComponent2 orchestrationComponent2 = this;
      orchestrationComponent2.PrepareStoredProcedure("prc_GetRunnableOrchestrationSessions");
      orchestrationComponent2.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      orchestrationComponent2.BindByte("@dispatcherType", (byte) dispatcherType);
      orchestrationComponent2.BindInt("@maxSessionCount", maxSessionCount);
      IList<RunnableOrchestrationSession> list;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) await orchestrationComponent2.ExecuteReaderAsync(), orchestrationComponent2.ProcedureName, orchestrationComponent2.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationSession>(orchestrationComponent2.GetOrchestrationSessionBinder());
        resultCollection.AddBinder<OrchestrationMessage>((ObjectBinder<OrchestrationMessage>) new OrchestrationMessageBinder());
        Dictionary<string, RunnableOrchestrationSession> dictionary = resultCollection.GetCurrent<OrchestrationSession>().Items.ToDictionary<OrchestrationSession, string, RunnableOrchestrationSession>((System.Func<OrchestrationSession, string>) (x => x.SessionId), (System.Func<OrchestrationSession, RunnableOrchestrationSession>) (x => new RunnableOrchestrationSession()
        {
          Session = x
        }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        resultCollection.NextResult();
        foreach (OrchestrationMessage orchestrationMessage in resultCollection.GetCurrent<OrchestrationMessage>().Items)
        {
          RunnableOrchestrationSession orchestrationSession;
          if (dictionary.TryGetValue(orchestrationMessage.SessionId, out orchestrationSession))
            orchestrationSession.Messages.Add(orchestrationMessage);
        }
        list = (IList<RunnableOrchestrationSession>) dictionary.Values.ToList<RunnableOrchestrationSession>();
      }
      return list;
    }

    public override IList<OrchestrationState> GetOrchestrationState(
      string hubName,
      string instanceId,
      string executionId)
    {
      this.PrepareStoredProcedure("prc_GetOrchestrationState");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@instanceIdentifier", instanceId, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@executionIdentifier", executionId, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<OrchestrationState>((ObjectBinder<OrchestrationState>) new OrchestrationStateBinder());
        return (IList<OrchestrationState>) resultCollection.GetCurrent<OrchestrationState>().ToArray<OrchestrationState>();
      }
    }

    public override void InstallData()
    {
      this.PrepareStoredProcedure("prc_InstallOrchestrationData");
      this.ExecuteNonQuery();
    }

    public override OrchestrationHubDescription UpdateHub(string hubName, string newHubName) => throw new ServiceVersionNotSupportedException("Orchestration", 2, 2);

    public override Task UpdateOrchestrationSessionAsync(
      string hubName,
      OrchestrationHubDispatcherType dispatcherType,
      IEnumerable<OrchestrationSession> sessions = null,
      IEnumerable<long> messagesToDelete = null,
      IEnumerable<OrchestrationMessage> activityMessages = null,
      IEnumerable<OrchestrationMessage> orchestrationMessages = null,
      IEnumerable<OrchestrationState> instances = null,
      bool ensureSessionExists = false,
      bool alwaysSaveToHistory = false)
    {
      this.PrepareStoredProcedure("prc_UpdateOrchestrationSession");
      this.BindString("@hubName", hubName, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindByte("@dispatcherType", (byte) dispatcherType);
      this.BindOrchestrationSessionTable("@sessionUpdates", sessions);
      this.BindOrchestrationMessageIdTable("@messagesToDelete", messagesToDelete);
      this.BindOrchestrationMessageTable("@activityMessages", activityMessages);
      this.BindOrchestrationMessageTable("@orchestrationMessages", orchestrationMessages);
      this.BindOrchestrationStateTable("@stateUpdates", instances);
      return (Task) this.ExecuteNonQueryAsync();
    }

    protected virtual SqlParameter BindOrchestrationMessageIdTable(
      string parameterName,
      IEnumerable<long> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationMessageIdTable", (rows ?? Enumerable.Empty<long>()).Select<long, SqlDataRecord>(new System.Func<long, SqlDataRecord>(this.ConvertToSqlRecord)));
    }

    protected virtual SqlParameter BindOrchestrationMessageTable(
      string parameterName,
      IEnumerable<OrchestrationMessage> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationMessageTable", (rows ?? Enumerable.Empty<OrchestrationMessage>()).Select<OrchestrationMessage, SqlDataRecord>(new System.Func<OrchestrationMessage, SqlDataRecord>(this.ConvertToSqlRecord)));
    }

    protected virtual SqlParameter BindOrchestrationSessionTable(
      string parameterName,
      IEnumerable<OrchestrationSession> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationSessionTable", (rows ?? Enumerable.Empty<OrchestrationSession>()).Select<OrchestrationSession, SqlDataRecord>(new System.Func<OrchestrationSession, SqlDataRecord>(this.ConvertToSqlRecord)));
    }

    protected virtual SqlParameter BindOrchestrationStateTable(
      string parameterName,
      IEnumerable<OrchestrationState> rows)
    {
      return this.BindTable(parameterName, "typ_OrchestrationStateTable", (rows ?? Enumerable.Empty<OrchestrationState>()).Select<OrchestrationState, SqlDataRecord>(new System.Func<OrchestrationState, SqlDataRecord>(this.ConvertToSqlRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlRecord(long row)
    {
      SqlDataRecord sqlRecord = new SqlDataRecord(OrchestrationComponent2.typ_OrchestrationMessageIdTable);
      sqlRecord.SetInt64(0, row);
      return sqlRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlRecord(OrchestrationMessage row)
    {
      SqlDataRecord record = new SqlDataRecord(OrchestrationComponent2.typ_OrchestrationMessageTable);
      record.SetInt32(0, ++this.m_messageId);
      record.SetString(1, row.SessionId, BindStringBehavior.EmptyStringToNull);
      if (!row.ScheduledDeliveryTime.HasValue)
        record.SetDBNull(2);
      else
        record.SetDateTime(2, row.ScheduledDeliveryTime.Value);
      record.SetByte(3, (byte) row.CompressionType);
      record.SetBytes(4, 0L, row.Content, 0, row.Content.Length);
      return record;
    }

    protected virtual SqlDataRecord ConvertToSqlRecord(OrchestrationSession row)
    {
      SqlDataRecord sqlRecord = new SqlDataRecord(OrchestrationComponent2.typ_OrchestrationSessionTable);
      sqlRecord.SetString(0, row.SessionId);
      sqlRecord.SetBoolean(1, row.IsComplete);
      if (!row.IsComplete)
      {
        sqlRecord.SetByte(2, (byte) row.CompressionType);
        if (row.State == null)
          sqlRecord.SetDBNull(3);
        else
          sqlRecord.SetBytes(3, 0L, row.State, 0, row.State.Length);
      }
      return sqlRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlRecord(OrchestrationState row)
    {
      SqlDataRecord record = new SqlDataRecord(OrchestrationComponent2.typ_OrchestrationStateTable);
      record.SetString(0, row.OrchestrationInstance.InstanceId, BindStringBehavior.Unchanged);
      record.SetString(1, row.OrchestrationInstance.ExecutionId, BindStringBehavior.Unchanged);
      if (row.ParentInstance != null)
      {
        record.SetString(2, row.ParentInstance.OrchestrationInstance.InstanceId, BindStringBehavior.Unchanged);
        record.SetString(3, row.ParentInstance.OrchestrationInstance.ExecutionId, BindStringBehavior.Unchanged);
      }
      else
      {
        record.SetDBNull(2);
        record.SetDBNull(3);
      }
      record.SetString(4, row.Name);
      record.SetString(5, row.Version);
      record.SetByte(6, (byte) row.OrchestrationStatus);
      record.SetDateTime(7, row.CreatedTime);
      if (row.CompletedTime.HasValue)
        record.SetDateTime(8, row.CompletedTime.Value);
      else
        record.SetDBNull(8);
      record.SetInt64(9, row.Size);
      record.SetInt64(10, row.CompressedSize);
      record.SetString(11, row.Output, BindStringBehavior.EmptyStringToNull);
      record.SetString(12, row.Status, BindStringBehavior.EmptyStringToNull);
      return record;
    }
  }
}
