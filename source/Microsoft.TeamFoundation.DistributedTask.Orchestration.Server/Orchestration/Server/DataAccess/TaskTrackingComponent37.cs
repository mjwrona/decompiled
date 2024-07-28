// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent37
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent37 : TaskTrackingComponent36
  {
    public override async Task DeletePlanContextsAsync(Guid scopeIdentifier, Guid planId)
    {
      TaskTrackingComponent37 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (DeletePlanContextsAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_DeletePlanContexts");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
    }
  }
}
