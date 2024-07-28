// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.PackageSearchV2Controller
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  [VersionedApiControllerCustomName(Area = "search", ResourceName = "packageSearchResults")]
  public class PackageSearchV2Controller : PackageSearchControllerBase
  {
    public PackageSearchV2Controller()
    {
    }

    protected PackageSearchV2Controller(
      IIndexMapper indexMapper,
      ISearchRequestForwarder<PackageSearchRequest, PackageSearchResponseContent> packageRequestQueryForwarder)
      : base(indexMapper, packageRequestQueryForwarder)
    {
    }

    [HttpPost]
    [ClientLocationId("F62ADA48-EEDC-4C8E-93F0-DE870E4ECCE0")]
    [ClientExample("POST__PackageSearchResults.json", null, null, null)]
    [ClientResponseType(typeof (PackageSearchResponse), null, null)]
    public HttpResponseMessage FetchPackageSearchResults(PackageSearchRequest request)
    {
      if (request == null)
        throw new InvalidQueryException(SearchWebApiResources.NullQueryMessage);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(this.TfsRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083088, "REST-API", "REST-API", nameof (FetchPackageSearchResults));
      try
      {
        request.ValidateQuery();
        return this.Request.CreateResponse<PackageSearchResponseContent>(HttpStatusCode.OK, this.HandlePostPackageQueryRequest(this.TfsRequestContext, request));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083089, "REST-API", "REST-API", nameof (FetchPackageSearchResults));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }
  }
}
