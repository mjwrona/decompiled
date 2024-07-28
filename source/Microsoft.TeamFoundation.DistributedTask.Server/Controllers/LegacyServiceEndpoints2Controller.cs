// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyServiceEndpoints2Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "serviceendpoints", ResourceVersion = 2)]
  public class LegacyServiceEndpoints2Controller : LegacyServiceEndpointsControllerBase
  {
    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpGet]
    [ClientLocationId("DCA61D2F-3444-410A-B5EC-DB2FC4EFB4C5")]
    public override Task<IEnumerable<ServiceEndpoint>> GetServiceEndpoints(
      [ClientQueryParameter] string type = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string authSchemes = null,
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string endpointIds = null,
      [ClientQueryParameter] bool includeFailed = false)
    {
      return base.GetServiceEndpoints(type, authSchemes, endpointIds, includeFailed);
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpGet]
    [ClientLocationId("DCA61D2F-3444-410A-B5EC-DB2FC4EFB4C5")]
    public override ServiceEndpoint GetServiceEndpointDetails(Guid endpointId) => base.GetServiceEndpointDetails(endpointId);

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPost]
    [ClientLocationId("DCA61D2F-3444-410A-B5EC-DB2FC4EFB4C5")]
    public override ServiceEndpoint CreateServiceEndpoint(ServiceEndpoint endpoint) => base.CreateServiceEndpoint(endpoint);

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPut]
    [ClientLocationId("DCA61D2F-3444-410A-B5EC-DB2FC4EFB4C5")]
    public override ServiceEndpoint UpdateServiceEndpoint(
      Guid endpointId,
      ServiceEndpoint endpoint,
      string operation = null)
    {
      return base.UpdateServiceEndpoint(endpointId, endpoint, operation);
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpDelete]
    [ClientLocationId("DCA61D2F-3444-410A-B5EC-DB2FC4EFB4C5")]
    public override void DeleteServiceEndpoint(Guid endpointId) => base.DeleteServiceEndpoint(endpointId);
  }
}
