// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskGroups2Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "taskgroups")]
  public class TaskGroups2Controller : TaskGroupsController
  {
    [HttpGet]
    [ClientInternalUseOnly(false)]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    public virtual HttpResponseMessage GetTaskGroupRevision(Guid taskGroupId, int revision)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(this.TfsRequestContext.GetService<MetaTaskService>().GetRevision(this.TfsRequestContext, this.ProjectId, taskGroupId, revision));
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
      return response;
    }
  }
}
