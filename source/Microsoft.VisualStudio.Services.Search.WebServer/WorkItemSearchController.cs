// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WorkItemSearchController
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientIgnore]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "workItemQueryResults")]
  public class WorkItemSearchController : WorkItemSearchControllerBase
  {
    public WorkItemSearchController()
    {
    }

    protected WorkItemSearchController(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<WorkItemSearchRequest, WorkItemSearchResponse> workItemSearchQueryForwarder)
      : base(indexMapper, workItemSearchQueryForwarder)
    {
    }

    [HttpPost]
    [ClientLocationId("B635B830-2E4B-46E2-BA87-030D42C7A6AA")]
    [ClientResponseType(typeof (EntitySearchResponseWithActivityId), null, null)]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage PostWorkItemQuery(WorkItemSearchRequest query)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080010, "REST-API", "REST-API", nameof (PostWorkItemQuery));
      try
      {
        return this.Request.CreateResponse<WorkItemSearchResponse>(HttpStatusCode.OK, this.HandlePostWorkItemQueryRequest(this.TfsRequestContext, query, this.ProjectInfo));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080011, "REST-API", "REST-API", nameof (PostWorkItemQuery));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
