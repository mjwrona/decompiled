// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskDefinitionsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "tasks")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class TaskDefinitionsController : TaskDefinitionsControllerBase
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
    public async Task<HttpResponseMessage> CreateTaskDefinition([FromBody] TaskDefinition definition)
    {
      TaskDefinitionsController definitionsController = this;
      await ((IInternalDistributedTaskService) definitionsController.TaskService).AddTaskDefinitionAsync(definitionsController.TfsRequestContext, definition, false);
      return definitionsController.Request.CreateResponse(HttpStatusCode.Created);
    }

    [HttpPut]
    public async Task<HttpResponseMessage> UploadTaskDefinition(
      Guid taskId,
      string versionString,
      bool overwrite = false)
    {
      TaskDefinitionsController definitionsController = this;
      HttpContent content = definitionsController.Request.Content;
      if (content == null)
        return definitionsController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "No content");
      if (content.Headers.ContentEncoding.Contains("gzip"))
        return definitionsController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Gzip not supported");
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
            goto label_10;
          }
        }
      }
      nullable = content.Headers.ContentLength;
      long num3 = 0;
      if (nullable.GetValueOrDefault() > num3 & nullable.HasValue)
        return definitionsController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "ContentRange header incomplete");
label_10:
      if (num1 == 0L)
      {
        long num4 = num2;
        nullable = content.Headers.ContentLength;
        long valueOrDefault = nullable.GetValueOrDefault();
        if (num4 == valueOrDefault & nullable.HasValue)
        {
          using (Stream tempfile = (Stream) System.IO.File.Create(definitionsController.GetTempFileName(), 32768, FileOptions.DeleteOnClose))
          {
            await content.CopyToAsync(tempfile);
            Stream iconStream = (Stream) null;
            long iconStreamLength = 0;
            Stream helpStream = (Stream) null;
            long helpStreamLength = 0;
            using (ZipArchive zipArchive = new ZipArchive(tempfile, ZipArchiveMode.Read, true))
            {
              ZipArchiveEntry entry = zipArchive.GetEntry("icon.png");
              if (entry != null)
              {
                iconStreamLength = entry.Length;
                iconStream = entry.Open();
              }
            }
            tempfile.Position = 0L;
            try
            {
              await ((IInternalDistributedTaskService) definitionsController.TaskService).UploadTaskDefinitionAsync(definitionsController.TfsRequestContext.Elevate(), taskId, new TaskVersion(versionString), tempfile, iconStream, iconStreamLength, helpStream, helpStreamLength);
            }
            finally
            {
              iconStream?.Dispose();
              helpStream?.Dispose();
            }
            iconStream = (Stream) null;
            helpStream = (Stream) null;
          }
          return definitionsController.Request.CreateResponse(HttpStatusCode.Created);
        }
      }
      return definitionsController.Request.CreateResponse<string>(HttpStatusCode.BadRequest, "Only accepting complete uploads.");
    }
  }
}
