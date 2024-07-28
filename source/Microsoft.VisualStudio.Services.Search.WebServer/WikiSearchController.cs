// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WikiSearchController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.ComponentModel;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "wikiQueryResults")]
  [ClientInclude(RestClientLanguages.TypeScript)]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WikiSearchController : WikiSearchControllerBase
  {
    public WikiSearchController()
    {
    }

    protected WikiSearchController(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse> wikiSearchQueryForwarder)
      : base(indexMapper, wikiSearchQueryForwarder)
    {
    }

    [HttpPost]
    [ClientLocationId("EC68B2E5-277D-4026-9DA8-BB72DC70D9F8")]
    public WikiQueryResponse CreateWikiQuery(WikiSearchQuery query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080091, "REST-API", "REST-API", nameof (CreateWikiQuery));
      try
      {
        return this.HandlePostWikiQueryRequest(this.TfsRequestContext, query);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080092, "REST-API", "REST-API", nameof (CreateWikiQuery));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
