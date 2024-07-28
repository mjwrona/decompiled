// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskGroups4Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "taskgroups")]
  public class TaskGroups4Controller : TaskGroups3Controller
  {
    [HttpPut]
    [ClientExample("PUT__UpdateTaskGroup.json", "Update a task group", null, null)]
    public virtual TaskGroup UpdateTaskGroup(Guid taskGroupId, [FromBody] TaskGroupUpdateParameter taskGroup)
    {
      if (taskGroup == null)
        throw new InvalidRequestException(TaskResources.InvalidTaskGroupInput());
      if (taskGroup.Id != taskGroupId)
        throw new TaskGroupIdConflictException(TaskResources.WrongIdSpecifiedForTaskGroupId((object) taskGroupId, (object) taskGroup.Id));
      return this.TfsRequestContext.GetService<MetaTaskService>().UpdateTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroup);
    }

    [HttpGet]
    [ClientInternalUseOnly(false)]
    public virtual TaskGroup GetTaskGroup(
      Guid taskGroupId,
      string versionSpec,
      [FromUri(Name = "$expand")] TaskGroupExpands expands = TaskGroupExpands.None)
    {
      return this.TfsRequestContext.GetService<MetaTaskService>().GetTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroupId, versionSpec, new TaskGroupExpands?(expands));
    }
  }
}
