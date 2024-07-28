// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskGroups3Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "taskgroups")]
  public class TaskGroups3Controller : TaskGroups2Controller
  {
    [HttpPut]
    [ClientInternalUseOnly(false)]
    public virtual IEnumerable<TaskGroup> PublishTaskGroup(
      Guid parentTaskGroupId,
      [FromBody] PublishTaskGroupMetadata taskGroupMetadata)
    {
      return this.TfsRequestContext.GetService<MetaTaskService>().PublishTaskGroup(this.TfsRequestContext, this.ProjectId, parentTaskGroupId, taskGroupMetadata);
    }

    [HttpPatch]
    [ClientInternalUseOnly(false)]
    public virtual IEnumerable<TaskGroup> UpdateTaskGroupProperties(
      Guid taskGroupId,
      [FromBody] TaskGroupUpdatePropertiesBase taskGroupUpdateProperties,
      bool disablePriorVersions = false)
    {
      MetaTaskService service = this.TfsRequestContext.GetService<MetaTaskService>();
      switch (taskGroupUpdateProperties)
      {
        case TaskGroupPublishPreviewParameter publishPreviewParameter:
          if (disablePriorVersions)
            publishPreviewParameter.DisablePriorVersions = true;
          return service.PublishPreviewTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroupId, publishPreviewParameter);
        case TaskGroupRestoreParameter restoreParameter:
          if (restoreParameter.Restore)
            service.UndeleteTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroupId, restoreParameter.Comment);
          else
            this.TfsRequestContext.TraceWarning(10015188, "TaskGroup", TaskResources.SkippingTaskGroupRestore());
          return (IEnumerable<TaskGroup>) service.GetTaskGroups(this.TfsRequestContext, this.ProjectId, new Guid?(taskGroupId), new bool?(false), new Guid?(), new bool?(false), new DateTime?(), 0, TaskGroupQueryOrder.CreatedOnDescending);
        default:
          throw new TaskGroupUpdateFailedException(TaskResources.InValidTaskGroupUpdateProperties());
      }
    }

    [HttpPatch]
    [ClientInternalUseOnly(false)]
    public virtual IEnumerable<TaskGroup> UndeleteTaskGroup(TaskGroup taskGroup)
    {
      MetaTaskService service = this.TfsRequestContext.GetService<MetaTaskService>();
      service.UndeleteTaskGroup(this.TfsRequestContext, this.ProjectId, taskGroup.Id, taskGroup.Comment);
      return (IEnumerable<TaskGroup>) service.GetTaskGroups(this.TfsRequestContext, this.ProjectId, new Guid?(taskGroup.Id), new bool?(false), new Guid?(), new bool?(false), new DateTime?(), 0, TaskGroupQueryOrder.CreatedOnDescending);
    }
  }
}
