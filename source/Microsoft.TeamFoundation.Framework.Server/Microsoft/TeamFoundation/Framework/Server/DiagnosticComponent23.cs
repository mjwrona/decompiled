// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent23
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent23 : DiagnosticComponent22
  {
    protected override ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> GetDatabasePerformanceStatisticsViewBinder(
      string serviceObjective,
      string resourceVersion,
      int schedulers,
      short compatibilityLevel)
    {
      return (ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>) new DiagnosticComponent23.DatabasePerformanceStatisticsViewBinder5(serviceObjective, resourceVersion, schedulers, compatibilityLevel);
    }

    protected class DatabasePerformanceStatisticsViewBinder5 : 
      ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>
    {
      private SqlColumnBinder m_periodStart = new SqlColumnBinder("PeriodStart");
      private SqlColumnBinder m_averageCpuPercentage = new SqlColumnBinder("AverageCpuPercentage");
      private SqlColumnBinder m_maximumCpuPercentage = new SqlColumnBinder("MaximumCpuPercentage");
      private SqlColumnBinder m_averageDataIOPercentage = new SqlColumnBinder("AverageDataIOPercentage");
      private SqlColumnBinder m_maximumDataIOPercentage = new SqlColumnBinder("MaximumDataIOPercentage");
      private SqlColumnBinder m_averageLogWriteUtilizationPercentage = new SqlColumnBinder("AverageLogWriteUtilizationPercentage");
      private SqlColumnBinder m_maximumLogWriteUtilizationPercentage = new SqlColumnBinder("MaximumLogWriteUtilizationPercentage");
      private SqlColumnBinder m_averageMemoryUsagePercentage = new SqlColumnBinder("AverageMemoryUsagePercentage");
      private SqlColumnBinder m_maximumMemoryUsagePercentage = new SqlColumnBinder("MaximumMemoryUsagePercentage");
      private SqlColumnBinder m_averageWorkerPercentage = new SqlColumnBinder("AverageWorkerPercentage");
      private SqlColumnBinder m_maximumWorkerPercentage = new SqlColumnBinder("MaximumWorkerPercentage");
      private SqlColumnBinder m_averageSessionPercentage = new SqlColumnBinder("AverageSessionPercentage");
      private SqlColumnBinder m_maximumSessionPercentage = new SqlColumnBinder("MaximumSessionPercentage");
      private SqlColumnBinder m_averageXtpStoragePercent = new SqlColumnBinder("AverageXtpStoragePercent");
      private SqlColumnBinder m_maximumXtpStoragePercent = new SqlColumnBinder("MaximumXtpStoragePercent");
      private SqlColumnBinder m_dtuLimit = new SqlColumnBinder("DtuLimit");
      private SqlColumnBinder m_averageInstanceCpuPercentage = new SqlColumnBinder("AverageInstanceCpuPercentage");
      private SqlColumnBinder m_averageInstanceMemoryPercentage = new SqlColumnBinder("AverageInstanceMemoryPercentage");
      private SqlColumnBinder m_replicaRole = new SqlColumnBinder("ReplicaRole");
      private SqlColumnBinder m_redoQueueSize = new SqlColumnBinder("RedoQueueSize");
      private SqlColumnBinder m_redoRate = new SqlColumnBinder("RedoRate");
      private SqlColumnBinder m_secondaryLagSeconds = new SqlColumnBinder("SecondaryLagSeconds");
      private SqlColumnBinder m_synchronizationHealth = new SqlColumnBinder("SynchronizationHealth");

      private string ServiceObjective { get; set; }

      private string ResourceVersion { get; set; }

      private int Schedulers { get; set; }

      private short CompatibilityLevel { get; set; }

      internal DatabasePerformanceStatisticsViewBinder5(
        string serviceObjective,
        string resourceVersion,
        int schedulers,
        short compatibilityLevel)
      {
        this.ResourceVersion = resourceVersion;
        this.ServiceObjective = serviceObjective;
        this.Schedulers = schedulers;
        this.CompatibilityLevel = compatibilityLevel;
      }

      internal DatabasePerformanceStatisticsViewBinder5(SqlDataReader reader)
        : base(reader, "DIAGNOSTIC.prc_QueryDatabasePerformanceStatistics")
      {
      }

      protected override DiagnosticComponent.DatabasePerformanceStatisticsViewResult Bind()
      {
        DiagnosticComponent.DatabasePerformanceStatisticsViewResult statisticsViewResult = new DiagnosticComponent.DatabasePerformanceStatisticsViewResult();
        statisticsViewResult.PeriodStart = this.m_periodStart.GetDateTime((IDataReader) this.Reader);
        object obj1 = this.m_averageCpuPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageCpuPercentage = Convert.ToDecimal(obj1);
        object obj2 = this.m_maximumCpuPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.MaximumCpuPercentage = Convert.ToDecimal(obj2);
        object obj3 = this.m_averageDataIOPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageDataIOPercentage = Convert.ToDecimal(obj3);
        object obj4 = this.m_maximumDataIOPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.MaximumDataIOPercentage = Convert.ToDecimal(obj4);
        object obj5 = this.m_averageLogWriteUtilizationPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageLogWriteUtilizationPercentage = Convert.ToDecimal(obj5);
        object obj6 = this.m_maximumLogWriteUtilizationPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.MaximumLogWriteUtilizationPercentage = Convert.ToDecimal(obj6);
        object obj7 = this.m_averageMemoryUsagePercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageMemoryUsagePercentage = Convert.ToDecimal(obj7);
        object obj8 = this.m_maximumMemoryUsagePercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.MaximumMemoryUsagePercentage = Convert.ToDecimal(obj8);
        statisticsViewResult.ServiceObjective = this.ServiceObjective;
        object obj9 = this.m_averageWorkerPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageWorkerPercentage = Convert.ToDecimal(obj9);
        object obj10 = this.m_maximumWorkerPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.MaximumWorkerPercentage = Convert.ToDecimal(obj10);
        object obj11 = this.m_averageSessionPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageSessionPercentage = Convert.ToDecimal(obj11);
        object obj12 = this.m_maximumSessionPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.MaximumSessionPercentage = Convert.ToDecimal(obj12);
        object obj13 = this.m_averageXtpStoragePercent.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageXtpStoragePercent = Convert.ToDecimal(obj13);
        object obj14 = this.m_maximumXtpStoragePercent.GetObject((IDataReader) this.Reader);
        statisticsViewResult.MaximumXtpStoragePercent = Convert.ToDecimal(obj14);
        object obj15 = this.m_averageInstanceCpuPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageInstanceCpuPercentage = Convert.ToDecimal(obj15);
        object obj16 = this.m_averageInstanceMemoryPercentage.GetObject((IDataReader) this.Reader);
        statisticsViewResult.AverageInstanceMemoryPercentage = Convert.ToDecimal(obj16);
        object obj17 = this.m_replicaRole.GetObject((IDataReader) this.Reader);
        statisticsViewResult.ReplicaRole = Convert.ToInt16(obj17);
        statisticsViewResult.DtuLimit = (short) this.m_dtuLimit.GetInt32((IDataReader) this.Reader);
        statisticsViewResult.ResourceVersion = this.ResourceVersion;
        statisticsViewResult.Schedulers = this.Schedulers;
        statisticsViewResult.CompatibilityLevel = this.CompatibilityLevel;
        object obj18 = this.m_redoQueueSize.GetObject((IDataReader) this.Reader);
        statisticsViewResult.RedoQueueSize = Convert.ToInt64(obj18);
        object obj19 = this.m_redoRate.GetObject((IDataReader) this.Reader);
        statisticsViewResult.RedoRate = Convert.ToInt64(obj19);
        object obj20 = this.m_secondaryLagSeconds.GetObject((IDataReader) this.Reader);
        statisticsViewResult.SecondaryLagSeconds = Convert.ToDecimal(obj20);
        object obj21 = this.m_synchronizationHealth.GetObject((IDataReader) this.Reader);
        statisticsViewResult.SynchronizationHealth = Convert.ToInt16(obj21);
        if (obj1 == null || obj2 == null || obj3 == null || obj4 == null || obj5 == null || obj6 == null || obj7 == null || obj8 == null || obj9 == null || obj10 == null || obj11 == null || obj12 == null || obj13 == null || obj15 == null || obj16 == null || obj17 == null)
          TeamFoundationTracingService.TraceRawAlwaysOn(653954319, TraceLevel.Warning, "DiagnosticComponent", this.ProcedureName, "DatabasePerformanceStatistics detected a null value:\r\nPeriodStart={0},\r\nAverageCpuPercentage={1},\r\nMaximumCpuPercentage={2},\r\nAverageDataIOPercentage={3},\r\nMaximumDataIOPercentage={4},\r\nAverageLogWriteUtilizationPercentage={5},\r\nMaximumLogWriteUtilizationPercentage={6},\r\nAverageMemoryUsagePercentage={7},\r\nMaximumMemoryUsagePercentage={8},\r\nAverageWorkerPercentage={9},\r\nMaximumWorkerPercentage={10},\r\nAverageSessionPercentage={11},\r\nMaximumSessionPercentage={12},\r\nAverageXtpStoragePercent={13},\r\nMaximumXtpStoragePercent={14}\r\nAverageInstanceCpuPercentage={15}\r\nAverageInstanceMemoryPercentage={16}\r\nReplicaRole={17}", (object) statisticsViewResult.PeriodStart, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9, obj10, obj11, obj12, obj13, obj14, obj15, obj16, obj17);
        return statisticsViewResult;
      }
    }
  }
}
