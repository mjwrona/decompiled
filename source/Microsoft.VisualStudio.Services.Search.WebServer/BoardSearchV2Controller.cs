// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.BoardSearchV2Controller
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "boardSearchResults")]
  public class BoardSearchV2Controller : BoardSearchControllerBase
  {
    public BoardSearchV2Controller()
    {
    }

    protected BoardSearchV2Controller(
      IIndexMapper indexMapper,
      ISearchRequestForwarder<BoardSearchRequest, BoardSearchResponse> boardRequestQueryForwarder)
      : base(indexMapper, boardRequestQueryForwarder)
    {
    }

    [HttpPost]
    [ClientInclude(RestClientLanguages.TypeScriptWebPlatform)]
    [ClientLocationId("4AFF4E05-AB57-43DD-AD8F-661AE3831EAB")]
    [ClientResponseType(typeof (BoardSearchResponse), null, null)]
    public HttpResponseMessage FetchBoardSearchResults(BoardSearchRequest request)
    {
      if (request == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083108, "REST-API", "REST-API", nameof (FetchBoardSearchResults));
      try
      {
        request.ValidateQuery();
        return this.Request.CreateResponse<BoardSearchResponse>(HttpStatusCode.OK, this.HandlePostBoardQueryRequest(this.TfsRequestContext, request, this.ProjectInfo));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083089, "REST-API", "REST-API", nameof (FetchBoardSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
