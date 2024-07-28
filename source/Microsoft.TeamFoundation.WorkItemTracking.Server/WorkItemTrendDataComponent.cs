// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrendDataComponent
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class WorkItemTrendDataComponent : WorkItemTrackingResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<WorkItemTrendDataComponent>(1),
      (IComponentCreator) new ComponentCreator<WorkItemTrendDataComponent2>(2)
    }, "WorkItemTrendData", "WorkItem");

    public static WorkItemTrendDataComponent CreateComponent(IVssRequestContext requestContext) => requestContext.CreateComponent<WorkItemTrendDataComponent>();

    public virtual ResultCollection GetTrendData(
      int fieldId,
      DateTime beginDate,
      DateTime? endDate)
    {
      this.PrepareStoredProcedure("prc_GetTrendData");
      this.BindInt("@fieldId", fieldId);
      this.BindDateTime("@beginDate", beginDate);
      if (endDate.HasValue)
        this.BindDateTime("@endDate", endDate.Value);
      try
      {
        ResultCollection trendData = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        trendData.AddBinder<DateTime>((ObjectBinder<DateTime>) new DateTimeBinder());
        trendData.AddBinder<TrendDataRecord>((ObjectBinder<TrendDataRecord>) new TrendDataRecordBinder());
        trendData.AddBinder<TrendDataRecord>((ObjectBinder<TrendDataRecord>) new TrendDataRecordBinder());
        return trendData;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public virtual void SetTrendDataBaseline(int fieldId, IList<TrendDataRecord> records)
    {
      this.PrepareStoredProcedure("prc_SetTrendDataBaseline");
      this.BindInt("@fieldId", fieldId);
      this.BindTrendDataRecordTable("@trendDataRecords", (IEnumerable<TrendDataRecord>) records);
      this.ExecuteNonQueryEx();
    }

    public virtual void StampTrendDataBaseline(int fieldId)
    {
      this.PrepareStoredProcedure("prc_StampTrendDataBaseline");
      this.BindInt("@fieldId", fieldId);
      this.ExecuteNonQueryEx();
    }

    public virtual void AddTrendDataRecords(int fieldId, IList<TrendDataRecord> records)
    {
    }

    public virtual ResultCollection GetChangedWorkItemData(
      int fieldId,
      int markerFieldId,
      DateTime beginDate,
      DateTime endDate)
    {
      throw new NotSupportedException();
    }

    public virtual List<TrendDataRecord> BeginRebuildTrendData(
      int fieldId,
      int markerFieldId,
      int interval,
      out DateTime endDate)
    {
      throw new NotSupportedException();
    }

    protected virtual SqlParameter BindTrendDataRecordTable(
      string parameterName,
      IEnumerable<TrendDataRecord> records)
    {
      return this.BindBasicTvp<TrendDataRecord>((WorkItemTrackingResourceComponent.TvpRecordBinder<TrendDataRecord>) new WorkItemTrendDataComponent.TrendDataRecordTableBinder(), parameterName, records);
    }

    private class TrendDataRecordTableBinder : 
      WorkItemTrackingResourceComponent.TvpRecordBinder<TrendDataRecord>
    {
      private static readonly SqlMetaData[] s_metaData = new SqlMetaData[4]
      {
        new SqlMetaData("AuthorizedDate", SqlDbType.DateTime),
        new SqlMetaData("RevisedDate", SqlDbType.DateTime),
        new SqlMetaData("Value", SqlDbType.NVarChar, 256L),
        new SqlMetaData("Count", SqlDbType.Int)
      };

      public override string TypeName => "typ_WitTrendDataRecordTable";

      protected override SqlMetaData[] TvpMetadata => WorkItemTrendDataComponent.TrendDataRecordTableBinder.s_metaData;

      public override void SetRecordValues(
        WorkItemTrackingSqlDataRecord record,
        TrendDataRecord entry)
      {
        record.SetDateTime(0, entry.AuthorizedDate);
        record.SetDateTime(1, entry.RevisedDate);
        record.SetString(2, entry.Value);
        record.SetInt32(3, entry.Count);
      }
    }
  }
}
