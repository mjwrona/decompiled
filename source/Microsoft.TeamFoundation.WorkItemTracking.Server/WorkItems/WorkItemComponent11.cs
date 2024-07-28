// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent11
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent11 : WorkItemComponent10
  {
    protected override SqlParameter BindCoreFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCoreFieldUpdatesRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCoreFieldUpdatesRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>) new WorkItemComponent11.WorkItemCoreFieldValuesTable4RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected override SqlParameter BindCustomFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCustomFieldUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCustomFieldUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCustomFieldUpdateRecord>) new WorkItemComponent11.WorkItemFieldUpdateTable2RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected override SqlParameter BindWorkItemResourceLinkUpdates(
      string parameterName,
      IEnumerable<WorkItemResourceLinkUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemResourceLinkUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemResourceLinkUpdateRecord>) new WorkItemComponent11.WorkItemResourceLinkUpdateTable3RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected override SqlParameter BindWorkItemLinkUpdates(
      string parameterName,
      IEnumerable<WorkItemLinkUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemLinkUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemLinkUpdateRecord>) new WorkItemComponent11.WorkItemLinkUpdateTable2RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected override SqlParameter BindWorkItemTextFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemTextFieldUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemTextFieldUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTextFieldUpdateRecord>) new WorkItemComponent11.WorkItemTextFieldUpdateTable2RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    private class WorkItemCoreFieldValuesTable4RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCoreFieldUpdatesRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[13]
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
        new SqlMetaData("AssignedTo", SqlDbType.NVarChar, 256L)
      };
      private System.Func<Guid, int> m_dataspaceResolver;

      public override string TypeName => "typ_WorkItemCoreFieldValuesTable4";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent11.WorkItemCoreFieldValuesTable4RecordBinder.s_metadata;

      public WorkItemCoreFieldValuesTable4RecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCoreFieldUpdatesRecord workItem)
      {
        record.SetInt32(0, workItem.Id);
        record.SetInt32(1, this.m_dataspaceResolver(workItem.ProjectId));
        record.SetInt32(2, workItem.Revision);
        record.SetString(3, workItem.WorkItemType);
        record.SetInt32(4, workItem.AreaId);
        record.SetInt32(5, workItem.IterationId);
        record.SetNullableString(6, workItem.CreatedBy);
        if (workItem.Revision == 1)
          record.SetDateTime(7, workItem.CreatedDate);
        else
          record.SetDBNull(7);
        record.SetString(8, workItem.ChangedBy);
        record.SetDateTime(9, workItem.ChangedDate);
        record.SetNullableString(10, workItem.State);
        record.SetNullableString(11, workItem.Reason);
        record.SetNullableString(12, workItem.AssignedTo);
      }
    }

    protected class WorkItemFieldUpdateTable2RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCustomFieldUpdateRecord>
    {
      protected static readonly SqlMetaData[] TvpMetadata2 = new SqlMetaData[10]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("DataspaceId", SqlDbType.Int),
        new SqlMetaData("FieldId", SqlDbType.Int),
        new SqlMetaData("TrendOption", SqlDbType.TinyInt),
        new SqlMetaData("IntValue", SqlDbType.Int),
        new SqlMetaData("FloatValue", SqlDbType.Float),
        new SqlMetaData("DateTimeValue", SqlDbType.DateTime),
        new SqlMetaData("GuidValue", SqlDbType.UniqueIdentifier),
        new SqlMetaData("BitValue", SqlDbType.Bit),
        new SqlMetaData("StringValue", SqlDbType.NVarChar, 256L)
      };
      private System.Func<Guid, int> m_dataspaceResolver;

      public override string TypeName => "typ_WorkItemFieldUpdateTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent11.WorkItemFieldUpdateTable2RecordBinder.TvpMetadata2;

      public WorkItemFieldUpdateTable2RecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCustomFieldUpdateRecord update)
      {
        record.SetInt32(0, update.WorkItemId);
        record.SetInt32(1, this.m_dataspaceResolver(update.ProjectId));
        record.SetInt32(2, update.Field.FieldId);
        for (int ordinal = 4; ordinal <= 9; ++ordinal)
          record.SetDBNull(ordinal);
        object obj1 = update.Value;
        if (obj1 is TrendDataValue)
        {
          object obj2 = obj1 != TrendDataValue.Increase ? (obj1 != TrendDataValue.Decrease ? (object) 0 : (object) -1) : (object) 1;
          record.SetByte(3, (byte) (update.TrendOption | TrendDataUpdateOption.ImplicitTrendDataUpdate));
          record.SetInt32(4, (int) obj2);
        }
        else
        {
          record.SetByte(3, (byte) update.TrendOption);
          try
          {
            if (obj1 == null)
              return;
            switch (update.Field.SqlType)
            {
              case SqlDbType.Bit:
                if (obj1 is bool flag)
                {
                  record.SetBoolean(8, flag);
                  break;
                }
                record.SetBoolean(8, Convert.ToBoolean(obj1));
                break;
              case SqlDbType.DateTime:
                if (obj1 is DateTime dateTime)
                {
                  record.SetDateTime(6, dateTime);
                  break;
                }
                record.SetDateTime(6, DateTime.SpecifyKind(Convert.ToDateTime(obj1), DateTimeKind.Utc));
                break;
              case SqlDbType.Float:
                if (obj1 is double num1)
                {
                  record.SetDouble(5, num1);
                  break;
                }
                record.SetDouble(5, Convert.ToDouble(obj1));
                break;
              case SqlDbType.Int:
                if (obj1 is int num2)
                {
                  record.SetInt32(4, num2);
                  break;
                }
                record.SetInt32(4, Convert.ToInt32(obj1));
                break;
              case SqlDbType.UniqueIdentifier:
                if (obj1 is Guid guid)
                {
                  record.SetGuid(7, guid);
                  break;
                }
                record.SetGuid(7, new Guid(obj1.ToString()));
                break;
              default:
                if (obj1 is string)
                {
                  record.SetString(9, (string) obj1);
                  break;
                }
                record.SetString(9, Convert.ToString(obj1, (IFormatProvider) CultureInfo.InvariantCulture));
                break;
            }
          }
          catch (NullReferenceException ex)
          {
            throw new TeamFoundationServiceException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.FailedSetRecordValues((object) update.Field.ReferenceName, update.Value), (Exception) ex);
          }
        }
      }
    }

    private class WorkItemResourceLinkUpdateTable3RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemResourceLinkUpdateRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[12]
      {
        new SqlMetaData("Order", SqlDbType.Int),
        new SqlMetaData("UpdateType", SqlDbType.TinyInt),
        new SqlMetaData("SourceId", SqlDbType.Int),
        new SqlMetaData("DataspaceId", SqlDbType.Int),
        new SqlMetaData("ResourceType", SqlDbType.Int),
        new SqlMetaData("ResourceId", SqlDbType.Int),
        new SqlMetaData("Location", SqlDbType.NVarChar, 2083L),
        new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("Length", SqlDbType.Int),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("ModifiedDate", SqlDbType.DateTime),
        new SqlMetaData("Comment", SqlDbType.NVarChar, (long) byte.MaxValue)
      };
      private System.Func<Guid, int> m_dataspaceResolver;

      public override string TypeName => "typ_WorkItemResourceLinkUpdateTable3";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent11.WorkItemResourceLinkUpdateTable3RecordBinder.s_metadata;

      public WorkItemResourceLinkUpdateTable3RecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemResourceLinkUpdateRecord update)
      {
        record.SetInt32(0, update.Order);
        record.SetByte(1, (byte) update.UpdateType);
        record.SetInt32(2, update.SourceId);
        record.SetInt32(3, this.m_dataspaceResolver(update.ProjectId));
        if (update.Type.HasValue)
          record.SetInt32(4, (int) update.Type.Value);
        else
          record.SetDBNull(4);
        record.SetNullableInt32(5, update.ResourceId);
        record.SetNullableString(6, update.Location);
        record.SetNullableString(7, update.Name);
        record.SetInt32(8, update.Length.GetValueOrDefault(-1));
        WorkItemTrackingSqlDataRecord trackingSqlDataRecord1 = record;
        DateTime? nullable = update.CreationDate;
        DateTime valueOrDefault1 = nullable.GetValueOrDefault(SqlDateTime.MinValue.Value);
        trackingSqlDataRecord1.SetDateTime(9, valueOrDefault1);
        WorkItemTrackingSqlDataRecord trackingSqlDataRecord2 = record;
        nullable = update.LastModifiedDate;
        DateTime valueOrDefault2 = nullable.GetValueOrDefault(SqlDateTime.MinValue.Value);
        trackingSqlDataRecord2.SetDateTime(10, valueOrDefault2);
        record.SetNullableString(11, update.Comment);
      }
    }

    protected class WorkItemLinkUpdateTable2RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemLinkUpdateRecord>
    {
      protected static readonly SqlMetaData[] s_metadata2 = new SqlMetaData[11]
      {
        new SqlMetaData("Order", SqlDbType.Int),
        new SqlMetaData("UpdateType", SqlDbType.TinyInt),
        new SqlMetaData("DataspaceId", SqlDbType.Int),
        new SqlMetaData("SourceId", SqlDbType.Int),
        new SqlMetaData("SourceAreaId", SqlDbType.Int),
        new SqlMetaData("TargetDataspaceId", SqlDbType.Int),
        new SqlMetaData("TargetId", SqlDbType.Int),
        new SqlMetaData("TargetAreaId", SqlDbType.Int),
        new SqlMetaData("LinkType", SqlDbType.Int),
        new SqlMetaData("Locked", SqlDbType.Bit),
        new SqlMetaData("Comment", SqlDbType.NVarChar, (long) byte.MaxValue)
      };
      protected System.Func<Guid, int> m_dataspaceResolver;

      public override string TypeName => "typ_WorkItemLinkUpdateTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent11.WorkItemLinkUpdateTable2RecordBinder.s_metadata2;

      public WorkItemLinkUpdateTable2RecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemLinkUpdateRecord update)
      {
        record.SetInt32(0, update.Order);
        record.SetByte(1, (byte) update.UpdateType);
        record.SetInt32(2, this.m_dataspaceResolver(update.SourceProjectId));
        record.SetInt32(3, update.SourceId);
        record.SetInt32(4, update.SourceAreaId);
        record.SetInt32(5, this.m_dataspaceResolver(update.TargetProjectId));
        record.SetInt32(6, update.TargetId);
        record.SetInt32(7, update.TargetAreaId);
        record.SetInt32(8, update.LinkType);
        record.SetNullableBool(9, update.Locked);
        record.SetString(10, update.Comment ?? string.Empty);
      }
    }

    protected class WorkItemTextFieldUpdateTable2RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTextFieldUpdateRecord>
    {
      protected static readonly SqlMetaData[] TvpMetadata2 = new SqlMetaData[6]
      {
        new SqlMetaData("Id", SqlDbType.Int),
        new SqlMetaData("DataspaceId", SqlDbType.Int),
        new SqlMetaData("FieldId", SqlDbType.Int),
        new SqlMetaData("Revision", SqlDbType.Int),
        new SqlMetaData("IsHtml", SqlDbType.Bit),
        new SqlMetaData("Value", SqlDbType.NText)
      };
      private System.Func<Guid, int> m_dataspaceResolver;

      public override string TypeName => "typ_WorkItemTextFieldUpdateTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent11.WorkItemTextFieldUpdateTable2RecordBinder.TvpMetadata2;

      public WorkItemTextFieldUpdateTable2RecordBinder(System.Func<Guid, int> dataspaceResolver) => this.m_dataspaceResolver = dataspaceResolver;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemTextFieldUpdateRecord update)
      {
        record.SetInt32(0, update.WorkItemId);
        record.SetInt32(1, this.m_dataspaceResolver(update.ProjectId));
        record.SetInt32(2, update.FieldId);
        record.SetInt32(3, update.Revision);
        record.SetBoolean(4, update.IsHtml);
        record.SetString(5, update.Text ?? "");
      }
    }
  }
}
