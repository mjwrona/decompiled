// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.ServiceEndpoints3Controller
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "endpoints", ResourceVersion = 3)]
  public class ServiceEndpoints3Controller : ServiceEndpointsProjectApiController
  {
    [HttpGet]
    [ClientExample("GET__ServiceEndpointDetails.json", "Get service endpoint details", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual ServiceEndpoint GetServiceEndpointDetails(Guid endpointId) => this.TfsRequestContext.GetService<PlatformServiceEndpointService>().GetServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpointId, ServiceEndpointActionFilter.None, (RefreshAuthenticationParameters) null);

    [HttpGet]
    [ClientExample("GET__ServiceEndpoints.json", "Get service endpoints", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public Task<IEnumerable<ServiceEndpoint>> GetServiceEndpoints(
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
      int num1 = includeFailed ? 1 : 0;
      int num2 = includeDetails ? 1 : 0;
      return service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, type1, (IEnumerable<string>) authSchemes1, (IEnumerable<Guid>) endpointIds1, owner1, num1 != 0, num2 != 0, ServiceEndpointActionFilter.None, (IEnumerable<RefreshAuthenticationParameters>) null);
    }

    [HttpGet]
    [ClientExample("Get__ServiceEndpointsByNames.json", "Get service endpoints by names", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public Task<IEnumerable<ServiceEndpoint>> GetServiceEndpointsByNames(
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
      int num1 = includeFailed ? 1 : 0;
      int num2 = includeDetails ? 1 : 0;
      return service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, type1, (IEnumerable<string>) authSchemes1, (IEnumerable<string>) endpointNames1, owner1, num1 != 0, num2 != 0, ServiceEndpointActionFilter.None, (IEnumerable<RefreshAuthenticationParameters>) null);
    }

    [HttpPost]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    public virtual ServiceEndpoint CreateServiceEndpoint(ServiceEndpoint endpoint) => endpoint != null ? this.TfsRequestContext.GetService<PlatformServiceEndpointService>().CreateServiceEndpointForCollection(this.TfsRequestContext, endpoint) : throw new ArgumentNullException(nameof (endpoint));

    [HttpPut]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    [ClientResponseType(typeof (ServiceEndpoint), null, null)]
    public virtual HttpResponseMessage UpdateServiceEndpoint(
      Guid endpointId,
      ServiceEndpoint endpoint,
      string operation)
    {
      if (endpoint == null)
        throw new ArgumentNullException(nameof (endpoint));
      endpoint.Id = endpointId;
      bool completed;
      ServiceEndpoint serviceEndpoint = this.TfsRequestContext.GetService<PlatformServiceEndpointService>().HandleOperationServiceEndpointForCollection(this.TfsRequestContext, endpoint, operation, out completed);
      return completed ? this.Request.CreateResponse<ServiceEndpoint>(HttpStatusCode.OK, serviceEndpoint) : this.Request.CreateResponse<ServiceEndpoint>(HttpStatusCode.Accepted, serviceEndpoint);
    }

    [HttpPatch]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    public void ShareServiceEndpoint(
      Guid endpointId,
      List<ServiceEndpointProjectReference> endpointProjectReferences)
    {
      if (endpointProjectReferences == null)
        throw new ArgumentNullException(nameof (endpointProjectReferences));
      this.TfsRequestContext.GetService<PlatformServiceEndpointService>().ShareServiceEndpoint(this.TfsRequestContext, endpointId, endpointProjectReferences);
    }

    [HttpDelete]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    public virtual void DeleteServiceEndpoint(Guid endpointId, [ClientParameterAsIEnumerable(typeof (string), ',')] string projectIds)
    {
      List<Guid> projectIdsAsGuid = ServiceEndpointHelper.GetProjectIdsAsGuid(projectIds);
      this.TfsRequestContext.GetService<PlatformServiceEndpointService>().DeleteServiceEndpointForCollection(this.TfsRequestContext, endpointId, projectIdsAsGuid);
    }
  }
}
