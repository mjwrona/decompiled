// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.ServiceEndpointExecutionHistoryController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "executionhistory")]
  public class ServiceEndpointExecutionHistoryController : ServiceEndpointsProjectApiController
  {
    public const int DefaultTop = 50;
    public const int MaxAllowedTop = 1000;
    public const long DefaultContinuationToken = 0;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<ServiceEndpointExecutionRecord>), null, null)]
    [ClientExample("GET__ServiceEndpointExecutionRecords.json", "Get service endpoint execution record", null, null)]
    [ClientLocationId("10A16738-9299-4CD1-9A81-FD23AD6200D0")]
    public virtual HttpResponseMessage GetServiceEndpointExecutionRecords(
      Guid endpointId,
      [ClientQueryParameter] int top = 50,
      [ClientQueryParameter] long continuationToken = 0)
    {
      top = top > 1000 || top < 0 ? 1000 : top;
      IList<ServiceEndpointExecutionRecord> executionRecords = this.TfsRequestContext.GetService<PlatformServiceEndpointService>().GetServiceEndpointExecutionRecords(this.TfsRequestContext, this.ProjectId, endpointId, top, continuationToken);
      HttpResponseMessage responseMessage = (HttpResponseMessage) null;
      try
      {
        responseMessage = this.Request.CreateResponse<IList<ServiceEndpointExecutionRecord>>(HttpStatusCode.OK, executionRecords);
        if (executionRecords.Count == top && executionRecords[top - 1] != null)
          ServiceEndpointsProjectApiController.SetContinuationToken(responseMessage, executionRecords[top - 1].Data.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        return responseMessage;
      }
      catch (Exception ex)
      {
        responseMessage?.Dispose();
        this.TfsRequestContext.TraceException(34000850, "ServiceEndpoints", "Service", ex);
        throw;
      }
    }

    [HttpPost]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("55B9ED4B-5404-41B1-B9D2-7ED757D02BB0")]
    public virtual IList<ServiceEndpointExecutionRecord> AddServiceEndpointExecutionRecords(
      ServiceEndpointExecutionRecordsInput input)
    {
      return this.TfsRequestContext.GetService<PlatformServiceEndpointService>().AddServiceEndpointExecutionRecords(this.TfsRequestContext, this.ProjectId, input);
    }
  }
}
