// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionReportsController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "reports")]
  public class ExtensionReportsController : GalleryController
  {
    public string LayerName => nameof (ExtensionReportsController);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExtensionDailyStatsVersionMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ExtensionDailyStatsNotSupportedException>(HttpStatusCode.MethodNotAllowed);
      exceptionMap.AddStatusCode<ExtensionDailyStatsAccessDeniedException>(HttpStatusCode.Unauthorized);
    }

    [HttpGet]
    [ClientLocationId("79E0C74F-157F-437E-845F-74FBB4121D4C")]
    [ClientResponseType(typeof (Stream), null, null)]
    public HttpResponseMessage GetExtensionReports(
      string publisherName,
      string extensionName,
      [FromUri] int? days = null,
      [FromUri] int? count = null,
      [FromUri] DateTime? afterDate = null)
    {
      HttpResponseMessage httpResponseMessage = (HttpResponseMessage) null;
      HttpResponseMessage extensionReports1 = (HttpResponseMessage) null;
      try
      {
        httpResponseMessage = this.Request.CreateResponse(HttpStatusCode.OK);
        byte[] extensionReports2 = this.TfsRequestContext.GetService<IExtensionReportsService>().GetExtensionReports(this.TfsRequestContext, publisherName, extensionName, days, count, afterDate);
        string str = publisherName + "." + extensionName + "-report.xlsx";
        httpResponseMessage.Content = (HttpContent) new ByteArrayContent(extensionReports2);
        httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
          FileName = str
        };
        extensionReports1 = httpResponseMessage;
        httpResponseMessage = (HttpResponseMessage) null;
      }
      finally
      {
        httpResponseMessage?.Dispose();
      }
      return extensionReports1;
    }
  }
}
