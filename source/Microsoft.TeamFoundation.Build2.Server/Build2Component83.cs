// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Build2Component83
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class Build2Component83 : Build2Component82
  {
    protected static readonly SqlMetaData[] typ_CheckEventStatusUpdateTable = new SqlMetaData[5]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("CheckEventId", SqlDbType.BigInt),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("Attempts", SqlDbType.TinyInt),
      new SqlMetaData("ScheduledTime", SqlDbType.DateTime2)
    };

    public override BuildCheckEvent AddCheckEvent(
      BuildCheckEvent checkEvent,
      bool securityFixEnabled)
    {
      using (this.TraceScope(method: nameof (AddCheckEvent)))
      {
        this.PrepareStoredProcedure("Build.prc_AddCheckEvent");
        this.BindInt("@dataspaceId", this.GetDataspaceId(checkEvent.ProjectId));
        this.BindInt("@buildId", checkEvent.BuildId);
        this.BindByte("@eventType", (byte) checkEvent.EventType);
        if (checkEvent.Payload != null)
          this.BindBinary("@payload", CheckEventSerializerUtil.Serialize((object) checkEvent.Payload), SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildCheckEvent>((ObjectBinder<BuildCheckEvent>) this.GetCheckEventBinder(securityFixEnabled));
          return resultCollection.GetCurrent<BuildCheckEvent>().Single<BuildCheckEvent>();
        }
      }
    }

    public override CheckEventResults GetCheckEvents(int? maxEvents, bool securityFixEnabled)
    {
      using (this.TraceScope(method: nameof (GetCheckEvents)))
      {
        this.PrepareStoredProcedure("Build.prc_GetCheckEvents");
        this.BindNullableInt32("@maxCheckEvents", maxEvents);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildCheckEvent>((ObjectBinder<BuildCheckEvent>) this.GetCheckEventBinder(securityFixEnabled));
          SqlColumnBinder scheduledTimeBinder = new SqlColumnBinder("ScheduledTime");
          resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => scheduledTimeBinder.GetDateTime(reader))));
          resultCollection.AddBinder<int>((ObjectBinder<int>) new Int32Binder());
          List<BuildCheckEvent> items = resultCollection.GetCurrent<BuildCheckEvent>().Items;
          resultCollection.NextResult();
          DateTime dateTime = resultCollection.GetCurrent<DateTime>().FirstOrDefault<DateTime>();
          resultCollection.NextResult();
          int num = resultCollection.GetCurrent<int>().FirstOrDefault<int>();
          return new CheckEventResults()
          {
            CheckEvents = items,
            NextScheduledEvent = dateTime,
            QueueDepth = num
          };
        }
      }
    }

    public override List<BuildCheckEvent> UpdateCheckEvents(
      IEnumerable<BuildCheckEvent> events,
      bool securityFixEnabled)
    {
      using (this.TraceScope(method: nameof (UpdateCheckEvents)))
      {
        this.PrepareStoredProcedure("Build.prc_UpdateCheckEventStatus");
        this.BindCheckEventStatusUpdateTable("@checkEventUpdateTable", events);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<BuildCheckEvent>((ObjectBinder<BuildCheckEvent>) this.GetCheckEventBinder(securityFixEnabled));
          return resultCollection.GetCurrent<BuildCheckEvent>().Items;
        }
      }
    }

    public override void DeleteCheckEvents(
      CheckEventStatus status,
      DateTime minCreatedTime,
      int? batchSize)
    {
      using (this.TraceScope(method: nameof (DeleteCheckEvents)))
      {
        this.PrepareStoredProcedure("Build.prc_DeleteCheckEvents");
        this.BindByte("@status", (byte) status);
        this.BindDateTime("@minCreatedTime", minCreatedTime);
        this.BindNullableInt("@batchSize", batchSize);
        this.ExecuteNonQuery();
      }
    }

    protected virtual SqlParameter BindCheckEventStatusUpdateTable(
      string parameterName,
      IEnumerable<BuildCheckEvent> buildCheckEventUpdates)
    {
      buildCheckEventUpdates = buildCheckEventUpdates ?? Enumerable.Empty<BuildCheckEvent>();
      System.Func<BuildCheckEvent, SqlDataRecord> selector = (System.Func<BuildCheckEvent, SqlDataRecord>) (buildCheckEventUpdate =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(Build2Component83.typ_CheckEventStatusUpdateTable);
        sqlDataRecord.SetInt32(0, this.GetDataspaceId(buildCheckEventUpdate.ProjectId));
        sqlDataRecord.SetInt64(1, buildCheckEventUpdate.CheckEventId);
        sqlDataRecord.SetByte(2, (byte) buildCheckEventUpdate.Status);
        sqlDataRecord.SetByte(3, buildCheckEventUpdate.Attempts);
        sqlDataRecord.SetDateTime(4, buildCheckEventUpdate.ScheduledTime);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Build.typ_CheckEventStatusUpdateTable", buildCheckEventUpdates.Select<BuildCheckEvent, SqlDataRecord>(selector));
    }
  }
}
