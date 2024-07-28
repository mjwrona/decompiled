// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent5
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

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent5 : WorkItemComponent4
  {
    protected override SqlParameter BindWorkItemResourceLinkUpdates(
      string parameterName,
      IEnumerable<WorkItemResourceLinkUpdateRecord> rows)
    {
      return this.BindBasicTvp<WorkItemResourceLinkUpdateRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemResourceLinkUpdateRecord>) new WorkItemComponent5.WorkItemResourceLinkUpdateTable2RecordBinder(), parameterName, rows);
    }

    private class WorkItemResourceLinkUpdateTable2RecordBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<WorkItemResourceLinkUpdateRecord>
    {
      private static readonly SqlMetaData[] s_metadata = new SqlMetaData[11]
      {
        new SqlMetaData("Order", SqlDbType.Int),
        new SqlMetaData("UpdateType", SqlDbType.TinyInt),
        new SqlMetaData("SourceId", SqlDbType.Int),
        new SqlMetaData("ResourceType", SqlDbType.Int),
        new SqlMetaData("ResourceId", SqlDbType.Int),
        new SqlMetaData("Location", SqlDbType.NVarChar, 2083L),
        new SqlMetaData("Name", SqlDbType.NVarChar, (long) byte.MaxValue),
        new SqlMetaData("Length", SqlDbType.Int),
        new SqlMetaData("CreatedDate", SqlDbType.DateTime),
        new SqlMetaData("ModifiedDate", SqlDbType.DateTime),
        new SqlMetaData("Comment", SqlDbType.NVarChar, (long) byte.MaxValue)
      };

      public override string TypeName => "typ_WorkItemResourceLinkUpdateTable2";

      protected override SqlMetaData[] TvpMetadata => WorkItemComponent5.WorkItemResourceLinkUpdateTable2RecordBinder.s_metadata;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        WorkItemResourceLinkUpdateRecord update)
      {
        record.SetInt32(0, update.Order);
        record.SetByte(1, (byte) update.UpdateType);
        record.SetInt32(2, update.SourceId);
        if (update.Type.HasValue)
          record.SetInt32(3, (int) update.Type.Value);
        else
          record.SetDBNull(3);
        record.SetNullableInt32(4, update.ResourceId);
        record.SetNullableString(5, update.Location);
        record.SetNullableString(6, update.Name);
        record.SetInt32(7, update.Length.GetValueOrDefault(-1));
        WorkItemTrackingSqlDataRecord trackingSqlDataRecord1 = record;
        DateTime? nullable = update.CreationDate;
        DateTime valueOrDefault1 = nullable.GetValueOrDefault(SqlDateTime.MinValue.Value);
        trackingSqlDataRecord1.SetDateTime(8, valueOrDefault1);
        WorkItemTrackingSqlDataRecord trackingSqlDataRecord2 = record;
        nullable = update.LastModifiedDate;
        DateTime valueOrDefault2 = nullable.GetValueOrDefault(SqlDateTime.MinValue.Value);
        trackingSqlDataRecord2.SetDateTime(9, valueOrDefault2);
        record.SetNullableString(10, update.Comment);
      }
    }
  }
}
