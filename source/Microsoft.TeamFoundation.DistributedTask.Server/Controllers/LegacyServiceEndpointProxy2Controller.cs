// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyServiceEndpointProxy2Controller
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "serviceendpointproxy", ResourceVersion = 2)]
  public class LegacyServiceEndpointProxy2Controller : LegacyServiceEndpointProxyControllerBase
  {
    [Obsolete("Use ExecuteServiceEndpointRequest API Instead")]
    [HttpPost]
    [ClientLocationId("F956A7DE-D766-43AF-81B1-E9E349245634")]
    public override IList<string> QueryServiceEndpoint(DataSourceBinding binding) => base.QueryServiceEndpoint(binding);

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPost]
    [ClientLocationId("F956A7DE-D766-43AF-81B1-E9E349245634")]
    public override ServiceEndpointRequestResult ExecuteServiceEndpointRequest(
      string endpointId,
      [FromBody] ServiceEndpointRequest serviceEndpointRequest)
    {
      return base.ExecuteServiceEndpointRequest(endpointId, serviceEndpointRequest);
    }
  }
}
