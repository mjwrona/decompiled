// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.DeprecatedServiceEndpointsController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "endpoints")]
  public class DeprecatedServiceEndpointsController : ServiceEndpointsProjectApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> SHttpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UriFormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (DuplicateServiceConnectionException),
        HttpStatusCode.Conflict
      }
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DeprecatedServiceEndpointsController.SHttpExceptions;

    [HttpGet]
    [ClientExample("Get__ServiceEndpointsByNames.json", "Get service endpoints by names", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual Task<IEnumerable<ServiceEndpoint>> GetServiceEndpointsByNames(
      [ClientParameterAsIEnumerable(typeof (string), ',')] string endpointNames,
      [ClientQueryParameter] string type = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string authSchemes = null,
      [ClientQueryParameter] string owner = null,
      [ClientQueryParameter] bool includeFailed = false,
      [ClientQueryParameter] bool includeDetails = false)
    {
      PlatformServiceEndpointService service = this.TfsRequestContext.GetService<PlatformServiceEndpointService>();
      IList<string> stringList1 = ServiceEndpointHelper.AsPropertyFilters(authSchemes);
      IList<string> stringList2 = ServiceEndpointHelper.AsPropertyFilters(endpointNames);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      string type1 = type;
      IList<string> authSchemes1 = stringList1;
      IList<string> endpointNames1 = stringList2;
      string owner1 = owner;
      int num = includeFailed ? 1 : 0;
      return service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, type1, (IEnumerable<string>) authSchemes1, (IEnumerable<string>) endpointNames1, owner1, num != 0, false, ServiceEndpointActionFilter.None, (IEnumerable<RefreshAuthenticationParameters>) null);
    }

    [HttpPut]
    [ClientExample("PUT__UpdateServiceEndpoints.json", "Update service endpoints", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual IEnumerable<ServiceEndpoint> UpdateServiceEndpoints(
      List<ServiceEndpoint> endpoints)
    {
      return this.TfsRequestContext.GetService<PlatformServiceEndpointService>().UpdateServiceEndpoints(this.TfsRequestContext, this.ProjectId, (IEnumerable<ServiceEndpoint>) endpoints);
    }

    [HttpGet]
    [ClientExample("GET__ServiceEndpoints.json", "Get service endpoints", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual Task<IEnumerable<ServiceEndpoint>> GetServiceEndpoints(
      [ClientQueryParameter] string type = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string authSchemes = null,
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string endpointIds = null,
      [ClientQueryParameter] string owner = null,
      [ClientQueryParameter] bool includeFailed = false,
      [ClientQueryParameter] bool includeDetails = false)
    {
      PlatformServiceEndpointService service = this.TfsRequestContext.GetService<PlatformServiceEndpointService>();
      IList<string> stringList = ServiceEndpointHelper.AsPropertyFilters(authSchemes);
      List<Guid> endpointIdsAsGuid = ServiceEndpointHelper.GetEndpointIdsAsGuid(endpointIds);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      string type1 = type;
      IList<string> authSchemes1 = stringList;
      List<Guid> endpointIds1 = endpointIdsAsGuid;
      string owner1 = owner;
      int num = includeFailed ? 1 : 0;
      return service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, type1, (IEnumerable<string>) authSchemes1, (IEnumerable<Guid>) endpointIds1, owner1, num != 0, false, ServiceEndpointActionFilter.None, (IEnumerable<RefreshAuthenticationParameters>) null);
    }

    [HttpGet]
    [ClientExample("GET__ServiceEndpointDetails.json", "Get service endpoint details", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual ServiceEndpoint GetServiceEndpointDetails(Guid endpointId) => this.TfsRequestContext.GetService<PlatformServiceEndpointService>().GetServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpointId, ServiceEndpointActionFilter.None, (RefreshAuthenticationParameters) null);

    [HttpPost]
    [ClientExample("POST__CreateServiceEndpoint.json", "Create a service endpoint", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual ServiceEndpoint CreateServiceEndpoint(ServiceEndpoint endpoint) => this.TfsRequestContext.GetService<PlatformServiceEndpointService>().CreateServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpoint);

    [HttpPut]
    [ClientExample("PUT__UpdateServiceEndpoint.json", "Update a service endpoint", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual ServiceEndpoint UpdateServiceEndpoint(
      Guid endpointId,
      ServiceEndpoint endpoint,
      string operation = null)
    {
      if (endpoint == null)
        throw new ArgumentNullException(nameof (endpoint));
      PlatformServiceEndpointService service = this.TfsRequestContext.GetService<PlatformServiceEndpointService>();
      endpoint.Id = endpointId;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      ServiceEndpoint endpoint1 = endpoint;
      string operation1 = operation;
      return service.UpdateServiceEndpoint(tfsRequestContext, projectId, endpoint1, operation1);
    }

    [HttpDelete]
    [ClientExample("DELETE__ServiceEndpoint.json", "Delete a service endpoint", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual void DeleteServiceEndpoint(Guid endpointId, [ClientQueryParameter] bool deep = true) => this.TfsRequestContext.GetService<PlatformServiceEndpointService>().DeleteServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpointId, deep);

    public override string ActivityLogArea => "ServiceEndpoints";
  }
}
