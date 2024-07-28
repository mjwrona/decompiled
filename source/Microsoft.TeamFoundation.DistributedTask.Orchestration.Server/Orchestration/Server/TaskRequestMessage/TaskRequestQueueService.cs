// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage.TaskRequestQueueService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage
{
  internal class TaskRequestQueueService : ITaskRequestQueueService, IVssFrameworkService
  {
    private const string c_layer = "TaskRequestQueueService";
    private readonly string m_serverTaskRequestProcessorJobExtensionName = "Microsoft.TeamFoundation.DistributedTask.Orchestration.Plugins.ServerTaskRequestProcessorJob";
    private readonly string m_serverTaskRequestProcessorJobName = "ServerTaskRequestProcessorJob";
    private readonly Guid m_serverTaskRequestProcessorJobGuid = new Guid("F3EFDB50-556B-4661-A6C3-C68AC9B2B899");
    private static string m_drawerName = "/DistributedTask/Sdk/TaskMessageQueueDrawerName";
    private static string m_lookupKey = "AesEncryption";

    public virtual void AddMessage(
      IVssRequestContext requestContext,
      Guid taskInstanceId,
      TaskRequestMessageType messageType,
      string message)
    {
      using (new MethodScope(requestContext, nameof (TaskRequestQueueService), nameof (AddMessage)))
      {
        message = EncryptionHelper.Encrypt(requestContext, message, (IAesEncryptionKeyProvider) new StrongBoxBasedEncryptionKeyProvider(TaskRequestQueueService.m_drawerName, TaskRequestQueueService.m_lookupKey, true));
        using (TaskRequestQueueComponent component = requestContext.CreateComponent<TaskRequestQueueComponent>())
          component.AddTaskRequestMessage(taskInstanceId, messageType, message);
        this.QueueServerTaskRequestProcessorJob(requestContext);
      }
    }

    public virtual IEnumerable<TaskRequestQueueMessage> GetTopMessages(
      IVssRequestContext requestContext,
      int top)
    {
      using (new MethodScope(requestContext, nameof (TaskRequestQueueService), nameof (GetTopMessages)))
      {
        List<TaskRequestQueueMessage> instanceRequestMessages;
        using (TaskRequestQueueComponent component = requestContext.CreateComponent<TaskRequestQueueComponent>())
          instanceRequestMessages = component.GetTopTaskInstanceRequestMessages(top);
        if (instanceRequestMessages.Count > 0)
        {
          StrongBoxBasedEncryptionKeyProvider strongBoxBasedKeyProvider = new StrongBoxBasedEncryptionKeyProvider(TaskRequestQueueService.m_drawerName, TaskRequestQueueService.m_lookupKey, false);
          instanceRequestMessages.ForEach((Action<TaskRequestQueueMessage>) (message => message.Message = EncryptionHelper.Decrypt(requestContext, message.Message, (IAesEncryptionKeyProvider) strongBoxBasedKeyProvider)));
        }
        return (IEnumerable<TaskRequestQueueMessage>) instanceRequestMessages;
      }
    }

    public virtual void DeleteMessages(IVssRequestContext requestContext, List<int> messageIds)
    {
      using (new MethodScope(requestContext, nameof (TaskRequestQueueService), nameof (DeleteMessages)))
      {
        using (TaskRequestQueueComponent component = requestContext.CreateComponent<TaskRequestQueueComponent>())
          component.DeleteTaskRequestMessages(messageIds);
      }
    }

    private void QueueServerTaskRequestProcessorJob(IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          this.m_serverTaskRequestProcessorJobGuid
        });
      }
      catch (JobDefinitionNotFoundException ex)
      {
        TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(this.m_serverTaskRequestProcessorJobGuid, this.m_serverTaskRequestProcessorJobName, this.m_serverTaskRequestProcessorJobExtensionName);
        service.UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          this.m_serverTaskRequestProcessorJobGuid
        });
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
