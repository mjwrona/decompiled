// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.EnvironmentDeploymentExecutionHistoryComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class EnvironmentDeploymentExecutionHistoryComponent2 : 
    EnvironmentDeploymentExecutionHistoryComponent
  {
    public override EnvironmentResourceDeploymentExecutionRecord QueueEnvironmentResourceDeploymentRequest(
      EnvironmentResourceDeploymentExecutionRecord record)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (QueueEnvironmentResourceDeploymentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_QueueEnvironmentResourceDeploymentRequest");
        this.BindInt("@environmentId", record.EnvironmentId);
        this.BindLong("@requestId", record.RequestId);
        this.BindInt("@resourceId", record.ResourceId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentResourceDeploymentExecutionRecord>((ObjectBinder<EnvironmentResourceDeploymentExecutionRecord>) new EnvironmentResourceDeploymentExecutionRecordBinder((EnvironmentDeploymentExecutionHistoryComponent) this));
          return resultCollection.GetCurrent<EnvironmentResourceDeploymentExecutionRecord>().Items.FirstOrDefault<EnvironmentResourceDeploymentExecutionRecord>();
        }
      }
    }

    public override EnvironmentResourceDeploymentExecutionRecord UpdateEnvironmentResourceDeploymentRequest(
      int environmentId,
      long requestId,
      int resourceId,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateEnvironmentResourceDeploymentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateEnvironmentResourceDeploymentRequest");
        this.BindInt("@environmentId", environmentId);
        this.BindLong("@requestId", requestId);
        this.BindInt("@resourceId", resourceId);
        this.BindNullableDateTime2("@finishTime", finishTime);
        if (result.HasValue)
          this.BindByte("@result", (byte) result.Value);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<EnvironmentResourceDeploymentExecutionRecord>((ObjectBinder<EnvironmentResourceDeploymentExecutionRecord>) new EnvironmentResourceDeploymentExecutionRecordBinder((EnvironmentDeploymentExecutionHistoryComponent) this));
          return resultCollection.GetCurrent<EnvironmentResourceDeploymentExecutionRecord>().Items.FirstOrDefault<EnvironmentResourceDeploymentExecutionRecord>();
        }
      }
    }
  }
}
