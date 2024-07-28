// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.ServiceEndpoints4Controller
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
  [ControllerApiVersion(5.3)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "endpoints", ResourceVersion = 4)]
  public class ServiceEndpoints4Controller : ServiceEndpointsProjectApiController
  {
    [HttpGet]
    [ClientExample("GetServiceEndpointDetails.json", "Get service endpoint details", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public virtual ServiceEndpoint GetServiceEndpointDetails(
      Guid endpointId,
      ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None)
    {
      return this.TfsRequestContext.GetService<PlatformServiceEndpointService>().GetServiceEndpoint(this.TfsRequestContext, this.ProjectId, endpointId, actionFilter, (RefreshAuthenticationParameters) null);
    }

    [HttpGet]
    [ClientExample("ListEndpoints.json", "Get service endpoints", null, null)]
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public Task<IEnumerable<ServiceEndpoint>> GetServiceEndpoints(
      [ClientQueryParameter] string type = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string authSchemes = null,
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string endpointIds = null,
      [ClientQueryParameter] string owner = null,
      [ClientQueryParameter] bool includeFailed = false,
      [ClientQueryParameter] bool includeDetails = false,
      [ClientQueryParameter] ServiceEndpointActionFilter actionFilter = ServiceEndpointActionFilter.None)
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
      int actionFilter1 = (int) actionFilter;
      return service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, type1, (IEnumerable<string>) authSchemes1, (IEnumerable<Guid>) endpointIds1, owner1, num1 != 0, num2 != 0, (ServiceEndpointActionFilter) actionFilter1, (IEnumerable<RefreshAuthenticationParameters>) null);
    }

    [HttpGet]
    [ClientExample("GetServiceEndpointsByName.json", "Get service endpoints by names", null, null)]
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
    [ClientLocationId("E85F1C62-ADFC-4B74-B618-11A150FB195E")]
    public Task<IEnumerable<ServiceEndpoint>> GetServiceEndpointsWithRefreshedAuthentication(
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string endpointIds,
      [ClientParameterAsIEnumerable(typeof (RefreshAuthenticationParameters), ',')] IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters)
    {
      PlatformServiceEndpointService service = this.TfsRequestContext.GetService<PlatformServiceEndpointService>();
      List<Guid> endpointIdsAsGuid = ServiceEndpointHelper.GetEndpointIdsAsGuid(endpointIds);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      List<Guid> endpointIds1 = endpointIdsAsGuid;
      IEnumerable<RefreshAuthenticationParameters> refreshAuthenticationParameters1 = refreshAuthenticationParameters;
      return service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, (IEnumerable<Guid>) endpointIds1, refreshAuthenticationParameters1);
    }

    [HttpPost]
    [ClientExample("CreateGenericEndpoint.json", "Create service endpoint", null, null)]
    [ClientExample("CreateAzureRMEndpoint.json", "Create Azure RM service endpoint", null, null)]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    public virtual ServiceEndpoint CreateServiceEndpoint(ServiceEndpoint endpoint) => endpoint != null ? this.TfsRequestContext.GetService<PlatformServiceEndpointService>().CreateServiceEndpointForCollection(this.TfsRequestContext, endpoint) : throw new ArgumentNullException(nameof (endpoint));

    [HttpPut]
    [ClientExample("UpdateServiceEndpoint.json", "Update service endpoint", null, null)]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    [ClientResponseType(typeof (ServiceEndpoint), null, null)]
    public virtual HttpResponseMessage UpdateServiceEndpoint(
      Guid endpointId,
      ServiceEndpoint endpoint,
      string operation = null)
    {
      if (endpoint == null)
        throw new ArgumentNullException(nameof (endpoint));
      endpoint.Id = endpointId;
      bool completed;
      ServiceEndpoint serviceEndpoint = this.TfsRequestContext.GetService<PlatformServiceEndpointService>().HandleOperationServiceEndpointForCollection(this.TfsRequestContext, endpoint, operation, out completed);
      return completed ? this.Request.CreateResponse<ServiceEndpoint>(HttpStatusCode.OK, serviceEndpoint) : this.Request.CreateResponse<ServiceEndpoint>(HttpStatusCode.Accepted, serviceEndpoint);
    }

    [HttpPut]
    [ClientExample("UpdateServiceEndpoints.json", "Update service endpoints", null, null)]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    public virtual IEnumerable<ServiceEndpoint> UpdateServiceEndpoints(
      List<ServiceEndpoint> endpoints)
    {
      return this.TfsRequestContext.GetService<PlatformServiceEndpointService>().UpdateServiceEndpointsForCollection(this.TfsRequestContext, (IEnumerable<ServiceEndpoint>) endpoints);
    }

    [HttpPatch]
    [ClientExample("ShareServiceEndpoint.json", "Share service endpoint", null, null)]
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
    [ClientExample("DeleteServiceEndpoint.json", "Delete service endpoint", null, null)]
    [ClientLocationId("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0")]
    public virtual void DeleteServiceEndpoint(Guid endpointId, [ClientParameterAsIEnumerable(typeof (string), ',')] string projectIds, [ClientQueryParameter] bool deep = true)
    {
      List<Guid> projectIdsAsGuid = ServiceEndpointHelper.GetProjectIdsAsGuid(projectIds);
      this.TfsRequestContext.GetService<PlatformServiceEndpointService>().DeleteServiceEndpointForCollection(this.TfsRequestContext, endpointId, projectIdsAsGuid, deep);
    }
  }
}
