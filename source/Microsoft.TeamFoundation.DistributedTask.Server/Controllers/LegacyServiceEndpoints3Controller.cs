// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyServiceEndpoints3Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "serviceendpoints", ResourceVersion = 2)]
  public class LegacyServiceEndpoints3Controller : LegacyServiceEndpoints2Controller
  {
    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpGet]
    [ClientLocationId("DCA61D2F-3444-410A-B5EC-DB2FC4EFB4C5")]
    public virtual async Task<IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>> GetServiceEndpointsByNames(
      [ClientParameterAsIEnumerable(typeof (string), ',')] string endpointNames,
      [ClientQueryParameter] string type = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string authSchemes = null,
      [ClientQueryParameter] bool includeFailed = false)
    {
      LegacyServiceEndpoints3Controller endpoints3Controller = this;
      IServiceEndpointService2 service = endpoints3Controller.TfsRequestContext.GetService<IServiceEndpointService2>();
      IList<string> stringList1 = ArtifactPropertyKinds.AsPropertyFilters(authSchemes);
      IList<string> stringList2 = ArtifactPropertyKinds.AsPropertyFilters(endpointNames);
      IVssRequestContext tfsRequestContext = endpoints3Controller.TfsRequestContext;
      Guid projectId = endpoints3Controller.ProjectId;
      string type1 = type;
      IList<string> authSchemes1 = stringList1;
      IList<string> endpointNames1 = stringList2;
      int num = includeFailed ? 1 : 0;
      return (await service.QueryServiceEndpointsAsync(tfsRequestContext, projectId, type1, (IEnumerable<string>) authSchemes1, (IEnumerable<string>) endpointNames1, (string) null, num != 0).ConfigureAwait(true)).Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToLegacyServiceEndpoint()));
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPut]
    [ClientLocationId("DCA61D2F-3444-410A-B5EC-DB2FC4EFB4C5")]
    public virtual List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> UpdateServiceEndpoints(
      List<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint> endpoints)
    {
      return this.TfsRequestContext.GetService<IServiceEndpointService2>().UpdateServiceEndpoints(this.TfsRequestContext, this.ProjectId, (IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) endpoints.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint, Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToServiceEndpoint())).ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>()).Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>) (endpoint => endpoint.ToLegacyServiceEndpoint())).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpoint>();
    }
  }
}
