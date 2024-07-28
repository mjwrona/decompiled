// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeSearchController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "codeQueryResults")]
  [SearchDemandExtension("ms", "vss-code-search")]
  public class CodeSearchController : CodeSearchControllerBase
  {
    public CodeSearchController()
    {
    }

    internal CodeSearchController(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<SearchQuery, CodeQueryResponse> codeSearchQueryForwarder)
      : base(indexMapper, codeSearchQueryForwarder)
    {
    }

    [HttpPost]
    [ClientLocationId("948CA594-7923-4834-B721-7F3875F51E6C")]
    [ClientResponseType(typeof (EntitySearchResponseWithActivityId), null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage PostCodeQuery(SearchQuery query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080000, "REST-API", "REST-API", nameof (PostCodeQuery));
      try
      {
        return this.Request.CreateResponse<CodeQueryResponse>(HttpStatusCode.OK, this.HandlePostCodeQueryRequest(this.TfsRequestContext, query, this.EnableSecurityChecksInQueryPipeline, this.ProjectInfo));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080001, "REST-API", "REST-API", nameof (PostCodeQuery));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
