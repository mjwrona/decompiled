// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomerSupportRequestController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "csr")]
  public class CustomerSupportRequestController : TfsApiController
  {
    public string LayerName => nameof (CustomerSupportRequestController);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<CSRCreateThresholdExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<CSRAuthorIdentityMismatchException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidReCaptchaTokenException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<AnonymousCustomerSupportRequestException>(HttpStatusCode.BadRequest);
    }

    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("8eded385-026a-4c15-b810-b8eb402771f1")]
    public async Task<HttpResponseMessage> CreateSupportRequest(
      [FromBody] CustomerSupportRequest customerSupportRequest)
    {
      CustomerSupportRequestController requestController = this;
      requestController.TfsRequestContext.TraceEnter(12062079, "gallery", requestController.LayerName, nameof (CreateSupportRequest));
      HttpResponseMessage response;
      try
      {
        await requestController.TfsRequestContext.GetService<ICustomerSupportRequestService>().CreateCustomerSupportRequestTicket(requestController.TfsRequestContext, customerSupportRequest);
        response = requestController.Request.CreateResponse(HttpStatusCode.Created);
      }
      finally
      {
        requestController.TfsRequestContext.TraceLeave(12062079, "gallery", requestController.LayerName, nameof (CreateSupportRequest));
      }
      return response;
    }
  }
}
