// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent9
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent9 : WorkItemComponent8
  {
    protected override SqlParameter BindCoreFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCoreFieldUpdatesRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCoreFieldUpdatesRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>) new WorkItemComponent9.WorkItemCoreFieldValuesTable2RecordBinder(), parameterName, rows);
    }

    protected override void BindUpdateReconciledWorkItemsChangedByColumn(
      IVssIdentity changedByIdentity)
    {
      this.BindGuid("@changedBy", changedByIdentity.Id);
    }

    private class WorkItemCoreFieldValuesTable2RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[15]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("Rev", SqlDbType.Int),
        new SqlMetaData("WorkItemType", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AreaId", SqlDbType.Int),
        new SqlMetaData("IterationId", SqlDbType.Int),
        new SqlMetaData("CreatedBy", SqlDbType.NVarChar, 256L),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("ChangedBy", SqlDbType.NVarChar, 256L),
        new SqlMetaData("ChangedDate", SqlDbType.DateTime),
        new SqlMetaData("State", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Reason", SqlDbType.NVarChar, 256L),
        new SqlMetaData("AssignedTo", SqlDbType.NVarChar, 256L),
        new SqlMetaData("CreatedBy_TeamFoundationId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("ChangedBy_TeamFoundationId", SqlDbType.UniqueIdentifier),
        new SqlMetaData("AssignedTo_TeamFoundationId", SqlDbType.UniqueIdentifier)
      };

      public override string TypeName => "typ_WorkItemCoreFieldValuesTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent9.WorkItemCoreFieldValuesTable2RecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCoreFieldUpdatesRecord workItem)
      {
        record.SetInt32(0, workItem.Id);
        record.SetInt32(1, workItem.Revision);
        record.SetString(2, workItem.WorkItemType);
        record.SetInt32(3, workItem.AreaId);
        record.SetInt32(4, workItem.IterationId);
        record.SetNullableString(5, workItem.CreatedBy);
        SqlDateTime minValue;
        if (workItem.Revision == 1)
        {
          WorkItemTrackingSqlDataRecord trackingSqlDataRecord = record;
          minValue = SqlDateTime.MinValue;
          DateTime dateTime = minValue.Value;
          trackingSqlDataRecord.SetDateTime(6, dateTime);
        }
        else
          record.SetDBNull(6);
        record.SetString(7, workItem.ChangedBy);
        WorkItemTrackingSqlDataRecord trackingSqlDataRecord1 = record;
        minValue = SqlDateTime.MinValue;
        DateTime dateTime1 = minValue.Value;
        trackingSqlDataRecord1.SetDateTime(8, dateTime1);
        record.SetNullableString(9, workItem.State);
        record.SetNullableString(10, workItem.Reason);
        record.SetNullableString(11, workItem.AssignedTo);
        record.SetNullableGuid(12, new Guid?());
        record.SetNullableGuid(13, new Guid?());
        record.SetNullableGuid(14, new Guid?());
      }
    }
  }
}
