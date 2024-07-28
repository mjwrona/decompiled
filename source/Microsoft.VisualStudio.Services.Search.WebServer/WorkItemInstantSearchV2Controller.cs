// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.WorkItemInstantSearchV2Controller
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "workItemInstantSearchResults")]
  public class WorkItemInstantSearchV2Controller : WorkItemSearchControllerBase
  {
    public WorkItemInstantSearchV2Controller()
    {
    }

    protected WorkItemInstantSearchV2Controller(
      IIndexMapper indexMapper,
      ISearchQueryForwarder<Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchRequest, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchResponse> workItemSearchQueryForwarder)
      : base(indexMapper, workItemSearchQueryForwarder)
    {
    }

    [HttpPost]
    [ClientInternalUseOnly(true)]
    [ClientInclude(RestClientLanguages.TypeScriptWebPlatform)]
    [ClientLocationId("68B8D283-C022-4704-92C4-79BFD6EA53A9")]
    public Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse FetchWorkItemInstantSearchResults(
      Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchRequest request)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080046, "REST-API", "REST-API", nameof (FetchWorkItemInstantSearchResults));
      try
      {
        if (request == null)
          throw new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions.InvalidQueryException(SearchWebApiResources.NullQueryMessage);
        request.ValidateQuery();
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchRequest oldRequestContract = request.ToOldRequestContract();
        oldRequestContract.IsInstantSearch = true;
        Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchResponse response = this.HandlePostWorkItemQueryRequest(this.TfsRequestContext, oldRequestContract, this.ProjectInfo);
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
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080047, "REST-API", "REST-API", nameof (FetchWorkItemInstantSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
