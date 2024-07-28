// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent11
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent11 : ServicingComponent10
  {
    public override ServicingJobDetail UpdateServicingJobDetail(
      Guid hostId,
      Guid jobId,
      string operationClass,
      string operations,
      DateTime queueTime,
      ServicingJobStatus jobStatus,
      ServicingJobResult result,
      short totalStepCount)
    {
      this.PrepareStoredProcedure("prc_UpdateServicingJobDetail");
      this.BindGuid("@hostId", hostId);
      this.BindGuid("@jobId", jobId);
      this.BindString("@operationClass", operationClass, 64, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindDateTime("@queueTime", queueTime);
      this.BindString("@operations", operations, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindInt("@jobStatus", (int) jobStatus);
      this.BindShort("@totalStepCount", totalStepCount);
      if (jobStatus == ServicingJobStatus.Complete)
        this.BindInt("@result", (int) result);
      else
        this.BindNullValue("@result", SqlDbType.Int);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_UpdateServicingJobDetail", this.RequestContext);
      resultCollection.AddBinder<ServicingJobDetail>((ObjectBinder<ServicingJobDetail>) new ServicingJobDetailColumns());
      return resultCollection.GetCurrent<ServicingJobDetail>().SingleOrDefault<ServicingJobDetail>();
    }
  }
}
