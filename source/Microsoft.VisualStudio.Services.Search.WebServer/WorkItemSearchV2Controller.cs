// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WorkItemSearchV2Controller
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "workItemSearchResults")]
  public class WorkItemSearchV2Controller : WorkItemSearchControllerBase
  {
    public WorkItemSearchV2Controller()
    {
    }

    protected WorkItemSearchV2Controller(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchRequest, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchResponse> workItemSearchQueryForwarder)
      : base(indexMapper, workItemSearchQueryForwarder)
    {
    }

    [HttpPost]
    [ClientLocationId("73B2C9E2-FF9E-4447-8CDA-5F5B21FF7CAE")]
    [ClientExample("POST__WorkItemSearchResults.json", null, null, null)]
    [PublicProjectRequestRestrictions]
    public Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse FetchWorkItemSearchResults(
      Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchRequest request)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080034, "REST-API", "REST-API", nameof (FetchWorkItemSearchResults));
      try
      {
        if (request == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(SearchWebApiResources.NullQueryMessage);
        request.ValidateQuery();
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchResponse response = this.HandlePostWorkItemQueryRequest(this.TfsRequestContext, request.ToOldRequestContract(), this.ProjectInfo);
        Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse responseContract = response != null ? response.ToNewResponseContract(this.TfsRequestContext, WorkItemContract.ServiceFieldNames.Id) : (Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse) null;
        this.PopulateSearchSecuredObjectInResponse(this.TfsRequestContext, (SearchSecuredV2Object) responseContract);
        return responseContract;
      }
      catch (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.SearchException ex)
      {
        throw ex.ConvertLegacyExceptionToCorrectException();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080035, "REST-API", "REST-API", nameof (FetchWorkItemSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
