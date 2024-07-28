// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WikiSearchV2Controller
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System.ComponentModel;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "wikiSearchResults")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WikiSearchV2Controller : WikiSearchControllerBase
  {
    public WikiSearchV2Controller()
    {
    }

    protected WikiSearchV2Controller(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<WikiSearchQuery, WikiQueryResponse> wikiSearchQueryForwarder)
      : base(indexMapper, wikiSearchQueryForwarder)
    {
    }

    [HttpPost]
    [ClientLocationId("E90E7664-7049-4100-9A86-66B161D81080")]
    [ClientExample("POST__WikiSearchResults.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public WikiSearchResponse FetchWikiSearchResults(WikiSearchRequest request)
    {
      if (request == null)
        throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083015, "REST-API", "REST-API", nameof (FetchWikiSearchResults));
      try
      {
        request.ValidateQuery();
        WikiQueryResponse response = this.HandlePostWikiQueryRequest(this.TfsRequestContext, request.ToOldRequestContract(), this.ProjectInfo);
        WikiSearchResponse wikiSearchResponse = new WikiSearchResponse();
        if (response != null)
          response.ToNewResponseContract(wikiSearchResponse);
        this.PopulateSearchSecuredObjectInResponse(this.TfsRequestContext, (SearchSecuredV2Object) wikiSearchResponse);
        return wikiSearchResponse;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083016, "REST-API", "REST-API", nameof (FetchWikiSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
