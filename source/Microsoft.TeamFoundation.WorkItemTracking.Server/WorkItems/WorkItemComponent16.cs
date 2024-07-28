// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent16
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent16 : WorkItemComponent15
  {
    protected override void BindWorkItemTeamProjectChangeUpdates(
      string parameterName,
      IEnumerable<WorkItemTeamProjectChangeRecord> rows)
    {
      this.BindBasicTvp<WorkItemTeamProjectChangeRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTeamProjectChangeRecord>) new WorkItemComponent16.WorkItemTeamProjectChangeTableRecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected class WorkItemTeamProjectChangeTableRecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTeamProjectChangeRecord>
    {
      protected static readonly SqlMetaData[] TvpMetadata1 = new SqlMetaData[4]
      {
        new SqlMetaData("WorkItemId", SqlDbType.Int),
        new SqlMetaData("Revision", SqlDbType.Int),
        new SqlMetaData("SourceDataspaceId", SqlDbType.Int),
        new SqlMetaData("TargetDataspaceId", SqlDbType.Int)
      };
      private System.Func<Guid, int> m_dataspaceResolver;

      public WorkItemTeamProjectChangeTableRecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override string TypeName => "typ_WorkItemTeamProjectUpdateTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent16.WorkItemTeamProjectChangeTableRecordBinder.TvpMetadata1;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemTeamProjectChangeRecord update)
      {
        record.SetInt32(0, update.WorkItemId);
        record.SetInt32(1, update.Revision);
        record.SetInt32(2, this.m_dataspaceResolver(update.SourceProjectId));
        record.SetInt32(3, this.m_dataspaceResolver(update.TargetProjectId));
      }
    }
  }
}
