// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent21
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
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent21 : WorkItemComponent20
  {
    protected override SqlParameter BindCustomFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemCustomFieldUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemCustomFieldUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemCustomFieldUpdateRecord>) new WorkItemComponent21.WorkItemFieldUpdateTable3RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    protected override SqlParameter BindWorkItemTextFieldUpdates(
      string parameterName,
      IEnumerable<WorkItemTextFieldUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemTextFieldUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemTextFieldUpdateRecord>) new WorkItemComponent21.WorkItemTextFieldUpdateTable3RecordBinder(new System.Func<Guid, int>(((TeamFoundationSqlResourceComponent) this).GetDataspaceId)), parameterName, rows);
    }

    internal override WorkItemComponent.UpdateWorkItemsResultsReader GetUpdateWorkItemsResultsReader(
      bool bypassRules,
      bool isAdmin,
      WorkItemUpdateDataset updateDataset)
    {
      return (WorkItemComponent.UpdateWorkItemsResultsReader) new WorkItemComponent21.UpdateWorkItemsResultsReader3(bypassRules, isAdmin, updateDataset);
    }

    protected class WorkItemFieldUpdateTable3RecordBinder : 
      WorkItemComponent11.WorkItemFieldUpdateTable2RecordBinder
    {
      protected static readonly SqlMetaData[] TvpMetadata3 = ((IEnumerable<SqlMetaData>) WorkItemComponent11.WorkItemFieldUpdateTable2RecordBinder.TvpMetadata2).Concat<SqlMetaData>((IEnumerable<SqlMetaData>) new SqlMetaData[1]
      {
        new SqlMetaData("NeedsNewRevision", SqlDbType.Bit)
      }).ToArray<SqlMetaData>();

      public override string TypeName => "typ_WorkItemFieldUpdateTable3";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent21.WorkItemFieldUpdateTable3RecordBinder.TvpMetadata3;

      public WorkItemFieldUpdateTable3RecordBinder(System.Func<Guid, int> dataspaceResolver)
        : base(dataspaceResolver)
      {
      }

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemCustomFieldUpdateRecord update)
      {
        base.SetRecordValues(record, update);
        record.SetBoolean(10, update.NeedsNewRevision);
      }
    }

    protected class WorkItemTextFieldUpdateTable3RecordBinder : 
      WorkItemComponent11.WorkItemTextFieldUpdateTable2RecordBinder
    {
      protected static readonly SqlMetaData[] TvpMetadata3 = ((IEnumerable<SqlMetaData>) WorkItemComponent11.WorkItemTextFieldUpdateTable2RecordBinder.TvpMetadata2).Concat<SqlMetaData>((IEnumerable<SqlMetaData>) new SqlMetaData[1]
      {
        new SqlMetaData("NeedsNewRevision", SqlDbType.Bit)
      }).ToArray<SqlMetaData>();

      public override string TypeName => "typ_WorkItemTextFieldUpdateTable3";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent21.WorkItemTextFieldUpdateTable3RecordBinder.TvpMetadata3;

      public WorkItemTextFieldUpdateTable3RecordBinder(System.Func<Guid, int> dataspaceResolver)
        : base(dataspaceResolver)
      {
      }

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemTextFieldUpdateRecord update)
      {
        base.SetRecordValues(record, update);
        record.SetBoolean(6, update.NeedsNewRevision);
      }
    }

    internal class UpdateWorkItemsResultsReader3 : WorkItemComponent20.UpdateWorkItemsResultsReader2
    {
      internal UpdateWorkItemsResultsReader3(
        bool bypassRules,
        bool isAdmin,
        WorkItemUpdateDataset updateDataset)
        : base(bypassRules, isAdmin, updateDataset)
      {
      }

      internal override WorkItemUpdateResultSet Read(IDataReader reader)
      {
        base.Read(reader);
        if (this.result.Success)
          this.ReadPerformanceTimings(reader);
        return this.result;
      }

      protected override void CheckAndReadWatermark(IDataReader reader)
      {
        if (!this.updateDataset.CoreFieldUpdates.Any<WorkItemCoreFieldUpdatesRecord>() && !this.updateDataset.CustomFieldUpdates.Any<WorkItemCustomFieldUpdateRecord>() && !this.updateDataset.TextFieldUpdates.Any<WorkItemTextFieldUpdateRecord>())
          return;
        reader.NextResult();
        this.result.Watermark = reader.Read() ? reader.GetInt32(0) : throw new WorkItemTrackingSqlDataBindingException("The watermark table has no records! - should have one");
        if (reader.Read())
          throw new WorkItemTrackingSqlDataBindingException("The watermark table has more than one record - should have only one");
      }

      protected virtual void ReadPerformanceTimings(IDataReader reader)
      {
        if (!reader.NextResult())
          return;
        this.result.PerformanceTimings = (IEnumerable<KeyValuePair<string, int>>) WorkItemTrackingResourceComponent.Bind<KeyValuePair<string, int>>(reader, (System.Func<IDataReader, KeyValuePair<string, int>>) (r => new KeyValuePair<string, int>(r.GetString(0), r.GetInt32(1)))).ToArray<KeyValuePair<string, int>>();
      }
    }
  }
}
