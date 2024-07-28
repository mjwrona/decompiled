// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SettingSearchV2Controller
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Setting;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Search.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "settingSearchResults")]
  public class SettingSearchV2Controller : SettingSearchControllerBase
  {
    public SettingSearchV2Controller()
    {
    }

    protected SettingSearchV2Controller(
      IIndexMapper indexMapper,
      ISearchRequestForwarder<SettingSearchRequest, SettingSearchResponse> settingRequestQueryForwarder)
      : base(indexMapper, settingRequestQueryForwarder)
    {
    }

    [HttpPost]
    [ClientInclude(RestClientLanguages.TypeScriptWebPlatform)]
    [ClientLocationId("67F96d3C-A96D-4AD1-A3EA-31CAD33B1BF1")]
    [ClientResponseType(typeof (SettingSearchResponse), null, null)]
    public HttpResponseMessage FetchSettingSearchResults(SettingSearchRequest request)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Search.Server.Setting.EnableSearchSettings") || this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      if (request == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083132, "REST-API", "REST-API", nameof (FetchSettingSearchResults));
      try
      {
        request.ValidateQuery();
        return this.Request.CreateResponse<SettingSearchResponse>(HttpStatusCode.OK, this.HandlePostSettingQueryRequest(this.TfsRequestContext, request));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083133, "REST-API", "REST-API", nameof (FetchSettingSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
