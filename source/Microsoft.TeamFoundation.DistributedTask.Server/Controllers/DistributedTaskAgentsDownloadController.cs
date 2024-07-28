// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DistributedTaskAgentsDownloadController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientIgnore]
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "downloads")]
  public class DistributedTaskAgentsDownloadController : DistributedTaskApiController
  {
    [HttpGet]
    public HttpResponseMessage DownloadPackage(string package)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(package, nameof (package));
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      string mediaType;
      if (package.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
      {
        mediaType = "application/zip";
      }
      else
      {
        if (!package.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase))
          throw new AgentMediaTypeNotSupportedException(TaskResources.AgentMediaTypeNotSupported((object) package));
        mediaType = "application/gzip";
      }
      response.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          this.ResourceService.WritePackageFile(this.TfsRequestContext, package, stream);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue(mediaType));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(package);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return response;
    }
  }
}
