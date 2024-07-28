// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskRequestQueueComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskRequestQueueComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<TaskRequestQueueComponent>(1),
      (IComponentCreator) new ComponentCreator<TaskRequestQueueComponent2>(2)
    }, "DistributedTaskRequest");

    public TaskRequestQueueComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void AddTaskRequestMessage(
      Guid taskInstanceId,
      TaskRequestMessageType messageType,
      string message)
    {
      this.PrepareStoredProcedure("Task.prc_AddTaskRequestMessage");
      this.BindGuid("@taskInstanceId", taskInstanceId);
      this.BindByte("@messagetype", (byte) messageType);
      this.BindString("@message", message, int.MaxValue, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual List<TaskRequestQueueMessage> GetTopTaskInstanceRequestMessages(int top)
    {
      this.PrepareStoredProcedure("Task.prc_GetTopTaskInstanceRequestMessages");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TaskRequestQueueMessage>((ObjectBinder<TaskRequestQueueMessage>) new TaskRequestQueueBinder(this));
        return resultCollection.GetCurrent<TaskRequestQueueMessage>().Items;
      }
    }

    public virtual void DeleteTaskRequestMessages(List<int> messageIds)
    {
      this.PrepareStoredProcedure("Task.prc_DeleteTaskRequestMessages");
      this.BindInt32Table("@messageIds", (IEnumerable<int>) messageIds);
      this.ExecuteNonQuery();
    }
  }
}
