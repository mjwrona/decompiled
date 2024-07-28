// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.LogsController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Servicing", ResourceName = "Logs")]
  public class LogsController : BaseServicingJobsController
  {
    [HttpGet]
    public HttpResponseMessage Get(Guid jobId)
    {
      this.CheckDiagnosticPermission(this.TfsRequestContext);
      this.CheckHostType();
      TeamFoundationServicingService service = this.TfsRequestContext.GetService<TeamFoundationServicingService>();
      Guid hostId = Guid.Empty;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid jobId1 = jobId;
      List<ServicingJobInfo> servicingJobsInfo = service.GetServicingJobsInfo(tfsRequestContext, jobId1);
      if (servicingJobsInfo.Count > 0)
        hostId = servicingJobsInfo[0].HostId;
      bool flag = false;
      bool result = false;
      foreach (KeyValuePair<string, string> queryNameValuePair in this.Request.GetQueryNameValuePairs())
      {
        if (queryNameValuePair.Key.Equals("format", StringComparison.OrdinalIgnoreCase) && queryNameValuePair.Value.Equals("structured", StringComparison.OrdinalIgnoreCase))
          flag = true;
        else if (string.Equals("WrapArray", queryNameValuePair.Key, StringComparison.OrdinalIgnoreCase))
          bool.TryParse(queryNameValuePair.Value, out result);
      }
      if (flag)
      {
        List<ServicingLogEntry> structuredLog = ServicingLogWriter.GetStructuredLog(this.TfsRequestContext, hostId, jobId);
        HttpResponseMessage response;
        if (result)
          response = this.Request.CreateResponse<JsonArrayWrapper>(HttpStatusCode.OK, new JsonArrayWrapper()
          {
            __wrappedArray = (IEnumerable) structuredLog
          });
        else
          response = this.Request.CreateResponse<List<ServicingLogEntry>>(HttpStatusCode.OK, structuredLog);
        return response;
      }
      HttpResponseMessage response1 = this.Request.CreateResponse();
      response1.Content = (HttpContent) new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, content, context) =>
      {
        using (StreamWriter streamWriter = new StreamWriter(stream))
          ServicingLogWriter.WriteLogToStream(this.TfsRequestContext, hostId, jobId, streamWriter);
      }), "text/plain");
      return response1;
    }
  }
}
