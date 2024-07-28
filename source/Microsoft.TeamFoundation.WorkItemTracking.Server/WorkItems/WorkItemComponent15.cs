// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent15
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

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent15 : WorkItemComponent14
  {
    protected override SqlParameter BindCoreFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCoreFieldUpdatesRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCoreFieldUpdatesRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>) new WorkItemComponent15.WorkItemCoreFieldValuesTable5RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected override WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset> GetWorkItemDataSetBinder(
      bool bindTitle,
      bool bindCountFields,
      IdentityDisplayType identityDisplayType)
    {
      return (WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset>) new WorkItemComponent.WorkItemDatasetBinder4<WorkItemDataset>(bindTitle, bindCountFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier));
    }

    protected override WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues> GetWorkItemFieldValuesBinder(
      IEnumerable<int> wideTableFields,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree)
    {
      return (WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValuesBinder4<WorkItemFieldValues>(wideTableFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), disableProjectionLevelThree);
    }

    protected override void ReadTipValueForWorkItemRevision(
      IDataReader reader,
      Dictionary<int, IEnumerable<WorkItemFieldValues>> fieldValuesRecordsMap)
    {
      Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
      Dictionary<int, bool> dictionary2 = new Dictionary<int, bool>();
      while (reader.Read())
      {
        dictionary1.Add(reader.GetInt32(0), reader.GetInt32(1));
        dictionary2.Add(reader.GetInt32(0), reader.GetBoolean(2));
      }
      foreach (KeyValuePair<int, IEnumerable<WorkItemFieldValues>> fieldValuesRecords in fieldValuesRecordsMap)
      {
        int num;
        bool flag;
        if (dictionary1.TryGetValue(fieldValuesRecords.Key, out num) && dictionary2.TryGetValue(fieldValuesRecords.Key, out flag))
        {
          foreach (WorkItemFieldValues workItemFieldValues in fieldValuesRecords.Value)
          {
            workItemFieldValues.LatestAreaId = num;
            workItemFieldValues.LatestIsDeleted = flag;
          }
        }
      }
    }

    private class WorkItemCoreFieldValuesTable5RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[14]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("DataspaceId", SqlDbType.Int),
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
        new SqlMetaData("IsDeleted", SqlDbType.Bit)
      };
      private System.Func<Guid, int> m_dataspaceResolver;

      public override string TypeName => "typ_WorkItemCoreFieldValuesTable5";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent15.WorkItemCoreFieldValuesTable5RecordBinder.s_metadata;

      public WorkItemCoreFieldValuesTable5RecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCoreFieldUpdatesRecord workItem)
      {
        record.SetInt32(0, workItem.Id);
        record.SetInt32(1, this.m_dataspaceResolver(workItem.ProjectId));
        record.SetInt32(2, workItem.Revision);
        if (string.IsNullOrWhiteSpace(workItem.WorkItemType))
          throw new WorkItemFieldValueEmptyException("WorkItemType", workItem.ProjectId, workItem.Id);
        record.SetString(3, workItem.WorkItemType);
        record.SetInt32(4, workItem.AreaId);
        record.SetInt32(5, workItem.IterationId);
        record.SetNullableString(6, workItem.CreatedBy);
        if (workItem.Revision == 1)
          record.SetDateTime(7, workItem.CreatedDate);
        else
          record.SetDBNull(7);
        if (string.IsNullOrWhiteSpace(workItem.ChangedBy))
          throw new WorkItemFieldValueEmptyException("ChangedBy", workItem.ProjectId, workItem.Id);
        record.SetString(8, workItem.ChangedBy);
        record.SetDateTime(9, workItem.ChangedDate);
        record.SetNullableString(10, workItem.State);
        record.SetNullableString(11, workItem.Reason);
        record.SetNullableString(12, workItem.AssignedTo);
        record.SetBoolean(13, workItem.IsDeleted);
      }
    }
  }
}
