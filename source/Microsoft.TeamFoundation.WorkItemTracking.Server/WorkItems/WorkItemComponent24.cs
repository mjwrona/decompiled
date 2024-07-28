// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent24
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
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent24 : WorkItemComponent23
  {
    protected override void BindWorkItemTeamProjectChangeUpdates(
      string parameterName,
      IEnumerable<WorkItemTeamProjectChangeRecord> rows)
    {
      this.BindBasicTvp<WorkItemTeamProjectChangeRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTeamProjectChangeRecord>) new WorkItemComponent24.WorkItemTeamProjectChangeTable2RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId), new Func<Guid, string, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected class WorkItemTeamProjectChangeTable2RecordBinder : 
      WorkItemComponent16.WorkItemTeamProjectChangeTableRecordBinder
    {
      protected static readonly SqlMetaData[] TvpMetadata2 = ((IEnumerable<SqlMetaData>) WorkItemComponent16.WorkItemTeamProjectChangeTableRecordBinder.TvpMetadata1).Concat<SqlMetaData>((IEnumerable<SqlMetaData>) new SqlMetaData[2]
      {
        new SqlMetaData("MentionsSourceDataspaceId", SqlDbType.Int),
        new SqlMetaData("MentionsTargetDataspaceId", SqlDbType.Int)
      }).ToArray<SqlMetaData>();
      private Func<Guid, string, int> m_mentionsDataspaceResolver;

      public WorkItemTeamProjectChangeTable2RecordBinder(
        System.Func<Guid, int> dataspaceResolver,
        Func<Guid, string, int> mentionsDataspaceResolver)
        : base(dataspaceResolver)
      {
        this.m_mentionsDataspaceResolver = mentionsDataspaceResolver;
      }

      public override string TypeName => "typ_WorkItemTeamProjectUpdateTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent24.WorkItemTeamProjectChangeTable2RecordBinder.TvpMetadata2;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemTeamProjectChangeRecord update)
      {
        base.SetRecordValues(record, update);
        record.SetInt32(4, this.m_mentionsDataspaceResolver(update.SourceProjectId, "Default"));
        record.SetInt32(5, this.m_mentionsDataspaceResolver(update.TargetProjectId, "Default"));
      }
    }
  }
}
