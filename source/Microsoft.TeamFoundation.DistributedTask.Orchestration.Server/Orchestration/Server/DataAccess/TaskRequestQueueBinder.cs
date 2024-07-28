// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskRequestQueueBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal sealed class TaskRequestQueueBinder : ObjectBinder<TaskRequestQueueMessage>
  {
    private SqlColumnBinder messageId = new SqlColumnBinder("MessageId");
    private SqlColumnBinder taskInstanceId = new SqlColumnBinder("TaskInstanceId");
    private SqlColumnBinder queuedTime = new SqlColumnBinder("QueuedTime");
    private SqlColumnBinder messageType = new SqlColumnBinder("MessageType");
    private SqlColumnBinder message = new SqlColumnBinder("Message");
    private TaskRequestQueueComponent m_component;

    public TaskRequestQueueBinder(TaskRequestQueueComponent component) => this.m_component = component;

    protected override TaskRequestQueueMessage Bind() => new TaskRequestQueueMessage()
    {
      MessageId = this.messageId.GetInt32((IDataReader) this.Reader),
      TaskInstanceId = this.taskInstanceId.GetGuid((IDataReader) this.Reader, false),
      QueuedTime = this.queuedTime.GetDateTime((IDataReader) this.Reader),
      MessageType = (TaskRequestMessageType) this.messageType.GetByte((IDataReader) this.Reader),
      Message = this.message.GetString((IDataReader) this.Reader, false)
    };
  }
}
