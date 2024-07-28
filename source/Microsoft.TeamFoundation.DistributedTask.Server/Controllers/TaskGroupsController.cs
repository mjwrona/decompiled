// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskGroupsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "taskgroups")]
  public class TaskGroupsController : DistributedTaskProjectApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<TaskGroup>), null, null)]
    [ClientExample("LIST__ListAllTaskGroups.json", "List all task groups", null, null)]
    [ClientExample("LIST__ListAllVersionsOfTaskGroup.json", "List all versions of a task group", null, null)]
    public virtual HttpResponseMessage GetTaskGroups(
      Guid? taskGroupId = null,
      bool? expanded = false,
      Guid? taskIdFilter = null,
      bool? deleted = false,
      [FromUri(Name = "$top")] int top = -1,
      DateTime? continuationToken = null,
      TaskGroupQueryOrder queryOrder = TaskGroupQueryOrder.CreatedOnDescending)
    {
      List<TaskGroup> taskGroups1 = this.TfsRequestContext.GetService<MetaTaskService>().GetTaskGroups(this.TfsRequestContext, this.ProjectId, taskGroupId, expanded, taskIdFilter, deleted, continuationToken, top + 1, queryOrder);
      IEnumerable<TaskGroup> taskGroups2 = top < 0 ? (IEnumerable<TaskGroup>) taskGroups1 : taskGroups1.Take<TaskGroup>(top);
      HttpResponseMessage responseMessage = (HttpResponseMessage) null;
      try
      {
        responseMessage = this.Request.CreateResponse<IEnumerable<TaskGroup>>(HttpStatusCode.OK, taskGroups2);
        if (top >= 0)
        {
          if (taskGroups1.Count > top)
          {
            if (taskGroups1[top] != null)
            {
              string tokenValue = (string) null;
              switch (queryOrder)
              {
                case TaskGroupQueryOrder.CreatedOnAscending:
                case TaskGroupQueryOrder.CreatedOnDescending:
                  tokenValue = taskGroups1[top].CreatedOn.ToString((IFormatProvider) CultureInfo.InvariantCulture);
                  break;
              }
              DistributedTaskProjectApiController.SetContinuationToken(responseMessage, tokenValue);
            }
          }
        }
      }
      catch (Exception ex)
      {
        responseMessage?.Dispose();
        throw;
      }
      return responseMessage;
    }

    [HttpPost]
    [ClientExample("POST__CreateTaskGroup.json", "Create a task group", null, null)]
    public virtual TaskGroup AddTaskGroup([FromBody] TaskGroupCreateParameter taskGroup) => this.TfsRequestContext.GetService<MetaTaskService>().AddTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroup);

    [HttpPut]
    [ClientInternalUseOnly(false)]
    [Obsolete("Use UpdateTaskGroup(Guid taskGroupId, [FromBody] TaskGroupUpdateParameter taskGroup) instead", false)]
    public virtual TaskGroup UpdateTaskGroup([FromBody] TaskGroupUpdateParameter taskGroup) => this.TfsRequestContext.GetService<MetaTaskService>().UpdateTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroup);

    [HttpDelete]
    [ClientExample("DELETE__DeleteATaskGroup.json", "Delete a task group", null, null)]
    public virtual void DeleteTaskGroup(Guid taskGroupId, string comment = null) => this.TfsRequestContext.GetService<MetaTaskService>().SoftDeleteTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroupId, comment);
  }
}
