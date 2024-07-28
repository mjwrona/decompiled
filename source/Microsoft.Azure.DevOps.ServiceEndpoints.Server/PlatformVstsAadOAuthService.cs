// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.PlatformVstsAadOAuthService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public class PlatformVstsAadOAuthService : IVstsAadOAuthService2, IVssFrameworkService
  {
    public string CreateAadOAuthRequest(
      IVssRequestContext requestContext,
      string tenantId,
      string redirectUri,
      AadLoginPromptOption promptOption = AadLoginPromptOption.SelectAccount,
      string completeCallbackPayload = null,
      bool completeCallbackByAuthCode = false)
    {
      return MsalAzureAccessTokenHelper.GetAadOAuthRequestUrl(requestContext, tenantId, redirectUri, (AadAuthUrlUtility.PromptOption) promptOption, completeCallbackPayload, completeCallbackByAuthCode);
    }

    public string GetVstsAadTenantId(IVssRequestContext requestContext) => AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext).ToString();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
