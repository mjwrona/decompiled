// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DiagnosticComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DiagnosticComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[28]
    {
      (IComponentCreator) new ComponentCreator<DiagnosticComponent>(1),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent2>(2),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent3>(3),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent4>(4),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent5>(5),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent6>(6),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent9>(9),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent10>(10),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent11>(11),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent12>(12),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent13>(13),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent14>(14),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent15>(15),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent16>(16),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent17>(17),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent18>(18),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent19>(19),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent20>(20),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent21>(21),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent22>(22),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent23>(23),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent24>(24),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent25>(25),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent26>(26),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent27>(27),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent28>(28),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent29>(29),
      (IComponentCreator) new ComponentCreator<DiagnosticComponent30>(30)
    }, "Diagnostic");

    public virtual List<DatabaseManagementViewResult> QueryWhatsRunning()
    {
      this.PrepareStoredProcedure("prc_QueryWhatsRunning");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DatabaseManagementViewResult>((ObjectBinder<DatabaseManagementViewResult>) new DMVBinder());
        return resultCollection.GetCurrent<DatabaseManagementViewResult>().Items;
      }
    }

    public virtual List<DiagnosticComponent.DatabasePerformanceStatisticsViewResult> QueryDatabasePerformanceStatistics(
      int databaseId,
      DateTime? lastProcessedTimeStamp,
      int maxRecordCount,
      int periodInMinutes)
    {
      this.PrepareStoredProcedure("DIAGNOSTIC.prc_QueryDatabasePerformanceStatistics");
      this.BindNullableDateTime2("@lastProcessedTimestamp", lastProcessedTimeStamp);
      this.BindInt("@maxRecordCount", maxRecordCount);
      this.BindInt("@periodInMinutes", periodInMinutes);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>((ObjectBinder<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>) new DiagnosticComponent.DatabasePerformanceStatisticsViewBinder(string.Empty));
        return resultCollection.GetCurrent<DiagnosticComponent.DatabasePerformanceStatisticsViewResult>().Items;
      }
    }

    public virtual List<LogspaceSummary> GetLogUtilization(
      int transactionsOlderThanSeconds,
      long transactionsMinimumBytes,
      out List<LogspaceDetails> details)
    {
      details = new List<LogspaceDetails>();
      return new List<LogspaceSummary>();
    }

    public virtual DatabaseResourceStats QueryDbmsResourceStats(int samples) => (DatabaseResourceStats) null;

    public virtual List<TableSpaceUsage> QueryTableSpaceUsage() => new List<TableSpaceUsage>();

    public virtual List<QDSExpensiveQuery> QueryQDS(DateTime start, DateTime end) => new List<QDSExpensiveQuery>();

    public virtual List<VirtualFileStats> QueryVirtualFileStats()
    {
      string sqlStatement = "SELECT * FROM sys.dm_io_virtual_file_stats(DB_ID(), NULL)";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, "dm_db_virtual_file_stats", this.RequestContext))
        {
          resultCollection.AddBinder<VirtualFileStats>((ObjectBinder<VirtualFileStats>) new VirtualFileStatsBinder());
          return resultCollection.GetCurrent<VirtualFileStats>().Items;
        }
      }
    }

    public virtual List<MemoryClerkInfo> QueryMemoryClerks() => new List<MemoryClerkInfo>();

    public List<TraceFlagStatus> QueryTraceFlags()
    {
      string sqlStatement = "dbcc tracestatus";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.ExecuteReader();
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.DataReader, "dbcc tracestatus", this.RequestContext);
      resultCollection.AddBinder<TraceFlagStatus>((ObjectBinder<TraceFlagStatus>) new TraceFlagStatusBinder());
      return resultCollection.GetCurrent<TraceFlagStatus>().Items;
    }

    public virtual List<ResourceSemaphoreInfo> QueryResourceSemaphores() => new List<ResourceSemaphoreInfo>();

    public virtual List<QueryOptimizerMemoryGatewaysInfo> QueryOptimizerMemoryGateways() => new List<QueryOptimizerMemoryGatewaysInfo>();

    public virtual List<PerformanceCounterInfo> QueryPerformanceCounters() => new List<PerformanceCounterInfo>();

    public virtual List<GeoReplicationLinkStatus> QueryGeoReplicationLinkStatus() => new List<GeoReplicationLinkStatus>();

    public virtual List<TuningRecommendation> QueryTuningRecommendations(DateTime lastCheck) => new List<TuningRecommendation>();

    public virtual List<TableName> QueryEmptyTables() => new List<TableName>();

    public virtual List<SpinlockInformation> QuerySpinlocks()
    {
      string sqlStatement = "\r\nIF (DIAGNOSTIC.func_IsAtLeastS2DB() = 1)\r\nBEGIN\r\n    SELECT [name], collisions, spins, spins_per_collision, sleep_time, backoffs \r\n    FROM   sys.dm_os_spinlock_stats\r\n    WHERE  [collisions] > 0\r\nEND";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      using (IDataReader reader = (IDataReader) this.ExecuteReader())
      {
        using (ResultCollection resultCollection = new ResultCollection(reader, "DIAGNOSTIC.prc_QuerySpinlocks", this.RequestContext))
        {
          resultCollection.AddBinder<SpinlockInformation>((ObjectBinder<SpinlockInformation>) new SpinlockInformationBinder());
          return resultCollection.GetCurrent<SpinlockInformation>().Items;
        }
      }
    }

    public virtual IReadOnlyList<DatabaseConnectionInfo> QueryConnectionInfo() => (IReadOnlyList<DatabaseConnectionInfo>) Array.Empty<DatabaseConnectionInfo>();

    public virtual List<DatabaseServicePrincipal> QueryDatabaseServicePrincipals() => new List<DatabaseServicePrincipal>();

    public virtual List<DatabasePrincipal> QueryDatabasePrincipals() => new List<DatabasePrincipal>();

    public virtual List<SqlRowLockInfo> QueryRowLockInfo(List<string> hobtIds) => new List<SqlRowLockInfo>();

    public virtual Task<IReadOnlyList<XEventSession>> QueryXEventSessions() => Task.FromResult<IReadOnlyList<XEventSession>>((IReadOnlyList<XEventSession>) Array.Empty<XEventSession>());

    public virtual List<SqlObjectLockInfo> QueryObjectLockInfo(List<string> objectIds) => new List<SqlObjectLockInfo>();

    internal class DatabasePerformanceStatisticsViewResult
    {
      public DateTime PeriodStart;
      public Decimal AverageCpuPercentage;
      public Decimal MaximumCpuPercentage;
      public Decimal AverageDataIOPercentage;
      public Decimal MaximumDataIOPercentage;
      public Decimal AverageLogWriteUtilizationPercentage;
      public Decimal MaximumLogWriteUtilizationPercentage;
      public Decimal AverageMemoryUsagePercentage;
      public Decimal MaximumMemoryUsagePercentage;
      public string ServiceObjective;
      public Decimal AverageWorkerPercentage;
      public Decimal MaximumWorkerPercentage;
      public Decimal AverageSessionPercentage;
      public Decimal MaximumSessionPercentage;
      public short DtuLimit;
      public Decimal AverageXtpStoragePercent;
      public Decimal MaximumXtpStoragePercent;
      public string ResourceVersion;
      public int Schedulers;
      public Decimal AverageInstanceCpuPercentage;
      public Decimal AverageInstanceMemoryPercentage;
      public short ReplicaRole;
      public short CompatibilityLevel;
      public long RedoQueueSize;
      public long RedoRate;
      public Decimal SecondaryLagSeconds;
      public short SynchronizationHealth;
      public string ServiceObjectiveHardware;

      public override string ToString() => string.Format("PeriodStart:{0} AverageCpuPercentage:{1} MaximumCpuPercentage:{2} AverageDataIOPercentage:{3} MaximumDataIOPercentage:{4} AverageLogWriteUtilizationPercentage:{5} MaximumLogWriteUtilizationPercentage:{6} AverageMemoryUsagePercentage:{7} MaximumMemoryUsagePercentage:{8} ServiceObjective:{9} AverageWorkerPercent {10} MaximumWorkerPercent {11} AverageSessionPercent {12} MaximumSessionPercent {13} DtuLimit {14} AverageXtpStoragePercent {15} MaximumXtpStoragePercent {16} ResourceVersion {17} Schedulers {18} AverageInstanceCpuPercentage {19} AverageInstanceMemoryPercentage {20} ReplicaRole {21} CompatibilityLevel {22} RedoQueueSize {23} RedoRate {24} SecondaryLagSeconds {25} SynchronizationHealth {26} ServiceObjectiveHardware {27}", (object) this.PeriodStart, (object) this.AverageCpuPercentage, (object) this.MaximumCpuPercentage, (object) this.AverageDataIOPercentage, (object) this.MaximumDataIOPercentage, (object) this.AverageLogWriteUtilizationPercentage, (object) this.MaximumLogWriteUtilizationPercentage, (object) this.AverageMemoryUsagePercentage, (object) this.MaximumMemoryUsagePercentage, (object) this.ServiceObjective, (object) this.AverageWorkerPercentage, (object) this.MaximumWorkerPercentage, (object) this.AverageSessionPercentage, (object) this.MaximumSessionPercentage, (object) this.DtuLimit, (object) this.AverageXtpStoragePercent, (object) this.MaximumXtpStoragePercent, (object) this.ResourceVersion, (object) this.Schedulers, (object) this.AverageInstanceCpuPercentage, (object) this.AverageInstanceMemoryPercentage, (object) this.ReplicaRole, (object) this.CompatibilityLevel, (object) this.RedoQueueSize, (object) this.RedoRate, (object) this.SecondaryLagSeconds, (object) this.SynchronizationHealth, (object) this.ServiceObjectiveHardware);
    }

    internal class DatabasePerformanceStatisticsViewBinder : 
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

      private string ServiceObjective { get; set; }

      internal DatabasePerformanceStatisticsViewBinder(string serviceObjective) => this.ServiceObjective = serviceObjective;

      internal DatabasePerformanceStatisticsViewBinder(SqlDataReader reader)
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
        ServiceObjective = this.ServiceObjective
      };
    }
  }
}
