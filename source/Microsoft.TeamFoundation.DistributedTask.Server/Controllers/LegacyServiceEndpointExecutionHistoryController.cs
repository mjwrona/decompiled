// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyServiceEndpointExecutionHistoryController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "executionhistory")]
  public class LegacyServiceEndpointExecutionHistoryController : DistributedTaskProjectApiController
  {
    public const int DefaultTop = 50;
    public const int MaxAllowedTop = 1000;

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpGet]
    [ClientLocationId("3AD71E20-7586-45F9-A6C8-0342E00835AC")]
    public IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord> GetServiceEndpointExecutionRecords(
      Guid endpointId,
      [ClientQueryParameter] int top = 50)
    {
      top = top > 1000 || top < 0 ? 1000 : top;
      return (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>) this.TfsRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpointExecutionRecords(this.TfsRequestContext, this.ProjectId, endpointId, top).Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>) (executionRecord => executionRecord.ToLegacyServiceEndpointExecutionRecord())).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>();
    }

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPost]
    [ClientLocationId("11A45C69-2CCE-4ADE-A361-C9F5A37239EE")]
    public IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord> AddServiceEndpointExecutionRecords(
      Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecordsInput input)
    {
      return (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>) this.TfsRequestContext.GetService<IServiceEndpointService2>().AddServiceEndpointExecutionRecords(this.TfsRequestContext, this.ProjectId, input.ToServiceEndpointExecutionRecordsInput()).Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointExecutionRecord, Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>) (executionRecord => executionRecord.ToLegacyServiceEndpointExecutionRecord())).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.ServiceEndpointExecutionRecord>();
    }
  }
}
