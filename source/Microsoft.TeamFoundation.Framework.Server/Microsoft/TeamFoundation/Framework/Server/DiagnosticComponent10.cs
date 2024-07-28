// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent10
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent10 : DiagnosticComponent9
  {
    internal override List<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> ReadResults(
      IDataReader reader)
    {
      using (ResultCollection resultCollection = new ResultCollection(reader, this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new DiagnosticComponent4.ServiceObjectiveBinder());
        string serviceObjective = resultCollection.GetCurrent<string>().FirstOrDefault<string>();
        resultCollection.NextResult();
        resultCollection.AddBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>((ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>) new DiagnosticComponent10.DatabasePerformanceStatisticsViewBinder2(serviceObjective));
        return resultCollection.GetCurrent<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>().Items;
      }
    }

    protected class DatabasePerformanceStatisticsViewBinder2 : 
      ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>
    {
      private SqlColumnBinder m_periodStart = new SqlColumnBinder("PeriodStart");
      private SqlColumnBinder m_averageCpuPercentage = new SqlColumnBinder("AverageCpuPercentage");
      private SqlColumnBinder m_maximumCpuPercentage = new SqlColumnBinder("MaximumCpuPercentage");
      private SqlColumnBinder m_averageDataIOPercentage = new SqlColumnBinder("AverageDataIOPercentage");
      private SqlColumnBinder m_maximumDataIOPercentage = new SqlColumnBinder("MaximumDataIOPercentage");
      private SqlColumnBinder m_average_logWriteUtilizationPercentage = new SqlColumnBinder("AverageLogWriteUtilizationPercentage");
      private SqlColumnBinder m_maximum_logWriteUtilizationPercentage = new SqlColumnBinder("MaximumLogWriteUtilizationPercentage");
      private SqlColumnBinder m_averageMemoryUsagePercentage = new SqlColumnBinder("AverageMemoryUsagePercentage");
      private SqlColumnBinder m_maximumMemoryUsagePercentage = new SqlColumnBinder("MaximumMemoryUsagePercentage");
      private SqlColumnBinder m_averageWorkerPercentage = new SqlColumnBinder("AverageWorkerPercentage");
      private SqlColumnBinder m_maximumWorkerPercentage = new SqlColumnBinder("MaximumWorkerPercentage");
      private SqlColumnBinder m_averageSessionPercentage = new SqlColumnBinder("AverageSessionPercentage");
      private SqlColumnBinder m_maximumSessionPercentage = new SqlColumnBinder("MaximumSessionPercentage");
      private SqlColumnBinder m_dtuLimit = new SqlColumnBinder("DtuLimit");

      private string ServiceObjective { get; set; }

      internal DatabasePerformanceStatisticsViewBinder2(string serviceObjective) => this.ServiceObjective = serviceObjective;

      internal DatabasePerformanceStatisticsViewBinder2(SqlDataReader reader)
        : base(reader, "DIAGNOSTIC.prc_QueryDatabasePerformanceStatistics")
      {
      }

      protected override DiagnosticComponent.DatabasePerformanceStatisticsViewResult Bind() => new DiagnosticComponent.DatabasePerformanceStatisticsViewResult()
      {
        PeriodStart = this.m_periodStart.GetDateTime((IDataReader) this.Reader),
        AverageCpuPercentage = Convert.ToDecimal(this.m_averageCpuPercentage.GetObject((IDataReader) this.Reader)),
        MaximumCpuPercentage = Convert.ToDecimal(this.m_maximumCpuPercentage.GetObject((IDataReader) this.Reader)),
        AverageDataIOPercentage = Convert.ToDecimal(this.m_averageDataIOPercentage.GetObject((IDataReader) this.Reader)),
        MaximumDataIOPercentage = Convert.ToDecimal(this.m_maximumDataIOPercentage.GetObject((IDataReader) this.Reader)),
        AverageLogWriteUtilizationPercentage = Convert.ToDecimal(this.m_average_logWriteUtilizationPercentage.GetObject((IDataReader) this.Reader)),
        MaximumLogWriteUtilizationPercentage = Convert.ToDecimal(this.m_maximum_logWriteUtilizationPercentage.GetObject((IDataReader) this.Reader)),
        AverageMemoryUsagePercentage = Convert.ToDecimal(this.m_averageMemoryUsagePercentage.GetObject((IDataReader) this.Reader)),
        MaximumMemoryUsagePercentage = Convert.ToDecimal(this.m_maximumMemoryUsagePercentage.GetObject((IDataReader) this.Reader)),
        ServiceObjective = this.ServiceObjective,
        AverageWorkerPercentage = Convert.ToDecimal(this.m_averageWorkerPercentage.GetObject((IDataReader) this.Reader)),
        MaximumWorkerPercentage = Convert.ToDecimal(this.m_maximumWorkerPercentage.GetObject((IDataReader) this.Reader)),
        AverageSessionPercentage = Convert.ToDecimal(this.m_averageSessionPercentage.GetObject((IDataReader) this.Reader)),
        MaximumSessionPercentage = Convert.ToDecimal(this.m_maximumSessionPercentage.GetObject((IDataReader) this.Reader)),
        DtuLimit = (short) this.m_dtuLimit.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
