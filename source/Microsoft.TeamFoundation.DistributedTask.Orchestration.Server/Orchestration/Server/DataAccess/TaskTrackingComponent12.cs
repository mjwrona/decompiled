// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent12
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent12 : TaskTrackingComponent11
  {
    public override TaskOrchestrationPlanReference GetPlanData(Guid scopeIdentifier, Guid planId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPlanData)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPlan");
        if (scopeIdentifier != Guid.Empty)
          this.BindDataspaceId(scopeIdentifier);
        else
          this.BindInt("@dataspaceId", 0);
        this.BindGuid("@planId", planId);
        this.BindBoolean("@includeSubscriptions", true);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlanReference>((ObjectBinder<TaskOrchestrationPlanReference>) new TaskOrchestrationPlanReferenceBinder2((TaskTrackingComponent) this));
          return resultCollection.GetCurrent<TaskOrchestrationPlanReference>().FirstOrDefault<TaskOrchestrationPlanReference>();
        }
      }
    }
  }
}
