// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskDefinitions3Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(2.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "tasks")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class TaskDefinitions3Controller : TaskDefinitionsControllerBase
  {
    protected const string PendingTaskInstallationToken = "x-ms-pendingtasks";

    [HttpGet]
    [ClientResponseType(typeof (IList<TaskDefinition>), null, null)]
    public virtual HttpResponseMessage GetTaskDefinitions(
      Guid? taskId = null,
      [FromUri] IEnumerable<string> visibility = null,
      [FromUri] bool scopeLocal = false,
      [FromUri] bool allVersions = true)
    {
      IList<TaskDefinition> taskDefinitions = this.TaskService.GetTaskDefinitions(this.TfsRequestContext, taskId, visibility: visibility, scopeLocal: scopeLocal, allVersions: allVersions);
      if (taskDefinitions.Count == 0 && taskId.HasValue)
        throw new TaskDefinitionNotFoundException(Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.TaskDefinitionIdNotFound((object) taskId));
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      foreach (TaskDefinition taskDefinition in (IEnumerable<TaskDefinition>) taskDefinitions)
        taskDefinition.IconUrl = service.GetResourceUri(this.TfsRequestContext, "distributedtask", TaskResourceIds.TaskIcons, (object) new
        {
          taskId = taskDefinition.Id,
          versionString = taskDefinition.Version.ToString()
        }).AbsoluteUri;
      return this.Request.CreateResponse<IList<TaskDefinition>>(HttpStatusCode.OK, taskDefinitions);
    }

    [HttpGet]
    [ClientResponseType(typeof (TaskDefinition), null, null)]
    [ClientResponseType(typeof (Stream), "GetTaskContentZip", "application/zip")]
    public HttpResponseMessage GetTaskDefinition(
      Guid taskId,
      string versionString,
      [FromUri] IEnumerable<string> visibility = null,
      [FromUri] bool scopeLocal = false)
    {
      ArgumentUtility.CheckForEmptyGuid(taskId, nameof (taskId));
      ArgumentUtility.CheckStringForNullOrEmpty(versionString, nameof (versionString));
      TaskVersion version = new TaskVersion(versionString);
      if (this.ZipFileRequested())
      {
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        CompressionType compressionType;
        Stream taskDefinition = this.TaskService.GetTaskDefinition(this.TfsRequestContext, taskId, version, out compressionType);
        response.Content = (HttpContent) new StreamContent(taskDefinition);
        if (compressionType == CompressionType.GZip)
          response.Content.Headers.ContentEncoding.Add("gzip");
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
          FileName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.zip", (object) taskId, (object) versionString)
        };
        return response;
      }
      IList<TaskDefinition> taskDefinitions = this.TaskService.GetTaskDefinitions(this.TfsRequestContext, new Guid?(taskId), version, visibility, scopeLocal, true);
      if (taskDefinitions.Count == 0)
        throw new TaskDefinitionNotFoundException(Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.TaskDefinitionNotFound((object) taskId, (object) version));
      TaskDefinition taskDefinition1 = taskDefinitions.Count <= 1 ? taskDefinitions.First<TaskDefinition>() : throw new TaskDefinitionNotFoundException(Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.TaskDefinitionDuplicateFound((object) taskId, (object) version));
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      taskDefinition1.IconUrl = service.GetResourceUri(this.TfsRequestContext, "distributedtask", TaskResourceIds.TaskIcons, (object) new
      {
        taskId = taskDefinition1.Id,
        versionString = taskDefinition1.Version.ToString()
      }).AbsoluteUri;
      return this.Request.CreateResponse<TaskDefinition>(HttpStatusCode.OK, taskDefinition1);
    }

    [HttpPut]
    [ClientIgnore]
    public HttpResponseMessage CreateTaskDefinition([FromBody] TaskDefinition definition) => this.Request.CreateResponse(HttpStatusCode.BadRequest);

    [HttpPut]
    [ClientIgnore]
    public async Task<HttpResponseMessage> UploadTaskDefinition(Guid taskId)
    {
      TaskDefinitions3Controller definitions3Controller = this;
      if (!definitions3Controller.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return definitions3Controller.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpContent content = definitions3Controller.Request.Content;
      if (content == null)
        return definitions3Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "No content");
      if (content.Headers.ContentEncoding.Contains("gzip"))
        return definitions3Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Gzip not supported");
      long num1 = 0;
      long num2 = 0;
      long? nullable;
      if (content.Headers.ContentRange != null)
      {
        nullable = content.Headers.ContentRange.From;
        if (nullable.HasValue)
        {
          nullable = content.Headers.ContentRange.Length;
          if (nullable.HasValue)
          {
            nullable = content.Headers.ContentRange.From;
            num1 = nullable.Value;
            nullable = content.Headers.ContentRange.Length;
            num2 = nullable.Value;
            goto label_12;
          }
        }
      }
      nullable = content.Headers.ContentLength;
      long num3 = 0;
      if (nullable.GetValueOrDefault() > num3 & nullable.HasValue)
        return definitions3Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "ContentRange header incomplete");
label_12:
      if (num1 == 0L)
      {
        long num4 = num2;
        nullable = content.Headers.ContentLength;
        long valueOrDefault = nullable.GetValueOrDefault();
        if (num4 == valueOrDefault)
        {
          SegmentedMemoryStream packageStream = new SegmentedMemoryStream();
          await content.CopyToAsync((Stream) packageStream);
          using (TaskContribution taskPackage = TaskContribution.Create(definitions3Controller.TfsRequestContext, packageStream))
          {
            if (taskPackage?.Definition == null)
              return definitions3Controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.TaskDefinitionCouldNotBeDeserialized());
            if (taskPackage.Definition.Id != taskId)
              return definitions3Controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, Microsoft.TeamFoundation.DistributedTask.Server.TaskResources.TaskDefinitionIdRouteParameterMismatch((object) taskId, (object) taskPackage.Definition.Id));
            int num5 = await definitions3Controller.TaskService.UploadTaskDefinitionAsync(definitions3Controller.TfsRequestContext, taskPackage) ? 1 : 0;
            DistributedTaskEventSource.Log.PublishTaskDefinitionInstallHistory(definitions3Controller.TfsRequestContext.ServiceHost.InstanceId, TaskDefinitionInstallType.UploadTaskDefinitionHttpApi, taskPackage.Definition.ContributionIdentifier, taskPackage.Definition.Id, taskPackage.Definition.Name, taskPackage.Definition.Version.ToString());
          }
          return definitions3Controller.Request.CreateResponse(HttpStatusCode.Created);
        }
      }
      return definitions3Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Only accepting complete uploads.");
    }
  }
}
