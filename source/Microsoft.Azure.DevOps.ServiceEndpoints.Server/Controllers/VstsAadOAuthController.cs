// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.VstsAadOAuthController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(5.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "vstsaadoauth")]
  public class VstsAadOAuthController : ServiceEndpointsProjectApiController
  {
    [HttpPost]
    [ClientLocationId("47911d38-53e1-467a-8c32-d871599d5498")]
    public string CreateAadOAuthRequest(
      string tenantId,
      string redirectUri,
      AadLoginPromptOption promptOption = AadLoginPromptOption.SelectAccount,
      string completeCallbackPayload = null,
      bool completeCallbackByAuthCode = false)
    {
      return this.TfsRequestContext.GetService<IVstsAadOAuthService2>().CreateAadOAuthRequest(this.TfsRequestContext, tenantId, redirectUri, promptOption, completeCallbackPayload, completeCallbackByAuthCode);
    }

    [HttpGet]
    [ClientLocationId("47911d38-53e1-467a-8c32-d871599d5498")]
    public string GetVstsAadTenantId() => this.TfsRequestContext.GetService<IVstsAadOAuthService2>().GetVstsAadTenantId(this.TfsRequestContext);
  }
}
