// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskAgentQueuesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "queues")]
  public sealed class TaskAgentQueuesController : DistributedTaskProjectApiController
  {
    [HttpPost]
    public TaskAgentQueue AddAgentQueue(TaskAgentQueue queue, bool authorizePipelines = false)
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      return this.ResourceService.AddAgentQueue(this.TfsRequestContext, this.ProjectId, queue, authorizePipelines);
    }

    [HttpDelete]
    public void DeleteAgentQueue(int queueId)
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      this.ResourceService.DeleteAgentQueue(this.TfsRequestContext, this.ProjectId, queueId);
    }

    [HttpGet]
    public TaskAgentQueue GetAgentQueue(int queueId, TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      return this.ResourceService.GetAgentQueue(this.TfsRequestContext, this.ProjectId, queueId, actionFilter) ?? throw new TaskAgentQueueNotFoundException(TaskResources.QueueNotFound((object) queueId));
    }

    [HttpGet]
    public IList<TaskAgentQueue> GetAgentQueues(
      [ClientQueryParameter] string queueName = null,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      return this.ResourceService.GetAgentQueues(this.TfsRequestContext, this.ProjectId, queueName, actionFilter);
    }

    [HttpGet]
    public IList<TaskAgentQueue> GetAgentQueuesByIds(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string queueIds,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      return this.ResourceService.GetAgentQueues(this.TfsRequestContext, this.ProjectId, (IEnumerable<int>) DistributedTaskApiControllerHelper.ParseArray(queueIds), actionFilter);
    }

    [HttpGet]
    public IList<TaskAgentQueue> GetAgentQueuesByNames(
      [ClientParameterAsIEnumerable(typeof (string), ',')] string queueNames,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      string[] names;
      if (queueNames == null)
        names = (string[]) null;
      else
        names = queueNames.Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
      if (names == null)
        names = Array.Empty<string>();
      return this.ResourceService.GetAgentQueues(this.TfsRequestContext, this.ProjectId, (IEnumerable<string>) names, actionFilter);
    }

    [HttpGet]
    public IList<TaskAgentQueue> GetAgentQueuesForPools(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string poolIds,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      return this.ResourceService.GetAgentQueuesForPools(this.TfsRequestContext, this.ProjectId, (IEnumerable<int>) DistributedTaskApiControllerHelper.ParseArray(poolIds), actionFilter);
    }

    [ClientInternalUseOnly(false)]
    [HttpPut]
    public void CreateTeamProject()
    {
      ArgumentUtility.CheckForEmptyGuid(this.ProjectId, "ProjectId", "DistributedTask");
      this.ResourceService.CreateTeamProject(this.TfsRequestContext, this.ProjectId);
    }
  }
}
