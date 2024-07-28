// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent6 : DiagnosticComponent5
  {
    public override List<QDSExpensiveQuery> QueryQDS(DateTime start, DateTime end)
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryQDS", 60);
      this.BindDateTime2("@start", start);
      this.BindDateTime2("@end", end);
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, "DIAGNOSTIC.prc_QueryQDS", this.RequestContext))
        {
          resultCollection.AddBinder<QDSExpensiveQuery>(this.GetQDSBinder());
          return resultCollection.GetCurrent<QDSExpensiveQuery>().Items;
        }
      }
    }

    protected virtual ObjectBinder<QDSExpensiveQuery> GetQDSBinder() => (ObjectBinder<QDSExpensiveQuery>) new DiagnosticComponent6.QDSExpensiveQueriesBinder();

    public override List<MemoryClerkInfo> QueryMemoryClerks()
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryMemoryClerks");
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, "DIAGNOSTIC.prc_QueryMemoryClerks", this.RequestContext))
        {
          resultCollection.AddBinder<MemoryClerkInfo>((ObjectBinder<MemoryClerkInfo>) new DiagnosticComponent6.MemoryClerkInfoBinder());
          return resultCollection.GetCurrent<MemoryClerkInfo>().Items;
        }
      }
    }

    internal class QDSExpensiveQueriesBinder : ObjectBinder<QDSExpensiveQuery>
    {
      private SqlColumnBinder m_queryText = new SqlColumnBinder("query_sql_text");
      private SqlColumnBinder m_queryId = new SqlColumnBinder("query_id");
      private SqlColumnBinder m_objectName = new SqlColumnBinder("object_name");
      private SqlColumnBinder m_queryTextId = new SqlColumnBinder("query_text_id");
      private SqlColumnBinder m_planId = new SqlColumnBinder("plan_id");
      private SqlColumnBinder m_totalPhysicalReads = new SqlColumnBinder("total_physical_reads");
      private SqlColumnBinder m_totalCpuTime = new SqlColumnBinder("total_cpu_time_ms");
      private SqlColumnBinder m_averageRowCount = new SqlColumnBinder("avg_row_count");
      private SqlColumnBinder m_totalExecutions = new SqlColumnBinder("total_executions");
      private SqlColumnBinder m_totalLogicalReads = new SqlColumnBinder("total_logical_reads");
      private SqlColumnBinder m_averageCpuTime = new SqlColumnBinder("avg_cpu_time_ms");
      private SqlColumnBinder m_totalAborted = new SqlColumnBinder("total_aborted");
      private SqlColumnBinder m_totalExceptions = new SqlColumnBinder("total_exceptions");
      private SqlColumnBinder m_totalLogicalWrites = new SqlColumnBinder("total_logical_writes");
      private SqlColumnBinder m_avgDop = new SqlColumnBinder("avg_dop");
      private SqlColumnBinder m_avgQueryMaxUsedMemory = new SqlColumnBinder("avg_query_max_used_memory");

      protected override QDSExpensiveQuery Bind() => new QDSExpensiveQuery()
      {
        AverageCpuTime = (long) this.m_averageCpuTime.GetDouble((IDataReader) this.Reader),
        AverageDop = (float) this.m_avgDop.GetDouble((IDataReader) this.Reader),
        AverageQueryMaxUsedMemory = (long) this.m_avgQueryMaxUsedMemory.GetDouble((IDataReader) this.Reader),
        AverageRowCount = (long) this.m_averageRowCount.GetDouble((IDataReader) this.Reader),
        ObjectName = this.m_objectName.GetString((IDataReader) this.Reader, true),
        PlanId = this.m_planId.GetInt64((IDataReader) this.Reader),
        QueryId = this.m_queryId.GetInt64((IDataReader) this.Reader),
        QueryText = this.m_queryText.GetString((IDataReader) this.Reader, true),
        QueryTextId = this.m_queryTextId.GetInt64((IDataReader) this.Reader),
        TotalAborted = (long) this.m_totalAborted.GetInt32((IDataReader) this.Reader),
        TotalExceptions = (long) this.m_totalExceptions.GetInt32((IDataReader) this.Reader),
        TotalCpuTime = (long) this.m_totalCpuTime.GetDouble((IDataReader) this.Reader),
        TotalExecutions = this.m_totalExecutions.GetInt64((IDataReader) this.Reader),
        TotalLogicalReads = (long) this.m_totalLogicalReads.GetDouble((IDataReader) this.Reader),
        TotalLogicalWrites = (long) this.m_totalLogicalWrites.GetDouble((IDataReader) this.Reader),
        TotalPhysicalReads = (long) this.m_totalPhysicalReads.GetDouble((IDataReader) this.Reader),
        QueryHash = 0,
        QueryPlanHash = 0
      };
    }

    internal class MemoryClerkInfoBinder : ObjectBinder<MemoryClerkInfo>
    {
      private SqlColumnBinder m_type = new SqlColumnBinder("Type");
      private SqlColumnBinder m_memoryUsageInMb = new SqlColumnBinder("MemoryUsageInMB");

      protected override MemoryClerkInfo Bind() => new MemoryClerkInfo()
      {
        Type = this.m_type.GetString((IDataReader) this.Reader, false),
        MemoryUsageInMB = this.m_memoryUsageInMb.GetDouble((IDataReader) this.Reader)
      };
    }
  }
}
