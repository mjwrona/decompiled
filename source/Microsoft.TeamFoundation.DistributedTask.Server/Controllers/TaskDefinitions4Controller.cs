// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskDefinitions4Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "tasks")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class TaskDefinitions4Controller : TaskDefinitions3Controller
  {
    [HttpGet]
    [ClientResponseType(typeof (IList<TaskDefinition>), null, null)]
    public override HttpResponseMessage GetTaskDefinitions(
      Guid? taskId = null,
      [FromUri] IEnumerable<string> visibility = null,
      [FromUri] bool scopeLocal = false,
      [FromUri] bool allVersions = false)
    {
      IList<TaskDefinition> taskDefinitions = this.TaskService.GetTaskDefinitions(this.TfsRequestContext, taskId, visibility: visibility, scopeLocal: scopeLocal, allVersions: allVersions);
      if (taskDefinitions.Count == 0 && taskId.HasValue)
        throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionIdNotFound((object) taskId));
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      foreach (TaskDefinition taskDefinition in (IEnumerable<TaskDefinition>) taskDefinitions)
        taskDefinition.IconUrl = service.GetResourceUri(this.TfsRequestContext, "distributedtask", TaskResourceIds.TaskIcons, (object) new
        {
          taskId = taskDefinition.Id,
          versionString = taskDefinition.Version.ToString()
        }).AbsoluteUri;
      return this.Request.CreateResponse<IList<TaskDefinition>>(HttpStatusCode.OK, taskDefinitions);
    }
  }
}
