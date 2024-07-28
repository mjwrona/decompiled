// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentMessagesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "messages")]
  public sealed class TaskAgentMessagesController : DistributedTaskApiController
  {
    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public Task DeleteMessage(int poolId, Guid sessionId, long messageId) => this.ResourceService.DeleteMessageAsync(this.TfsRequestContext, poolId, sessionId, messageId);

    [HttpGet]
    [ClientResponseType(typeof (TaskAgentMessage), null, null)]
    [MethodInformation(IsLongRunning = true, MethodType = MethodType.LightWeight)]
    public async Task<HttpResponseMessage> GetMessage(
      int poolId,
      Guid sessionId,
      long? lastMessageId = null)
    {
      TaskAgentMessagesController messagesController = this;
      TaskAgentMessage messageAsync = await messagesController.ResourceService.GetMessageAsync(messagesController.TfsRequestContext, poolId, sessionId, TimeSpan.FromSeconds(50.0), lastMessageId);
      return messageAsync != null ? messagesController.Request.CreateResponse<TaskAgentMessage>(HttpStatusCode.OK, messageAsync) : messagesController.Request.CreateResponse(HttpStatusCode.Accepted);
    }

    [HttpPost]
    public void SendMessage(int poolId, long requestId, TaskAgentMessage message) => this.ResourceService.SendJobMessageToAgent(this.TfsRequestContext, poolId, requestId, message);

    [HttpPost]
    public async Task RefreshAgent(int poolId, int agentId)
    {
      TaskAgentMessagesController messagesController = this;
      await messagesController.ResourceService.SendRefreshMessageToAgentAsync(messagesController.TfsRequestContext, poolId, agentId);
    }

    [HttpPost]
    public async Task RefreshAgents(int poolId)
    {
      TaskAgentMessagesController messagesController = this;
      await messagesController.ResourceService.SendRefreshMessageToAgentsAsync(messagesController.TfsRequestContext, poolId);
    }
  }
}
