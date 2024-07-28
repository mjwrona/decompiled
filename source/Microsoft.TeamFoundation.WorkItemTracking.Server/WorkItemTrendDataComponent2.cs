// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrendDataComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTrendDataComponent2 : WorkItemTrendDataComponent
  {
    public override void AddTrendDataRecords(int fieldId, IList<TrendDataRecord> records)
    {
      this.PrepareStoredProcedure("prc_AddTrendDataRecords");
      this.BindInt("@fieldId", fieldId);
      this.BindTrendDataRecordTable("@trendDataRecords", (IEnumerable<TrendDataRecord>) records);
      this.ExecuteNonQueryEx();
    }

    public override ResultCollection GetChangedWorkItemData(
      int fieldId,
      int markerFieldId,
      DateTime beginDate,
      DateTime endDate)
    {
      this.PrepareStoredProcedure("prc_GetChangedWorkItemData");
      this.BindInt("@fieldId", fieldId);
      this.BindInt("@markerFieldId", markerFieldId);
      this.BindDateTime("@beginDate", beginDate);
      this.BindDateTime("@endDate", endDate);
      try
      {
        ResultCollection changedWorkItemData = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        changedWorkItemData.AddBinder<WorkItemDataRecord>((ObjectBinder<WorkItemDataRecord>) new WorkItemDataRecordBinder());
        changedWorkItemData.AddBinder<WorkItemDataRecord>((ObjectBinder<WorkItemDataRecord>) new WorkItemDataRecordBinder());
        return changedWorkItemData;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public override List<TrendDataRecord> BeginRebuildTrendData(
      int fieldId,
      int markerFieldId,
      int interval,
      out DateTime endDate)
    {
      this.PrepareStoredProcedure("prc_BeginRebuildTrendData");
      this.BindInt("@fieldId", fieldId);
      this.BindInt("@markerFieldId", markerFieldId);
      this.BindInt("@interval", interval);
      try
      {
        using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new DateTimeBinder());
          resultCollection.AddBinder<TrendDataRecord>((ObjectBinder<TrendDataRecord>) new TrendDataRecordBinder());
          ObjectBinder<DateTime> current = resultCollection.GetCurrent<DateTime>();
          current.MoveNext();
          endDate = current.Current;
          resultCollection.NextResult();
          return resultCollection.GetCurrent<TrendDataRecord>().Items;
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }
  }
}
