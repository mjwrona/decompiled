// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.RedeployTriggerDeploymentGroupEventSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class RedeployTriggerDeploymentGroupEventSqlComponent2 : 
    RedeployTriggerDeploymentGroupEventSqlComponent
  {
    private static readonly SqlMetaData[] TypInt64Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.BigInt)
    };

    public override IList<RedeployTriggerDeploymentGroupEvent> AddRedeployTriggerDeploymentGroupEvents(
      Guid projectId,
      string eventType,
      int deploymentGroupId,
      IList<RedeployTriggerDeploymentGroupEvent> events)
    {
      this.PrepareStoredProcedure("Release.prc_AddRedeployTriggerDeploymentGroupEvent", projectId);
      this.BindString("@eventType", eventType, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@deploymentGroupId", deploymentGroupId);
      this.BindTable("@redeployTriggerTagUpdateEventTable", "Release.typ_RedeployTriggerTagUpdateEventTable", this.GetTagUpdateEventSqlDataRecords(events));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RedeployTriggerDeploymentGroupEvent>((ObjectBinder<RedeployTriggerDeploymentGroupEvent>) new RedeployTriggerDeploymentGroupEventBinder(this.RequestContext));
        return (IList<RedeployTriggerDeploymentGroupEvent>) resultCollection.GetCurrent<RedeployTriggerDeploymentGroupEvent>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Top is optional")]
    public override IList<RedeployTriggerDeploymentGroupEvent> GetRedeployTriggerDeploymentGroupEvents(
      Guid projectId,
      int top = 50)
    {
      this.PrepareStoredProcedure("Release.prc_GetRedeployTriggerDeploymentGroupEvents", projectId);
      this.BindInt("@top", top);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RedeployTriggerDeploymentGroupEvent>((ObjectBinder<RedeployTriggerDeploymentGroupEvent>) new RedeployTriggerDeploymentGroupEventBinder(this.RequestContext));
        return (IList<RedeployTriggerDeploymentGroupEvent>) resultCollection.GetCurrent<RedeployTriggerDeploymentGroupEvent>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    public override IList<RedeployTriggerDeploymentGroupEvent> UpdateRedeployTriggerDeploymentGroupEventReadyForProcessing(
      Guid projectId,
      int environmentId,
      bool readyForProcessing,
      DateTime lastModifiedOn)
    {
      this.PrepareStoredProcedure("Release.prc_UpdateRedeployTriggerDeploymentGroupEventReadyForProcessing", projectId);
      this.BindInt("@environmentId", environmentId);
      this.BindBoolean("@isReadyForProcessing", readyForProcessing);
      this.BindDateTime("@lastModifiedOn", lastModifiedOn);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RedeployTriggerDeploymentGroupEvent>((ObjectBinder<RedeployTriggerDeploymentGroupEvent>) new RedeployTriggerDeploymentGroupEventBinder(this.RequestContext));
        return (IList<RedeployTriggerDeploymentGroupEvent>) resultCollection.GetCurrent<RedeployTriggerDeploymentGroupEvent>().Items;
      }
    }

    public override void DeleteRedeployTriggerDeploymentGroupEvents(
      Guid projectId,
      IList<long> eventIds)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteRedeployTriggerDeploymentGroupEvent", projectId);
      this.BindTable("@eventIds", "Release.typ_Int64Table", RedeployTriggerDeploymentGroupEventSqlComponent2.BindInt64Rows(eventIds.Distinct<long>()));
      this.ExecuteNonQuery();
    }

    protected IEnumerable<SqlDataRecord> GetTagUpdateEventSqlDataRecords(
      IList<RedeployTriggerDeploymentGroupEvent> deploymentGroupEvents)
    {
      deploymentGroupEvents = deploymentGroupEvents ?? (IList<RedeployTriggerDeploymentGroupEvent>) new List<RedeployTriggerDeploymentGroupEvent>();
      foreach (RedeployTriggerDeploymentGroupEvent deploymentGroupEvent in deploymentGroupEvents.Where<RedeployTriggerDeploymentGroupEvent>((System.Func<RedeployTriggerDeploymentGroupEvent, bool>) (e => e != null)))
      {
        int ordinal = 0;
        SqlDataRecord eventSqlDataRecord = new SqlDataRecord(this.GetDGTagUpdateEventSqlMetadata());
        eventSqlDataRecord.SetInt32(ordinal, deploymentGroupEvent.EnvironmentId);
        int num1;
        eventSqlDataRecord.SetInt32(num1 = ordinal + 1, deploymentGroupEvent.TargetIds.FirstOrDefault<int>());
        string str = string.Empty;
        if (deploymentGroupEvent.TagsInfo != null)
          str = JsonConvert.SerializeObject((object) deploymentGroupEvent.TagsInfo);
        int num2;
        eventSqlDataRecord.SetString(num2 = num1 + 1, string.IsNullOrEmpty(str) ? string.Empty : str);
        yield return eventSqlDataRecord;
      }
    }

    protected virtual SqlMetaData[] GetDGTagUpdateEventSqlMetadata() => new SqlMetaData[3]
    {
      new SqlMetaData("EnvironmentId", SqlDbType.Int),
      new SqlMetaData("TargetId", SqlDbType.Int),
      new SqlMetaData("Tags", SqlDbType.NVarChar, 400L)
    };

    private static IEnumerable<SqlDataRecord> BindInt64Rows(IEnumerable<long> eventIds)
    {
      foreach (long eventId in eventIds)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(RedeployTriggerDeploymentGroupEventSqlComponent2.TypInt64Table);
        sqlDataRecord.SetInt64(0, eventId);
        yield return sqlDataRecord;
      }
    }
  }
}
