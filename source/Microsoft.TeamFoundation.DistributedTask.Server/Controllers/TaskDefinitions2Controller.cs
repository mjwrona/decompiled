// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskDefinitions2Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "tasks")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class TaskDefinitions2Controller : TaskDefinitionsControllerBase
  {
    [HttpGet]
    public virtual HttpResponseMessage GetTaskDefinitions(
      Guid? taskId = null,
      string versionString = null,
      [FromUri] IEnumerable<string> visibility = null,
      [FromUri] bool scopeLocal = false)
    {
      TaskVersion version = (TaskVersion) null;
      if (!string.IsNullOrEmpty(versionString))
        version = new TaskVersion(versionString);
      if (this.ZipFileRequested())
      {
        if (!taskId.HasValue)
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, (Exception) new ArgumentNullException(nameof (taskId)));
        if (version == (TaskVersion) null)
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, (Exception) new ArgumentNullException("taskVersion"));
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        CompressionType compressionType;
        Stream taskDefinition = this.TaskService.GetTaskDefinition(this.TfsRequestContext, taskId.Value, version, out compressionType);
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
      IList<TaskDefinition> taskDefinitions = this.TaskService.GetTaskDefinitions(this.TfsRequestContext, taskId, version, visibility, scopeLocal, true);
      if (taskDefinitions.Count == 0)
      {
        if (version != (TaskVersion) null)
          throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionNotFound((object) taskId, (object) version));
        if (taskId.HasValue)
          throw new TaskDefinitionNotFoundException(TaskResources.TaskDefinitionIdNotFound((object) taskId));
      }
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      foreach (TaskDefinition taskDefinition in (IEnumerable<TaskDefinition>) taskDefinitions)
        taskDefinition.IconUrl = service.GetResourceUri(this.TfsRequestContext, "distributedtask", TaskResourceIds.TaskIcons, (object) new
        {
          taskId = taskDefinition.Id,
          versionString = taskDefinition.Version.ToString()
        }).AbsoluteUri;
      return this.Request.CreateResponse<IList<TaskDefinition>>(HttpStatusCode.OK, taskDefinitions);
    }

    [HttpPut]
    public HttpResponseMessage CreateTaskDefinition([FromBody] TaskDefinition definition) => this.Request.CreateResponse(HttpStatusCode.BadRequest);

    [HttpPut]
    public async Task<HttpResponseMessage> UploadTaskDefinition(Guid taskId)
    {
      TaskDefinitions2Controller definitions2Controller = this;
      if (!definitions2Controller.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return definitions2Controller.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpContent content = definitions2Controller.Request.Content;
      if (content == null)
        return definitions2Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "No content");
      if (content.Headers.ContentEncoding.Contains("gzip"))
        return definitions2Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Gzip not supported");
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
        return definitions2Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "ContentRange header incomplete");
label_12:
      if (num1 == 0L)
      {
        long num4 = num2;
        nullable = content.Headers.ContentLength;
        long valueOrDefault = nullable.GetValueOrDefault();
        if (num4 == valueOrDefault & nullable.HasValue)
        {
          SegmentedMemoryStream packageStream = new SegmentedMemoryStream();
          await content.CopyToAsync((Stream) packageStream);
          using (TaskContribution taskPackage = TaskContribution.Create(definitions2Controller.TfsRequestContext, packageStream))
          {
            if (taskPackage?.Definition == null)
              return definitions2Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Missing task.json");
            if (taskPackage.Definition.Id != taskId)
              return definitions2Controller.Request.CreateErrorResponse(HttpStatusCode.BadRequest, TaskResources.TaskDefinitionIdRouteParameterMismatch((object) taskId, (object) taskPackage.Definition.Id));
            int num5 = await definitions2Controller.TaskService.UploadTaskDefinitionAsync(definitions2Controller.TfsRequestContext, taskPackage) ? 1 : 0;
          }
          return definitions2Controller.Request.CreateResponse(HttpStatusCode.Created);
        }
      }
      return definitions2Controller.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Only accepting complete uploads.");
    }
  }
}
