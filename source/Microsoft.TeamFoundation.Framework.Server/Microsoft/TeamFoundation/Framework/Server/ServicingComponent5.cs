// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent5 : ServicingComponent4
  {
    private static readonly string s_jobInfoQueryTemplate = "\r\nSELECT  {0} JobId,\r\n        sjd.HostId,\r\n        sjd.OperationClass,\r\n        sjd.Operations,\r\n        td.DatabaseId,\r\n        td.DatabaseName,\r\n        p.PoolName,\r\n        p.PoolId,\r\n        JobStatus,\r\n        Result JobResult,\r\n        QueueTime,\r\n        StartTime,\r\n        EndTime,\r\n        CompletedStepCount,\r\n        TotalStepCount,\r\n        COALESCE(sh.Name, '') Name,\r\n        COALESCE(psh.Name, '') ParentName\r\nFROM    tbl_ServicingJobDetail sjd\r\nLEFT JOIN tbl_ServiceHost sh\r\nON      sjd.HostId = sh.HostId\r\nLEFT JOIN tbl_ServiceHost psh\r\nON      sh.ParentHostId = psh.HostId\r\nLEFT JOIN    tbl_Database td\r\nON      td.DatabaseId = sh.DatabaseId\r\nLEFT JOIN    tbl_DatabasePool p\r\nON      td.PoolId = p.PoolId\r\nWHERE   {1}\r\n{2}";

    public List<ServicingJobsStatsRaw> GetServicingJobsStats(
      DateTime startTime,
      DateTime endTime,
      string operationClass)
    {
      this.PrepareStoredProcedure("prc_GetServicingJobsStats");
      this.BindDateTime("@startTime", startTime);
      this.BindDateTime("@endTime", endTime);
      this.BindString("@operationClass", operationClass, 64, true, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetServicingJobsStats", this.RequestContext);
      resultCollection.AddBinder<ServicingJobsStatsRaw>((ObjectBinder<ServicingJobsStatsRaw>) new ServicingJobsStatsColumns());
      return resultCollection.GetCurrent<ServicingJobsStatsRaw>().ToList<ServicingJobsStatsRaw>();
    }

    public List<ServicingJobInfo> QueryServicingJobsInfo(
      DateTime queueTimeFrom,
      DateTime queueTimeTo,
      string operationClass,
      ServicingJobResult? result,
      ServicingJobStatus? status,
      string databaseName,
      int? databaseId,
      Guid? accountId,
      string poolName,
      int? top,
      IList<KeyValuePair<ServicingJobInfoColumn, SortOrder>> sortOrder)
    {
      string topClause = JobInfoQueryHelper.GetTopClause(top);
      string whereClause = JobInfoQueryHelper.GetWhereClause(queueTimeFrom, queueTimeTo, operationClass, result, status, databaseName, databaseId, accountId, poolName);
      string orderByClause = JobInfoQueryHelper.GetOrderByClause(sortOrder);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServicingComponent5.s_jobInfoQueryTemplate, (object) topClause, (object) whereClause, (object) orderByClause);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindDateTime("@queueTimeFrom", queueTimeFrom);
      this.BindDateTime("@queueTimeTo", queueTimeTo);
      if (!string.IsNullOrEmpty(operationClass))
        this.BindString("@operationClass", JobInfoQueryHelper.ReplaceWildCards(operationClass), 64, true, SqlDbType.VarChar);
      if (result.HasValue)
        this.BindInt("@jobResult", (int) result.Value);
      if (status.HasValue)
        this.BindInt("@jobStatus", (int) status.Value);
      if (!string.IsNullOrEmpty(databaseName))
        this.BindString("@databaseName", JobInfoQueryHelper.ReplaceWildCards(databaseName), 520, false, SqlDbType.VarChar);
      if (databaseId.HasValue)
        this.BindInt("@databaseId", databaseId.Value);
      if (accountId.HasValue)
        this.BindGuid("@accountId", accountId.Value);
      if (!string.IsNullOrEmpty(poolName))
        this.BindString("@poolName", JobInfoQueryHelper.ReplaceWildCards(poolName), 256, false, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ServicingJobInfo>((ObjectBinder<ServicingJobInfo>) new ServicingJobInfoColumns());
      return resultCollection.GetCurrent<ServicingJobInfo>().ToList<ServicingJobInfo>();
    }

    public virtual ServicingJobInfo GetServicingJobInfo(Guid hostId, Guid jobId)
    {
      this.PrepareStoredProcedure("prc_GetServicingJobInfo");
      this.BindGuid("@jobId", jobId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetServicingJobInfo", this.RequestContext);
      resultCollection.AddBinder<ServicingJobInfo>((ObjectBinder<ServicingJobInfo>) new ServicingJobInfoColumns());
      return resultCollection.GetCurrent<ServicingJobInfo>().FirstOrDefault<ServicingJobInfo>((System.Func<ServicingJobInfo, bool>) (sji => sji.HostId == hostId));
    }

    public virtual List<ServicingJobInfo> GetServicingJobsInfo(Guid jobId)
    {
      this.PrepareStoredProcedure("prc_GetServicingJobInfo");
      this.BindGuid("@jobId", jobId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetServicingJobInfo", this.RequestContext);
      resultCollection.AddBinder<ServicingJobInfo>((ObjectBinder<ServicingJobInfo>) new ServicingJobInfoColumns());
      return resultCollection.GetCurrent<ServicingJobInfo>().Items;
    }
  }
}
