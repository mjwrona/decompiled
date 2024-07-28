// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.RedeployTriggerDeploymentGroupEventSqlComponent3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class RedeployTriggerDeploymentGroupEventSqlComponent3 : 
    RedeployTriggerDeploymentGroupEventSqlComponent2
  {
    public override IList<RedeployTriggerDeploymentGroupEvent> AddRedeployTriggerDeploymentGroupEvents(
      Guid projectId,
      string eventType,
      int deploymentGroupId,
      IList<RedeployTriggerDeploymentGroupEvent> events)
    {
      this.PrepareStoredProcedure("Release.prc_AddRedeployTriggerDeploymentGroupEvent", projectId);
      this.BindString("@eventType", eventType, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@deploymentGroupId", deploymentGroupId);
      this.BindTable("@redeployTriggerTagUpdateEventTable", "Release.typ_RedeployTriggerTagUpdateEventTableV2", this.GetTagUpdateEventSqlDataRecords(events));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<RedeployTriggerDeploymentGroupEvent>((ObjectBinder<RedeployTriggerDeploymentGroupEvent>) new RedeployTriggerDeploymentGroupEventBinder(this.RequestContext));
        return (IList<RedeployTriggerDeploymentGroupEvent>) resultCollection.GetCurrent<RedeployTriggerDeploymentGroupEvent>().Items;
      }
    }

    protected override SqlMetaData[] GetDGTagUpdateEventSqlMetadata() => new SqlMetaData[3]
    {
      new SqlMetaData("EnvironmentId", SqlDbType.Int),
      new SqlMetaData("TargetId", SqlDbType.Int),
      new SqlMetaData("Tags", SqlDbType.NVarChar, 4000L)
    };
  }
}
