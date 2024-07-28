// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildEventComponent
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
  internal class BuildEventComponent : BuildSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<BuildEventComponent>(1),
      (IComponentCreator) new ComponentCreator<BuildEventComponent2>(2),
      (IComponentCreator) new ComponentCreator<BuildEventComponent3>(3),
      (IComponentCreator) new ComponentCreator<BuildEventComponent4>(4),
      (IComponentCreator) new ComponentCreator<BuildEventComponent5>(5),
      (IComponentCreator) new ComponentCreator<BuildEventComponent6>(6),
      (IComponentCreator) new ComponentCreator<BuildEventComponent7>(7)
    }, "BuildEvent", "Build");
    protected static readonly SqlMetaData[] typ_BuildEventStatusUpdateTable = new SqlMetaData[2]
    {
      new SqlMetaData("BuildEventId", SqlDbType.BigInt),
      new SqlMetaData("Status", SqlDbType.TinyInt)
    };

    public virtual BuildChangeEvent AddBuildEvent(
      Guid projectId,
      int buildId,
      BuildEventType eventType)
    {
      this.TraceEnter(0, nameof (AddBuildEvent));
      this.PrepareStoredProcedure("Build.prc_AddBuildEvent");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.BindInt("@buildId", buildId);
      this.BindByte("@eventType", (byte) eventType);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildChangeEvent>(this.GetBuildEventBinder());
        BuildChangeEvent buildChangeEvent = resultCollection.GetCurrent<BuildChangeEvent>().FirstOrDefault<BuildChangeEvent>();
        this.TraceLeave(0, nameof (AddBuildEvent));
        return buildChangeEvent;
      }
    }

    public virtual BuildEventResults GetBuildEvents(BuildEventStatus status, int? maxBuildEvents)
    {
      this.TraceEnter(0, nameof (GetBuildEvents));
      this.PrepareStoredProcedure("Build.prc_GetBuildEvents");
      this.BindByte("@eventType", (byte) 0);
      this.BindByte("@status", (byte) status);
      this.BindNullableInt("@maxBuildEvents", maxBuildEvents);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildChangeEvent>(this.GetBuildEventBinder());
        List<BuildChangeEvent> items = resultCollection.GetCurrent<BuildChangeEvent>().Items;
        this.TraceLeave(0, nameof (GetBuildEvents));
        return new BuildEventResults()
        {
          BuildEvents = items
        };
      }
    }

    public virtual void DeleteBuildEvents(
      BuildEventStatus status,
      DateTime minCreatedTime,
      int? batchSize)
    {
      this.TraceEnter(0, nameof (DeleteBuildEvents));
      this.PrepareStoredProcedure("Build.prc_DeleteBuildEvents");
      this.BindByte("@eventType", (byte) 0);
      this.BindByte("@status", (byte) status);
      this.BindDateTime("@minCreatedTime", minCreatedTime);
      this.BindNullableInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildEvents));
    }

    public virtual List<BuildChangeEvent> UpdateBuildEventsStatus(
      IEnumerable<BuildChangeEvent> buildEventStatusUpdates)
    {
      this.TraceEnter(0, nameof (UpdateBuildEventsStatus));
      this.PrepareStoredProcedure("Build.prc_UpdateBuildEventStatus");
      this.BindBuildEventStatusUpdateTable("@buildEventUpdateTable", buildEventStatusUpdates);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildChangeEvent>(this.GetBuildEventBinder());
        List<BuildChangeEvent> items = resultCollection.GetCurrent<BuildChangeEvent>().Items;
        this.TraceLeave(0, nameof (UpdateBuildEventsStatus));
        return items;
      }
    }

    protected virtual SqlParameter BindBuildEventStatusUpdateTable(
      string parameterName,
      IEnumerable<BuildChangeEvent> buildEventStatusUpdates)
    {
      buildEventStatusUpdates = buildEventStatusUpdates ?? Enumerable.Empty<BuildChangeEvent>();
      System.Func<BuildChangeEvent, SqlDataRecord> selector = (System.Func<BuildChangeEvent, SqlDataRecord>) (buildEventStatusUpdate =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(BuildEventComponent.typ_BuildEventStatusUpdateTable);
        sqlDataRecord.SetInt64(0, buildEventStatusUpdate.BuildEventId);
        sqlDataRecord.SetByte(1, (byte) buildEventStatusUpdate.Status);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Build.typ_BuildEventStatusUpdateTable", buildEventStatusUpdates.Select<BuildChangeEvent, SqlDataRecord>(selector));
    }

    public virtual long GetBuildEventQueueData() => -1;

    protected virtual ObjectBinder<BuildChangeEvent> GetBuildEventBinder() => (ObjectBinder<BuildChangeEvent>) new BuildEventBinder((BuildSqlComponentBase) this);
  }
}
