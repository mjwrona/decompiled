// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildReportController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Build2.Xaml;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "report", ResourceVersion = 2)]
  public sealed class BuildReportController : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (BuildReportMetadata), null, null, MethodName = "GetBuildReport")]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetBuildReportHtmlContent", MediaType = "text/html")]
    public HttpResponseMessage GetBuildReport(
      int buildId,
      [FromUri(Name = "type")] string reportType = "",
      [FromUri] Microsoft.TeamFoundation.Build.WebApi.DefinitionType? definitionType = null)
    {
      BuildData srvBuildData = (BuildData) null;
      if (((int) definitionType ?? 2) == 2)
        srvBuildData = this.InternalBuildService.GetBuildById(this.TfsRequestContext, buildId, includeDeleted: true);
      Microsoft.TeamFoundation.Build.WebApi.Build build = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      if (srvBuildData == null && ((int) definitionType ?? 1) == 1)
        build = this.TfsRequestContext.GetService<IXamlBuildProvider>().GetBuild(this.TfsRequestContext, this.ProjectInfo, buildId);
      if (build == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      bool flag1 = false;
      bool flag2 = false;
      string str = "";
      foreach (MediaTypeWithQualityHeaderValue qualityHeaderValue in this.Request.Headers.Accept)
      {
        str = str + qualityHeaderValue.MediaType + ";";
        if (qualityHeaderValue.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase))
          flag2 = true;
        else if (qualityHeaderValue.MediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase))
          flag1 = true;
      }
      if (string.IsNullOrEmpty(reportType))
      {
        if (flag2)
        {
          reportType = "Html";
        }
        else
        {
          if (!flag1)
            throw new ReportStreamNotSupportedException(Resources.ReportStreamNotSupported((object) str));
          reportType = "Html";
        }
      }
      IReportGenerator reportGenerator = this.TfsRequestContext.GetService<ITeamFoundationBuildService2>().GetReportGenerator(this.TfsRequestContext, reportType);
      string buildReport = reportGenerator.GetBuildReport(this.TfsRequestContext, srvBuildData == null ? srvBuildData.ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion) : build);
      if (flag2)
        return this.Request.CreateResponse<BuildReportMetadata>(HttpStatusCode.OK, new BuildReportMetadata(buildId, reportType)
        {
          Content = buildReport
        });
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StringContent(buildReport, reportGenerator.Encoding, reportGenerator.MediaType);
      return response;
    }
  }
}
