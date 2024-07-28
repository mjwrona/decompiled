// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskRequestQueueComponent2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskRequestQueueComponent2 : TaskRequestQueueComponent
  {
    public TaskRequestQueueComponent2() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public override List<TaskRequestQueueMessage> GetTopTaskInstanceRequestMessages(int top)
    {
      this.PrepareStoredProcedure("Task.prc_GetTopTaskInstanceRequestMessages");
      this.BindInt("@top", top);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TaskRequestQueueMessage>((ObjectBinder<TaskRequestQueueMessage>) new TaskRequestQueueBinder((TaskRequestQueueComponent) this));
        return resultCollection.GetCurrent<TaskRequestQueueMessage>().Items;
      }
    }
  }
}
