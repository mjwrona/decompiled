// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent2
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
  internal class ServicingComponent2 : ServicingComponent
  {
    private static readonly SqlMetaData[] typ_ServicingStepDetail = new SqlMetaData[7]
    {
      new SqlMetaData("OperationName", SqlDbType.VarChar, 64L),
      new SqlMetaData("GroupName", SqlDbType.VarChar, 64L),
      new SqlMetaData("StepName", SqlDbType.VarChar, 128L),
      new SqlMetaData("DetailTime", SqlDbType.DateTime),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("EntryKind", SqlDbType.TinyInt),
      new SqlMetaData("Message", SqlDbType.NVarChar, -1L)
    };

    protected virtual SqlParameter BindServicingStepDetailTable(
      string parameterName,
      IEnumerable<ServicingStepDetail> rows)
    {
      if (rows == null)
        rows = Enumerable.Empty<ServicingStepDetail>();
      System.Func<ServicingStepDetail, SqlDataRecord> selector = (System.Func<ServicingStepDetail, SqlDataRecord>) (detail =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServicingComponent2.typ_ServicingStepDetail);
        sqlDataRecord.SetString(0, detail.ServicingOperation ?? string.Empty);
        sqlDataRecord.SetString(1, detail.ServicingStepGroupId ?? string.Empty);
        sqlDataRecord.SetString(2, detail.ServicingStepId ?? string.Empty);
        sqlDataRecord.SetDateTime(3, detail.DetailTime);
        if (detail is ServicingStepLogEntry servicingStepLogEntry2)
        {
          sqlDataRecord.SetByte(5, (byte) servicingStepLogEntry2.EntryKind);
          sqlDataRecord.SetString(6, servicingStepLogEntry2.Message);
        }
        else
        {
          ServicingStepStateChange servicingStepStateChange = (ServicingStepStateChange) detail;
          sqlDataRecord.SetByte(4, (byte) servicingStepStateChange.StepState);
        }
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ServicingStepDetail", rows.Select<ServicingStepDetail, SqlDataRecord>(selector));
    }

    public void AddServicingStepDetails(
      Guid jobId,
      DateTime queueTime,
      ICollection<ServicingStepDetail> details,
      Guid hostId,
      short completedStepCount)
    {
      this.PrepareStoredProcedure("prc_AddServicingStepDetails");
      this.BindGuid("@jobId", jobId);
      this.BindDateTime("@queueTime", queueTime);
      this.BindServicingStepDetailTable("@stepDetails", (IEnumerable<ServicingStepDetail>) details);
      this.BindGuid("@hostId", hostId);
      if (completedStepCount >= (short) 0)
        this.BindShort("@completedStepCount", completedStepCount);
      this.ExecuteNonQuery();
    }

    public override ResultCollection QueryServicingStepDetails(
      Guid hostId,
      Guid jobId,
      DateTime queueTime,
      ServicingStepDetailFilterOptions filterOptions,
      long minDetailId)
    {
      this.PrepareStoredProcedure("prc_QueryServicingStepDetails");
      if (this.Version >= 12)
        this.BindGuid("@hostId", hostId);
      this.BindGuid("@jobId", jobId);
      this.BindInt("@filterOptions", (int) filterOptions);
      if (minDetailId > 0L)
        this.BindLong("@minDetailId", minDetailId);
      if (filterOptions == ServicingStepDetailFilterOptions.SpecificQueueTime)
        this.BindDateTime("@queueTime", queueTime.ToUniversalTime());
      else
        this.BindNullValue("@queueTime", SqlDbType.DateTime);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServicingStepDetails", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      resultCollection.AddBinder<ServicingStepDetail>((ObjectBinder<ServicingStepDetail>) new ServicingStepDetailColumns2());
      return resultCollection;
    }
  }
}
