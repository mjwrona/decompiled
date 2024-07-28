// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.AdvancedCodeSearchController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "codeAdvancedQueryResults")]
  [SearchDemandExtension("ms", "vss-code-search")]
  public class AdvancedCodeSearchController : CodeSearchControllerBase
  {
    protected AdvancedCodeSearchController()
    {
    }

    [HttpPost]
    [ClientLocationId("B0D4A14A-520F-40D2-A7DA-281A055ADDCC")]
    [ClientResponseType(typeof (EntitySearchResponseWithActivityId), null, null)]
    [PublicProjectRequestRestrictions]
    [MethodInformation(TimeoutSeconds = 100)]
    public HttpResponseMessage PostAdvancedCodeQuery(SearchQuery query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080000, "REST-API", "REST-API", nameof (PostAdvancedCodeQuery));
      try
      {
        return this.Request.CreateResponse<CodeQueryResponse>(HttpStatusCode.OK, this.HandlePostCodeQueryRequest(this.TfsRequestContext, query, this.EnableSecurityChecksInQueryPipeline, this.ProjectInfo));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080001, "REST-API", "REST-API", nameof (PostAdvancedCodeQuery));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    protected override void ValidateQuery(
      EntitySearchQuery query,
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      base.ValidateQuery(query, requestContext, projectInfo);
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableAdvancedCodeQueryCheck") && !QueryHelper.IsAdvancedCodeQuery(query.SearchText))
        throw new InvalidQueryException(SearchWebApiResources.SearchTextIsNotAdvancedMessage);
    }
  }
}
