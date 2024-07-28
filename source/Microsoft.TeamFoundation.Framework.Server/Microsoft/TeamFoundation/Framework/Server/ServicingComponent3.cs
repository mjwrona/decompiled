// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent3
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent3 : ServicingComponent2
  {
    private static readonly SqlMetaData[] typ_ServicingStepDetail2 = new SqlMetaData[8]
    {
      new SqlMetaData("SequenceNumber", SqlDbType.Int),
      new SqlMetaData("OperationName", SqlDbType.VarChar, 64L),
      new SqlMetaData("GroupName", SqlDbType.VarChar, 64L),
      new SqlMetaData("StepName", SqlDbType.VarChar, 128L),
      new SqlMetaData("DetailTime", SqlDbType.DateTime),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("EntryKind", SqlDbType.TinyInt),
      new SqlMetaData("Message", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_ServicingJobDetail = new SqlMetaData[11]
    {
      new SqlMetaData("HostId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("OperationClass", SqlDbType.VarChar, 64L),
      new SqlMetaData("Operations", SqlDbType.VarChar, -1L),
      new SqlMetaData("JobStatus", SqlDbType.Int),
      new SqlMetaData("Result", SqlDbType.Int),
      new SqlMetaData("QueueTime", SqlDbType.DateTime),
      new SqlMetaData("StartTime", SqlDbType.DateTime),
      new SqlMetaData("EndTime", SqlDbType.DateTime),
      new SqlMetaData("CompletedStepCount", SqlDbType.SmallInt),
      new SqlMetaData("TotalStepCount", SqlDbType.SmallInt)
    };

    protected override SqlParameter BindServicingStepDetailTable(
      string parameterName,
      IEnumerable<ServicingStepDetail> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<ServicingStepDetail>();
      int sequenceNumber = 0;
      System.Func<ServicingStepDetail, SqlDataRecord> selector = (System.Func<ServicingStepDetail, SqlDataRecord>) (detail =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServicingComponent3.typ_ServicingStepDetail2);
        sqlDataRecord.SetInt32(0, ++sequenceNumber);
        sqlDataRecord.SetString(1, detail.ServicingOperation ?? string.Empty);
        sqlDataRecord.SetString(2, detail.ServicingStepGroupId ?? string.Empty);
        sqlDataRecord.SetString(3, detail.ServicingStepId ?? string.Empty);
        sqlDataRecord.SetDateTime(4, detail.DetailTime);
        if (detail is ServicingStepLogEntry servicingStepLogEntry2)
        {
          sqlDataRecord.SetByte(6, (byte) servicingStepLogEntry2.EntryKind);
          sqlDataRecord.SetString(7, servicingStepLogEntry2.Message);
        }
        else
        {
          ServicingStepStateChange servicingStepStateChange = (ServicingStepStateChange) detail;
          sqlDataRecord.SetByte(5, (byte) servicingStepStateChange.StepState);
        }
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ServicingStepDetail2", rows.Select<ServicingStepDetail, SqlDataRecord>(selector));
    }

    protected virtual SqlParameter BindServicingJobDetailTable(
      string parameterName,
      IEnumerable<ServicingJobDetail> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<ServicingJobDetail>();
      System.Func<ServicingJobDetail, SqlDataRecord> selector = (System.Func<ServicingJobDetail, SqlDataRecord>) (jobDetail =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServicingComponent3.typ_ServicingJobDetail);
        sqlDataRecord.SetGuid(0, jobDetail.HostId);
        sqlDataRecord.SetGuid(1, jobDetail.JobId);
        sqlDataRecord.SetString(2, jobDetail.OperationClass);
        sqlDataRecord.SetString(3, jobDetail.OperationString);
        sqlDataRecord.SetInt32(4, jobDetail.JobStatusValue);
        if (jobDetail.Result != ServicingJobResult.None)
          sqlDataRecord.SetInt32(5, jobDetail.ResultValue);
        if (jobDetail.QueueTime != DateTime.MinValue)
          sqlDataRecord.SetDateTime(6, jobDetail.QueueTime);
        if (jobDetail.StartTime != DateTime.MinValue)
          sqlDataRecord.SetDateTime(7, jobDetail.StartTime);
        if (jobDetail.EndTime != DateTime.MinValue)
          sqlDataRecord.SetDateTime(8, jobDetail.EndTime);
        if (jobDetail.CompletedStepCount >= (short) 0)
          sqlDataRecord.SetInt16(9, jobDetail.CompletedStepCount);
        if (jobDetail.TotalStepCount >= (short) 0)
          sqlDataRecord.SetInt16(10, jobDetail.TotalStepCount);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ServicingJobDetail", rows.Select<ServicingJobDetail, SqlDataRecord>(selector));
    }

    public List<ServicingJobDetail> AddServicingJobDetails(
      IEnumerable<ServicingJobDetail> servicingJobDetails)
    {
      this.PrepareStoredProcedure("prc_AddServicingJobDetails");
      this.BindServicingJobDetailTable("@jobDetails", servicingJobDetails);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_AddServicingJobDetails", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      return resultCollection.GetCurrent<ServicingJobDetail>().ToList<ServicingJobDetail>();
    }

    public List<ServicingJobDetail> QueryFailedServicingJobs(
      DateTime minQueueTime,
      string operationClass)
    {
      this.PrepareStoredProcedure("prc_QueryFailedServicingJobs");
      this.BindDateTime("@minQueueTime", minQueueTime);
      this.BindString("@operationClass", operationClass, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryFailedServicingJobs", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      return resultCollection.GetCurrent<ServicingJobDetail>().Items;
    }

    public List<ServicingJobDetail> QueryQueuedServicingJobs(
      DateTime minQueueTime,
      string operationClass)
    {
      this.PrepareStoredProcedure("prc_QueryQueuedServicingJobs");
      this.BindDateTime("@minQueueTime", minQueueTime);
      this.BindString("@operationClass", operationClass, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryQueuedServicingJobs", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      return resultCollection.GetCurrent<ServicingJobDetail>().Items;
    }
  }
}
