// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportDownloadServiceController
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName("PermissionsReport", "PermissionsReportDownload", 1)]
  public class PermissionsReportDownloadServiceController : TfsApiController
  {
    private const string c_PermissionsReportApis = "Microsoft.TeamFoundation.PermissionsReport.Enable";
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (NotSupportedException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (NotImplementedException),
        HttpStatusCode.NotImplemented
      },
      {
        typeof (UnexpectedHostTypeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AccessCheckException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PermissionsReportNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PermissionsReportDownloadNotAvailableException),
        HttpStatusCode.NotFound
      }
    };

    public override string ActivityLogArea => "PermissionsReport";

    public override string TraceArea => "PermissionsReport";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PermissionsReportDownloadServiceController.s_httpExceptions;

    [HttpGet]
    [ClientResponseType(typeof (Stream), "Download", "application/octet-stream")]
    public HttpResponseMessage Download(Guid id)
    {
      this.EnsureFeatureIsEnabled();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      CompressionType commpressionType;
      Stream reportForDownload = this.TfsRequestContext.GetService<IPermissionsReportService>().GetPermissionsReportForDownload(this.TfsRequestContext, id, out commpressionType);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(reportForDownload);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      response.Content.Headers.ContentEncoding.Add(commpressionType.ToString());
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(string.Format("PermissionsReport_{0}.json", (object) id));
      return response;
    }

    private void EnsureFeatureIsEnabled()
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.TeamFoundation.PermissionsReport.Enable"))
        throw new NotImplementedException();
    }
  }
}
