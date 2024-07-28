// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportServiceController
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.PermissionsReport.Client;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName("PermissionsReport", "PermissionsReport", 1)]
  public class PermissionsReportServiceController : TfsApiController
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
        typeof (PermissionsReportException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PermissionsReportNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PermissionsReportRequestInvalidException),
        HttpStatusCode.BadRequest
      }
    };

    public override string ActivityLogArea => "PermissionsReport";

    public override string TraceArea => "PermissionsReport";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PermissionsReportServiceController.s_httpExceptions;

    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport> GetPermissionsReports()
    {
      this.EnsureFeatureIsEnabled();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.TfsRequestContext.GetService<IPermissionsReportService>().GetPermissionsReports(this.TfsRequestContext);
    }

    [HttpGet]
    public Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport GetPermissionsReport(
      Guid id)
    {
      this.EnsureFeatureIsEnabled();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.TfsRequestContext.GetService<IPermissionsReportService>().GetPermissionsReportByReportId(this.TfsRequestContext, id);
    }

    [HttpPost]
    [ClientResponseType(typeof (ReferenceLinks), null, null)]
    public HttpResponseMessage CreatePermissionsReport(
      [FromBody] PermissionsReportRequest permissionsReportRequest)
    {
      this.EnsureFeatureIsEnabled();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReport permissionsReport = this.TfsRequestContext.GetService<IPermissionsReportService>().CreatePermissionsReport(this.TfsRequestContext, permissionsReportRequest);
      Uri resourceUri1 = PermissionsReportServiceController.GetLocationDataProvider(this.TfsRequestContext).GetResourceUri(this.TfsRequestContext, "PermissionsReport", PermissionsReportResourceIds.PermissionsReportResourceLocationId, (object) new
      {
        id = permissionsReport.Id
      });
      Uri resourceUri2 = PermissionsReportServiceController.GetLocationDataProvider(this.TfsRequestContext).GetResourceUri(this.TfsRequestContext, "PermissionsReport", PermissionsReportResourceIds.PermissionsReportDownloadResourceLocationId, (object) new
      {
        id = permissionsReport.Id
      });
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("_link", resourceUri1.AbsoluteUri);
      referenceLinks.AddLink("_downloadLink", resourceUri2.AbsoluteUri);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.Accepted,
        Content = (HttpContent) new ObjectContent<ReferenceLinks>(referenceLinks, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true))
      };
    }

    private void EnsureFeatureIsEnabled()
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.TeamFoundation.PermissionsReport.Enable"))
        throw new NotImplementedException();
    }

    private static ILocationDataProvider GetLocationDataProvider(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationData(requestContext, PermissionsReportResourceIds.ResourceAreaId);
  }
}
