// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.CodeSearchV2Controller
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "codeSearchResults")]
  [SearchDemandExtension("ms", "vss-code-search")]
  public class CodeSearchV2Controller : CodeSearchV2ControllerBase
  {
    public CodeSearchV2Controller()
    {
    }

    internal CodeSearchV2Controller(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<SearchQuery, CodeQueryResponse> codeSearchQueryForwarder)
      : base(indexMapper, codeSearchQueryForwarder)
    {
    }

    [HttpPost]
    [ClientLocationId("E7F29993-5B82-4FCA-9386-F5CFE683D524")]
    [ClientExample("POST__CodeSearchResults.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public CodeSearchResponse FetchCodeSearchResults(CodeSearchRequest request)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080036, "REST-API", "REST-API", nameof (FetchCodeSearchResults));
      try
      {
        if (request != null && request.IncludeSnippet)
          this.TfsRequestContext.Items["includeSnippetInCodeSearchKey"] = (object) true;
        return this.HandleCodeSearchResults(request, this.ProjectInfo);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080037, "REST-API", "REST-API", nameof (FetchCodeSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
