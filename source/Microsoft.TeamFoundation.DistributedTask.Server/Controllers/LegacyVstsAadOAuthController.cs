// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyVstsAadOAuthController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "vstsaadoauth")]
  public class LegacyVstsAadOAuthController : DistributedTaskProjectApiController
  {
    [HttpPost]
    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    public string CreateAadOAuthRequest(
      string tenantId,
      string redirectUri,
      AadLoginPromptOption promptOption = AadLoginPromptOption.SelectAccount,
      string completeCallbackPayload = null,
      bool completeCallbackByAuthCode = false)
    {
      return this.TfsRequestContext.GetService<IVstsAadOAuthService2>().CreateAadOAuthRequest(this.TfsRequestContext, tenantId, redirectUri, promptOption.ToAadLoginPromptOption(), completeCallbackPayload, completeCallbackByAuthCode);
    }

    [HttpGet]
    public string GetVstsAadTenantId() => this.TfsRequestContext.GetService<IVstsAadOAuthService2>().GetVstsAadTenantId(this.TfsRequestContext);
  }
}
