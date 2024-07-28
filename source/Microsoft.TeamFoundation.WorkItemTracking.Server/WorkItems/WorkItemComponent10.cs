// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent10
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
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent10 : WorkItemComponent9
  {
    protected override SqlParameter BindCoreFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCoreFieldUpdatesRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCoreFieldUpdatesRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>) new WorkItemComponent10.WorkItemCoreFieldValuesTable3RecordBinder(), parameterName, rows);
    }

    public override WorkItemUpdateResultSet ValidatePendingSetMembershipChecks(
      IEnumerable<PendingSetMembershipCheckRecord> pendingSetMembershipChecks)
    {
      if (!pendingSetMembershipChecks.Any<PendingSetMembershipCheckRecord>())
        return new WorkItemUpdateResultSet()
        {
          Success = true,
          FailureReason = "Default"
        };
      this.PrepareStoredProcedure("prc_iiCheckSetMemberships");
      this.BindPendingSetMembershipChecks("@pendingSetMembershipChecks", pendingSetMembershipChecks);
      return this.ExecuteUnknown<WorkItemUpdateResultSet>((System.Func<IDataReader, WorkItemUpdateResultSet>) (reader =>
      {
        WorkItemUpdateResultSet itemUpdateResultSet = new WorkItemUpdateResultSet()
        {
          Success = false,
          FailureReason = "Default"
        };
        itemUpdateResultSet.SetMembershipCheckResults = (IEnumerable<PendingSetMembershipCheckResultRecord>) WorkItemTrackingResourceComponent.Bind<PendingSetMembershipCheckResultRecord>(reader, (System.Func<IDataReader, PendingSetMembershipCheckResultRecord>) (r => new PendingSetMembershipCheckResultRecord()
        {
          WorkItemId = r.GetInt32(0),
          FieldId = r.GetInt32(1)
        })).ToArray<PendingSetMembershipCheckResultRecord>();
        itemUpdateResultSet.Success = !itemUpdateResultSet.SetMembershipCheckResults.Any<PendingSetMembershipCheckResultRecord>();
        if (!itemUpdateResultSet.Success)
          itemUpdateResultSet.FailureReason = "PendingSetMembershipChecks present in updateDataset";
        return itemUpdateResultSet;
      }));
    }

    private class WorkItemCoreFieldValuesTable3RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[12]
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
        new SqlMetaData("AssignedTo", SqlDbType.NVarChar, 256L)
      };

      public override string TypeName => "typ_WorkItemCoreFieldValuesTable3";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent10.WorkItemCoreFieldValuesTable3RecordBinder.s_metadata;

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
      }
    }
  }
}
