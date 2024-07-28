// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.TaskDefinitionIconsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "icon")]
  public class TaskDefinitionIconsController : DistributedTaskApiController
  {
    private const string c_DefaultTaskDefinitionIconResource = "Microsoft.TeamFoundation.DistributedTask.Server.Resources.TaskDefinitionDefaultIcon.png";

    [HttpGet]
    [ClientResponseType(typeof (StreamContent), null, null)]
    public HttpResponseMessage GetTaskIcon(Guid taskId, string versionString)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      CompressionType compressionType;
      Stream content = this.TaskService.GetTaskDefinitionIcon(this.TfsRequestContext, taskId, new TaskVersion(versionString), out compressionType);
      if (content == null)
      {
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        foreach (string manifestResourceName in executingAssembly.GetManifestResourceNames())
        {
          if (manifestResourceName.Equals("Microsoft.TeamFoundation.DistributedTask.Server.Resources.TaskDefinitionDefaultIcon.png", StringComparison.OrdinalIgnoreCase))
            content = executingAssembly.GetManifestResourceStream(manifestResourceName);
        }
        if (content == null)
        {
          response.StatusCode = HttpStatusCode.NotFound;
          return response;
        }
      }
      response.Content = (HttpContent) new StreamContent(content);
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
      return response;
    }
  }
}
