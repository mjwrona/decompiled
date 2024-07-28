// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageAnalyticsController
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.Server.Controllers;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Location.Server;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "projectanalysis", ResourceName = "languagemetrics")]
  public class LanguageAnalyticsController : ProjectAnalysisApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (ProjectLanguageAnalytics), null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetProjectLanguageAnalytics()
    {
      ProjectLanguageAnalytics languageAnalytics = this.ProjectAnalysisService.GetLanguageAnalytics(this.TfsRequestContext, this.ProjectId);
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      languageAnalytics.Url = service.GetResourceUri(this.TfsRequestContext, "projectanalysis", ProjectAnalysisResourceIds.ProjectLanguageAnalyticsResourceId, this.ProjectId, (object) new
      {
      }).AbsoluteUri;
      return languageAnalytics.ResultPhase == ResultPhase.Full ? this.Request.CreateResponse<ProjectLanguageAnalytics>(HttpStatusCode.OK, languageAnalytics) : this.Request.CreateResponse<ProjectLanguageAnalytics>(HttpStatusCode.PartialContent, languageAnalytics);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<RepositoryNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
